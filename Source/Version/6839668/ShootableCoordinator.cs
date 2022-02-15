using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class ShootableCoordinator : Singleton<ShootableCoordinator>
{
	private Dictionary<ShootableType, List<Shootable>> m_allShootables = new Dictionary<ShootableType, List<Shootable>>();

	public void RegisterShootable(Shootable st)
	{
		List<Shootable> value = null;
		m_allShootables.TryGetValue(st.GetShootableType(), out value);
		if (value == null)
		{
			value = new List<Shootable>();
			m_allShootables[st.GetShootableType()] = value;
		}
		value.Add(st);
	}

	public void RemoveShootable(Shootable st)
	{
		List<Shootable> value = null;
		m_allShootables.TryGetValue(st.GetShootableType(), out value);
		value?.Remove(st);
	}

	public List<Shootable> GetShootablesOfType(ShootableType sType)
	{
		List<Shootable> value = null;
		m_allShootables.TryGetValue(sType, out value);
		return value;
	}

	public Shootable GetNearestShootable(ShootableType sType, Vector3 toPos)
	{
		Shootable result = null;
		List<Shootable> value = null;
		m_allShootables.TryGetValue(sType, out value);
		if (value != null)
		{
			float num = float.MaxValue;
			{
				foreach (Shootable item in value)
				{
					Vector3 vector = item.GetCentreTransform().position - toPos;
					if (vector.magnitude < num)
					{
						num = vector.magnitude;
						result = item;
					}
				}
				return result;
			}
		}
		return result;
	}

	public Shootable GetNearestShootableNotDestroyed(ShootableType sType, Vector3 toPos)
	{
		Shootable result = null;
		List<Shootable> value = null;
		m_allShootables.TryGetValue(sType, out value);
		if (value != null)
		{
			float num = float.MaxValue;
			{
				foreach (Shootable item in value)
				{
					if (!item.IsDestroyed())
					{
						Vector3 vector = item.GetCentreTransform().position - toPos;
						if (vector.magnitude < num)
						{
							num = vector.magnitude;
							result = item;
						}
					}
				}
				return result;
			}
		}
		return result;
	}

	public Shootable GetShootableOfType(ShootableType sType)
	{
		List<Shootable> value = null;
		m_allShootables.TryGetValue(sType, out value);
		if (value != null && value.Count > 0)
		{
			return value[Random.Range(0, value.Count)];
		}
		return null;
	}
}
