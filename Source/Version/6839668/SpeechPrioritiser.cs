using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class SpeechPrioritiser : Singleton<SpeechPrioritiser>
{
	public class SpeechRequestActive
	{
		private string m_stringToDisplay;

		private int m_priority;

		private float m_expiryTimer;

		private bool m_manualExpiry;

		private float m_visibilityTimer;

		private CrewmanAvatar m_crewmanAvatar;

		private Crewman m_crewman;

		private bool m_hasBeenLogged;

		private string m_languageString;

		private CrewmanAvatar m_useName;

		private string m_useText;

		public SpeechRequestActive(SpeechRequestTemplate sre, CrewmanAvatar ca, CrewmanAvatar useName, string useText)
		{
			m_languageString = sre.GetLanguageString();
			m_useName = useName;
			m_useText = useText;
			m_stringToDisplay = Singleton<LanguageProvider>.Instance.GetTextGroupOrTextImmediate(m_languageString);
			m_priority = sre.GetPriority();
			m_expiryTimer = sre.GetExpiry();
			if (m_expiryTimer < 0f)
			{
				m_manualExpiry = true;
			}
			m_crewman = ca.GetCrewman();
			m_crewmanAvatar = ca;
			m_visibilityTimer = 5f;
		}

		public void Update()
		{
			if (!m_manualExpiry)
			{
				m_expiryTimer -= Time.deltaTime;
			}
		}

		public bool IsExpired()
		{
			return m_expiryTimer < 0f && !m_manualExpiry;
		}

		public bool IsUserCapableOfSpeech()
		{
			return m_crewmanAvatar != null && !m_crewmanAvatar.GetHealthState().IsDead();
		}

		public Crewman GetCrewman()
		{
			return m_crewman;
		}

		public int GetImportance()
		{
			return m_priority;
		}

		public void UpdateVisible()
		{
			float num = ((Time.timeScale != 0f) ? (Time.deltaTime / Time.timeScale) : 0f);
			m_visibilityTimer -= num;
		}

		public bool IsVisibleExpired()
		{
			return m_visibilityTimer < 0f;
		}

		public string GetText()
		{
			return m_stringToDisplay;
		}

		public CrewmanAvatar GetUseName()
		{
			return m_useName;
		}

		public string GetUseText()
		{
			return m_useText;
		}

		public bool IsEquivalent(SpeechRequestTemplate sr, Crewman cm)
		{
			if (cm == m_crewman && sr.GetLanguageString() == m_languageString)
			{
				return true;
			}
			return false;
		}

		public void Expire()
		{
			m_manualExpiry = false;
			m_expiryTimer = -1f;
		}
	}

	public class SpeechRequestTemplate
	{
		private int m_priority;

		private float m_expiryTimer;

		private string m_languageString;

		public SpeechRequestTemplate(string languageString, int priority, float expiry)
		{
			m_languageString = languageString;
			m_priority = priority;
			m_expiryTimer = expiry;
		}

		public SpeechRequestTemplate(string languageString, int priority)
		{
			m_languageString = languageString;
			m_priority = priority;
			m_expiryTimer = -1f;
		}

		public string GetLanguageString()
		{
			return m_languageString;
		}

		public float GetExpiry()
		{
			return m_expiryTimer;
		}

		public int GetPriority()
		{
			return m_priority;
		}
	}

	[SerializeField]
	private int m_maxSpeakingAtOnce = 2;

	[SerializeField]
	private SpeechLog m_log;

	private List<SpeechRequestActive> m_currentSpeechRequests = new List<SpeechRequestActive>();

	private Dictionary<Crewman, SpeechRequestActive> m_currentlyActiveSpeech = new Dictionary<Crewman, SpeechRequestActive>();

	public SpeechRequestActive RequestNewSpeech(SpeechRequestTemplate sr, CrewmanAvatar cm, CrewmanAvatar insertName, string insertText)
	{
		if (cm != null)
		{
			bool flag = true;
			foreach (SpeechRequestActive currentSpeechRequest in m_currentSpeechRequests)
			{
				if (currentSpeechRequest.IsEquivalent(sr, cm.GetCrewman()))
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				SpeechRequestActive speechRequestActive = new SpeechRequestActive(sr, cm, insertName, insertText);
				m_currentSpeechRequests.Add(speechRequestActive);
				return speechRequestActive;
			}
		}
		return null;
	}

	public SpeechRequestActive RequestNewSpeech(SpeechRequestTemplate sr, CrewmanAvatar cm, CrewmanAvatar insertName)
	{
		return RequestNewSpeech(sr, cm, insertName, null);
	}

	public SpeechRequestActive RequestNewSpeech(SpeechRequestTemplate sr, CrewmanAvatar cm, string insertText)
	{
		return RequestNewSpeech(sr, cm, null, insertText);
	}

	public SpeechRequestActive RequestNewSpeech(SpeechRequestTemplate sr, CrewmanAvatar cm)
	{
		return RequestNewSpeech(sr, cm, null, null);
	}

	public SpeechRequestActive GetCurrentSpeechFor(Crewman ca)
	{
		SpeechRequestActive value = null;
		m_currentlyActiveSpeech.TryGetValue(ca, out value);
		return value;
	}

	private void Update()
	{
		List<SpeechRequestActive> list = new List<SpeechRequestActive>();
		foreach (SpeechRequestActive currentSpeechRequest in m_currentSpeechRequests)
		{
			currentSpeechRequest.Update();
			if (currentSpeechRequest.IsVisibleExpired() || currentSpeechRequest.IsExpired() || !currentSpeechRequest.IsUserCapableOfSpeech())
			{
				list.Add(currentSpeechRequest);
			}
		}
		foreach (SpeechRequestActive item in list)
		{
			m_currentSpeechRequests.Remove(item);
		}
		m_currentSpeechRequests.Sort((SpeechRequestActive x, SpeechRequestActive y) => x.GetImportance() - y.GetImportance());
		int num = m_currentlyActiveSpeech.Count;
		foreach (SpeechRequestActive currentSpeechRequest2 in m_currentSpeechRequests)
		{
			if (num == m_maxSpeakingAtOnce)
			{
				break;
			}
			if (!currentSpeechRequest2.IsVisibleExpired() && !m_currentlyActiveSpeech.ContainsKey(currentSpeechRequest2.GetCrewman()))
			{
				m_currentlyActiveSpeech[currentSpeechRequest2.GetCrewman()] = currentSpeechRequest2;
				num++;
			}
		}
		if (Singleton<ContextControl>.Instance.IsTargetingMode())
		{
			return;
		}
		List<Crewman> list2 = new List<Crewman>();
		foreach (KeyValuePair<Crewman, SpeechRequestActive> item2 in m_currentlyActiveSpeech)
		{
			item2.Value.UpdateVisible();
			if (!item2.Value.IsUserCapableOfSpeech() || item2.Value.IsVisibleExpired())
			{
				list2.Add(item2.Key);
			}
		}
		foreach (Crewman item3 in list2)
		{
			m_currentlyActiveSpeech.Remove(item3);
		}
	}
}
