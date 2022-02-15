using System;
using System.Collections.Generic;
using UnityEngine;

public class MED_FlakFocused : LevelItem
{
	private string m_gizmoName = "Gizmo_FlakFocused";

	[SerializeField]
	private string m_activateTrigger;

	public override Dictionary<string, string> GetParameters(Func<LevelItem, string> resolverFunc)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["activateTrigger"] = m_activateTrigger;
		return dictionary;
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.transform.position, m_gizmoName);
		Gizmos.color = new Color(0.4f, 0.4f, 0.4f, 0.4f);
		Gizmos.DrawSphere(base.transform.position, 2f);
	}

	public override string GetName()
	{
		return "FlakFocused";
	}

	public override bool HasSize()
	{
		return false;
	}
}
