using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtagSpin : MonoBehaviour
{
	Vector3 vectorJerk;
	Vector3 vectorIntertia;
	Vector3 lastControllerJerk;
	Move controller;
	private void Start()
	{
		controller = transform.parent.GetComponent<Move>();
	}
	void Update()
	{
		//Controller rolling
#if UNITY_EDITOR
		 Vector3 controllerJerk = Input.GetAxis("Horizontal") * Vector3.right;
#else
		Vector3 controllerJerk = Move.ConvertRotation(Input.gyro.attitude) * Vector3.forward;
#endif
		Vector3 deltaControllerJerk = controllerJerk - lastControllerJerk;

		//RandomRolling
		//Throwing math at the problem until it looks good
		vectorJerk =  (vectorJerk + Random.onUnitSphere + Vector3.forward).normalized;
		vectorIntertia = (Quaternion.LookRotation((deltaControllerJerk + Vector3.forward * 10).normalized, Vector3.up) * (vectorIntertia + vectorJerk * .001f) + Vector3.forward * .001f).normalized;
		transform.localRotation = 
			Quaternion.LookRotation(vectorIntertia, Vector3.up) * 
			transform.localRotation
			;

		lastControllerJerk = controllerJerk;
	}
}
