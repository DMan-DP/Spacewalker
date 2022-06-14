using UnityEngine;

namespace Client
{
    public class UIController : MonoBehaviour
    {
        public GameObject ButtonPanel;
        public GameObject TaskPanel;
        public GameObject MenuPanel;

        [SerializeField] private UIState uiState;
        private AudioSource audioSource;

        private Camera mainCamera;
        private Animator animator;
        private static readonly int showMainPanel = Animator.StringToHash("ShowMainPanel");
        private static readonly int showMenuPanel = Animator.StringToHash("ShowMenuPanel");
        private static readonly int showScannerPanel = Animator.StringToHash("ShowScannerPanel");

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            
            animator = GetComponent<Animator>();
            mainCamera = Camera.main;
            SetUIState(UIState.Idle);
        }

        public void ButtonPanelClick()
        {
            audioSource.Play();
            switch (uiState)
            {
                case UIState.Idle:
                {
                    SetUIState(UIState.MainPanel);
                    break;
                }
                case UIState.MainPanel:
                case UIState.Scanner:
                case UIState.MenuPanel:
                {
                    if (uiState == UIState.MenuPanel)
                    {
                        MenuPanel.GetComponent<GameMenu>().ShowMenuPanel();
                    }
                    SetUIState(UIState.Idle);
                    break;
                }
            }
        }

        public void OpenScanner()
        {
            if (uiState == UIState.MainPanel)
            {
                SetUIState(UIState.Scanner);
            }
        }

        public void OpenMenu()
        {
            if (uiState == UIState.MainPanel)
            {
                SetUIState(UIState.MenuPanel);
            }
        }

        private void SetUIState(UIState newUIState)
        {
            audioSource.Play();
            
            uiState = newUIState;
            switch (newUIState)
            {
                case UIState.Idle:
                {
                    animator.SetBool(showMainPanel, false);
                    animator.SetBool(showMenuPanel, false);
                    animator.SetBool(showScannerPanel, false);
                    uiState = UIState.Idle;
                    break;
                }
                case UIState.MainPanel:
                {
                    animator.SetBool(showMainPanel, true);
                    break;
                }
                case UIState.MenuPanel:
                {
                    animator.SetBool(showMainPanel, false);
                    animator.SetBool(showMenuPanel, true);
                    break;
                }
                case UIState.Scanner:
                {
                    animator.SetBool(showMainPanel, false);
                    animator.SetBool(showScannerPanel, true);
                    break;
                }
            }
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