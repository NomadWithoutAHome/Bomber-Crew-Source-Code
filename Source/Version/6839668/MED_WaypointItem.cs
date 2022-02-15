using System;
using System.Collections.Generic;
using UnityEngine;

public class MED_WaypointItem : LevelItem
{
	public override string GetName()
	{
		return "Waypoint";
	}

	public override bool HasSize()
	{
		return false;
	}

	public override Dictionary<string, string> GetParameters(Func<LevelItem, string> resolverFunc)
	{
		return new Dictionary<string, string>();
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = new Color(0f, 0.5f, 0f);
		Gizmos.DrawSphere(base.transform.position, 0.05f);
		Gizmos.DrawLine(base.transform.position, new Vector3(base.transform.position.x, 0f, base.transform.position.z));
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		if (HasSize())
		{
			Gizmos.DrawWireCube(base.transform.position, base.transform.localScale);
		}
		else
		{
			Gizmos.DrawWireSphere(base.transform.position, 0.1f);
		}
	}
}
