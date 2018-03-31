using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarEngine : MonoBehaviour {

    public Transform path;
    public List<Transform> nodes;
    private int currentNode = 0;
    public float maxSteerAngle = 45f;
    public float turnSpeed = 5;
    public WheelCollider wheelFL;
    public WheelCollider wheelFR;
    public float maxMotorTorque = 100;
    public float currentSpeed;
    public float maximumSpeed = 30;
    public Vector3 centerOfMass;
    private float targetSteerAngle = 0;

	// Use this for initialization
	void Start () {

        GetComponent<Rigidbody>().centerOfMass = centerOfMass;
        nodes = new List<Transform>();

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
        ApplySteer();
        Drive();
        CheckWaypointDistance();
        LerpToSteerAngle();
	}

    private void ApplySteer() {
        Vector3 relativeVector = transform.InverseTransformPoint(nodes[currentNode].position);
        float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;

        targetSteerAngle = newSteer;
    }

    private void Drive() {
        currentSpeed = 2 * Mathf.PI * wheelFL.radius * wheelFL.rpm * 60 / 1000; // current speed

        if (currentSpeed < maximumSpeed) {
            wheelFL.motorTorque = maxMotorTorque;
            wheelFR.motorTorque = maxMotorTorque;    
        } else {
            wheelFL.motorTorque = 0;
            wheelFR.motorTorque = 0;
        }

    }

    private void CheckWaypointDistance() {
        print(Vector3.Distance(transform.position+transform.forward, nodes[currentNode].position));
        if(Vector3.Distance(transform.position, nodes[currentNode].position) < 3) {
            print("Hit detector");
            if (currentNode != nodes.Count - 1) {
                currentNode++;
            }
        }
        print(currentNode);
    }

    private void LerpToSteerAngle() {
        wheelFL.steerAngle = Mathf.Lerp(wheelFL.steerAngle, targetSteerAngle, Time.deltaTime * turnSpeed);
        wheelFR.steerAngle = Mathf.Lerp(wheelFR.steerAngle, targetSteerAngle, Time.deltaTime * turnSpeed);
    }
}
