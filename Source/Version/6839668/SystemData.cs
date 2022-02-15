using System.Collections.Generic;
using BomberCrewCommon;
using Newtonsoft.Json;
using UnityEngine;
using WingroveAudio;

[JsonObject(MemberSerialization.OptIn)]
public class SystemData
{
	[JsonObject(MemberSerialization.OptIn)]
	public class CrewmanLite
	{
		[JsonProperty]
		private string m_firstName;

		[JsonProperty]
		private string m_secondName;

		[JsonProperty]
		private Crewman.Skill m_primarySkill;

		public void SetFrom(Crewman cm)
		{
			m_primarySkill = cm.GetPrimarySkill();
			m_firstName = cm.GetFirstName();
			m_secondName = cm.GetSurname();
		}

		public string GetCrewmanRankTranslated()
		{
			return Singleton<CrewmanSkillUpgradeInfo>.Instance.GetRankTranslated(this);
		}

		public Crewman.Skill GetPrimarySkill()
		{
			return m_primarySkill;
		}

		public string GetFullName()
		{
			return m_firstName + " " + m_secondName;
		}
	}

	[JsonProperty]
	private bool m_invertY;

	[JsonProperty]
	private bool m_muteMusic;

	[JsonProperty]
	private List<CrewmanLite> m_lostCrew;

	[JsonProperty]
	private List<Crewman> m_memorialCrew;

	[JsonProperty]
	private List<Crewman> m_completionCrew;

	[JsonProperty]
	private float m_sensitivityTargeting;

	[JsonProperty]
	private float m_sensitivityCamera;

	[JsonProperty]
	private bool m_horizontalInvertInBomber;

	[JsonProperty]
	private string m_currentLanguage;

	[JsonProperty]
	private bool m_hasEverSetLanguage;

	[JsonProperty]
	private Dictionary<string, List<string>> m_lastPlayedDataSetting;

	[JsonProperty]
	private float m_musicVolume = 1f;

	[JsonProperty]
	private float m_sfxVolume = 1f;

	[JsonProperty]
	private bool m_hasEverSetMusicVolume;

	[JsonProperty]
	private Dictionary<string, int> m_nonSteamStats = new Dictionary<string, int>();

	[JsonProperty]
	private EndlessModeData m_endlessModeData;

	[JsonProperty]
	private Dictionary<string, List<EndlessModeVariant.LoadoutJS>> m_endlessModeLoadouts;

	[JsonProperty]
	private bool m_noVibration;

	[JsonProperty]
	private bool m_blockHints;

	[JsonProperty]
	private bool m_blockSlowDown;

	[JsonProperty]
	private bool m_useGyroAim;

	[JsonProperty]
	private bool m_useInvertYGyroAim;

	public float GetMusicVolume()
	{
		if (!m_hasEverSetMusicVolume)
		{
			m_musicVolume = 1f;
			m_sfxVolume = 1f;
			m_hasEverSetMusicVolume = true;
		}
		return m_musicVolume;
	}

	public bool AllowVibration()
	{
		return !m_noVibration;
	}

	public void SetAllowVibration(bool allow)
	{
		m_noVibration = !allow;
	}

	public bool GetInvertYMotionAiming()
	{
		return m_useInvertYGyroAim;
	}

	public void SetInvertYMotionAiming(bool inv)
	{
		m_useInvertYGyroAim = inv;
	}

	public void SetHintsAllowed(bool allowed)
	{
		m_blockHints = !allowed;
	}

	public bool BlockHints()
	{
		return m_blockHints;
	}

	public bool BlockSlowDown()
	{
		return m_blockSlowDown;
	}

	public void SetSlowdownAllowed(bool allowed)
	{
		m_blockSlowDown = !allowed;
	}

	public bool UseGyroAim()
	{
		return m_useGyroAim;
	}

	public void SetGyroAim(bool allowed)
	{
		m_useGyroAim = allowed;
	}

