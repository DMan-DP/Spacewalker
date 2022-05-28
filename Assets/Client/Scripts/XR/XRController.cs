using Autohand;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.OpenXR.Input;

namespace Client
{
	public class XRController : MonoBehaviour
	{
		#region References
		[field: Header("Trigger")]
		[field: SerializeField] public InputActionProperty TriggerValueProperty { get; private set; }
		[field: SerializeField] public InputActionProperty TriggerActivateProperty { get; private set; }
		[field: SerializeField] public InputActionProperty TriggerTouchedProperty { get; private set; }

		[field: Header("Grip")]
		[field: SerializeField] public InputActionProperty GripValueProperty { get; private set; }
		[field: SerializeField] public InputActionProperty GripActivateProperty { get; private set; }

		[field: Header("Button")]
		[field: SerializeField] public InputActionProperty PrimaryButtonProperty { get; private set; }
		[field: SerializeField] public InputActionProperty SecondaryButtonProperty { get; private set; }

		[field: Header("Stick")]
		[field: SerializeField] public InputActionProperty ThumbstickProperty { get; private set; }

		[field: Header("Physics")]
		[field: SerializeField] public InputActionProperty VelocityProperty { get; private set; }
		
		[field: Header("Other")]
		[field: SerializeField] public InputActionProperty ButtonsTouchedProperty { get; private set; }
		[field: SerializeField] public InputActionProperty HapticProperty { get; private set; }
		#endregion

		#region Controller Values
		public float TriggerValue { get; private set; }
		public bool IsTriggerActivate { get; private set; }
		public bool IsTriggerTouched { get; private set; }
		public float GripValue { get; private set; }
		public bool IsGripActivate { get; private set; }
		public bool IsPrimaryButtonActivate { get; private set; }
		public bool IsSecondaryButtonActivate { get; private set; }
		public Vector2 ThumbstickValue { get; private set; }
		public bool IsButtonsTouched { get; private set; }
		public Vector3 ControllerVelocity { get; private set; }
		#endregion

		#region MonoBehaviour Methods
		protected void OnEnable()
		{
			InputSystem.onAfterUpdate += UpdateCallback;
			BindActions();
		}

		protected void OnDisable()
		{
			UnbindActions();
			InputSystem.onAfterUpdate -= UpdateCallback;
		}
		#endregion

		#region Binding
		private void BindActions()
		{
			var triggerValueAction = TriggerValueProperty.action;
			triggerValueAction.performed += TriggerPerformed;
			triggerValueAction.canceled += TriggerCanceled;

			var thumbstickAction = ThumbstickProperty.action;
			thumbstickAction.performed += ThumbstickPerformed;
			thumbstickAction.canceled += ThumbstickCanceled;

			var gripValueAction = GripValueProperty.action;
			gripValueAction.performed += GripPerformed;
			gripValueAction.canceled += GripCanceled;
		}
		
		private void UnbindActions()
		{
			var triggerValueAction = TriggerValueProperty.action;
			triggerValueAction.performed -= TriggerPerformed;
			triggerValueAction.canceled -= TriggerCanceled;
			
			var thumbstickAction = ThumbstickProperty.action;
			thumbstickAction.performed -= ThumbstickPerformed;
			thumbstickAction.canceled -= ThumbstickCanceled;

			var gripValueAction = GripValueProperty.action;
			gripValueAction.performed -= GripPerformed;
			gripValueAction.canceled -= GripCanceled;
		}
		#endregion
		
		#region Update Callback
		private void UpdateCallback()
		{
			if (InputState.currentUpdateType == InputUpdateType.BeforeRender)
				OnBeforeRender();
			else
				OnUpdate();
		}

		private void OnBeforeRender()
		{
			UpdateInputActions();
		}

		private void OnUpdate() { }
		#endregion

		#region Input Callback
		private void TriggerPerformed(InputAction.CallbackContext context)
		{
			TriggerValue = context.ReadValue<float>();
		}

		private void TriggerCanceled(InputAction.CallbackContext context)
		{
			TriggerValue = 0f;
		}

		private void GripPerformed(InputAction.CallbackContext context)
		{
			GripValue = context.ReadValue<float>();
		}

		private void GripCanceled(InputAction.CallbackContext context)
		{
			TriggerValue = 0f;
		}

		private void ThumbstickPerformed(InputAction.CallbackContext context)
		{
			ThumbstickValue = context.ReadValue<Vector2>();
		}

		private void ThumbstickCanceled(InputAction.CallbackContext context)
		{
			ThumbstickValue = Vector2.zero;
		}

		private void UpdateInputActions()
		{
			IsTriggerActivate = TriggerActivateProperty.action.IsPressed();
			IsTriggerTouched = TriggerTouchedProperty.action.IsPressed();
			IsGripActivate = GripActivateProperty.action.IsPressed();

			IsPrimaryButtonActivate = PrimaryButtonProperty.action.IsPressed();
			IsSecondaryButtonActivate = SecondaryButtonProperty.action.IsPressed();

			IsButtonsTouched = ButtonsTouchedProperty.action.IsPressed();

			ControllerVelocity = VelocityProperty.action.ReadValue<Vector3>();
		}
		#endregion
	}
}