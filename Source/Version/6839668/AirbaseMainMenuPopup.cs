using BomberCrewCommon;
using UnityEngine;

public class AirbaseMainMenuPopup : MonoBehaviour
{
	[SerializeField]
	private tk2dUIItem m_quitButton;

	[SerializeField]
	private tk2dUIItem m_dismissButton;

	[SerializeField]
	private tk2dUIItem m_saveButton;

	private void Awake()
	{
		m_quitButton.OnClick += OnQuitClick;
		m_dismissButton.OnClick += OnDismissClick;
		m_saveButton.OnClick += OnSaveClick;
	}

	private void OnQuitClick()
	{
		Singleton<UIPopupManager>.Instance.DismissCurrentPopup();
		Singleton<GameFlow>.Instance.ReturnToMainMenu();
	}

	private void OnDismissClick()
	{
		Singleton<UIPopupManager>.Instance.DismissCurrentPopup();
	}

	private void OnSaveClick()
	{
		Singleton<SystemDataContainer>.Instance.Save();
		Singleton<SaveDataContainer>.Instance.Save();
		Singleton<UIPopupManager>.Instance.DismissCurrentPopup();
	}
}
