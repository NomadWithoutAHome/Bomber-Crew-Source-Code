using System;
using System.Collections.Generic;
using System.Globalization;
using BomberCrewCommon;
using UnityEngine;

public class MED_FlakVolume : LevelItem
{
	[SerializeField]
	private float m_density = 1f;

	[SerializeField]
	private float m_radius = 3000f;

	[SerializeField]
	private bool m_midAltitude;

	[SerializeField]
	private bool m_onSea;

	[SerializeField]
	private string m_activateTrigger;

	private string m_gizmoName = "Gizmo_Flak";

	public override Dictionary<string, string> GetParameters(Func<LevelItem, string> resolverFunc)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["density"] = Convert.ToString(m_density, CultureInfo.InvariantCulture);
		dictionary["radius"] = Convert.ToString(m_radius, CultureInfo.InvariantCulture);
		dictionary["height"] = ((!m_midAltitude) ? 320 : 425).ToString();
		dictionary["altitude"] = (m_midAltitude ? 525 : 0).ToString();
		dictionary["activateTrigger"] = m_activateTrigger;
		return dictionary;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0.25f, 0.75f, m_density));
		float num = ((!m_midAltitude) ? 300 : 400);
		float num2 = (m_midAltitude ? 525 : 0);
		Vector3 position = base.transform.position;
		position.y = 0f;
		Gizmos.DrawCube(position + new Vector3(0f, num + num2, 0f) / Singleton<SceneSetUp>.Instance.GetExportSettings().m_worldScaleUp, new Vector3(m_radius, num, m_radius) * 2f / Singleton<SceneSetUp>.Instance.GetExportSettings().m_worldScaleUp);
		Gizmos.color = Color.blue;
		Gizmos.DrawSphere(base.transform.position, 0.05f);
		Gizmos.DrawLine(base.transform.position, new Vector3(base.transform.position.x, 0f, base.transform.position.z));
		Gizmos.DrawIcon(base.transform.position, m_gizmoName);
	}

	public override string GetName()
	{
		if (m_onSea)
		{
			return "FlakVolumeSea";
		}
		return "FlakVolume";
	}

	public override bool HasSize()
	{
		return false;
	}
}
