using System.Collections.Generic;
using BomberCrewCommon;
using Steamworks;

public class HiScoreProviderSteam : HiScoreProvider
{
	public class HiScoreTableDataSteam
	{
		public SteamLeaderboard_t m_leaderboard;

		public bool m_hasLeaderboardData;

		public bool m_hasLeaderboardDataTop10;

		public bool m_hasLeaderboardDataFriends;

		public bool m_hasLeaderboardDataAroundPlayer;
	}

	private enum OperationTypes
	{
		FetchLeaderboardInfo,
		FetchLeaderboardScores,
		UploadScore
	}

	private class OperationDetails
	{
		public OperationTypes m_operation;

		public HiScoreTable m_relatedTable;
	}

	private CallResult<LeaderboardFindResult_t> OnLeaderboardFindResultCallResult;

	private CallResult<LeaderboardScoresDownloaded_t> OnLeaderboardScoresDownloadedCallResult;

	private CallResult<LeaderboardScoresDownloaded_t> OnLeaderboardScoresDownloadedTop10CallResult;

	private CallResult<LeaderboardScoresDownloaded_t> OnLeaderboardScoresDownloadedFriendsCallResult;

	private CallResult<LeaderboardScoresDownloaded_t> OnLeaderboardScoresDownloadedForCurrentUserCallResult;

	private CallResult<LeaderboardScoreUploaded_t> OnLeaderboardScoreUploadedCallResult;

	private bool m_requiresRefresh;

	private List<HiScoreTable> m_allTables = new List<HiScoreTable>();

	private OperationDetails m_operationDetails;

	public override void SetUp()
	{
		OnLeaderboardFindResultCallResult = CallResult<LeaderboardFindResult_t>.Create(OnLeaderboardFindResult);
		OnLeaderboardScoresDownloadedCallResult = CallResult<LeaderboardScoresDownloaded_t>.Create(OnLeaderboardScoresDownloaded);
		OnLeaderboardScoresDownloadedTop10CallResult = CallResult<LeaderboardScoresDownloaded_t>.Create(OnLeaderboardScoresDownloadedTop10);
		OnLeaderboardScoresDownloadedFriendsCallResult = CallResult<LeaderboardScoresDownloaded_t>.Create(OnLeaderboardScoresDownloadedFriends);
		OnLeaderboardScoresDownloadedForCurrentUserCallResult = CallResult<LeaderboardScoresDownloaded_t>.Create(OnLeaderboardScoresDownloadedForCurrentUser);
		OnLeaderboardScoreUploadedCallResult = CallResult<LeaderboardScoreUploaded_t>.Create(OnLeaderboardScoreUploaded);
	}

	private void OnLeaderboardFindResult(LeaderboardFindResult_t pCallback, bool bIOFailure)
	{
		bool success = false;
		if (!bIOFailure && pCallback.m_bLeaderboardFound != 0)
		{
			HiScoreTableDataSteam associatedData = m_operationDetails.m_relatedTable.GetAssociatedData<HiScoreTableDataSteam>();
			associatedData.m_hasLeaderboardData = true;
			associatedData.m_leaderboard = pCallback.m_hSteamLeaderboard;
			success = true;
		}
		m_operationDetails = null;
		DoSuccess(success);
	}

	private void OnLeaderboardScoresDownloaded(LeaderboardScoresDownloaded_t pCallback, bool bIOFailure)
	{
		bool success = false;
		if (pCallback.m_cEntryCount != 0 && !bIOFailure)
		{
			int playerIndex = -1;
			List<HiScoreTable.HiScore> scores = ScoreListToLocal(pCallback, m_operationDetails.m_relatedTable, out playerIndex);
			m_operationDetails.m_relatedTable.SetHiScores(scores, HiScoreTable.ScoreListType.NearMyScore);
			success = true;
			HiScoreTableDataSteam associatedData = m_operationDetails.m_relatedTable.GetAssociatedData<HiScoreTableDataSteam>();
			associatedData.m_hasLeaderboardDataAroundPlayer = true;
		}
		else if (pCallback.m_cEntryCount == 0)
		{
			List<HiScoreTable.HiScore> scores2 = new List<HiScoreTable.HiScore>();
			m_operationDetails.m_relatedTable.SetHiScores(scores2, HiScoreTable.ScoreListType.NearMyScore);
			success = true;
			HiScoreTableDataSteam associatedData2 = m_operationDetails.m_relatedTable.GetAssociatedData<HiScoreTableDataSteam>();
			associatedData2.m_hasLeaderboardDataAroundPlayer = true;
		}
		m_operationDetails = null;
		DoSuccess(success);
	}

