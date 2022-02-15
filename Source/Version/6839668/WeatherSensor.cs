using UnityEngine;
using WingroveAudio;

public class WeatherSensor : MonoBehaviour
{
	[SerializeField]
	private float m_radius;

	[SerializeField]
	private LayerMask m_layers;

	[SerializeField]
	private BomberWeatherEffects m_effects;

	private bool m_rain;

	private bool m_snow;

	private bool m_lightning;

	private bool m_cloudFog;

	private bool m_heavySnow;

	public bool GetRain()
	{
		return m_rain;
	}

	public bool GetSnow()
	{
		return m_snow;
	}

	public bool GetHeavySnow()
	{
		return m_heavySnow;
	}

	public bool GetLightning()
	{
		return m_lightning;
	}

	public bool GetCloudFog()
	{
		return m_cloudFog;
	}

	private void Awake()
	{
		WingroveRoot.Instance.PostEvent("CLOUD_EXIT");
	}

	public void SetFromCloudGrid(bool snow, bool rain, bool lightning)
	{
		rain &= base.transform.position.y <= 270f;
		if (rain != m_rain)
		{
			m_rain = rain;
			if (m_rain)
			{
				WingroveRoot.Instance.PostEventGO("RAIN_START", base.gameObject);
			}
			else
			{
				WingroveRoot.Instance.PostEventGO("RAIN_STOP", base.gameObject);
			}
			m_effects.EnableRainEffects(m_rain);
		}
		snow &= base.transform.position.y <= 270f;
		if (snow != m_snow)
		{
			m_snow = snow;
			m_effects.EnableSnowfallEffects(m_snow);
		}
	}

	private void Update()
	{
		Collider[] array = Physics.OverlapSphere(base.transform.position, m_radius, m_layers);
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			if (collider.isTrigger)
			{
				WeatherArea component = collider.GetComponent<WeatherArea>();
				if (component != null)
				{
					flag |= component.IsLightning();
					flag2 |= component.IsCloudFog();
					flag3 |= component.IsHeavySnow();
				}
			}
		}
		if (flag2 != m_cloudFog)
		{
			WingroveRoot.Instance.PostEvent("CLOUD_PUFF");
			m_cloudFog = flag2;
			m_effects.EnableCloudEffects(m_cloudFog);
			if (flag2)
			{
				WingroveRoot.Instance.PostEvent("CLOUD_ENTER");
			}
			else
			{
				WingroveRoot.Instance.PostEvent("CLOUD_EXIT");
			}
		}
		if (flag != m_lightning)
		{
			m_lightning = flag;
			m_effects.EnableLightning(m_lightning);
		}
		if (flag3 != m_heavySnow)
		{
			m_heavySnow = flag3;
			m_effects.EnableHeavySnowEffects(m_heavySnow);
		}
	}
}
