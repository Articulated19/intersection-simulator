using System.Collections;
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
    private bool reachedEndOfPath = false;
    public bool active = true;
    private bool tooCloseToPrevailing = false;
    public bool inDeepIntersection = false;

    public GameObject intersection;
    public GameObject intersectionEntrance;
    public GameObject trafficLightObj;
    public GameObject prevailingVehicle;

    public GameObject onComingCar = null;
    private CarPathFinder carPathFinder;
    private CarEngine carEngine;
    private CarSensors carSensors;
    private CarPathFinder oncomingCarPf;
    private CarController oncomingCarCtrl;
    private TrafficLight trafficLight;


	// Use this for initialization
	void Start () {
        carEngine = GetComponentInChildren<CarEngine>();
        carSensors = GetComponentInChildren<CarSensors>();
        carPathFinder = GetComponentInParent<CarPathFinder>();

        trafficLight = trafficLightObj.GetComponent<TrafficLight>();

        // Events
        CarEventManager.StartListening("JustOutisdeIntersection", HandleJustOutsideIntersection);
        CarEventManager.StartListening("ExitIntersection", HandleExitIntersection);
        CarEventManager.StartListening("EnterIntersection", HandleEnterIntersection);
        CarEventManager.StartListening("EnterDeepIntersection", HandleEnterDeepIntersection);
        CarEventManager.StartListening("PathEndReached", HandlePathEndReached);
	}
	
	// Update is called once per frame
	void Update () {
        CheckOnComingTraffic();

        if(IsAllowedToDrive()) {
            carEngine.Drive();
            CarLog("Im allowed to drive :)");
            CarLog("Reached end of path: " + reachedEndOfPath + ", Too close to prevailing: " + tooCloseToPrevailing + ", Traffic Light blocks me: " + (!trafficLight.allow && justOutsideIntersection) + ", I'm giving way: " + isGivingWay + ", I have priority: " + havePriority);
        } else {
            CarLog("Im not allowed to drive :(");
            CarLog("Reached end of path: " + reachedEndOfPath + ", Too close to prevailing: " + tooCloseToPrevailing + ", Traffic Light blocks me: " + (!trafficLight.allow && justOutsideIntersection)+ ", I'm giving way: " + isGivingWay);

            carEngine.Stop();
        }
        if (oncomingCarCtrl)
        {
            CarLog("My opposite car is " + oncomingCarCtrl.carId);
        }
        CheckPrevailingTruck();
	}

    public void SetOncomingCar(GameObject oncomingCar) {
        this.onComingCar = oncomingCar;
        oncomingCarPf = onComingCar.GetComponent<CarPathFinder>();
        oncomingCarCtrl = onComingCar.GetComponent<CarController>();
    }

    private void CheckPrevailingTruck()
    {
        if (prevailingVehicle)
        {
            if (carSensors.DistanceToPrevailingTruck() < 30)
            {
                tooCloseToPrevailing = true;
            } else {
                tooCloseToPrevailing = false;
            }
        }
    }

	private bool IsAllowedToDrive()
    {
        if (!active) return false;
        if (reachedEndOfPath) return false;
        if (tooCloseToPrevailing) return false;

        // If I have priority or if I'm approaching intersection I can drive
        if (havePriority || !(justOutsideIntersection || inIntersection))
        {
            CarLog("Do I have priority: " + havePriority + " Am I approaching the intersection: " + !(justOutsideIntersection || inIntersection));
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
        if (onComingCar == null) {
            isGivingWay = false;
            return;
        }

        if (carPathFinder.willTurn 
            && !havePriority
            && trafficLight.allow)
        {
            
            double occSndsUntilIntersection = oncomingCarPf.SecondsUntilIntersection();

            // If an oncomming car has less than 8 seconds until it reaches the 
            // intersection

            if (occSndsUntilIntersection < 6)
            {
                // If the car is waiting for me and has lower priority I will drive
                if (isGivingWay && oncomingCarCtrl.isGivingWay 
                    && oncomingCarCtrl.priority < this.priority)
                {
                    CarLog("I have priority, so I will drive");
                    havePriority = true;
                }
                else
                {
                    isGivingWay = true;
                }

            } else {
                // Else we have time to manouver
                CarLog("My oncoming car is more than 6 seconds away!");
                isGivingWay = false;
            }
        }

        if (!carPathFinder.willTurn)
        {
            if (oncomingCarCtrl.inDeepIntersection && !inDeepIntersection) {
                isGivingWay = true;
            } else {
                isGivingWay = false;
            }
        }
    }

    private void HandleExitIntersection(int carId) {
        if (oncomingCarCtrl && oncomingCarCtrl.carId == carId)
        {
            CarLog("We can drive again!");
            isGivingWay = false;
        }
        // it was me who exited
        if (this.carId == carId) {
            inIntersection = false;
            justOutsideIntersection = false;
            inDeepIntersection = false;
        }
    }

    private void HandleJustOutsideIntersection(int carId) {
        justOutsideIntersection |= carId == this.carId;
    }

    private void HandlePathEndReached(int carId) {
        if(carId == this.carId) {
            reachedEndOfPath = true;
            Destroy(transform.gameObject);
        }
    }

    private void HandleEnterIntersection(int carId) {
        if (carId == this.carId)
        {
            inIntersection = true;
            justOutsideIntersection = false;
        }
    }

    private void HandleEnterDeepIntersection(int carId) {
        if (carId == this.carId) {
            inDeepIntersection = true;
        }
    }

    public GameObject getIntersection() {
        return intersection;
    }

    public Component GetCar() {
        return transform.Find("Car");
    }

    public Component GetTrailer()
    {
        return transform.Find("Trailer");
    }

    public void CarLog(string message)
    {
        print(carId + ": " + message);
    }

    public void SetPrevailingVehicle(GameObject prevVehicle) {
        prevailingVehicle = prevVehicle;
    }
}
