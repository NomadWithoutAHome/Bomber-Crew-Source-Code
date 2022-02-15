using System;
using System.Collections.Generic;
using UnityEngine;

public class MED_MissionStart : LevelItem
{
	[SerializeField]
	private bool m_b17;

	private string m_gizmoName = "Gizmo_MissionStart";

	public override Dictionary<string, string> GetParameters(Func<LevelItem, string> resolverFunc)
	{
		return new Dictionary<string, string>();
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.transform.position, m_gizmoName);
	}

	public override string GetName()
	{
		if (m_b17)
		{
			return "MissionStart_B17";
		}
		return "MissionStart";
	}

	public override string GetNameBase()
	{
		return "MissionStart";
	}

	public override bool HasSize()
	{
		return false;
	}
}