	public void SaveEndlessModeCustomLoadout(EndlessModeVariant.LoadoutJS data, string endlessMode, int index, int maxSlots)
	{
		if (m_endlessModeLoadouts == null)
		{
			m_endlessModeLoadouts = new Dictionary<string, List<EndlessModeVariant.LoadoutJS>>();
		}
		List<EndlessModeVariant.LoadoutJS> value = null;
		m_endlessModeLoadouts.TryGetValue(endlessMode, out value);
		if (value == null)
		{
			value = new List<EndlessModeVariant.LoadoutJS>();
			m_endlessModeLoadouts[endlessMode] = value;
		}
		while (value.Count < maxSlots)
		{
			value.Add(null);
		}
		value[index] = data;
		if (m_lastPlayedDataSetting == null)
		{
			m_lastPlayedDataSetting = new Dictionary<string, List<string>>();
		}
		List<string> value2 = null;
		m_lastPlayedDataSetting.TryGetValue(endlessMode, out value2);
		if (value2 == null)
		{
			value2 = new List<string>();
			m_lastPlayedDataSetting[endlessMode] = value2;
		}
		while (value2.Count < maxSlots)
		{
			value2.Add(string.Empty);
		}
		if (data != null)
		{
			string text = (value2[index] = data.GetUniqueString());
		}
		else
		{
			value2[index] = string.Empty;
		}
	}

	public EndlessModeVariant.LoadoutJS GetEndlessModeCustomLoadout(string endlessMode, int index, int maxSlots)
	{
		if (m_endlessModeLoadouts == null)
		{
			m_endlessModeLoadouts = new Dictionary<string, List<EndlessModeVariant.LoadoutJS>>();
		}
		List<EndlessModeVariant.LoadoutJS> value = null;
		m_endlessModeLoadouts.TryGetValue(endlessMode, out value);
		if (value == null)
		{
			value = new List<EndlessModeVariant.LoadoutJS>();
			m_endlessModeLoadouts[endlessMode] = value;
		}
		while (value.Count < maxSlots)
		{
			value.Add(null);
		}
		if (m_lastPlayedDataSetting == null)
		{
			m_lastPlayedDataSetting = new Dictionary<string, List<string>>();
		}
		List<string> value2 = null;
		m_lastPlayedDataSetting.TryGetValue(endlessMode, out value2);
		if (value2 == null)
		{
			value2 = new List<string>();
			m_lastPlayedDataSetting[endlessMode] = value2;
		}
		while (value2.Count < maxSlots)
		{
			value2.Add(string.Empty);
		}
		if (value[index] != null)
		{
			string uniqueString = value[index].GetUniqueString();
			if (value2[index] != uniqueString)
			{
				return null;
			}
		}
		return value[index];
	}

	public EndlessModeData GetEndlessModeData()
	{
		if (m_endlessModeData == null)
		{
			m_endlessModeData = new EndlessModeData();
		}
		return m_endlessModeData;
	}

	public float GetSFXVolume()
	{
		if (!m_hasEverSetMusicVolume)
		{
			m_musicVolume = 1f;
			m_sfxVolume = 1f;
			m_hasEverSetMusicVolume = true;
		}
		return m_sfxVolume;
	}

	public int GetStatFor(string key)
	{
		if (m_nonSteamStats == null)
		{
			m_nonSteamStats = new Dictionary<string, int>();
		}
		int value = 0;
		m_nonSteamStats.TryGetValue(key, out value);
		return value;
	}

	public void SetStatFor(string key, int val)
	{
		if (m_nonSteamStats == null)
		{
			m_nonSteamStats = new Dictionary<string, int>();
		}
		m_nonSteamStats[key] = val;
	}

	public void SetMusicVolume(float vol)
	{
		m_musicVolume = vol;
	}

	public void SetSFXVolume(float vol)
	{
		m_sfxVolume = vol;
	}

	public void SetInvert(bool invert)
	{
		m_invertY = invert;
	}

	public bool GetInvert()
	{
		return m_invertY;
	}

	public bool GetMusic()
	{
		return !m_muteMusic;
	}

	public float GetSensitivityTargeting()
	{
		return m_sensitivityTargeting * 0.5f + 0.5f;
	}

	public float GetSensitivityCamera()
	{
		return m_sensitivityCamera * 0.5f + 0.5f;
	}

	public bool HasEverSetLanguage()
	{
		return m_hasEverSetLanguage;
	}

	public void SetHasEverSetLanguage()
	{
		m_hasEverSetLanguage = true;
	}

	public string GetCurrentLanguage()
	{
		return m_currentLanguage;
	}

