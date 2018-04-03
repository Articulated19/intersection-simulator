using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeepIntersection : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Car")
        {
            CarController cc = other.gameObject.GetComponentInParent<CarController>();
            CarEventManager.TriggerEvent("EnterDeepIntersection", cc.carId);
        }
    }
}
