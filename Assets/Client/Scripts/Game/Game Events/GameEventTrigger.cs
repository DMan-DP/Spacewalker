using UnityEngine;
using UnityEngine.Events;

namespace Client
{
	[RequireComponent(typeof(BoxCollider))]
	public class GameEventTrigger : MonoBehaviour
	{
		[SerializeField] private bool oneOffEvent = true;
		[SerializeField] protected UnityEvent gameEvent;

		public void OnTriggerEnter(Collider other)
		{
			if (other.tag == "MainCamera")
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