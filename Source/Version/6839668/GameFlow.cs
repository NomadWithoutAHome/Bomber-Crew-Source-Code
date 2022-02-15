using System;
using System.Collections;
using System.Collections.Generic;
using BomberCrewCommon;
using GameAnalyticsSDK;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using WingroveAudio;

public class GameFlow : Singleton<GameFlow>
{
	[SerializeField]
	private bool m_autoStartNewCrew;

	[SerializeField]
	private CurrentMission m_currentMission;

	[SerializeField]
	private string m_menuScene = "Memorial";

	[SerializeField]
	private bool m_loadToMenu;

	[SerializeField]
	private LoadingIndicator m_loadingTransform;

	[SerializeField]
	private LevelDescription m_initialMission;

	[SerializeField]
	private BombLoadConfig m_initialMissionBombLoadConfig;

	[SerializeField]
	private CampaignStructure m_mainCampaign;

	[SerializeField]
	private string[] m_equipmentPresetsInOrder;

	[SerializeField]
	private GameObject m_pauseMenuPrefab;

	[SerializeField]
	private GameMode m_defaultGameMode;

	[SerializeField]
	private Animator m_saveIconDisplayAnimator;

	[SerializeField]
	private TextSetter m_saveTextSetter;

	private bool m_isPaused;

	private int m_loadingCounter;

	private bool m_isInMission;

	private bool m_isLoopDemoMode;

	private GameMode m_currentGameMode;

	private float m_minShowDuration;

	private int m_numActiveSaves;

	private bool m_showingSave;

	private bool m_doneFirstLevelLoad;

	private string m_currentlyLoadedScene;

	private bool m_loadingTestActive;

	private int m_numLevels;

	private bool m_isLoadingLevel;

	public static bool Is30FPS()
	{
		return false;
	}

	private void Awake()
	{
		m_currentGameMode = m_defaultGameMode;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		if (!Screen.fullScreen)
		{
			float num = (float)Screen.width / (float)Screen.height;
			if (num < 1.58f)
			{
				Screen.SetResolution(Screen.width, (int)((float)Screen.width / 16f * 10f), fullscreen: false);
			}
		}
		m_minShowDuration = 0f;
		FileSystem.OnSaveStart += SaveStart;
		FileSystem.OnSaveFinish += SaveFinish;
		FileSystem.OnSaveReset += SaveReset;
	}

	public void SaveStart()
	{
		m_numActiveSaves++;
		m_minShowDuration = 3f;
	}

	public void SaveFinish()
	{
		m_numActiveSaves--;
		if (m_numActiveSaves < 0)
		{
			m_numActiveSaves = 0;
		}
	}

	public bool IsLoading()
	{
		return m_isLoadingLevel;
	}

	public void SaveReset()
	{
		m_numActiveSaves = 0;
		m_minShowDuration = 0f;
		Debug.Log("Num active saves: " + m_numActiveSaves);
	}

	private void Update()
	{
		bool flag = false;
		if (m_numActiveSaves > 0 || m_minShowDuration > 0f)
		{
			m_minShowDuration -= Time.unscaledDeltaTime;
			flag = true;
		}
		if (m_showingSave != flag)
		{
			m_showingSave = flag;
			if (flag)
			{
				m_saveTextSetter.SetTextFromLanguageString("consolesystem_saving");
				m_saveIconDisplayAnimator.gameObject.SetActive(value: true);
			}
			m_saveIconDisplayAnimator.SetBool("Show", flag);
		}
	}

	public bool RequiresEngagement()
	{
		return false;
	}

	public void SetGameMode(GameMode gm)
	{
		m_currentGameMode = gm;
	}

	public GameMode GetGameMode()
	{
		return m_currentGameMode;
	}

	public GameObject GetPauseMenuPrefab()
	{
		return m_pauseMenuPrefab;
	}

