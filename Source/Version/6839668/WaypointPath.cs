using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class WaypointPath : MonoBehaviour
{
	[SerializeField]
	private MissionPlaceableObject m_placeable;

	private List<MissionPlaceableObject> m_allWaypoints = new List<MissionPlaceableObject>();

	private void Start()
	{
		int num = 0;
		while (true)
		{
			string param = "Waypoint_" + num;
			string parameter = m_placeable.GetParameter(param);
			if (parameter == null)
			{
				break;
			}
			m_allWaypoints.Add(Singleton<MissionCoordinator>.Instance.GetPlaceableByRef(parameter));
			num++;
		}
	}

	public List<MissionPlaceableObject> GetAllWaypoints()
	{
		return m_allWaypoints;
	}
}
