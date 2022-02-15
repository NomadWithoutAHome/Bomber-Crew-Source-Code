using UnityEngine;

public class DamageableHomingRocket : Damageable
{
	[SerializeField]
	private float m_startingHealth;

	[SerializeField]
	private ProjectileEffectRocketHoming m_rocket;

	private bool m_isDestroyed;

	private float m_health;

	private void Awake()
	{
		m_health = m_startingHealth;
	}

	public override float DamageGetPassthrough(float amt, DamageSource damageSource)
	{
		if (!m_isDestroyed)
		{
			m_health -= amt;
			if (m_health < 0f)
			{
				m_isDestroyed = true;
				m_rocket.DestroyEarly();
			}
		}
		return 0f;
	}

	public override bool DirectDamageOnly()
	{
		return true;
	}

	public override bool IsDamageBlocker()
	{
		return false;
	}
}
