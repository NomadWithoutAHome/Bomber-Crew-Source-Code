using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class FighterWing
{
	private List<FighterPlane> m_allFightersInWing = new List<FighterPlane>(5);

	private float m_cachedVisibilityCountdown;

	private bool m_hasEverCached;

	public void AssignFighter(FighterPlane fighter)
	{
		m_allFightersInWing.Add(fighter);
	}

	public void RemoveFighter(FighterPlane fighter)
	{
		m_allFightersInWing.Remove(fighter);
		if (m_allFightersInWing.Count == 0)
		{
			Singleton<FighterCoordinator>.Instance.DeRegisterWing(this);
		}
	}

	public FighterPlane GetLeadFighter()
	{
		using (List<FighterPlane>.Enumerator enumerator = m_allFightersInWing.GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				return enumerator.Current;
			}
		}
		return null;
	}

	public void UpdateCachedValues()
	{
		float num = 0f;
		foreach (FighterPlane item in m_allFightersInWing)
		{
			num = Mathf.Max(num, item.GetAI().GetVisibilityCountdown());
		}
		m_cachedVisibilityCountdown = num;
		m_hasEverCached = true;
	}

	public float GetVisibilityCountdown()
	{
		if (!m_hasEverCached)
		{
			UpdateCachedValues();
		}
		return m_cachedVisibilityCountdown;
	}

	public List<FighterPlane> GetAllFighters()
	{
		return m_allFightersInWing;
	}

	public int GetMyFighterIndex(FighterPlane me)
	{
		int num = 0;
		foreach (FighterPlane item in m_allFightersInWing)
		{
			if (item == me)
			{
				return num;
			}
			num++;
		}
		return -1;
	}
}
