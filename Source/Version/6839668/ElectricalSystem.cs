using UnityEngine;
using WingroveAudio;

public class ElectricalSystem : SmoothDamageableRepairable
{
	[SerializeField]
	private FlashManager m_flashManager;

	[SerializeField]
	private BomberSystemUniqueId m_systemSpawner;

	[SerializeField]
	private DamageFlash m_damageFlash;

	[SerializeField]
	private float m_unreliabilityCountdown = 30f;

	[SerializeField]
	private ParticleSystem m_sparkEffects;

	[SerializeField]
	private FireArea m_fireArea;

	[SerializeField]
	private float m_chanceOfFireStart;

	[SerializeField]
	private float m_minTimeBeforeFireStart;

	[SerializeField]
	private float m_checkFireChanceAgain;

	private bool m_isBroken;

	private bool m_isUnreliable;

	private bool m_isBeingRepaired;

	private float m_health;

	private float m_initialHealth;

	private float m_reliability;

	private float m_reliabilityRVal;

	private float m_reliabilityTimer;

	private float m_unreliableTimer;

	private GameObject m_sparksInstance;

	private bool m_invincible;

	private float m_fireCountdown;

	private float m_sparkEffectTimer;

	private void Start()
	{
		m_initialHealth = (m_health = m_systemSpawner.GetUpgrade().GetArmour());
		m_reliability = m_systemSpawner.GetUpgrade().GetReliability();
	}

	private void OnDestroy()
	{
	}

	public override bool IsDamageBlocker()
	{
		return false;
	}

	public override float DamageGetPassthrough(float amt, DamageSource ds)
	{
		if (!m_invincible)
		{
			m_health -= amt;
		}
		if (m_health < 0f)
		{
			m_health = 0f;
			if (!m_isBroken)
			{
				m_fireCountdown = m_minTimeBeforeFireStart;
				m_damageFlash.DoDestroyed();
				WingroveRoot.Instance.PostEvent("ELECTRICAL_SYSTEM_SHUTDOWN");
			}
			m_isBroken = true;
		}
		else if (m_health < 0.1f)
		{
			m_damageFlash.DoLowHealth(critical: true);
		}
		else if (m_health < 0.3f)
		{
			m_damageFlash.DoLowHealth(critical: false);
		}
		m_damageFlash.DoFlash();
		return 0f;
	}

	public override bool IsBroken()
	{
		return m_isBroken;
	}

	public override bool IsUnreliable()
	{
		return m_isUnreliable;
	}

	public override bool CanBeRepaired()
	{
		return m_isBroken || m_isUnreliable;
	}

	public override bool NeedsRepair()
	{
		return m_isBroken || m_isUnreliable;
	}

	private void Update()
	{
		if (m_invincible)
		{
			return;
		}
		if (!m_isBroken)
		{
			if (!m_isUnreliable)
			{
				m_reliabilityTimer -= Time.deltaTime;
				if (m_reliabilityTimer < 0f)
				{
					m_reliabilityTimer = 600f;
					m_reliabilityRVal = Random.Range(0f, 1f);
				}
				float num = 1f - m_reliability;
				float num2 = (600f - m_reliabilityTimer) / 600f * num;
				if (num2 > m_reliabilityRVal)
				{
					m_isUnreliable = true;
					m_unreliableTimer = m_unreliabilityCountdown;
					WingroveRoot.Instance.PostEventGO("ELECTRICAL_SPARK", base.gameObject);
					m_sparkEffects.Emit(Random.Range(6, 16));
					m_sparkEffectTimer = Random.Range(1.5f, 3f);
				}
			}
			else
			{
				if (!m_isUnreliable)
				{
					return;
				}
				m_sparkEffectTimer -= Time.deltaTime;
				if (m_sparkEffectTimer < 0f && !m_isBeingRepaired)
				{
					m_sparkEffectTimer = Random.Range(1.5f, 3f);
					if (Time.deltaTime > 0f)
					{
						m_sparkEffects.Emit(Random.Range(6, 16));
						WingroveRoot.Instance.PostEventGO("ELECTRICAL_SPARK", base.gameObject);
					}
				}
				if (!m_isBeingRepaired)
				{
					m_unreliableTimer -= Time.deltaTime;
					if (m_unreliableTimer < 0f)
					{
						m_damageFlash.DoLowHealth(m_health < 0.1f);
						WingroveRoot.Instance.PostEvent("ELECTRICAL_SYSTEM_SHUTDOWN");
						m_fireCountdown = m_minTimeBeforeFireStart;
						m_isBroken = true;
						m_isUnreliable = false;
					}
				}
			}
		}
		else if (!m_isBeingRepaired)
		{
			m_sparkEffectTimer -= Time.deltaTime;
			if (m_sparkEffectTimer < 0f)
			{
				m_sparkEffectTimer = Random.Range(0.5f, 1.25f);
				m_sparkEffects.Emit(Random.Range(6, 16));
			}
			m_fireCountdown -= Time.deltaTime;
			if (m_fireCountdown < 0f)
			{
				if (Random.Range(0f, 1f) < m_chanceOfFireStart)
				{
					m_fireArea.StartFire(1);
				}
				else
				{
					m_fireCountdown = m_checkFireChanceAgain;
				}
			}
		}
		else
		{
			m_fireCountdown += Time.deltaTime;
			if (m_fireCountdown > m_minTimeBeforeFireStart)
			{
				m_fireCountdown = m_minTimeBeforeFireStart;
			}
		}
	}

	public override void Repair()
	{
		m_damageFlash.ReturnToNormal();
		m_isBroken = false;
		m_isUnreliable = false;
		m_reliabilityTimer = 0f;
		m_health = m_initialHealth;
		m_isBeingRepaired = false;
		WingroveRoot.Instance.PostEvent("ELECTRICAL_SYSTEM_REPAIRED");
		if (m_sparksInstance != null)
		{
			Object.Destroy(m_sparksInstance);
		}
	}

	public void SetInvincible()
	{
		m_isBroken = false;
		m_isUnreliable = false;
		m_reliabilityTimer = 0f;
		m_invincible = true;
		m_health = m_initialHealth;
		m_isBeingRepaired = false;
		if (m_sparksInstance != null)
		{
			Object.Destroy(m_sparksInstance);
		}
	}

	public void SetNoLongerInvincible()
	{
		m_invincible = false;
	}

	public override void AbandonRepair()
	{
		m_isBeingRepaired = false;
	}

	public override void StartRepair()
	{
		m_isBeingRepaired = true;
	}

	public override float GetHealthNormalised()
	{
		return m_health / m_initialHealth;
	}
}
