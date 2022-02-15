using System.Collections.Generic;
using UnityEngine;

public class AirbasePersistentCrewTarget : MonoBehaviour
{
	[SerializeField]
	private Transform[] m_endTransforms;

	[SerializeField]
	private Transform[] m_startTransforms;

	[SerializeField]
	private bool m_randomiseFromStart;

	private float m_maxDist;

	private void Start()
	{
		Transform[] endTransforms = m_endTransforms;
		foreach (Transform transform in endTransforms)
		{
			Transform[] startTransforms = m_startTransforms;
			foreach (Transform transform2 in startTransforms)
			{
				m_maxDist = Mathf.Max((transform.position - transform2.position).magnitude, m_maxDist);
			}
		}
		m_maxDist *= 0.5f;
	}

	public float GetMaxDist()
	{
		return m_maxDist;
	}

	public Transform[] GetEndTransforms()
	{
		return m_endTransforms;
	}

	public Transform[] GetStartTransformsDirect()
	{
		return m_startTransforms;
	}

	public List<Transform> GetStartTransforms(int num)
	{
		List<Transform> list = new List<Transform>();
		list.AddRange(m_startTransforms);
		list.Sort((Transform a, Transform b) => Random.Range(-1, 2));
		while (list.Count > num)
		{
			list.RemoveAt(Random.Range(0, list.Count));
		}
		return list;
	}
}
