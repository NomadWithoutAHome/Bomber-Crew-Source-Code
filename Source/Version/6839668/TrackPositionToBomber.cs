using BomberCrewCommon;
using UnityEngine;

public class TrackPositionToBomber : MonoBehaviour
{
	private Transform m_bomberTransform;

	private void Update()
	{
		if (m_bomberTransform == null)
		{
			m_bomberTransform = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState().transform;
		}
		else
		{
			base.transform.position = m_bomberTransform.position;
		}
	}
}
