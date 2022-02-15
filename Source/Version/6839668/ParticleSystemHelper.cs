using UnityEngine;

public class ParticleSystemHelper : MonoBehaviour
{
	[SerializeField]
	private int m_emitCountMin = 1;

	[SerializeField]
	private int m_emitCountMax = 1;

	[SerializeField]
	private Vector2 m_sizeRange = new Vector2(0.5f, 1f);

	[SerializeField]
	private Vector2 m_lifetimeRange = new Vector2(0.5f, 1f);

	[SerializeField]
	private bool m_useRandomVelocity;

	[SerializeField]
	private Vector3 m_randomVelocityMin;

	[SerializeField]
	private Vector3 m_randomVelocityMax;

	private Vector3 m_velocity = Vector3.zero;

	[SerializeField]
	private Color m_startColour = Color.white;

	[SerializeField]
	private ParticleSystem m_particleSystem;

	private void Awake()
	{
		if (m_particleSystem == null)
		{
			m_particleSystem = m_particleSystem;
		}
	}

	public void Initialise()
	{
		m_particleSystem.emissionRate = 0f;
		DisableParticleEmission();
	}

	public void SetParticlesLifeTime(float time)
	{
		m_particleSystem.startLifetime = time;
	}

	public float GetParticlesLifeTime()
	{
		return m_particleSystem.startLifetime;
	}

	public void EmitParticlesBurst()
	{
		EnableParticleEmission();
		m_particleSystem.Play();
		m_particleSystem.Emit(GetRandomCount());
	}

	public void EmitParticlesBurst(int count)
	{
		EnableParticleEmission();
		m_particleSystem.Play();
		m_particleSystem.Emit(count);
	}

	public void EmitParticleAtPos(Vector3 pos)
	{
		EnableParticleEmission();
		m_particleSystem.Play();
		if (m_useRandomVelocity)
		{
			m_velocity = GetRandomVelocity();
		}
		m_particleSystem.Emit(pos, m_velocity, Random.Range(m_sizeRange.x, m_sizeRange.y), Random.Range(m_lifetimeRange.x, m_lifetimeRange.y), m_startColour);
	}

	public void EmitParticlesAtPos(Vector3 pos)
	{
		EnableParticleEmission();
		m_particleSystem.Play();
		for (int num = GetRandomCount(); num >= 0; num--)
		{
			if (m_useRandomVelocity)
			{
				m_velocity = GetRandomVelocity();
			}
			m_particleSystem.Emit(pos, m_velocity, Random.Range(m_sizeRange.x, m_sizeRange.y), Random.Range(m_lifetimeRange.x, m_lifetimeRange.y), m_startColour);
		}
	}

	private Vector3 GetRandomVelocity()
	{
		return new Vector3(Random.Range(m_randomVelocityMin.x, m_randomVelocityMax.x), Random.Range(m_randomVelocityMin.y, m_randomVelocityMax.y), Random.Range(m_randomVelocityMin.z, m_randomVelocityMax.z));
	}

	private int GetRandomCount()
	{
		return Random.Range(m_emitCountMin, m_emitCountMax);
	}

	public void StartParticleEmission()
	{
		EnableParticleEmission();
		m_particleSystem.Play();
		m_particleSystem.emissionRate = GetRandomCount();
	}

	public void StopParticleEmission()
	{
		m_particleSystem.emissionRate = 0f;
		m_particleSystem.Stop();
	}

	public void EnableParticleEmission()
	{
		m_particleSystem.enableEmission = true;
		m_particleSystem.GetComponent<Renderer>().enabled = true;
	}

	public void DisableParticleEmission()
	{
		m_particleSystem.enableEmission = false;
		m_particleSystem.GetComponent<Renderer>().enabled = false;
	}
}
