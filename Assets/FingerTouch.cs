using System.Collections;
using System.Collections.Generic;
using Autohand;
using UnityEngine;

public class FingerTouch : MonoBehaviour
{
    private Hand hand;
    private CollisionTracker collisionTracker;
    private void Awake()
    {
        collisionTracker = GetComponent<CollisionTracker>();
        hand = AutoHandPlayer.Instance.handRight;
    }

    private void OnEnable()
    {
        collisionTracker.OnTriggerFirstEnter += OnTriggerFirstEnter;
        collisionTracker.OnTriggeLastExit += OnTriggerLastExit;
    }

    private void OnDisable()
    {
        collisionTracker.OnTriggerFirstEnter += OnTriggerFirstEnter;
        collisionTracker.OnTriggeLastExit += OnTriggerLastExit;
    }

    private void OnTriggerLastExit(GameObject other)
    {
        if(other.CanGetComponent(out HandTouchEvent touchEvent)) {
            touchEvent.Touch(hand);
        }
    }

    private void OnTriggerFirstEnter(GameObject other)
    {
        if(other.CanGetComponent(out HandTouchEvent touchEvent)) {
            touchEvent.Untouch(hand);
        }
    }
}
