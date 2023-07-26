using UnityEngine;
using System.Collections;
using System;

//  2017-2023 M.Saravanan. developer.vib@gmail.com
public class GizmoOperations : MonoBehaviour 
{
	// 26 views and their positions

	Vector3 front, back, top, left, right, bottom;
	//backBottom, backLeft, backRight, bottomLeft, bottomRight, frontLeftBottom, frontRightBottom,
	//topBack, topLeft, topRight, topFront, frontLeft, frontRight, frontBottom, topLeftBack, topLeftFront,topFrontRight, topRightBack, bottomRightBack, bottomBackLeft
	
	Quaternion frontRot, leftRot, rightRot, backRot, topRot, bottomRot;

	float yMin = -179f, yMax = 179f;
	public Transform origin;
	public GameObject viewCubeOptions;
	public GameObject myGizmo;// spaceGizmo, spaceGizmoTextX, spaceGizmoTextY, spaceGizmoTextZ;
	public Camera maincam;
	public Transform camRig, camXPivot;
	public Camera gizmoCam;
	public float camDistance, camOrthoSize;
	float currentViewClicked, previousViewClicked; // 1 - horizontal views only - Rotating around Y Axis. 2 - four top corner views 3 - four top edge views 4 - top view 5 - bottom view. 6 - bottom four corner views. 7 - bottom four edge views

	private Vector3 curCamPos, destCamPos;
	Quaternion destCamRot;
	public float transitionDuration;
	private float startTime;

    [SerializeField]
    private GameObject grid;
	
	// Use this for initialization
	void Start () 
	{
        //	camDistance = 75.0f;
        setCameraParameters();
			
		float xx, yy, zz;
		xx = maincam.transform.rotation.x;
		yy = maincam.transform.rotation.y;
		zz = maincam.transform.rotation.z;
		myGizmo.transform.rotation = new Quaternion (-xx,-yy,-zz,1);
		previousViewClicked = 1.0f; // default view is front
		currentViewClicked = 1.0f;

	}
	

    public void setCameraParameters()
    {
        camDistance = Vector3.Distance(Camera.main.transform.position, Vector3.zero);
        Debug.Log("camDistance ++++++++++++++ " + camDistance);
        front = new Vector3(0.0f, 0.0f, -camDistance); frontRot = new Quaternion(0.0f, 0.0f, 0.0f, 1);
        left = new Vector3(-camDistance, 0.0f, 0.0f); leftRot = new Quaternion(0.0f, 90.0f, 0.0f, 1);
        right = new Vector3(camDistance, 0.0f, 0.0f); rightRot = new Quaternion(0.0f, -90.0f, 0.0f, 1);
        back = new Vector3(0.0f, 0.0f, camDistance); backRot = new Quaternion(0.0f, 180.0f, 0.0f, 1);
        top = new Vector3(0.0f, camDistance, 0.0f); topRot = new Quaternion(90.0f, 0.0f, 0.0f, 1);
        bottom = new Vector3(0.0f, -camDistance, 0.0f); bottomRot = new Quaternion(-90.0f, 0.0f, 0.0f, 1);

    }

    // Update is called once per frame
    void Update () 
	{
		if(Input.GetMouseButtonDown (0))		
			VerifyTouch();		
	}



	bool VerifyTouch()
	{
		Ray ray = gizmoCam.ScreenPointToRay(Input.mousePosition);	
		RaycastHit hit ;
		
		//Check if there is a collider attached already, otherwise add one on the fly
		if(GetComponent<Collider>() == null) gameObject.AddComponent(typeof(BoxCollider));
		
		if (Physics.Raycast (ray, out hit)) {			
			Debug.Log ("inssss");

			if (hit.collider.gameObject.name.Equals ("viewCubeOptionsToggle")) {			
				viewCubeOptions.SetActive (true);
			}

            DoCameraInterpolations(hit.collider.gameObject.name);

			return true;
			
		}
		return false;
	}


