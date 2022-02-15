using BomberCrewCommon;
using UnityEngine;

public class ButtonPressOnMainAction : MonoBehaviour
{
	[SerializeField]
	private MainActionButtonMonitor.ButtonPress m_buttonPressDown;

	[SerializeField]
	private MainActionButtonMonitor.ButtonPress m_buttonPressClick;

	[SerializeField]
	private tk2dUIItem m_uiItem;

	[SerializeField]
	private bool m_ignoreEnabled;

	private void OnEnable()
	{
		Singleton<MainActionButtonMonitor>.Instance.AddListener(ListenForButton);
	}

	private void OnDisable()
	{
		Singleton<MainActionButtonMonitor>.Instance.RemoveListener(ListenForButton, invalidateCurrentPress: false);
	}

	private bool ListenForButton(MainActionButtonMonitor.ButtonPress bp)
	{
		if (m_ignoreEnabled || Singleton<UISelector>.Instance.IsValid(m_uiItem))
		{
			if (bp == m_buttonPressClick)
			{
				m_uiItem.SimulateUpClick();
				return true;
			}
			if (bp == m_buttonPressDown)
			{
				m_uiItem.SimulateDown();
				return true;
			}
		}
		return false;
	}
}
