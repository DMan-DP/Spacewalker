using Autohand;
using UnityEngine;

namespace Client
{
	public class XRPlayerControllerLink : MonoBehaviour
	{
		public AutoHandPlayer Player;
		public XRController LeftController;
		public XRController RightController;
		[HideInInspector] public bool CanTurn = false;
		
		#region MonoBehaviour Methods
		private void Awake()
		{
			if (!RightController || !LeftController)
			{
				Debug.LogWarning("Cannot found controllers");
			}
		}
		
		private void Update()
		{
			if (CanTurn)
			{
				Player.Turn(RightController.ThumbstickValue.x);
			}
		}
		#endregion
	}
}