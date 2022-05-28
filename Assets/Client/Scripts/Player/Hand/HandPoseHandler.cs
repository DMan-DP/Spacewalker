using Autohand;
using UnityEngine;

namespace Client
{
	public class HandPoseHandler : MonoBehaviour
	{
		[SerializeField] private FingerBend defaultBend;
		[Header("Animations")] 
		[SerializeField] private FingerBend thumbBend;
		[SerializeField] private FingerBend gripBend;
		[SerializeField] private FingerBend gripAndThumbBend;
		[SerializeField] private FingerBend triggerBend;
		
		private Hand hand;
		private XRController xrController;
		
		private void BendAction(FingerBend bend)
		{
			for (var i = 0; i < hand.fingers.Length; i++) hand.fingers[i].bendOffset += bend.GetBendById(i);
		}

		private void UnbendAction(FingerBend bend)
		{
			for (var i = 0; i < hand.fingers.Length; i++) hand.fingers[i].bendOffset -= bend.GetBendById(i);
		}

		private void ResetBend()
		{
			for (var i = 0; i < hand.fingers.Length; i++) hand.fingers[i].bendOffset = defaultBend.GetBendById(i);
		}

		private void UpdateBend()
		{
			for (var i = 0; i < hand.fingers.Length; i++)
			{
				float value = 0;

				if (xrController.TriggerValue > 0)
				{
					value += xrController.TriggerValue * triggerBend.GetBendById(i);
				}

				if (xrController.GripValue > 0)
				{
					if (xrController.IsButtonsTouched)
						value += xrController.GripValue * gripAndThumbBend.GetBendById(i);
					else
						value += xrController.GripValue * gripBend.GetBendById(i);
				}
				else if (xrController.IsButtonsTouched)
				{
					value += thumbBend.GetBendById(i);
				}
				
				hand.fingers[i].bendOffset = Mathf.Clamp(value, defaultBend.GetBendById(i), 1);
			}
		}

		#region MonoBehaviour Methods
		private void Start()
		{
			hand = GetComponent<Hand>();

			var handInteractorXRLink = GetComponent<XRHandControllerLink>();
			if (handInteractorXRLink == null) Debug.LogError("Can't find XR Controller link");
			else xrController = handInteractorXRLink.xrController;
		}

		private void Update()
		{
			UpdateBend();
		}
		#endregion
	}
}