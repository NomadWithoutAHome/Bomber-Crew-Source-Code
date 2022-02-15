using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using BomberCrewCommon;
using GameAnalyticsSDK;
using Newtonsoft.Json;
using Steamworks;
using UnityEngine.Analytics;

[JsonObject(MemberSerialization.OptIn)]
public class SaveData
{
	[JsonObject(MemberSerialization.OptIn)]
	public class StockedItems
	{
		[JsonProperty]
		public int m_itemCount;
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class PlayedMissionInfo
	{
		[JsonProperty]
		public string m_missionReference;

		[JsonProperty]
		public bool m_completed;

		[JsonProperty]
		public int m_lastPlayedNumber;

		[JsonProperty]
		public int m_numberOfTimesPlayed;
	}

	public enum PerkType
	{
		None,
		EnemyDamageDown,
		EnemyHealthDown,
		FlakInterruptions
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class ActivePerk
	{
		[JsonProperty]
		public PerkType m_perkType;

		[JsonProperty]
		public float m_reduceAmountFactor;

		[JsonProperty]
		public int m_numMissionRemaining;
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class CrewCompletedMission
	{
		[JsonProperty]
		public string m_missionReference;

		[JsonProperty]
		public Crewman[] m_crewCompletedWith;

		[JsonProperty]
		public BomberUpgradeConfig m_bomberUpgradeConfig;
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class DifficultyHistorySummary
	{
		[JsonProperty]
		public int m_crewLost;

		[JsonProperty]
		public bool m_missionComplete;
	}

	[JsonProperty]
	private List<CrewCompletedMission> m_crewCompletedMissions = new List<CrewCompletedMission>();

	[JsonProperty]
	private List<Crewman> m_activeCrewmen = new List<Crewman>();

	[JsonProperty]
	private List<Crewman> m_pastCrewmen = new List<Crewman>();

	[JsonProperty]
	private BomberUpgradeConfig m_currentConfig;

	[JsonProperty]
	private int m_currentBalance;

	[JsonProperty]
	private int m_currentIntel;

	[JsonProperty]
	private Dictionary<string, StockedItems> m_crewGearStocks = new Dictionary<string, StockedItems>();

	[JsonProperty]
	private int m_numMissionsPlayed;

	[JsonProperty]
	private List<string> m_viewedItems = new List<string>();

	private HashSet<string> m_viewedItemsHash = new HashSet<string>();

	[JsonProperty]
	private List<string> m_announcedItems = new List<string>();

	private HashSet<string> m_announcedItemsHash = new HashSet<string>();

	[JsonProperty]
	private List<PlayedMissionInfo> m_playedMissions = new List<PlayedMissionInfo>();

	[JsonProperty]
	private Dictionary<string, float> m_hintShownForTimes = new Dictionary<string, float>();

	[JsonProperty]
	private List<string> m_defeatedAceFighters = new List<string>();

	[JsonProperty]
	private List<string> m_encounteredAceFighters = new List<string>();

	[JsonProperty]
	private float m_enemyAceChanceCounter;

	[JsonProperty]
	private DateTime m_saveTime;

	[JsonProperty]
	private TimeSpan m_saveTimeSpan;

	[JsonProperty]
	private List<ActivePerk> m_currentPerks = new List<ActivePerk>();

	[JsonProperty]
	private Guid m_trackingGUID;

	[JsonProperty]
	private bool m_isCampaign;

	[JsonProperty]
	private bool m_skippedTutorial;

	[JsonProperty]
	private Dictionary<string, int> m_previousBomberNames = new Dictionary<string, int>();

	[JsonProperty]
	private List<string> m_allDLC = new List<string>();

	[JsonProperty]
	private bool m_hasEverCorkscrewed;

	[JsonProperty]
	private bool m_hasEverFocused;

	[JsonProperty]
	private Dictionary<string, int> m_numTimesEncounteredAce = new Dictionary<string, int>();

	[JsonProperty]
	private string m_lastDLCCampaignSelected;

	[JsonProperty]
	private bool m_hasStats;

	[JsonProperty]
	private int m_numFightersDestroyed;

	[JsonProperty]
	private int m_numTargetsDestroyed;

	[JsonProperty]
	private int m_fundsSpent;

	[JsonProperty]
	private int m_milesTravelled;

	[JsonProperty]
	private int m_bombersLost;

	[JsonProperty]
	private int m_crewLost;

	[JsonProperty]
	private int m_crewRescued;

	[JsonProperty]
	private int m_numReconPhotosTaken;

	[JsonProperty]
	private int m_numMissionsFlownActual;

	[JsonProperty]
	private Dictionary<string, EndlessModeVariant.LoadoutJS> m_lastSeenEndlessLoadouts;

	[JsonProperty]
	private Dictionary<string, string> m_lastUpdatedSaveData;

	[JsonProperty]
	private List<DifficultyHistorySummary> m_difficultyHistory = new List<DifficultyHistorySummary>();

	public event Action OnSaveUpdate;

	public void AddPostMissionStats(int fightersDestroyed, int targetsDestroyed, int milesTravelled, bool bomberLost, int crewLost, int crewRescued, int reconPhotos)
	{
		m_numMissionsFlownActual++;
		m_numFightersDestroyed += fightersDestroyed;
		m_numTargetsDestroyed += targetsDestroyed;
		m_milesTravelled += milesTravelled;
		m_bombersLost += (bomberLost ? 1 : 0);
		m_crewLost += crewLost;
		m_crewRescued += crewRescued;
		m_numReconPhotosTaken += reconPhotos;
	}

	public EndlessModeVariant.LoadoutJS LastSeenEndlessLoadout(string forMode)
	{
		if (m_lastSeenEndlessLoadouts == null)
		{
			m_lastSeenEndlessLoadouts = new Dictionary<string, EndlessModeVariant.LoadoutJS>();
		}
		EndlessModeVariant.LoadoutJS value = null;
		m_lastSeenEndlessLoadouts.TryGetValue(forMode, out value);
		if (value != null)
		{
			string uniqueString = value.GetUniqueString();
			if (m_lastUpdatedSaveData == null)
			{
				m_lastUpdatedSaveData = new Dictionary<string, string>();
			}
			string value2 = null;
			m_lastUpdatedSaveData.TryGetValue(forMode, out value2);
			if (value2 == null || value2 != uniqueString)
			{
				return null;
			}
		}
		return value;
	}

	public void SetLastSeenEndlessLoadout(string forMode, EndlessModeVariant.LoadoutJS loadout)
	{
		if (m_lastSeenEndlessLoadouts == null)
		{
			m_lastSeenEndlessLoadouts = new Dictionary<string, EndlessModeVariant.LoadoutJS>();
		}
		m_lastSeenEndlessLoadouts[forMode] = loadout;
		string uniqueString = loadout.GetUniqueString();
		if (m_lastUpdatedSaveData == null)
		{
			m_lastUpdatedSaveData = new Dictionary<string, string>();
		}
		m_lastUpdatedSaveData[forMode] = uniqueString;
	}

	public int GetNumMissionsFlownActual()
	{
		return m_numMissionsFlownActual;
	}

	public bool HasStats()
	{
		return m_hasStats;
	}

	public int GetNumFightersDestroyed()
	{
		return m_numFightersDestroyed;
	}

	public int GetNumTargetsDestroyed()
	{
		return m_numTargetsDestroyed;
	}

	public int GetNumMilesTravelled()
	{
		return m_milesTravelled;
	}

	public int GetNumBombersLost()
	{
		return m_bombersLost;
	}

	public int GetNumCrewLost()
	{
		return m_crewLost;
	}

	public int GetNumCrewRescued()
	{
		return m_crewRescued;
	}

	public void SpendFunds(int fundsSpent)
	{
		m_fundsSpent += fundsSpent;
	}

	public int GetFundsSpent()
	{
		return m_fundsSpent;
	}

	public int GetNumReconPhotosTaken()
	{
		return m_numReconPhotosTaken;
	}

	public BomberUpgradeConfig GetCurrentBomber()
	{
		return m_currentConfig;
	}

	public DateTime GetSaveTime()
	{
		return m_saveTime;
	}

	public bool HasEverCorkscrewed()
	{
		return m_hasEverCorkscrewed;
	}

	public string GetLastDLCCampaignSelected()
	{
		return m_lastDLCCampaignSelected;
	}

	public void SetLastDLCCampaignSelected(string dlcCamp)
	{
		m_lastDLCCampaignSelected = dlcCamp;
	}

	public void SetHasCorkscrewed()
	{
		m_hasEverCorkscrewed = true;
	}

	public bool HasEverFocused()
	{
		return m_hasEverFocused;
	}

	public void SetHasFocused()
	{
		m_hasEverFocused = true;
	}

	public void SetIsCampaign(bool skippedTutorial)
	{
		m_isCampaign = true;
		m_skippedTutorial = skippedTutorial;
		Random random = new Random((int)(DateTime.UtcNow.Ticks % 65535));
		byte[] array = new byte[16];
		random.NextBytes(array);
		m_trackingGUID = new Guid(array);
	}

	public void AddToDifficultyHistory(int numCrewLost, bool complete)
	{
		if (m_difficultyHistory == null)
		{
			m_difficultyHistory = new List<DifficultyHistorySummary>();
		}
		DifficultyHistorySummary difficultyHistorySummary = new DifficultyHistorySummary();
		difficultyHistorySummary.m_crewLost = numCrewLost;
		difficultyHistorySummary.m_missionComplete = complete;
		m_difficultyHistory.Add(difficultyHistorySummary);
		if (m_difficultyHistory.Count > 3)
		{
			m_difficultyHistory.RemoveAt(0);
		}
	}

	public int GetDifficultyBump()
	{
		if (m_difficultyHistory == null)
		{
			m_difficultyHistory = new List<DifficultyHistorySummary>();
			return 0;
		}
		if (m_difficultyHistory.Count == 0)
		{
			return 0;
		}
		bool flag = true;
		bool flag2 = true;
		int num = 0;
		int num2 = 0;
		foreach (DifficultyHistorySummary item in m_difficultyHistory)
		{
			if (item.m_missionComplete)
			{
				flag2 = false;
			}
			else
			{
				flag = true;
			}
			num = item.m_crewLost;
			num2 += item.m_crewLost;
		}
		if (m_difficultyHistory.Count > 2)
		{
			if (flag2 && num2 > 0)
			{
				if (num2 > 10 || num > 6)
				{
					return -2;
				}
				return -1;
			}
			if (num2 > 10)
			{
				return -1;
			}
			if (flag)
			{
				if (num2 == 0)
				{
					return 2;
				}
				if (num2 < 5)
				{
					return 1;
				}
			}
			else if (!flag2 && num2 == 0)
			{
				return 1;
			}
		}
		if (num > 4 && !flag)
		{
			return -1;
		}
		if (num > 6)
		{
			return -1;
		}
		return 0;
	}

	public bool GetIsCampaign()
	{
		return m_isCampaign;
	}

	public int GetNumPreviousBombersOfName(string name)
	{
		if (m_previousBomberNames == null)
		{
			m_previousBomberNames = new Dictionary<string, int>();
		}
		int value = 0;
		m_previousBomberNames.TryGetValue(name, out value);
		return value;
	}

	public void SetBomberDestroyed(string name)
	{
		if (m_previousBomberNames == null)
		{
			m_previousBomberNames = new Dictionary<string, int>();
		}
		if (string.IsNullOrEmpty(name))
		{
			return;
		}
		int numPreviousBombersOfName = GetNumPreviousBombersOfName(name);
		m_previousBomberNames[name] = numPreviousBombersOfName + 1;
		if (m_previousBomberNames.Count <= 100)
		{
			return;
		}
		string text = null;
		using (Dictionary<string, int>.Enumerator enumerator = m_previousBomberNames.GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				text = enumerator.Current.Key;
			}
		}
		if (!string.IsNullOrEmpty(text))
		{
			m_previousBomberNames.Remove(text);
		}
	}

	public string GetCampaignTrackingID()
	{
		if (SteamManager.Initialized)
		{
			return SteamFriends.GetPersonaName();
		}
		return Environment.MachineName;
	}

	public void AddTime(float amt)
	{
		m_saveTimeSpan += TimeSpan.FromSeconds(amt);
	}

	public TimeSpan GetTimePlayed()
	{
		return m_saveTimeSpan;
	}

	public bool IsAceDefeated(string aceRef)
	{
		if (m_defeatedAceFighters == null)
		{
			m_defeatedAceFighters = new List<string>();
		}
		return m_defeatedAceFighters.Contains(aceRef);
	}

	public List<string> GetKeysUnlocked(CampaignStructure cs)
	{
		List<string> list = new List<string>();
		CampaignStructure.CampaignMission[] allMissions = cs.GetAllMissions();
		foreach (CampaignStructure.CampaignMission campaignMission in allMissions)
		{
			if (!string.IsNullOrEmpty(campaignMission.m_unlockOnCompleteTag) && HasCompletedMission(campaignMission.m_missionReferenceName))
			{
				list.Add(campaignMission.m_unlockOnCompleteTag);
			}
			if (!string.IsNullOrEmpty(campaignMission.m_unlockOnFinishTag) && HasPlayedMission(campaignMission.m_missionReferenceName))
			{
				list.Add(campaignMission.m_unlockOnFinishTag);
			}
		}
		return list;
	}

	public void SetAceDefeated(string aceRef)
	{
		if (m_defeatedAceFighters == null)
		{
			m_defeatedAceFighters = new List<string>();
		}
		if (!m_defeatedAceFighters.Contains(aceRef))
		{
			m_defeatedAceFighters.Add(aceRef);
		}
		AchievementSystem.UserStat userStat = Singleton<AchievementSystem>.Instance.GetUserStat("AcesDestroyed");
		if (userStat != null)
		{
			if (Singleton<GameFlow>.Instance.GetGameMode().UseEndlessDifficulty())
			{
				userStat.SetValueHighest((m_defeatedAceFighters.Count != 0) ? 1 : 0);
			}
			else
			{
				userStat.SetValueHighest(m_defeatedAceFighters.Count);
			}
		}
	}

	public int GetNumDefeatedAces()
	{
		return m_defeatedAceFighters.Count;
	}

	public void SetAceEncountered(string aceRef)
	{
		if (m_encounteredAceFighters == null)
		{
			m_encounteredAceFighters = new List<string>();
		}
		if (!m_encounteredAceFighters.Contains(aceRef))
		{
			m_encounteredAceFighters.Add(aceRef);
		}
		if (m_numTimesEncounteredAce == null)
		{
			m_numTimesEncounteredAce = new Dictionary<string, int>();
		}
		int value = 0;
		m_numTimesEncounteredAce.TryGetValue(aceRef, out value);
		m_numTimesEncounteredAce[aceRef] = value + 1;
		AchievementSystem.Achievement achievement = Singleton<AchievementSystem>.Instance.GetAchievement("AceEncounter");
		if (achievement != null)
		{
			Singleton<AchievementSystem>.Instance.AwardAchievement(achievement);
		}
	}

	public bool HasPreviouslyEncountered(string aceRef)
	{
		if (m_encounteredAceFighters == null)
		{
			m_encounteredAceFighters = new List<string>();
		}
		return m_encounteredAceFighters.Contains(aceRef);
	}

	public int GetNumTimesEncountered(string aceRef)
	{
		if (m_numTimesEncounteredAce == null)
		{
			m_numTimesEncounteredAce = new Dictionary<string, int>();
		}
		int value = 0;
		m_numTimesEncounteredAce.TryGetValue(aceRef, out value);
		return value;
	}

	public float GetAceChanceCounter()
	{
		return m_enemyAceChanceCounter;
	}

	public void SetAceChanceCounter(float val)
	{
		m_enemyAceChanceCounter = val;
	}

	public void SetUp()
	{
		m_currentBalance = 0;
		m_currentConfig = Singleton<BomberContainer>.Instance.GetNewBomber(this);
		m_hasStats = true;
	}

	public bool HasBeenViewed(string name)
	{
		return m_viewedItemsHash.Contains(name) || m_currentConfig.ContainsUpgrade(name);
	}

	public bool HasBeenAnnounced(string name)
	{
		return m_announcedItemsHash.Contains(name) || m_currentConfig.ContainsUpgrade(name);
	}

	public void SetAnnounced(string name)
	{
		if (m_announcedItems == null)
		{
			m_announcedItems = new List<string>();
		}
		m_announcedItemsHash.Add(name);
		if (!m_announcedItems.Contains(name))
		{
			m_announcedItems.Add(name);
		}
	}

	public void SetViewed(string name)
	{
		if (m_viewedItems == null)
		{
			m_viewedItems = new List<string>();
		}
		m_viewedItemsHash.Add(name);
		if (!m_viewedItems.Contains(name))
		{
			m_viewedItems.Add(name);
		}
	}

	public void ResetToNewBomber()
	{
		Singleton<SaveDataContainer>.Instance.Get().SetBomberDestroyed(Singleton<BomberContainer>.Instance.GetCurrentConfig().GetName());
		m_currentConfig = Singleton<BomberContainer>.Instance.GetNewBomber(this);
		Singleton<BomberContainer>.Instance.GetLivery().Reset();
	}

	public float GetTimeHintShownFor(string id)
	{
		if (m_hintShownForTimes == null)
		{
			m_hintShownForTimes = new Dictionary<string, float>();
		}
		float value = 0f;
		m_hintShownForTimes.TryGetValue(id, out value);
		return value;
	}

	public void AddTimeHintShownFor(string id, float time)
	{
		if (m_hintShownForTimes == null)
		{
			m_hintShownForTimes = new Dictionary<string, float>();
		}
		float value = 0f;
		m_hintShownForTimes.TryGetValue(id, out value);
		m_hintShownForTimes[id] = value + time;
	}

	public void AddPerk(PerkType pt, float amt, int forMissions)
	{
		if (pt != 0)
		{
			ActivePerk activePerk = new ActivePerk();
			activePerk.m_perkType = pt;
			activePerk.m_reduceAmountFactor = amt;
			activePerk.m_numMissionRemaining = forMissions;
			m_currentPerks.Add(activePerk);
		}
	}

	public List<ActivePerk> GetCurrentPerks()
	{
		return m_currentPerks;
	}

	public float PerkGetFighterDamageReduction()
	{
		float num = 1f;
		foreach (ActivePerk currentPerk in m_currentPerks)
		{
			if (currentPerk.m_perkType == PerkType.EnemyDamageDown)
			{
				num *= 1f - currentPerk.m_reduceAmountFactor;
			}
		}
		return num;
	}

	public float PerkGetFighterHealthReduction()
	{
		float num = 1f;
		foreach (ActivePerk currentPerk in m_currentPerks)
		{
			if (currentPerk.m_perkType == PerkType.EnemyHealthDown)
			{
				num *= 1f - currentPerk.m_reduceAmountFactor;
			}
		}
		return num;
	}

	public float PerkGetFlakUpTime()
	{
		float num = 1f;
		foreach (ActivePerk currentPerk in m_currentPerks)
		{
			if (currentPerk.m_perkType == PerkType.FlakInterruptions)
			{
				num *= 1f - currentPerk.m_reduceAmountFactor;
			}
		}
		return num;
	}

	public void LowerPerkCount()
	{
		if (m_currentPerks == null)
		{
			m_currentPerks = new List<ActivePerk>();
		}
		List<ActivePerk> list = new List<ActivePerk>();
		foreach (ActivePerk currentPerk in m_currentPerks)
		{
			currentPerk.m_numMissionRemaining--;
			if (currentPerk.m_numMissionRemaining <= 0)
			{
				list.Add(currentPerk);
			}
		}
		foreach (ActivePerk item in list)
		{
			m_currentPerks.Remove(item);
		}
	}

	public void SetDLC(List<string> allDLC)
	{
		m_allDLC = new List<string>();
		m_allDLC.AddRange(allDLC);
	}

	public bool ShouldShowDLCWarning(List<string> allDLC)
	{
		if (m_allDLC == null)
		{
			m_allDLC = new List<string>();
		}
		foreach (string item in m_allDLC)
		{
			if (!allDLC.Contains(item))
			{
				return true;
			}
		}
		return false;
	}

	public void SetMissionPlayed(string campaignReference, bool completed, bool saveCrewAndBomber, List<Crewman> addCrewmen)
	{
		if (m_playedMissions == null)
		{
			m_playedMissions = new List<PlayedMissionInfo>();
		}
		if (m_isCampaign)
		{
			GameAnalytics.NewProgressionEvent((!completed) ? GAProgressionStatus.Fail : GAProgressionStatus.Complete, campaignReference);
		}
		if (m_isCampaign)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["missionName"] = campaignReference;
			Analytics.CustomEvent((!completed) ? "MissionFailed" : "MissionComplete", dictionary);
		}
		PlayedMissionInfo playedMissionInfo = null;
		foreach (PlayedMissionInfo playedMission in m_playedMissions)
		{
			if (playedMission.m_missionReference == campaignReference)
			{
				playedMissionInfo = playedMission;
				break;
			}
		}
		if (playedMissionInfo == null)
		{
			playedMissionInfo = new PlayedMissionInfo();
			playedMissionInfo.m_missionReference = campaignReference;
			playedMissionInfo.m_numberOfTimesPlayed = 0;
			m_playedMissions.Add(playedMissionInfo);
		}
		playedMissionInfo.m_completed |= completed;
		playedMissionInfo.m_lastPlayedNumber = m_numMissionsPlayed;
		playedMissionInfo.m_numberOfTimesPlayed++;
		m_numMissionsPlayed++;
		if (!saveCrewAndBomber)
		{
			return;
		}
		CrewCompletedMission crewCompletedMission = new CrewCompletedMission();
		crewCompletedMission.m_bomberUpgradeConfig = new BomberUpgradeConfig(m_currentConfig);
		crewCompletedMission.m_crewCompletedWith = addCrewmen.ToArray();
		crewCompletedMission.m_missionReference = campaignReference;
		if (m_crewCompletedMissions == null)
		{
			m_crewCompletedMissions = new List<CrewCompletedMission>();
		}
		bool flag = true;
		foreach (CrewCompletedMission crewCompletedMission2 in m_crewCompletedMissions)
		{
			if (crewCompletedMission2.m_missionReference == campaignReference)
			{
				flag = false;
			}
		}
		if (flag)
		{
			m_crewCompletedMissions.Add(crewCompletedMission);
		}
	}