    public void DoCameraInterpolations(string currentClickedView)
    {

        //Horizontal 8 views. Rotate Around Y Axis Only.
        if (currentClickedView.Equals("Front")) //  || hit.collider.gameObject.name.Contains ("Left") || hit.collider.gameObject.name.Contains ("Front") || hit.collider.gameObject.name.Contains ("Back")
        {			
			setRotatedCamera(currentViewClicked,1f, front, new Quaternion(0f, 0f, 0f, 1f));
        }

        if (currentClickedView.Equals("leftFront1") || currentClickedView.Equals("leftFront2"))
        {            
			setRotatedCamera(currentViewClicked, 1.1f, front + left, new Quaternion(0f, 45f, 0f, 1f));
        }

        if (currentClickedView.Equals("Left")) //  || currentClickedView.Contains ("Left") || currentClickedView.Contains ("Front") || currentClickedView.Contains ("Back")
        {            
			setRotatedCamera(currentViewClicked, 1.2f, left, leftRot);
        }

        if (currentClickedView.Equals("backLeft1") || currentClickedView.Equals("backLeft2")) //  || currentClickedView.Contains ("Left") || hit.collider.gameObject.name.Contains ("Front") || hit.collider.gameObject.name.Contains ("Back")
        {            
			setRotatedCamera(currentViewClicked, 1.3f, back + left, new Quaternion(0f, 135f, 0f, 1f));
        }

        if (currentClickedView.Equals("Back"))
        {            
			setRotatedCamera(currentViewClicked, 1.4f, back, backRot);
        }
        if (currentClickedView.Equals("rightBack1") || currentClickedView.Equals("rightBack2"))
        {
			setRotatedCamera(currentViewClicked, 1.5f, back + right, new Quaternion(0f, 225f, 0f, 1f));
        }

        if (currentClickedView.Equals("Right"))
	    {
			setRotatedCamera(currentViewClicked, 1.6f, right, rightRot);
        }

        if (currentClickedView.Equals("frontRight1") || currentClickedView.Equals("frontRight2"))
        {          

			setRotatedCamera(currentViewClicked, 1.7f, front + right, new Quaternion(0f, 315f, 0f, 1f));
        }


        if (currentClickedView.Equals("Top")) //  || currentClickedView.Contains ("Left") || currentClickedView.Contains ("Front") || currentClickedView.Contains ("Back")
        {            
			setRotatedCamera(currentViewClicked, 4f, top, topRot);				           			

        }

        if (currentClickedView.Equals("Bottom")) //  || currentClickedView.Contains ("Left") || currentClickedView.Contains ("Front") || currentClickedView.Contains ("Back")
        {            
			setRotatedCamera(currentViewClicked, 12f, bottom, bottomRot);
        }


        // Four Top edges Rotates around X Axis -- its taking 180 degrees only -- so we have to set 22.5 degrees each
        if (currentClickedView.Equals("topFront1") || currentClickedView.Equals("topFront2"))
        {            
			setRotatedCamera(currentViewClicked, 5f, front + top, new Quaternion(-22.5f, 0f, 0f, 1f));
        }

        if (currentClickedView.Equals("topLeft1") || currentClickedView.Equals("topLeft2"))
        {            
			setRotatedCamera(currentViewClicked, 6f, left + top, new Quaternion(-22.5f, 0f, 0f, 1f));
        }

        if (currentClickedView.Equals("topBack1") || currentClickedView.Equals("topBack2"))
        {            
			setRotatedCamera(currentViewClicked, 7f, back + top, new Quaternion(22.5f, 0f, 0f, 1f));
        }
        if (currentClickedView.Equals("topRight1") || currentClickedView.Equals("topRight2"))
        {            
			setRotatedCamera(currentViewClicked, 8f, right + top, new Quaternion(22.5f, 0f, 0f, 1f));
        }


        // Four top corners
        if (currentClickedView.Equals("topLeftFront1") || currentClickedView.Equals("topLeftFront2") || currentClickedView.Equals("topLeftFront3") || currentClickedView.Equals("homeView"))
        {
            setCameraParameters();            
			setRotatedCamera(currentViewClicked, 2f, front + left + top, new Quaternion(-22.5f, 45f, 22.5f, 1f));
        }
        
		if (currentClickedView.Equals("topFrontRight1") || currentClickedView.Equals("topFrontRight2") || currentClickedView.Equals("topFrontRight3"))
        {   
			setRotatedCamera(currentViewClicked, 3f, front + right + top, new Quaternion(22.5f, 45f, -22.5f, 1f));
			Debug.Log("Axis Touched TFR");
        }

        if (currentClickedView.Equals("topBackRight1") || currentClickedView.Equals("topBackRight2") || currentClickedView.Equals("topBackRight3"))
        {            
			setRotatedCamera(currentViewClicked, 9f, back + right + top, new Quaternion(-22.5f, 135f, 22.5f, 1f));
			Debug.Log("Axis Touched TBR");
        }

        if (currentClickedView.Equals("topBackLeft1") || currentClickedView.Equals("topBackLeft2") || currentClickedView.Equals("topBackLeft3"))
        {            
			setRotatedCamera(currentViewClicked, 10f, back + left + top, new Quaternion(-22.5f, 135f, -22.5f, 1f));
			Debug.Log("Axis Touched TBL");
        }


        if (currentClickedView.Equals("bottomLeft1") || currentClickedView.Equals("bottomLeft2"))
        {
			setRotatedCamera(currentViewClicked, 11f, left + bottom, new Quaternion(22.5f, 0f, 0f, 1f));
			Debug.Log("Axis Touched BL");

        }

        if (currentClickedView.Equals("bottomFront1") || currentClickedView.Equals("bottomFront2"))
        {            
			setRotatedCamera(currentViewClicked, 13f, front + bottom, new Quaternion(22.5f, 0f, 0f, 1f));
			Debug.Log("Axis Touched DF");

        }
        if (currentClickedView.Equals("bottomRight1") || currentClickedView.Equals("bottomRight2"))
        {            
			setRotatedCamera(currentViewClicked, 14f, right + bottom, new Quaternion(-22.5f, 0f, 0f, 1f));
			Debug.Log("Axis Touched DR");

        }
        if (currentClickedView.Equals("bottomBack1") || currentClickedView.Equals("bottomBack2"))
        {            
			setRotatedCamera(currentViewClicked, 15f, back + bottom, new Quaternion(-22.5f, 0f, 0f, 1f));
			Debug.Log("Axis Touched DB");
        }
        // Four bottom corners
        if (currentClickedView.Equals("bottomLeftFront1") || currentClickedView.Equals("bottomLeftFront2") || currentClickedView.Equals("bottomLeftFront3"))
        {
            setRotatedCamera(currentViewClicked, 16f, front + left + bottom, new Quaternion(22.5f, 45f, 22.5f, 1f));
			Debug.Log("Axis Touched BLF " + previousViewClicked + "," + currentViewClicked);
        }
        if (currentClickedView.Equals("bottomFrontRight1") || currentClickedView.Equals("bottomFrontRight2") || currentClickedView.Equals("bottomFrontRight3"))
        {            
			setRotatedCamera(currentViewClicked, 17f, front + right + bottom, new Quaternion(22.5f, 45f, -22.5f, 1f));
			Debug.Log("Axis Touched DFR");
        }
        if (currentClickedView.Equals("bottomRightBack1") || currentClickedView.Equals("bottomRightBack2") || currentClickedView.Equals("bottomRightBack3"))
        {            
			setRotatedCamera(currentViewClicked, 18f, back + right + bottom, new Quaternion(-22.5f, 135f, 22.5f, 1f));
			Debug.Log("Axis Touched DBR");
        }
        if (currentClickedView.Equals("bottomBackLeft1") || currentClickedView.Equals("bottomBackLeft2") || currentClickedView.Equals("bottomBackLeft3"))
        {
			setRotatedCamera(currentViewClicked, 19f, back + left + bottom, new Quaternion(-22.5f, 135f, -22.5f, 1f));
            Debug.Log("Axis Touched TBL");
        }
    }

