using Autohand;
using Client.Gameplay;
using UnityEngine;
using UnityEngine.Events;

namespace Client
{
	public class PlayerCapsuleController : CapsuleControllerBase
	{
		[Space] 
		public AnimationClip OpenStationAnimatonClip;
		[SerializeField] private UnityEvent OnStationOpenedEvent;
		
		[Space] 
		public AnimationClip OpenHandleAnimationClip;
		[SerializeField] private UnityEvent OnHandOpenedEvent;

		[Space] 
		public AnimationClip CloseStationAnimationClip;
		[SerializeField] private UnityEvent OnStationClosedEvent;

		[Space] 
		public Transform PlayerOffset;

		public void OpenStation()
		{
			Animation.clip = OpenStationAnimatonClip;
			Animation.Play();
		}

		public void OpenHandle()
		{
			Animation.clip = OpenHandleAnimationClip;
			Animation.Play();
		}

		public void CloseStation()
		{
			AutoHandPlayer.Instance.transform.parent.SetParent(null, true);
			
			Animation.clip = CloseStationAnimationClip;
			Animation.Play();
		}

		protected void OnStationOpened()
		{
			OnStationOpenedEvent.Invoke();
		}

		protected void OnHandleOpened()
		{
			OnHandOpenedEvent.Invoke();
		}

		protected void OnStationClosed()
		{
			OnStationClosedEvent.Invoke();
		}
	}
}