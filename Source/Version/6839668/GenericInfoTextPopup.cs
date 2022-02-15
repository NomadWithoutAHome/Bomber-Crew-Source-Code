using BomberCrewCommon;
using UnityEngine;

public class GenericInfoTextPopup : MonoBehaviour
{
	[SerializeField]
	private tk2dUIItem m_closeButton;

	[SerializeField]
	private TextSetter m_textToSet;

	private void Start()
	{
		m_closeButton.OnClick += Close;
	}

	private void OnEnable()
	{
		Singleton<MainActionButtonMonitor>.Instance.AddListener(StartButtons);
	}

	private void OnDisable()
	{
		Singleton<MainActionButtonMonitor>.Instance.RemoveListener(StartButtons, invalidateCurrentPress: true);
	}

	private bool StartButtons(MainActionButtonMonitor.ButtonPress bp)
	{
		if (bp == MainActionButtonMonitor.ButtonPress.Confirm)
		{
			Close();
		}
		return true;
	}

	public void SetText(string namedText)
	{
		m_textToSet.SetTextFromLanguageString(namedText);
	}

	private void Close()
	{
		Singleton<UIPopupManager>.Instance.DismissCurrentPopup();
	}
}
