using System.Collections.Generic;
using UnityEngine;

public class CarEngine : MonoBehaviour {

    public string carName;
    public int carId;

    public int priority = 0;
    public Transform[] paths;
    public List<Transform> nodes;
    private int currentNode = 0;
    public float maxSteerAngle = 45f;
    public float turnSpeed = 5;
    public WheelCollider wheelFL;
    public WheelCollider wheelFR;
    public float maxMotorTorque = 500f;
    public float maxBrakeTorque = 150f;
    public float currentSpeed;
    public float maximumSpeed = 30;
    public GameObject intersection;
    public Vector3 centerOfMass;
    private float targetSteerAngle = 0;
    public GameObject trafficLight;
    private bool canDrive = false;
    public bool active = true;
    public double secondsUntilIntersection = 100;
    public Queue<float> speedAvg = new Queue<float>();
    public Queue<double> secondsAvg = new Queue<double>();
    public GameObject onComingCar;
    public float distanceUntilIntersection = 100;
    public bool inIntersection = false;
    public bool isWaiting = false;
    public bool trafficLightOk = false;

    public bool willTurn = false;
    private bool havePriority = false;
    public delegate void EnterIntersection(int id);
    public static event EnterIntersection OnEnterInterSection;

    public delegate void ExitIntersection(int id);
    public static event ExitIntersection OnExitInterSection;

	// Use this for initialization
	void Start () {

        GetComponent<Rigidbody>().centerOfMass = centerOfMass;
        nodes = new List<Transform>();

        int rnd = Random.Range(0, paths.Length);
        Transform path = paths[rnd];

        if(active) {
            print("Car " + carName + " will take path " + rnd);
            if (rnd != 2)
            {
                willTurn = true;
                print("Car " + carName + ": I will turn");
            }    
        }

        Transform[] pathTransforms = path.GetComponentsInChildren<Transform>();

        for (int i = 0; i < pathTransforms.Length; i++)
        {
            if (pathTransforms[i] != path.transform)
            {
                nodes.Add(pathTransforms[i]);
            }
        }
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (!active) return;
        CheckTrafficLight();
        CheckIfCanDrive();
        ApplySteer();
        Drive();
        CalculateDistanceToIntersection();
        CheckWaypointDistance();
        LerpToSteerAngle();
        Brake();
        CheckIntersectionEvent();
        ExpectedArrivalTimeToIntersection();
        CheckOnComingTraffic();

	}

    private void CheckIfCanDrive() {
        if (trafficLightOk && !isWaiting || havePriority) {
            canDrive = true;
        } else {
            if (inIntersection) {
                canDrive = false;    
            } else {
                canDrive = true;
            }
        }
    }

    private void CalculateDistanceToIntersection() {
        // We measure how close we are to the intersection
        Vector3 distanceVector =
            transform.InverseTransformPoint(intersection.transform.position);

        // Save this for other calculations
        distanceUntilIntersection = distanceVector.magnitude;
    }


    private void CheckIntersectionEvent() {
        // Trigger enter event when truck enters intersection
        if (distanceUntilIntersection < 17 && inIntersection == false) {
            print(carName + ": I entered the intersection");
            inIntersection = true;
            if (OnEnterInterSection != null) {
                OnEnterInterSection(carId);
            }
        }

        // Trigger exit event when truck exits intersection
        if (distanceUntilIntersection > 17 && inIntersection == true) {
            inIntersection = false;
            print(carName + ": haha! I existed the interseciton");
            if (OnExitInterSection != null)
            {
                print(carName +": haha! I existed the interseciton");
                OnExitInterSection(carId);
            }
        }
    }

    private void CheckOnComingTraffic() {
        if (onComingCar != null && canDrive && willTurn && !havePriority) {
            CarEngine ce = onComingCar.GetComponent<CarEngine>();
            double sue = ce.ExpectedArrivalTimeToIntersection();

            // An oncomming car i approaching
            if (sue < 6)
            {
                // If the car is waiting for me and has lower priority I will drive
                if (isWaiting && ce.isWaiting && ce.priority < this.priority) {
                    print("Car " + carName + ": I have priority! I'll drive!");
                    havePriority = true;
                } else {
                    print("Car " + carName + ": Oh noes i can't drive!");
                    isWaiting = true;
                    CarEngine.OnExitInterSection += (id) =>
                    {   if (ce.carId == id) {
                            print("We can drive again!");
                            isWaiting = false;
                            havePriority = true;    
                        }
                    };
                }

            }
        }
    }

    private void CheckTrafficLight() {
        TrafficLight tl = trafficLight.GetComponent<TrafficLight>();

        // If we are close and the traffic light is red, then we cannot drive
        if (distanceUntilIntersection < 17 && tl.allow == false) {
            trafficLightOk = false;
        } else {
            trafficLightOk = true;
        }
    }

    private void ApplySteer() {
        Vector3 relativeVector = transform.InverseTransformPoint(nodes[currentNode].position);
        float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;

        targetSteerAngle = newSteer;
    }

    private void Drive() {
        
        this.currentSpeed = 2 * Mathf.PI * wheelFL.radius * wheelFL.rpm * 60 / 1000; // current speed

        if (currentSpeed > 0) {
            if (speedAvg.Count > 10)
            {
                speedAvg.Dequeue();
            }
            speedAvg.Enqueue(currentSpeed);
        }

        if (currentSpeed < maximumSpeed) {
            wheelFL.motorTorque = maxMotorTorque;
            wheelFR.motorTorque = maxMotorTorque;    
        } else {
            wheelFL.motorTorque = 0;
            wheelFR.motorTorque = 0;
        }

    }

    private void Brake() {
        if (!canDrive) {
            wheelFL.brakeTorque = maxBrakeTorque;
            wheelFR.brakeTorque = maxBrakeTorque;
        } else {
            wheelFL.brakeTorque = 0;
            wheelFR.brakeTorque = 0;
        }
    }

    private void CheckWaypointDistance() {
        if(Vector3.Distance(transform.position, nodes[currentNode].position) < 2) {
            if (currentNode != nodes.Count - 1) {
                currentNode++;
            }
        }
    }

    private void LerpToSteerAngle() {
        wheelFL.steerAngle = Mathf.Lerp(wheelFL.steerAngle, targetSteerAngle, Time.deltaTime * turnSpeed);
        wheelFR.steerAngle = Mathf.Lerp(wheelFR.steerAngle, targetSteerAngle, Time.deltaTime * turnSpeed);
    }

    public double ExpectedArrivalTimeToIntersection(){

        // We remove last measurement of seconds until intersection
        // and current speed. This smoothes out the values.
        if (secondsAvg.Count > 10) {
            secondsAvg.Dequeue();
        }


        // We measure how close we are to the intersection
        Vector3 distanceVector =
            transform.InverseTransformPoint(intersection.transform.position);
        
        double vs = ((currentSpeed / 3.6) / distanceVector.magnitude);

        if (vs > 0.00001) {
            secondsUntilIntersection = 1 / vs;
            secondsAvg.Enqueue(secondsUntilIntersection);
        } else {
            secondsUntilIntersection = 100;
        }


        // Because of noise
        IEnumerator<double> ie = secondsAvg.GetEnumerator();
        double tot = 0;
        while(ie.MoveNext()) {
            double snd = ie.Current;
            tot += snd;
        }

        if (secondsAvg.Count > 0) {
            secondsUntilIntersection = tot / secondsAvg.Count;
        }

        return secondsUntilIntersection;
    }

}
