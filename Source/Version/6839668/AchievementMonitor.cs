using System;
using System.Collections.Generic;
using BomberCrewCommon;

public class AchievementMonitor : Singleton<AchievementMonitor>, LoadableSystem
{
	public enum CheckType
	{
		OnStatUpdate,
		Constant,
		DebriefStart,
		DebriefEnd,
		ConstantInMissionOnly,
		OnLandBomberAtBase,
		OnLandBomberAtBaseComplete,
		DebriefEndAtBaseComplete
	}

	public class AchievementCheck
	{
		public Func<AchievementCheck, bool> m_shouldUnlock;

		public AchievementSystem.Achievement m_achievement;

		public CheckType m_checkType;

		public AchievementSystem.UserStat m_userStat;

		public int m_userStatRefValue;

		public AchievementCheck(Func<AchievementCheck, bool> checkFunc, string achievementRef, CheckType checkType)
		{
			m_checkType = checkType;
			m_achievement = Singleton<AchievementSystem>.Instance.GetAchievement(achievementRef);
			m_shouldUnlock = checkFunc;
		}

		public AchievementCheck(Func<AchievementCheck, bool> checkFunc, string achievementRef, string statRef, int mValue)
		{
			m_checkType = CheckType.OnStatUpdate;
			m_userStatRefValue = mValue;
			m_achievement = Singleton<AchievementSystem>.Instance.GetAchievement(achievementRef);
			m_userStat = Singleton<AchievementSystem>.Instance.GetUserStat(statRef);
			m_shouldUnlock = checkFunc;
		}
	}

	private List<AchievementCheck> m_achievementChecks = new List<AchievementCheck>();

	private bool m_isInitialised;

	private void Awake()
	{
		Singleton<SystemLoader>.Instance.RegisterLoadableSystem(this);
	}

	private void Update()
	{
		CheckAchievementsOnEvent(CheckType.Constant);
		if (Singleton<GameFlow>.Instance.GetIsInMissionProbable() && Singleton<MissionCoordinator>.Instance != null)
		{
			CheckAchievementsOnEvent(CheckType.ConstantInMissionOnly);
		}
	}

	private void DoAchievementCheck(AchievementCheck ac)
	{
		if (ac.m_achievement != null && !ac.m_achievement.m_unlocked && ac.m_shouldUnlock != null && ac.m_shouldUnlock(ac))
		{
			Singleton<AchievementSystem>.Instance.AwardAchievement(ac.m_achievement);
		}
	}

	private void OnStatsUpdate()
	{
		CheckAchievementsOnEvent(CheckType.OnStatUpdate);
	}

	public void CheckAchievementsOnEvent(CheckType ct)
	{
		foreach (AchievementCheck achievementCheck in m_achievementChecks)
		{
			if (achievementCheck.m_checkType == ct)
			{
				DoAchievementCheck(achievementCheck);
			}
		}
	}

	public bool IsLoadComplete()
	{
		return m_isInitialised;
	}

