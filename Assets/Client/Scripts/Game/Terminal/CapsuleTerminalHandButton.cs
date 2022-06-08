using TMPro;
using UnityEngine;

namespace Client.Gameplay
{
	public class CapsuleTerminalHandButton : TerminalHandButtonBase
	{
		public CapsuleControllerBase CapsuleController;
		public AudioSource TerminalAudioSource;
		public bool IsLockedButton = false;
		[Space] 
		public AudioClip InteractableClip;
		public AudioClip NonInteractableClip;

		private TextMeshProUGUI buttonText;
		private bool isLocked = false;

		private void Start()
		{
			buttonText = GetComponentInChildren<TextMeshProUGUI>();
			CapsuleController.OnDoorOpenedEvent.AddListener(CapsuleOpened);
			CapsuleController.OnDoorClosedEvent.AddListener(CapsuleClosed);

			if (CapsuleController is PlayerCapsuleController)
			{
				buttonText.text = "ЗАКРЫТЬ";
			}
		}

		protected override void PressButton()
		{
			base.PressButton();

			if (IsLockedButton)
			{
				TerminalAudioSource.clip = NonInteractableClip;
				TerminalAudioSource.Play();
			}
			
			if (!isLocked)
			{
				isLocked = true;
				
				TerminalAudioSource.clip = InteractableClip;
				TerminalAudioSource.Play();
				
				if (CapsuleController.IsOpened)
					CapsuleController.CloseDoor();
				else
					CapsuleController.OpenDoor();
			}
		}

		protected override void ReleaseButton()
		{
			if (IsLockedButton)
			{
				base.ReleaseButton();
			}
		}

		private void CapsuleOpened()
		{
			if (IsPressed) IsPressed = false;
			transform.localPosition -= pressOffset;
			buttonText.text = "ЗАКРЫТЬ";
			isLocked = false;
		}

		private void CapsuleClosed()
		{
			if (IsPressed) IsPressed = false;
			transform.localPosition -= pressOffset;
			buttonText.text = "ОТКРЫТЬ";
			isLocked = false;
		}
	}
}