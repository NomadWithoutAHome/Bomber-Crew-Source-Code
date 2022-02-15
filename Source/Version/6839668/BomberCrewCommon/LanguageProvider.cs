using System.Collections.Generic;
using UnityEngine;

namespace BomberCrewCommon;

public class LanguageProvider : Singleton<LanguageProvider>
{
	public delegate void LanguageCallback(string text);

	private class RegisteredCallback
	{
		public string m_key;

		public LanguageCallback m_callback;
	}

	[SerializeField]
	private bool m_languageTesting;

	private Dictionary<string, string> m_languageDb = new Dictionary<string, string>();

	private Dictionary<string, List<string>> m_groupsDb = new Dictionary<string, List<string>>();

	private bool m_isLoaded;

	private List<RegisteredCallback> m_callbackList = new List<RegisteredCallback>();

	public void SetUpFromJSON(Dictionary<string, List<Dictionary<string, string>>> languageRoot)
	{
		m_languageDb.Clear();
		m_groupsDb.Clear();
		foreach (KeyValuePair<string, List<Dictionary<string, string>>> item in languageRoot)
		{
			for (int i = 0; i < item.Value.Count; i++)
			{
				Dictionary<string, string> dictionary = item.Value[i];
				m_languageDb[dictionary["key"]] = dictionary["value"];
				if (dictionary.ContainsKey("group") && !string.IsNullOrEmpty(dictionary["group"]))
				{
					if (!m_groupsDb.ContainsKey(dictionary["group"]))
					{
						m_groupsDb[dictionary["group"]] = new List<string>();
					}
					m_groupsDb[dictionary["group"]].Add(dictionary["value"]);
				}
			}
		}
		foreach (RegisteredCallback callback in m_callbackList)
		{
			string value = string.Empty;
			m_languageDb.TryGetValue(callback.m_key, out value);
			if (value != null)
			{
				callback.m_callback(value);
			}
		}
		m_isLoaded = true;
	}

	public void DoLongestFromJSON(Dictionary<string, List<Dictionary<string, string>>> languageRoot)
	{
		foreach (KeyValuePair<string, List<Dictionary<string, string>>> item in languageRoot)
		{
			for (int i = 0; i < item.Value.Count; i++)
			{
				Dictionary<string, string> dictionary = item.Value[i];
				string text = dictionary["value"];
				if (text.Length <= m_languageDb[dictionary["key"]].Length)
				{
					continue;
				}
				m_languageDb[dictionary["key"]] = dictionary["value"];
				if (dictionary.ContainsKey("group") && !string.IsNullOrEmpty(dictionary["group"]))
				{
					if (!m_groupsDb.ContainsKey(dictionary["group"]))
					{
						m_groupsDb[dictionary["group"]] = new List<string>();
					}
					m_groupsDb[dictionary["group"]].Add(dictionary["value"]);
				}
			}
		}
		foreach (RegisteredCallback callback in m_callbackList)
		{
			string value = string.Empty;
			m_languageDb.TryGetValue(callback.m_key, out value);
			if (value != null)
			{
				callback.m_callback(value);
			}
		}
		m_isLoaded = true;
	}

	public bool IsLanguageTesting()
	{
		return false;
	}

	public void GetNamedText(string key, LanguageCallback callback)
	{
		if (m_isLoaded)
		{
			string value = string.Empty;
			m_languageDb.TryGetValue(key, out value);
			if (value == null)
			{
				callback(string.Empty);
			}
			else
			{
				callback(value);
			}
		}
		RegisteredCallback registeredCallback = new RegisteredCallback();
		registeredCallback.m_callback = callback;
		registeredCallback.m_key = key;
		m_callbackList.Add(registeredCallback);
	}

	public bool HasNamedText(string key)
	{
		return m_isLoaded && m_languageDb.ContainsKey(key);
	}

	public string GetNamedTextImmediate(string key)
	{
		if (m_isLoaded)
		{
			string value = string.Empty;
			m_languageDb.TryGetValue(key, out value);
			if (value == null)
			{
				return string.Empty;
			}
			return value;
		}
		return "*" + key + "*";
	}

	public string GetNamedTextGroupImmediate(string groupKey)
	{
		if (m_isLoaded)
		{
			List<string> value = null;
			m_groupsDb.TryGetValue(groupKey, out value);
			if (value == null)
			{
				return string.Empty;
			}
			return value[Random.Range(0, value.Count)];
		}
		return "*" + groupKey + "*";
	}

	public string GetTextGroupOrTextImmediate(string keyForEither)
	{
		if (m_isLoaded)
		{
			List<string> value = null;
			m_groupsDb.TryGetValue(keyForEither, out value);
			if (value == null)
			{
				return GetNamedTextImmediate(keyForEither);
			}
			return value[Random.Range(0, value.Count)];
		}
		return "*" + keyForEither + "*";
	}

	public bool NamedGroupKeyExists(string groupKey)
	{
		if (m_isLoaded)
		{
			return m_groupsDb.ContainsKey(groupKey);
		}
		return false;
	}

	public bool NamedEitherKeyExists(string keyForEither)
	{
		if (m_isLoaded)
		{
			return m_groupsDb.ContainsKey(keyForEither) || m_languageDb.ContainsKey(keyForEither);
		}
		return false;
	}

	public bool NamedTextKeyExists(string key)
	{
		if (m_isLoaded)
		{
			return m_languageDb.ContainsKey(key);
		}
		return false;
	}

	public bool IsLoaded()
	{
		return m_isLoaded;
	}
}
