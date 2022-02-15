using System;
using BomberCrewCommon;
using UnityEngine;

public class CrewmanLifeStatus : MonoBehaviour
{
	[SerializeField]
	private float m_startingHealth;

	[SerializeField]
	private float m_standardHealAmount = 40f;

	[SerializeField]
	private float m_maxHealAmount = 100f;

	[SerializeField]
	private float m_standardCountdownDuration = 12f;

	[SerializeField]
	private float m_damageFactorReductionPerArmourPoint = 1.5f;

	[SerializeField]
	private float m_temperatureDamagePerSecondPerDegreeUnderZero = 0.1f;

	[SerializeField]
	private float m_minTemperatureDamage = 4f;

	[SerializeField]
	private float m_temperatureRecoverySpeed = 30f;

	[SerializeField]
	private float m_noOxygenDamagePerSecond = 0.5f;

	[SerializeField]
	private float m_oxygenRecoverySpeed = 30f;

	[SerializeField]
	private float m_minOxygenDamage = 8f;

	[SerializeField]
	private CrewmanSkillAbilityFloat m_healSkill;

	[SerializeField]
	private float m_temperatureBelowZeroGate = 4f;

	[SerializeField]
	private float m_temperatureBelowZeroMax = 20f;

	private float m_currentHealth;

	private float m_currentTemperatureDamage;

	private float m_currentOxygenDamage;

	private float m_temperatureResist;

	private int m_damageResist;

	private int m_oxygenSeconds;

	private bool m_isCountingDown;

	private float m_currentHealthCountdown;

	private bool m_isDead;

	private float m_healthRedCounter;

	private float m_temperature;

	private bool m_hasOxygen;

	private float m_temperatureAccumulator;

	private float m_oxygenAccumulator;

	private float m_oxygenCountupTimer;

	private bool m_healedThisFrame;

	private float m_overDamageIncapacitated;

	private string m_healSkillId;

	public event Action OnCountdownBegin;

	public event Action OnRevive;

	public event Action OnDead;

	private void Awake()
	{
		m_healSkillId = m_healSkill.GetCachedName();
	}

	public void SetUp(int temperatureResistance, int damageResistance, int oxygenSeconds)
	{
		m_currentHealth = m_startingHealth;
		m_damageResist = damageResistance;
		m_oxygenSeconds = oxygenSeconds;
		m_temperatureResist = temperatureResistance;
	}

	public float GetTotalHealth()
	{
		return m_currentHealth - (m_currentTemperatureDamage + m_currentOxygenDamage);
	}

	public void SetHealing()
	{
		m_healedThisFrame = true;
	}

	public float GetPhysicalHealth()
	{
		return m_currentHealth;
	}

	public float GetTotalHealthN()
	{
		return GetTotalHealth() / m_startingHealth;
	}

	public float GetPhysicalHealthN()
	{
		return m_currentHealth / m_startingHealth;
	}

	public float GetOxygenTemperatureN()
	{
		return (m_currentTemperatureDamage + m_currentOxygenDamage) / m_startingHealth;
	}

	public float GetOxygenN()
	{
		return m_currentOxygenDamage / m_startingHealth;
	}

	public float GetRedDisplayN()
	{
		return m_healthRedCounter / m_startingHealth;
	}

	public void DoDamage(float amt)
	{
		if (!m_isDead && !m_isCountingDown)
		{
			float num = Mathf.Clamp01(m_damageFactorReductionPerArmourPoint * (float)m_damageResist);
			m_currentHealth -= amt;
			m_healthRedCounter += amt;
		}
		else if (!m_isDead && !m_healedThisFrame)
		{
			m_overDamageIncapacitated += amt * 0.5f;
		}
	}

	public void ForceDead()
	{
		m_isDead = true;
		this.OnDead();
	}

	public void Heal(Crewman fromCrewman)
	{
		if (m_isDead)
		{
			return;
		}
		if (m_isCountingDown)
		{
			if (this.OnRevive != null)
			{
				this.OnRevive();
			}
			m_isCountingDown = false;
			m_overDamageIncapacitated = 0f;
		}
		float proficiency = Singleton<CrewmanSkillUpgradeInfo>.Instance.GetProficiency(m_healSkillId, fromCrewman);
		m_currentHealth += Mathf.Lerp(m_standardHealAmount, m_maxHealAmount, proficiency);
		m_currentHealth = Mathf.Clamp(m_currentHealth, 0f, m_startingHealth);
		m_currentTemperatureDamage = 0f;
		m_currentOxygenDamage = 0f;
	}