	public void SetPaused(bool paused)
	{
		m_isPaused = paused;
		if (m_isPaused)
		{
			if (WingroveRoot.Instance != null)
			{
				WingroveRoot.Instance.PostEvent("PAUSE_GAME");
			}
			Time.timeScale = 0f;
			return;
		}
		if (WingroveRoot.Instance != null)
		{
			WingroveRoot.Instance.PostEvent("UNPAUSE_GAME");
		}
		Time.timeScale = 1f;
		if (Singleton<MissionSpeedControls>.Instance != null)
		{
			Singleton<MissionSpeedControls>.Instance.SetToCurrent();
		}
	}

	public bool IsPaused()
	{
		return m_isPaused;
	}

	public CampaignStructure GetCampaign()
	{
		return m_currentGameMode.GetCampaign();
	}

	public void RegisterIsLoading()
	{
		m_loadingCounter++;
		m_loadingTransform.SetIsLoading(loading: true);
	}

	public void UnregisterIsLoading()
	{
		m_loadingCounter--;
		if (m_loadingCounter == 0)
		{
			m_loadingTransform.SetIsLoading(loading: false);
		}
	}

	public bool GetIsInMissionProbable()
	{
		return m_isInMission;
	}

	private IEnumerator Start()
	{
		if (Is30FPS())
		{
			Time.fixedDeltaTime = 1f / 30f;
			Time.maximumDeltaTime = 1f / 30f;
			Time.maximumParticleDeltaTime = 0.05f;
			Application.targetFrameRate = 30;
			QualitySettings.vSyncCount = 2;
		}
		else
		{
			Time.fixedDeltaTime = 1f / 60f;
		}
		RegisterIsLoading();
		yield return null;
		Resources.UnloadUnusedAssets();
		GC.Collect();
		AsyncOperation ao = SceneManager.LoadSceneAsync("AsyncLoadGameConstants");
		while (!ao.isDone)
		{
			yield return null;
		}
		Resources.UnloadUnusedAssets();
		GC.Collect();
		GameObject go = GameObject.Find("GameConstants");
		UnityEngine.Object.DontDestroyOnLoad(go);
		yield return null;
		while (!Singleton<SystemLoader>.Instance.IsLoadComplete())
		{
			yield return null;
		}
		yield return new WaitForSecondsRealtime(0.25f);
		UnregisterIsLoading();
		EditorLoadToMission eltm = UnityEngine.Object.FindObjectOfType<EditorLoadToMission>();
		if (eltm == null)
		{
			DoLoadLevel(m_menuScene, delegate
			{
				if (Singleton<SystemDataContainer>.Instance.Get().HasEverSetLanguage())
				{
					if (RequiresEngagement())
					{
						GameObject gameObject = Singleton<UIScreenManager>.Instance.ShowScreen("EngagementScreen", showNavBarButtons: true);
					}
					else
					{
						Singleton<UIScreenManager>.Instance.ShowScreen("StartScreen", showNavBarButtons: true);
					}
				}
				else if (RequiresEngagement())
				{
					GameObject gameObject2 = Singleton<UIScreenManager>.Instance.ShowScreen("EngagementScreen", showNavBarButtons: true);
				}
				else
				{
					Singleton<UIScreenManager>.Instance.ShowScreen("LanguageSelectionScreen", showNavBarButtons: true);
				}
				m_isInMission = false;
			});
			yield break;
		}
		CampaignStructure.CampaignMission campaignMission = null;
		CampaignStructure.CampaignMission[] allMissions = m_mainCampaign.GetAllMissions();
		foreach (CampaignStructure.CampaignMission campaignMission2 in allMissions)
		{
			if (campaignMission2.m_level == eltm.GetMissionToLoad())
			{
				campaignMission = campaignMission2;
			}
		}
		foreach (string currentInstalledDLC in Singleton<DLCManager>.Instance.GetCurrentInstalledDLCs())
		{
			DLCExpansionCampaign dLCExpansionCampaign = Singleton<DLCManager>.Instance.LoadAssetFromBundle<DLCExpansionCampaign>(currentInstalledDLC, "CAMPAIGN_" + currentInstalledDLC);
			if (!(dLCExpansionCampaign != null))
			{
				continue;
			}
			if (dLCExpansionCampaign.GetCampaign() != null)
			{
				CampaignStructure.CampaignMission[] allMissions2 = dLCExpansionCampaign.GetCampaign().GetAllMissions();
				foreach (CampaignStructure.CampaignMission campaignMission3 in allMissions2)
				{
					if (campaignMission3.m_level == eltm.GetMissionToLoad())
					{
						campaignMission = campaignMission3;
					}
				}
			}
			if (!(dLCExpansionCampaign.GetGameMode() != null) || !(dLCExpansionCampaign.GetGameMode().GetCampaign() != null))
			{
				continue;
			}
			CampaignStructure.CampaignMission[] allMissions3 = dLCExpansionCampaign.GetGameMode().GetCampaign().GetAllMissions();
			foreach (CampaignStructure.CampaignMission campaignMission4 in allMissions3)
			{
				if (campaignMission4.m_level == eltm.GetMissionToLoad())
				{
					campaignMission = campaignMission4;
				}
			}
		}
		if (campaignMission == null)
		{
			campaignMission = new CampaignStructure.CampaignMission();
			campaignMission.m_level = eltm.GetMissionToLoad();
			campaignMission.m_missionReferenceName = "EDITOR_MISSION";
		}
		Singleton<SaveDataContainer>.Instance.New(999, isCampaign: false, skippedTutorial: false);
		if (string.IsNullOrEmpty(Singleton<BomberContainer>.Instance.GetCurrentConfig().GetUpgradeFor("gun_turret_ventral")))
		{
			Singleton<BomberContainer>.Instance.GetCurrentConfig().SetUpgradeDebug("gun_turret_ventral", "GunTurret303x2Mk1");
		}
		Singleton<BomberContainer>.Instance.GetCurrentConfig().SetUpgradeDebug("rack_1", "EquipmentRack3");
		Singleton<BomberContainer>.Instance.GetCurrentConfig().SetUpgradeDetail("rack_1", "rack0", "PARACHUTE");
		Singleton<BomberContainer>.Instance.GetCurrentConfig().SetUpgradeDetail("rack_1", "rack1", "EXTINGUISHER");
		Singleton<BomberContainer>.Instance.GetCurrentConfig().SetUpgradeDetail("rack_1", "rack2", "FIRSTAID");
		Singleton<BomberContainer>.Instance.GetCurrentConfig().SetUpgradeDebug("rack_2", "EquipmentRack3");
		Singleton<BomberContainer>.Instance.GetCurrentConfig().SetUpgradeDetail("rack_2", "rack0", "PARACHUTE");
		Singleton<BomberContainer>.Instance.GetCurrentConfig().SetUpgradeDetail("rack_2", "rack1", "PARACHUTE");
		Singleton<BomberContainer>.Instance.GetCurrentConfig().SetUpgradeDetail("rack_2", "rack2", "FIRSTAID");
		Singleton<BomberContainer>.Instance.GetCurrentConfig().SetUpgradeDebug("rack_3", "EquipmentRack3");
		Singleton<BomberContainer>.Instance.GetCurrentConfig().SetUpgradeDetail("rack_3", "rack0", "PARACHUTE");
		Singleton<BomberContainer>.Instance.GetCurrentConfig().SetUpgradeDetail("rack_3", "rack1", "PARACHUTE");
		Singleton<BomberContainer>.Instance.GetCurrentConfig().SetUpgradeDetail("rack_3", "rack2", "FIRSTAID");
		Singleton<BomberContainer>.Instance.GetCurrentConfig().SetUpgradeDebug("rack_4", "EquipmentRack3");
		Singleton<BomberContainer>.Instance.GetCurrentConfig().SetUpgradeDetail("rack_4", "rack0", "PARACHUTE");
		Singleton<BomberContainer>.Instance.GetCurrentConfig().SetUpgradeDetail("rack_4", "rack1", "PARACHUTE");
		Singleton<BomberContainer>.Instance.GetCurrentConfig().SetUpgradeDetail("rack_4", "rack2", "EXTINGUISHER");
		for (int l = 0; l < m_currentGameMode.GetMaxCrew(); l++)
		{
			Singleton<CrewContainer>.Instance.HireNewCrewmanAuto();
		}
		m_isInMission = true;
		m_currentMission.StartNewMission(campaignMission);
		MissionLog.s_missionCompletedSimple = false;
		DoLoadLevel(m_currentGameMode.GetMissionScene(), null);
	}

