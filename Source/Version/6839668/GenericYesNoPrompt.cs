using System;
using BomberCrewCommon;
using UnityEngine;

public class GenericYesNoPrompt : MonoBehaviour
{
	[SerializeField]
	private tk2dUIItem m_overwriteButton;

	[SerializeField]
	private tk2dUIItem m_cancelButton;

	[SerializeField]
	private TextSetter m_yesButtonText;

	[SerializeField]
	private TextSetter m_noButtonText;

	[SerializeField]
	private TextSetter m_mainTextText;

	[SerializeField]
	private GameObject[] m_normalModeColors;

	[SerializeField]
	private GameObject[] m_redDangerModeColors;

	[SerializeField]
	private UISelectFinder m_finder;

	private UISelectFinder m_previousFinder;

	private tk2dUIItem m_previouslyPointedAt;

	public event Action OnYes;

	public event Action OnNo;

	public void SetUp(string mainText, string yesButton, string noButton, bool danger)
	{
		m_yesButtonText.SetText(yesButton);
		m_noButtonText.SetText(noButton);
		m_mainTextText.SetText(mainText);
		GameObject[] normalModeColors = m_normalModeColors;
		foreach (GameObject gameObject in normalModeColors)
		{
			gameObject.SetActive(!danger);
		}
		GameObject[] redDangerModeColors = m_redDangerModeColors;
		foreach (GameObject gameObject2 in redDangerModeColors)
		{
			gameObject2.SetActive(danger);
		}
	}

	private void OnEnable()
	{
		Singleton<MainActionButtonMonitor>.Instance.AddListener(StartButtons);
		m_previousFinder = Singleton<UISelector>.Instance.GetCurrentFinder();
		m_previouslyPointedAt = Singleton<UISelector>.Instance.GetCurrentMovementType().GetCurrentlyPointedAtItem();
		Singleton<UISelector>.Instance.SetFinder(m_finder);
	}

	private void OnDisable()
	{
		Singleton<MainActionButtonMonitor>.Instance.RemoveListener(StartButtons, invalidateCurrentPress: true);
		Singleton<UISelector>.Instance.SetFinder(m_previousFinder);
		if (m_previouslyPointedAt != null)
		{
			Singleton<UISelector>.Instance.ForcePointAt(m_previouslyPointedAt);
		}
	}

	private bool StartButtons(MainActionButtonMonitor.ButtonPress bp)
	{
		if (bp == MainActionButtonMonitor.ButtonPress.Back)
		{
			Cancel();
		}
		return true;
	}

	private void Start()
	{
		m_overwriteButton.OnClick += Overwrite;
		m_cancelButton.OnClick += Cancel;
	}

	private void Overwrite()
	{
		Singleton<UIPopupManager>.Instance.DismissCurrentPopup();
		if (this.OnYes != null)
		{
			this.OnYes();
		}
	}

	private void Cancel()
	{
		Singleton<UIPopupManager>.Instance.DismissCurrentPopup();
		if (this.OnNo != null)
		{
			this.OnNo();
		}
	}
}
