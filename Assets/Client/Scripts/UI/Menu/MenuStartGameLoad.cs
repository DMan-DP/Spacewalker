using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace Client
{
	public class MenuStartGameLoad : MonoBehaviour
	{
		private static readonly int LoadGame = Animator.StringToHash("LoadGame");

		public Animator MenuObjectAnimator;

		[Space] public GameObject LoadUI;

		public Image[] ProgressBar;
		public float[] speed;

		[Space] public GameObject MenuUI;

		public GameObject ExitUI;
		public GameObject ExitButton;

		[Space] public PlayableDirector PlayableDirector;

		private bool isLoaded;
		private bool continueLoad = false;
		private SceneLoader sceneLoader;

		private void Start()
		{
			LoadUI.SetActive(false);
		}

		private void Update()
		{
			if (isLoaded && !continueLoad)
			{
				var loadPorgress = sceneLoader.GetLoadProgress();
				
				for (var i = 0; i < ProgressBar.Length; i++)
				{
					ProgressBar[i].fillAmount =
						Mathf.Lerp(ProgressBar[i].fillAmount, loadPorgress, Time.deltaTime * speed[i]);
				}
				
				if (sceneLoader.IsSceneLoaded() || loadPorgress >= 0.85)
				{
				    for (var i = 0; i < ProgressBar.Length; i++)
                    {
						ProgressBar[i].fillAmount = 1;
	                    ProgressBar[i].GetComponentInChildren<TextMeshProUGUI>().text = "УСПЕШНО";
                    }
					sceneLoader.StartLoadScene();
					continueLoad = true;
				}
			}
		}

		public void LoadMenu()
		{
			PlayableDirector?.Play();
		}

		public void StartGame()
		{
			if (isLoaded) return;

			MenuUI.SetActive(false);
			ExitUI.SetActive(false);
			ExitButton.SetActive(false);

			LoadUI.SetActive(true);
			sceneLoader = SceneLoader.GetInstance();
			SceneLoader.SwitchToScene(SceneLoader.SpaceshipSceneName);
			MenuObjectAnimator.SetBool(LoadGame, true);
			isLoaded = true;
		}
	}
}