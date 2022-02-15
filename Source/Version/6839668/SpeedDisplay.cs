using BomberCrewCommon;
using UnityEngine;

public class SpeedDisplay : MonoBehaviour
{
	private BomberState m_bomberState;

	[SerializeField]
	private UIGaugeDisplay m_altitudeGauge;

	private void Start()
	{
		m_bomberState = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState();
	}

	private void Update()
	{
		Vector3 velocity = m_bomberState.GetPhysicsModel().GetVelocity();
		velocity.y = 0f;
		m_altitudeGauge.DisplayValue(velocity.magnitude);
	}
}
