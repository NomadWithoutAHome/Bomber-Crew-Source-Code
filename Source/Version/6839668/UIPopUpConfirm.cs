using System;
using BomberCrewCommon;
using UnityEngine;

public class UIPopUpConfirm : MonoBehaviour
{
	[SerializeField]
	private TextSetter m_titleText;

	[SerializeField]
	private TextSetter m_messageText;

	[SerializeField]
	private tk2dUIItem m_yesButton;

	[SerializeField]
	private tk2dUIItem m_noButton;

	[SerializeField]
	private TextSetter m_yesButtonText;

	[SerializeField]
	private TextSetter m_noButtonText;

	[SerializeField]
	private GameObject m_yesButtonHierarchy;

	[SerializeField]
	private GameObject m_noButtonHierarchy;

	[SerializeField]
	private LayoutGrid m_buttonLayoutGrid;

	[SerializeField]
	private bool m_dontCloseOnEnd;

	[SerializeField]
	private UISelectFinder m_finder;

	private UISelectFinder m_previousFinder;

	private tk2dUIItem m_previouslyPointedAt;

	private bool m_dontChangeFinder;

	public event Action OnConfirm;

	public event Action OnCancel;

	private void Awake()
	{
		m_yesButton.OnClick += OnConfirmClicked;
		m_noButton.OnClick += OnCancelClicked;
	}

	private void OnEnable()
	{
		m_previousFinder = Singleton<UISelector>.Instance.GetCurrentFinder();
		m_previouslyPointedAt = Singleton<UISelector>.Instance.GetCurrentMovementType().GetCurrentlyPointedAtItem();
		Singleton<UISelector>.Instance.SetFinder(m_finder);
		Singleton<MainActionButtonMonitor>.Instance.AddListener(StartButtons);
	}

	public void DontChangeFinder()
	{
		m_dontChangeFinder = true;
	}

	private bool StartButtons(MainActionButtonMonitor.ButtonPress bp)
	{
		if (bp == MainActionButtonMonitor.ButtonPress.Back)
		{
			OnCancelClicked();
		}
		return true;
	}

	private void OnDisable()
	{
		Singleton<MainActionButtonMonitor>.Instance.RemoveListener(StartButtons, invalidateCurrentPress: true);
		if (!m_dontChangeFinder)
		{
			Singleton<UISelector>.Instance.SetFinder(m_previousFinder);
			if (m_previouslyPointedAt != null)
			{
				Singleton<UISelector>.Instance.ForcePointAt(m_previouslyPointedAt);
			}
		}
	}

	public void SetUp(string titleText, string messageText, string yesButtonText, string noButtonText)
	{
		m_titleText.SetText(titleText);
		m_messageText.SetText(messageText);
		m_yesButtonText.SetText(yesButtonText);
		m_noButtonText.SetText(noButtonText);
	}

	private void OnConfirmClicked()
	{
		if (this.OnConfirm != null)
		{
			this.OnConfirm();
		}
		if (!m_dontCloseOnEnd)
		{
			Singleton<UIPopupManager>.Instance.DismissCurrentPopup();
		}
	}

	private void OnCancelClicked()
	{
		if (this.OnCancel != null)
		{
			this.OnCancel();
		}
		if (!m_dontCloseOnEnd)
		{
			Singleton<UIPopupManager>.Instance.DismissCurrentPopup();
		}
	}
}
