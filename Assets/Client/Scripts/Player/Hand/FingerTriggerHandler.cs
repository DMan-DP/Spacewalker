using Autohand;
using UnityEngine;

namespace Client
{
    public class FingerTriggerHandler : MonoBehaviour
    {
        [SerializeField] private Hand hand;
        private CollisionTracker collisionTracker;

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
            if(From.CanGetComponent(out HandTouchEvent touchEvent)) touchEvent.Touch(hand);
        }

        private void OnTriggerFirstEnter(GameObject From) {
            if(From.CanGetComponent(out HandTouchEvent touchEvent)) touchEvent.Untouch(hand); 
        }
        
        
    }
}