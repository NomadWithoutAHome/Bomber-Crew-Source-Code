using AudioNames;
using BomberCrewCommon;
using dbox;
using UnityEngine;
using WingroveAudio;

public class Engine : SmoothDamageableRepairable, FireOverview.Extinguishable
{
	[SerializeField]
	private GameObject m_engineFireFX;

	[SerializeField]
	private GameObject m_engineMesh;

	[SerializeField]
	private MeshCollider m_damageMesh;

	[SerializeField]
	private float m_healthInitial;

	[SerializeField]
	private float m_fireChanceMax;

	[SerializeField]
	private float m_fireDamageRate;

	[SerializeField]
	private GameObject m_propFastSpin;

	[SerializeField]
	private GameObject m_propSlowSpin;

	[SerializeField]
	private Transform m_propSlowSpinTransform;

	[SerializeField]
	private GameObject m_propDamaged;

	[SerializeField]
	private float m_spinSpeedFull = 2850f;

	[SerializeField]
	private float m_fireReduceRate = 0.03f;

	[SerializeField]
	private DamageFlash m_damageFlash;

	[SerializeField]
	private float m_fireSpreadRate = 0.05f;

	[SerializeField]
	private DamageEffects m_damageEffects;

	[SerializeField]
	private Transform m_putOutFireTransform;

	[SerializeField]
	private GameObject m_EngineStartEffect;

	private float m_propSpinMultiplier;

	private bool m_isOnFire;

	private bool m_isDestroyed;

	private float m_fireAmount;

	private float m_fireCountdown;

	private GameObject m_fireFXInstance;

	private BomberSystems m_bomberSystems;

	private BomberState m_bomberState;

	private bool m_switchedOnTarget;

	private bool m_currentlyActive;

	private float m_activeCountdown;

	private float m_currentHealth;

	private DamageSource m_fireDamageSource;

	private bool m_isAttached = true;

	private bool m_invincible;

	private int m_cachedGameObjectId;

	private void Awake()
	{
		m_cachedGameObjectId = base.gameObject.GetInstanceID();
	}

	public Transform GetPutOutPositionOverride()
	{
		return m_putOutFireTransform;
	}

	private void Start()
	{
		m_EngineStartEffect.SetActive(value: false);
		m_propSlowSpinTransform.localEulerAngles = new Vector3(0f, 0f, Random.Range(0f, 90f));
		m_fireDamageSource = new DamageSource();
		m_fireDamageSource.m_damageType = DamageSource.DamageType.Fire;
		m_fireDamageSource.m_damageShapeEffect = DamageSource.DamageShape.None;
		m_bomberSystems = Singleton<BomberSpawn>.Instance.GetBomberSystems();
		m_bomberSystems.RegisterEngine(this);
		m_bomberState = m_bomberSystems.GetBomberState();
		m_currentHealth = m_healthInitial;
		BomberDestroyableSection.FindDestroyableSectionFor(base.transform).OnSectionDestroy += OnRemovedFromBomber;
	}

	private void OnRemovedFromBomber()
	{
		WingroveRoot.Instance.PostEventGO("ENGINE_STOP", base.gameObject);
		SwitchOff();
		m_bomberSystems.RemoveEngine(this);
		m_damageFlash.ReturnToNormal();
		m_isAttached = false;
	}

	private void ShowDebug()
	{
		if (!GUILayout.Button("Damage " + base.name))
		{
		}
	}

	public void SwitchOff()
	{
		StopCoroutine("DoSwitchOn");
		m_EngineStartEffect.SetActive(value: false);
		m_switchedOnTarget = false;
	}

	public void SwitchOn()
	{
		m_switchedOnTarget = true;
	}

	private void IgnitionEffect()
	{
		m_EngineStartEffect.SetActive(value: false);
		m_EngineStartEffect.SetActive(value: true);
		WingroveRoot.Instance.PostEventGO("ENGINE_IGNITION", base.gameObject);
		Singleton<ControllerRumble>.Instance.GetRumbleMixerShockLong().Kick();
	}

	public float GetCapability()
	{
		if (m_bomberSystems.GetFuelTank(0).GetFuelNormalised() == 0f && m_bomberSystems.GetFuelTank(1).GetFuelNormalised() == 0f)
		{
			return 0f;
		}
		if (m_isDestroyed)
		{
			return 0f;
		}
		if (m_isOnFire)
		{
			return 0.5f;
		}
		return 1f;
	}

	public bool IsOn()
	{
		return m_activeCountdown > 0f;
	}

	public bool IsDestroyed()
	{
		return m_isDestroyed;
	}

	public float GetFireIntensityNormalised()
	{
		if (m_isOnFire)
		{
			return m_fireAmount;
		}
		return 0f;
	}

	public void SetInvincible(bool invincible)
	{
		m_invincible = invincible;
	}

	public void UndoDamage(float amt)
	{
		m_currentHealth += amt * m_healthInitial;
		if (m_currentHealth > m_healthInitial)
		{
			m_currentHealth = m_healthInitial;
		}
	}

