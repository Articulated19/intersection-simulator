using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntersectionExit : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Car") {
            CarController cc = other.gameObject.GetComponentInParent<CarController>();
            CarEventManager.TriggerEvent("ExitIntersection", cc.carId);
        }
    }
}
