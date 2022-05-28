using UnityEngine;

namespace Client
{
	public class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
	{
		private static T instance;
		public static bool IsInitialized => instance != null;
		
		public static T GetInstance()
		{
			if (instance == null)
			{
				var instances = FindObjectsOfType<T>();
				if (instances.Length == 1) instance = instances[0];
				
				if (instances.Length > 1)
				{
					Debug.LogError("[Singleton] Instance '" + typeof(T) + "' already exist. \n " +
					               "There should never be more than one singleton!");
				}
			}

			return instance;
		}

		protected virtual void Awake()
		{
			GetInstance();
			DontDestroyOnLoad(gameObject);
		}
	}
}