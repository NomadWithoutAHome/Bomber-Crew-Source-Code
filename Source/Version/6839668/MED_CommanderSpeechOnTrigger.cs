using System;
using System.Collections.Generic;
using UnityEngine;

public class MED_CommanderSpeechOnTrigger : LevelItem
{
	[SerializeField]
	private string m_trigger;

	[SerializeField]
	private string m_cancelTrigger;

	[SerializeField]
	[NamedText]
	private string m_text;

	private string m_gizmoName = "Gizmo_Trigger";

	public override Dictionary<string, string> GetParameters(Func<LevelItem, string> resolverFunc)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["trigger"] = m_trigger;
		dictionary["cancelTrigger"] = m_cancelTrigger;
		dictionary["text"] = m_text;
		return dictionary;
	}

	public override string GetName()
	{
		return "CommanderSpeech";
	}

	public override bool HasSize()
	{
		return false;
	}
}
