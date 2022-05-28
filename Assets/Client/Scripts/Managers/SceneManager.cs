using UnityEngine;

namespace Client
{
	public class SceneManager : SingletonBehaviour<SceneManager>
	{
		public void GoToManiMenu()
		{
			Debug.Log("Go to menu");
		}
	}
}