using BomberCrewCommon;
using UnityEngine;

public class BomberEffectLight : Singleton<BomberEffectLight>
{
	[SerializeField]
	private Light m_light;

	[SerializeField]
	private float m_fadeRate = 8f;

	private void Start()
	{
		m_light.enabled = false;
	}

	public void AddLighting(float intensity, Color lightColor, Transform lightTransform)
	{
		m_light.enabled = true;
		m_light.intensity = intensity;
		m_light.color = lightColor;
		m_light.transform.rotation = Quaternion.LookRotation((base.transform.position - lightTransform.position).normalized, Vector3.up);
	}

	private void Update()
	{
		if (m_light.enabled)
		{
			m_light.intensity -= m_fadeRate * Time.deltaTime;
			if (m_light.intensity <= 0f)
			{
				m_light.enabled = false;
			}
		}
	}
}
