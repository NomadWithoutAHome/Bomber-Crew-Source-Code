using System.Collections.Generic;
using UnityEngine;

public class BomberAllWalkZones : MonoBehaviour
{
	private List<BomberWalkZone> m_allWalkZones = new List<BomberWalkZone>();

	public void RegisterWalkZone(BomberWalkZone bwz)
	{
		m_allWalkZones.Add(bwz);
	}

	public BomberWalkZone FindContainingWalkZone(Vector3 inPoint, bool ignoreExternal)
	{
		float num = float.MaxValue;
		BomberWalkZone result = null;
		foreach (BomberWalkZone allWalkZone in m_allWalkZones)
		{
			if (allWalkZone != null && !allWalkZone.ShouldSkipInClickSearch() && (!ignoreExternal || !allWalkZone.IsExternal()))
			{
				Vector3 planarPointLocal = allWalkZone.GetPlanarPointLocal(inPoint);
				Vector3 nearestPlanarPointLocalFast = allWalkZone.GetNearestPlanarPointLocalFast(planarPointLocal);
				float sqrMagnitude = (planarPointLocal - nearestPlanarPointLocalFast).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					result = allWalkZone;
					num = sqrMagnitude;
				}
			}
		}
		return result;
	}
}
