using System;
using System.Globalization;
using BomberCrewCommon;
using UnityEngine;

public class EndlessModeArea : MonoBehaviour
{
	[SerializeField]
	private MissionPlaceableObject m_placeable;

	private bool m_land;

	private bool m_sea;

	private float m_squareRadius;

	private void Start()
	{
		m_land = m_placeable.GetParameter("land") == "true";
		m_sea = m_placeable.GetParameter("sea") == "true";
		m_squareRadius = (float)Convert.ToDouble(m_placeable.GetParameter("radius"), CultureInfo.InvariantCulture);
		Singleton<EndlessModeController>.Instance.RegisterArea(this);
	}

	public float GetSquareRadius()
	{
		return m_squareRadius;
	}

	public bool IsLand()
	{
		return m_land;
	}

	public bool IsSea()
	{
		return m_sea;
	}
}
