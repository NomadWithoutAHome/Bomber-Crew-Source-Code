using System;
using System.Collections.Generic;
using UnityEngine;

public class MED_TriggerVolume : LevelItem
{
	[SerializeField]
	private string m_triggerToFire;

	private string m_gizmoName = "Gizmo_Trigger";

	public override Dictionary<string, string> GetParameters(Func<LevelItem, string> resolverFunc)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["trigger"] = m_triggerToFire;
		return dictionary;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireCube(base.transform.position, base.transform.localScale);
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(base.transform.position, new Vector3(base.transform.position.x, 0f, base.transform.position.z));
		Gizmos.DrawIcon(base.transform.position, m_gizmoName);
	}

	public override string GetName()
	{
		return "TriggerVolume";
	}

	public override bool HasSize()
	{
		return true;
	}
}
