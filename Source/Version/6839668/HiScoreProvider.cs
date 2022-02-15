using System.Collections.Generic;
using UnityEngine;

public abstract class HiScoreProvider : MonoBehaviour
{
	private float m_errorCountdown;

	private int m_errorsInARow;

	public abstract void SetUp();

	public abstract void AddHighScoreTable(HiScoreTable hst);

	public abstract void SetCheck();

	public abstract bool IsUpToDate(HiScoreTable hst, HiScoreTable.ScoreListType slt);

	public virtual void ResetAll()
	{
	}

	public virtual bool IsOnline()
	{
		return true;
	}

	public virtual string GetErrorText()
	{
		return string.Empty;
	}

	public abstract string GetPlayerName();

	public virtual bool ShouldShowConnectPrompt()
	{
		return true;
	}

	public abstract bool IsWorking();

	public bool HasErrors()
	{
		return m_errorsInARow != 0;
	}

	public virtual void RequestLogin(bool requestedByPlayer)
	{
	}

	public void ResetError()
	{
		m_errorCountdown = 0f;
		m_errorsInARow = 0;
	}

	public void DoSuccess(bool success)
	{
		if (success)
		{
			m_errorCountdown = 0f;
			m_errorsInARow = 0;
		}
		else
		{
			m_errorsInARow++;
			m_errorCountdown = (float)Mathf.Min(m_errorsInARow, 4) * 15f;
		}
	}

	public void UpdateErrorCtr()
	{
		m_errorCountdown -= Time.deltaTime;
		if (m_errorCountdown < 0f)
		{
			m_errorCountdown = 0f;
		}
	}

	public bool CanUpdate()
	{
		return m_errorCountdown <= 0f;
	}

	public void SetFriendsScoresUngrouped(List<HiScoreTable.HiScore> hiScoresFriends, HiScoreTable table, int playerIndex)
	{
		if (hiScoresFriends.Count > 11)
		{
			List<HiScoreTable.HiScore> list = new List<HiScoreTable.HiScore>();
			for (int i = 0; i < 10; i++)
			{
				list.Add(hiScoresFriends[i]);
			}
			table.SetHiScores(list, HiScoreTable.ScoreListType.FriendsTop);
			List<HiScoreTable.HiScore> list2 = new List<HiScoreTable.HiScore>();
			if (playerIndex == -1)
			{
				int count = hiScoresFriends.Count;
				int num = Mathf.Max(count - 11, 0);
				for (int j = num; j < count; j++)
				{
					list2.Add(hiScoresFriends[j]);
				}
			}
			else
			{
				int num2 = Mathf.Min(hiScoresFriends.Count, playerIndex + 5);
				int num3 = Mathf.Max(playerIndex - 5, 0);
				while (num2 - num3 < 11)
				{
					int num4 = num2 - num3;
					if (num4 == hiScoresFriends.Count)
					{
						break;
					}
					if (num3 > 0)
					{
						num3--;
						continue;
					}
					if (num2 < hiScoresFriends.Count)
					{
						num2++;
						continue;
					}
					break;
				}
				for (int k = num3; k < num2; k++)
				{
					list2.Add(hiScoresFriends[k]);
				}
			}
			table.SetHiScores(list2, HiScoreTable.ScoreListType.FriendsNear);
		}
		else
		{
			table.SetHiScores(hiScoresFriends, HiScoreTable.ScoreListType.FriendsTop);
			table.SetHiScores(hiScoresFriends, HiScoreTable.ScoreListType.FriendsNear);
		}
	}
}
