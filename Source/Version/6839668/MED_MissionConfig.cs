using System;
using System.Collections.Generic;
using UnityEngine;

public class MED_MissionConfig : LevelItem
{
	[SerializeField]
	[Range(0f, 1f)]
	private float m_timeOfDayStart;

	[SerializeField]
	private bool m_winterEnvironment;

	public override string GetName()
	{
		return "MissionConfig";
	}

	public override bool HasSize()
	{
		return false;
	}

	public override Dictionary<string, string> GetParameters(Func<LevelItem, string> resolverFunc)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("timeOfDay", m_timeOfDayStart.ToString());
		dictionary.Add("winterEnvironment", (!m_winterEnvironment) ? "false" : "true");
		return dictionary;
	}
}