	private List<HiScoreTable.HiScore> ScoreListToLocal(LeaderboardScoresDownloaded_t pCallback, HiScoreTable table, out int playerIndex)
	{
		List<HiScoreTable.HiScore> list = new List<HiScoreTable.HiScore>();
		playerIndex = -1;
		for (int i = 0; i < pCallback.m_cEntryCount; i++)
		{
			SteamUserStats.GetDownloadedLeaderboardEntry(pCallback.m_hSteamLeaderboardEntries, i, out var pLeaderboardEntry, null, 0);
			HiScoreTable.HiScore hiScore = new HiScoreTable.HiScore();
			hiScore.m_score = pLeaderboardEntry.m_nScore;
			hiScore.m_rank = pLeaderboardEntry.m_nGlobalRank;
			hiScore.m_name = SteamFriends.GetFriendPersonaName(pLeaderboardEntry.m_steamIDUser);
			if (pLeaderboardEntry.m_steamIDUser == SteamUser.GetSteamID())
			{
				playerIndex = i;
				hiScore.m_isLocalPlayer = true;
				table.SetHiScoreFetched(hiScore.m_score, hiScore.m_rank, hiScore.m_name);
			}
			list.Add(hiScore);
		}
		return list;
	}

	private void OnLeaderboardScoresDownloadedTop10(LeaderboardScoresDownloaded_t pCallback, bool bIOFailure)
	{
		bool success = false;
		if (pCallback.m_cEntryCount != 0 && !bIOFailure)
		{
			int playerIndex = -1;
			List<HiScoreTable.HiScore> scores = ScoreListToLocal(pCallback, m_operationDetails.m_relatedTable, out playerIndex);
			m_operationDetails.m_relatedTable.SetHiScores(scores, HiScoreTable.ScoreListType.Top10);
			HiScoreTableDataSteam associatedData = m_operationDetails.m_relatedTable.GetAssociatedData<HiScoreTableDataSteam>();
			associatedData.m_hasLeaderboardDataTop10 = true;
			success = true;
		}
		else if (pCallback.m_cEntryCount == 0)
		{
			List<HiScoreTable.HiScore> scores2 = new List<HiScoreTable.HiScore>();
			m_operationDetails.m_relatedTable.SetHiScores(scores2, HiScoreTable.ScoreListType.Top10);
			HiScoreTableDataSteam associatedData2 = m_operationDetails.m_relatedTable.GetAssociatedData<HiScoreTableDataSteam>();
			associatedData2.m_hasLeaderboardDataTop10 = true;
			success = true;
		}
		m_operationDetails = null;
		DoSuccess(success);
	}

	private void OnLeaderboardScoresDownloadedFriends(LeaderboardScoresDownloaded_t pCallback, bool bIOFailure)
	{
		bool success = false;
		if (pCallback.m_cEntryCount != 0 && !bIOFailure)
		{
			int playerIndex = -1;
			List<HiScoreTable.HiScore> hiScoresFriends = ScoreListToLocal(pCallback, m_operationDetails.m_relatedTable, out playerIndex);
			SetFriendsScoresUngrouped(hiScoresFriends, m_operationDetails.m_relatedTable, playerIndex);
			HiScoreTableDataSteam associatedData = m_operationDetails.m_relatedTable.GetAssociatedData<HiScoreTableDataSteam>();
			associatedData.m_hasLeaderboardDataFriends = true;
			success = true;
		}
		else if (pCallback.m_cEntryCount == 0)
		{
			int playerIndex2 = -1;
			List<HiScoreTable.HiScore> hiScoresFriends2 = new List<HiScoreTable.HiScore>();
			SetFriendsScoresUngrouped(hiScoresFriends2, m_operationDetails.m_relatedTable, playerIndex2);
			HiScoreTableDataSteam associatedData2 = m_operationDetails.m_relatedTable.GetAssociatedData<HiScoreTableDataSteam>();
			associatedData2.m_hasLeaderboardDataFriends = true;
			success = true;
		}
		m_operationDetails = null;
		DoSuccess(success);
	}

