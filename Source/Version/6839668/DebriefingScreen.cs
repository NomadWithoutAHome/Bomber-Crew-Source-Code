using System;
using System.Collections;
using System.Collections.Generic;
using BomberCrewCommon;
using Common;
using GameAnalyticsSDK;
using UnityEngine;
using UnityEngine.Analytics;
using WingroveAudio;

public class DebriefingScreen : MonoBehaviour
{
	[Serializable]
	public class GearTypeToNamedText
	{
		[SerializeField]
		public CrewmanGearType m_gearType;

		[NamedText]
		[SerializeField]
		public string m_namedText;
	}

	[Serializable]
	public class BomberUpgradeTypeToNamedText
	{
		[SerializeField]
		public BomberUpgradeType m_upgradeType;

		[NamedText]
		[SerializeField]
		public string m_namedText;
	}

	[SerializeField]
	private GameObject m_crewmanSummaryPanel;

	[SerializeField]
	private LayoutGrid m_layoutGrid;

	[SerializeField]
	private GameObject m_finishedHierarchy;

	[SerializeField]
	private tk2dUIItem m_exitDebriefButton;

	[SerializeField]
	private GameObject m_continueHierarchy;

	[SerializeField]
	private tk2dUIItem m_continueButton;

	[SerializeField]
	private GameObject m_missionResultHierarchy;

	[SerializeField]
	private GameObject m_aceEncounterHierarchy;

	[SerializeField]
	private GameObject m_crewResultHierarchy;

	[SerializeField]
	private GameObject m_bomberStateHierarchy;

	[SerializeField]
	private Renderer m_bomberPortraitRender;

	[SerializeField]
	private GameObject m_bomberSafeHierarchy;

	[SerializeField]
	private GameObject m_bomberDestroyedHierarchy;

	[SerializeField]
	private GameObject m_bomberLostHierarchy;

	[SerializeField]
	private GameObject m_bomberRecoverableHierarchy;

	[SerializeField]
	private GameObject m_bomberRecoveredHierarchy;

	[SerializeField]
	private TextSetter m_bomberRecoveryCostText;

	[SerializeField]
	[NamedText]
	private string m_bomberRecoveryCostFormat;

	[SerializeField]
	private tk2dUIItem m_recoverButton;

	[SerializeField]
	private tk2dUIItem m_dontRecoverButton;

	[SerializeField]
	private TextSetter m_photosTakenCounter;

	[SerializeField]
	private AceEncounterResultDisplay m_aceResultDisplayer;

	[SerializeField]
	[AudioEventName]
	private string m_bomberLostAudioEvent;

	[SerializeField]
	[AudioEventName]
	private string m_bomberReturnedAudioEvent;

	[SerializeField]
	[AudioEventName]
	private string m_objectiveWonAudioEvent;

	[SerializeField]
	[AudioEventName]
	private string m_objectiveFailedAudioEvent;

	[SerializeField]
	[AudioEventName]
	private string m_photoTakenAudioEvent;

	[SerializeField]
	private UISelectFinder m_mainFinder;

	[SerializeField]
	private GameObject m_targetDestroyMissionObjectivePrefab;

	[SerializeField]
	private LayoutGrid m_missionObjectiveLayout;

	[SerializeField]
	private GameObject m_missionIsTutorialLabel;

	[SerializeField]
	private GameObject m_newSkillsUnlockScreen;

	[SerializeField]
	private LayoutGrid m_newSkillsLayoutGrid;

	[SerializeField]
	private GameObject m_newSkillsPrefab;

	[SerializeField]
	private GameObject m_newUnlocksUnlockScreen;

	[SerializeField]
	private LayoutGrid m_newUnlocksLayoutGrid;

	[SerializeField]
	private GameObject m_newUnlocksPrefab;

	[SerializeField]
	private GameObject m_miaCalculator;

	[SerializeField]
	private AirbaseCameraNode m_initialCamera;

	[SerializeField]
	private AirbaseCameraNode m_bomberStateCamera;

	[SerializeField]
	private BomberUpgradeHangarBomberPositionController m_bomberController;

	[SerializeField]
	private AirbaseCameraNode m_crewmanStateCamera;

	[SerializeField]
	private AirbaseCameraNode m_endCamera;

	[SerializeField]
	private AirbaseCameraNode m_aceCamera;

	[SerializeField]
	private AirbasePersistentCrew m_persistentCrew;

	[SerializeField]
	private AirbasePersistentCrewTarget m_persistentCrewTargets;

	[SerializeField]
	private EnemyFighterAcesBoardPersistent m_aceBoardPersistent;

	[SerializeField]
	private PerkDisplay m_perkDisplay;

	[SerializeField]
	private TextSetter m_bomberName;

	[SerializeField]
	private CampaignProgressIndicator m_campaignProgress;

	[SerializeField]
	private EquipmentUpgradeFittableBase.AircraftUpgradeType m_aircraftUpgradeFilter;

	[SerializeField]
	private GearTypeToNamedText[] m_gearToNamedText;

	[SerializeField]
	private BomberUpgradeTypeToNamedText[] m_upgradeToNamedText;

	[SerializeField]
	private GameCompleteSequence m_gameCompleteSequence;

	[SerializeField]
	private GameObject m_skipHierarchy;

	[SerializeField]
	private tk2dUIItem m_skipButton;

	private List<DebriefingCrewmanPanel> m_panels = new List<DebriefingCrewmanPanel>();

	private List<GameObject> m_createdObjects = new List<GameObject>();

	private bool m_recoverButtonPressed;

	private bool m_dontRecoverButtonPressed;

	private bool m_continueButtonPresed;

	private bool m_shouldResetToNewBomber;

	private bool m_gotLandAtBaseBonus;

	private int m_numRescued;

	private bool m_shouldFastForward;

	private bool m_skipAllowed;

	private Dictionary<Crewman.SpecialisationSkill, List<CrewmanSkillAbilityBool>> m_unlockedSkills = new Dictionary<Crewman.SpecialisationSkill, List<CrewmanSkillAbilityBool>>();

