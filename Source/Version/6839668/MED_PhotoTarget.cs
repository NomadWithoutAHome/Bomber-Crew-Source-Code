using System;
using System.Collections.Generic;
using UnityEngine;

public class MED_PhotoTarget : LevelItem
{
	public enum ModelType
	{
		Placeholder,
		Submarine,
		Battleship,
		AirDefenseShipAA,
		TorpedoBoat
	}

	[SerializeField]
	private ModelType m_modelType;

	[SerializeField]
	private string m_triggerOnPhoto;

	[SerializeField]
	private string m_allowTagOnTrigger;

	[SerializeField]
	private bool m_optionalPhoto;

	[SerializeField]
	private string m_submarineLeaveTrigger;

	[SerializeField]
	private bool m_overrideBombTargetFlags;

	private string m_gizmoName = "Gizmo_PhotoTarget";

	public override Dictionary<string, string> GetParameters(Func<LevelItem, string> resolverFunc)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["triggerOnPhoto"] = m_triggerOnPhoto;
		dictionary["enableTagTrigger"] = m_allowTagOnTrigger;
		dictionary["optional"] = ((!m_optionalPhoto) ? "false" : "true");
		dictionary["leaveTrigger"] = m_submarineLeaveTrigger;
		if (m_overrideBombTargetFlags)
		{
			dictionary["optionalTarget"] = "true";
			dictionary["noObjective"] = "true";
			dictionary["noTagging"] = "true";
		}
		return dictionary;
	}

	public override string GetGizmoName()
	{
		return m_gizmoName;
	}

	public override string GetName()
	{
		return m_modelType switch
		{
			ModelType.Submarine => "PhotoTarget_Submarine", 
			ModelType.Battleship => "PhotoTarget_Battleship", 
			ModelType.AirDefenseShipAA => "PhotoTarget_AirDefenseShip_AA", 
			ModelType.TorpedoBoat => "PhotoTarget_TorpedoBoat", 
			_ => "PhotoTarget", 
		};
	}

	public override bool HasSize()
	{
		return false;
	}
}
