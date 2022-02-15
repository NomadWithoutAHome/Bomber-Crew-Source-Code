using BomberCrewCommon;
using UnityEngine;

public class AreaTrigger : MonoBehaviour
{
	[SerializeField]
	private MissionPlaceableObject m_placeable;

	private void Start()
	{
	}

	private void Update()
	{
		Vector3d position = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBigTransform().position;
		if (m_placeable.IsPointWithinArea(position))
		{
			Singleton<MissionCoordinator>.Instance.FireTrigger(m_placeable.GetParameter("trigger"));
		}
	}
}