	public void StartLoad()
	{
		m_isInitialised = true;
		Singleton<AchievementSystem>.Instance.OnStatUpdate += OnStatsUpdate;
		m_achievementChecks.Add(new AchievementCheck(CheckFirstMissionComplete, "CompleteIntroMission", CheckType.DebriefStart));
		m_achievementChecks.Add(new AchievementCheck(CheckStatBasedSimple, "SinkXSubmarines", "SubmarinesDestroyed", 5));
		m_achievementChecks.Add(new AchievementCheck(CheckStatBasedSimple, "SevenSurviveSeven", "MostMissionsCompletedFull", 7));
		m_achievementChecks.Add(new AchievementCheck(CheckStatBasedSimple, "TourOfDuty", "MostMissionsSurvived", 30));
		m_achievementChecks.Add(new AchievementCheck(CheckStatBasedSimple, "Fighters50", "FightersDestroyed", 50));
		m_achievementChecks.Add(new AchievementCheck(CheckStatBasedSimple, "Fighters250", "FightersDestroyed", 250));
		m_achievementChecks.Add(new AchievementCheck(CheckStatBasedSimple, "DestroyTarget", "TargetsDestroyed", 1));
		m_achievementChecks.Add(new AchievementCheck(CheckStatBasedSimple, "Targets20", "TargetsDestroyed", 20));
		m_achievementChecks.Add(new AchievementCheck(CheckStatBasedSimple, "Targets200", "TargetsDestroyed", 200));
		m_achievementChecks.Add(new AchievementCheck(CheckStatBasedSimple, "Rookie", "MissionsPlayed", 10));
		m_achievementChecks.Add(new AchievementCheck(CheckStatBasedSimple, "Experienced", "MissionsPlayed", 25));
		m_achievementChecks.Add(new AchievementCheck(CheckStatBasedSimple, "Veteran", "MissionsPlayed", 40));
		m_achievementChecks.Add(new AchievementCheck(CheckStatBasedSimple, "Ace1", "AcesDestroyed", 1));
		m_achievementChecks.Add(new AchievementCheck(CheckStatBasedSimple, "Ace4", "AcesDestroyed", 4));
		m_achievementChecks.Add(new AchievementCheck(CheckStatBasedSimple, "Ace8", "AcesDestroyed", 8));
		m_achievementChecks.Add(new AchievementCheck(CheckAllFourFocus, "FocusFour", CheckType.ConstantInMissionOnly));
		m_achievementChecks.Add(new AchievementCheck(CheckFullSurviveAndSuccess, "SevenSurvive", CheckType.DebriefEnd));
		m_achievementChecks.Add(new AchievementCheck(CheckAllTargetsBombed, "MissionSuccess", CheckType.DebriefStart));
		m_achievementChecks.Add(new AchievementCheck(CheckForOneEngine, "FinishOneEngine", CheckType.OnLandBomberAtBase));
		m_achievementChecks.Add(new AchievementCheck(CheckForTwoOrFewer, "FinishOneOrTwoCrew", CheckType.DebriefEndAtBaseComplete));
		m_achievementChecks.Add(new AchievementCheck(CheckForLowFuel, "FinishLowFuel", CheckType.OnLandBomberAtBaseComplete));
		m_achievementChecks.Add(new AchievementCheck(CheckForAllBailedOut, "FullBailOut", CheckType.ConstantInMissionOnly));
		OnStatsUpdate();
	}

	private bool CheckForAllBailedOut(AchievementCheck ac)
	{
		int num = 0;
		foreach (CrewSpawner.CrewmanAvatarPairing item in Singleton<CrewSpawner>.Instance.GetAllCrew())
		{
			if (!item.m_crewman.IsDead() && item.m_spawnedAvatar.IsBailedOut())
			{
				num++;
			}
		}
		if (num == Singleton<GameFlow>.Instance.GetGameMode().GetMaxCrew())
		{
			return true;
		}
		return false;
	}

	private bool CheckForLowFuel(AchievementCheck ac)
	{
		FuelTank[] fuelTanks = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetFuelTanks();
		float num = 0f;
		FuelTank[] array = fuelTanks;
		foreach (FuelTank fuelTank in array)
		{
			num += fuelTank.GetFuelNormalised();
		}
		num /= (float)fuelTanks.Length;
		if (num <= 0.05f)
		{
			return true;
		}
		return false;
	}

	private bool CheckForTwoOrFewer(AchievementCheck ac)
	{
		int num = 0;
		int currentCrewCount = Singleton<CrewContainer>.Instance.GetCurrentCrewCount();
		for (int i = 0; i < currentCrewCount; i++)
		{
			Crewman crewman = Singleton<CrewContainer>.Instance.GetCrewman(i);
			if (!crewman.IsDead())
			{
				num++;
			}
		}
		if (num <= 2)
		{
			return true;
		}
		return false;
	}