    private void setRotatedCamera(float pViewClicked, float cViewClicked, Vector3 dCamPos, Quaternion dCamRot)
    {
		previousViewClicked = pViewClicked;
		currentViewClicked = cViewClicked;
		destCamPos = dCamPos;
		destCamRot = dCamRot;
		StartCoroutine(lerpCamPos(destCamPos));

		maincam.orthographic = true;
		maincam.orthographicSize = camOrthoSize;
	}

    float ClampAngle(float angle, float min, float max) {
		if (angle < -360)
			angle += 360;
		if (angle > 360)
			angle -= 360;
		return Mathf.Clamp(angle,min, max);
	}


	public IEnumerator lerpCamPos(Vector3 targetPos)
	{
		float t = 0.0f, startingX, startingY, startingZ;
		float xRot = 0f, zRot = 0f, yRot = 0.0f, yyRot = 0.0f, xxRot = 0.0f, zzRot = 0f;
		Vector3 startingPos = camRig.transform.position;
		Vector3 origin = new Vector3 (0f, 0f, 0f);

		startingY = myGizmo.transform.eulerAngles.y;
		startingX = myGizmo.transform.eulerAngles.x;
		startingZ = myGizmo.transform.eulerAngles.z;

		if(currentViewClicked == previousViewClicked)
		{
			yield return 0;
		}

		if(currentViewClicked == 4f)
			startingPos = camRig.transform.position;
		else
			startingPos = maincam.transform.position;

		while (t < 1.0f)	
		{
			t += Time.deltaTime * (Time.timeScale/transitionDuration);
							
			Vector3 newrot;
			newrot.x = destCamRot.x;
			newrot.y = destCamRot.y;
			newrot.z = destCamRot.z;

			if (currentViewClicked == 4f)  // top view
			{
				
				if (previousViewClicked == 6f) {	
					maincam.transform.localPosition = Vector3.zero;
					newrot.y = 90f;
				} 
				else if (previousViewClicked == 7f) {
					maincam.transform.localPosition = Vector3.zero;
					newrot.y = 180f;
				} 
				else if (previousViewClicked == 8f) {
					maincam.transform.localPosition = Vector3.zero;
					newrot.y = -90f;
				
				}
				else // if (previousViewClicked == 5f)
				{
					maincam.transform.localRotation = Quaternion.identity;
					maincam.transform.localPosition = Vector3.zero;
					maincam.transform.rotation = Quaternion.identity;

				}

				Debug.Log (newrot.x + ":" + newrot.y);
				destCamRot.y = newrot.y;

				//Actual Camera Rig Transformations
				Quaternion QT = Quaternion.Euler(newrot.x, newrot.y, 0);
				camRig.rotation = Quaternion.Lerp(camRig.rotation, QT, t);
				camRig.position = Vector3.Slerp (startingPos, targetPos, t);

				maincam.transform.LookAt (origin);

				maincam.transform.localEulerAngles = Vector3.zero;
			}
			else 
			{			
				destCamRot.x = newrot.x;
				destCamRot.y = newrot.y;
				destCamRot.z = newrot.z;
					
				maincam.transform.rotation = Quaternion.Slerp (maincam.transform.rotation, destCamRot, t);
				maincam.transform.position = Vector3.Slerp (startingPos, targetPos, t);
				maincam.transform.LookAt (origin);

			}

			if (currentViewClicked >= 1.0f && currentViewClicked <= 1.7f) { // horizontal views.
				if (previousViewClicked == 1) {
					Debug.Log ("solvingProblem");	

					yyRot = Mathf.LerpAngle (startingY, -destCamRot.y, t);
					myGizmo.transform.eulerAngles = new Vector3 (0f, yyRot, 0f); // for the first 8 views - rotating around Y Axis only.


				} else { // if(previousViewClicked == 5 || previousViewClicked == 6 || previousViewClicked == 7 || previousViewClicked == 8)
					Debug.Log ("Pre : -------- " + previousViewClicked + "," + currentViewClicked);
					xxRot = Mathf.LerpAngle (startingX, -destCamRot.x, t);
					yyRot = Mathf.LerpAngle (startingY, -destCamRot.y, t);
					zzRot = Mathf.LerpAngle (startingZ, destCamRot.z, t);
					myGizmo.transform.eulerAngles = new Vector3 (xxRot, yyRot, zzRot); // rotating from edge view to horizontal.
                    Debug.Log("YY ROT ----------- " + yyRot);

				}
			} 
			else if (currentViewClicked == 2) { // 			
				//Debug.Log("leftFrontTop");	
				xxRot = Mathf.LerpAngle (startingX, destCamRot.x, t);
				yyRot = Mathf.LerpAngle (startingY, -destCamRot.y, t);
				zzRot = Mathf.LerpAngle (startingZ, destCamRot.z, t);
				myGizmo.transform.eulerAngles = new Vector3 (xxRot, yyRot, zzRot);
			} 
			else if (currentViewClicked == 3) { // topfrontright
				xxRot = Mathf.LerpAngle (startingX, -destCamRot.x, t);
				yyRot = Mathf.LerpAngle (startingY, destCamRot.y, t);
				zzRot = Mathf.LerpAngle (startingZ, destCamRot.z, t);
				myGizmo.transform.eulerAngles = new Vector3 (xxRot, yyRot, zzRot);
			} 
			else if (currentViewClicked == 9) { // 
				xxRot = Mathf.LerpAngle (startingX, -destCamRot.x, t);
				yyRot = Mathf.LerpAngle (startingY, destCamRot.y, t);
				zzRot = Mathf.LerpAngle (startingZ, -destCamRot.z, t);
				myGizmo.transform.eulerAngles = new Vector3 (xxRot, yyRot, zzRot);
			} 
			else if (currentViewClicked == 10) { // 
				xxRot = Mathf.LerpAngle (startingX, -destCamRot.x, t);
				yyRot = Mathf.LerpAngle (startingY, -destCamRot.y, t);
				zzRot = Mathf.LerpAngle (startingZ, -destCamRot.z, t);
				myGizmo.transform.eulerAngles = new Vector3 (xxRot, yyRot, zzRot);
			} 
			else if (currentViewClicked == 4 && currentViewClicked != previousViewClicked) // top view
			{
				if (previousViewClicked == 1.0f || previousViewClicked == 5 || previousViewClicked == 2 || previousViewClicked == 3 || previousViewClicked == 9 || previousViewClicked == 10) { // pre view is front or topfront or top four corner pieces
					xxRot = Mathf.LerpAngle (startingX, -destCamRot.x, t);
					yyRot = Mathf.LerpAngle (startingY, 0.0f, t);
					zzRot = Mathf.LerpAngle (startingZ, 0.0f, t);
					myGizmo.transform.eulerAngles = new Vector3 (xxRot, yyRot, zzRot);
				} else if (previousViewClicked == 1.2f || previousViewClicked == 6.0f) { // pre view is left or topleft

					xxRot = Mathf.LerpAngle (startingX, myGizmo.transform.eulerAngles.x, t);
					yyRot = Mathf.LerpAngle (startingY, myGizmo.transform.eulerAngles.y, t);
					zzRot = Mathf.LerpAngle (startingZ, destCamRot.x, t);
					myGizmo.transform.eulerAngles = new Vector3 (xxRot, yyRot, zzRot);

				} else if (previousViewClicked == 7.0f) {
					xxRot = Mathf.LerpAngle (startingX, destCamRot.x, t);
					yyRot = Mathf.LerpAngle (startingY, 180.0f, t);
					zzRot = Mathf.LerpAngle (startingZ, 0.0f, t);
					myGizmo.transform.eulerAngles = new Vector3 (xxRot, yyRot, zzRot);				
				} else{
				
					xxRot = Mathf.LerpAngle (startingX, myGizmo.transform.eulerAngles.x, t);
					yyRot = Mathf.LerpAngle (startingY, myGizmo.transform.eulerAngles.y , t);
					zzRot = Mathf.LerpAngle (startingZ, -destCamRot.x, t);
					myGizmo.transform.eulerAngles = new Vector3 (xxRot, yyRot, zzRot);
				}

			} 
			else if (currentViewClicked == 5) // top front 
			{
				if (previousViewClicked == 1.0f) 
				{
					xxRot = Mathf.LerpAngle (startingX, destCamRot.x, t);
					myGizmo.transform.eulerAngles = new Vector3 (xxRot, 0f, 0f);
				} 
				else 
				{
					xxRot = Mathf.LerpAngle (startingX, destCamRot.x, t);
					yyRot = Mathf.LerpAngle (startingY, -destCamRot.y, t);
					zzRot = Mathf.LerpAngle (startingZ, -destCamRot.z, t);
					myGizmo.transform.eulerAngles = new Vector3 (xxRot, yyRot, zzRot);
				}

			}
			else if (currentViewClicked == 6.0f) // top left
			{
				if(previousViewClicked == 1.2f) // pre view is left
				{
					zzRot = Mathf.LerpAngle (startingZ, -destCamRot.x, t);
					myGizmo.transform.eulerAngles = new Vector3 (myGizmo.transform.eulerAngles.x, myGizmo.transform.eulerAngles.y, zzRot);
				}
				else
				{
					zzRot = Mathf.LerpAngle (startingZ, -destCamRot.x, t);
					xxRot = Mathf.LerpAngle (startingX, 0.0f, t);
					yyRot = Mathf.LerpAngle (startingY, -90.0f, t);
					myGizmo.transform.eulerAngles = new Vector3 (xxRot, yyRot,zzRot);
				}
			}
			else if (currentViewClicked == 7) // top back
			{

				if (previousViewClicked == 1.4f) { // pre view is back
					xxRot = Mathf.LerpAngle (startingX, destCamRot.x, t);
					myGizmo.transform.eulerAngles = new Vector3 (xxRot, myGizmo.transform.eulerAngles.y, myGizmo.transform.eulerAngles.z);
				} else {
					xxRot = Mathf.LerpAngle (startingX, destCamRot.x, t);
					yyRot = Mathf.LerpAngle (startingY, 180.0f, t);
					zzRot = Mathf.LerpAngle (startingZ, 0.0f, t);
					myGizmo.transform.eulerAngles = new Vector3 (xxRot, yyRot, zzRot);
				}
			}
			else if (currentViewClicked == 8) // topRight
			{
				if (previousViewClicked == 1.6f) // pre view is right
				{
					zzRot = Mathf.LerpAngle (startingZ, -destCamRot.x, t);
					myGizmo.transform.eulerAngles = new Vector3 (myGizmo.transform.eulerAngles.x, myGizmo.transform.eulerAngles.y, zzRot);
				} else
				{
					zzRot = Mathf.LerpAngle (startingZ, -destCamRot.x, t);
					xxRot = Mathf.LerpAngle (startingX, 0.0f, t);
					yyRot = Mathf.LerpAngle (startingY, 90.0f, t);
					myGizmo.transform.eulerAngles = new Vector3 (xxRot, yyRot,zzRot);

				}
			}
			else if (currentViewClicked == 11) // bottomLeft
			{
				if (previousViewClicked == 1.2f) // pre view is bottom or left
				{
					zzRot = Mathf.LerpAngle (startingZ, -destCamRot.x, t);
					myGizmo.transform.eulerAngles = new Vector3 (myGizmo.transform.eulerAngles.x, myGizmo.transform.eulerAngles.y, zzRot);
				}
				else 
				{					
					zzRot = Mathf.LerpAngle (startingZ, -destCamRot.x, t);
					xxRot = Mathf.LerpAngle (startingX, 0.0f, t);
					yyRot = Mathf.LerpAngle (startingY, -90.0f, t);
					myGizmo.transform.eulerAngles = new Vector3 (xxRot, yyRot,zzRot);
				}

			}
			else if (currentViewClicked == 12) // bottom view
			{
			//	xxRot = Mathf.LerpAngle (startingX, -destCamRot.x, t);
			//	myGizmo.transform.eulerAngles = new Vector3 (xxRot, 0f, 0f);
				if (previousViewClicked == 13 || previousViewClicked == 2 || previousViewClicked == 3 || previousViewClicked == 9 || previousViewClicked == 10) { // pre view is front or bottomfront or top four corner pieces
					Debug.Log("bottom four corners");
					xxRot = Mathf.LerpAngle (startingX, -destCamRot.x, t);
					yyRot = Mathf.LerpAngle (startingY, 0.0f, t);
					zzRot = Mathf.LerpAngle (startingZ, 0.0f, t);
					myGizmo.transform.eulerAngles = new Vector3 (xxRot, yyRot, zzRot);
				} 
				else if (previousViewClicked == 11.0f)
				{ // pre view is bottomleft

					xxRot = Mathf.LerpAngle (startingX, myGizmo.transform.eulerAngles.x, t);
					yyRot = Mathf.LerpAngle (startingY, myGizmo.transform.eulerAngles.y, t);
					zzRot = Mathf.LerpAngle (startingZ, destCamRot.x, t);
					myGizmo.transform.eulerAngles = new Vector3 (xxRot, yyRot, zzRot);

				} 
				else if (previousViewClicked == 14.0f)
				{ // pre view is bottomRight
					Debug.Log("bottom bottomright");

					xxRot = Mathf.LerpAngle (startingX, myGizmo.transform.eulerAngles.x, t);
					yyRot = Mathf.LerpAngle (startingY, myGizmo.transform.eulerAngles.y, t);
					zzRot = Mathf.LerpAngle (startingZ, -destCamRot.x, t);
					myGizmo.transform.eulerAngles = new Vector3 (xxRot, yyRot, zzRot);

				}
				else if (previousViewClicked == 15.0f) // pre view is bottomback
				{
					xxRot = Mathf.LerpAngle (startingX, destCamRot.x, t);
					yyRot = Mathf.LerpAngle (startingY, 180.0f, t);
					zzRot = Mathf.LerpAngle (startingZ, 0.0f, t);
					myGizmo.transform.eulerAngles = new Vector3 (xxRot, yyRot, zzRot);				
				}
				else if(previousViewClicked == 16 || previousViewClicked == 17 || previousViewClicked == 18 || previousViewClicked == 19)
				{
					Debug.Log ("bottom pre view four bottom edge pieces");
					xxRot = Mathf.LerpAngle (startingX, 90.0f, t);
					yyRot = Mathf.LerpAngle (startingY, 0.0f , t);
					zzRot = Mathf.LerpAngle (startingZ, 0.0f, t);
					myGizmo.transform.eulerAngles = new Vector3 (xxRot, yyRot, zzRot);
				}
			}
			else if (currentViewClicked == 13)  // bottomFront
			{
				if (previousViewClicked == 1.0f || previousViewClicked == 12) 
				{
					xxRot = Mathf.LerpAngle (startingX, destCamRot.x, t);
					yyRot = Mathf.LerpAngle (startingY, 0.0f, t);
					zzRot = Mathf.LerpAngle (startingZ, 0.0f, t);
					myGizmo.transform.eulerAngles = new Vector3 (xxRot, yyRot, zzRot);

				} 
				else 
				{
					xxRot = Mathf.LerpAngle (startingX, destCamRot.x, t);
					yyRot = Mathf.LerpAngle (startingY, 0.0f, t);
					zzRot = Mathf.LerpAngle (startingZ, 0.0f, t);
					myGizmo.transform.eulerAngles = new Vector3 (xxRot, yyRot, zzRot);

				}
			}
			else if (currentViewClicked == 14)  // bottomRight
			{
				if (previousViewClicked == 1.6f) { // if pre view is  right
					zzRot = Mathf.LerpAngle (startingZ, -destCamRot.x, t);
					myGizmo.transform.eulerAngles = new Vector3 (myGizmo.transform.eulerAngles.x, myGizmo.transform.eulerAngles.y, zzRot);
				} 
				else 
				{				
					xxRot = Mathf.LerpAngle (startingX, 0.0f ,t);
					yyRot = Mathf.LerpAngle (startingY, 90.0f ,t);
					zzRot = Mathf.LerpAngle (startingZ, -destCamRot.x, t);
					myGizmo.transform.eulerAngles = new Vector3 (xxRot, yyRot, zzRot);

				}
			}
			else if (currentViewClicked == 15) // bottomBack
			{
				if (previousViewClicked == 1.4f) { // pre view is back
					xxRot = Mathf.LerpAngle (startingX, destCamRot.x, t);
					myGizmo.transform.eulerAngles = new Vector3 (xxRot, myGizmo.transform.eulerAngles.y, myGizmo.transform.eulerAngles.z);
				} else {
					xxRot = Mathf.LerpAngle (startingX, destCamRot.x, t);
					yyRot = Mathf.LerpAngle (startingY, 180.0f, t);
					zzRot = Mathf.LerpAngle (startingZ, 0.0f, t);
					myGizmo.transform.eulerAngles = new Vector3 (xxRot, yyRot, zzRot);
				}

			}
			else if (currentViewClicked == 16) { // 			
				//Debug.Log("bottomleftFront");	
				xxRot = Mathf.LerpAngle (startingX, destCamRot.x, t);
				yyRot = Mathf.LerpAngle (startingY, -destCamRot.y, t);
				zzRot = Mathf.LerpAngle (startingZ, -destCamRot.z, t);
				myGizmo.transform.eulerAngles = new Vector3 (xxRot, yyRot, zzRot);
			} 
			else if (currentViewClicked == 17) { // 			
				Debug.Log("bottomFrontRight");	
				xxRot = Mathf.LerpAngle (startingX, destCamRot.x, t);
				yyRot = Mathf.LerpAngle (startingY, destCamRot.y, t);
				zzRot = Mathf.LerpAngle (startingZ, -destCamRot.z, t);
				myGizmo.transform.eulerAngles = new Vector3 (xxRot, yyRot, zzRot);
			}
			else if (currentViewClicked == 18) { // bottomRightBack 
				xxRot = Mathf.LerpAngle (startingX, destCamRot.x, t);
				yyRot = Mathf.LerpAngle (startingY, destCamRot.y, t);
				zzRot = Mathf.LerpAngle (startingZ, destCamRot.z, t);
				myGizmo.transform.eulerAngles = new Vector3 (xxRot, yyRot, zzRot);
			} 
			else if (currentViewClicked == 19) { // bottomBackLeft
				xxRot = Mathf.LerpAngle (startingX, destCamRot.x, t);
				yyRot = Mathf.LerpAngle (startingY, -destCamRot.y, t);
				zzRot = Mathf.LerpAngle (startingZ, destCamRot.z, t);
				myGizmo.transform.eulerAngles = new Vector3 (xxRot, yyRot, zzRot);
			} 

			yield return 0;
		}
	}
}