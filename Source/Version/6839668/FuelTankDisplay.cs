using BomberCrewCommon;
using UnityEngine;

public class FuelTankDisplay : MonoBehaviour
{
	[SerializeField]
	private int m_fuelTankIndex;

	[SerializeField]
	private UIGaugeDisplay m_gauge;

	[SerializeField]
	private GameObject m_warningLightEmpty;

	[SerializeField]
	private GameObject m_warningLightLeaking;

	[SerializeField]
	private float m_fuelTankWarningThreshold;

	private FuelTank m_fuelTank;

	private void Start()
	{
		BomberSystems bomberSystems = Singleton<BomberSpawn>.Instance.GetBomberSystems();
		m_fuelTank = bomberSystems.GetFuelTank(m_fuelTankIndex);
		if (m_warningLightEmpty != null)
		{
			m_warningLightEmpty.SetActive(value: false);
		}
	}

	private void Update()
	{
		m_gauge.DisplayValue(m_fuelTank.GetFuelNormalised());
		if (m_fuelTank.GetFuelNormalised() <= 0f)
		{
			m_warningLightEmpty.SetActive(value: true);
		}
		else if (m_fuelTank.GetFuelNormalised() < m_fuelTankWarningThreshold || m_fuelTank.IsLeaking())
		{
			m_warningLightLeaking.SetActive(value: true);
		}
		else
		{
			m_warningLightLeaking.SetActive(value: false);
		}
	}
}
