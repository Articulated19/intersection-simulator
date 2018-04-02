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
    
    // Use this for initialization
    void Start () {
        tl1 = trafficLightObject1.GetComponent<TrafficLight>();
        tl2 = trafficLightObject2.GetComponent<TrafficLight>();
        tl3 = trafficLightObject3.GetComponent<TrafficLight>();
        tl4 = trafficLightObject4.GetComponent<TrafficLight>();

        tl2.allow = true;
        tl4.allow = true;

        InvokeRepeating("nextLight", 70.0f, 70.0f);

    }

    void nextLight() {
        tl1.allow = !tl1.allow;
        tl2.allow = !tl2.allow;
        tl3.allow = !tl3.allow;
        tl4.allow = !tl4.allow;
    }

}
