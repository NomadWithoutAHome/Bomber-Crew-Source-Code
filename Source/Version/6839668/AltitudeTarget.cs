using System;
using UnityEngine;

[Serializable]
public class AltitudeTarget
{
	[SerializeField]
	private float m_heightWorldUnits;

	[SerializeField]
	private float m_oxygenPresence;

	[SerializeField]
	private float m_temperature;

	public static float m_worldUnitsToFeet = 3.3f;

	public float GetHeightFeet()
	{
		return m_heightWorldUnits * m_worldUnitsToFeet;
	}

	public float GetHeight()
	{
		return m_heightWorldUnits;
	}
}