	private void LoadingTest()
	{
		StartCoroutine(LoadingTestCo());
	}

	private IEnumerator LoadingTestCo()
	{
		m_loadingTestActive = true;
		StartNewGameSkip(0);
		yield return new WaitForSeconds(15f);
		while (true)
		{
			CampaignStructure.CampaignMission[] cm = GetCampaign().GetAllMissions();
			GoToMission(cm[UnityEngine.Random.Range(0, cm.Length)]);
			yield return new WaitForSeconds(25f);
			Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState().TakeOff();
			float t = 600f;
			while (t > 0f)
			{
				t -= Time.deltaTime;
				yield return null;
				if (Singleton<MissionCoordinator>.Instance == null)
				{
					break;
				}
			}
			if (Singleton<MissionCoordinator>.Instance != null)
			{
				ToDebrief();
			}
			yield return new WaitForSeconds(15f);
			DebriefingScreen dbs = UnityEngine.Object.FindObjectOfType<DebriefingScreen>();
			while (dbs != null && dbs.gameObject.activeInHierarchy)
			{
				dbs.TrySkipping();
				yield return new WaitForSeconds(5f);
				yield return null;
			}
			yield return new WaitForSeconds(5f);
			m_numLevels++;
		}
	}

	private void OnGUI()
	{
		if (m_loadingTestActive)
		{
			GUI.Label(new Rect(0f, 64f, 128f, 64f), "CYCLES: " + m_numLevels);
		}
	}

