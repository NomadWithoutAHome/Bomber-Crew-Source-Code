using System;
using System.Collections.Generic;
using UnityEngine;

public class MED_FriendlyBoat : LevelItem
{
	[SerializeField]
	private MED_WaypointPathItem m_waypointPath;

	[SerializeField]
	private bool m_transportShip;

	[SerializeField]
	private bool m_battleShip;

	[SerializeField]
	private bool m_landingShip;

	[SerializeField]
	private string m_triggerOnDestroy;

	public override string GetName()
	{
		if (m_battleShip)
		{
			return "AlliedBattleShip";
		}
		if (m_transportShip)
		{
			return "AlliedTransportShip";
		}
		if (m_landingShip)
		{
			return "AlliedLandingShipTank";
		}
		return "FriendlyBoat";
	}

	public override Dictionary<string, string> GetParameters(Func<LevelItem, string> resolverFunc)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["waypoints"] = resolverFunc(m_waypointPath);
		dictionary["triggerOnDestroy"] = m_triggerOnDestroy;
		return dictionary;
	}

	public override bool HasSize()
	{
		return false;
	}
}
