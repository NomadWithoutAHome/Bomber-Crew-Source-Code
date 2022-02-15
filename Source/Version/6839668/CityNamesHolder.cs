using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class CityNamesHolder : Singleton<CityNamesHolder>, LoadableSystem
{
	[SerializeField]
	private CityNamesDatabase m_cityDatabase;

	private Dictionary<string, Dictionary<string, string>> m_optimisedLookUpTable = new Dictionary<string, Dictionary<string, string>>();

	private bool m_isLoaded;

	private int m_citiesLoaded;

	public void ContinueLoad()
	{
		int num = 0;
		while (m_citiesLoaded < m_cityDatabase.m_allCityNames.Length)
		{
			CityNamesDatabase.CityName cityName = m_cityDatabase.m_allCityNames[m_citiesLoaded];
			string cityGeoNamesId = cityName.m_cityGeoNamesId;
			foreach (CityNamesDatabase.CityNameTranslated allTranslation in cityName.m_allTranslations)
			{
				m_optimisedLookUpTable[allTranslation.m_languageCode][cityGeoNamesId] = allTranslation.m_cityNameInLanguage;
			}
			m_citiesLoaded++;
			if (num > 20)
			{
				break;
			}
		}
		if (m_citiesLoaded == m_cityDatabase.m_allCityNames.Length)
		{
			m_isLoaded = true;
		}
	}

	public LoadableSystem[] GetDependencies()
	{
		return null;
	}

	public string GetName()
	{
		return "CityNameDatabase";
	}

	public bool IsLoadComplete()
	{
		return m_isLoaded;
	}

	public void StartLoad()
	{
		string[] targetLanguages = m_cityDatabase.m_targetLanguages;
		foreach (string key in targetLanguages)
		{
			m_optimisedLookUpTable[key] = new Dictionary<string, string>();
		}
	}

	public string GetLocalisedCityName(string geonamesId)
	{
		string text = Singleton<SystemDataContainer>.Instance.GetCurrentLanguage();
		if (string.IsNullOrEmpty(text))
		{
			text = "en";
		}
		string value = "[not found]";
		Dictionary<string, string> value2 = null;
		m_optimisedLookUpTable.TryGetValue(text, out value2);
		if (value2 == null)
		{
			text = "en";
			value2 = m_optimisedLookUpTable["en"];
		}
		if (!m_optimisedLookUpTable[text].TryGetValue(geonamesId, out value))
		{
			DebugLogWrapper.LogError("City not found in database: " + geonamesId);
			return "[not found]: " + geonamesId;
		}
		return value;
	}

	public string GetLocalisedCityNameSlowEditorEn(string geonamesId)
	{
		CityNamesDatabase.CityName[] allCityNames = m_cityDatabase.m_allCityNames;
		foreach (CityNamesDatabase.CityName cityName in allCityNames)
		{
			if (!(cityName.m_cityGeoNamesId == geonamesId))
			{
				continue;
			}
			foreach (CityNamesDatabase.CityNameTranslated allTranslation in cityName.m_allTranslations)
			{
				if (allTranslation.m_cityNameInLanguage == "en")
				{
					return allTranslation.m_cityNameInLanguage;
				}
			}
		}
		return "#NOT FOUND#";
	}

	private void Awake()
	{
		Singleton<SystemLoader>.Instance.RegisterLoadableSystem(this);
	}
}
