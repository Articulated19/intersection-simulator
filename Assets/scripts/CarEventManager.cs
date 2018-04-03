using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine;

public class CarEventManager : MonoBehaviour {

    CarController carController;

    private Dictionary<string, CarEvent> eventDictionary;

    private static CarEventManager eventManager;

    public static CarEventManager instance
    {
        get
        {
            if (!eventManager)
            {
                eventManager = FindObjectOfType(typeof(CarEventManager)) as CarEventManager;

                if (!eventManager)
                {
                    Debug.LogError("There needs to be one active CarEventManager script on a GameObject in your scene.");
                }
                else
                {
                    eventManager.Init();
                }
            }

            return eventManager;
        }
    }

    void Init()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<string, CarEvent>();
        }
    }

    public static void StartListening(string eventName, UnityAction<int> listener)
    {
        CarEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new CarEvent();
            thisEvent.AddListener(listener);
            instance.eventDictionary.Add(eventName, thisEvent);
        }
    }

    public static void StopListening(string eventName, UnityAction<int> listener)
    {
        if (eventManager == null) return;
        CarEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public static void TriggerEvent(string eventName, int carId)
    {
        CarEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke(carId);
        }
    }

}


// Our default car event takes an int, the car id preferably
[System.Serializable]
public class CarEvent : UnityEvent<int>   {
    
}