	public void TrySkipping()
	{
		if (m_skipAllowed)
		{
			SetSkipEnabled();
		}
		else if (m_continueHierarchy.activeInHierarchy)
		{
			m_continueButton.SimulateClick();
		}
		else if (m_finishedHierarchy.activeInHierarchy)
		{
			m_exitDebriefButton.SimulateClick();
		}
	}

	private void SetSkipAllowed(bool allowed)
	{
		if (m_skipAllowed != allowed)
		{
			m_shouldFastForward = false;
			m_skipAllowed = allowed;
			m_skipHierarchy.CustomActivate(allowed);
		}
	}

	private void SetSkipEnabled()
	{
		if (m_skipAllowed && !m_shouldFastForward)
		{
			m_shouldFastForward = true;
			m_skipHierarchy.CustomActivate(active: false);
		}
	}

	private void Awake()
	{
		m_exitDebriefButton.OnClick += OnExitClick;
		m_recoverButton.OnClick += delegate
		{
			m_recoverButtonPressed = true;
		};
		m_dontRecoverButton.OnClick += delegate
		{
			m_dontRecoverButtonPressed = true;
		};
		m_continueButton.OnClick += delegate
		{
			m_continueButtonPresed = true;
		};
		m_shouldResetToNewBomber = false;
		m_numRescued = 0;
		m_shouldFastForward = false;
		m_skipButton.OnClick += SetSkipEnabled;
	}

	public bool ShouldFastForward()
	{
		return m_shouldFastForward;
	}

	private void OnExitClick()
	{
		MissionLog missionLog = Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetMissionLog();
		int num = 0;
		for (int i = 0; i < Singleton<CrewContainer>.Instance.GetCurrentCrewCount(); i++)
		{
			Crewman crewman = Singleton<CrewContainer>.Instance.GetCrewman(i);
			if (crewman != null && crewman.IsDead())
			{
				num++;
			}
		}
		GameAnalytics.NewDesignEvent("MissionEnded:MissionDamage", missionLog.GetDamageTaken());
		GameAnalytics.NewDesignEvent("MissionEnded:EnemiesDefeated", missionLog.GetFightersDestroyed());
		GameAnalytics.NewDesignEvent("MissionEnded:NumCrewDead", num);
		GameAnalytics.NewDesignEvent("MissionEnded:BomberDestroyed", m_shouldResetToNewBomber ? 1 : 0);
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["missionDamage"] = missionLog.GetDamageTaken();
		dictionary["enemiesDefeated"] = missionLog.GetFightersDestroyed();
		dictionary["numCrewDead"] = num;
		dictionary["bomberDestroyed"] = m_shouldResetToNewBomber;
		Analytics.CustomEvent("MissionEnded", dictionary);
		if (m_shouldResetToNewBomber)
		{
			Singleton<AchievementSystem>.Instance.GetUserStat("BombersLost")?.ModifyValue(1);
			int num2 = 0;
			BomberRequirements.BomberEquipmentRequirement[] requirements = Singleton<BomberContainer>.Instance.GetRequirements().GetRequirements();
			BomberRequirements.BomberEquipmentRequirement[] array = requirements;
			foreach (BomberRequirements.BomberEquipmentRequirement req in array)
			{
				EquipmentUpgradeFittableBase upgradeFor = Singleton<SaveDataContainer>.Instance.Get().GetCurrentBomber().GetUpgradeFor(req);
				if (upgradeFor != null)
				{
					num2 += upgradeFor.GetCost();
				}
			}
			Singleton<SaveDataContainer>.Instance.Get().ResetToNewBomber();
			BomberRequirements.BomberEquipmentRequirement[] array2 = requirements;
			foreach (BomberRequirements.BomberEquipmentRequirement req2 in array2)
			{
				EquipmentUpgradeFittableBase upgradeFor2 = Singleton<SaveDataContainer>.Instance.Get().GetCurrentBomber().GetUpgradeFor(req2);
				if (upgradeFor2 != null)
				{
					num2 -= upgradeFor2.GetCost();
				}
			}
			List<BomberDefaults> defaultUpgrades = Singleton<BomberUpgradeCatalogueLoader>.Instance.GetCatalogue().GetDefaultUpgrades();
			BomberDefaults bomberDefaults = null;
			int num3 = 0;
			EquipmentUpgradeFittableBase.AircraftUpgradeType aircraftUpgradeType = EquipmentUpgradeFittableBase.AircraftUpgradeType.LancasterOnly;
			if (Singleton<GameFlow>.Instance.GetGameMode().GetUseUSNaming())
			{
				aircraftUpgradeType = EquipmentUpgradeFittableBase.AircraftUpgradeType.B17Only;
			}
			foreach (BomberDefaults item in defaultUpgrades)
			{
				if (item.m_intelMinReq <= Singleton<SaveDataContainer>.Instance.Get().GetIntel() && item.m_intelMinReq >= num3 && item.m_upgradeType == aircraftUpgradeType)
				{
					num3 = item.m_intelMinReq;
					bomberDefaults = item;
				}
			}
			if (bomberDefaults != null)
			{
				Singleton<BomberContainer>.Instance.UpgradeWithDefaults(bomberDefaults, num2);
			}
			Singleton<BomberContainer>.Instance.GetLivery().RefreshInstant();
		}
		m_bomberController.SetBomberVisible(visible: true);
		m_persistentCrew.InitialisePersistentCrew();
		m_persistentCrew.DoFreeWalk();
		Singleton<AchievementMonitor>.Instance.CheckAchievementsOnEvent(AchievementMonitor.CheckType.DebriefEnd);
		if (missionLog.IsComplete() && m_gotLandAtBaseBonus)
		{
			Singleton<AchievementMonitor>.Instance.CheckAchievementsOnEvent(AchievementMonitor.CheckType.DebriefEndAtBaseComplete);
		}
		UpdateSystemSave();
		Singleton<CrewContainer>.Instance.RemoveDeadCrewmen();
		Singleton<AirbaseNavigation>.Instance.SetCrewPhotoRequiresRefresh();
		Singleton<AirbaseNavigation>.Instance.GoToDefaultScreen(jump: false);
		Singleton<SaveDataContainer>.Instance.Get().AddToDifficultyHistory(num, missionLog.IsComplete());
		Singleton<SaveDataContainer>.Instance.Get().AddPostMissionStats(missionLog.GetFightersDestroyed(), missionLog.GetNumTargetsDestroyed(), (int)missionLog.GetMilesFlown(), m_shouldResetToNewBomber, num, m_numRescued, missionLog.GetPhotosTaken());
		Singleton<SaveDataContainer>.Instance.Save();
		Singleton<AirbaseNavigation>.Instance.SaveCrewPhoto(instant: false);
		Singleton<GameFlow>.Instance.BriefingEnded();
		if (Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetCurrentlySelectedMissionDetails().m_isFinalMission)
		{
			if (missionLog.IsComplete())
			{
				m_gameCompleteSequence.DoGameCompleteSequence(Singleton<GameFlow>.Instance.GetCampaign(), forDLC: false);
			}
		}
		else if (Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetCurrentlySelectedMissionDetails().m_isFinalMissionOfDLC && missionLog.IsComplete())
		{
			CampaignStructure campaignStructure = null;
			foreach (string currentInstalledDLC in Singleton<DLCManager>.Instance.GetCurrentInstalledDLCs())
			{
				DLCExpansionCampaign dLCExpansionCampaign = Singleton<DLCManager>.Instance.LoadAssetFromBundle<DLCExpansionCampaign>(currentInstalledDLC, "CAMPAIGN_" + currentInstalledDLC);
				if (dLCExpansionCampaign != null && dLCExpansionCampaign.GetCampaign() != null)
				{
					CampaignStructure.CampaignMission[] allMissions = dLCExpansionCampaign.GetCampaign().GetAllMissions();
					foreach (CampaignStructure.CampaignMission campaignMission in allMissions)
					{
						if (campaignMission == Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetCurrentlySelectedMissionDetails())
						{
							campaignStructure = dLCExpansionCampaign.GetCampaign();
							break;
						}
					}
				}
				if (campaignStructure != null)
				{
					break;
				}
			}
			m_gameCompleteSequence.DoGameCompleteSequence(campaignStructure, forDLC: true);
		}
		Singleton<GameFlow>.Instance.GetCurrentMissionInfo().SetCleared();
		AirbaseMainMenuButton.SetPauseBlocked(blocked: false);
		Singleton<AirbaseNavigation>.Instance.SetContentUpdateAllowed();
	}

