using System;
using BomberCrewCommon;
using UnityEngine;

public class RenameTextPopup : MonoBehaviour
{
	[SerializeField]
	private tk2dUITextInput m_textInputField;

	[SerializeField]
	private tk2dUIItem m_okButton;

	[SerializeField]
	private tk2dUIItem m_cancelButton;

	[SerializeField]
	private TextSetter m_headerText;

	[SerializeField]
	private TextSetter m_lengthText;

	[SerializeField]
	private GameObject m_badLengthDisable;

	[SerializeField]
	private GameObject m_badLengthEnable;

	[SerializeField]
	private Color m_badLengthColor;

	[SerializeField]
	private Color m_goodLengthColor;

	[SerializeField]
	private UISelectFinder m_finder;

	private UISelectFinder m_uiSelectFinderPrevious;

	public event Action<string> OnTextUpdate;

	public event Action OnTextCancel;

	public void SetUp(string startingText, string headerText, int maxLength)
	{
		m_headerText.SetText(headerText);
		m_okButton.OnClick += ConfirmText;
		m_cancelButton.OnClick += Cancel;
		m_textInputField.maxCharacterLength = maxLength;
		m_textInputField.Text = startingText;
		m_textInputField.SetFocus(focus: true);
		Refresh();
	}

	private void OnEnable()
	{
		RewiredKeyboardDisable.SetKeyboardDisable(disable: true);
		if (m_uiSelectFinderPrevious == null)
		{
			m_uiSelectFinderPrevious = Singleton<UISelector>.Instance.GetCurrentFinder();
		}
		Singleton<UISelector>.Instance.SetFinder(m_finder);
		Singleton<MainActionButtonMonitor>.Instance.AddListener(MainActionBlock);
	}

	private bool MainActionBlock(MainActionButtonMonitor.ButtonPress bp)
	{
		return true;
	}

	private void OnDisable()
	{
		RewiredKeyboardDisable.SetKeyboardDisable(disable: false);
		Singleton<MainActionButtonMonitor>.Instance.RemoveListener(MainActionBlock, invalidateCurrentPress: true);
	}

	private void Refresh()
	{
		m_lengthText.SetText($"{m_textInputField.Text.Length} / {m_textInputField.maxCharacterLength}");
		if (m_textInputField.Text.Length == 0)
		{
			m_badLengthDisable.SetActive(value: false);
			m_badLengthEnable.SetActive(value: true);
			m_lengthText.SetColor(m_badLengthColor);
		}
		else
		{
			m_badLengthDisable.SetActive(value: true);
			m_badLengthEnable.SetActive(value: false);
			m_lengthText.SetColor(m_goodLengthColor);
		}
	}

	private void Update()
	{
		Refresh();
		if (Input.GetKeyUp(KeyCode.Return))
		{
			ConfirmText();
		}
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			Cancel();
		}
	}

	public void ConfirmText()
	{
		if (m_textInputField.Text.Length > 0)
		{
			if (this.OnTextUpdate != null)
			{
				this.OnTextUpdate(m_textInputField.Text);
			}
			Singleton<UISelector>.Instance.SetFinder(m_uiSelectFinderPrevious);
			Singleton<UIPopupManager>.Instance.DismissCurrentPopup();
		}
	}

	public void Cancel()
	{
		if (this.OnTextCancel != null)
		{
			this.OnTextCancel();
		}
		Singleton<UISelector>.Instance.SetFinder(m_uiSelectFinderPrevious);
		Singleton<UIPopupManager>.Instance.DismissCurrentPopup();
	}
}
