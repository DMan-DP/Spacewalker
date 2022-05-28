using UnityEngine;
using UnityEngine.Events;

namespace Client
{
	[RequireComponent(typeof(BoxCollider))]
	public class XRButton : MonoBehaviour
	{
		public static string ClickTag = "PlayerFinger"; 
		public UnityEvent OnClick;

		private void Awake()
		{
			GetComponent<Collider>().isTrigger = true;
		}

		public void OnTriggerEnter(Collider other)
		{
			if (other.tag == ClickTag)
			{
				OnClick.Invoke();
			}
		}
	}
}