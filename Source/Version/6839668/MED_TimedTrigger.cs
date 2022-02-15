using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class MED_TimedTrigger : LevelItem
{
	[SerializeField]
	private string m_triggerToFire;

	[SerializeField]
	private int m_secondsCountdown;

	[SerializeField]
	private bool m_hasObjective;

	[SerializeField]
	private bool m_fullObjective;

	[SerializeField]
	[NamedText]
	private string m_namedTextObjective;

	[SerializeField]
	private string m_completeObjectiveTrigger;

	[SerializeField]
	private string m_completeObjectiveTriggerForward;

	[SerializeField]
	private bool m_failIfNoBombs;

	public override string GetName()
	{
		return "TimedTrigger";
	}

	public override Dictionary<string, string> GetParameters(Func<LevelItem, string> resolverFunc)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["hasObjective"] = ((!m_hasObjective) ? "false" : "true");
		dictionary["fullObjective"] = ((!m_fullObjective) ? "false" : "true");
		dictionary["triggerToFire"] = m_triggerToFire;
		dictionary["completeTrigger"] = m_completeObjectiveTrigger;
		dictionary["completeForward"] = m_completeObjectiveTriggerForward;
		dictionary["objectiveText"] = m_namedTextObjective;
		dictionary["countdown"] = Convert.ToString(m_secondsCountdown, CultureInfo.InvariantCulture);
		dictionary["failIfNoBombs"] = ((!m_failIfNoBombs) ? "false" : "true");
		return dictionary;
	}

	public override bool HasSize()
	{
		return false;
	}
}
