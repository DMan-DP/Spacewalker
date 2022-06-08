using UnityEngine;

namespace Client
{
    public class UIController : MonoBehaviour
    {
        public GameObject ButtonPanel;
        public GameObject TaskPanel;
        public GameObject MenuPanel;
        public Vector3 LookAtCameraWorldUp = Vector3.forward;

        [SerializeField] private UIState uiState;

        private Camera mainCamera;
        private Animator animator;
        private static readonly int showMainPanel = Animator.StringToHash("ShowMainPanel");
        private static readonly int showMenuPanel = Animator.StringToHash("ShowMenuPanel");
        private static readonly int showScannerPanel = Animator.StringToHash("ShowScannerPanel");

        private void Start()
        {
            animator = GetComponent<Animator>();
            mainCamera = Camera.main;
        }

        private void Update()
        {
            if (uiState == UIState.MainPanel)
            {
                TaskPanel.transform.LookAt(mainCamera.transform, LookAtCameraWorldUp);
            }
            else if (uiState == UIState.MenuPanel)
            {
                MenuPanel.transform.LookAt(mainCamera.transform, LookAtCameraWorldUp);
            }
        }

        public void ButtonPanelClick()
        {
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