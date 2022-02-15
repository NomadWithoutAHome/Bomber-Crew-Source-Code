using UnityEngine;
using WingroveAudio;

public class HydraulicsTank : SmoothDamageableRepairable
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
	private GameObject m_damageEffects;

	[SerializeField]
	private Animation m_damageAnimation;

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

	private EffectScaler m_hydraulicEffectScalar;

	private float m_hydraulicEffect;

	private float m_unreliableEffectTimer;

	private void Start()
	{
		m_initialHealth = (m_health = m_systemSpawner.GetUpgrade().GetArmour());
		m_reliability = m_systemSpawner.GetUpgrade().GetReliability();
		SpawnSparks();
		m_hydraulicEffectScalar.SetScale(0f);
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
				m_damageFlash.DoDestroyed();
				WingroveRoot.Instance.PostEvent("HYDRAULIC_SHUT_DOWN");
				SpawnSparks();
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

	private void SpawnSparks()
	{
		if (m_sparksInstance == null)
		{
			m_sparksInstance = Object.Instantiate(m_damageEffects);
			m_sparksInstance.transform.parent = base.transform;
			m_sparksInstance.transform.localPosition = Vector3.zero;
			m_sparksInstance.transform.localRotation = Quaternion.identity;
			m_sparksInstance.transform.localScale = Vector3.one;
			m_hydraulicEffectScalar = m_sparksInstance.GetComponent<EffectScaler>();
		}
	}

	private void Update()
	{
		if (m_invincible)
		{
			m_hydraulicEffectScalar.SetScale(0f);
		}
		else if (!m_isBroken)
		{
			if (!m_isUnreliable)
			{
				m_hydraulicEffectScalar.SetScale(0f);
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
					WingroveRoot.Instance.PostEventGO("HYDRAULIC_BROKEN_RANDOM", base.gameObject);
					m_damageAnimation.Play();
					m_hydraulicEffect = 1f;
					m_unreliableEffectTimer = Random.Range(1.5f, 3f);
				}
			}
			else
			{
				if (!m_isUnreliable)
				{
					return;
				}
				m_hydraulicEffect -= Time.deltaTime;
				if (m_hydraulicEffect < 0f)
				{
					m_hydraulicEffect = 0f;
				}
				m_unreliableEffectTimer -= Time.deltaTime;
				if (m_unreliableEffectTimer < 0f && !m_isBeingRepaired)
				{
					m_unreliableEffectTimer = Random.Range(1.5f, 3f);
					if (Time.deltaTime > 0f)
					{
						m_damageAnimation.Play();
						WingroveRoot.Instance.PostEventGO("HYDRAULIC_BROKEN_RANDOM", base.gameObject);
						m_hydraulicEffect = 1f;
					}
				}
				m_hydraulicEffectScalar.SetScale(m_hydraulicEffect);
				if (!m_isBeingRepaired)
				{
					m_unreliableTimer -= Time.deltaTime;
					if (m_unreliableTimer < 0f)
					{
						m_damageFlash.DoLowHealth(m_health < 0.1f);
						WingroveRoot.Instance.PostEvent("HYDRAULIC_SHUT_DOWN");
						m_damageAnimation.Play();
						m_isBroken = true;
						m_isUnreliable = false;
					}
				}
			}
		}
		else
		{
			m_hydraulicEffectScalar.SetScale(1f);
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
		WingroveRoot.Instance.PostEvent("HYDRAULIC_REPAIR");
	}

	public void SetInvincible()
	{
		m_isBroken = false;
		m_isUnreliable = false;
		m_reliabilityTimer = 0f;
		m_invincible = true;
		m_health = m_initialHealth;
		m_isBeingRepaired = false;
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
