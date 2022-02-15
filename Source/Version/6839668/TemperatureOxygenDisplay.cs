using BomberCrewCommon;
using UnityEngine;

public class TemperatureOxygenDisplay : MonoBehaviour
{
	[SerializeField]
	private UIGaugeDisplay[] m_temperatureGauges;

	[SerializeField]
	private UIGaugeDisplay[] m_oxygenGauges;

	[SerializeField]
	private GameObject m_enabledHierarchy;

	[SerializeField]
	private GameObject m_warningLightTemperature;

	[SerializeField]
	private GameObject m_warningLightOxygen;

	[SerializeField]
	private float m_dangerLevelOxygen = 0.25f;

	[SerializeField]
	private float m_temperatureZeroVal;

	[SerializeField]
	private float m_temperatureOneVal;

	private BomberTemperatureOxygen m_temperatureOxygen;

	private void Start()
	{
		m_temperatureOxygen = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetTemperatureOxygen();
	}

	private void Update()
	{
		m_enabledHierarchy.SetActive(!m_temperatureOxygen.IsInactive());
		int num = int.MaxValue;
		foreach (CrewSpawner.CrewmanAvatarPairing item in Singleton<CrewSpawner>.Instance.GetAllCrew())
		{
			if (!item.m_crewman.IsDead() && !item.m_spawnedAvatar.IsBailedOut())
			{
				num = Mathf.Min(item.m_crewman.GetTemperatureResistance(), num);
			}
		}
		UIGaugeDisplay[] temperatureGauges = m_temperatureGauges;
		foreach (UIGaugeDisplay uIGaugeDisplay in temperatureGauges)
		{
			float value = Mathf.InverseLerp(m_temperatureZeroVal, m_temperatureOneVal, m_temperatureOxygen.GetCurrentTemperature());
			uIGaugeDisplay.DisplayValue(value);
		}
		if (m_temperatureOxygen.GetCurrentTemperature() <= (float)(-num))
		{
			m_warningLightTemperature.SetActive(value: true);
		}
		else
		{
			m_warningLightTemperature.SetActive(value: false);
		}
		UIGaugeDisplay[] oxygenGauges = m_oxygenGauges;
		foreach (UIGaugeDisplay uIGaugeDisplay2 in oxygenGauges)
		{
			uIGaugeDisplay2.DisplayValue(m_temperatureOxygen.GetCurrentOxygen());
		}
		if (m_temperatureOxygen.GetCurrentOxygen() < m_dangerLevelOxygen)
		{
			m_warningLightOxygen.SetActive(value: true);
		}
		else
		{
			m_warningLightOxygen.SetActive(value: false);
		}
	}
}
