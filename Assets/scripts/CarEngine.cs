using System.Collections.Generic;
using UnityEngine;

public class CarEngine : MonoBehaviour {
    public float maxSteerAngle = 45f;
    public float turnSpeed = 5;
    public WheelCollider wheelFL;
    public WheelCollider wheelFR;
    public float maxMotorTorque = 500f;
    public float maxBrakeTorque = 150f;
    public float currentSpeed;
    public float maximumSpeed = 30;
    private float targetSteerAngle = 0;
    private CarPathFinder carPathFinder;
    private CarController carController;

    public Queue<double> secondsAvg = new Queue<double>();

	// Use this for initialization
	void Start () {
        carPathFinder = GetComponentInParent<CarPathFinder>();
        carController = GetComponentInParent<CarController>();
	}

    public void Drive() {
        Gas();
        Steer();
        LerpToSteerAngle();
    }

    public void Stop() {
        Brake();
    }

    // Decice steering angle from the path finding
    private void Steer() {
        Vector3 relativeVector = 
            transform.InverseTransformPoint(
                carPathFinder.nodes[carPathFinder.currentNode].position
            );
        float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;

        targetSteerAngle = newSteer;
    }

    private void Gas() {

        // Release the breaks first
        wheelFL.brakeTorque = 0;
        wheelFR.brakeTorque = 0;
        
        this.currentSpeed = 2 * Mathf.PI * wheelFL.radius * wheelFL.rpm * 60 / 1000; // current speed

        if (currentSpeed < maximumSpeed) {
            wheelFL.motorTorque = maxMotorTorque;
            wheelFR.motorTorque = maxMotorTorque;    
        } else {
            wheelFL.motorTorque = 0;
            wheelFR.motorTorque = 0;
        }

    }

    private void Brake() {
        wheelFL.brakeTorque = maxBrakeTorque;
        wheelFR.brakeTorque = maxBrakeTorque;
    }

    // Some smoothing for the steering
    private void LerpToSteerAngle() {
        wheelFL.steerAngle = Mathf.Lerp(wheelFL.steerAngle, targetSteerAngle, Time.deltaTime * turnSpeed);
        wheelFR.steerAngle = Mathf.Lerp(wheelFR.steerAngle, targetSteerAngle, Time.deltaTime * turnSpeed);
    }

}
