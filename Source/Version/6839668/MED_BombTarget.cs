using System;
using System.Collections.Generic;
using System.Globalization;
using BomberCrewCommon;
using UnityEngine;

public class MED_BombTarget : LevelItem
{
	public enum TargetType
	{
		Factory,
		Submarine,
		Training,
		LaunchSiteV1,
		LaunchSiteV2,
		EnemyAirbase,
		ArtilleryHazard,
		SubmarineBase,
		AmmoDump,
		Dam,
		SubmarineTorpedo,
		RadarSite,
		BunkerLarge,
		FlakTowerSmall,
		FlakTowerLarge,
		PowerStation,
		FactoryTanks,
		FactoryAircraft,
		FactoryV2Rockets,
		FuelDump,
		DamTraining,
		LaCoupoleConstruction,
		FactoryMotor,
		SubmarinePens,
		EnemyHQ,
		DamDestroyed,
		Ba349LauncherInvisible,
		FuelDumpHighExplosive,
		GrafZeppelin,
		TorpedoBoat,
		Battleship,
		TransportShip,
		AirDefenseShip_AA,
		RadarShip
	}

	public enum MED_FighterSpawn
	{
		Me109,
		Bf110,
		Me109Incendiary,
		Me109Tutorial,
		Me109AI,
		Ju88,
		Me163,
		FockeWulf,
		Me262,
		Do335,
		Ba349,
		Ho229,
		Cr42Falco,
		Ju87Stuka,
		Sm92,
		Sm79Sparviero
	}

	[SerializeField]
	private TargetType m_targetType;

	[SerializeField]
	private bool m_optionalTarget;

	[SerializeField]
	private bool m_noMissionObjective;

	[SerializeField]
	private bool m_noTagging;

	[SerializeField]
	private string m_missionObjectiveAddOnTrigger;

	[SerializeField]
	private string m_launcherTriggerName;

	[SerializeField]
	private string m_launcherTriggerOnLaunch;

	[SerializeField]
	private string m_destroyedTriggerName;

	[SerializeField]
	private string m_leaveTriggerSubmarine;

	[SerializeField]
	private float m_radiusForRadarSite;

	[SerializeField]
	private string m_radarTrigger;

	[SerializeField]
	private float m_launcherTimerOverride;

	[SerializeField]
	private string m_deactivateRadarOnTrigger;

	[SerializeField]
	private string m_radarStartTrigger;

	[SerializeField]
	private string m_flakTowerTriggerOnAlert;

	[SerializeField]
	private string m_flakTowerSwitchOffTrigger;

	[SerializeField]
	private bool m_flakTowerMidAltitude;

	[SerializeField]
	private bool m_flakTowerFullAltitude;

	[SerializeField]
	private int m_maxToLaunch;

	[SerializeField]
	private int m_maxAtOnce;

	[SerializeField]
	private MED_FighterSpawn[] m_fighterGroupTypeMix;

	[SerializeField]
	private bool m_failIfNoBombs;

	[SerializeField]
	private MED_WaypointItem m_runAwayTarget;

	[SerializeField]
	private MED_WaypointPathItem m_waypointPath;

	private string m_gizmoName = "Gizmo_Target";

	public override Dictionary<string, string> GetParameters(Func<LevelItem, string> resolverFunc)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["optionalTarget"] = ((!m_optionalTarget) ? "false" : "true");
		dictionary["noObjective"] = ((!m_noMissionObjective) ? "false" : "true");
		dictionary["noTagging"] = ((!m_noTagging) ? "false" : "true");
		dictionary["launcherTrigger"] = m_launcherTriggerName;
		dictionary["destroyedTrigger"] = m_destroyedTriggerName;
		dictionary["triggerOnLaunch"] = m_launcherTriggerOnLaunch;
		dictionary["objectiveOnTrigger"] = m_missionObjectiveAddOnTrigger;
		dictionary["leaveTrigger"] = m_leaveTriggerSubmarine;
		dictionary["failIfNoBombs"] = ((!m_failIfNoBombs) ? "false" : "true");
		dictionary["maxToLaunch"] = m_maxToLaunch.ToString();
		dictionary["maxAtOnce"] = m_maxAtOnce.ToString();
		dictionary["radius"] = Convert.ToString(m_radiusForRadarSite, CultureInfo.InvariantCulture);
		dictionary["trigger"] = m_radarTrigger;
		dictionary["launcherTimer"] = Convert.ToString(m_launcherTimerOverride, CultureInfo.InvariantCulture);
		dictionary["deactivate"] = m_deactivateRadarOnTrigger;
		dictionary["startTrigger"] = m_radarStartTrigger;
		dictionary["triggerOnAlert"] = m_flakTowerTriggerOnAlert;
		dictionary["switchOffTrigger"] = m_flakTowerSwitchOffTrigger;
		dictionary["waypoints"] = resolverFunc(m_waypointPath);
		if (m_runAwayTarget != null)
		{
			string text2 = (dictionary["runAwayTarget"] = resolverFunc(m_runAwayTarget));
		}
		if (m_flakTowerFullAltitude)
		{
			dictionary["height"] = 425.ToString();
			dictionary["altitude"] = 425.ToString();
		}
		else
		{
			dictionary["height"] = ((!m_flakTowerMidAltitude) ? 320 : 425).ToString();
			dictionary["altitude"] = (m_flakTowerMidAltitude ? 525 : 0).ToString();
		}
		string text3 = string.Empty;
		if (m_fighterGroupTypeMix != null)
		{
			MED_FighterSpawn[] fighterGroupTypeMix = m_fighterGroupTypeMix;
			for (int i = 0; i < fighterGroupTypeMix.Length; i++)
			{
				MED_FighterSpawn mED_FighterSpawn = fighterGroupTypeMix[i];
				text3 = text3 + mED_FighterSpawn.ToString() + ",";
			}
			text3 = ((!(text3 == string.Empty)) ? text3.Substring(0, text3.Length - 1) : MED_FighterSpawn.Me109.ToString());
		}
		dictionary["groupType"] = text3;
		return dictionary;
	}

	public override string GetGizmoName()
	{
		return m_gizmoName;
	}

	private void OnDrawGizmos()
	{
		if (m_targetType == TargetType.Submarine)
		{
			Gizmos.DrawSphere(base.transform.position, 0.4f);
		}
		if (m_targetType == TargetType.RadarSite || m_targetType == TargetType.RadarShip)
		{
			Gizmos.color = new Color(1f, 0f, 0f, Mathf.Lerp(0.25f, 0.75f, 0.5f));
			Gizmos.DrawSphere(base.transform.position, m_radiusForRadarSite / Singleton<SceneSetUp>.Instance.GetExportSettings().m_worldScaleUp);
		}
		Gizmos.DrawIcon(base.transform.position, m_gizmoName);
	}

	public override string GetName()
	{
		return "BombTarget_" + m_targetType;
	}

	public override string GetNameBase()
	{
		return "BombTarget";
	}

	public override bool HasSize()
	{
		return false;
	}
}
