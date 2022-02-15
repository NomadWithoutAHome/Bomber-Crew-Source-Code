using System;
using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class HiScoreInterface : Singleton<HiScoreInterface>, LoadableSystem
{
	[SerializeField]
	private HiScoreProvider m_steamProvider;

	[SerializeField]
	private HiScoreProvider m_switchProvider;

	[SerializeField]
	private HiScoreProvider m_xboxProvider;

	[SerializeField]
	private HiScoreProvider m_ps4Provider;

	[SerializeField]
	private HiScoreProvider m_nullProvider;

	private List<HiScoreTable> m_allTables = new List<HiScoreTable>();

	private DateTime m_lastRefreshTime;

	private HiScoreProvider m_currentProvider;

	private bool m_loadComplete;

	private void Awake()
	{
		m_currentProvider = m_nullProvider;
		m_currentProvider = m_steamProvider;
		Singleton<SystemLoader>.Instance.RegisterLoadableSystem(this);
	}

	private void ResetAll()
	{
		m_allTables.Clear();
		m_currentProvider.ResetAll();
	}

	public HiScoreTable.HiScoreList GetNameList(HiScoreIdentifier identifier, HiScoreTable.ScoreListType listType)
	{
		HiScoreTable table = GetTable(identifier);
		return table.GetHiScoreList(listType);
	}

	public HiScoreTable GetTable(HiScoreIdentifier identifier)
	{
		HiScoreTable hiScoreTable = null;
		foreach (HiScoreTable allTable in m_allTables)
		{
			if (allTable.GetIdentifier().Matches(identifier))
			{
				hiScoreTable = allTable;
			}
		}
		if (hiScoreTable == null)
		{
			hiScoreTable = new HiScoreTable(identifier);
			m_allTables.Add(hiScoreTable);
			if (m_currentProvider != null)
			{
				m_currentProvider.AddHighScoreTable(hiScoreTable);
			}
		}
		return hiScoreTable;
	}

	public void SubmitScore(HiScoreIdentifier identifier, int score)
	{
		HiScoreTable.HiScoreUpload hiScoreUpload = new HiScoreTable.HiScoreUpload();
		hiScoreUpload.m_identifier = identifier;
		hiScoreUpload.m_score = score;
		GetTable(identifier).SetNewLocalScore(score);
		if (m_currentProvider != null)
		{
			m_currentProvider.SetCheck();
		}
	}

	public void CheckRefresh()
	{
		if (m_lastRefreshTime < DateTime.UtcNow - TimeSpan.FromMinutes(10.0))
		{
			m_lastRefreshTime = DateTime.UtcNow;
			if (m_currentProvider != null)
			{
				m_currentProvider.SetCheck();
			}
		}
	}

	public void ResetError()
	{
		if (m_currentProvider != null)
		{
			m_currentProvider.ResetError();
		}
	}

	public bool ShouldShowConnectPrompt()
	{
		if (m_currentProvider != null)
		{
			return m_currentProvider.ShouldShowConnectPrompt();
		}
		return false;
	}

	public string GetPlayerName()
	{
		if (m_currentProvider != null)
		{
			return m_currentProvider.GetPlayerName();
		}
		return string.Empty;
	}

	public void RequestLogin(bool requestedByPlayer)
	{
		if (m_currentProvider != null)
		{
			m_currentProvider.RequestLogin(requestedByPlayer);
		}
	}

	public bool GetIsUpToDate(HiScoreIdentifier identifier, HiScoreTable.ScoreListType slt)
	{
		bool result = false;
		if (m_currentProvider != null)
		{
			result = m_currentProvider.IsUpToDate(GetTable(identifier), slt);
		}
		return result;
	}

	public bool IsLoadComplete()
	{
		return m_loadComplete;
	}

	public bool IsError()
	{
		if (m_currentProvider != null)
		{
			return m_currentProvider.HasErrors();
		}
		return false;
	}

	public bool IsWorking()
	{
		if (m_currentProvider != null)
		{
			return m_currentProvider.IsWorking();
		}
		return false;
	}

	public bool IsOnline()
	{
		if (m_currentProvider != null)
		{
			return m_currentProvider.IsOnline();
		}
		return false;
	}

	public string GetErrorText()
	{
		if (m_currentProvider != null)
		{
			return m_currentProvider.GetErrorText();
		}
		return string.Empty;
	}

	public void StartLoad()
	{
		if (m_currentProvider != null)
		{
			m_currentProvider.SetUp();
		}
		m_loadComplete = true;
	}

	public void ContinueLoad()
	{
	}

	public string GetName()
	{
		return "[HISCORES]";
	}

	public LoadableSystem[] GetDependencies()
	{
		return null;
	}
}