	public List<CrewCompletedMission> GetCrewCompletedMissions()
	{
		if (m_crewCompletedMissions == null)
		{
			m_crewCompletedMissions = new List<CrewCompletedMission>();
		}
		return m_crewCompletedMissions;
	}

	public int GetNumMissionsPlayed()
	{
		return m_numMissionsPlayed;
	}

	public List<Crewman> GetActiveCrewmen()
	{
		return m_activeCrewmen;
	}

	public Crewman GetCrewmanByIndex(int index)
	{
		if (index < m_activeCrewmen.Count)
		{
			return m_activeCrewmen[index];
		}
		return null;
	}

	public void MoveCrewmenToPast(Crewman crewman)
	{
		m_activeCrewmen.Remove(crewman);
		m_pastCrewmen.Add(crewman);
	}

	public List<PlayedMissionInfo> GetPlayedMissionInfo()
	{
		if (m_playedMissions == null)
		{
			m_playedMissions = new List<PlayedMissionInfo>();
		}
		return m_playedMissions;
	}

	public bool HasCompletedMission(string missionRef)
	{
		if (m_playedMissions == null)
		{
			m_playedMissions = new List<PlayedMissionInfo>();
		}
		foreach (PlayedMissionInfo playedMission in m_playedMissions)
		{
			if (playedMission.m_missionReference == missionRef && playedMission.m_completed)
			{
				return true;
			}
		}
		return false;
	}

