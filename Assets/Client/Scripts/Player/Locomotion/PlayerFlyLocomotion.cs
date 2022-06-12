using Autohand;
using UnityEngine;

namespace Client
{
	public class PlayerFlyLocomotion : MonoBehaviour
	{
		[SerializeField] private float speed = 1f;
		[SerializeField] private float speedVelocity = 2f;
		[HideInInspector] public bool PlayerCanFly = false;

		private AutoHandPlayer autoHandPlayer;
		private XRController leftController;
		private XRController rightController;

		private bool isFly;
		private bool leftHandActive;
		private bool rightHandActive;
		[SerializeField] private Vector3 previousVelocity;
		[SerializeField] private Vector3 velocity;

		private void TryBeginFly()
		{
			if (CanFly() && !isFly) isFly = true;
		}

		private void TryEndFly()
		{
			if (!CanFly() && isFly)
			{
				isFly = false;
				autoHandPlayer.body.velocity = previousVelocity * speed;
				previousVelocity = Vector3.zero;
			}
		}

		private bool CanFly()
		{
			return !autoHandPlayer.IsClimbing() && (leftHandActive || rightHandActive) && PlayerCanFly;
		}

		private void ApplyVelocity()
		{
			previousVelocity = velocity;
			velocity = CollectVelocity();
			//autoHandPlayer.AddVelocity(velocity * speed);
			autoHandPlayer.body.velocity = Vector3.Lerp(previousVelocity, velocity * speed, Time.fixedTime * speedVelocity);
		}

		private Vector3 CollectVelocity()
		{
			var velocity = Vector3.zero;
			if (leftHandActive) velocity += leftController.transform.forward;
			if (rightHandActive) velocity += rightController.transform.forward;
			return velocity;
		}

		#region MonoBehaviour Methods
		private void Awake()
		{
			autoHandPlayer = AutoHandPlayer.Instance;

			var xrPlayerControllerLink = GetComponent<XRPlayerControllerLink>();
			if (!xrPlayerControllerLink)
			{
				Debug.LogWarning("Cannot found xr player controller");
				return;
			}

			leftController = xrPlayerControllerLink.LeftController;
			rightController = xrPlayerControllerLink.RightController;
		}

		private void Update()
		{
			leftHandActive = leftController.IsSecondaryButtonActivate;
			rightHandActive = rightController.IsSecondaryButtonActivate;
		}

		private void FixedUpdate()
		{
			if (!autoHandPlayer.IsClimbing())
			{
				TryBeginFly();
				if (isFly) ApplyVelocity();
				TryEndFly();
			}
		}
		#endregion
	}
}