using System;
using System.Collections.Generic;
using UnityEngine;

public class MED_FighterWingItem : LevelItem
{
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
		SpitfireDrogue,
		FockeWulfTutorial,
		Do335,
		Ba349,
		Ho229,
		Cr42Falco,
		Ju87Stuka,
		Sm92,
		Sm79Sparviero
	}

	[SerializeField]
	private int m_numFightersInWing;

	[SerializeField]
	private MED_FighterSpawn[] m_fighterGroupTypeMix;

	[SerializeField]
	private bool m_forceSpawnInRange;

	[SerializeField]
	private bool m_huntPlayer;

	[SerializeField]
	private bool m_dontSpawnIfAcePresent;

	private string m_gizmoName = "Gizmo_Fighters";

	public override Dictionary<string, string> GetParameters(Func<LevelItem, string> resolverFunc)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["numFighters"] = m_numFightersInWing.ToString();
		dictionary["forceSpawnInRange"] = ((!m_forceSpawnInRange) ? "false" : "true");
		dictionary["hunt"] = ((!m_huntPlayer) ? "false" : "true");
		dictionary["notIfAce"] = ((!m_dontSpawnIfAcePresent) ? "false" : "true");
		string text = string.Empty;
		MED_FighterSpawn[] fighterGroupTypeMix = m_fighterGroupTypeMix;
		for (int i = 0; i < fighterGroupTypeMix.Length; i++)
		{
			MED_FighterSpawn mED_FighterSpawn = fighterGroupTypeMix[i];
			text = text + mED_FighterSpawn.ToString() + ",";
		}
		text = (dictionary["groupType"] = ((!(text == string.Empty)) ? text.Substring(0, text.Length - 1) : MED_FighterSpawn.Me109.ToString()));
		return dictionary;
	}

	private void OnDrawGizmos()
	{
		string gizmoName = GetGizmoName();
		if (!string.IsNullOrEmpty(gizmoName))
		{
			Gizmos.DrawIcon(base.transform.position, gizmoName, allowScaling: true);
		}
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(base.transform.position, 0.1f);
		Gizmos.DrawLine(base.transform.position, new Vector3(base.transform.position.x, 0f, base.transform.position.z));
	}

	private void OnDrawGizmosSelected()
	{
		string gizmoName = GetGizmoName();
		if (!string.IsNullOrEmpty(gizmoName))
		{
			Gizmos.DrawIcon(base.transform.position, gizmoName, allowScaling: true);
		}
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(base.transform.position, 0.1f);
		Gizmos.DrawLine(base.transform.position, new Vector3(base.transform.position.x, 0f, base.transform.position.z));
		Gizmos.DrawWireSphere(base.transform.position, 3f);
	}

	public override string GetGizmoName()
	{
		return m_gizmoName;
	}

	public override string GetName()
	{
		return "FighterWing";
	}

	public override bool HasSize()
	{
		return false;
	}
}
