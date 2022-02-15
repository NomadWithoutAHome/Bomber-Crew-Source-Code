using System;
using System.Collections.Generic;
using UnityEngine;

public class MED_AceFighter : LevelItem
{
	[SerializeField]
	[Range(0f, 1f)]
	private float m_chanceOfAppear;

	[SerializeField]
	private int m_forceId;

	[SerializeField]
	private string m_spawnTrigger;

	[SerializeField]
	private string m_fromDLC;

	private string m_gizmoName = "Gizmo_Fighters";

	public override Dictionary<string, string> GetParameters(Func<LevelItem, string> resolverFunc)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["chanceOfAppear"] = m_chanceOfAppear.ToString();
		dictionary["forceId"] = m_forceId.ToString();
		dictionary["spawnTrigger"] = m_spawnTrigger;
		dictionary["fromDLC"] = m_fromDLC;
		return dictionary;
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.transform.position, m_gizmoName);
	}

	public override string GetName()
	{
		return "AceFighterSpawn";
	}

	public override bool HasSize()
	{
		return false;
	}
}
