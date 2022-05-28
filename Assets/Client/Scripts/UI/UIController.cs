using UnityEditor;
using UnityEngine;

namespace Client
{
	public class UIController : MonoBehaviour
	{
		public GameObject ButtonPanel;
		[Space]
		public GameObject Panel;
		[Space]
		[Header("UI Panels")]
		public GameObject PanelMenu;
		public GameObject MenuPause;
		public GameObject SettingsMenu;
		public GameObject ExitMenu;
		public void OpenPanel()
		{
			PanelMenu.SetActive(true);
			Panel.SetActive(true);
			ButtonPanel.SetActive(false);
		}

		public void ClosePanel()
		{
			HideAllUIPanels();
			Panel.SetActive(false);
			ButtonPanel.SetActive(true);
		}

		private void HideAllUIPanels()
		{
			PanelMenu.SetActive(false);
			MenuPause.SetActive(false);
			SettingsMenu.SetActive(false);
			ExitMenu.SetActive(false);
		}

		public void OnPause()
		{
			HideAllUIPanels();
			MenuPause.SetActive(true);
		}

		public void OnSettings()
		{
			HideAllUIPanels();
			SettingsMenu.SetActive(true);
		}
		
		public void OnExit()
		{
			HideAllUIPanels();
			SettingsMenu.SetActive(true);
		}
		
		private void Start()
		{
			HideAllUIPanels();
			ButtonPanel.SetActive(true);
			Panel.SetActive(false);
		}
	}
}