using Autohand;
using AutoHand;
using UnityEngine;

namespace Client.Gameplay
{
	public class TerminalHandButtonBase : MonoBehaviour
	{
		[SerializeField] protected Vector3 pressOffset;
		
		private HandTouchEvent touchEvent;
		public bool IsPressed { get; protected set; } = false;
		
		private void Awake()
		{
			touchEvent = GetComponent<HandTouchEvent>();
		}
		
		protected virtual void OnEnable() {
			touchEvent.HandStartTouchEvent += OnTouch;
			touchEvent.HandStopTouchEvent += OnUntouch;
		}
		protected virtual void OnDisable() {
			touchEvent.HandStartTouchEvent -= OnTouch;
			touchEvent.HandStopTouchEvent -= OnUntouch;
		}
		
		private void OnTouch(Hand hand) {
			if (!IsPressed) PressButton();
		}
		private void OnUntouch(Hand hand) {
			if(IsPressed) ReleaseButton();
		}

		protected virtual void PressButton()
		{
			if (!IsPressed) transform.localPosition += pressOffset;
			IsPressed = true;
		}
		
		protected virtual void ReleaseButton()
		{
			if(IsPressed) transform.localPosition -= pressOffset;
			IsPressed = false;
		}
	}
}