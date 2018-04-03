﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour {

    public int carId;
    public string carName;
    public int priority;

    private bool havePriority = false;
    public bool isGivingWay = false;
    public bool justOutsideIntersection = false;
    public bool inIntersection = false;
    private bool allowedToDrive = false;

    public GameObject onComingCar;
    public GameObject intersection;
    public GameObject intersectionEntrance;
    public GameObject trafficLightObj;

    private CarPathFinder carPathFinder;
    private CarEngine carEngine;
    private CarPathFinder oncomingCarPf;
    private CarController oncomingCarCtrl;
    private TrafficLight trafficLight;

	// Use this for initialization
	void Start () {
        carEngine = GetComponentInChildren<CarEngine>();
        carPathFinder = GetComponentInParent<CarPathFinder>();
        oncomingCarPf = onComingCar.GetComponent<CarPathFinder>();
        oncomingCarCtrl = onComingCar.GetComponent<CarController>();
        trafficLight = trafficLightObj.GetComponent<TrafficLight>();

        // Events
        CarEventManager.StartListening("JustOutisdeIntersection", HandleJustOutsideIntersection);
        CarEventManager.StartListening("ExitIntersection", HandleExitIntersection);
	}
	
	// Update is called once per frame
	void Update () {
        if(IsAllowedToDrive()) {
            carEngine.Drive();
        } else {
            carEngine.Stop();
        }
        CheckOnComingTraffic();
	}

	private bool IsAllowedToDrive()
    {
        // If I have priority or if I'm approaching intersection I can drive
        if (havePriority || !justOutsideIntersection)
        {
            return true;

        // If I do not have priority, and I'm at the intersection
        // I can only drive if the traffic light is OK and I'm not giving way
        }
        else if (trafficLight.allow && !isGivingWay)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    private void CheckOnComingTraffic()
    {
        if (onComingCar != null && carPathFinder.willTurn && !havePriority)
        {
            
            double occSndsUntilIntersection = oncomingCarPf.SecondsUntilIntersection();

            // If an oncomming car has less than 6 seconds until it reaches the 
            // intersection

            if (occSndsUntilIntersection < 6)
            {
                // If the car is waiting for me and has lower priority I will drive
                if (isGivingWay && oncomingCarCtrl.isGivingWay 
                    && oncomingCarCtrl.priority < this.priority)
                {
                    print("Car " + carName + ": I have priority! I'll drive!");
                    CarLog("I have priority, so I will drive");
                    havePriority = true;
                }
                else
                {
                    CarLog("I must wait for oncomming traffic");
                    isGivingWay = true;
                }

            }
        }
    }

    private void HandleExitIntersection(int carId) {
        if (oncomingCarCtrl.carId == carId)
        {
            CarLog("We can drive again!");
            isGivingWay = false;
            havePriority = true;
        }
    }

    public void HandleJustOutsideIntersection(int carId) {
        print(carId);
        justOutsideIntersection |= carId == this.carId;
    }

    public GameObject getIntersection() {
        return intersection;
    }


    public void CarLog(string message)
    {
        print(carName + ": " + message);
    }
}
