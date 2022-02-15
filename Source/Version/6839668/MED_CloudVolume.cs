using System;
using System.Collections.Generic;
using System.Globalization;
using BomberCrewCommon;
using UnityEngine;

public class MED_CloudVolume : LevelItem
{
	[SerializeField]
	[Range(0f, 1f)]
	private float m_density = 1f;

	[SerializeField]
	private float m_radius;

	[SerializeField]
	private bool m_rain;

	[SerializeField]
	private bool m_snow;

	public override Dictionary<string, string> GetParameters(Func<LevelItem, string> resolverFunc)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["density"] = Convert.ToString(m_density, CultureInfo.InvariantCulture);
		dictionary["radius"] = Convert.ToString(m_radius, CultureInfo.InvariantCulture);
		dictionary["rain"] = ((!m_rain) ? "false" : "true");
		dictionary["snow"] = ((!m_snow) ? "false" : "true");
		return dictionary;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = new Color(1f, 1f, 1f, Mathf.Lerp(0.25f, 0.75f, m_density));
		Gizmos.DrawSphere(base.transform.position, m_radius / Singleton<SceneSetUp>.Instance.GetExportSettings().m_worldScaleUp);
		Gizmos.color = Color.blue;
		Gizmos.DrawSphere(base.transform.position, 0.05f);
		Gizmos.DrawLine(base.transform.position, new Vector3(base.transform.position.x, 0f, base.transform.position.z));
	}

	public override string GetName()
	{
		return "CloudVolume";
	}

	public override bool HasSize()
	{
		return true;
	}
}