	private void OnLeaderboardScoresDownloadedForCurrentUser(LeaderboardScoresDownloaded_t pCallback, bool bIOFailure)
	{
		bool success = false;
		if (!bIOFailure)
		{
			success = true;
			if (pCallback.m_cEntryCount != 0)
			{
				SteamUserStats.GetDownloadedLeaderboardEntry(pCallback.m_hSteamLeaderboardEntries, 0, out var pLeaderboardEntry, null, 0);
				m_operationDetails.m_relatedTable.SetHiScoreFetched(pLeaderboardEntry.m_nScore, pLeaderboardEntry.m_nGlobalRank, SteamFriends.GetPersonaName());
			}
			else
			{
				m_operationDetails.m_relatedTable.SetHiScoreFetched(0, -1, SteamFriends.GetPersonaName());
			}
		}
		m_operationDetails = null;
		DoSuccess(success);
	}

	private void OnLeaderboardScoreUploaded(LeaderboardScoreUploaded_t pCallback, bool bIOFailure)
	{
		bool success = false;
		if (pCallback.m_bSuccess == 1)
		{
			m_operationDetails.m_relatedTable.SetHiScoreFetched(pCallback.m_nScore, pCallback.m_nGlobalRankNew, SteamFriends.GetPersonaName());
			success = true;
		}
		m_operationDetails = null;
		DoSuccess(success);
	}

	public override void AddHighScoreTable(HiScoreTable hst)
	{
		m_allTables.Add(hst);
		m_requiresRefresh = true;
	}

	public override void SetCheck()
	{
		m_requiresRefresh = true;
		foreach (HiScoreTable allTable in m_allTables)
		{
			HiScoreTableDataSteam associatedData = allTable.GetAssociatedData<HiScoreTableDataSteam>();
			associatedData.m_hasLeaderboardDataAroundPlayer = false;
			associatedData.m_hasLeaderboardDataFriends = false;
			associatedData.m_hasLeaderboardDataTop10 = false;
		}
	}

	public override string GetErrorText()
	{
		if (!IsOnline() || HasErrors())
		{
			return Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("leaderboard_failed_unknown");
		}
		return string.Empty;
	}

