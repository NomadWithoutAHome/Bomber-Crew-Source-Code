using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class MissionTargetAreaMarker : MonoBehaviour
{
	[SerializeField]
	private MissionPlaceableObject m_placeable;

	private void Start()
	{
		if (!(Singleton<MissionCoordinator>.Instance != null))
		{
			return;
		}
		List<MissionPlaceableObject> objectsByType = Singleton<MissionCoordinator>.Instance.GetObjectsByType("MissionTarget");
		if (objectsByType == null)
		{
			return;
		}
		int num = 0;
		foreach (MissionPlaceableObject item in objectsByType)
		{
			if (item != null && item != m_placeable)
			{
				num++;
				Object.Destroy(item.gameObject);
			}
		}
		if (num == 1)
		{
			return;
		}
		foreach (MissionPlaceableObject item2 in objectsByType)
		{
			if (item2 == m_placeable)
			{
				break;
			}
			if (item2 != null && item2 != m_placeable)
			{
				Object.Destroy(item2.gameObject);
			}
		}
	}
}
