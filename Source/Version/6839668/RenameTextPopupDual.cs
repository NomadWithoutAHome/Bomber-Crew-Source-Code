using System;
using BomberCrewCommon;
using UnityEngine;

public class RenameTextPopupDual : MonoBehaviour
{
	[SerializeField]
	private tk2dUITextInput m_textInputFieldFirst;

	[SerializeField]
	private tk2dUITextInput m_textInputFieldSecond;

	[SerializeField]
	private tk2dUIItem m_okButton;

	[SerializeField]
	private tk2dUIItem m_cancelButton;

	[SerializeField]
	private TextSetter m_headerText;

	[SerializeField]
	private TextSetter m_lengthTextFirst;

	[SerializeField]
	private TextSetter m_lengthTextSecond;

	[SerializeField]
	private GameObject m_badLengthDisable;

	[SerializeField]
	private GameObject m_badLengthEnable;

	[SerializeField]
	private Color m_badLengthColor;

	[SerializeField]
	private Color m_goodLengthColor;

	public event Action<string, string> OnTextUpdate;

	public event Action OnTextCancel;

	public void SetUp(string startingTextFirst, string startingTextSecond, string headerText, int maxLength)
	{
		m_headerText.SetText(headerText);
		m_okButton.OnClick += ConfirmText;
		m_cancelButton.OnClick += Cancel;
		m_textInputFieldFirst.maxCharacterLength = maxLength;
		m_textInputFieldFirst.Text = startingTextFirst;
		m_textInputFieldFirst.SetFocus(focus: true);
		m_textInputFieldSecond.maxCharacterLength = maxLength;
		m_textInputFieldSecond.Text = startingTextSecond;
		Refresh();
	}

	private void OnEnable()
	{
		RewiredKeyboardDisable.SetKeyboardDisable(disable: true);
	}

	private void OnDisable()
	{
		RewiredKeyboardDisable.SetKeyboardDisable(disable: false);
	}

	private void Refresh()
	{
		m_lengthTextFirst.SetText($"{m_textInputFieldFirst.Text.Length} / {m_textInputFieldFirst.maxCharacterLength}");
		m_lengthTextSecond.SetText($"{m_textInputFieldSecond.Text.Length} / {m_textInputFieldSecond.maxCharacterLength}");
		bool flag = false;
		if (m_textInputFieldFirst.Text.Length == 0)
		{
			flag = true;
			m_lengthTextFirst.SetColor(m_badLengthColor);
		}
		else
		{
			m_lengthTextFirst.SetColor(m_goodLengthColor);
		}
		if (m_textInputFieldSecond.Text.Length == 0)
		{
			flag = true;
			m_lengthTextSecond.SetColor(m_badLengthColor);
		}
		else
		{
			m_lengthTextSecond.SetColor(m_goodLengthColor);
		}
		m_badLengthDisable.SetActive(!flag);
		m_badLengthEnable.SetActive(flag);
	}

	private void Update()
	{
		Refresh();
		if (Input.GetKeyUp(KeyCode.Return))
		{
			if (m_textInputFieldFirst.IsFocus)
			{
				m_textInputFieldFirst.SetFocus(focus: false);
				m_textInputFieldSecond.SetFocus(focus: true);
			}
			else if (m_textInputFieldSecond.IsFocus)
			{
				ConfirmText();
			}
		}
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			Cancel();
		}
	}

	public void ConfirmText()
	{
		if (m_textInputFieldFirst.Text.Length > 0 && m_textInputFieldSecond.Text.Length > 0)
		{
			if (this.OnTextUpdate != null)
			{
				this.OnTextUpdate(m_textInputFieldFirst.Text, m_textInputFieldSecond.Text);
			}
			Singleton<UIPopupManager>.Instance.DismissCurrentPopup();
		}
	}

	public void Cancel()
	{
		if (this.OnTextCancel != null)
		{
			this.OnTextCancel();
		}
		Singleton<UIPopupManager>.Instance.DismissCurrentPopup();
	}
}
