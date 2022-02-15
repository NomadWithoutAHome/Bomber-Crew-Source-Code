using System;
using System.Collections.Generic;
using UnityEngine;

public class MED_ExtraObjective : LevelItem
{
	private enum ObjectiveType
	{
		TakePhoto,
		ClearArea,
		V1sDontHit,
		WaitForTrigger
	}

	[SerializeField]
	private ObjectiveType m_objectiveType;

	[SerializeField]
	private int m_associatedValue;

	[SerializeField]
	[NamedText]
	private string m_namedText;

	[SerializeField]
	[NamedText]
	private string m_namedTextDebrief;

	[SerializeField]
	private bool m_fullObjective;

	[SerializeField]
	private string m_associatedValueString;

	[SerializeField]
	private string m_failObjectiveTrigger;

	[SerializeField]
	private string m_failObjectiveTrigger2;

	[SerializeField]
	private string m_triggerOnComplete;

	[SerializeField]
	private bool m_lightClearObjective;

	public override string GetName()
	{
		return "ExtraObjective";
	}

	public override bool HasSize()
	{
		return false;
	}

	public override Dictionary<string, string> GetParameters(Func<LevelItem, string> resolverFunc)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["objectiveType"] = m_objectiveType.ToString();
		dictionary["value"] = m_associatedValue.ToString();
		dictionary["namedText"] = m_namedText;
		dictionary["namedTextDebrief"] = m_namedTextDebrief;
		dictionary["fullObjective"] = ((!m_fullObjective) ? "false" : "true");
		dictionary["valueString"] = m_associatedValueString;
		dictionary["failObjective"] = m_failObjectiveTrigger;
		dictionary["failObjective2"] = m_failObjectiveTrigger2;
		dictionary["triggerOnComplete"] = m_triggerOnComplete;
		dictionary["lightClearObjective"] = ((!m_lightClearObjective) ? "false" : "true");
		return dictionary;
	}
}
