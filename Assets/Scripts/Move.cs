//the code was based off the code from the following:
//https://raw.githubusercontent.com/djp3/TerraTower/v0.0.1_candidate/UnityTerraTowerClient/UnityTerraTowerClient/Assets/Scripts/GyroController.cs

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
	public static Move staticAccess;
	public int current = 0;
	public float speed = 2;
	float count = 0.5f;
	private List<GameObject> splines;
	public GameObject posObj;
	public GameObject rotObj;
	public SplineMesh.Spline splineScript;
	Vector3 splineLocation;
	Quaternion splineRotation;
	bool isJumping = false;
	bool isFalling = false;
	float maxY;
	const string k_HORIZONTAL = "Horizontal";
	const string k_VERTICAL = "Vertical";
	Vector3 localDefaultPos;
	GameObject pCamera;

	#region [gyro fields]

	private bool gyroEnabled = true;
	private const float lowPassFilterFactor = 0.2f;

	private readonly Quaternion baseIdentity = Quaternion.Euler(90, 0, 0);
	private readonly Quaternion landscapeRight = Quaternion.Euler(0, 0, 90);
	private readonly Quaternion landscapeLeft = Quaternion.Euler(0, 0, -90);
	private readonly Quaternion upsideDown = Quaternion.Euler(0, 0, 180);

	private Quaternion cameraBase = Quaternion.identity;
	private Quaternion calibration = Quaternion.identity;
	private Quaternion baseOrientation = Quaternion.Euler(90, 0, 0);
	private Quaternion baseOrientationRotationFix = Quaternion.identity;

	private Quaternion referanceRotation = Quaternion.identity;
	private bool debug = true;

	#endregion

	#region [Unity events]
	private void Awake()
	{
		// Initialize the static access so everything has access to the player
		staticAccess = this;
		pCamera = posObj.GetComponentInChildren<Camera>().transform.parent.gameObject;
	}

	protected void Start()
	{
		AttachGyro();
		localDefaultPos = this.transform.localPosition;
	}

	protected void FixedUpdate()
	{
		//Debug.Log(this.transform.localPosition);
		splines = GameObject.Find("HalfpipeManager").GetComponent<SpineSpawnManager>().spawnedSplines;
		splineScript = splines[current].GetComponentInChildren<SplineMesh.Spline>();
		//move along the spline
		if (count >= splineScript.nodes.Count - 1)
		{
			if (current < splines.Count - 1)
			{
				count = 0;
				current++;
			}
		}
		//Debug.Log("Current Pos:"+(splines[current].GetComponentInChildren<SplineMesh.Spline>().GetSample(count)).location);
		Vector3 splineLocationLocal = (splineScript.GetSample(count)).location; //location tracked 2 parents up
		splineLocation = splines[current].transform.GetChild(0).TransformPoint(splineLocationLocal);

		//Dynamic camera
		Vector3 splineLocationCam = new Vector3();
		if (count + .2f >= splineScript.nodes.Count - 1)
			splineLocationCam = splines[current + 1].transform.GetChild(0).TransformPoint(splineScript.GetSample(count + .2f - splineScript.nodes.Count + 1).location);
		else
			splineLocationCam = splines[current].transform.GetChild(0).TransformPoint(splineScript.GetSample(count + .2f).location);
		

		pCamera.transform.rotation = Quaternion.LookRotation(splineLocationCam - pCamera.transform.position + new Vector3(0,5,0), posObj.transform.up);
		/*
        for(int i=0;i< splines[current].GetComponentInChildren<SplineMesh.Spline>().nodes.Count-1;i++)
        {
            splines[current].transform.FindChild("Extruder").transform.TransformPoint(splines[current].GetComponentInChildren<SplineMesh.Spline>().nodes[i].Position);
        }*/
		splineRotation = (splineScript.GetSample(count)).Rotation; //rotation tracked 1 parent up, camera being in same level but above
		posObj.transform.position = splineLocation;
		posObj.transform.rotation = splineRotation;

		//Professor Baker's code
#if UNITY_EDITOR

		//MoveChar(Input.GetAxis(k_HORIZONTAL),Input.GetAxis(k_VERTICAL));
		rotObj.transform.rotation = Quaternion.Slerp(transform.rotation,
			splineRotation * Quaternion.Euler(0, 0, Input.GetAxis(k_HORIZONTAL) * 90), lowPassFilterFactor);
		//jump code
		if (Input.GetKeyDown(KeyCode.W) || Input.GetMouseButtonDown(0))
		{
#else

		rotObj.transform.rotation = Quaternion.Slerp (transform.rotation,
			splineRotation * Quaternion.Euler (0, 0, Input.acceleration.x * 90), lowPassFilterFactor);
        //posObj.transform.position = new Vector3(posObj.transform.position.x,(posObj.transform.position.y+Mathf.Abs(Input.acceleration.y*2)),posObj.transform.position.z);
        //this.transform.localPosition= new Vector3(0,(Mathf.Abs(Input.acceleration.y*2)-6.5f),0);

        //jump code (replace acceleration.y with tap if need be)
        //if (Input.acceleration.y > 0)
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
#endif

			if (!isJumping & !isFalling)
			{
				maxY = 4.0f;
				isJumping = true;
			}

		}
		if (isJumping & transform.localPosition.y + 6.5f >= maxY)
		{
			isFalling = true;
			isJumping = false;
		}
		if (isJumping)
		{
			transform.localPosition += new Vector3(0, (maxY + 6.5f) / 20, 0);
		}
		if (isFalling & transform.localPosition.y <= -6.5)
		{
			isFalling = false;
			transform.localPosition = localDefaultPos;
		}
		if (isFalling)
		{
			transform.localPosition += new Vector3(0, -((maxY + 6.5f) / 20), 0);
		}
		count += .01f;
		

	}

	#endregion

	#region [Public methods]

	/// <summary>
	/// Attaches gyro controller to the transform.
	/// </summary>
	private void AttachGyro()
	{
		gyroEnabled = true;
		ResetBaseOrientation();
		UpdateCalibration(true);
		UpdateCameraBaseRotation(true);
		RecalculateReferenceRotation();
	}

	/// <summary>
	/// Detaches gyro controller from the transform
	/// </summary>
	private void DetachGyro()
	{
		gyroEnabled = false;
	}

	#endregion

	#region [Private methods]

	/// <summary>
	/// Update the gyro calibration.
	/// </summary>
	private void UpdateCalibration(bool onlyHorizontal)
	{
		if (onlyHorizontal)
		{
			var fw = (Input.gyro.attitude) * (-Vector3.forward);
			fw.z = 0;
			if (fw == Vector3.zero)
			{
				calibration = Quaternion.identity;
			}
			else
			{
				calibration = (Quaternion.FromToRotation(baseOrientationRotationFix * Vector3.up, fw));
			}
		}
		else
		{
			calibration = Input.gyro.attitude;
		}
	}

	/// <summary>
	/// Update the camera base rotation.
	/// </summary>
	/// <param name='onlyHorizontal'>
	/// Only y rotation.
	/// </param>
	private void UpdateCameraBaseRotation(bool onlyHorizontal)
	{
		if (onlyHorizontal)
		{
			var fw = transform.forward;
			fw.y = 0;
			if (fw == Vector3.zero)
			{
				cameraBase = Quaternion.identity;
			}
			else
			{
				cameraBase = Quaternion.FromToRotation(Vector3.forward, fw);
			}
		}
		else
		{
			cameraBase = rotObj.transform.rotation;
		}
	}

	/// <summary>
	/// Converts the rotation from right handed to left handed.
	/// </summary>
	/// <returns>
	/// The result rotation.
	/// </returns>
	/// <param name='q'>
	/// The rotation to convert.
	/// </param>
	public static Quaternion ConvertRotation(Quaternion q)
	{

		return new Quaternion(q.x, q.y, -q.z, -q.w);
	}

	/// <summary>
	/// Gets the rot fix for different orientations.
	/// </summary>
	/// <returns>
	/// The rot fix.
	/// </returns>
	private Quaternion GetRotFix()
	{
#if UNITY_3_5
		if (Screen.orientation == ScreenOrientation.Portrait)
			return Quaternion.identity;

		if (Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.Landscape)
			return landscapeLeft;

		if (Screen.orientation == ScreenOrientation.LandscapeRight)
			return landscapeRight;

		if (Screen.orientation == ScreenOrientation.PortraitUpsideDown)
			return upsideDown;
		return Quaternion.identity;
#else
		return Quaternion.identity;
#endif
	}

	/// <summary>
	/// Recalculates reference system.
	/// </summary>
	private void ResetBaseOrientation()
	{
		baseOrientationRotationFix = GetRotFix();
		baseOrientation = baseOrientationRotationFix * baseIdentity;
	}

	/// <summary>
	/// Recalculates reference rotation.
	/// </summary>
	private void RecalculateReferenceRotation()
	{
		referanceRotation = Quaternion.Inverse(baseOrientation) * Quaternion.Inverse(calibration);
	}

	#endregion
}