	public void SetToAppropriateLanguage(SystemDataContainer.SupportedLanguage[] languages)
	{
		bool flag = false;
		if (string.IsNullOrEmpty(m_currentLanguage))
		{
			flag = true;
		}
		else
		{
			bool flag2 = false;
			foreach (SystemDataContainer.SupportedLanguage supportedLanguage in languages)
			{
				if (supportedLanguage.m_languageToken == m_currentLanguage)
				{
					flag2 = true;
				}
			}
			if (!flag2)
			{
				flag = true;
			}
		}
		if (!flag)
		{
			return;
		}
		foreach (SystemDataContainer.SupportedLanguage supportedLanguage2 in languages)
		{
			SystemLanguage[] forLanaguages = supportedLanguage2.m_forLanaguages;
			foreach (SystemLanguage systemLanguage in forLanaguages)
			{
				if (systemLanguage == Application.systemLanguage)
				{
					m_currentLanguage = supportedLanguage2.m_languageToken;
				}
			}
		}
		if (!string.IsNullOrEmpty(m_currentLanguage))
		{
			return;
		}
		foreach (SystemDataContainer.SupportedLanguage supportedLanguage3 in languages)
		{
			if (supportedLanguage3.m_isDefault)
			{
				m_currentLanguage = supportedLanguage3.m_languageToken;
			}
		}
	}

	public void SetCurrentLanguage(string langTok)
	{
		m_currentLanguage = langTok;
	}

	public int GetInsideBomberHorizontalMultiplier()
	{
		if (m_horizontalInvertInBomber)
		{
			return 1;
		}
		return -1;
	}

	public void SetSensitivityTargeting(float amt)
	{
		m_sensitivityTargeting = (amt - 0.5f) * 2f;
	}

	public void SetHorizontalZoomedFlip(bool hzoomFlip)
	{
		m_horizontalInvertInBomber = hzoomFlip;
	}

	public bool GetHorizontalZoomedFlip()
	{
		return m_horizontalInvertInBomber;
	}

	public void SetSensitivityCamera(float amt)
	{
		m_sensitivityCamera = (amt - 0.5f) * 2f;
	}

	public void SetMusic(bool musicOn)
	{
		m_muteMusic = !musicOn;
		if (m_muteMusic)
		{
			WingroveRoot.Instance.PostEvent("MUSIC_MUTE");
		}
		else
		{
			WingroveRoot.Instance.PostEvent("MUSIC_UNMUTE");
		}
	}

	public List<CrewmanLite> GetCrewForWallNames()
	{
		if (m_lostCrew == null)
		{
			m_lostCrew = new List<CrewmanLite>();
		}
		return m_lostCrew;
	}

	public void RegisterCrewmanLost(Crewman cm)
	{
		if (m_lostCrew == null)
		{
			m_lostCrew = new List<CrewmanLite>();
		}
		CrewmanLite crewmanLite = new CrewmanLite();
		crewmanLite.SetFrom(cm);
		m_lostCrew.Add(crewmanLite);
		while (m_lostCrew.Count > 864)
		{
			m_lostCrew.RemoveAt(0);
		}
	}

	public void SetMemorialCrew(List<Crewman> memorial)
	{
		m_memorialCrew = new List<Crewman>();
		m_memorialCrew.AddRange(memorial);
	}

	public void SetCompletionCrew(List<Crewman> memorial)
	{
		m_completionCrew = new List<Crewman>();
		m_completionCrew.AddRange(memorial);
	}

	public List<Crewman> GetCrewForStatue()
	{
		if (m_completionCrew != null && m_completionCrew.Count == 7)
		{
			return m_completionCrew;
		}
		return m_memorialCrew;
	}

	public int GetNumberOfMissionsStatue()
	{
		if (m_memorialCrew != null && m_memorialCrew.Count > 0)
		{
			int num = int.MaxValue;
			{
				foreach (Crewman item in m_memorialCrew)
				{
					num = Mathf.Min(item.GetMissionsPlayed(), num);
				}
				return num;
			}
		}
		return 0;
	}

	public int GetNumberOfMissionsSuccessStatue()
	{
		if (m_memorialCrew != null && m_memorialCrew.Count > 0)
		{
			int num = int.MaxValue;
			{
				foreach (Crewman item in m_memorialCrew)
				{
					num = Mathf.Min(item.GetMissionsSuccessful(), num);
				}
				return num;
			}
		}
		return 0;
	}
}
