using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarPathFinder : MonoBehaviour {

    public Transform[] paths;
    public List<Transform> nodes;
    private CarEngine carEngine;
    private CarController carController;
    public bool willTurn = false;
    public int currentNode = 0;
    private Queue<double> secondsAvg = new Queue<double>();

	// Use this for initialization
	void Start () {
        carEngine = GetComponentInChildren<CarEngine>();
        carController = GetComponentInParent<CarController>();

        nodes = new List<Transform>();
        int rnd = Random.Range(0, paths.Length);
        Transform path = paths[rnd];

        carController.CarLog("I'll take path" + rnd);
        if (rnd != 2)
        {
            willTurn = true;
            carController.CarLog("I will turn");
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
	void Update () {
        CheckWaypointDistance();
	}

    public double SecondsUntilIntersection()
    {
        // We measure how close we are to the intersection
        Vector3 distanceVector =
            transform.InverseTransformPoint(carController.getIntersection().transform.position);

        double vs = ((carEngine.currentSpeed / 3.6) / distanceVector.magnitude);
        double seconds = 100;

        // Since 
        if (vs > 0.00001)
        {

            // We remove last measurement of seconds until intersection
            // and current speed. This smoothes out the values.
        if (secondsAvg.Count > 10) {
                secondsAvg.Dequeue();
            }
            seconds = 1 / vs;
            secondsAvg.Enqueue(seconds);
        }


        // Because of noise we average out the seconds
        IEnumerator<double> ie = secondsAvg.GetEnumerator();
        double tot = 0;
        while (ie.MoveNext())
        {
            double snd = ie.Current;
            tot += snd;
        }

        if (secondsAvg.Count > 0)
        {
            seconds = tot / secondsAvg.Count;
        }

        return seconds;
    }

    private void CheckWaypointDistance()
    {
        if (Vector3.Distance(transform.position, nodes[currentNode].position) < 2)
        {
            if (currentNode != nodes.Count - 1)
            {
                currentNode++;
            } else {
                
            }
        }
    }
}