	public int GetLastPlayedTime(string missionRef)
	{
		if (m_playedMissions == null)
		{
			m_playedMissions = new List<PlayedMissionInfo>();
		}
		foreach (PlayedMissionInfo playedMission in m_playedMissions)
		{
			if (playedMission.m_missionReference == missionRef)
			{
				return playedMission.m_lastPlayedNumber;
			}
		}
		return -1;
	}

	public int GetNumberOfTimesPlayed(string missionRef)
	{
		if (m_playedMissions == null)
		{
			m_playedMissions = new List<PlayedMissionInfo>();
		}
		foreach (PlayedMissionInfo playedMission in m_playedMissions)
		{
			if (playedMission.m_missionReference == missionRef)
			{
				return playedMission.m_numberOfTimesPlayed;
			}
		}
		return 0;
	}

	public bool HasPlayedMission(string missionRef)
	{
		if (m_playedMissions == null)
		{
			m_playedMissions = new List<PlayedMissionInfo>();
		}
		foreach (PlayedMissionInfo playedMission in m_playedMissions)
		{
			if (playedMission.m_missionReference == missionRef)
			{
				return true;
			}
		}
		return false;
	}

	public void AddNewCrewman(Crewman crewman)
	{
		m_activeCrewmen.Add(crewman);
	}

