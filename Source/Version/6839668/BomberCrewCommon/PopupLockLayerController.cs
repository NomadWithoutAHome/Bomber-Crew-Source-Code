using UnityEngine;

namespace BomberCrewCommon;

public class PopupLockLayerController : MonoBehaviour
{
	[SerializeField]
	private string[] m_inputLayers;

	[SerializeField]
	private bool m_debug;

	private bool m_wasLocked;

	private void Awake()
	{
		m_wasLocked = false;
		if (Singleton<UIPopupManager>.Instance != null)
		{
			Singleton<UIPopupManager>.Instance.PopupWasShown += HandlePopupWasShown;
			Singleton<UIPopupManager>.Instance.PopupWasDismissed += HandlePopupWasDismissed;
		}
	}

	private void OnDestroy()
	{
		if (Singleton<UIPopupManager>.Instance != null)
		{
			Singleton<UIPopupManager>.Instance.PopupWasShown -= HandlePopupWasShown;
			Singleton<UIPopupManager>.Instance.PopupWasDismissed -= HandlePopupWasDismissed;
		}
	}

	private void HandlePopupWasShown(UIPopUp popup)
	{
		LockInput(locked: true);
	}

	private void HandlePopupWasDismissed(UIPopUp popup)
	{
		LockInput(locked: false);
	}

	private void LockInput(bool locked)
	{
		if (m_wasLocked == locked)
		{
			return;
		}
		if (locked)
		{
			string[] inputLayers = m_inputLayers;
			foreach (string layerId in inputLayers)
			{
				Singleton<InputLayerInterface>.Instance.DisableLayerInput(layerId);
			}
		}
		else
		{
			string[] inputLayers2 = m_inputLayers;
			foreach (string layerId2 in inputLayers2)
			{
				Singleton<InputLayerInterface>.Instance.EnableLayerInput(layerId2);
			}
		}
		m_wasLocked = locked;
	}
}
