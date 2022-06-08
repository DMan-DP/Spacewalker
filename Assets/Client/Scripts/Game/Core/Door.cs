using UnityEngine;
using UnityEngine.Events;

namespace Client
{
	public class Door : MonoBehaviour
	{
		private static readonly int isOpened = Animator.StringToHash("IsOpened");
		public UnityEvent OnDoorBeginOpenEvent;
		public UnityEvent OnDoorOpenCompletedEvent;

		[SerializeField] [Tooltip("Dont work")]
		private bool isAutoCloseDoor;

		[SerializeField] [Tooltip("Dont work")]
		private float timeToCloseDoor = 20;

		[SerializeField] 
		private AudioClip audioClip;

		private Animator animator;
		private AudioSource audioSource;

		public void OpenDoor()
		{
			animator.SetBool(isOpened, true);
			if (audioClip != null) audioSource.PlayOneShot(audioClip);
			OnDoorBeginOpenEvent.Invoke();
		}

		protected void OnDoorOpenCompleted()
		{
			OnDoorOpenCompletedEvent.Invoke();
		}

		#region MonoBehaviour Methods
		private void Start()
		{
			animator = GetComponentInChildren<Animator>();
			audioSource = GetComponentInChildren<AudioSource>();
		}
		#endregion
	}
}