	private void OnEnable()
	{
		AirbaseMainMenuButton.SetPauseBlocked(blocked: true);
		m_bomberPortraitRender.material.mainTexture = Singleton<BomberPhotoBooth>.Instance.RenderForBomber(null);
		Singleton<AirbaseCameraController>.Instance.MoveCameraToLocationInstant(m_initialCamera);
		GetComponent<AirbaseAreaScreen>().SetDefaultSelection();
		Singleton<UISelector>.Instance.SetFinder(m_mainFinder);
		Singleton<AchievementSystem>.Instance.GetUserStat("MissionsPlayed")?.ModifyValue(1);
		MissionLog missionLog = Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetMissionLog();
		if (missionLog.IsComplete())
		{
			Singleton<AchievementSystem>.Instance.GetUserStat("MissionsCompleted")?.ModifyValue(1);
		}
		Singleton<AchievementMonitor>.Instance.CheckAchievementsOnEvent(AchievementMonitor.CheckType.DebriefStart);
		StartCoroutine(DoSequence());
	}

	private IEnumerator DoSequence()
	{
		m_finishedHierarchy.SetActive(value: false);
		m_continueHierarchy.SetActive(value: false);
		m_bomberStateHierarchy.SetActive(value: false);
		m_aceEncounterHierarchy.SetActive(value: false);
		m_photosTakenCounter.SetText("0");
		m_missionIsTutorialLabel.SetActive(Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetCurrentlySelectedMissionDetails().m_isTrainingMission);
		yield return StartCoroutine(DoMissionStatus());
		if (Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetMissionLog().GetAceEncountered() != null)
		{
			yield return StartCoroutine(DoAceStatus());
		}
		m_bomberController.SetBomberVisible(visible: false);
		yield return StartCoroutine(DoBomberStatus());
		yield return StartCoroutine(WaitForContinuePressed());
		Singleton<AirbaseCameraController>.Instance.MoveCameraToLocation(m_crewmanStateCamera);
		yield return StartCoroutine(DoCrewmenStatus());
		if (!Singleton<GameFlow>.Instance.IsDemoMode())
		{
			if (m_unlockedSkills.Count > 0)
			{
				yield return StartCoroutine(WaitForContinuePressed());
				Singleton<AirbaseCameraController>.Instance.MoveCameraToLocation(m_endCamera);
				m_persistentCrew.DoFreeWalk();
				yield return StartCoroutine(DoNewSkillsStatus());
			}
			yield return StartCoroutine(DoNewUnlocksStatus());
		}
		SetSkipAllowed(allowed: false);
		m_finishedHierarchy.CustomActivate(active: true);
	}

	private IEnumerator DoAceStatus()
	{
		SetSkipAllowed(allowed: true);
		m_aceEncounterHierarchy.SetActive(value: true);
		Singleton<AirbaseCameraController>.Instance.MoveCameraToLocation(m_aceCamera);
		m_aceResultDisplayer.SetUp(Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetMissionLog().GetAceEncountered(), Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetMissionLog().GetAceDefeated());
		float delay = m_aceResultDisplayer.GetDelay();
		yield return StartCoroutine(DoSkipWait(delay));
		m_aceBoardPersistent.Refresh();
		yield return StartCoroutine(DoSkipWait(Mathf.Max(4f - delay, 0f)));
		m_aceEncounterHierarchy.SetActive(value: false);
		if (Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetMissionLog().GetAceDefeated())
		{
			EnemyFighterAce aceEncountered = Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetMissionLog().GetAceEncountered();
			GameObject gameObject = UnityEngine.Object.Instantiate(m_targetDestroyMissionObjectivePrefab);
			gameObject.GetComponent<DebriefingObjectiveDisplay>().SetUp("ui_debrief_defeat_enemy_ace");
			gameObject.transform.parent = m_missionObjectiveLayout.transform;
			m_missionObjectiveLayout.RepositionChildren();
			if (Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetMissionLog().GetAceDefeatedForFirstTime())
			{
				gameObject.GetComponent<DebriefingObjectiveDisplay>().ShowObjectiveCompletion(destroyed: true, aceEncountered.GetCashBonus(), 0, isSubObjective: true);
				Singleton<SaveDataContainer>.Instance.Get().AddBalance(aceEncountered.GetCashBonus());
			}
			else
			{
				gameObject.GetComponent<DebriefingObjectiveDisplay>().ShowObjectiveCompletion(destroyed: true, 0, 0, isSubObjective: true);
			}
		}
		Singleton<AirbaseCameraController>.Instance.MoveCameraToLocation(m_initialCamera);
	}

