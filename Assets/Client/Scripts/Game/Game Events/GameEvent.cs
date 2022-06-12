using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Client
{
    public class GameEvent : MonoBehaviour
    {
        public bool isOnceEvent = false;
        public UnityEvent gameEvents;

        private bool isEnabale = true;

        public void InvokeEvent()
        {
            if (isEnabale) gameEvents?.Invoke();
            
            if (isOnceEvent)
            {
                isEnabale = false;
            }
        }
    }
}