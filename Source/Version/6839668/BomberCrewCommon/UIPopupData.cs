using System;
using UnityEngine;

namespace BomberCrewCommon;

public class UIPopupData
{
	public GameObject Prefab;

	public Action<UIPopUp> PopupStartCallback;

	public Action<UIPopUp> PopupShownCallback;

	public Action<UIPopUp> PopupDismissedCallback;

	public Action<UIPopUp> PopupWillBeDismissedCallback;

	public UIPopupData()
	{
	}

	public UIPopupData(Action<UIPopUp> shownCallback, Action<UIPopUp> dismissCallback)
	{
		PopupShownCallback = shownCallback;
		PopupDismissedCallback = dismissCallback;
	}
}
