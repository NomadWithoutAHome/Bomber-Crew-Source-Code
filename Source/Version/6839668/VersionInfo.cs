using System;
using System.Collections.Generic;
using BomberCrewCommon;
using Newtonsoft.Json;
using UnityEngine;

public class VersionInfo : Singleton<VersionInfo>
{
	[SerializeField]
	private int m_changeListVersion;

	[SerializeField]
	private string m_dateTimeString;

	[SerializeField]
	private string m_builtByUser;

	private void Awake()
	{
		TextAsset textAsset = Resources.Load<TextAsset>("UnityCloudBuildManifest.json");
		if (textAsset != null)
		{
			try
			{
				Dictionary<string, object> dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(textAsset.text);
				int versionCL = Convert.ToInt32(dictionary["scmCommitId"]);
				string dateTime = (string)dictionary["buildStartTime"];
				SetInfo(versionCL, dateTime, "CloudBuild");
			}
			catch
			{
			}
		}
	}

	public int GetChangeListVersion()
	{
		return m_changeListVersion;
	}

	public void SetInfo(int versionCL, string dateTime, string byUser)
	{
		m_changeListVersion = versionCL;
		m_dateTimeString = dateTime;
		m_builtByUser = byUser;
	}

	public string GetDateTime()
	{
		return m_dateTimeString;
	}

	public string GetByUser()
	{
		return m_builtByUser;
	}
}
