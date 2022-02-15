using BomberCrewCommon;
using UnityEngine;

public class BomberFuselageSectionGroup : MonoBehaviour
{
	[SerializeField]
	private BomberSystemUniqueId m_systemIdentifier;

	[SerializeField]
	private float m_multiplierFactor = 1f;

	private float m_currentHealth;

	private float m_initialHealth = 300f;

	private bool m_isInitialised;

	private FlashManager.ActiveFlash m_damageFlash;

	private void Start()
	{
		if (!m_isInitialised)
		{
			Init();
		}
	}

	private void Init()
	{
		m_currentHealth = (m_initialHealth = (float)m_systemIdentifier.GetUpgrade().GetArmour() * m_multiplierFactor);
		m_isInitialised = true;
	}

	public float GetHealth()
	{
		if (!m_isInitialised)
		{
			Init();
		}
		return m_currentHealth;
	}

	public void UndoDamage(float amtN)
	{
		if (!m_isInitialised)
		{
			Init();
		}
		m_currentHealth += m_initialHealth * amtN;
		if (m_currentHealth > m_initialHealth)
		{
			m_currentHealth = m_initialHealth;
		}
	}

	public void ChangeHealth(float changeAmt)
	{
		if (!m_isInitialised)
		{
			Init();
		}
		float currentHealth = m_currentHealth;
		m_currentHealth += changeAmt;
		if (m_currentHealth < 0f)
		{
			m_currentHealth = 0f;
		}
		float num = currentHealth - m_currentHealth;
		if (num > 0f)
		{
			Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetMissionLog().AddDamageTaken(num);
		}
	}

	public float GetMaxHealth()
	{
		if (!m_isInitialised)
		{
			Init();
		}
		return m_initialHealth;
	}
}