	private IEnumerator WaitForContinuePressed()
	{
		SetSkipAllowed(allowed: false);
		m_continueHierarchy.CustomActivate(active: true);
		m_continueButtonPresed = false;
		while (!m_continueButtonPresed)
		{
			yield return null;
		}
		m_continueButtonPresed = false;
		m_continueHierarchy.CustomActivate(active: false);
	}

	private void UpdateSystemSave()
	{
		MissionLog missionLog = Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetMissionLog();
		bool flag = missionLog.IsComplete();
		if (flag && Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetCurrentlySelectedMissionDetails().m_isFinalMission)
		{
			AchievementSystem.Achievement achievement = Singleton<AchievementSystem>.Instance.GetAchievement("FinalMission");
			if (achievement != null)
			{
				Singleton<AchievementSystem>.Instance.AwardAchievement(achievement);
			}
		}
		int currentCrewCount = Singleton<CrewContainer>.Instance.GetCurrentCrewCount();
		List<Crewman> list = new List<Crewman>();
		int num = 0;
		int num2 = int.MaxValue;
		int num3 = 0;
		int num4 = int.MaxValue;
		for (int i = 0; i < currentCrewCount; i++)
		{
			Crewman crewman = Singleton<CrewContainer>.Instance.GetCrewman(i);
			crewman.SetMissionFinished(flag);
			list.Add(crewman);
			if (crewman.IsDead())
			{
				Singleton<SystemDataContainer>.Instance.Get().RegisterCrewmanLost(crewman);
				num2 = 0;
				num4 = 0;
			}
			else
			{
				num = Mathf.Max(crewman.GetMissionsPlayed(), num);
				num2 = Mathf.Min(crewman.GetMissionsPlayed(), num2);
				num3 = Mathf.Max(crewman.GetMissionsSuccessful(), num3);
				num4 = Mathf.Min(crewman.GetMissionsSuccessful(), num4);
			}
		}
		if (num2 <= num && num2 < int.MaxValue)
		{
			Singleton<AchievementSystem>.Instance.GetUserStat("MostMissionsSurvivedFull")?.SetValueHighest(num2);
		}
		if (num4 <= num3 && num4 < int.MaxValue)
		{
			Singleton<AchievementSystem>.Instance.GetUserStat("MostMissionsCompletedFull")?.SetValueHighest(num4);
		}
		Singleton<AchievementSystem>.Instance.GetUserStat("MostMissionsSurvived")?.SetValueHighest(num);
		Singleton<AchievementSystem>.Instance.GetUserStat("MostMissionsCompleted")?.SetValueHighest(num3);
		if (missionLog.IsComplete() && Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetCurrentlySelectedMissionDetails().m_isFinalMission)
		{
			Singleton<SystemDataContainer>.Instance.Get().SetCompletionCrew(list);
		}
		Singleton<SystemDataContainer>.Instance.Get().SetMemorialCrew(list);
		Singleton<SystemDataContainer>.Instance.Save();
	}

	public IEnumerator DoSkipWait(float time)
	{
		float t = time;
		while (t > 0f)
		{
			t = ((!m_shouldFastForward) ? (t - Time.deltaTime) : (t - Time.deltaTime * 5f));
			yield return null;
		}
	}

	private IEnumerator DoMissionStatus()
	{
		SetSkipAllowed(allowed: true);
		yield return StartCoroutine(DoSkipWait(1f));
		m_missionResultHierarchy.SetActive(value: true);
		MissionLog ml = Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetMissionLog();
		m_bomberStateHierarchy.SetActive(value: false);
		yield return StartCoroutine(DoSkipWait(1.5f));
		bool missionSuccess = ml.IsComplete();
		GameObject newTargetObj = UnityEngine.Object.Instantiate(m_targetDestroyMissionObjectivePrefab);
		newTargetObj.GetComponent<DebriefingObjectiveDisplay>().SetUp(Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetCurrentlySelectedMissionDetails().m_titleNamedText);
		newTargetObj.transform.parent = m_missionObjectiveLayout.transform;
		m_missionObjectiveLayout.RepositionChildren();
		float moneyReward = (float)Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetCurrentlySelectedMissionDetails().m_totalMoneyReward * 0.6f;
		float intelReward = (float)Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetCurrentlySelectedMissionDetails().m_totalIntelReward * 0.6f;
		yield return StartCoroutine(DoSkipWait(1f));
		Singleton<SaveDataContainer>.Instance.Get().LowerPerkCount();
		string missionReference = Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetCurrentlySelectedMissionDetails().m_missionReferenceName;
		if (missionSuccess)
		{
			WingroveRoot.Instance.PostEvent(m_objectiveWonAudioEvent);
			newTargetObj.GetComponent<DebriefingObjectiveDisplay>().ShowObjectiveCompletion(destroyed: true, Mathf.RoundToInt(moneyReward), Mathf.RoundToInt(intelReward), isSubObjective: false);
			Singleton<SaveDataContainer>.Instance.Get().AddBalance(Mathf.RoundToInt(moneyReward));
			Singleton<SaveDataContainer>.Instance.Get().AddIntel(Mathf.RoundToInt(intelReward));
			CampaignPerk perk = Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetCurrentlySelectedMissionDetails().m_campaignPerk;
			if (perk != null)
			{
				yield return StartCoroutine(DoSkipWait(1f));
				m_perkDisplay.SetUp(perk.GetPerk(), perk.GetMissions(), perk.GetFactor());
				m_perkDisplay.gameObject.SetActive(value: true);
				Singleton<SaveDataContainer>.Instance.Get().AddPerk(perk.GetPerk(), perk.GetFactor(), perk.GetMissions());
				yield return StartCoroutine(DoSkipWait(1f));
			}
		}
		else
		{
			WingroveRoot.Instance.PostEvent(m_objectiveFailedAudioEvent);
			newTargetObj.GetComponent<DebriefingObjectiveDisplay>().ShowObjectiveCompletion(destroyed: false, 0, 0, isSubObjective: false);
		}
		List<Crewman> toSave = new List<Crewman>();
		for (int i = 0; i < Singleton<CrewContainer>.Instance.GetCurrentCrewCount(); i++)
		{
			toSave.Add(Singleton<CrewContainer>.Instance.GetCrewman(i));
		}
		string missionRef = Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetCurrentlySelectedMissionDetails().m_missionReferenceName;
		bool shouldSaveCrew = false;
		if (missionRef == "BombRunTraining")
		{
			if (missionSuccess && !Singleton<SaveDataContainer>.Instance.Get().HasCompletedMission("BombRunTraining"))
			{
				shouldSaveCrew = true;
			}
		}
		else if (Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetCurrentlySelectedMissionDetails().m_isKeyMission && Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetCurrentlySelectedMissionDetails().m_showKeyMissionTitle && missionSuccess)
		{
			shouldSaveCrew = true;
		}
		Singleton<SaveDataContainer>.Instance.Get().SetMissionPlayed(missionRef, missionSuccess, shouldSaveCrew, toSave);
		m_campaignProgress.Refresh();
		yield return StartCoroutine(DoSkipWait(1.5f));
	}