	private CSteamID[] GetFriends()
	{
		int friendCount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);
		List<CSteamID> list = new List<CSteamID>();
		for (int i = 0; i < friendCount; i++)
		{
			CSteamID friendByIndex = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate);
			list.Add(friendByIndex);
		}
		list.Add(SteamUser.GetSteamID());
		return list.ToArray();
	}

	private void Update()
	{
		UpdateErrorCtr();
		if (!CanUpdate() || !m_requiresRefresh || m_operationDetails != null)
		{
			return;
		}
		foreach (HiScoreTable allTable in m_allTables)
		{
			HiScoreTableDataSteam associatedData = allTable.GetAssociatedData<HiScoreTableDataSteam>();
			if (!associatedData.m_hasLeaderboardData)
			{
				m_operationDetails = new OperationDetails();
				m_operationDetails.m_operation = OperationTypes.FetchLeaderboardInfo;
				m_operationDetails.m_relatedTable = allTable;
				SteamAPICall_t hAPICall = SteamUserStats.FindLeaderboard(allTable.GetIdentifier().GetSteamIdentifier());
				OnLeaderboardFindResultCallResult.Set(hAPICall);
				break;
			}
			if (allTable.GetFetchedHiScore() == null)
			{
				m_operationDetails = new OperationDetails();
				m_operationDetails.m_operation = OperationTypes.FetchLeaderboardScores;
				m_operationDetails.m_relatedTable = allTable;
				CSteamID[] prgUsers = new CSteamID[1] { SteamUser.GetSteamID() };
				SteamAPICall_t hAPICall2 = SteamUserStats.DownloadLeaderboardEntriesForUsers(associatedData.m_leaderboard, prgUsers, 1);
				OnLeaderboardScoresDownloadedForCurrentUserCallResult.Set(hAPICall2);
				break;
			}
			if (allTable.GetLocalHiScore() != null && allTable.GetLocalHiScore().m_score > allTable.GetFetchedHiScore().m_score)
			{
				m_operationDetails = new OperationDetails();
				m_operationDetails.m_operation = OperationTypes.UploadScore;
				m_operationDetails.m_relatedTable = allTable;
				SteamAPICall_t hAPICall3 = SteamUserStats.UploadLeaderboardScore(associatedData.m_leaderboard, ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodKeepBest, allTable.GetLocalHiScore().m_score, null, 0);
				OnLeaderboardScoreUploadedCallResult.Set(hAPICall3);
				break;
			}
			if (!associatedData.m_hasLeaderboardDataAroundPlayer)
			{
				m_operationDetails = new OperationDetails();
				m_operationDetails.m_operation = OperationTypes.FetchLeaderboardScores;
				m_operationDetails.m_relatedTable = allTable;
				SteamAPICall_t hAPICall4 = SteamUserStats.DownloadLeaderboardEntries(associatedData.m_leaderboard, ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobalAroundUser, -5, 5);
				OnLeaderboardScoresDownloadedCallResult.Set(hAPICall4);
				break;
			}
			if (!associatedData.m_hasLeaderboardDataTop10)
			{
				m_operationDetails = new OperationDetails();
				m_operationDetails.m_operation = OperationTypes.FetchLeaderboardScores;
				m_operationDetails.m_relatedTable = allTable;
				SteamAPICall_t hAPICall5 = SteamUserStats.DownloadLeaderboardEntries(associatedData.m_leaderboard, ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobal, 0, 10);
				OnLeaderboardScoresDownloadedTop10CallResult.Set(hAPICall5);
				break;
			}
			if (!associatedData.m_hasLeaderboardDataFriends)
			{
				m_operationDetails = new OperationDetails();
				m_operationDetails.m_operation = OperationTypes.FetchLeaderboardScores;
				m_operationDetails.m_relatedTable = allTable;
				CSteamID[] friends = GetFriends();
				SteamAPICall_t hAPICall6 = SteamUserStats.DownloadLeaderboardEntriesForUsers(associatedData.m_leaderboard, friends, friends.Length);
				OnLeaderboardScoresDownloadedFriendsCallResult.Set(hAPICall6);
				break;
			}
		}
		if (m_operationDetails == null)
		{
			m_requiresRefresh = false;
		}
	}

	public override bool IsUpToDate(HiScoreTable hst, HiScoreTable.ScoreListType slt)
	{
		HiScoreTableDataSteam associatedData = hst.GetAssociatedData<HiScoreTableDataSteam>();
		bool result = false;
		if (associatedData.m_hasLeaderboardData)
		{
			switch (slt)
			{
			case HiScoreTable.ScoreListType.FriendsNear:
			case HiScoreTable.ScoreListType.FriendsTop:
				result = associatedData.m_hasLeaderboardDataFriends;
				break;
			case HiScoreTable.ScoreListType.NearMyScore:
				result = associatedData.m_hasLeaderboardDataAroundPlayer;
				break;
			case HiScoreTable.ScoreListType.Top10:
				result = associatedData.m_hasLeaderboardDataTop10;
				break;
			}
		}
		return result;
	}

	public override bool IsWorking()
	{
		return m_operationDetails != null;
	}

	public override string GetPlayerName()
	{
		return SteamFriends.GetPersonaName();
	}
}
