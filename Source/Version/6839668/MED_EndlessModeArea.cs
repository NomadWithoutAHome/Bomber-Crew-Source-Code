using System;
using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class MED_EndlessModeArea : LevelItem
{
	[SerializeField]
	private float m_radius;

	[SerializeField]
	private bool m_land;

	[SerializeField]
	private bool m_sea;

	public override string GetName()
	{
		return "EndlessModeArea";
	}

	public override bool HasSize()
	{
		return false;
	}

	private void OnDrawGizmos()
	{
		Vector3 position = base.transform.position;
		Gizmos.DrawWireCube(position, new Vector3(m_radius, 1f, m_radius) * 2f / Singleton<SceneSetUp>.Instance.GetExportSettings().m_worldScaleUp);
		Gizmos.color = Color.blue;
		Gizmos.DrawSphere(base.transform.position, 0.05f);
		Gizmos.DrawLine(base.transform.position, new Vector3(base.transform.position.x, 0f, base.transform.position.z));
	}

	public override Dictionary<string, string> GetParameters(Func<LevelItem, string> resolverFunc)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["radius"] = m_radius.ToString();
		dictionary["land"] = ((!m_land) ? "false" : "true");
		dictionary["sea"] = ((!m_sea) ? "false" : "true");
		return dictionary;
	}
}
