using UnityEngine;
using UnityEngine.Events;

namespace Client.Gameplay
{
	public class CapsuleControllerBase : MonoBehaviour
	{
		[SerializeField] protected AudioClip openDoorAudioClip;
		public UnityEvent OnDoorOpenedEvent;
		[Space] 
		[SerializeField] protected AudioClip closeDoorAudioClip;
		public UnityEvent OnDoorClosedEvent;

		public bool IsOpened { get; protected set; } = false;
		protected Animator Animator;
		protected AudioSource AudioSource;
		private static readonly int isOpenDoor = Animator.StringToHash("IsOpenDoor");


		#region MonoBehaviour Methods
		private void Awake()
		{
			Animator = GetComponent<Animator>();
			AudioSource = GetComponent<AudioSource>();
		}
		#endregion

		public void OpenDoor()
		{
			AudioSource.clip = openDoorAudioClip;
			AudioSource.Play();
			Animator.SetBool(isOpenDoor, true);
		}

		public void CloseDoor()
		{
			AudioSource.clip = closeDoorAudioClip;
			AudioSource.Play();
			Animator.SetBool(isOpenDoor, false);
		}

		protected void OnDoorOpened()
		{
			OnDoorOpenedEvent.Invoke();
			IsOpened = true;
		}

		protected void OnDoorClosed()
		{
			OnDoorClosedEvent.Invoke();
			IsOpened = false;
		}
	}
}