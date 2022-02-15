using BomberCrewCommon;
using UnityEngine;

public class FuelTank : SmoothDamageableRepairable
{
	[SerializeField]
	private float m_capacityMax;

	[SerializeField]
	private float m_capacityFill;

	[SerializeField]
	private float m_rate = 0.1f;

	[SerializeField]
	private FlashManager m_flashManager;

	[SerializeField]
	private float m_leakRate;

	[SerializeField]
	private DamageFlash m_damageFlash;

	[SerializeField]
	private BomberSystemUniqueId m_upgradeSystem;

	[SerializeField]
	private GameObject m_fuelLeakEffectPrefab;

	private float m_fuelLevel;

	private bool m_isLeaking;

	private float m_initialHealth;

	private float m_health;

	private FlashManager.ActiveFlash m_lowHealthFlash;

	private GameObject m_fuelLeakEffectInstance;

	private bool m_dontUseFuel;

	public void SetDontUse(bool dontuse)
	{
		m_dontUseFuel = dontuse;
	}

	private void Start()
	{
		float num = 1f + Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetCurrentlySelectedMissionDetails().m_fuelBonus;
		m_fuelLevel = m_capacityFill * num;
		m_initialHealth = (m_health = m_upgradeSystem.GetUpgrade().GetArmour());
	}

	private void ShowDebug()
	{
		if (GUILayout.Button("Empty Fuel Tank " + base.name))
		{
			m_fuelLevel = 0f;
		}
	}

	public bool IsEmpty()
	{
		return m_fuelLevel == 0f;
	}

	private void ShowFuelLeak(bool show)
	{
		if (show)
		{
			if (m_fuelLeakEffectInstance == null)
			{
				m_fuelLeakEffectInstance = Object.Instantiate(m_fuelLeakEffectPrefab);
				m_fuelLeakEffectInstance.transform.parent = base.transform;
				m_fuelLeakEffectInstance.transform.localPosition = Vector3.zero;
				m_fuelLeakEffectInstance.transform.localRotation = Quaternion.identity;
			}
		}
		else if (m_fuelLeakEffectInstance != null)
		{
			Object.Destroy(m_fuelLeakEffectInstance);
		}
	}

	public float GetFuelNormalised()
	{
		float result = 0f;
		if (m_fuelLevel > 0f)
		{
			result = m_fuelLevel / m_capacityMax;
		}
		return result;
	}

	public float GetFuel()
	{
		return m_fuelLevel;
	}

	public float UseFuel(float amt)
	{
		float result = 0f;
		if (m_dontUseFuel)
		{
			result = 0f;
		}
		else
		{
			m_fuelLevel -= amt * m_rate;
			if (m_fuelLevel < 0f)
			{
				result = (0f - m_fuelLevel) / m_rate;
				m_fuelLevel = 0f;
			}
		}
		return result;
	}

	public float PumpFuelIn(float amt)
	{
		float result = 0f;
		m_fuelLevel += amt * m_rate;
		if (m_fuelLevel > m_capacityMax)
		{
			result = (m_fuelLevel - m_capacityMax) / m_rate;
			m_fuelLevel = m_capacityMax;
		}
		return result;
	}

	private void Update()
	{
		if (m_isLeaking && !m_dontUseFuel)
		{
			m_fuelLevel -= Time.deltaTime * m_leakRate;
			if (m_fuelLevel <= 0f)
			{
				ShowFuelLeak(show: false);
				m_fuelLevel = 0f;
			}
			else
			{
				ShowFuelLeak(show: true);
			}
		}
		else
		{
			ShowFuelLeak(show: false);
		}
	}

	public bool IsLeaking()
	{
		return m_isLeaking;
	}

	public override bool IsDamageBlocker()
	{
		return false;
	}

	public override float DamageGetPassthrough(float amt, DamageSource damageSource)
	{
		if (!m_isLeaking)
		{
			if (OnDamage != null)
			{
				OnDamage(damageSource, amt);
			}
			m_health -= amt;
			if (m_health <= 0f)
			{
				m_health = 0f;
				m_damageFlash.DoDestroyed();
				m_isLeaking = true;
				Singleton<CrewMiscSpeechTriggers>.Instance.DoExternalTrigger(CrewMiscSpeechTriggers.SpeechExternalTrigger.FuelTankLeak, null);
			}
			else if (m_health < m_initialHealth * 0.25f)
			{
				m_damageFlash.DoLowHealth(critical: true);
			}
			else if (m_health < m_initialHealth * 0.4f)
			{
				m_damageFlash.DoLowHealth(critical: false);
			}
			m_damageFlash.DoFlash();
		}
		return 0f;
	}

	public override bool CanBeRepaired()
	{
		return m_isLeaking;
	}

	public override bool NeedsRepair()
	{
		return m_isLeaking;
	}

	public override bool IsUnreliable()
	{
		return m_isLeaking;
	}

	public override bool IsBroken()
	{
		return m_isLeaking;
	}

	public override void Repair()
	{
		m_isLeaking = false;
		m_health = m_initialHealth;
		m_damageFlash.ReturnToNormal();
	}

	public override float GetHealthNormalised()
	{
		return Mathf.Clamp01(m_health / m_initialHealth);
	}
}
