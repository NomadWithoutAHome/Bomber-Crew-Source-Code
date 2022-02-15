using System;
using System.Collections.Generic;
using UnityEngine;

public class MED_StormCloud : LevelItem
{
	[SerializeField]
	private bool m_noLightning;

	[SerializeField]
	private bool m_heavySnow;

	public override Dictionary<string, string> GetParameters(Func<LevelItem, string> resolverFunc)
	{
		return new Dictionary<string, string>();
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawSphere(base.transform.position, 0.05f);
		Gizmos.DrawLine(base.transform.position, new Vector3(base.transform.position.x, 0f, base.transform.position.z));
		Color blue = Color.blue;
		blue.a = 0.25f;
		Gizmos.color = blue;
		Gizmos.DrawCube(base.transform.position + new Vector3(0f, 0.5f, 0f), new Vector3(3f, 1f, 3f));
		Gizmos.DrawLine(base.transform.position, new Vector3(base.transform.position.x, 0f, base.transform.position.z));
	}

	public override string GetName()
	{
		if (m_heavySnow)
		{
			return "StormCloudHS";
		}
		if (m_noLightning)
		{
			return "StormCloudNL";
		}
		return "StormCloud";
	}

	public override bool HasSize()
	{
		return false;
	}
}
