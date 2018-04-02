using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLightCoordinator : MonoBehaviour {

    public List<GameObject> trafficLightsObjects = new List<GameObject>();
    private Queue<TrafficLight> tlQueue = new Queue<TrafficLight>();
    private TrafficLight currentTl;

	// Use this for initialization
	void Start () {
        for (int i = 0; i < trafficLightsObjects.Count; i++) {
            TrafficLight tl = trafficLightsObjects[i].GetComponent<TrafficLight>();
            tlQueue.Enqueue(tl);
        }
        InvokeRepeating("nextLight", 0.0f, 5.0f);

	}

    void nextLight() {
        if (currentTl) {
            currentTl.allow = false;
        }
        TrafficLight tl = tlQueue.Dequeue();
        tl.allow = true;
        currentTl = tl;
        tlQueue.Enqueue(tl);
    }

}