	public int GetBalance()
	{
		return m_currentBalance;
	}

	public void AddBalance(int amt)
	{
		m_currentBalance += amt;
	}

	public int GetIntel()
	{
		return m_currentIntel;
	}

	public void AddIntel(int amt)
	{
		m_currentIntel += amt;
	}

	[OnDeserialized]
	private void OnDeserializedMethod(StreamingContext context)
	{
		Singleton<BomberContainer>.Instance.PatchUp(m_currentConfig, this);
		if (m_crewGearStocks == null)
		{
			m_crewGearStocks = new Dictionary<string, StockedItems>();
		}
		if (m_pastCrewmen == null)
		{
			m_pastCrewmen = new List<Crewman>();
		}
		if (m_activeCrewmen == null)
		{
			m_activeCrewmen = new List<Crewman>();
		}
		List<Crewman> list = new List<Crewman>();
		foreach (Crewman activeCrewman in m_activeCrewmen)
		{
			if (activeCrewman.IsDead())
			{
				list.Add(activeCrewman);
			}
		}
		foreach (Crewman item in list)
		{
			m_activeCrewmen.Remove(item);
		}
		foreach (string viewedItem in m_viewedItems)
		{
			m_viewedItemsHash.Add(viewedItem);
		}
		m_viewedItems.Clear();
		m_viewedItems.AddRange(m_viewedItemsHash);
		foreach (string announcedItem in m_announcedItems)
		{
			m_announcedItemsHash.Add(announcedItem);
		}
		m_announcedItems.Clear();
		m_announcedItems.AddRange(m_announcedItemsHash);
	}

	[OnSerializing]
	private void OnSerializingMethod(StreamingContext context)
	{
		m_saveTime = DateTime.Now;
	}

	public int GetStockForCrewGear(CrewmanEquipmentBase ceb)
	{
		if (ceb != null && ceb.GetCost() != 0)
		{
			StockedItems value = null;
			m_crewGearStocks.TryGetValue(ceb.name, out value);
			if (value == null)
			{
				value = new StockedItems();
				m_crewGearStocks[ceb.name] = value;
			}
			return value.m_itemCount;
		}
		return 0;
	}

	public void SetBalance(int balance)
	{
		m_currentBalance = balance;
	}

	public void SetIntel(int intel)
	{
		m_currentIntel = intel;
	}

	public void ModifyStockForCrewGear(CrewmanEquipmentBase ceb, int i)
	{
		if (ceb != null && ceb.GetCost() != 0)
		{
			StockedItems value = null;
			m_crewGearStocks.TryGetValue(ceb.name, out value);
			if (value == null)
			{
				value = new StockedItems();
				m_crewGearStocks[ceb.name] = value;
			}
			value.m_itemCount += i;
		}
	}
}
