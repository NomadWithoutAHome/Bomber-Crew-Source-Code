using System;
using UnityEngine;

public class HiScoreProviderSwitch : HiScoreProvider
{
	[Serializable]
	public struct NP_NGSINFO
	{
		public string gameServerId;

		public string accessKey;

		public int timeOut;
	}

	[Serializable]
	public struct NEX_INIT_PARAM
	{
		public NP_NGSINFO ngsInfo;

		public uint pluginMemSize;

		public uint nexMemSize;

		public uint reserveMemSize;

		public uint dispatchTimeOut;
	}

	[SerializeField]
	private NEX_INIT_PARAM m_nexInitParam;

	private bool m_hasEnsuredContext;

	private bool m_isEnsuringContext;

	private string m_cachedPlayerName;

	public override bool IsUpToDate(HiScoreTable hst, HiScoreTable.ScoreListType slt)
	{
		return false;
	}

	public override void AddHighScoreTable(HiScoreTable hst)
	{
	}

	public override void SetCheck()
	{
	}

	public override void SetUp()
	{
	}

	public override bool IsWorking()
	{
		return false;
	}

	public override string GetPlayerName()
	{
		return string.Empty;
	}
}
