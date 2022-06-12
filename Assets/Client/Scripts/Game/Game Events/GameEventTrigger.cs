using UnityEngine;
using UnityEngine.Events;

namespace Client
{
	[RequireComponent(typeof(BoxCollider))]
	public class GameEventTrigger : MonoBehaviour
	{
		[SerializeField] private bool oneOffEvent = true;
		[SerializeField] protected UnityEvent gameEvent;
		
		private static string playerTag = "Player";
		
		public void OnTriggerEnter(Collider other)
		{
			if (other.tag == playerTag)
			{
				gameEvent.Invoke();
				
				if (oneOffEvent)
				{
					Destroy(gameObject);
				}
			}
		}
	}
}