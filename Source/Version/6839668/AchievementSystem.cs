using System;
using BomberCrewCommon;
using Steamworks;
using UnityEngine;

public class AchievementSystem : Singleton<AchievementSystem>, LoadableSystem
{
	[Serializable]
	public class Achievement
	{
		[SerializeField]
		public string m_internalRef;

		[SerializeField]
		public string m_steamRefMain;

		[SerializeField]
		public string m_xboxRefMain;

		[SerializeField]
		public int m_ps4Ref;

		public bool m_unlocked;
	}

	[Serializable]
	public class UserStat
	{
		[SerializeField]
		public string m_internalRef;

		[SerializeField]
		public string m_steamStatReference;

		[SerializeField]
		public string m_xboxStatId;

		private int m_intValue;

		private int m_loadValue;

		public event Action OnStatUpdate;

		public int GetValue()
		{
			return m_intValue;
		}

		public void ModifyValue(int mod)
		{
			m_intValue += mod;
			if (this.OnStatUpdate != null)
			{
				this.OnStatUpdate();
			}
		}

		public void SetValueHighest(int highest)
		{
			if (highest > m_intValue)
			{
				m_intValue = highest;
				if (this.OnStatUpdate != null)
				{
					this.OnStatUpdate();
				}
			}
		}

		public bool HasUpdatedSinceLoad()
		{
			return m_intValue != m_loadValue;
		}

		public void SetValue(int val, bool fromSteamOrCache)
		{
			m_intValue = val;
			if (!fromSteamOrCache)
			{
				if (this.OnStatUpdate != null)
				{
					this.OnStatUpdate();
				}
			}
			else
			{
				m_loadValue = val;
			}
		}
	}

	[SerializeField]
	private UserStat[] m_userStats;

	[SerializeField]
	private Achievement[] m_achievements;

	[SerializeField]
	private TextAsset m_achievementManifest;

	private bool m_shouldUpdateStats;

	private bool m_hasAskedForStats;

	private bool m_hasReceivedStats;

	private float m_waitForStatsTimer;

	private CGameID m_gameID;

	protected Callback<UserStatsReceived_t> m_UserStatsReceived;

	protected Callback<UserStatsStored_t> m_UserStatsStored;

	protected Callback<UserAchievementStored_t> m_UserAchievementStored;

	private bool m_isInitialised;

	private bool m_isXboxInitialised;

	public event Action OnStatUpdate;

	private void Awake()
	{
		Singleton<SystemLoader>.Instance.RegisterLoadableSystem(this);
	}

	private void OnDestroy()
	{
	}

	private void AchievementsDebug()
	{
		if (SteamManager.Initialized && SteamAPI.IsSteamRunning() && GUILayout.Button("RESET ALL ACHIEVEMENTS & STATS"))
		{
			SteamUserStats.ResetAllStats(bAchievementsToo: true);
			m_shouldUpdateStats = true;
		}
	}

	private void OnUserStatsReceived(UserStatsReceived_t pCallback)
	{
		if (SteamManager.Initialized && (ulong)m_gameID == pCallback.m_nGameID && pCallback.m_eResult == EResult.k_EResultOK)
		{
			m_hasReceivedStats = true;
			UserStat[] userStats = m_userStats;
			foreach (UserStat userStat in userStats)
			{
				int pData = 0;
				SteamUserStats.GetStat(userStat.m_steamStatReference, out pData);
				userStat.SetValue(pData, fromSteamOrCache: true);
			}
			Achievement[] achievements = m_achievements;
			foreach (Achievement achievement in achievements)
			{
				bool pbAchieved = false;
				SteamUserStats.GetAchievement(achievement.m_steamRefMain, out pbAchieved);
				achievement.m_unlocked = pbAchieved;
			}
			if (this.OnStatUpdate != null)
			{
				this.OnStatUpdate();
			}
		}
	}

	public UserStat GetUserStat(string token)
	{
		UserStat[] userStats = m_userStats;
		foreach (UserStat userStat in userStats)
		{
			if (userStat.m_internalRef == token)
			{
				return userStat;
			}
		}
		DebugLogWrapper.LogError("Could not find user stat for: " + token);
		return null;
	}

	public Achievement GetAchievement(string token)
	{
		Achievement[] achievements = m_achievements;
		foreach (Achievement achievement in achievements)
		{
			if (achievement.m_internalRef == token)
			{
				return achievement;
			}
		}
		DebugLogWrapper.LogError("Could not find achievement for: " + token);
		return null;
	}

	public void AwardAchievement(Achievement ach)
	{
		if (SteamManager.Initialized && SteamAPI.IsSteamRunning() && !ach.m_unlocked)
		{
			if (SteamUserStats.SetAchievement(ach.m_steamRefMain))
			{
				ach.m_unlocked = true;
			}
			m_shouldUpdateStats = true;
		}
	}

	private void Update()
	{
		if (!m_isInitialised || !SteamManager.Initialized || !SteamAPI.IsSteamRunning())
		{
			return;
		}
		if (m_shouldUpdateStats)
		{
			UserStat[] userStats = m_userStats;
			foreach (UserStat userStat in userStats)
			{
				SteamUserStats.SetStat(userStat.m_steamStatReference, userStat.GetValue());
			}
			if (SteamUserStats.StoreStats())
			{
				m_shouldUpdateStats = false;
				if (this.OnStatUpdate != null)
				{
					this.OnStatUpdate();
				}
			}
		}
		if (m_hasReceivedStats)
		{
			return;
		}
		if (!m_hasAskedForStats)
		{
			m_hasAskedForStats = SteamUserStats.RequestCurrentStats();
			return;
		}
		m_waitForStatsTimer += Time.deltaTime;
		if (m_waitForStatsTimer > 30f)
		{
			m_waitForStatsTimer = 0f;
			m_hasAskedForStats = false;
		}
	}

	public bool IsLoadComplete()
	{
		return m_isInitialised;
	}

	public void StartLoad()
	{
		if (SteamManager.Initialized && SteamAPI.IsSteamRunning())
		{
			m_gameID = new CGameID(SteamUtils.GetAppID());
			Achievement[] achievements = m_achievements;
			foreach (Achievement achievement in achievements)
			{
				achievement.m_unlocked = false;
			}
			UserStat[] userStats = m_userStats;
			foreach (UserStat userStat in userStats)
			{
				userStat.OnStatUpdate += delegate
				{
					if (this.OnStatUpdate != null)
					{
						this.OnStatUpdate();
					}
					m_shouldUpdateStats = true;
				};
			}
			m_UserStatsReceived = Callback<UserStatsReceived_t>.Create(OnUserStatsReceived);
		}
		m_isInitialised = true;
	}

	public void ContinueLoad()
	{
	}

	public string GetName()
	{
		return "Achievements";
	}

	public LoadableSystem[] GetDependencies()
	{
		return null;
	}
}
