using System;
using System.Collections.Generic;
using UnityEngine;

public class MED_ExtraStartSettings : LevelItem
{
	public enum ExtraStartType
	{
		ExtraStart_FirstMission,
		ExtraStart_BombRunTraining,
		PlaceholderMission,
		Tutorial1NewTraining,
		Key02MissionObjectives,
		Key03MissionObjectives,
		Key04MissionObjectives,
		Key05MissionObjectives,
		GamesComDemo
	}

	[SerializeField]
	private ExtraStartType m_startType;

	public override Dictionary<string, string> GetParameters(Func<LevelItem, string> resolverFunc)
	{
		return new Dictionary<string, string>();
	}

	private void OnDrawGizmos()
	{
	}

	public override string GetName()
	{
		return m_startType switch
		{
			ExtraStartType.ExtraStart_BombRunTraining => "ExtraStart_BRT", 
			ExtraStartType.ExtraStart_FirstMission => "ExtraStart_FM", 
			ExtraStartType.PlaceholderMission => "PlaceholderMission", 
			ExtraStartType.Tutorial1NewTraining => "ExtraStart_Tutorial1", 
			ExtraStartType.Key02MissionObjectives => "Key02MissionObjectives", 
			ExtraStartType.Key03MissionObjectives => "Key03MissionObjectives", 
			ExtraStartType.Key04MissionObjectives => "Key04MissionObjectives", 
			ExtraStartType.Key05MissionObjectives => "Key05MissionObjectives", 
			ExtraStartType.GamesComDemo => "GamesComDemo", 
			_ => "ExtraStart_FM", 
		};
	}

	public override bool HasSize()
	{
		return false;
	}
}
