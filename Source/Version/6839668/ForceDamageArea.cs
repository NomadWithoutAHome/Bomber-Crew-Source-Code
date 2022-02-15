using BomberCrewCommon;
using UnityEngine;

public class ForceDamageArea : MonoBehaviour
{
	[SerializeField]
	private MissionPlaceableObject m_placeableObject;

	private bool m_triggered;

	private void Update()
	{
		if (!m_triggered)
		{
			Vector3d position = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState().GetBigTransform()
				.position;
			if (m_placeableObject.IsPointWithinArea(position))
			{
				m_triggered = true;
			}
		}
	}
}