	private void DamageUpdate(float amount, bool isIncendiary)
	{
		if (!m_invincible)
		{
			m_currentHealth -= amount;
		}
		if (m_isDestroyed || !m_isAttached)
		{
			return;
		}
		if (m_currentHealth <= 0f)
		{
			m_currentHealth = 0f;
			m_damageFlash.DoDestroyed();
			m_isDestroyed = true;
			int num = 0;
			BomberSystems bomberSystems = Singleton<BomberSpawn>.Instance.GetBomberSystems();
			for (int i = 0; i < bomberSystems.GetEngineCount(); i++)
			{
				Engine engine = bomberSystems.GetEngine(i);
				if (engine != null && !engine.IsDestroyed())
				{
					num++;
				}
			}
			DboxInMissionController.DBoxCall(DboxSdkWrapper.PostPlaneFailure);
			switch (num)
			{
			case 1:
				Singleton<CrewMiscSpeechTriggers>.Instance.DoExternalTrigger(CrewMiscSpeechTriggers.SpeechExternalTrigger.EnginesRemainOne, null);
				break;
			case 2:
				Singleton<CrewMiscSpeechTriggers>.Instance.DoExternalTrigger(CrewMiscSpeechTriggers.SpeechExternalTrigger.EnginesRemainTwo, null);
				break;
			case 0:
				Singleton<CrewMiscSpeechTriggers>.Instance.DoExternalTrigger(CrewMiscSpeechTriggers.SpeechExternalTrigger.EnginesRemainNone, null);
				break;
			}
		}
		else
		{
			float num2 = m_fireChanceMax * (1f - GetHealthNormalised());
			if (isIncendiary)
			{
				num2 *= 2f;
			}
			if (!m_isOnFire && Random.Range(0f, 1f) < num2)
			{
				Singleton<ControllerRumble>.Instance.GetRumbleMixerShockLong().Kick();
				Singleton<ControllerRumble>.Instance.GetRumbleMixerShock().Kick();
				m_isOnFire = true;
				m_fireFXInstance = Object.Instantiate(m_engineFireFX);
				m_fireFXInstance.transform.parent = base.transform;
				m_fireFXInstance.transform.localScale = Vector3.one;
				m_fireFXInstance.transform.localPosition = Vector3.zero;
				m_fireFXInstance.transform.localEulerAngles = Vector3.zero;
				m_fireAmount = 0.1f;
				SetLayerRecursively(m_fireFXInstance, m_engineMesh.layer);
			}
		}
	}

	public void Damage(float amount, bool isIncendiary)
	{
		m_damageFlash.DoFlash();
		WingroveRoot.Instance.PostEventGO("IMPACT_HEAVY", base.gameObject);
		if (!m_isDestroyed)
		{
			DamageUpdate(amount, isIncendiary);
		}
	}

	private void SetLayerRecursively(GameObject obj, int newLayer)
	{
		if (obj == null)
		{
			return;
		}
		obj.layer = newLayer;
		foreach (Transform item in obj.transform)
		{
			if (!(item == null))
			{
				SetLayerRecursively(item.gameObject, newLayer);
			}
		}
	}

