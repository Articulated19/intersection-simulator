using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarEvents : MonoBehaviour {

    CarController carController;

    public delegate void JustOutsideIntersection(int id);
    public static event JustOutsideIntersection OnJustOutsideIntersection;

    public delegate void EnterIntersection(int id);
    public static event EnterIntersection OnEnterInterSection;


}