	private bool CheckForOneEngine(AchievementCheck ac)
	{
		int engineCount = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetEngineCount();
		int num = 0;
		for (int i = 0; i < engineCount; i++)
		{
			Engine engine = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetEngine(i);
			if (engine != null && !engine.IsBroken())
			{
				num++;
			}
		}
		if (num == 1)
		{
			return true;
		}
		return false;
	}

	private bool CheckAllTargetsBombed(AchievementCheck ac)
	{
		if (!Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetCurrentlySelectedMissionDetails().m_isTrainingMission)
		{
			MissionLog missionLog = Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetMissionLog();
			if (missionLog.IsComplete())
			{
				if (missionLog.GetLandingState() == MissionLog.LandingState.AtBase)
				{
					return true;
				}
				return false;
			}
		}
		return false;
	}

	private bool CheckFullSurviveAndSuccess(AchievementCheck ac)
	{
		if (!Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetCurrentlySelectedMissionDetails().m_isTrainingMission)
		{
			int currentCrewCount = Singleton<CrewContainer>.Instance.GetCurrentCrewCount();
			if (Singleton<GameFlow>.Instance.GetGameMode().GetMaxCrew() == currentCrewCount)
			{
				bool flag = true;
				for (int i = 0; i < currentCrewCount; i++)
				{
					Crewman crewman = Singleton<CrewContainer>.Instance.GetCrewman(i);
					if (crewman.IsDead())
					{
						flag = false;
					}
				}
				if (flag)
				{
					MissionLog missionLog = Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetMissionLog();
					if (missionLog.IsComplete())
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	private bool CheckAllFourFocus(AchievementCheck ac)
	{
		if (Singleton<BomberSpawn>.Instance != null && Singleton<BomberSpawn>.Instance.GetBomberSystems() != null)
		{
			StationGunner stationGunner = (StationGunner)Singleton<BomberSpawn>.Instance.GetBomberSystems().GetStationFor(BomberSystems.StationType.GunsLowerDeck);
			if (stationGunner != null)
			{
				StationGunner stationGunner2 = (StationGunner)Singleton<BomberSpawn>.Instance.GetBomberSystems().GetStationFor(BomberSystems.StationType.GunsNose);
				StationGunner stationGunner3 = (StationGunner)Singleton<BomberSpawn>.Instance.GetBomberSystems().GetStationFor(BomberSystems.StationType.GunsTail);
				StationGunner stationGunner4 = (StationGunner)Singleton<BomberSpawn>.Instance.GetBomberSystems().GetStationFor(BomberSystems.StationType.GunsUpperDeck);
				if (stationGunner2 != null && stationGunner2.GetCurrentCrewman() != null && stationGunner3.GetCurrentCrewman() != null && stationGunner4.GetCurrentCrewman() != null && stationGunner.GetCurrentCrewman() != null && stationGunner2.GetFocusTimer().IsActive() && stationGunner3.GetFocusTimer().IsActive() && stationGunner4.GetFocusTimer().IsActive() && stationGunner.GetFocusTimer().IsActive())
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool CheckStatBasedSimple(AchievementCheck ac)
	{
		if (ac.m_userStat != null && ac.m_userStat.GetValue() >= ac.m_userStatRefValue)
		{
			return true;
		}
		return false;
	}

	private bool CheckFirstMissionComplete(AchievementCheck ac)
	{
		if (Singleton<GameFlow>.Instance.GetCurrentMissionInfo() != null && Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetCurrentlySelectedMission() != null && Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetCurrentlySelectedMission().name == "Mission_Tutorial1_lvl")
		{
			return true;
		}
		return false;
	}

	public void ContinueLoad()
	{
	}

	public string GetName()
	{
		return "AchievementMonitoring";
	}

	public LoadableSystem[] GetDependencies()
	{
		return new LoadableSystem[1] { Singleton<AchievementSystem>.Instance };
	}
}
