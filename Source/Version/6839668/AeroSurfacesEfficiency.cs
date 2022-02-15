using System.Collections.Generic;
using UnityEngine;

public class AeroSurfacesEfficiency : MonoBehaviour
{
	private List<AeroSurface> m_aeroSurfaces = new List<AeroSurface>(16);

	public void Register(AeroSurface aeroSurface)
	{
		m_aeroSurfaces.Add(aeroSurface);
	}

	public float GetInverseEfficiency()
	{
		float num = 1f;
		foreach (AeroSurface aeroSurface in m_aeroSurfaces)
		{
			num *= aeroSurface.GetInverseEfficiency();
		}
		return num;
	}
}
