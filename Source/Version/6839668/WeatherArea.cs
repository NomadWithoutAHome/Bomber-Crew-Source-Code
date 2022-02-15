using UnityEngine;

public class WeatherArea : MonoBehaviour
{
	[SerializeField]
	private bool m_rain;

	[SerializeField]
	private bool m_lightning;

	[SerializeField]
	private bool m_cloudFog;

	[SerializeField]
	private bool m_heavySnow;

	public bool IsRain()
	{
		return m_rain;
	}

	public bool IsLightning()
	{
		return m_lightning;
	}

	public bool IsCloudFog()
	{
		return m_cloudFog;
	}

	public bool IsHeavySnow()
	{
		return m_heavySnow;
	}
}
