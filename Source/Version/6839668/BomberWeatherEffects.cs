using System.Collections;
using BomberCrewCommon;
using UnityEngine;
using WingroveAudio;

public class BomberWeatherEffects : Singleton<BomberWeatherEffects>
{
	[SerializeField]
	private ParticleSystem m_cloudParticles;

	[SerializeField]
	private ParticleSystem m_heavySnowParticles;

	[SerializeField]
	private Material m_cloudsEffectMaterial;

	[SerializeField]
	private Material m_trailsEffectMaterial;

	[SerializeField]
	private ParticleSystem m_rainDropParticles;

	[SerializeField]
	private GameObject m_rainSplashes;

	[SerializeField]
	private ParticleSystem m_snowfallParticles;

	[SerializeField]
	private ParticleSystem m_lightSnowflakesParticles;

	[SerializeField]
	private BomberFlightPhysicsModel m_bomberPhysics;

	[SerializeField]
	private Collider[] m_lightningHitColliders;

	[SerializeField]
	private float m_velocityMultiplierClouds = 2.5f;

	[SerializeField]
	private float m_velocityMultiplierRain = 1.5f;

	[SerializeField]
	private GameObject m_lightningBolt;

	[SerializeField]
	private Transform m_lightningBoltLightTransform;

	[SerializeField]
	private float m_lightningFireChance = 0.3f;

	private float m_lightningStrikeTime = 1f;

	[SerializeField]
	private float m_lightningTimeMin;

	[SerializeField]
	private float m_lightningTimeMax;

	[SerializeField]
	private FireOverview m_fireOverview;

	private bool m_isLightningActive;

	private float m_timeToNextLightningStrike;

	private void Start()
	{
		EnableCloudEffects(enable: false);
		EnableHeavySnowEffects(enable: false);
		EnableRainEffects(enable: false);
		EnableSnowfallEffects(enable: false);
		m_lightningBolt.SetActive(value: false);
	}

	public void EnableCloudEffects(bool enable)
	{
		m_cloudParticles.enableEmission = enable;
	}

	public void EnableHeavySnowEffects(bool enable)
	{
		m_heavySnowParticles.enableEmission = enable;
	}

	public void EnableLightning(bool enable)
	{
		if (enable != m_isLightningActive)
		{
			m_isLightningActive = enable;
			if (enable)
			{
				m_timeToNextLightningStrike = Random.Range(m_lightningTimeMin, m_lightningTimeMax);
			}
		}
	}

	public void EnableRainEffects(bool enable)
	{
		m_rainDropParticles.enableEmission = enable;
		m_rainSplashes.SetActive(enable);
	}

	public void EnableSnowfallEffects(bool enable)
	{
		m_snowfallParticles.enableEmission = enable;
	}

	public void EnableLightSnowflakeEffects(bool enable)
	{
		m_lightSnowflakesParticles.enableEmission = enable;
	}

	private void ShowDebug()
	{
		if (GUILayout.Button("Toggle Clouds Weather Effects"))
		{
			EnableCloudEffects(!m_cloudParticles.emission.enabled);
		}
		if (GUILayout.Button("Toggle Rain Weather Effects"))
		{
			EnableRainEffects(!m_rainDropParticles.emission.enabled);
		}
		if (GUILayout.Button("Toggle Snowfall Weather Effects"))
		{
			EnableRainEffects(!m_snowfallParticles.emission.enabled);
		}
		if (GUILayout.Button("Toggle Light Snowflakes Weather Effects"))
		{
			EnableLightSnowflakeEffects(!m_lightSnowflakesParticles.emission.enabled);
		}
		if (GUILayout.Button("Lightning Strike"))
		{
			LightningStrike(m_bomberPhysics.transform.position, Random.rotation);
		}
	}

	private void OnDestroy()
	{
	}

