using System;
using System.Collections.Generic;
using System.Globalization;
using BomberCrewCommon;
using UnityEngine;

public class MED_FlakTower : LevelItem
{
	public enum FlakTowerType
	{
		FlakTowerLarge,
		FlakTowerSmall
	}

	[SerializeField]
	private float m_density = 1f;

	[SerializeField]
	private float m_radius = 3000f;

	[SerializeField]
	private bool m_midAltitude;

	[SerializeField]
	private string m_triggerOnAlert;

	[SerializeField]
	private string m_switchOffTrigger;

	[SerializeField]
	private string m_missionObjectiveAddOnTrigger;

	private string m_gizmoName = "Gizmo_FlakTower";

	[SerializeField]
	private FlakTowerType m_flakTowerType;

	public override Dictionary<string, string> GetParameters(Func<LevelItem, string> resolverFunc)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["density"] = Convert.ToString(m_density, CultureInfo.InvariantCulture);
		dictionary["radius"] = Convert.ToString(m_radius, CultureInfo.InvariantCulture);
		dictionary["height"] = ((!m_midAltitude) ? 320 : 425).ToString();
		dictionary["altitude"] = (m_midAltitude ? 525 : 0).ToString();
		dictionary["triggerOnAlert"] = m_triggerOnAlert;
		dictionary["switchOffTrigger"] = m_switchOffTrigger;
		dictionary["objectiveOnTrigger"] = m_missionObjectiveAddOnTrigger;
		return dictionary;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0.25f, 0.75f, m_density));
		Gizmos.DrawSphere(base.transform.position, m_radius / Singleton<SceneSetUp>.Instance.GetExportSettings().m_worldScaleUp);
		Gizmos.color = Color.blue;
		Gizmos.DrawSphere(base.transform.position, 0.05f);
		Gizmos.DrawLine(base.transform.position, new Vector3(base.transform.position.x, 0f, base.transform.position.z));
		Gizmos.DrawIcon(base.transform.position, m_gizmoName);
	}

	public override string GetName()
	{
		return m_flakTowerType switch
		{
			FlakTowerType.FlakTowerLarge => "FlakTowerLarge", 
			FlakTowerType.FlakTowerSmall => "FlakTowerSmall", 
			_ => "FlakTowerLarge", 
		};
	}

	public override bool HasSize()
	{
		return false;
	}
}