	private IEnumerator DoNewSkillsStatus()
	{
		SetSkipAllowed(allowed: true);
		m_crewResultHierarchy.SetActive(value: false);
		m_newSkillsUnlockScreen.SetActive(value: true);
		foreach (KeyValuePair<Crewman.SpecialisationSkill, List<CrewmanSkillAbilityBool>> kvp in m_unlockedSkills)
		{
			foreach (CrewmanSkillAbilityBool sk in kvp.Value)
			{
				GameObject go = UnityEngine.Object.Instantiate(m_newSkillsPrefab);
				go.transform.parent = m_newSkillsLayoutGrid.transform;
				go.transform.localPosition = Vector3.zero;
				go.GetComponent<NewSkillSummaryDisplay>().SetUp(sk, kvp.Key);
				m_newSkillsLayoutGrid.RepositionChildren();
				yield return StartCoroutine(DoSkipWait(0.2f));
			}
		}
		yield return StartCoroutine(DoSkipWait(1.5f));
	}

	private IEnumerator DoNewUnlocksStatus()
	{
		SetSkipAllowed(allowed: true);
		List<string> categoryNames = new List<string>();
		List<string> actualNames = new List<string>();
		BomberUpgradeCatalogueLoader.BomberUpgradeCatalogueInternal catalogue = Singleton<BomberUpgradeCatalogueLoader>.Instance.GetCatalogue();
		List<EquipmentUpgradeFittableBase> allEquipment = catalogue.GetAll();
		int intel = Singleton<SaveDataContainer>.Instance.Get().GetIntel();
		string bomberUpgradeText = Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("ui_bomber_upgrade_type");
		string bomberUpgradeLiveryText = Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("ui_bomber_upgrade_livery_type");
		string crewGearUpgrade = Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("ui_crewman_gear_upgrade_type");
		foreach (EquipmentUpgradeFittableBase item in allEquipment)
		{
			if (intel < item.GetIntelUnlockRequirement() || item.GetIntelUnlockRequirement() <= 0)
			{
				continue;
			}
			bool flag = m_aircraftUpgradeFilter == EquipmentUpgradeFittableBase.AircraftUpgradeType.AllAircraft;
			if (m_aircraftUpgradeFilter == item.GetAircraftUpgradeType())
			{
				flag = true;
			}
			if (Singleton<SaveDataContainer>.Instance.Get().HasBeenAnnounced(item.name) || !flag)
			{
				continue;
			}
			if (item.GetUpgradeType() == BomberUpgradeType.Livery)
			{
				categoryNames.Add(bomberUpgradeLiveryText);
			}
			else
			{
				categoryNames.Add(bomberUpgradeText);
			}
			BomberUpgradeType upgradeType = item.GetUpgradeType();
			string text = string.Empty;
			BomberUpgradeTypeToNamedText[] upgradeToNamedText = m_upgradeToNamedText;
			foreach (BomberUpgradeTypeToNamedText bomberUpgradeTypeToNamedText in upgradeToNamedText)
			{
				if (bomberUpgradeTypeToNamedText.m_upgradeType == upgradeType)
				{
					text = Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(bomberUpgradeTypeToNamedText.m_namedText) + ": ";
					break;
				}
			}
			actualNames.Add(text + item.GetNameTranslated());
			Singleton<SaveDataContainer>.Instance.Get().SetAnnounced(item.name);
		}
		CrewmanGearCatalogueLoader.CrewmanGearCatalogueInternal gearCat = Singleton<CrewmanGearCatalogueLoader>.Instance.GetCatalogue();
		List<CrewmanEquipmentBase> cebAll = gearCat.GetAll();
		foreach (CrewmanEquipmentBase item2 in cebAll)
		{
			if (intel < item2.GetIntelUnlockRequirement() || item2.GetIntelUnlockRequirement() <= 0 || Singleton<SaveDataContainer>.Instance.Get().HasBeenAnnounced(item2.name))
			{
				continue;
			}
			categoryNames.Add(crewGearUpgrade);
			CrewmanGearType gearType = item2.GetGearType();
			string text2 = string.Empty;
			GearTypeToNamedText[] gearToNamedText = m_gearToNamedText;
			foreach (GearTypeToNamedText gearTypeToNamedText in gearToNamedText)
			{
				if (gearTypeToNamedText.m_gearType == gearType)
				{
					text2 = Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(gearTypeToNamedText.m_namedText) + ": ";
					break;
				}
			}
			actualNames.Add(text2 + item2.GetNamedTextTranslated());
			Singleton<SaveDataContainer>.Instance.Get().SetAnnounced(item2.name);
		}
		if (categoryNames.Count <= 0)
		{
			yield break;
		}
		yield return StartCoroutine(WaitForContinuePressed());
		Singleton<AirbaseCameraController>.Instance.MoveCameraToLocation(m_endCamera);
		m_persistentCrew.DoFreeWalk();
		m_crewResultHierarchy.SetActive(value: false);
		m_newUnlocksUnlockScreen.SetActive(value: true);
		m_newSkillsUnlockScreen.SetActive(value: false);
		List<GameObject> created = new List<GameObject>();
		int num = categoryNames.Count;
		for (int i = 0; i < num; i++)
		{
			GameObject go = UnityEngine.Object.Instantiate(m_newUnlocksPrefab);
			go.transform.parent = m_newUnlocksLayoutGrid.transform;
			go.transform.localPosition = Vector3.zero;
			go.GetComponent<UnlockSummaryDisplay>().SetUp(categoryNames[i], actualNames[i]);
			created.Add(go);
			if (created.Count > 10)
			{
				UnityEngine.Object.DestroyImmediate(created[0]);
				created.RemoveAt(0);
			}
			m_newUnlocksLayoutGrid.RepositionChildren();
			yield return StartCoroutine(DoSkipWait(0.2f));
		}
		yield return StartCoroutine(DoSkipWait(1.5f));
	}

