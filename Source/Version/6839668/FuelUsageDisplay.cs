using BomberCrewCommon;
using UnityEngine;

public class FuelUsageDisplay : MonoBehaviour
{
	private BomberSystems m_bomberSystems;

	[SerializeField]
	private UIGaugeDisplay m_altitudeGauge;

	private void Start()
	{
		m_bomberSystems = Singleton<BomberSpawn>.Instance.GetBomberSystems();
	}

	private void Update()
	{
		float fuelUsage = m_bomberSystems.GetBomberState().GetPhysicsModel().GetFuelUsage();
		float value = Mathf.Clamp01((fuelUsage - 0.4f) * 0.75f);
		m_altitudeGauge.DisplayValue(value);
	}
}
