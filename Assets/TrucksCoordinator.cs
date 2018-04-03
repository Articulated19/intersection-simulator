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

    private int carsSpawned = 0;

	// Use this for initialization
	void Start () {
        InvokeRepeating("SpawnTruck", 5f, 4f);
        CarEventManager.StartListening("ExitIntersection", HandleExitIntersection);
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

        carsSpawned++;

        // print("Truck added to " + lane);
        GameObject newCar = Instantiate(template, template.transform.position, template.transform.rotation);
        newCar.SetActive(true);
        CarController cc = newCar.GetComponent<CarController>();
        cc.active = true;
        cc.carId = carsSpawned;
        cc.priority = 231481848 - carsSpawned;
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

        // If I'm the first car then I am the oncoming car for my oncoming car
        if (trucksInLane[opositeLane].Count > 0 && trucksInLane[rnd].Count == 1) {
            CarController occ = trucksInLane[opositeLane][0].GetComponent<CarController>();
            occ.SetOncomingCar(newCar);
        }



    }

    // When one car leaves the oncoming car gets the car behind this one
    // as it's new oncoming car.
    void HandleExitIntersection(int carId) {
        UpdateOncomingCar(carId);
    }

    void UpdateOncomingCar(int carId) {
        // We scan each lane
        for (int i = 0; i < trucksInLane.Length; i++)
        {

            List<GameObject> trucks = trucksInLane[i];
            bool foundCar = false;
            bool carsBehind = false;
            int foundInLaneIndex = -1;

            // For each of the trucks in the lane
            for (int j = 0; j < trucks.Count; j++)
            {

                CarController cc = trucks[j].GetComponent<CarController>();

                // If the car is found
                if (cc.carId == carId)
                {

                    // We remove it from the list
                    trucks.Remove(trucks[j]);
                    foundCar = true;

                    // Do the removed car have cars behind it?
                    carsBehind |= trucks.Count > 0;
                    foundInLaneIndex = i;
                }
            }

            // If we removed a car and it had cars behind it
            if (foundCar && carsBehind)
            {

                // We update the removed car oncoming car, oncoming car :)
                int oncomingLaneId = (i + 2) % 4;

                // If there is an oncoming car in the lane opposite to the removed car
                if (trucksInLane[oncomingLaneId].Count > 0)
                {
                    GameObject oncomingCar = trucksInLane[oncomingLaneId][0];
                    CarController occ = oncomingCar.GetComponent<CarController>();
                    occ.SetOncomingCar(trucksInLane[foundInLaneIndex][0]);
                    occ.CarLog("I have a new opposite car!");
                }
            }
        }
    }

    void UpdatePrevailingCar(int carId) {
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
