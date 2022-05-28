using Autohand;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.OpenXR.Input;
using InputDevice = UnityEngine.XR.InputDevice;

namespace Client
{
	public class XRHandControllerLink : HandControllerLink
	{
		private bool isGrabbing;
		private bool isSqueezing;
		[field: SerializeField] public XRController xrController { get; private set; }

		private void Grab(InputAction.CallbackContext callbackContext)
		{
			if (!isGrabbing)
			{
				hand.Grab();
				isGrabbing = true;
			}
		}

		private void Release(InputAction.CallbackContext callbackContext)
		{
			if (isGrabbing)
			{
				hand.Release();
				isGrabbing = false;
			}
		}

		private void Squeeze(InputAction.CallbackContext callbackContext)
		{
			if (!isSqueezing)
			{
				hand.Squeeze();
				isSqueezing = true;
			}
		}

		private void StopSqueeze(InputAction.CallbackContext callbackContext)
		{
			if (isSqueezing)
			{
				hand.Unsqueeze();
				isSqueezing = false;
			}
		}

		public override void TryHapticImpulse(float duration, float amp, float freq = 10)
		{
			base.TryHapticImpulse(duration, amp, freq);
			
			// TODO: CHECK THIS
			if (hand.left)
			{
				OpenXRInput.SendHapticImpulse(xrController.HapticProperty.action, duration, amp, UnityEngine.InputSystem.XR.XRController.leftHand);
			}
			else
			{
				OpenXRInput.SendHapticImpulse(xrController.HapticProperty.action, duration, amp, UnityEngine.InputSystem.XR.XRController.rightHand);
			}
		}

		#region MonoBehaviour Methods
		protected  override void Awake()
		{
			base.Awake();
			if (xrController == null) Debug.LogError("Can't find XR Controller");
		}

		private void OnEnable()
		{
			var triggerActivateAction = xrController.TriggerActivateProperty.action;
			triggerActivateAction.performed += Squeeze;
			triggerActivateAction.canceled += StopSqueeze;

			var gripActivateAction = xrController.GripActivateProperty.action;
			gripActivateAction.performed += Grab;
			gripActivateAction.canceled += Release;
		}

		private void OnDisable()
		{
			var triggerActivateAction = xrController.TriggerActivateProperty.action;
			triggerActivateAction.performed -= Squeeze;
			triggerActivateAction.canceled -= StopSqueeze;

			var gripActivateAction = xrController.GripActivateProperty.action;
			gripActivateAction.performed -= Grab;
			gripActivateAction.canceled -= Release;
		}

		private void Update()
		{
			//hand.SetGrip(xrController.GripValue);
		}
		#endregion
	}
}