using System;
using System.Collections.Generic;
using BomberCrewCommon;
using Newtonsoft.Json;
using UnityEngine;

public class LanguageLoader : Singleton<LanguageLoader>, LoadableSystem
{
	[Serializable]
	public class LanguageAssets
	{
		[SerializeField]
		public TextAsset m_languageAsset;

		[SerializeField]
		public string m_languageReference;

		[SerializeField]
		public int m_fontReference;

		[SerializeField]
		public bool m_useNonWesternLineBreaking;
	}

	[SerializeField]
	private TextAsset m_languageAsset;

	[SerializeField]
	private LanguageAssets[] m_languageAssets;

	[SerializeField]
	private int m_fontReferenceDefault = 1;

	private bool m_loaded;

	private int m_languageFontReference;

	private bool m_useNonWesternLineBreaking;

	public event Action OnLanguageReload;

	private void Awake()
	{
		Singleton<SystemLoader>.Instance.RegisterLoadableSystem(this);
	}

	public string GetName()
	{
		return "LanguageLoader";
	}

	public bool IsLoadComplete()
	{
		return m_loaded;
	}

	public int GetLanguageFontReference()
	{
		return m_languageFontReference;
	}

	public bool UseNonWesternLineBreaking()
	{
		return m_useNonWesternLineBreaking;
	}

	public int GetLanguageFontReference(string forLangToken)
	{
		int result = m_fontReferenceDefault;
		string currentLanguage = Singleton<SystemDataContainer>.Instance.GetCurrentLanguage();
		LanguageAssets[] languageAssets = m_languageAssets;
		foreach (LanguageAssets languageAssets2 in languageAssets)
		{
			if (languageAssets2.m_languageReference == forLangToken)
			{
				result = languageAssets2.m_fontReference;
			}
		}
		return result;
	}

	public void StartLoad()
	{
		ReLoadLanguage();
	}

	public void ReLoadLanguage()
	{
		TextAsset languageAsset = m_languageAsset;
		m_languageFontReference = m_fontReferenceDefault;
		m_useNonWesternLineBreaking = false;
		string currentLanguage = Singleton<SystemDataContainer>.Instance.GetCurrentLanguage();
		LanguageAssets[] languageAssets = m_languageAssets;
		foreach (LanguageAssets languageAssets2 in languageAssets)
		{
			if (languageAssets2.m_languageReference == currentLanguage)
			{
				languageAsset = languageAssets2.m_languageAsset;
				m_languageFontReference = languageAssets2.m_fontReference;
				m_useNonWesternLineBreaking = languageAssets2.m_useNonWesternLineBreaking;
			}
		}
		Dictionary<string, List<Dictionary<string, string>>> upFromJSON = JsonConvert.DeserializeObject<Dictionary<string, List<Dictionary<string, string>>>>(languageAsset.text);
		Singleton<LanguageProvider>.Instance.SetUpFromJSON(upFromJSON);
		m_loaded = true;
		FontGroup.UpdateFontGroup();
		if (this.OnLanguageReload != null)
		{
			this.OnLanguageReload();
		}
	}

	public void LoadLongestLanguage()
	{
		Dictionary<string, List<Dictionary<string, string>>> upFromJSON = JsonConvert.DeserializeObject<Dictionary<string, List<Dictionary<string, string>>>>(m_languageAsset.text);
		Singleton<LanguageProvider>.Instance.SetUpFromJSON(upFromJSON);
		LanguageAssets[] languageAssets = m_languageAssets;
		foreach (LanguageAssets languageAssets2 in languageAssets)
		{
			upFromJSON = JsonConvert.DeserializeObject<Dictionary<string, List<Dictionary<string, string>>>>(languageAssets2.m_languageAsset.text);
			Singleton<LanguageProvider>.Instance.DoLongestFromJSON(upFromJSON);
		}
	}

	public void ContinueLoad()
	{
	}

	public LoadableSystem[] GetDependencies()
	{
		return new LoadableSystem[1] { Singleton<SystemDataContainer>.Instance };
	}
}
