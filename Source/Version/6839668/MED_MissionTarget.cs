using System;
using System.Collections.Generic;
using UnityEngine;

public class MED_MissionTarget : LevelItem
{
	private string m_gizmoName = "Gizmo_MissionTarget";

	[SerializeField]
	private bool m_etaIsNa;

	[SerializeField]
	private bool m_primary;

	[SerializeField]
	private bool m_showOnCampaignRoute;

	[SerializeField]
	private int m_campaignRouteOrderHint;

	public override Dictionary<string, string> GetParameters(Func<LevelItem, string> resolverFunc)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["etaNa"] = ((!m_etaIsNa) ? "false" : "true");
		dictionary["MapDisplayPrimary"] = ((!m_primary) ? "false" : "true");
		dictionary["MapDisplay"] = ((!m_showOnCampaignRoute) ? "false" : "true");
		dictionary["MapDisplayIndex"] = m_campaignRouteOrderHint.ToString();
		return dictionary;
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.transform.position, m_gizmoName);
	}

	public override string GetName()
	{
		return "MissionTarget";
	}

	public override bool HasSize()
	{
		return false;
	}
}
