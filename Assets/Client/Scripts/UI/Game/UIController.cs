using UnityEngine;

namespace Client
{
    public class UIController : MonoBehaviour
    {
        private static readonly int showPanelMenu = Animator.StringToHash("ShowPanelMenu");
        public GameObject ButtonPanel;

        private Animator animator;

        private UIState uiState;

        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        public void ButtonPanelClick()
        {
            switch (uiState)
            {
                case UIState.Idle:
                {
                    animator.SetBool(showPanelMenu, true);
                    uiState = UIState.MainPanel;
                    break;
                }
                case UIState.MainPanel:
                {
                    ResetPanel();
                    break;
                }
                case UIState.Scanner:
                {
                    ResetPanel();
                    break;
                }
                case UIState.MenuPanel:
                {
                    ResetPanel();
                    break;
                }
            }
        }

        public void OpenScanner()
        {
            if (uiState == UIState.MainPanel)
            {
                animator.SetBool(showPanelMenu, false);
                uiState = UIState.Scanner;
            }
        }

        public void OpenMenu()
        {
            if (uiState == UIState.MainPanel)
            {
                animator.SetBool(showPanelMenu, false);
                uiState = UIState.MenuPanel;
            }
        }

        private void ResetPanel()
        {
            animator.SetBool(showPanelMenu, false);
            uiState = UIState.Idle;
        }

        private enum UIState
        {
            Idle,
            MainPanel,
            Scanner,
            MenuPanel
        }
    }
}