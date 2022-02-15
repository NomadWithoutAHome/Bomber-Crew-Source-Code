using BomberCrewCommon;
using UnityEngine;

public class LandingGearSingle : SmoothDamageable
{
	[SerializeField]
	private Animation m_animation;

	[SerializeField]
	private Transform m_trackingGearTransform;

	[SerializeField]
	private float m_startingHealth;

	[SerializeField]
	private DamageEffects m_damageEffects;

	[SerializeField]
	private Collider m_damageCollider;

	private float m_currentHealth;

	private void Start()
	{
		m_currentHealth = m_startingHealth;
	}

	public void UpdateFromState(float openness)
	{
		string animation = m_animation.clip.name;
		m_animation.Play(animation);
		m_animation[animation].enabled = true;
		m_animation[animation].speed = 0f;
		m_animation[animation].normalizedTime = openness;
	}

	public Transform GetTrackingTransform()
	{
		return m_trackingGearTransform;
	}

	public override bool IsDamageBlocker()
	{
		return false;
	}

	public override float DamageGetPassthrough(float amt, DamageSource damageSource)
	{
		if (damageSource.m_damageType != DamageSource.DamageType.Fire)
		{
			m_damageEffects.DoDamageEffect(m_damageCollider, damageSource, amt);
			Singleton<DboxInMissionController>.Instance.DoDamageEffect(damageSource.m_position, amt, Singleton<BomberSpawn>.Instance.GetBomberSystems().transform);
			m_currentHealth -= amt;
			if (m_currentHealth < 0f)
			{
				m_currentHealth = 0f;
			}
		}
		return 0f;
	}

	public override float GetHealthNormalised()
	{
		return Mathf.Clamp01(m_currentHealth / m_startingHealth);
	}

	public override bool DirectDamageOnly()
	{
		return true;
	}
}
