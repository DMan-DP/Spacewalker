using System.Collections;
using Autohand;
using UnityEngine;

namespace Client
{
    public class FingerTriggerHandler : MonoBehaviour
    {
        [SerializeField] private Hand hand;
        private bool isCanTriggering = true;
        private bool isTriggeing = false;

        private void OnTriggerEnter(Collider other)
        {
            if (isCanTriggering && !isTriggeing && other.CanGetComponent(out HandTouchEvent touchEvent))
            {
                touchEvent.Touch(hand);
                isCanTriggering = false;
                isTriggeing = true;
                StartCoroutine(TriggerDelay());
            }
        }

        private void OnTriggerStay(Collider other)
        {
            isTriggeing = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if(isTriggeing && other.CanGetComponent(out HandTouchEvent touchEvent))
            {
                isTriggeing = false;
                touchEvent.Untouch(hand);
            }
        }

        private IEnumerator TriggerDelay()
        {
            yield return new WaitForSeconds(0.5f);
            isCanTriggering = true;
        }
    }
}