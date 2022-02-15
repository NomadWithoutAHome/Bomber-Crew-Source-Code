using System.Collections.Generic;
using UnityEngine;

public class MissionPlaceableObject : MonoBehaviour
{
	[SerializeField]
	private bool m_isOnGround;

	[SerializeField]
	private bool m_isOnSea;

	private Vector3 m_size;

	private Dictionary<string, string> m_params = new Dictionary<string, string>();

	private Vector3d m_inPosition;

	public void SetPosition(Vector3d position, Vector3 size, List<LevelDescription.LevelParameter> parameters, Quaternion rotation)
	{
		m_inPosition = position;
		base.gameObject.btransform().position = position;
		m_size = size;
		if (m_isOnGround)
		{
			RaycastHit[] array = Physics.RaycastAll(new Vector3(base.transform.position.x, 1000f, base.transform.position.z), Vector3.down, 1001f, 1 << LayerMask.NameToLayer("Environment"));
			float num = float.MaxValue;
			RaycastHit[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				RaycastHit raycastHit = array2[i];
				if (!raycastHit.collider.isTrigger && raycastHit.collider.transform.parent != base.transform)
				{
					num = Mathf.Min(raycastHit.distance, num);
				}
			}
			position.y = 1000f - num;
			if (position.y < 0.0)
			{
				position.y = 0.0;
			}
		}
		else if (m_isOnSea)
		{
			position.y = 0.0;
		}
		base.gameObject.btransform().position = position;
		base.transform.rotation = rotation;
		if (parameters == null)
		{
			return;
		}
		foreach (LevelDescription.LevelParameter parameter in parameters)
		{
			m_params[parameter.m_key] = parameter.m_value;
		}
	}

	public Vector3d GetOriginalPosition()
	{
		return m_inPosition;
	}

	public string GetParameter(string param)
	{
		string value = null;
		m_params.TryGetValue(param, out value);
		return value;
	}

	public bool IsPointWithinArea(Vector3d point)
	{
		Vector3 vector = (Vector3)(point - base.gameObject.btransform().position);
		if (Mathf.Abs(vector.x) < m_size.x / 2f && Mathf.Abs(vector.y) < m_size.y / 2f && Mathf.Abs(vector.z) < m_size.z / 2f)
		{
			return true;
		}
		return false;
	}

	public bool IsPointWithinAreaExcludeHeight(Vector3d point)
	{
		Vector3 vector = (Vector3)(point - base.gameObject.btransform().position);
		if (Mathf.Abs(vector.x) < m_size.x / 2f && Mathf.Abs(vector.z) < m_size.z / 2f)
		{
			return true;
		}
		return false;
	}

	public Vector3 GetSize()
	{
		return m_size;
	}
}
