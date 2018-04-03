using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intersection : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Car")
        {
            print("Car in the intersection!");
            CarController cc = other.gameObject.GetComponentInParent<CarController>();
            CarEventManager.TriggerEvent("EnterIntersection", cc.carId);
        }
    }
}
