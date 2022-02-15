using BomberCrewCommon;
using UnityEngine;

public class BomberTemperatureOxygen : MonoBehaviour
{
	[SerializeField]
	private BomberState m_bomberState;

	[SerializeField]
	private float m_temperatureAtLowAltitude;

	[SerializeField]
	private float m_temperatureAtHighAltitude;

	[SerializeField]
	private float m_temperatureAltIgnore;

	[SerializeField]
	private float m_temperatureModifyNight;

	[SerializeField]
	private float m_oxygenAtLowAltitude;

	[SerializeField]
	private float m_oxygenAtHighAltitude;

	[SerializeField]
	private float m_oxygenAltIgnore;

	[SerializeField]
	private float m_winterEnvironmentTemperatureModify = -4f;

	[SerializeField]
	private float m_snowstormTemperatureModify = -4f;

	private float m_currentTemperature;

	private float m_currentOxygen;

	private bool m_isSetUp;

	private bool m_isInactive;

	private float m_temperatureModifier;

	private WeatherSensor m_weatherSensor;

	private void Start()
	{
		m_weatherSensor = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetWeatherSensor();
	}

	public void SetWinterEnvironmentOn()
	{
		m_temperatureModifier = m_winterEnvironmentTemperatureModify;
	}

	public float GetCurrentTemperature()
	{
		return (!m_isInactive) ? m_currentTemperature : 5f;
	}

	public float GetCurrentOxygen()
	{
		return (!m_isInactive) ? m_currentOxygen : 1f;
	}

	public void SetInactive(bool inactive)
	{
		m_isInactive = inactive;
	}

	public bool IsInactive()
	{
		return m_isInactive;
	}

	private void Update()
	{
		float altitudeNormalised = m_bomberState.GetAltitudeNormalised();
		float t = Mathf.Clamp01((altitudeNormalised - m_temperatureAltIgnore) / (1f - m_temperatureAltIgnore));
		float num = Mathf.Lerp(m_temperatureAtLowAltitude, m_temperatureAtHighAltitude, t);
		float nightFactor = Singleton<VisibilityHelpers>.Instance.GetNightFactor();
		num += nightFactor * m_temperatureModifyNight;
		num += m_temperatureModifier;
		if (m_weatherSensor.GetHeavySnow())
		{
			num += m_snowstormTemperatureModify;
		}
		float num2 = num - m_currentTemperature;
		m_currentTemperature += Time.deltaTime * num2;
		if (!m_isSetUp)
		{
			m_currentTemperature = num;
		}
		float t2 = Mathf.Clamp01((altitudeNormalised - m_oxygenAltIgnore) / (1f - m_oxygenAltIgnore));
		float num3 = Mathf.Lerp(m_oxygenAtLowAltitude, m_oxygenAtHighAltitude, t2);
		num2 = num3 - m_currentOxygen;
		m_currentOxygen += Time.deltaTime * num2;
		if (!m_isSetUp)
		{
			m_currentOxygen = num3;
		}
		m_isSetUp = true;
	}
}
