using BomberCrewCommon;
using UnityEngine;

public class PauseMenuOptionsPage : MonoBehaviour
{
	[SerializeField]
	private PanelToggleButton m_musicButton;

	[SerializeField]
	private GameObject m_mainHierachy;

	[SerializeField]
	private GameObject m_quitConfirmHierarchy;

	[SerializeField]
	private UIPopUpConfirm m_confirmQuitPopup;

	[SerializeField]
	private tk2dUIItem m_quitButton;

	[SerializeField]
	private SliderBar m_sliderVolumeMusic;

	[SerializeField]
	private SliderBar m_sliderVolumeEffects;

	[SerializeField]
	private UISelectFinder m_finder;

	[SerializeField]
	private UISelectorMovementType m_movementType;

	private float m_lastMusicVolume;

	private void OnEnable()
	{
		Singleton<UISelector>.Instance.SetFinder(m_finder);
	}

	private void Start()
	{
		m_musicButton.OnClick += ToggleMusic;
		m_musicButton.SetState(Singleton<SystemDataContainer>.Instance.Get().GetMusic());
		m_quitButton.OnClick += QuitConfirm;
		if (Singleton<MissionCoordinator>.Instance != null)
		{
			m_confirmQuitPopup.SetUp(Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("ui_pause_menu_quit_title"), Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("ui_pause_menu_quit_confirm_mission"), Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("ui_pause_menu_quit_yes"), Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("ui_pause_menu_quit_no"));
		}
		else
		{
			m_confirmQuitPopup.SetUp(Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("ui_pause_menu_quit_title"), Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("ui_pause_menu_quit_confirm"), Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("ui_pause_menu_quit_yes_save"), Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("ui_pause_menu_quit_no"));
		}
		m_confirmQuitPopup.OnCancel += ReturnToNormal;
		m_confirmQuitPopup.OnConfirm += ConfirmQuit;
		m_sliderVolumeEffects.SetValue(Singleton<SystemDataContainer>.Instance.Get().GetSFXVolume());
		m_sliderVolumeMusic.SetValue(Singleton<SystemDataContainer>.Instance.Get().GetMusicVolume());
		m_lastMusicVolume = Singleton<SystemDataContainer>.Instance.Get().GetMusicVolume();
	}

	private void ReturnToNormal()
	{
		m_mainHierachy.SetActive(value: true);
		m_quitConfirmHierarchy.SetActive(value: false);
	}

	private void ConfirmQuit()
	{
		if (Singleton<MissionCoordinator>.Instance == null)
		{
			Singleton<SaveDataContainer>.Instance.Save();
			Singleton<AirbaseNavigation>.Instance.SaveCrewPhoto(instant: true);
		}
		Singleton<SystemDataContainer>.Instance.Save();
		Singleton<UIPopupManager>.Instance.DismissCurrentPopup();
		Singleton<GameFlow>.Instance.ReturnToMainMenu();
	}

	private void QuitConfirm()
	{
		m_mainHierachy.SetActive(value: false);
		m_quitConfirmHierarchy.SetActive(value: true);
	}

	private void Update()
	{
		Singleton<SystemDataContainer>.Instance.Get().SetSFXVolume(m_sliderVolumeEffects.GetValue());
		Singleton<SystemDataContainer>.Instance.Get().SetMusicVolume(m_sliderVolumeMusic.GetValue());
		if (m_sliderVolumeMusic.GetValue() > m_lastMusicVolume && !Singleton<SystemDataContainer>.Instance.Get().GetMusic())
		{
			ToggleMusic();
		}
		if (m_sliderVolumeMusic.GetValue() == 0f && Singleton<SystemDataContainer>.Instance.Get().GetMusic())
		{
			ToggleMusic();
		}
	}

	private void ToggleMusic()
	{
		Singleton<SystemDataContainer>.Instance.Get().SetMusic(!Singleton<SystemDataContainer>.Instance.Get().GetMusic());
		m_musicButton.SetState(Singleton<SystemDataContainer>.Instance.Get().GetMusic());
		if (Singleton<SystemDataContainer>.Instance.Get().GetMusic() && Singleton<SystemDataContainer>.Instance.Get().GetMusicVolume() < 0.1f)
		{
			Singleton<SystemDataContainer>.Instance.Get().SetMusicVolume(0.5f);
			m_sliderVolumeMusic.SetValue(Singleton<SystemDataContainer>.Instance.Get().GetMusicVolume());
		}
		m_lastMusicVolume = Singleton<SystemDataContainer>.Instance.Get().GetMusicVolume();
		Singleton<SystemDataContainer>.Instance.Save();
	}
}