	public void GoToMission(CampaignStructure.CampaignMission campaignMission)
	{
		GoToMission(campaignMission, fromAirbase: true);
	}

	public void GoToMission(CampaignStructure.CampaignMission campaignMission, bool fromAirbase)
	{
		if (Singleton<SaveDataContainer>.Instance.Get().GetIsCampaign())
		{
			GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, campaignMission.m_missionReferenceName);
		}
		if (Singleton<SaveDataContainer>.Instance.Get().GetIsCampaign())
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["missionName"] = campaignMission.m_missionReferenceName;
			Analytics.CustomEvent("MissionStart", dictionary);
		}
		if (Singleton<SaveDataContainer>.Instance.Get().GetIsCampaign())
		{
			Singleton<SaveDataContainer>.Instance.Save();
			if (fromAirbase)
			{
				Singleton<AirbaseNavigation>.Instance.SaveCrewPhoto(instant: true);
			}
			Singleton<SystemDataContainer>.Instance.Save();
		}
		m_isInMission = true;
		m_currentMission.StartNewMission(campaignMission);
		MissionLog.s_missionCompletedSimple = false;
		DoLoadLevel(m_currentGameMode.GetMissionScene(), null);
	}

	public void StartNewGame(int saveIndex)
	{
		m_isLoopDemoMode = false;
		Singleton<SaveDataContainer>.Instance.New(saveIndex, isCampaign: true, skippedTutorial: false);
		GameAnalytics.SetCustomId(Singleton<SaveDataContainer>.Instance.Get().GetCampaignTrackingID());
		Analytics.SetUserId(Singleton<SaveDataContainer>.Instance.Get().GetCampaignTrackingID());
		Singleton<CrewContainer>.Instance.HireNewCrewman(new Crewman(new Crewman.Skill(Crewman.SpecialisationSkill.Piloting, 1), null));
		Singleton<CrewContainer>.Instance.HireNewCrewman(new Crewman(new Crewman.Skill(Crewman.SpecialisationSkill.RadioOp, 1), null));
		Singleton<CrewContainer>.Instance.HireNewCrewman(new Crewman(new Crewman.Skill(Crewman.SpecialisationSkill.Navigator, 1), null));
		Singleton<CrewContainer>.Instance.HireNewCrewman(new Crewman(new Crewman.Skill(Crewman.SpecialisationSkill.Gunning, 1), null));
		Singleton<CrewContainer>.Instance.GiveDefaultEquipment();
		Singleton<AchievementSystem>.Instance.GetUserStat("CampaignsStarted")?.ModifyValue(1);
		CampaignStructure.CampaignMission campaignMission = new CampaignStructure.CampaignMission();
		campaignMission.m_level = m_initialMission;
		campaignMission.m_missionReferenceName = "FIRST_MISSION";
		campaignMission.m_titleNamedText = "mission_first_mission_name";
		campaignMission.m_bombLoadConfig = m_initialMissionBombLoadConfig;
		campaignMission.m_totalMoneyReward = 1500;
		campaignMission.m_isTrainingMission = true;
		GoToMission(campaignMission, fromAirbase: false);
	}

	private IEnumerator DoRepooling()
	{
		AutoPool.RepoolAllToSize();
		Resources.UnloadUnusedAssets();
		GC.Collect();
		yield return new WaitForSecondsRealtime(0.25f);
		AsyncOperation aop = SceneManager.LoadSceneAsync("RepoolingScene");
		while (aop != null && !aop.isDone)
		{
			yield return null;
		}
		m_currentlyLoadedScene = "RepoolingScene";
		while (!Singleton<SystemLoader>.Instance.IsLoadComplete())
		{
			yield return null;
		}
		Resources.UnloadUnusedAssets();
		GC.Collect();
	}

	public void DoDemo()
	{
		m_isLoopDemoMode = true;
		Singleton<SaveDataContainer>.Instance.New(999, isCampaign: false, skippedTutorial: false);
		for (int i = 0; i < m_currentGameMode.GetMaxCrew(); i++)
		{
			Singleton<CrewContainer>.Instance.HireNewCrewmanAuto();
		}
		List<CrewmanEquipmentPreset> allPresets = Singleton<CrewmanGearCatalogueLoader>.Instance.GetCatalogue().GetAllPresets();
		for (int j = 0; j < m_currentGameMode.GetMaxCrew(); j++)
		{
			Crewman crewman = Singleton<CrewContainer>.Instance.GetCrewman(j);
			if (crewman.GetPrimarySkill().GetSkill() == Crewman.SpecialisationSkill.Piloting)
			{
				crewman.GetPrimarySkill().AddXP(35000);
			}
			else
			{
				crewman.GetPrimarySkill().AddXP(70000);
			}
			foreach (CrewmanEquipmentPreset item in allPresets)
			{
				if (item.name == m_equipmentPresetsInOrder[j])
				{
					CrewmanEquipmentBase[] allInPreset = item.GetAllInPreset();
					foreach (CrewmanEquipmentBase crewmanEquipmentBase in allInPreset)
					{
						crewman.SetEquippedFor(crewmanEquipmentBase.GetGearType(), crewmanEquipmentBase);
					}
				}
			}
		}
		Singleton<SaveDataContainer>.Instance.Get().AddIntel(8000);
		Singleton<SaveDataContainer>.Instance.Get().AddBalance(2250);
		Singleton<BomberContainer>.Instance.GetCurrentConfig().SetUpgradeDebug("fuselage_section_1", "FuselageArmouredMk1");
		Singleton<BomberContainer>.Instance.GetCurrentConfig().SetUpgradeDebug("fuselage_section_2", "FuselageArmouredMk1");
		Singleton<BomberContainer>.Instance.GetCurrentConfig().SetUpgradeDebug("fuselage_section_3", "FuselageArmouredMk1");
		Singleton<BomberContainer>.Instance.GetCurrentConfig().SetUpgradeDebug("fuselage_section_4", "FuselageArmouredMk1");
		Singleton<BomberContainer>.Instance.GetCurrentConfig().SetUpgradeDebug("fuselage_section_5", "FuselageArmouredMk1");
		Singleton<BomberContainer>.Instance.GetCurrentConfig().SetUpgradeDebug("fuselage_section_wings", "FuselageArmouredMk1");
		Singleton<BomberContainer>.Instance.GetCurrentConfig().SetUpgradeDebug("fuselage_section_bay_doors", "FuselageArmouredMk1");
		Singleton<BomberContainer>.Instance.GetCurrentConfig().SetUpgradeDebug("gun_turret_ventral", "GunTurret303x2Mk1");
		Singleton<BomberContainer>.Instance.GetCurrentConfig().SetUpgradeDebug("gun_turret_rear", "GunTurret303x4Mk3");
		Singleton<BomberContainer>.Instance.GetCurrentConfig().SetUpgradeDebug("radar_system", "RadarMk3");
		Singleton<BomberContainer>.Instance.GetCurrentConfig().SetUpgradeDebug("engine_1", "EngineStandardMk3");
		Singleton<BomberContainer>.Instance.GetCurrentConfig().SetUpgradeDebug("engine_2", "EngineStandardMk3");
		Singleton<BomberContainer>.Instance.GetCurrentConfig().SetUpgradeDebug("engine_3", "EngineStandardMk3");
		Singleton<BomberContainer>.Instance.GetCurrentConfig().SetUpgradeDebug("engine_4", "EngineStandardMk3");
		Singleton<BomberContainer>.Instance.GetCurrentConfig().SetUpgradeDebug("rack_1", "EquipmentRack3");
		Singleton<BomberContainer>.Instance.GetCurrentConfig().SetUpgradeDetail("rack_1", "rack0", "PARACHUTE");
		Singleton<BomberContainer>.Instance.GetCurrentConfig().SetUpgradeDetail("rack_1", "rack1", "EXTINGUISHER");
		Singleton<BomberContainer>.Instance.GetCurrentConfig().SetUpgradeDetail("rack_1", "rack2", "FIRSTAID");
		Singleton<BomberContainer>.Instance.GetCurrentConfig().SetUpgradeDebug("rack_2", "EquipmentRack3");
		Singleton<BomberContainer>.Instance.GetCurrentConfig().SetUpgradeDetail("rack_2", "rack0", "PARACHUTE");
		Singleton<BomberContainer>.Instance.GetCurrentConfig().SetUpgradeDetail("rack_2", "rack1", "PARACHUTE");
		Singleton<BomberContainer>.Instance.GetCurrentConfig().SetUpgradeDetail("rack_2", "rack2", "FIRSTAID");
		Singleton<BomberContainer>.Instance.GetCurrentConfig().SetUpgradeDebug("rack_3", "EquipmentRack3");
		Singleton<BomberContainer>.Instance.GetCurrentConfig().SetUpgradeDetail("rack_3", "rack0", "PARACHUTE");
		Singleton<BomberContainer>.Instance.GetCurrentConfig().SetUpgradeDetail("rack_3", "rack1", "PARACHUTE");
		Singleton<BomberContainer>.Instance.GetCurrentConfig().SetUpgradeDetail("rack_3", "rack2", "FIRSTAID");
		Singleton<BomberContainer>.Instance.GetCurrentConfig().SetUpgradeDebug("rack_4", "EquipmentRack3");
		Singleton<BomberContainer>.Instance.GetCurrentConfig().SetUpgradeDetail("rack_4", "rack0", "PARACHUTE");
		Singleton<BomberContainer>.Instance.GetCurrentConfig().SetUpgradeDetail("rack_4", "rack1", "PARACHUTE");
		Singleton<BomberContainer>.Instance.GetCurrentConfig().SetUpgradeDetail("rack_4", "rack2", "EXTINGUISHER");
		DoLoadLevel(m_currentGameMode.GetAirbaseScene(), delegate
		{
			Singleton<AirbaseNavigation>.Instance.GoToNormal();
		});
	}

	public void StartNewGameSkip(int saveIndex)
	{
		m_isLoopDemoMode = false;
		Singleton<SaveDataContainer>.Instance.New(saveIndex, isCampaign: true, skippedTutorial: true);
		GameAnalytics.SetCustomId(Singleton<SaveDataContainer>.Instance.Get().GetCampaignTrackingID());
		Analytics.SetUserId(Singleton<SaveDataContainer>.Instance.Get().GetCampaignTrackingID());
		Singleton<AchievementSystem>.Instance.GetUserStat("CampaignsStarted")?.ModifyValue(1);
		List<Crewman> list = new List<Crewman>();
		for (int i = 0; i < m_currentGameMode.GetMaxCrew(); i++)
		{
			Singleton<CrewContainer>.Instance.HireNewCrewmanAuto();
		}
		for (int j = 0; j < m_currentGameMode.GetMaxCrew(); j++)
		{
			list.Add(Singleton<CrewContainer>.Instance.GetCrewman(j));
		}
		Singleton<SaveDataContainer>.Instance.Get().AddBalance(m_currentGameMode.GetStartingMoney());
		Singleton<SaveDataContainer>.Instance.Get().AddIntel(m_currentGameMode.GetStartingIntel());
		if (GetGameMode().HasTutorial())
		{
			Singleton<SaveDataContainer>.Instance.Get().SetMissionPlayed("BombRunTraining", completed: true, saveCrewAndBomber: true, list);
		}
		int num = 0;
		for (int k = 0; k < GetGameMode().GetMaxCrew(); k++)
		{
			Crewman crewman = Singleton<CrewContainer>.Instance.GetCrewman(k);
			if (crewman.GetPrimarySkill().GetSkill() == Crewman.SpecialisationSkill.Gunning && num == 0)
			{
				Singleton<SaveDataContainer>.Instance.Get().GetCrewmanByIndex(k).GetPrimarySkill()
					.AddXP(1525);
				num++;
			}
			else if (crewman.GetPrimarySkill().GetSkill() == Crewman.SpecialisationSkill.Piloting || crewman.GetPrimarySkill().GetSkill() == Crewman.SpecialisationSkill.Navigator || crewman.GetPrimarySkill().GetSkill() == Crewman.SpecialisationSkill.RadioOp)
			{
				Singleton<SaveDataContainer>.Instance.Get().GetCrewmanByIndex(k).GetPrimarySkill()
					.AddXP(1525);
			}
			else
			{
				Singleton<SaveDataContainer>.Instance.Get().GetCrewmanByIndex(k).GetPrimarySkill()
					.AddXP(1000);
			}
		}
		Singleton<SaveDataContainer>.Instance.Save();
		DoLoadLevel(m_currentGameMode.GetAirbaseScene(), delegate
		{
			Singleton<AirbaseNavigation>.Instance.GoToNormal();
		});
		m_isInMission = false;
	}

	public bool LoadGame(int saveIndex)
	{
		m_isLoopDemoMode = false;
		if (Singleton<SaveDataContainer>.Instance.Load(saveIndex))
		{
			GameAnalytics.SetCustomId(Singleton<SaveDataContainer>.Instance.Get().GetCampaignTrackingID());
			Analytics.SetUserId(Singleton<SaveDataContainer>.Instance.Get().GetCampaignTrackingID());
			DoLoadLevel(m_currentGameMode.GetAirbaseScene(), delegate
			{
				Singleton<AirbaseNavigation>.Instance.GoToNormal();
			});
			m_isInMission = false;
			return true;
		}
		return false;
	}

	public void GoToAirbaseBasic()
	{
		DoLoadLevel(m_currentGameMode.GetAirbaseScene(), delegate
		{
			Singleton<AirbaseNavigation>.Instance.GoToNormal();
		}, 1.5f);
		m_isInMission = false;
	}

	public CurrentMission GetCurrentMissionInfo()
	{
		return m_currentMission;
	}

	public void ReturnToMainMenu()
	{
		m_isLoopDemoMode = false;
		WingroveRoot.Instance.PostEvent("MISSION_END");
		DoLoadLevel(m_menuScene, delegate
		{
			WingroveRoot.Instance.PostEvent("MISSION_END");
			Singleton<UIScreenManager>.Instance.ShowScreen("StartScreen", showNavBarButtons: true);
			m_isInMission = false;
		});
	}

	public void ResetToMainMenu()
	{
		m_isLoopDemoMode = false;
		WingroveRoot.Instance.PostEvent("MISSION_END");
		WingroveRoot.Instance.PostEvent("AIRBASE_LEAVE");
		WingroveRoot.Instance.PostEvent("MEMORIAL_LEAVE");
		DoLoadLevel(m_menuScene, delegate
		{
			WingroveRoot.Instance.PostEvent("AIRBASE_LEAVE");
			WingroveRoot.Instance.PostEvent("MISSION_END");
			Singleton<UIScreenManager>.Instance.ShowScreen("EngagementScreen", showNavBarButtons: true);
			m_isInMission = false;
		});
	}

	private void DoLoadLevel(string levelName, Action function, float preDelay = 0f)
	{
		StartCoroutine(LoadLevel(levelName, function, preDelay));
	}

	public IEnumerator LoadLevelBase(string levelName, bool additive)
	{
		Resources.UnloadUnusedAssets();
		GC.Collect();
		yield return new WaitForSecondsRealtime(0.5f);
		m_currentlyLoadedScene = levelName;
		AsyncOperation aso = SceneManager.LoadSceneAsync(levelName, additive ? LoadSceneMode.Additive : LoadSceneMode.Single);
		aso.allowSceneActivation = true;
		while (aso != null && !aso.isDone)
		{
			yield return null;
		}
		SetPaused(paused: false);
		Resources.UnloadUnusedAssets();
		GC.Collect();
	}

	private bool BlockButtons(MainActionButtonMonitor.ButtonPress bp)
	{
		return true;
	}

	private IEnumerator LoadLevel(string levelName, Action function, float preDelay)
	{
		while (m_isLoadingLevel)
		{
			yield return null;
		}
		m_isLoadingLevel = true;
		RegisterIsLoading();
		tk2dUICamera[] cams = UnityEngine.Object.FindObjectsOfType<tk2dUICamera>();
		tk2dUICamera[] array = cams;
		foreach (tk2dUICamera tk2dUICamera2 in array)
		{
			tk2dUICamera2.enabled = false;
		}
		if (Singleton<UISelector>.Instance != null)
		{
			Singleton<UISelector>.Instance.Pause();
		}
		if (Singleton<MainActionButtonMonitor>.Instance != null)
		{
			Singleton<MainActionButtonMonitor>.Instance.AddListener(BlockButtons);
		}
		while (!m_loadingTransform.IsFullyFaded())
		{
			yield return null;
		}
		yield return new WaitForSecondsRealtime(0.25f);
		yield return Resources.UnloadUnusedAssets();
		yield return null;
		GC.Collect();
		if (!levelName.Contains("Memorial") && m_doneFirstLevelLoad)
		{
			yield return StartCoroutine(DoRepooling());
		}
		m_doneFirstLevelLoad = true;
		yield return new WaitForSecondsRealtime(preDelay);
		yield return StartCoroutine(LoadLevelBase(levelName, additive: false));
		yield return new WaitForSecondsRealtime(0.25f);
		UnregisterIsLoading();
		function?.Invoke();
		if (Singleton<MainActionButtonMonitor>.Instance != null)
		{
			Singleton<MainActionButtonMonitor>.Instance.RemoveListener(BlockButtons, invalidateCurrentPress: false);
		}
		m_isLoadingLevel = false;
	}

	public void BriefingEnded()
	{
		if (m_isLoopDemoMode)
		{
			WingroveRoot.Instance.PostEvent("MISSION_END");
			DoLoadLevel("Endplate_Gamescom", delegate
			{
				m_isInMission = false;
			});
		}
	}

	public bool IsFaderFullyGone()
	{
		return m_loadingTransform.IsFullyInvisible();
	}

	public void EndSlateEnded()
	{
		DoLoadLevel(m_menuScene, delegate
		{
			WingroveRoot.Instance.PostEvent("MISSION_END");
			Singleton<UIScreenManager>.Instance.ShowScreen("StartScreenDemo", showNavBarButtons: true);
			m_isInMission = false;
		});
	}

	public bool IsDemoMode()
	{
		return m_isLoopDemoMode;
	}

	public void ToDebrief()
	{
		Time.timeScale = 1f;
		DoLoadLevel(m_currentGameMode.GetAirbaseScene(), delegate
		{
			WingroveRoot.Instance.PostEvent("MISSION_END");
			Singleton<AirbaseNavigation>.Instance.GoToDebrief();
			m_isInMission = false;
		});
	}
}
