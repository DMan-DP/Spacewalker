using System.Collections;
using Autohand;
using UnityEngine;

namespace Client
{
    public class FingerTriggerHandler : MonoBehaviour
    {
        [SerializeField] private Hand hand;
        private CollisionTracker collisionTracker;
        private bool isCanTriggering = true;
        private bool isTriggeing = false;

        private void Awake()
        {
            collisionTracker = GetComponent<CollisionTracker>();
        }

        private void OnEnable()
        {
            collisionTracker.OnTriggerFirstEnter += OnTriggerFirstEnter;
            collisionTracker.OnTriggeLastExit += OnTriggerLastExit;
            
            //collisionTracker.OnCollisionFirstEnter += OnTriggerFirstEnter;
            //collisionTracker.OnCollisionLastExit += OnTriggerLastExit;
        }

        private void OnDisable()
        {
           collisionTracker.OnTriggerFirstEnter -= OnTriggerFirstEnter;
           collisionTracker.OnTriggeLastExit -= OnTriggerLastExit;
            
           //collisionTracker.OnCollisionFirstEnter -= OnTriggerFirstEnter;
           //collisionTracker.OnCollisionLastExit -= OnTriggerLastExit;
        }

        private void OnTriggerLastExit(GameObject From)
        {
            if (From.CanGetComponent(out HandTouchEvent touchEvent) && isCanTriggering && !isTriggeing)
            {
                touchEvent.Touch(hand);
                isCanTriggering = false;
                isTriggeing = true;
                StartCoroutine(TriggerDelay());
            }
        }

        private void OnTriggerFirstEnter(GameObject From) {
            if(From.CanGetComponent(out HandTouchEvent touchEvent) && isTriggeing)
            {
                isTriggeing = false;
                touchEvent.Untouch(hand);
            }
        }

        private IEnumerator TriggerDelay()
        {
            yield return new WaitForSeconds(0.25f);
            isCanTriggering = true;
        }
    }
}