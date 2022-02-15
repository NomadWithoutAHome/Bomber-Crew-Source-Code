using System.Collections.Generic;

public class HiScoreTable
{
	public enum ScoreListType
	{
		Top10,
		NearMyScore,
		FriendsNear,
		FriendsTop
	}

	public class HiScoreList
	{
		public ScoreListType m_scoreListType;

		public HiScoreTable m_fromTable;

		public List<HiScore> m_scores;

		public int m_version;

		public void SetScores(List<HiScore> scores)
		{
			m_scores = scores;
			m_version++;
		}
	}

	public class HiScore
	{
		public string m_name;

		public int m_score;

		public bool m_isLocalPlayer;

		public int m_rank;
	}

	public class HiScoreUpload
	{
		public HiScoreIdentifier m_identifier;

		public int m_score;
	}

	private HiScoreIdentifier m_identifier;

	private List<HiScoreList> m_allHighscores = new List<HiScoreList>();

	private HiScore m_playerHiScoreFetched;

	private HiScore m_playerHiScoreLocal;

	private object m_associatedTableData;

	public HiScoreTable(HiScoreIdentifier identifier)
	{
		m_identifier = identifier;
	}

	public HiScoreIdentifier GetIdentifier()
	{
		return m_identifier;
	}

	public HiScoreList GetHiScoreList(ScoreListType slt)
	{
		HiScoreList hiScoreList = null;
		foreach (HiScoreList allHighscore in m_allHighscores)
		{
			if (allHighscore.m_scoreListType == slt)
			{
				hiScoreList = allHighscore;
				break;
			}
		}
		if (hiScoreList == null)
		{
			hiScoreList = new HiScoreList();
			hiScoreList.m_scoreListType = slt;
			hiScoreList.m_fromTable = this;
			m_allHighscores.Add(hiScoreList);
		}
		return hiScoreList;
	}

	public void SetHiScores(List<HiScore> scores, ScoreListType listType)
	{
		HiScoreList hiScoreList = GetHiScoreList(listType);
		hiScoreList.SetScores(scores);
	}

	public T GetAssociatedData<T>() where T : class, new()
	{
		if (m_associatedTableData == null)
		{
			m_associatedTableData = new T();
		}
		return (T)m_associatedTableData;
	}

	public HiScore GetLocalHiScore()
	{
		return m_playerHiScoreLocal;
	}

	public HiScore GetFetchedHiScore()
	{
		return m_playerHiScoreFetched;
	}

	public void SetNewLocalScore(int score)
	{
		if (m_playerHiScoreLocal == null)
		{
			m_playerHiScoreLocal = new HiScore();
		}
		if (score > m_playerHiScoreLocal.m_score)
		{
			m_playerHiScoreLocal.m_score = score;
		}
	}

	public void SetHiScoreFetched(int score, int rank, string playerName)
	{
		if (m_playerHiScoreFetched == null || score >= m_playerHiScoreFetched.m_score)
		{
			m_playerHiScoreFetched = new HiScore();
			m_playerHiScoreFetched.m_name = playerName;
			m_playerHiScoreFetched.m_isLocalPlayer = true;
			m_playerHiScoreFetched.m_score = score;
			m_playerHiScoreFetched.m_rank = rank;
		}
	}
}
