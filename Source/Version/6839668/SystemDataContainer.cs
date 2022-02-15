using System;
using BomberCrewCommon;
using Newtonsoft.Json;
using UnityEngine;

public class SystemDataContainer : Singleton<SystemDataContainer>, LoadableSystem
{
	[Serializable]
	public class SupportedLanguage
	{
		[SerializeField]
		public string m_languageToken;

		[SerializeField]
		public string m_localisedName;

		[SerializeField]
		public string m_confirmButtonText;

		[SerializeField]
		public SystemLanguage[] m_forLanaguages;

		[SerializeField]
		public int[] m_systemLanguagesPS4SCEE;

		[SerializeField]
		public int[] m_systemLanguagesPS4SCEA;

		[SerializeField]
		public bool m_isDefault;
	}

	[SerializeField]
	private SupportedLanguage[] m_allLanguages;

	[SerializeField]
	private SupportedLanguage[] m_allLanguagesSteamPC;

	[SerializeField]
	private SupportedLanguage[] m_allLanguagesJapaneseSwitch;

	[SerializeField]
	private SupportedLanguage[] m_allLanguagesChinesePC;

	private SystemData m_systemData;

	private bool m_loaded;

	private string m_detectedLanguage;

	public event Action OnSystemDataReloaded;

	public SupportedLanguage[] GetAllLanguages()
	{
		return m_allLanguagesSteamPC;
	}

	private void Awake()
	{
		Singleton<SystemLoader>.Instance.RegisterLoadableSystem(this);
	}

	public bool TryLoadBackup()
	{
		bool result = false;
		string filename = "BC_System.dat.backup";
		if (FileSystem.Exists(filename))
		{
			try
			{
				string text = FileSystem.ReadAllText(filename);
				if (text == null)
				{
					DebugLogWrapper.LogError("BACKUP: System data load returned null..");
					m_systemData = new SystemData();
					m_systemData.SetToAppropriateLanguage(GetAllLanguages());
					return result;
				}
				m_systemData = JsonConvert.DeserializeObject<SystemData>(text);
				m_systemData.SetToAppropriateLanguage(GetAllLanguages());
				result = true;
				return result;
			}
			catch (Exception)
			{
				DebugLogWrapper.LogError("BACKUP: System data read error...");
				m_systemData = new SystemData();
				m_systemData.SetToAppropriateLanguage(GetAllLanguages());
				return result;
			}
		}
		m_systemData = new SystemData();
		m_systemData.SetToAppropriateLanguage(GetAllLanguages());
		return true;
	}

	public bool Load()
	{
		bool flag = false;
		string text = "BC_System.dat";
		if (FileSystem.Exists(text) || FileSystem.Exists(text + ".backup"))
		{
			try
			{
				string text2 = FileSystem.ReadAllText(text);
				if (text2 == null)
				{
					return TryLoadBackup();
				}
				m_systemData = JsonConvert.DeserializeObject<SystemData>(text2);
				m_systemData.SetToAppropriateLanguage(GetAllLanguages());
				flag = true;
			}
			catch (Exception)
			{
				return TryLoadBackup();
			}
		}
		else
		{
			m_systemData = new SystemData();
			m_systemData.SetToAppropriateLanguage(GetAllLanguages());
			flag = true;
		}
		if (this.OnSystemDataReloaded != null)
		{
			this.OnSystemDataReloaded();
		}
		return flag;
	}

	public void Save()
	{
		string filename = "BC_System.dat";
		try
		{
			string contents = JsonConvert.SerializeObject(m_systemData, Formatting.Indented);
			FileSystem.WriteAllText(filename, contents);
		}
		catch (Exception)
		{
		}
	}

	public SystemData Get()
	{
		if (m_systemData == null)
		{
			Load();
		}
		return m_systemData;
	}

	public bool IsLoadComplete()
	{
		return m_loaded;
	}

	public void StartLoad()
	{
		Load();
		m_loaded = true;
	}

	public void ContinueLoad()
	{
	}

	public string GetName()
	{
		return "SystemData";
	}

	public LoadableSystem[] GetDependencies()
	{
		return null;
	}

	public string GetCurrentLanguage()
	{
		return Get().GetCurrentLanguage();
	}

	public void SetToAppropriateLanguage()
	{
		m_systemData.SetToAppropriateLanguage(GetAllLanguages());
	}
}
