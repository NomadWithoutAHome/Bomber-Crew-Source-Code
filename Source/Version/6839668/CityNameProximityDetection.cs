using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class CityNameProximityDetection : Singleton<CityNameProximityDetection>
{
	[SerializeField]
	private int m_numToUpdatePerFrame = 10;

	private List<CityNameLocation> m_allCities = new List<CityNameLocation>();

	private float m_nearestCityDistance;

	private CityNameLocation m_currentNearestCity;

	private int m_currentUpdateCtr;

	public void RegisterCity(CityNameLocation cnl)
	{
		m_allCities.Add(cnl);
	}

	private void Update()
	{
		int count = m_allCities.Count;
		for (int i = 0; i < m_numToUpdatePerFrame; i++)
		{
			int index = (i + m_currentUpdateCtr) % count;
			CityNameLocation cityNameLocation = m_allCities[index];
			Vector3 vector = cityNameLocation.transform.position - Singleton<BomberSpawn>.Instance.GetBomberSystems().transform.position;
			vector.y = 0f;
			if (cityNameLocation == m_currentNearestCity || m_currentNearestCity == null)
			{
				m_nearestCityDistance = vector.magnitude;
				m_currentNearestCity = cityNameLocation;
			}
			else if (vector.magnitude < m_nearestCityDistance)
			{
				m_nearestCityDistance = vector.magnitude;
				m_currentNearestCity = cityNameLocation;
			}
		}
		m_currentUpdateCtr += m_numToUpdatePerFrame;
		if (m_currentUpdateCtr > count)
		{
			m_currentUpdateCtr -= count;
		}
	}

	public CityNameLocation GetNearestCity()
	{
		return m_currentNearestCity;
	}

	public float GetNearestCityDistance()
	{
		return m_nearestCityDistance;
	}
}