	private void Update()
	{
		if (m_isAttached)
		{
			if (m_isOnFire || GetHealthNormalised() < 0.5f)
			{
				m_damageFlash.DoLowHealth(GetHealthNormalised() < 0.3f);
			}
			else if (!m_isDestroyed)
			{
				m_damageFlash.ReturnToNormal();
			}
		}
		if (m_isOnFire)
		{
			if (!m_isDestroyed)
			{
				m_fireAmount += 0.1f * Time.deltaTime;
				DamageUpdate(Time.deltaTime * m_fireAmount * m_fireDamageRate, isIncendiary: false);
				if (OnDamage != null)
				{
					OnDamage(m_fireDamageSource, Time.deltaTime * m_fireAmount * m_fireDamageRate);
				}
			}
			else
			{
				m_fireAmount -= 0.1f * Time.deltaTime;
			}
			float num = Mathf.Clamp(m_bomberState.GetAltitudeNormalised(), 0.75f, 1f) - 0.25f;
			float num2 = Mathf.Max(0f - m_bomberState.GetPhysicsModel().GetVelocity().y, 0f);
			float num3 = ((!m_bomberState.GetEmergencyDiveTimer().IsActive()) ? 0.3f : 1f);
			float num4 = num * num2 * num3;
			m_fireAmount -= m_fireReduceRate * Time.deltaTime * num4;
			if (m_fireAmount < 0f)
			{
				m_isOnFire = false;
				m_fireFXInstance.GetComponent<EffectScaler>().DestroyIn(5f);
				m_fireFXInstance = null;
			}
			m_fireAmount = Mathf.Clamp01(m_fireAmount);
			if (m_fireFXInstance != null)
			{
				m_fireFXInstance.GetComponent<EffectScaler>().SetScale(Mathf.Clamp01(m_fireAmount));
			}
		}
		bool flag = m_switchedOnTarget;
		if (m_bomberSystems.GetFuelTank(0).GetFuelNormalised() == 0f && m_bomberSystems.GetFuelTank(1).GetFuelNormalised() == 0f)
		{
			flag = false;
		}
		if (m_isDestroyed)
		{
			flag = false;
		}
		if (flag != m_currentlyActive)
		{
			m_currentlyActive = flag;
			if (m_currentlyActive)
			{
				IgnitionEffect();
				WingroveRoot.Instance.PostEventGO("ENGINE_START", base.gameObject);
			}
			else
			{
				WingroveRoot.Instance.PostEventGO("ENGINE_STOP", base.gameObject);
			}
		}
		if (m_currentlyActive)
		{
			m_activeCountdown = 4f;
		}
		else
		{
			m_activeCountdown -= Time.deltaTime;
		}
		float num5 = ((!m_isAttached) ? 0f : Mathf.Clamp(m_bomberState.GetPhysicsModel().GetVelocity().magnitude / 10f, 0f, 0.1f));
		if (!m_isDestroyed && m_currentlyActive)
		{
			m_propSpinMultiplier += Time.deltaTime * 0.5f;
		}
		else
		{
			m_propSpinMultiplier -= Time.deltaTime * 0.5f;
		}
		m_propSpinMultiplier = Mathf.Clamp(m_propSpinMultiplier, num5, 1f);
		if (m_propSlowSpinTransform != null)
		{
			m_propSlowSpinTransform.Rotate(new Vector3(0f, 0f, m_spinSpeedFull * Time.deltaTime * m_propSpinMultiplier));
			while (m_propSlowSpinTransform.localEulerAngles.z > 360f)
			{
				m_propSlowSpinTransform.localEulerAngles = new Vector3(m_propSlowSpinTransform.localEulerAngles.x, m_propSlowSpinTransform.localEulerAngles.y, m_propSlowSpinTransform.localEulerAngles.z - 360f);
			}
		}
		if (m_propFastSpin != null)
		{
			m_propFastSpin.SetActive(m_propSpinMultiplier > 0.8f);
		}
		if (m_propSlowSpin != null)
		{
			m_propSlowSpin.SetActive(m_propSpinMultiplier < 1f);
		}
		WingroveRoot.Instance.SetParameterForObject(GameEvents.Parameters.CacheVal_EngineIndividualSpeed(), m_cachedGameObjectId, base.gameObject, (m_propSpinMultiplier - num5) / (1f - num5));
	}

	public override bool IsDamageBlocker()
	{
		return false;
	}

	public override float DamageGetPassthrough(float amt, DamageSource damageSource)
	{
		if (OnDamage != null && !m_isDestroyed)
		{
			OnDamage(damageSource, amt);
		}
		if (!m_isDestroyed)
		{
			Singleton<DboxInMissionController>.Instance.DoDamageEffect(damageSource.m_position, amt, Singleton<BomberSpawn>.Instance.GetBomberSystems().transform);
		}
		Damage(amt, damageSource.m_incendiaryEffect);
		m_damageEffects.DoDamageEffect(m_damageMesh, damageSource, amt);
		return 0f;
	}

	private void OnDestroy()
	{
		if (WingroveRoot.Instance != null && base.gameObject != null)
		{
			WingroveRoot.Instance.PostEventGO("ENGINE_STOP", base.gameObject);
		}
	}

	public string GetIdentifier()
	{
		return base.transform.parent.parent.name + "_" + base.transform.parent.name + "_" + base.gameObject.name + "_FuselageSection";
	}

	public override float GetHealthNormalised()
	{
		return m_currentHealth / m_healthInitial;
	}

	public override bool CanBeRepaired()
	{
		return NeedsRepair() && !m_isOnFire;
	}

	public override bool NeedsRepair()
	{
		return m_isDestroyed;
	}

	public override bool IsBroken()
	{
		return m_isDestroyed;
	}

	public override bool IsUnreliable()
	{
		return m_isOnFire;
	}

	public override void Repair()
	{
		m_isDestroyed = false;
		m_currentHealth = m_healthInitial;
		AchievementSystem.Achievement achievement = Singleton<AchievementSystem>.Instance.GetAchievement("WingEngineRepair");
		if (achievement != null)
		{
			Singleton<AchievementSystem>.Instance.AwardAchievement(achievement);
		}
	}

	public bool Exists()
	{
		return m_isAttached;
	}

	public bool IsOnFire()
	{
		return m_isOnFire;
	}

	public void PutOutFire(float amt)
	{
		m_fireAmount -= amt;
		if (m_fireAmount < 0f && m_isOnFire)
		{
			m_fireAmount = 0f;
			m_isOnFire = false;
			if (m_fireFXInstance != null)
			{
				m_fireFXInstance.GetComponent<EffectScaler>().DestroyIn(5f);
				m_fireFXInstance = null;
			}
		}
	}

	public Transform GetTransform()
	{
		return base.transform;
	}

	public bool SearchableForNext()
	{
		return false;
	}

	public InteractiveItem NextInteractive()
	{
		return GetComponent<RepairableInteractive>();
	}
}
