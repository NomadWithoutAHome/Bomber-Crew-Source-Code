using System;
using System.Collections.Generic;
using UnityEngine;

public class MED_WaypointPathItem : LevelItem
{
	public enum Style
	{
		Linear,
		Loop,
		PingPong
	}

	[SerializeField]
	private MED_WaypointItem[] m_waypoints;

	[SerializeField]
	private Style m_style;

	public override string GetName()
	{
		return "WaypointPath";
	}

	public override bool HasSize()
	{
		return false;
	}

	public override Dictionary<string, string> GetParameters(Func<LevelItem, string> resolverFunc)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		int num = 0;
		MED_WaypointItem[] waypoints = m_waypoints;
		foreach (MED_WaypointItem arg in waypoints)
		{
			string value = resolverFunc(arg);
			dictionary["Waypoint_" + num] = value;
			num++;
		}
		return dictionary;
	}

	private void OnDrawGizmos()
	{
		if (m_waypoints != null)
		{
			Vector3 zero = Vector3.zero;
			int num = ((m_style != Style.Loop) ? (m_waypoints.Length - 1) : m_waypoints.Length);
			for (int i = 0; i < num; i++)
			{
				Gizmos.color = Color.green;
				Gizmos.DrawLine(m_waypoints[i].transform.position, m_waypoints[(i + 1) % m_waypoints.Length].transform.position);
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		OnDrawGizmos();
	}
}
