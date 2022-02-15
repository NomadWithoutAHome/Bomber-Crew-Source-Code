using System;
using System.Collections.Generic;
using UnityEngine;

public class MED_SearchLight : LevelItem
{
	[SerializeField]
	private string m_triggerOnAlert;

	[SerializeField]
	private string m_switchOffTrigger;

	[SerializeField]
	private string m_switchOnTrigger;

	private string m_gizmoName = "Gizmo_SearchLight";

	public override Dictionary<string, string> GetParameters(Func<LevelItem, string> resolverFunc)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["triggerOnAlert"] = m_triggerOnAlert;
		dictionary["switchOffTrigger"] = m_switchOffTrigger;
		dictionary["switchOnTrigger"] = m_switchOnTrigger;
		return dictionary;
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.transform.position, m_gizmoName);
	}

	public override string GetName()
	{
		return "SearchLight";
	}

	public override bool HasSize()
	{
		return false;
	}
}
