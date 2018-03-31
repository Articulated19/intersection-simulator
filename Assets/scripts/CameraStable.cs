using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraStable : MonoBehaviour {

	public GameObject TheTruck;
	public float TruckX;
	public float TruckY;
	public float TruckZ;


	// Update is called once per frame
	void Update () {
		TruckX = TheTruck.transform.eulerAngles.x;
		TruckY = TheTruck.transform.eulerAngles.y;
		TruckZ = TheTruck.transform.eulerAngles.z;

		transform.eulerAngles = new Vector3(TruckX - TruckX, TruckY, TruckZ - TruckZ);
	}
}