	private void Update()
	{
		if (m_cloudParticles.enableEmission)
		{
			m_cloudsEffectMaterial.SetColor("_TintColor", Singleton<SkyboxDayNight>.Instance.GetAmbientColor());
			SetCloudsDirection();
		}
		m_trailsEffectMaterial.SetColor("_TintColor", Singleton<SkyboxDayNight>.Instance.GetAmbientColor());
		if (m_rainDropParticles.enableEmission)
		{
			SetRainDirection();
		}
		if (m_snowfallParticles.enableEmission)
		{
			SetSnowfallDirection();
		}
		if (!m_isLightningActive)
		{
			return;
		}
		m_timeToNextLightningStrike -= Time.deltaTime;
		if (!(m_timeToNextLightningStrike < 0f))
		{
			return;
		}
		m_timeToNextLightningStrike = Random.Range(m_lightningTimeMin, m_lightningTimeMax);
		int num = Random.Range(0, m_lightningHitColliders.Length);
		int num2 = 0;
		Collider collider = null;
		bool flag = true;
		Collider[] lightningHitColliders = m_lightningHitColliders;
		foreach (Collider collider2 in lightningHitColliders)
		{
			if (collider2 != null)
			{
				flag = false;
			}
		}
		while (true && !flag)
		{
			if (m_lightningHitColliders[num2 % m_lightningHitColliders.Length] != null)
			{
				if (num == 0)
				{
					collider = m_lightningHitColliders[num2 % m_lightningHitColliders.Length];
					break;
				}
				num--;
			}
			num2++;
		}
		if (collider != null)
		{
			float magnitude = collider.bounds.size.magnitude;
			Vector3 vector = collider.ClosestPoint(collider.bounds.center + new Vector3(Random.Range((0f - magnitude) * 2f, magnitude * 2f), magnitude * 2f, Random.Range((0f - magnitude) * 2f, magnitude * 2f)));
			if (Random.Range(0f, 1f) < m_lightningFireChance)
			{
				m_fireOverview.StartFire(vector);
			}
			LightningStrike(vector, Random.rotation);
		}
	}

	private void SetCloudsDirection()
	{
		Vector3 vector = m_bomberPhysics.GetVelocity() * m_velocityMultiplierClouds;
		m_cloudParticles.transform.position = m_bomberPhysics.transform.position + vector;
		m_cloudParticles.transform.LookAt(m_bomberPhysics.transform.position - vector);
	}

	private void SetRainDirection()
	{
		Vector3 vector = new Vector3(0f, 100f, 0f) + m_bomberPhysics.GetVelocity() * m_velocityMultiplierRain;
		m_rainDropParticles.transform.position = m_bomberPhysics.transform.position + vector;
		m_rainDropParticles.transform.LookAt(m_bomberPhysics.transform.position - vector);
	}

	private void SetSnowfallDirection()
	{
		Vector3 vector = new Vector3(0f, 16f, 0f) + m_bomberPhysics.GetVelocity() * m_velocityMultiplierRain;
		m_snowfallParticles.transform.position = m_bomberPhysics.transform.position + vector;
		m_snowfallParticles.transform.LookAt(m_bomberPhysics.transform.position - vector);
	}

	public void LightningStrike(Vector3 hitPosition, Quaternion hitNormal)
	{
		m_lightningBolt.transform.position = hitPosition;
		m_lightningBolt.transform.rotation = hitNormal;
		m_lightningBolt.SetActive(value: false);
		StopCoroutine("DoLightningStrike");
		StartCoroutine("DoLightningStrike");
	}

	private IEnumerator DoLightningStrike()
	{
		m_lightningBolt.SetActive(value: true);
		Singleton<BomberEffectLight>.Instance.AddLighting(3f, Color.white, m_lightningBoltLightTransform);
		WingroveRoot.Instance.PostEvent("LIGHTNING_STRIKE");
		yield return new WaitForSeconds(m_lightningStrikeTime);
		m_lightningBolt.SetActive(value: false);
		yield return null;
	}
}
