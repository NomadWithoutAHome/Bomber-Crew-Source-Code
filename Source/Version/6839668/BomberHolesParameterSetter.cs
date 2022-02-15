using AudioNames;
using BomberCrewCommon;
using UnityEngine;
using WingroveAudio;

public class BomberHolesParameterSetter : MonoBehaviour
{
	[SerializeField]
	private BomberFuselageSection[] m_sectionsToMonitor;

	private WeatherSensor m_weatherSensor;

	private float m_storm;

	private void Start()
	{
		m_weatherSensor = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetWeatherSensor();
	}

	private void Update()
	{
		float num = 0f;
		int num2 = 0;
		BomberFuselageSection[] sectionsToMonitor = m_sectionsToMonitor;
		foreach (BomberFuselageSection bomberFuselageSection in sectionsToMonitor)
		{
			num += bomberFuselageSection.GetHealthNormalised();
			num2++;
		}
		if (num2 != 0)
		{
			float num3 = Mathf.Pow(1f - num / (float)num2, 2f);
			if (m_weatherSensor.GetHeavySnow())
			{
				m_storm += Time.deltaTime;
			}
			else
			{
				m_storm -= Time.deltaTime;
			}
			m_storm = Mathf.Clamp01(m_storm);
			WingroveRoot.Instance.SetParameterGlobal(GameEvents.Parameters.CacheVal_BomberHoles(), Mathf.Clamp01(num3 + m_storm));
		}
	}
}
