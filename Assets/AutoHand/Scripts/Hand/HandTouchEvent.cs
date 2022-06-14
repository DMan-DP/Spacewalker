using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Autohand{
    [HelpURL("https://www.notion.so/Touch-Events-1341b3e627dd443a99593ff7f0412aa6")]
    [SelectionBase]
    public class HandTouchEvent : MonoBehaviour{
        [Header("For Solid Collision")]
        [Tooltip("Whether or not first hand to enter should take ownership and be the only one to call events")]
        public bool oneHanded = true;
        public HandType handType = HandType.both;

        [Header("Events")]
        public UnityHandEvent HandStartTouch;
        public UnityHandEvent HandStopTouch;
        
        public HandEvent HandStartTouchEvent;
        public HandEvent HandStopTouchEvent;

        private void OnEnable() {
            //HandStartTouchEvent += (hand) => HandStartTouch?.Invoke(hand);
           // HandStopTouchEvent += (hand) => HandStopTouch?.Invoke(hand);
        }

        private void OnDisable()
        {
            //HandStartTouchEvent -= (hand) => HandStartTouch?.Invoke(hand);
           // HandStopTouchEvent -= (hand) => HandStopTouch?.Invoke(hand);
        }
        
        List<Hand> hands = new List<Hand>();

        public void Touch(Hand hand) {
            if (enabled == false || handType == HandType.none || (hand.left && handType == HandType.right) ||
                (!hand.left && handType == HandType.left))
            {
                return;
            }
            
            if(!hands.Contains(hand)) {
                if(oneHanded && hands.Count == 0)
                {
                    HandStartTouch?.Invoke(hand);
                    HandStartTouchEvent?.Invoke(hand);
                }
                else
                {
                    HandStartTouch?.Invoke(hand);
                    HandStartTouchEvent?.Invoke(hand);
                }

                hands.Add(hand);
            }
        }
        
        public void Untouch(Hand hand) {
            if (enabled == false || handType == HandType.none || (hand.left && handType == HandType.right) || (!hand.left && handType == HandType.left))
                return;

            if(hands.Contains(hand)) {
                if(oneHanded && hands[0] == hand)
                {
                    HandStopTouch?.Invoke(hand);
                    HandStopTouchEvent?.Invoke(hand);
                }
                else if(!oneHanded)
                {
                    HandStopTouch?.Invoke(hand);
                    HandStopTouchEvent?.Invoke(hand);
                }

                hands.Remove(hand);
            }
        }
    }
}
