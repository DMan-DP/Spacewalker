using Autohand;
using UnityEngine;

namespace Client
{
	public class PlayerSpawn : MonoBehaviour
	{
		public PlayerCapsuleController PlayerCapsuleController;

		public void FreePlayer()
		{
			var player = AutoHandPlayer.Instance;
			player.allowClimbing = true;
			player.useMovement = true;
			player.GetComponent<XRPlayerControllerLink>().CanTurn = true;
			player.GetComponent<PlayerFlyLocomotion>().PlayerCanFly = true;
		}

		#region MonoBehaviour Methods
		private void Start()
		{
			var player = AutoHandPlayer.Instance;
			player.allowClimbing = false;
			player.useMovement = false;
			player.GetComponent<XRPlayerControllerLink>().CanTurn = false;
			player.GetComponent<PlayerFlyLocomotion>().PlayerCanFly = false;

			var parentTransform = player.transform.parent;
			parentTransform.SetParent(PlayerCapsuleController.PlayerOffset);
			parentTransform.localPosition = Vector3.zero;
			parentTransform.localRotation = Quaternion.Euler(Vector3.zero);
			
			PlayerCapsuleController.OpenDoor();
		}
		#endregion
	}
}