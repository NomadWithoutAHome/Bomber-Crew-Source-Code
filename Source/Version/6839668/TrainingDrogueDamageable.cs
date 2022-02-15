using BomberCrewCommon;
using UnityEngine;

public class TrainingDrogueDamageable : SmoothDamageable
{
	[SerializeField]
	private float m_healthMax;

	[SerializeField]
	private GameObject m_hideWhenDestroyed;

	[SerializeField]
	private TaggableFighter m_taggableFighter;

	private float m_currentHealth;

	private bool m_isDestroyed;

	private void Awake()
	{
		m_currentHealth = m_healthMax;
	}

	public override float DamageGetPassthrough(float amt, DamageSource ds)
	{
		if (!m_isDestroyed)
		{
			m_currentHealth -= amt;
			if (ds.m_damageShapeEffect == DamageSource.DamageShape.Line)
			{
				Singleton<CommonEffectManager>.Instance.EffectHit(ds.m_raycastInfo.point, Quaternion.LookRotation(ds.m_raycastInfo.normal), base.transform, CommonEffectManager.AudioHitType.Fuselage, ds.m_fromProjectile);
			}
			if (m_currentHealth < 0f)
			{
				m_taggableFighter.BlockTagging();
				m_currentHealth = 0f;
				m_hideWhenDestroyed.SetActive(value: false);
				m_isDestroyed = true;
			}
		}
		return 0f;
	}

	public override float GetHealthNormalised()
	{
		return m_currentHealth / m_healthMax;
	}

	public override bool IsDamageBlocker()
	{
		return true;
	}

	public bool IsDestroyed()
	{
		return m_isDestroyed;
	}
}
