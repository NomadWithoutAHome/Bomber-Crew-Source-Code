using System;
using System.Collections.Generic;
using UnityEngine;

public class MED_TriggerAccumulator : LevelItem
{
	[SerializeField]
	private string[] m_triggersToReceive;

	[SerializeField]
	private int m_maxRequired;

	[SerializeField]
	private string m_triggerToFire;

	[SerializeField]
	[NamedText]
	private string m_objectiveName;

	[SerializeField]
	private bool m_hasVisibleObjective;

	[SerializeField]
	private bool m_hasRequiredObjective;

	[SerializeField]
	private string m_failOnTrigger;

	[SerializeField]
	private bool m_failIfNoBombs;

	public override string GetName()
	{
		return "TriggerAccumulator";
	}

	public override Dictionary<string, string> GetParameters(Func<LevelItem, string> resolverFunc)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["numTriggersTotal"] = m_triggersToReceive.Length.ToString();
		for (int i = 0; i < m_triggersToReceive.Length; i++)
		{
			dictionary["trigger" + i] = m_triggersToReceive[i];
		}
		dictionary["triggerToFire"] = m_triggerToFire;
		dictionary["maxRequired"] = m_maxRequired.ToString();
		dictionary["objectiveName"] = m_objectiveName;
		dictionary["hasVisibleObjective"] = ((!m_hasVisibleObjective) ? "false" : "true");
		dictionary["hasRequiredObjective"] = ((!m_hasRequiredObjective) ? "false" : "true");
		dictionary["failOnTrigger"] = m_failOnTrigger;
		dictionary["failIfNoBombs"] = ((!m_failIfNoBombs) ? "false" : "true");
		return dictionary;
	}

	public override bool HasSize()
	{
		return false;
	}
}
