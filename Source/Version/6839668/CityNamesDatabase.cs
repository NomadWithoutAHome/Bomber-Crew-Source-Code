using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Bomber Crew/City Name Database")]
public class CityNamesDatabase : ScriptableObject
{
	[Serializable]
	public class CityName
	{
		[SerializeField]
		public string m_cityNameBase;

		[SerializeField]
		public string m_cityGeoNamesId;

		[SerializeField]
		public List<CityNameTranslated> m_allTranslations = new List<CityNameTranslated>();
	}

	[Serializable]
	public class CityNameTranslated
	{
		[SerializeField]
		public string m_languageCode;

		[SerializeField]
		public string m_cityNameInLanguage;
	}

	[SerializeField]
	public string m_importCityNamesFromFile;

	[SerializeField]
	public string m_importAltNamesFromFile;

	[SerializeField]
	public CityName[] m_allCityNames;

	[SerializeField]
	public string[] m_targetLanguages;
}
