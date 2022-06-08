using Autohand;
using Client.Gameplay;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;

namespace Client
{
	public class PlayerCapsuleController : CapsuleControllerBase
	{
		[Space] 
		public AudioClip introAudioSequence;
		public Transform PlayerOffset;
		[Space] 
		public UnityEvent OnInitPlayerSequence;

		private bool isStarted = false;
		private static readonly int intro = Animator.StringToHash("Intro");
		private static readonly int freePlayer = Animator.StringToHash("FreePlayer");

		private void Start()
		{
			var player = AutoHandPlayer.Instance;
			player.allowClimbing = false;
			player.useMovement = false;
			player.GetComponent<XRPlayerControllerLink>().CanTurn = false;
			player.GetComponent<PlayerFlyLocomotion>().PlayerCanFly = false;

			var parentTransform = player.transform.parent;
			parentTransform.SetParent(PlayerOffset);
			parentTransform.localPosition = Vector3.zero;
			parentTransform.localRotation = Quaternion.Euler(Vector3.zero);
		}
		
		public void InitPlayerSequence()
		{
			if (isStarted) return;

			IsOpened = true;
			
			AudioSource.clip = introAudioSequence;
			AudioSource.Play();
			Animator.SetBool(intro, true);
		}

		public void InitPlayerHud()
		{
			OnInitPlayerSequence.Invoke();
		}

		public void FreePlayer()
		{
			if (isStarted) return;
			
			var player = AutoHandPlayer.Instance;
			player.allowClimbing = true;
			player.useMovement = true;
			player.GetComponent<XRPlayerControllerLink>().CanTurn = true;
			player.GetComponent<PlayerFlyLocomotion>().PlayerCanFly = true;
			
			var parentTransform = player.transform.parent;
			parentTransform.SetParent(null);
			
			AudioSource.clip = closeDoorAudioClip;
			AudioSource.Play();
			
			isStarted = true;
			Animator.SetBool(intro, false);
			Animator.SetBool(freePlayer, true);
		}
	}
}