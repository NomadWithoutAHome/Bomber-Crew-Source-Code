using UnityEngine;

public class EffectScalerHelper : MonoBehaviour
{
	[SerializeField]
	protected ParticleSystem m_particleSystem;

	protected ParticleSystem.EmissionModule m_particleSystemEmission;

	protected ParticleSystem.MainModule m_particleSystemMain;

	protected bool m_isSetup;

	private void Start()
	{
		if (!m_isSetup)
		{
			SetUp();
		}
	}

	public virtual void SetUp()
	{
		m_particleSystemEmission = m_particleSystem.emission;
		m_particleSystemMain = m_particleSystem.main;
		m_isSetup = true;
		SetEffectScale(0f);
	}

	public virtual void SetEffectScale(float effectScale)
	{
		if (effectScale == 0f)
		{
			m_particleSystemEmission.enabled = false;
		}
		else
		{
			m_particleSystemEmission.enabled = true;
		}
	}
}
