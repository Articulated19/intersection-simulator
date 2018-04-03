using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSensors : MonoBehaviour {
    CarController cc;
    float distanceToPrevailingTruck = 100f;

	// Use this for initialization
	void Start () {
        cc = GetComponentInParent<CarController>();
	}
	
	// Update is called once per frame
	void Update () {
        CalcDistanceToPrevailingTruck();
	}

    private void CalcDistanceToPrevailingTruck()
    {
        if (cc.prevailingVehicle)
        {
            Component prevailingTrailer =
                cc.prevailingVehicle.transform.Find("Trailer");

            Vector3 distVector =
                transform.InverseTransformPoint(prevailingTrailer.transform.position);

            distanceToPrevailingTruck = distVector.magnitude;
        }
    }

    public float DistanceToPrevailingTruck() {
        return distanceToPrevailingTruck;
    }
}
