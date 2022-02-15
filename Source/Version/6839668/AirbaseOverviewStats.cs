using System;
using System.Collections;
using BomberCrewCommon;
using Common;
using UnityEngine;

public class AirbaseOverviewStats : MonoBehaviour
{
	[SerializeField]
	private GameObject m_statObject;

	[SerializeField]
	private LayoutGrid[] m_layoutGrids;

	[SerializeField]
	private int m_maxCharactersPerLine = 64;

	[SerializeField]
	private int m_maxCharactersPerLineNonWesternLanguages = 50;

	[SerializeField]
	private float m_showStatsDelay = 2.5f;

	[SerializeField]
	private float m_showStatsInterval = 0.2f;

	[SerializeField]
	[NamedText]
	private string m_missionsFlownText;

	[SerializeField]
	[NamedText]
	private string m_fightersDestroyedText;

	[SerializeField]
	[NamedText]
	private string m_milesTravelledText;

	[SerializeField]
	[NamedText]
	private string m_targetsDestroyedText;

	[SerializeField]
	[NamedText]
	private string m_bombersLostText;

	[SerializeField]
	[NamedText]
	private string m_crewLostText;

	[SerializeField]
	[NamedText]
	private string m_fundsSpentText;

	[SerializeField]
	[NamedText]
	private string m_reconPhotosTakenText;

	[SerializeField]
	[NamedText]
	private string m_crewMembersRescuedText;

	private string[] m_stats;

	private Func<int>[] m_getStats;

	private void OnEnable()
	{
		m_stats = new string[9] { m_missionsFlownText, m_fightersDestroyedText, m_milesTravelledText, m_targetsDestroyedText, m_bombersLostText, m_crewLostText, m_fundsSpentText, m_reconPhotosTakenText, m_crewMembersRescuedText };
		m_getStats = new Func<int>[9] { GetMissionsFlown, GetFightersDestroyed, GetMilesTravelled, GetTargetsDestroyed, GetBombersLost, GetCrewLost, GetFundsSpent, GetReconPhotosTaken, GetCrewRescued };
		StartCoroutine(SetUp());
	}

	private int GetFightersDestroyed()
	{
		return Singleton<SaveDataContainer>.Instance.Get().GetNumFightersDestroyed();
	}

	private int GetMissionsFlown()
	{
		return Singleton<SaveDataContainer>.Instance.Get().GetNumMissionsFlownActual();
	}

	private int GetMilesTravelled()
	{
		return Singleton<SaveDataContainer>.Instance.Get().GetNumMilesTravelled();
	}

	private int GetTargetsDestroyed()
	{
		return Singleton<SaveDataContainer>.Instance.Get().GetNumTargetsDestroyed();
	}

	private int GetBombersLost()
	{
		return Singleton<SaveDataContainer>.Instance.Get().GetNumBombersLost();
	}

	private int GetCrewLost()
	{
		return Singleton<SaveDataContainer>.Instance.Get().GetNumCrewLost();
	}

	private int GetReconPhotosTaken()
	{
		return Singleton<SaveDataContainer>.Instance.Get().GetNumReconPhotosTaken();
	}

	private int GetCrewRescued()
	{
		return Singleton<SaveDataContainer>.Instance.Get().GetNumCrewRescued();
	}

	private int GetFundsSpent()
	{
		return Singleton<SaveDataContainer>.Instance.Get().GetFundsSpent();
	}

	private void OnDisable()
	{
		LayoutGrid[] layoutGrids = m_layoutGrids;
		foreach (LayoutGrid layoutGrid in layoutGrids)
		{
			for (int num = layoutGrid.transform.childCount - 1; num >= 0; num--)
			{
				UnityEngine.Object.Destroy(layoutGrid.transform.GetChild(num).gameObject);
			}
		}
	}

	private IEnumerator SetUp()
	{
		if (!Singleton<SaveDataContainer>.Instance.Get().HasStats())
		{
			yield break;
		}
		int charCount = 0;
		float showStatsDelay = m_showStatsDelay;
		int currentLayoutGridIndex = 0;
		int nextLayoutGridIndex = 0;
		for (int i = 0; i < m_stats.Length; i++)
		{
			string statString = Singleton<GameFlow>.Instance.GetGameMode().ReplaceCurrencySymbol(string.Format(Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_stats[i]), m_getStats[i]()));
			charCount += statString.Length;
			int maxChars = m_maxCharactersPerLine;
			if (Singleton<LanguageLoader>.Instance.UseNonWesternLineBreaking())
			{
				maxChars = m_maxCharactersPerLineNonWesternLanguages;
			}
			if (charCount >= maxChars)
			{
				nextLayoutGridIndex++;
				if (nextLayoutGridIndex >= m_layoutGrids.Length)
				{
					break;
				}
				charCount = 0;
			}
			if (nextLayoutGridIndex == currentLayoutGridIndex && i < m_stats.Length - 1)
			{
				statString += " ·";
			}
			GameObject instantiatedStatObject = UnityEngine.Object.Instantiate(m_statObject);
			instantiatedStatObject.GetComponent<tk2dTextMesh>().SetText(statString);
			instantiatedStatObject.transform.parent = m_layoutGrids[currentLayoutGridIndex].transform;
			instantiatedStatObject.CustomActivate(active: true, showStatsDelay);
			showStatsDelay += m_showStatsInterval;
			currentLayoutGridIndex = nextLayoutGridIndex;
			yield return null;
		}
		LayoutGrid[] layoutGrids = m_layoutGrids;
		foreach (LayoutGrid layoutGrid in layoutGrids)
		{
			layoutGrid.RepositionChildren();
		}
		yield return null;
	}
}
