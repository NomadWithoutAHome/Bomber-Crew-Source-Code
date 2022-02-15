using UnityEngine;

public class LightFlicker : MonoBehaviour
{
	private Light m_light;

	private float m_lightIntensity;

	[SerializeField]
	private float m_flickerInterval = 0.01f;

	private float m_flickerIntervalCounter;

	[SerializeField]
	private float m_flickerMagnitude = 0.25f;

	[SerializeField]
	private float m_fadeRate = 1f;

	[SerializeField]
	private bool m_randomDirection;

	private void Start()
	{
		m_light = base.gameObject.GetComponent<Light>();
		m_lightIntensity = m_light.intensity;
	}

	private void Update()
	{
		m_flickerIntervalCounter += Time.deltaTime;
		if (m_flickerIntervalCounter >= m_flickerInterval)
		{
			float intensity = Random.Range(m_lightIntensity - m_flickerMagnitude, m_lightIntensity + m_flickerMagnitude);
			m_light.intensity = intensity;
			base.transform.eulerAngles = new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
			m_flickerIntervalCounter = 0f;
		}
		m_light.intensity -= Time.deltaTime * m_fadeRate;
	}
}
