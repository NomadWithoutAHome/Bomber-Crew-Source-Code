using System;
using System.Collections.Generic;
using UnityEngine;

public class MED_OutOfBoundsVolume : LevelItem
{
	[SerializeField]
	private string m_triggerToFire;

	private string m_gizmoName = "Gizmo_Trigger";

	public override Dictionary<string, string> GetParameters(Func<LevelItem, string> resolverFunc)
	{
		return new Dictionary<string, string>();
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(base.transform.position, base.transform.localScale);
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(base.transform.position, new Vector3(base.transform.position.x, 0f, base.transform.position.z));
		Gizmos.DrawIcon(base.transform.position, m_gizmoName);
	}

	public override string GetName()
	{
		return "OutOfBoundsVolume";
	}

	public override bool HasSize()
	{
		return true;
	}
}
