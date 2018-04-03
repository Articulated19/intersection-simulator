using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntersectionCoordinator : MonoBehaviour {

    List<int> carIds = new List<int>();
	// Use this for initialization
	void Start () {
        CarEventManager.StartListening("EnterIntersection", HandleEnterIntersection);
        CarEventManager.StartListening("ExitIntersection", HandleExitIntersection);
	}
	
    void HandleEnterIntersection(int carId) {
        
    }

    void HandleExitIntersection(int carId) {
        
    }
}
