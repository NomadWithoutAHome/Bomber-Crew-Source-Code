using System;
using System.Collections;
using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class StartScreen : MonoBehaviour
{
	[SerializeField]
	private tk2dUIItem m_campaignMenuButton;

	[SerializeField]
	private tk2dUIItem m_changeLanguageButton;

	[SerializeField]
	private tk2dUIItem m_quitGameButton;

	[SerializeField]
	private AirbaseCameraNode m_thisCameraNode;

	[SerializeField]
	private SlotSelectScreen m_slotSelectionScreen;

	[SerializeField]
	private UISelectFinder m_finder;

	[SerializeField]
	private GameMode m_gameModeStandard;

	[SerializeField]
	private GameMode m_gameModeEndless;

	[SerializeField]
	private tk2dUIItem m_endlessModeButton;

	[SerializeField]
	private GameObject m_disableOnConsole;

	[SerializeField]
	private Transform m_dlcDisplay;

	[SerializeField]
	private Vector3 m_dlcDisplayPositionConsole;

	[SerializeField]
	private TextSetter m_gamerTagSet;

	[SerializeField]
	private GameObject m_gamerTagDisplay;

	[SerializeField]
	private tk2dUIItem m_changeProfileButton;

	[SerializeField]
	private LayoutGrid[] m_buttonLayoutGrids;

	[SerializeField]
	private GameObject m_challengeModePackPopup;

	[SerializeField]
	private GameObject m_campaignModePackPopup;

	[SerializeField]
	private GameObject m_notReadyHierarchy;

	[SerializeField]
	private GameObject m_confirmYesNoPopup;

	private bool m_hasShownDLCOutOfDateError;

	private void Awake()
	{
		m_campaignMenuButton.OnClick += GoToCampaignMenu;
		m_quitGameButton.OnClick += Quit;
		m_changeLanguageButton.OnClick += ChangeLanguage;
		m_endlessModeButton.OnClick += GoToEndless;
		Singleton<GameFlow>.Instance.SetGameMode(m_gameModeStandard);
		m_changeProfileButton.gameObject.SetActive(value: false);
		LayoutGrid[] buttonLayoutGrids = m_buttonLayoutGrids;
		foreach (LayoutGrid layoutGrid in buttonLayoutGrids)
		{
			layoutGrid.RepositionChildren();
		}
		if (!FileSystem.IsReady())
		{
			m_notReadyHierarchy.SetActive(value: false);
			StartCoroutine(WaitForFileSystemReady());
		}
	}

	private IEnumerator WaitForFileSystemReady()
	{
		while (!FileSystem.IsReady())
		{
			yield return null;
		}
		m_notReadyHierarchy.SetActive(value: true);
	}

	private void Quit()
	{
		Application.Quit();
	}

	private void GoToCampaignMenu()
	{
		Singleton<GameFlow>.Instance.SetGameMode(m_gameModeStandard);
		bool hasAnySaves = Singleton<SaveDataContainer>.Instance.Exists();
		UIPopupData uIPopupData = new UIPopupData();
		uIPopupData.PopupStartCallback = (Action<UIPopUp>)Delegate.Combine(uIPopupData.PopupStartCallback, (Action<UIPopUp>)delegate(UIPopUp uip)
		{
			uip.GetComponent<CampaignModePackSelect>().OnConfirm += delegate
			{
				if (hasAnySaves)
				{
					Singleton<UIScreenManager>.Instance.ShowScreen("CampaignMenuScreen", showNavBarButtons: true);
				}
				else if (Singleton<GameFlow>.Instance.GetGameMode() != m_gameModeStandard)
				{
					DoNoTutorialConfirm();
				}
				else
				{
					Singleton<UIScreenManager>.Instance.ShowScreen("CampaignMenuScreen", showNavBarButtons: true);
				}
			};
		});
		Singleton<UIPopupManager>.Instance.DisplayPopup(m_campaignModePackPopup, uIPopupData);
	}

	private void DoNoTutorialConfirm()
	{
		UIPopupData uIPopupData = new UIPopupData();
		uIPopupData.PopupStartCallback = (Action<UIPopUp>)Delegate.Combine(uIPopupData.PopupStartCallback, (Action<UIPopUp>)delegate(UIPopUp uip)
		{
			uip.GetComponent<UIPopUpConfirm>().SetUp(Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("ui_briefing_dlc_difficulty_warning_title"), Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("ui_briefing_dlc_no_tutorial_warning"), Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("ui_briefing_dlc_difficulty_warning_yes"), Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("ui_briefing_dlc_difficulty_warning_no"));
			uip.GetComponent<UIPopUpConfirm>().OnConfirm += delegate
			{
				uip.GetComponent<UIPopUpConfirm>().DontChangeFinder();
				Singleton<UIScreenManager>.Instance.ShowScreen("CampaignMenuScreen", showNavBarButtons: true);
			};
			uip.GetComponent<UIPopUpConfirm>().OnCancel += delegate
			{
				GoToCampaignMenu();
			};
		});
		Singleton<UIPopupManager>.Instance.DisplayPopup(m_confirmYesNoPopup, uIPopupData);
	}

	private void GoToEndless()
	{
		UIPopupData uIPopupData = new UIPopupData();
		uIPopupData.PopupStartCallback = (Action<UIPopUp>)Delegate.Combine(uIPopupData.PopupStartCallback, (Action<UIPopUp>)delegate(UIPopUp uip)
		{
			uip.GetComponent<EndlessModePackSelect>().OnConfirm += delegate
			{
				Singleton<GameFlow>.Instance.GoToAirbaseBasic();
			};
		});
		Singleton<UIPopupManager>.Instance.DisplayPopup(m_challengeModePackPopup, uIPopupData);
	}

	private void ChangeLanguage()
	{
		Singleton<MainActionButtonMonitor>.Instance.InvalidateCurrentFrame();
		Singleton<UIScreenManager>.Instance.ShowScreen("LanguageSelectionScreen", showNavBarButtons: true);
	}

	private void DoDemo()
	{
		Singleton<GameFlow>.Instance.SetGameMode(m_gameModeStandard);
		Singleton<UIScreenManager>.Instance.ShowScreen("StartScreenDemo", showNavBarButtons: true);
	}

	private void OnEnable()
	{
		Singleton<AirbaseCameraController>.Instance.MoveCameraToLocation(m_thisCameraNode);
		Singleton<UISelector>.Instance.SetFinder(m_finder);
	}

	private void OnDisable()
	{
	}

	private void FillSaves()
	{
		List<Crewman> list = new List<Crewman>();
		for (int i = 0; i < 1000; i++)
		{
			Crewman crewman = new Crewman(new Crewman.Skill(Crewman.SpecialisationSkill.BombAiming, 1), new Crewman.Skill(Crewman.SpecialisationSkill.Engineer, 1));
			Singleton<CrewContainer>.Instance.SetDefaultEquipmentTo(crewman);
			Singleton<SystemDataContainer>.Instance.Get().RegisterCrewmanLost(crewman);
			if (list.Count < 7)
			{
				list.Add(crewman);
			}
		}
		Singleton<SystemDataContainer>.Instance.Get().SetMemorialCrew(list);
		Singleton<SystemDataContainer>.Instance.Save();
	}

	private void Update()
	{
		Singleton<DLCManager>.Instance.DoContentLoad();
	}
}
