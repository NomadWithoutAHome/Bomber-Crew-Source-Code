using System;
using BomberCrewCommon;
using UnityEngine;

public class LanguageSelectionTable : Singleton<LanguageSelectionTable>
{
	[Serializable]
	public class LanguageSelection
	{
		[SerializeField]
		public SystemLanguage m_systemLanguage;

		[SerializeField]
		public string m_localeString;

		[SerializeField]
		public string m_languagePostfix;

		[SerializeField]
		public bool m_force;

		[SerializeField]
		public bool m_specificOnly;
	}

	[SerializeField]
	private LanguageSelection[] m_languageSelections;

	public string GetLanguageForCurrentSystemLocale()
	{
		string localeString = null;
		return GetLanguageFor(Application.systemLanguage, localeString);
	}

	public string GetLanguageFor(SystemLanguage sl, string localeString)
	{
		LanguageSelection[] languageSelections = m_languageSelections;
		foreach (LanguageSelection languageSelection in languageSelections)
		{
			if (languageSelection.m_force)
			{
				return languageSelection.m_languagePostfix;
			}
		}
		if (!string.IsNullOrEmpty(localeString))
		{
			LanguageSelection[] languageSelections2 = m_languageSelections;
			foreach (LanguageSelection languageSelection2 in languageSelections2)
			{
				if (!string.IsNullOrEmpty(languageSelection2.m_localeString) && localeString.ToLower().Replace("_", string.Empty).Replace("-", string.Empty)
					.StartsWith(languageSelection2.m_localeString))
				{
					return languageSelection2.m_languagePostfix;
				}
			}
		}
		LanguageSelection[] languageSelections3 = m_languageSelections;
		foreach (LanguageSelection languageSelection3 in languageSelections3)
		{
			if (languageSelection3.m_systemLanguage == sl && !languageSelection3.m_specificOnly)
			{
				return languageSelection3.m_languagePostfix;
			}
		}
		return null;
	}
}
