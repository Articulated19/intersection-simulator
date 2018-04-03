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

    public GameObject intersection;
    public GameObject intersectionEntrance;
    public GameObject trafficLightObj;
    public GameObject prevailingVehicle;

    private GameObject onComingCar = null;
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
        CarEventManager.StartListening("PathEndReached", HandlePathEndReached);
	}
	
	// Update is called once per frame
	void Update () {
        if(IsAllowedToDrive()) {
            carEngine.Drive();
        } else {
            carEngine.Stop();
        }
        CheckOnComingTraffic();
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
            if (carSensors.DistanceToPrevailingTruck() < 20)
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
        if (onComingCar != null 
            && carPathFinder.willTurn 
            && !havePriority
            && trafficLight.allow)
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
                    CarLog("I have priority, so I will drive");
                    havePriority = true;
                }
                else
                {
                    isGivingWay = true;
                }

            }
        }
    }

    private void HandleExitIntersection(int carId) {
        if (oncomingCarCtrl && oncomingCarCtrl.carId == carId)
        {
            CarLog("We can drive again!");
            isGivingWay = false;
            havePriority = true;
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
        print(carName + ": " + message);
    }

    public void SetPrevailingVehicle(GameObject prevVehicle) {
        prevailingVehicle = prevVehicle;
    }
}
