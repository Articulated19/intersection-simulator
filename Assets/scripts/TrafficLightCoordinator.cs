using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.ObjectModel;

public class TrafficLightCoordinator : MonoBehaviour {

    public GameObject trafficLightObject1;
    public GameObject trafficLightObject2;
    public GameObject trafficLightObject3;
    public GameObject trafficLightObject4;

    private TrafficLight tl1;
    private TrafficLight tl2;
    private TrafficLight tl3;
    private TrafficLight tl4;

    private bool[] savedState = new bool[4]; 

    private bool waitingForLastCarsToLeave = false;

    public List<int> carsInTheIntersection = new List<int>();
    
    // Use this for initialization
    void Start () {
        tl1 = trafficLightObject1.GetComponent<TrafficLight>();
        tl2 = trafficLightObject2.GetComponent<TrafficLight>();
        tl3 = trafficLightObject3.GetComponent<TrafficLight>();
        tl4 = trafficLightObject4.GetComponent<TrafficLight>();

        tl2.allow = true;
        tl4.allow = true;

        InvokeRepeating("NextLight", 30.0f, 30.0f);

        CarEventManager.StartListening("EnterIntersection", HandleEnterIntersection);
        CarEventManager.StartListening("ExitIntersection", HandleExitIntersection);
    }

    void NextLight() {

        // if there's still cars in the intersection we wait a bit
        if (carsInTheIntersection.Count > 0) {
            waitingForLastCarsToLeave = true;
            print("Waiting to switch");

            savedState[0] = tl1.allow;
            savedState[1] = tl2.allow;
            savedState[2] = tl3.allow;
            savedState[3] = tl4.allow;

            // Waiting for the last cars to leave
            tl1.allow = false;
            tl2.allow = false;
            tl3.allow = false;
            tl4.allow = false;

        } else {
            tl1.allow = !tl1.allow;
            tl2.allow = !tl2.allow;
            tl3.allow = !tl3.allow;
            tl4.allow = !tl4.allow;

            waitingForLastCarsToLeave = false;
        }
    }

    void HandleEnterIntersection(int carId)
    {
        carsInTheIntersection.Add(carId);
    }

    void HandleExitIntersection(int carId)
    {
        carsInTheIntersection.Remove(carId);

        print("Car " + carId + " just left the intersection. There's now " + carsInTheIntersection.Count + " in the intersection");

        for (int i = 0; i < carsInTheIntersection.Count; i++) {
            print(carsInTheIntersection[i]);
        }

        // If we are waiting for the last cars to leave we switch if the 
        // intersection is empty
        if (carsInTheIntersection.Count == 0 && waitingForLastCarsToLeave) {
            tl1.allow = !savedState[0];
            tl2.allow = !savedState[1];
            tl3.allow = !savedState[2];
            tl4.allow = !savedState[3];
        }
    }
}
