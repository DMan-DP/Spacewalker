using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Client.Inputs
{
	public class InputActionManager : MonoBehaviour
	{
		[SerializeField] [Tooltip("Input action assets to affect when inputs are enabled or disabled.")]
		private List<InputActionAsset> m_ActionAssets;

		public List<InputActionAsset> actionAssets
		{
			get => m_ActionAssets;
			set => m_ActionAssets = value ?? throw new ArgumentNullException(nameof(value));
		}

		protected void OnEnable()
		{
			EnableInput();
		}

		protected void OnDisable()
		{
			DisableInput();
		}

		public void EnableInput()
		{
			if (m_ActionAssets == null)
				return;

			foreach (var actionAsset in m_ActionAssets)
				if (actionAsset != null)
					actionAsset.Enable();
		}

		public void DisableInput()
		{
			if (m_ActionAssets == null)
				return;

			foreach (var actionAsset in m_ActionAssets)
				if (actionAsset != null)
					actionAsset.Disable();
		}
	}
}