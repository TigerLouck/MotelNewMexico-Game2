﻿//the code was based off the code from the following:
//https://raw.githubusercontent.com/djp3/TerraTower/v0.0.1_candidate/UnityTerraTowerClient/UnityTerraTowerClient/Assets/Scripts/GyroController.cs

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    //between 0 and -180 (degrees), char starts at -90
    //make it a child of empty gameobject in the spline, put the child a little below the parent, then rotate the parent to rotate the child

    float speed = 2;
    float count = 0f;
    private GameObject spline;
    Vector3 splineLocaton;
    const string k_HORIZONTAL = "Horizontal";
    const string k_VERTICAL = "Vertical";

    #region [Private fields]

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

    protected void Start()
    {
        AttachGyro();
        spline = GameObject.Find("Smoothed");
    }

    protected void Update()
    {
        splineLocaton = (spline.GetComponent<SplineMesh.Spline>().GetSample(count)).location;
        transform.position = splineLocaton;


        if (!gyroEnabled)
            return;
        transform.rotation = Quaternion.Slerp(transform.rotation,
            cameraBase * (ConvertRotation(referanceRotation * Input.gyro.attitude) * GetRotFix()), lowPassFilterFactor);
#if UNITY_EDITOR
        MoveChar(Input.GetAxis(k_HORIZONTAL), Input.GetAxis(k_VERTICAL));
#else
                MoveChar(Input.acceleration.x, Input.acceleration.y);
#endif

        count += .01f;
    }


    void MoveChar(float x, float z)
    {
        Vector3 newPos = transform.position;
        newPos.x += x * speed;
        newPos.z += z * speed;
        /*
    if (newPos.x < -boardSize.x)
        newPos.x = -boardSize.x;
    else if (newPos.x > boardSize.x)
        newPos.x = boardSize.x;

    if (newPos.y < -boardSize.y)
        newPos.y = -boardSize.y;
    else if (newPos.y > boardSize.y)
        newPos.y = boardSize.y;*/

        transform.position = newPos;
    }

    /*protected void OnGUI()
    {
        if (!debug)
            return;

        GUILayout.Label("Orientation: " + Screen.orientation);
        GUILayout.Label("Calibration: " + calibration);
        GUILayout.Label("Camera base: " + cameraBase);
        GUILayout.Label("input.gyro.attitude: " + Input.gyro.attitude);
        GUILayout.Label("transform.rotation: " + transform.rotation);

        if (GUILayout.Button("On/off gyro: " + Input.gyro.enabled, GUILayout.Height(100)))
        {
            Input.gyro.enabled = !Input.gyro.enabled;
        }

        if (GUILayout.Button("On/off gyro controller: " + gyroEnabled, GUILayout.Height(100)))
        {
            if (gyroEnabled)
            {
                DetachGyro();
            }
            else
            {
                AttachGyro();
            }
        }

        if (GUILayout.Button("Update gyro calibration (Horizontal only)", GUILayout.Height(80)))
        {
            UpdateCalibration(true);
        }


        if (GUILayout.Button("Reset base orientation", GUILayout.Height(80)))
        {
            ResetBaseOrientation();
        }

        if (GUILayout.Button("Reset camera rotation", GUILayout.Height(80)))
        {
            transform.rotation = Quaternion.identity;
        }

        if (GUILayout.Button("Update camera base rotation (Horizontal only)", GUILayout.Height(80)))
        {
            UpdateCameraBaseRotation(true);
        }

        if (GUILayout.Button("Close", GUILayout.Height(80)))
        {
            debug = false;
        }
    }*/

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
            cameraBase = transform.rotation;
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
    private static Quaternion ConvertRotation(Quaternion q)
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
