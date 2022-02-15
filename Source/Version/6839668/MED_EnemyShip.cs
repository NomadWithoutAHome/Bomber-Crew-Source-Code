using System;
using System.Collections.Generic;
using UnityEngine;

public class MED_EnemyShip : LevelItem
{
	[SerializeField]
	private MED_WaypointPathItem m_waypointPath;

	public override string GetName()
	{
		return "BombTarget_TransportShip";
	}

	public override Dictionary<string, string> GetParameters(Func<LevelItem, string> resolverFunc)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["waypoints"] = resolverFunc(m_waypointPath);
		return dictionary;
	}

	public override bool HasSize()
	{
		return false;
	}
}
