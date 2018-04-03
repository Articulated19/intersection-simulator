using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntersectionCoordinator : MonoBehaviour {

    public List<GameObject> carObjects;
    private List<CarEngine> cars;

	// Use this for initialization
	void Start () {
        for (int i = 0; i < carObjects.Count; i++)
        {
            cars.Add(carObjects[i].GetComponent<CarEngine>());
        }
	}
	
	void FixedUpdate () {
		
	}
}
