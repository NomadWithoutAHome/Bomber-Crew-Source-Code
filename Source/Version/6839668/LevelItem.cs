using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class LevelItem : MonoBehaviour
{
	[SerializeField]
	private string m_triggeredByEvent;

	[SerializeField]
	private string m_onlyIfMissionKey;

	[SerializeField]
	private string m_suppressIfMissionKey;

	public string GetSpawnTrigger()
	{
		return m_triggeredByEvent;
	}

	public string GetMissionKeyRequired()
	{
		return m_onlyIfMissionKey;
	}

	public string GetSuppressMissionKey()
	{
		return m_suppressIfMissionKey;
	}

	public abstract string GetName();

	public virtual string GetNameBase()
	{
		return GetName();
	}

	public abstract bool HasSize();

	public abstract Dictionary<string, string> GetParameters(Func<LevelItem, string> resolverFunc);

	public virtual string GetGizmoName()
	{
		return null;
	}

	private void OnDrawGizmos()
	{
		string gizmoName = GetGizmoName();
		if (!string.IsNullOrEmpty(gizmoName))
		{
			Gizmos.DrawIcon(base.transform.position, gizmoName, allowScaling: true);
		}
		if (HasSize())
		{
			Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.75f);
			Gizmos.DrawCube(base.transform.position, base.transform.localScale);
		}
		else
		{
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(base.transform.position, 0.1f);
		}
		Gizmos.color = Color.blue;
		Gizmos.DrawSphere(base.transform.position, 0.05f);
		Gizmos.DrawLine(base.transform.position, new Vector3(base.transform.position.x, 0f, base.transform.position.z));
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		if (HasSize())
		{
			Gizmos.DrawWireCube(base.transform.position, base.transform.localScale);
		}
		else
		{
			Gizmos.DrawWireSphere(base.transform.position, 0.1f);
		}
	}
}
