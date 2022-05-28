using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Client
{
    public class MenuStartGameLoad : MonoBehaviour
    {
        private static readonly int LoadGame = Animator.StringToHash("LoadGame");
        private SceneLoader sceneLoader;
        private bool isLoaded = false;

        public Animator MenuObjectAnimator;
        [Space]
        public GameObject LoadUI;
        public Image[] ProgressBar;
        public float[] speed;
        [Space] 
        public GameObject MenuUI;
        public GameObject ExitUI;
        public GameObject ExitButton;

        private void Start()
        {
            LoadUI.SetActive(false);
        }

        public void StartGame()
        {
            MenuUI.SetActive(false);
            ExitUI.SetActive(false);
            ExitButton.SetActive(false);
            
            LoadUI.SetActive(true);
            sceneLoader = SceneLoader.GetInstance();
            SceneLoader.SwitchToScene(SceneLoader.SpaceshipSceneName);
            MenuObjectAnimator.SetBool(LoadGame, true);
            isLoaded = true;
        }

        private void Update()
        {
            if (isLoaded)
            {
                var progress = sceneLoader.GetLoadProgress();
                for (int i = 0; i < ProgressBar.Length; i++)
                {
                    ProgressBar[i].fillAmount = Mathf.Lerp(ProgressBar[i].fillAmount, progress, Time.deltaTime * speed[i]);
                    if (ProgressBar[i].fillAmount > 0.95)
                    {
                        ProgressBar[i].fillAmount = 1;
                        ProgressBar[i].GetComponentInChildren<TextMeshProUGUI>().text = "Успешно";
                    }
                }
            }
        }
    }
}