using UnityEngine;

namespace Client
{
	public class GameMenu : MonoBehaviour
	{
		public GameObject MenuPanel;
		public GameObject ConfirmRestartPanel;
		public GameObject ConfirmExitPanel;
		public GameObject SettingsPanel;

		private void Start()
		{
			ShowPanel(MenuPanel);
		}

		private void ShowPanel(GameObject panel)
		{
			MenuPanel.SetActive(false);
			ConfirmRestartPanel.SetActive(false);
			ConfirmExitPanel.SetActive(false);
			SettingsPanel.SetActive(false);

			panel.SetActive(true);
		}

		public void ShowMenuPanel()
		{
			ShowPanel(MenuPanel);
		}

		public void ShowRestartPanel()
		{
			ShowPanel(ConfirmRestartPanel);
		}

		public void ShowExitPanel()
		{
			ShowPanel(ConfirmExitPanel);
		}

		public void ShowSettingsPanel()
		{
			ShowPanel(SettingsPanel);
		}

		public void ExitGame()
		{
			SceneLoader.SwitchToScene("Menu", true);
		}

		public void RestartGame()
		{
			SceneLoader.SwitchToScene("Spaceship", true);
		}
	}
}