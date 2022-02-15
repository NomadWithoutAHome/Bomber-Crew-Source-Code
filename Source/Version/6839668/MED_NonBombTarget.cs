using System;
using System.Collections.Generic;
using UnityEngine;

public class MED_NonBombTarget : LevelItem
{
	public enum TargetType
	{
		SearchAndRescue,
		FlyerDropArea,
		SearchSpitfire,
		SupplyDrop,
		SearchP51
	}

	[SerializeField]
	private TargetType m_targetType;

	[SerializeField]
	private bool m_optionalTarget;

	[SerializeField]
	private bool m_noMissionObjective;

	[SerializeField]
	private string m_launcherTriggerName;

	[SerializeField]
	private string m_destroyedTriggerName;

	private string m_gizmoName = "Gizmo_Target";

	public override Dictionary<string, string> GetParameters(Func<LevelItem, string> resolverFunc)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["optionalTarget"] = ((!m_optionalTarget) ? "false" : "true");
		dictionary["noObjective"] = ((!m_noMissionObjective) ? "false" : "true");
		return dictionary;
	}

	public override string GetGizmoName()
	{
		return m_gizmoName;
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.transform.position, m_gizmoName);
	}

	public override string GetName()
	{
		return m_targetType switch
		{
			TargetType.SearchAndRescue => "NonBombTarget_SearchAndRescue", 
			TargetType.FlyerDropArea => "NonBombTarget_FlyerDropArea", 
			TargetType.SearchSpitfire => "NonBombTarget_SearchSpitfire", 
			TargetType.SupplyDrop => "NonBombTarget_SupplyDrop", 
			TargetType.SearchP51 => "NonBombTarget_SearchP51", 
			_ => "NonBombTarget_SearchAndRescue", 
		};
	}

	public override string GetNameBase()
	{
		return "NonBombTarget";
	}

	public override bool HasSize()
	{
		return false;
	}
}
