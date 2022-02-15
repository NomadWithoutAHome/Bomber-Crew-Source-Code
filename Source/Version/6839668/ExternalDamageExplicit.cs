using UnityEngine;

public class ExternalDamageExplicit : MonoBehaviour
{
	[SerializeField]
	private float m_damageTotal = 50f;

	[SerializeField]
	private float m_damageMin;

	[SerializeField]
	private float m_radius;

	[SerializeField]
	private bool m_fast;

	[SerializeField]
	private DamageSource.DamageType m_damageType;

	[SerializeField]
	private bool m_onceOnStart;

	public void Start()
	{
		if (m_onceOnStart)
		{
			DoDamage();
		}
	}

	public float GetDamage()
	{
		return m_damageTotal;
	}

	public void DoDamage()
	{
		if (m_fast)
		{
			DamageUtils.DoDamageSphereFastEverything(base.transform.position, m_damageMin, m_damageTotal, m_radius, m_damageType);
		}
		else
		{
			DamageUtils.DoDamageSlowWithBlock(base.transform.position, m_damageMin, m_damageTotal, m_radius, m_damageType);
		}
	}

	public void DoDamage(float multiplier)
	{
		if (m_fast)
		{
			DamageUtils.DoDamageSphereFastEverything(base.transform.position, m_damageMin, m_damageTotal * multiplier, m_radius, m_damageType);
		}
		else
		{
			DamageUtils.DoDamageSlowWithBlock(base.transform.position, m_damageMin, m_damageTotal * multiplier, m_radius, m_damageType);
		}
	}
}
