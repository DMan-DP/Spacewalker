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
            ExitButton.SetActive(true);
            ConfirmExitPanel.SetActive(false);
        }
        
        public void ShowConfirmMenu()
        {
            ExitButton.GetComponent<Animator>().SetBool("IsHideButton", true);
            ExitButton.GetComponent<Collider>().enabled = false;
            StartCoroutine(ShowPanel());
        }

        public void HideConfirmMenu()
        {
            ConfirmExitPanel.SetActive(false);
            ExitButton.GetComponent<Animator>().SetBool("IsHideButton", false);
            ExitButton.GetComponent<Collider>().enabled = true;
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