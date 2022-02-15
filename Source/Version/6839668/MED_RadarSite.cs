using System;
using System.Collections.Generic;
using System.Globalization;
using BomberCrewCommon;
using UnityEngine;

public class MED_RadarSite : LevelItem
{
	[SerializeField]
	private string m_triggerOnDetect;

	[SerializeField]
	private float m_radius;

	[SerializeField]
	private string m_deactivateOnTrigger;

	[SerializeField]
	private string m_startTrigger;

	public override string GetName()
	{
		return "RadarSite";
	}

	public override Dictionary<string, string> GetParameters(Func<LevelItem, string> resolverFunc)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["radius"] = Convert.ToString(m_radius, CultureInfo.InvariantCulture);
		dictionary["trigger"] = m_triggerOnDetect;
		dictionary["deactivate"] = m_deactivateOnTrigger;
		dictionary["startTrigger"] = m_startTrigger;
		return dictionary;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = new Color(1f, 0f, 0f, Mathf.Lerp(0.25f, 0.75f, 0.5f));
		Gizmos.DrawSphere(base.transform.position, m_radius / Singleton<SceneSetUp>.Instance.GetExportSettings().m_worldScaleUp);
	}

	public override bool HasSize()
	{
		return false;
	}
}
