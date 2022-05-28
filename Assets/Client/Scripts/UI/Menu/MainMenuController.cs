using System.Collections;
using UnityEngine;

namespace Client
{
    public class MainMenuController : MonoBehaviour
    {
        public GameObject MainMenuPanel;
        public GameObject SelectedProfilePanel;
        public GameObject CreateOrOwerridePanel;
        public GameObject SettingsPanel;
        [Space] 
        public GameObject EnterButton;

        private bool isNewGame = false;

        private void Start()
        {
            HideAllPanels();
        }

        public void ShowMainMenu()
        {
            EnterButton.GetComponent<Animator>().SetBool("IsHideButton", true);
            StartCoroutine(ShowMainMenuPanel());
        }

        private IEnumerator ShowMainMenuPanel()
        {
            yield return new WaitForSeconds(0.2f);
            ShowPanel(MainMenuPanel);
        }

        public void ShowPanel(GameObject panel)
        {
            HideAllPanels();
            panel.SetActive(true);
        }

        private void HideAllPanels()
        {
            MainMenuPanel.SetActive(false);
            SelectedProfilePanel.SetActive(false);
            CreateOrOwerridePanel.SetActive(false);
            SettingsPanel.SetActive(false);
        }

        public void SetNewGame(bool value)
        {
            isNewGame = value;
        }

        public void SelectProfile(bool emptyProfile)
        {
            if (isNewGame)
            {
                if (emptyProfile)
                    StartNewGame();
                else
                    ShowPanel(CreateOrOwerridePanel);
            }
            else
            {
                if (!emptyProfile) StartNewGame();
            }
        }

        public void StartNewGame()
        {
            GetComponentInParent<MenuStartGameLoad>().StartGame();
        }
    }
}