using System;
using BomberCrewCommon;
using Steamworks;
using UnityEngine;

public class RenameStringUsingPlatform : Singleton<RenameStringUsingPlatform>
{
	[SerializeField]
	private GameObject m_pcPopup;

	[SerializeField]
	private string m_allowedCharactersXbox;

	private Callback<GamepadTextInputDismissed_t> m_gamepadTextEntered;

	private Action<string> OnStringChangeLast;

	private Action OnCancelLast;

	public void Start()
	{
		m_gamepadTextEntered = Callback<GamepadTextInputDismissed_t>.Create(OnSteamKeyboardTextDismissed);
	}

	private void OnSteamKeyboardTextDismissed(GamepadTextInputDismissed_t callbackResult)
	{
		uint enteredGamepadTextLength = SteamUtils.GetEnteredGamepadTextLength();
		string pchText = null;
		if (SteamUtils.GetEnteredGamepadTextInput(out pchText, enteredGamepadTextLength))
		{
			Action<string> onStringChangeLast = OnStringChangeLast;
			OnCancelLast = null;
			OnStringChangeLast = null;
			onStringChangeLast?.Invoke(pchText);
		}
		else
		{
			Action onCancelLast = OnCancelLast;
			OnCancelLast = null;
			OnStringChangeLast = null;
			onCancelLast?.Invoke();
		}
	}

	public void SoftwareKeyboard(string startingText, string textReferenceTranslated, int maxLength, Action<string> OnStringChange, Action OnCancel)
	{
		OnCancelLast = OnCancel;
		OnStringChangeLast = OnStringChange;
		bool flag = SteamUtils.ShowGamepadTextInput(EGamepadTextInputMode.k_EGamepadTextInputModeNormal, EGamepadTextInputLineMode.k_EGamepadTextInputLineModeSingleLine, textReferenceTranslated, (uint)maxLength, startingText);
	}

	public void Rename(string startingText, string textReferenceTranslated, int maxLength, Action<string> OnStringChange, Action OnCancel)
	{
		UIPopupData uIPopupData = new UIPopupData();
		uIPopupData.PopupStartCallback = (Action<UIPopUp>)Delegate.Combine(uIPopupData.PopupStartCallback, (Action<UIPopUp>)delegate(UIPopUp pop)
		{
			RenameTextPopup component = pop.GetComponent<RenameTextPopup>();
			component.SetUp(startingText, textReferenceTranslated, maxLength);
			if (OnStringChange != null)
			{
				component.OnTextUpdate += OnStringChange;
			}
			if (OnCancel != null)
			{
				component.OnTextCancel += OnCancel;
			}
		});
		Singleton<UIPopupManager>.Instance.DisplayPopup(m_pcPopup, uIPopupData);
	}
}