	public void HealFractional(Crewman fromCrewman, float fraction)
	{
		if (!m_isDead && !m_isCountingDown)
		{
			float proficiency = Singleton<CrewmanSkillUpgradeInfo>.Instance.GetProficiency(m_healSkillId, fromCrewman);
			m_currentHealth += Mathf.Lerp(m_standardHealAmount, m_maxHealAmount, proficiency) * fraction;
			m_currentHealth = Mathf.Clamp(m_currentHealth, 0f, m_startingHealth);
			m_currentTemperatureDamage = 0f;
			m_currentOxygenDamage = 0f;
		}
	}

	public bool IsCountingDown()
	{
		return m_isCountingDown && !m_isDead;
	}

	public bool IsDead()
	{
		return m_isDead;
	}

	public void InstantKill()
	{
		if (!m_isDead)
		{
			if (this.OnDead != null)
			{
				this.OnDead();
			}
			m_isDead = true;
		}
	}

	public float GetHealthCountdownNormalised()
	{
		return Mathf.Clamp01(m_currentHealthCountdown / m_standardCountdownDuration);
	}

	public float GetHealthCountdown()
	{
		return m_currentHealthCountdown;
	}

	public void SetStats(float temperature, bool hasOxygen)
	{
		m_temperature = temperature;
		m_hasOxygen = hasOxygen;
	}

	public bool AffectedByTemperature()
	{
		return m_temperature + m_temperatureResist < 0f;
	}

	public bool AffectedByOxygen()
	{
		return m_oxygenCountupTimer > (float)m_oxygenSeconds;
	}

	public void Update()
	{
		if (GetTotalHealth() <= 0f)
		{
			m_overDamageIncapacitated = 0f - GetTotalHealth();
			m_currentHealth = 0f;
			m_currentTemperatureDamage = 0f;
			m_currentOxygenDamage = 0f;
			m_healthRedCounter = 0f;
			if (!m_isCountingDown)
			{
				if (this.OnCountdownBegin != null)
				{
					this.OnCountdownBegin();
				}
				m_isCountingDown = true;
				m_currentHealthCountdown = m_standardCountdownDuration;
			}
		}
		if (m_isCountingDown)
		{
			if (!m_healedThisFrame)
			{
				m_currentHealthCountdown -= Time.deltaTime;
			}
			if ((m_currentHealthCountdown < 0f || m_overDamageIncapacitated > 300f) && !m_isDead)
			{
				if (this.OnDead != null)
				{
					this.OnDead();
				}
				m_isDead = true;
			}
		}
		else
		{
			float num = m_temperature + m_temperatureResist;
			if (num < 0f)
			{
				float num2 = Mathf.Clamp(0f - num, m_temperatureBelowZeroGate, m_temperatureBelowZeroMax);
				float num3 = num2 * m_temperatureDamagePerSecondPerDegreeUnderZero;
				m_temperatureAccumulator += num3 * Time.deltaTime;
				if (m_temperatureAccumulator > m_minTemperatureDamage)
				{
					m_currentTemperatureDamage += m_temperatureAccumulator;
					m_temperatureAccumulator = 0f;
				}
			}
			else if (m_currentTemperatureDamage > 0f)
			{
				m_currentTemperatureDamage -= Time.deltaTime * m_temperatureRecoverySpeed;
				if (m_currentTemperatureDamage < 0f)
				{
					m_currentTemperatureDamage = 0f;
				}
			}
			if (!m_hasOxygen)
			{
				m_oxygenCountupTimer += Time.deltaTime;
				if (m_oxygenCountupTimer > (float)m_oxygenSeconds)
				{
					m_oxygenAccumulator += Time.deltaTime * m_noOxygenDamagePerSecond;
					if (m_oxygenAccumulator > m_minOxygenDamage)
					{
						m_currentOxygenDamage += m_oxygenAccumulator;
						m_oxygenAccumulator = 0f;
					}
				}
			}
			else
			{
				if (m_currentOxygenDamage > 0f)
				{
					m_currentOxygenDamage -= Time.deltaTime * m_oxygenRecoverySpeed;
					if (m_currentOxygenDamage < 0f)
					{
						m_currentOxygenDamage = 0f;
					}
				}
				if (m_oxygenCountupTimer > (float)m_oxygenSeconds)
				{
					m_oxygenCountupTimer = m_oxygenSeconds;
				}
				m_oxygenCountupTimer -= Time.deltaTime * m_oxygenRecoverySpeed;
				if (m_oxygenCountupTimer < 0f)
				{
					m_oxygenCountupTimer = 0f;
				}
			}
		}
		if (m_healthRedCounter > 0f)
		{
			m_healthRedCounter -= Time.deltaTime * 20f;
			if (m_healthRedCounter < 0f)
			{
				m_healthRedCounter = 0f;
			}
		}
		m_healedThisFrame = false;
	}
}