	private void ShowBomberArrive(bool arrive)
	{
		if (arrive)
		{
			m_bomberController.SetBomberVisible(visible: true);
			m_bomberController.PlayArriveAnimation();
		}
		Singleton<AirbaseCameraController>.Instance.MoveCameraToLocation(m_bomberStateCamera);
	}

	private IEnumerator DoBomberStatus()
	{
		SetSkipAllowed(allowed: true);
		if (m_bomberName != null)
		{
			m_bomberName.SetText("\"" + Singleton<BomberContainer>.Instance.GetCurrentConfig().GetName() + "\"");
		}
		m_recoverButtonPressed = false;
		m_dontRecoverButtonPressed = false;
		MissionLog ml = Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetMissionLog();
		float moneyReward = (float)Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetCurrentlySelectedMissionDetails().m_totalMoneyReward * 0.4f;
		float intelReward = (float)Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetCurrentlySelectedMissionDetails().m_totalIntelReward * 0.4f;
		m_bomberSafeHierarchy.SetActive(value: false);
		m_bomberRecoverableHierarchy.SetActive(value: false);
		m_bomberLostHierarchy.SetActive(value: false);
		m_bomberDestroyedHierarchy.SetActive(value: false);
		m_bomberRecoveredHierarchy.SetActive(value: false);
		yield return StartCoroutine(DoSkipWait(1f));
		m_bomberStateHierarchy.CustomActivate(active: true);
		yield return StartCoroutine(DoSkipWait(1f));
		bool photosRecovered = false;
		bool getReturnToBaseBonus = true;
		MissionLog.LandingState landingState = ml.GetLandingState();
		string missionReference = Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetCurrentlySelectedMissionDetails().m_missionReferenceName;
		if (landingState == MissionLog.LandingState.InSea && !ml.WasBomberDestroyed())
		{
			m_bomberLostHierarchy.CustomActivate(active: true);
			ShowBomberArrive(arrive: false);
			WingroveRoot.Instance.PostEvent(m_bomberLostAudioEvent);
			if (Singleton<SaveDataContainer>.Instance.Get().GetIsCampaign())
			{
				GameAnalytics.NewDesignEvent("BomberStatus:" + missionReference + ":Lost");
			}
			if (Singleton<SaveDataContainer>.Instance.Get().GetIsCampaign())
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary["missionName"] = missionReference;
				Analytics.CustomEvent("BomberLost", dictionary);
			}
			m_shouldResetToNewBomber = true;
			getReturnToBaseBonus = false;
		}
		else if (landingState == MissionLog.LandingState.AtBase && !ml.WasBomberDestroyed())
		{
			ShowBomberArrive(arrive: true);
			yield return StartCoroutine(DoSkipWait(1f));
			m_bomberSafeHierarchy.CustomActivate(active: true);
			photosRecovered = true;
			WingroveRoot.Instance.PostEvent(m_bomberReturnedAudioEvent);
			if (Singleton<SaveDataContainer>.Instance.Get().GetIsCampaign())
			{
				GameAnalytics.NewDesignEvent("BomberStatus:" + missionReference + ":Safe");
			}
			if (Singleton<SaveDataContainer>.Instance.Get().GetIsCampaign())
			{
				Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
				dictionary2["missionName"] = missionReference;
				Analytics.CustomEvent("BomberSafe", dictionary2);
			}
		}
		else if (landingState == MissionLog.LandingState.InEngland && !ml.WasBomberDestroyed())
		{
			Vector3 endPos = ml.GetLandingPosition();
			float dist = new Vector3(endPos.x, 0f, endPos.z).magnitude;
			if (dist > 900f)
			{
				if (Singleton<SaveDataContainer>.Instance.Get().GetIsCampaign())
				{
					GameAnalytics.NewDesignEvent("BomberStatus:" + missionReference + ":Recovered");
				}
				if (Singleton<SaveDataContainer>.Instance.Get().GetIsCampaign())
				{
					Dictionary<string, object> dictionary3 = new Dictionary<string, object>();
					dictionary3["missionName"] = missionReference;
					Analytics.CustomEvent("BomberRecovered", dictionary3);
				}
				ShowBomberArrive(arrive: true);
				yield return StartCoroutine(DoSkipWait(1f));
				m_bomberRecoveredHierarchy.CustomActivate(active: true);
				photosRecovered = true;
				WingroveRoot.Instance.PostEvent(m_bomberReturnedAudioEvent);
				getReturnToBaseBonus = false;
			}
			else
			{
				if (Singleton<SaveDataContainer>.Instance.Get().GetIsCampaign())
				{
					GameAnalytics.NewDesignEvent("BomberStatus:" + missionReference + ":Safe");
				}
				if (Singleton<SaveDataContainer>.Instance.Get().GetIsCampaign())
				{
					Dictionary<string, object> dictionary4 = new Dictionary<string, object>();
					dictionary4["missionName"] = missionReference;
					Analytics.CustomEvent("BomberSafe", dictionary4);
				}
				ShowBomberArrive(arrive: true);
				yield return StartCoroutine(DoSkipWait(1f));
				m_bomberSafeHierarchy.CustomActivate(active: true);
				photosRecovered = true;
				WingroveRoot.Instance.PostEvent(m_bomberReturnedAudioEvent);
			}
		}
		else
		{
			if (Singleton<SaveDataContainer>.Instance.Get().GetIsCampaign())
			{
				GameAnalytics.NewDesignEvent("BomberStatus:" + missionReference + ":Lost");
			}
			if (Singleton<SaveDataContainer>.Instance.Get().GetIsCampaign())
			{
				Dictionary<string, object> dictionary5 = new Dictionary<string, object>();
				dictionary5["missionName"] = missionReference;
				Analytics.CustomEvent("BomberLost", dictionary5);
			}
			m_shouldResetToNewBomber = true;
			ShowBomberArrive(arrive: false);
			m_bomberDestroyedHierarchy.CustomActivate(active: true);
			WingroveRoot.Instance.PostEvent(m_bomberLostAudioEvent);
			getReturnToBaseBonus = false;
		}
		bool missionSuccess = ml.IsComplete();
		if (missionSuccess)
		{
			yield return StartCoroutine(DoSkipWait(1f));
			GameObject returnHomeSafely = UnityEngine.Object.Instantiate(m_targetDestroyMissionObjectivePrefab);
			returnHomeSafely.GetComponent<DebriefingObjectiveDisplay>().SetUp("ui_debrief_return_home_safely_reward");
			returnHomeSafely.transform.parent = m_missionObjectiveLayout.transform;
			m_missionObjectiveLayout.RepositionChildren();
			if (!m_shouldResetToNewBomber && getReturnToBaseBonus)
			{
				m_gotLandAtBaseBonus = true;
				Singleton<SaveDataContainer>.Instance.Get().AddBalance(Mathf.RoundToInt(moneyReward));
				Singleton<SaveDataContainer>.Instance.Get().AddIntel(Mathf.RoundToInt(intelReward));
				returnHomeSafely.GetComponent<DebriefingObjectiveDisplay>().ShowObjectiveCompletion(destroyed: true, Mathf.RoundToInt(moneyReward), Mathf.RoundToInt(intelReward), isSubObjective: true);
			}
			else
			{
				returnHomeSafely.GetComponent<DebriefingObjectiveDisplay>().ShowObjectiveCompletion(destroyed: false, 0, 0, isSubObjective: true);
			}
		}
		if (missionSuccess && !Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetMissionLog().DidUseSlowTime() && m_gotLandAtBaseBonus)
		{
			yield return StartCoroutine(DoSkipWait(1f));
			GameObject noSlowDownBonus = UnityEngine.Object.Instantiate(m_targetDestroyMissionObjectivePrefab);
			noSlowDownBonus.GetComponent<DebriefingObjectiveDisplay>().SetUp("ui_debrief_no_slowdown_bonus");
			noSlowDownBonus.transform.parent = m_missionObjectiveLayout.transform;
			m_missionObjectiveLayout.RepositionChildren();
			float slowMoneyReward = moneyReward * 0.1f;
			float slowIntelReward = intelReward * 0.1f;
			Singleton<SaveDataContainer>.Instance.Get().AddBalance(Mathf.RoundToInt(slowMoneyReward));
			Singleton<SaveDataContainer>.Instance.Get().AddIntel(Mathf.RoundToInt(slowIntelReward));
			noSlowDownBonus.GetComponent<DebriefingObjectiveDisplay>().ShowObjectiveCompletion(destroyed: true, Mathf.RoundToInt(slowMoneyReward), Mathf.RoundToInt(slowIntelReward), isSubObjective: true);
		}
		yield return StartCoroutine(DoSkipWait(1f));
		if (photosRecovered && ml.GetPhotosTaken() > 0)
		{
			yield return StartCoroutine(DoSkipWait(0.5f));
			for (int i = 0; i < ml.GetPhotosTaken(); i++)
			{
				m_photosTakenCounter.SetText((i + 1).ToString());
				WingroveRoot.Instance.PostEvent(m_photoTakenAudioEvent);
				yield return StartCoroutine(DoSkipWait(0.5f));
			}
			Singleton<SaveDataContainer>.Instance.Get().AddIntel(ml.GetIntelFromPhotos());
			Singleton<SaveDataContainer>.Instance.Get().AddBalance(ml.GetMoneyFromPhotos());
			if (ml.GetIntelFromPhotos() > 0 || ml.GetMoneyFromPhotos() > 0)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(m_targetDestroyMissionObjectivePrefab);
				gameObject.GetComponent<DebriefingObjectiveDisplay>().SetUp("ui_debrief_optional_photos_reward");
				gameObject.transform.parent = m_missionObjectiveLayout.transform;
				m_missionObjectiveLayout.RepositionChildren();
				gameObject.GetComponent<DebriefingObjectiveDisplay>().ShowObjectiveCompletion(destroyed: true, ml.GetMoneyFromPhotos(), ml.GetIntelFromPhotos(), isSubObjective: true);
			}
		}
	}

	private bool RequiresMIACalculation(Crewman cm, MissionLog.LandingState ls, bool bailedOut)
	{
		return ls switch
		{
			MissionLog.LandingState.AtBase => false, 
			MissionLog.LandingState.InEngland => false, 
			_ => true, 
		};
	}

	public void AddNewSkill(Crewman.SpecialisationSkill sk, CrewmanSkillAbilityBool sb)
	{
		List<CrewmanSkillAbilityBool> value = null;
		m_unlockedSkills.TryGetValue(sk, out value);
		if (value == null)
		{
			value = new List<CrewmanSkillAbilityBool>();
			m_unlockedSkills[sk] = value;
		}
		if (!value.Contains(sb))
		{
			value.Add(sb);
		}
	}

	private IEnumerator DoCrewmenStatus()
	{
		SetSkipAllowed(allowed: true);
		m_numRescued = 0;
		MissionLog ml = Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetMissionLog();
		m_missionResultHierarchy.SetActive(value: false);
		m_crewResultHierarchy.SetActive(value: true);
		m_panels.Clear();
		foreach (GameObject createdObject in m_createdObjects)
		{
			UnityEngine.Object.DestroyImmediate(createdObject);
		}
		m_createdObjects.Clear();
		string missionReference = Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetCurrentlySelectedMissionDetails().m_missionReferenceName;
		int numCrewmen = Singleton<CrewContainer>.Instance.GetCurrentCrewCount();
		for (int i = 0; i < numCrewmen; i++)
		{
			GameObject go = UnityEngine.Object.Instantiate(m_crewmanSummaryPanel);
			go.transform.parent = m_layoutGrid.transform;
			go.transform.localPosition = Vector3.zero;
			DebriefingCrewmanPanel dcp = go.GetComponent<DebriefingCrewmanPanel>();
			Crewman cm = Singleton<CrewContainer>.Instance.GetCrewman(i);
			dcp.SetUp(cm);
			m_panels.Add(dcp);
			m_createdObjects.Add(go);
			m_layoutGrid.RepositionChildren();
			yield return StartCoroutine(DoSkipWait(0.2f));
		}
		for (int j = 0; j < m_panels.Count; j++)
		{
			Crewman cm2 = Singleton<CrewContainer>.Instance.GetCrewman(j);
			MissionLog.LandingState landingState = ml.GetCrewmanLandingState(cm2);
			bool requiresMIACalculation = RequiresMIACalculation(cm2, landingState, ml.IsBailedOut(cm2));
			bool isMIA = false;
			bool isRecovered = false;
			if (requiresMIACalculation && !cm2.IsDead())
			{
				GameObject miaCalc = UnityEngine.Object.Instantiate(m_miaCalculator);
				DebriefingCrewmanSurvivalDisplay survivalDisplay = miaCalc.GetComponent<DebriefingCrewmanSurvivalDisplay>();
				survivalDisplay.transform.parent = m_panels[j].transform;
				survivalDisplay.transform.localPosition = Vector3.zero;
				survivalDisplay.SetUp(cm2, landingState, ml.IsBailedOut(cm2), ml.GetLandingPosition(), (!ml.IsBailedOut(cm2)) ? Vector3.zero : ml.GetCrewmanPosition(cm2), this);
				while (!survivalDisplay.IsDone())
				{
					yield return null;
				}
				if (survivalDisplay.IsMIA())
				{
					isMIA = true;
				}
				else
				{
					m_numRescued++;
					isRecovered = true;
				}
				miaCalc.CustomActivate(active: false);
			}
			else
			{
				if (landingState == MissionLog.LandingState.InEngland && !cm2.IsDead())
				{
					m_numRescued++;
					isRecovered = false;
				}
				isMIA = false;
			}
			if (isMIA)
			{
				cm2.SetDead();
			}
			m_panels[j].SetStatus(isMIA, isRecovered);
			yield return m_panels[j].ShowStatus(this);
			if (!cm2.IsDead())
			{
				m_persistentCrew.InitialiseFor(cm2);
				m_persistentCrew.DoTargetedWalk(m_persistentCrewTargets, cm2, j, forceWarp: true);
			}
		}
		int xpAmount = Mathf.RoundToInt((float)ml.GetTotalXP() * Singleton<GameFlow>.Instance.GetGameMode().GetXPMultiplier());
		foreach (DebriefingCrewmanPanel dcp2 in m_panels)
		{
			dcp2.DoShowLevelUp(xpAmount, this);
			yield return StartCoroutine(DoSkipWait(0.3f));
		}
		bool levelUpComplete = false;
		while (!levelUpComplete)
		{
			levelUpComplete = true;
			foreach (DebriefingCrewmanPanel panel in m_panels)
			{
				if (!panel.LevelUpSequenceIsCompleted())
				{
					levelUpComplete = false;
				}
			}
			yield return null;
		}
		int numDead = 0;
		for (int k = 0; k < numCrewmen; k++)
		{
			Crewman crewman = Singleton<CrewContainer>.Instance.GetCrewman(k);
			if (crewman.IsDead())
			{
				numDead++;
			}
		}
		if (Singleton<SaveDataContainer>.Instance.Get().GetIsCampaign())
		{
			GameAnalytics.NewDesignEvent("CrewmanKilled:" + missionReference, numDead);
		}
		if (Singleton<SaveDataContainer>.Instance.Get().GetIsCampaign())
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["missionName"] = missionReference;
			dictionary["numKilled"] = numDead;
			Analytics.CustomEvent("CrewmanKilled", dictionary);
		}
		AchievementSystem.Achievement achQ = Singleton<AchievementSystem>.Instance.GetAchievement("Qualified");
		AchievementSystem.Achievement achDQ = Singleton<AchievementSystem>.Instance.GetAchievement("DoubleQualified");
		for (int l = 0; l < numCrewmen; l++)
		{
			Crewman crewman2 = Singleton<CrewContainer>.Instance.GetCrewman(l);
			if (crewman2.IsDead())
			{
				continue;
			}
			if (Singleton<SaveDataContainer>.Instance.Get().GetIsCampaign())
			{
				GameAnalytics.NewDesignEvent("CrewmanLevel:" + missionReference, crewman2.GetPrimarySkill().GetLevel());
			}
			if (Singleton<SaveDataContainer>.Instance.Get().GetIsCampaign())
			{
				Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
				dictionary2["missionName"] = missionReference;
				dictionary2["level"] = crewman2.GetPrimarySkill().GetLevel();
				Analytics.CustomEvent("CrewmanLevel", dictionary2);
			}
			if (crewman2.GetPrimarySkill().GetLevel() == crewman2.GetPrimarySkill().GetMaxLevel())
			{
				if (achQ != null)
				{
					Singleton<AchievementSystem>.Instance.AwardAchievement(achQ);
				}
				if (crewman2.GetSecondarySkill() != null && crewman2.GetSecondarySkill().GetLevel() == crewman2.GetSecondarySkill().GetMaxLevel() && achDQ != null)
				{
					Singleton<AchievementSystem>.Instance.AwardAchievement(achDQ);
				}
			}
		}
	}
}
