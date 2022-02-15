using System;
using BomberCrewCommon;
using UnityEngine;

public class ShowHelpPageButton : MonoBehaviour
{
	[SerializeField]
	private tk2dUIItem m_helpButton;

	[SerializeField]
	private string m_manualRef;

	private void Start()
	{
		m_helpButton.OnClick += OnClickHelp;
	}

	private void OnClickHelp()
	{
		UIPopupData uIPopupData = new UIPopupData();
		uIPopupData.PopupStartCallback = (Action<UIPopUp>)Delegate.Combine(uIPopupData.PopupStartCallback, (Action<UIPopUp>)delegate(UIPopUp pop)
		{
			pop.GetComponent<InMissionPauseMenu>().SetShowManual(m_manualRef);
		});
		Singleton<UIPopupManager>.Instance.DisplayPopup(Singleton<GameFlow>.Instance.GetPauseMenuPrefab(), uIPopupData);
	}
}
