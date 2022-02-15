using BomberCrewCommon;
using UnityEngine;

public class FuelDisplay : MonoBehaviour
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
		float num = 0f;
		for (int i = 0; i < 2; i++)
		{
			FuelTank fuelTank = m_bomberSystems.GetFuelTank(i);
			num += fuelTank.GetFuel();
		}
		m_altitudeGauge.DisplayValue(num);
	}
}
