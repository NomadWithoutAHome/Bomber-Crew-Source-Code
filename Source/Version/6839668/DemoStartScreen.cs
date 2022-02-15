using BomberCrewCommon;
using UnityEngine;

public class DemoStartScreen : MonoBehaviour
{
	[SerializeField]
	private AirbaseCameraNode m_thisCameraNode;

	private void OnEnable()
	{
		Singleton<AirbaseCameraController>.Instance.MoveCameraToLocation(m_thisCameraNode);
	}
}
