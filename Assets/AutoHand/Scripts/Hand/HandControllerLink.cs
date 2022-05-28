using UnityEngine;

namespace Autohand
{
	public class HandControllerLink : MonoBehaviour
	{
		public static HandControllerLink handLeft, handRight;
		public Hand hand;

		public virtual void TryHapticImpulse(float duration, float amp, float freq = 10f) { }
		
		protected virtual void Awake()
		{
			hand = GetComponent<Hand>();
		}

		protected virtual void Start()
		{
			if (hand.left)
				handLeft = this;
			else
				handRight = this;
		}
	}
}