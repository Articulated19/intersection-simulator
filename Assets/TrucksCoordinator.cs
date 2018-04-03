using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrucksCoordinator : MonoBehaviour {

    public GameObject northCarTemplate;
    public GameObject eastCarTemplate;
    public GameObject southCarTemplate;
    public GameObject westCarTemplate;

    public List<GameObject> northTrucks;
    public List<GameObject> eastTrucks;
    public List<GameObject> southTrucks;
    public List<GameObject> westTrucks;

    public List<GameObject>[] trucksInLane = {
        new List<GameObject>(), // north
        new List<GameObject>(), // east
        new List<GameObject>(), // south
        new List<GameObject>(), // west
    };

	// Use this for initialization
	void Start () {
        InvokeRepeating("SpawnTruck", 5f, 4f);
	}

    void SpawnTruck() {

        GameObject template = null;
        int rnd = Random.Range(0, 4);
        string lane = "";

        switch(rnd) {
            case 0:
                template = northCarTemplate;
                lane = "NorthLane";
                break;
            case 1:
                template = eastCarTemplate;
                lane = "EastLane";
                break;

            case 2:
                template = southCarTemplate;
                lane = "SouthLane";
                break;

            case 3:
                template = westCarTemplate;
                lane = "WestLane";
                break;
        }

        // If we have more than 2 trucks in the lane we skip it
        if (trucksInLane[rnd].Count > 2) return;

        // If there are a truck already in the spawning position we also skip spawn
        if (Physics.OverlapSphere(template.transform.position, 10).Length > 2) {
            return;    
        }

        // print("Truck added to " + lane);
        GameObject newCar = Instantiate(template, template.transform.position, template.transform.rotation);
        newCar.SetActive(true);
        CarController cc = newCar.GetComponent<CarController>();
        cc.active = true;
        cc.carId = Random.Range(0, 231481848);
        cc.priority = Random.Range(0, 1000);
        newCar.transform.parent = GameObject.Find(lane).transform;

        // If there are other trucks in this lane
        // we set the one added before this truck as the prevailing vehicle

        trucksInLane[rnd].Add(newCar);
        int newCarAddedIndex = trucksInLane[rnd].Count - 1;

        if (newCarAddedIndex != 0) {
            cc.SetPrevailingVehicle(trucksInLane[rnd][newCarAddedIndex-1]);
        }

        // If there are cars in the opposite lane 
        // the one in front are oncoming to our new car

        int opositeLane = (rnd + 2) % 4;
        if (trucksInLane[opositeLane].Count > 0) {
            cc.SetOncomingCar(trucksInLane[opositeLane][0]);
        }

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
