using UnityEngine;
using UnityEngine.Events;

namespace Client.Gameplay
{
	public class CapsuleControllerBase : MonoBehaviour
	{
		[Space]
		public AnimationClip OpenDoorAnimationClip;
		[SerializeField] private UnityEvent onDoorOpenedEvent;
		[Space] 
		public AnimationClip CloseDoorAnimationClip;
		[SerializeField] private UnityEvent onDoorClosedEvent;

		protected Animation Animation;

		#region MonoBehaviour Methods
		private void Awake()
		{
			Animation = GetComponent<Animation>();
		}
		#endregion

		public void OpenDoor()
		{
			Animation.clip = OpenDoorAnimationClip;
			Animation.Play();
		}

		public void CloseDoor()
		{
			Animation.clip = CloseDoorAnimationClip;
			Animation.Play();
		}

		protected void OnDoorOpened()
		{
			onDoorOpenedEvent.Invoke();
		}

		protected void OnDoorClosed()
		{
			onDoorClosedEvent.Invoke();
		}
	}
}