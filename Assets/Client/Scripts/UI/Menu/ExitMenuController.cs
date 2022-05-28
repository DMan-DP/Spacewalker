using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class ExitMenuController : MonoBehaviour
    {
        public GameObject ExitButton;
        public GameObject ConfirmExitPanel;
        private bool showPanel = false;
        
        private void Start()
        {
            ConfirmExitPanel.SetActive(false);
        }
        
        public void ShowConfirmMenu()
        {
            ExitButton.GetComponent<Animator>().SetBool("IsHideButton", true);
            StartCoroutine(ShowPanel());
        }

        public void HideConfirmMenu()
        {
            ConfirmExitPanel.SetActive(false);
            ExitButton.GetComponent<Animator>().SetBool("IsHideButton", false);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
        
        IEnumerator ShowPanel()
        {
            yield return new WaitForSeconds(0.2f);
            ConfirmExitPanel.SetActive(true);
        }
    }
}