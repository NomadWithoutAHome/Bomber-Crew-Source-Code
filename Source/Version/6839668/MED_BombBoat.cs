using System;
using System.Collections.Generic;
using UnityEngine;

public class MED_BombBoat : LevelItem
{
	[SerializeField]
	private MED_WaypointPathItem m_waypointPath;

	[SerializeField]
	private MED_BombTarget m_target;

	public override string GetName()
	{
		return "BombBoat";
	}

	public override Dictionary<string, string> GetParameters(Func<LevelItem, string> resolverFunc)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["waypoints"] = resolverFunc(m_waypointPath);
		dictionary["mainTarget"] = resolverFunc(m_target);
		return dictionary;
	}

	public override bool HasSize()
	{
		return false;
	}
}
