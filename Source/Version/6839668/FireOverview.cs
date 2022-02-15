using System.Collections.Generic;
using UnityEngine;

public class FireOverview : MonoBehaviour
{
	public interface Extinguishable
	{
		bool Exists();

		bool IsOnFire();

		bool SearchableForNext();

		void PutOutFire(float amt);

		float GetFireIntensityNormalised();

		Transform GetTransform();

		Transform GetPutOutPositionOverride();

		InteractiveItem NextInteractive();
	}

	[SerializeField]
	private FireArea[] m_allInteractiveFireAreas;

	[SerializeField]
	private Engine[] m_engines;

	private List<Extinguishable> m_allExtinguishables = new List<Extinguishable>();

	private void Start()
	{
		FireArea[] allInteractiveFireAreas = m_allInteractiveFireAreas;
		foreach (FireArea item in allInteractiveFireAreas)
		{
			m_allExtinguishables.Add(item);
		}
		Engine[] engines = m_engines;
		foreach (Engine item2 in engines)
		{
			m_allExtinguishables.Add(item2);
		}
	}

	public bool IsNearestBurningInDirection(Extinguishable a, Vector3 fromPosition)
	{
		Vector3 vector = a.GetTransform().position - fromPosition;
		foreach (Extinguishable allExtinguishable in m_allExtinguishables)
		{
			if (allExtinguishable != null && allExtinguishable.Exists() && allExtinguishable.IsOnFire())
			{
				Vector3 vector2 = allExtinguishable.GetTransform().position - fromPosition;
				if (vector2.magnitude < vector.magnitude && Vector3.Dot(vector.normalized, vector2.normalized) > 0.8f)
				{
					return false;
				}
			}
		}
		return true;
	}

	public bool GetAnyOnFire(bool engine)
	{
		foreach (Extinguishable allExtinguishable in m_allExtinguishables)
		{
			if (allExtinguishable != null && allExtinguishable.Exists() && allExtinguishable.IsOnFire())
			{
				if (engine && allExtinguishable is Engine)
				{
					return true;
				}
				if (!engine && !(allExtinguishable is Engine))
				{
					return true;
				}
			}
		}
		return false;
	}

	public Extinguishable GetBiggestFire(bool engine)
	{
		float num = 0f;
		Extinguishable result = null;
		foreach (Extinguishable allExtinguishable in m_allExtinguishables)
		{
			if (allExtinguishable == null || !allExtinguishable.Exists() || !allExtinguishable.IsOnFire())
			{
				continue;
			}
			if (engine && allExtinguishable is Engine)
			{
				if (allExtinguishable.GetFireIntensityNormalised() > num)
				{
					num = allExtinguishable.GetFireIntensityNormalised();
					result = allExtinguishable;
				}
			}
			else if (!engine && !(allExtinguishable is Engine) && allExtinguishable.GetFireIntensityNormalised() > num)
			{
				num = allExtinguishable.GetFireIntensityNormalised();
				result = allExtinguishable;
			}
		}
		return result;
	}

	public List<Extinguishable> GetFiresWithin(Vector3 pos, float dist)
	{
		List<Extinguishable> list = new List<Extinguishable>();
		foreach (Extinguishable allExtinguishable in m_allExtinguishables)
		{
			if (allExtinguishable != null && allExtinguishable.Exists() && (allExtinguishable.GetTransform().position - pos).magnitude < dist)
			{
				list.Add(allExtinguishable);
			}
		}
		return list;
	}

	public Extinguishable GetNearestFire(Vector3 fromPosition, bool forNextFire)
	{
		Extinguishable extinguishable = null;
		foreach (Extinguishable allExtinguishable in m_allExtinguishables)
		{
			if (allExtinguishable == null || !allExtinguishable.Exists() || !allExtinguishable.IsOnFire() || (forNextFire && !allExtinguishable.SearchableForNext()))
			{
				continue;
			}
			if (extinguishable == null)
			{
				extinguishable = allExtinguishable;
				continue;
			}
			Vector3 vector = allExtinguishable.GetTransform().position - fromPosition;
			Vector3 vector2 = extinguishable.GetTransform().position - fromPosition;
			if (vector.magnitude < vector2.magnitude)
			{
				extinguishable = allExtinguishable;
			}
		}
		return extinguishable;
	}

	public Extinguishable GetNearestSafeArea(Vector3 fromPosition, bool excludeExternal)
	{
		Extinguishable extinguishable = null;
		foreach (Extinguishable allExtinguishable in m_allExtinguishables)
		{
			if (allExtinguishable == null || !allExtinguishable.Exists() || allExtinguishable.IsOnFire() || (!allExtinguishable.SearchableForNext() && excludeExternal))
			{
				continue;
			}
			if (extinguishable == null)
			{
				extinguishable = allExtinguishable;
				continue;
			}
			Vector3 vector = allExtinguishable.GetTransform().position - fromPosition;
			Vector3 vector2 = extinguishable.GetTransform().position - fromPosition;
			if (vector.magnitude < vector2.magnitude)
			{
				extinguishable = allExtinguishable;
			}
		}
		return extinguishable;
	}

	public void StartFire(Vector3 atPosition)
	{
		float num = float.MaxValue;
		FireArea fireArea = null;
		FireArea[] allInteractiveFireAreas = m_allInteractiveFireAreas;
		foreach (FireArea fireArea2 in allInteractiveFireAreas)
		{
			Vector3 vector = fireArea2.transform.position - atPosition;
			if (vector.magnitude < num)
			{
				fireArea = fireArea2;
				num = vector.magnitude;
			}
		}
		if (fireArea != null)
		{
			fireArea.StartFire(1);
		}
	}
}
