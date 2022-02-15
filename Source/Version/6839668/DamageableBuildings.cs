using UnityEngine;

public class DamageableBuildings : Damageable
{
	[SerializeField]
	private float m_startHP = 100f;

	[SerializeField]
	private GameObject m_model;

	[SerializeField]
	private GameObject m_modelDestroyed;

	private float m_hp;

	private bool m_destroyed;

	private void Start()
	{
		m_hp = m_startHP;
	}

	public override bool IsDamageBlocker()
	{
		return false;
	}

	public override float DamageGetPassthrough(float amt, DamageSource ds)
	{
		m_hp -= amt;
		if (!m_destroyed && m_hp <= 0f)
		{
			m_destroyed = true;
			m_model.SetActive(value: false);
			if (m_modelDestroyed != null)
			{
				m_modelDestroyed.SetActive(value: true);
			}
		}
		return 0f;
	}
}
