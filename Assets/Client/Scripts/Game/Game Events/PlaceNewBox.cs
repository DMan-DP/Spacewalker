using System.Collections;
using System.Collections.Generic;
using Autohand;
using UnityEngine;
using UnityEngine.Events;

public class PlaceNewBox : MonoBehaviour
{
    public PlacePoint placePoint;
    public UnityEvent OnNewBoxInstalled;
    void OnEnable() {
        placePoint.OnPlaceEvent += OnPlace;
    }

    private void OnDisable() {
        placePoint.OnPlaceEvent -= OnPlace;
    }

    private void OnPlace(PlacePoint Point, Grabbable Grabbable)
    {
        if (Grabbable.gameObject.name == "Box")
        {
            OnNewBoxInstalled.Invoke();
        }
    }
}
