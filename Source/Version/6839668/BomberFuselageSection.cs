using BomberCrewCommon;
using UnityEngine;

public class BomberFuselageSection : SmoothDamageable
{
	private bool m_testDamage = true;

	[SerializeField]
	private LayerMask m_damagePassThroughLayerMask;

	[SerializeField]
	private float m_healthPerHole;

	[SerializeField]
	private int m_maxHolesAtOnce = 3;

	[SerializeField]
	private Damageable[] m_directDamageSystems;

	[SerializeField]
	private DamagePassthroughModifier m_damagePassthroughModifier;

	[SerializeField]
	private DamageableMeshSharedMask m_damageableMesh;

	[SerializeField]
	private MeshCollider m_collider;

	[SerializeField]
	private float m_fireChanceMaxPerDamageUnit = 0.02f;

	[SerializeField]
	private FireArea[] m_fireAreas;

	[SerializeField]
	private float m_maxDamageability = 0.75f;

	[SerializeField]
	private float m_reachMaxDamageabilityAt = 0.5f;

	[SerializeField]
	private DamageEffects m_damageEffects;

	[SerializeField]
	private BomberFuselageSectionGroup m_fuselageSectionGroup;

	[SerializeField]
	private bool m_useSharedHealth;

	[SerializeField]
	private float m_nonSharedHealthFactor;

	[SerializeField]
	private bool m_absolutePassthrough;

	private float m_impactDamageCounter;

	private Vector3 m_lastDamageHole;

	private float m_currentHealth;

	private float m_initialHealth = 300f;

	private bool m_invincible;

	private bool m_destroyed;

	private BomberSystems m_bomberSystems;

	private void Start()
	{
		m_damageableMesh.RegisterMesh(m_collider, m_collider.sharedMesh);
		if (!m_useSharedHealth)
		{
			m_initialHealth = (m_currentHealth = m_fuselageSectionGroup.GetMaxHealth() * m_nonSharedHealthFactor);
		}
		else
		{
			m_initialHealth = m_fuselageSectionGroup.GetMaxHealth();
		}
		BomberDestroyableSection bomberDestroyableSection = BomberDestroyableSection.FindDestroyableSectionFor(base.transform);
		if (bomberDestroyableSection != null)
		{
			bomberDestroyableSection.OnSectionDestroy += OnSectionDestroy;
		}
		m_bomberSystems = Singleton<BomberSpawn>.Instance.GetBomberSystems();
	}

	private void OnSectionDestroy()
	{
		m_destroyed = true;
	}

	private float ThisHealth()
	{
		if (m_useSharedHealth)
		{
			return m_fuselageSectionGroup.GetHealth();
		}
		return m_currentHealth;
	}

	private void ModifyHealth(float amt)
	{
		if (m_useSharedHealth)
		{
			m_fuselageSectionGroup.ChangeHealth(amt);
			return;
		}
		float currentHealth = m_currentHealth;
		m_currentHealth += amt;
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

	public void UndoDamage(float amtN)
	{
		if (m_destroyed)
		{
			return;
		}
		if (m_useSharedHealth)
		{
			m_fuselageSectionGroup.UndoDamage(amtN);
			return;
		}
		m_currentHealth += amtN * m_initialHealth;
		if (m_currentHealth > m_initialHealth)
		{
			m_currentHealth = m_initialHealth;
		}
	}

	public float GetHealth()
	{
		return ThisHealth();
	}

	public override float GetHealthNormalised()
	{
		return Mathf.Clamp01(GetHealth() / GetMaxHealth());
	}

	public float GetMaxHealth()
	{
		return m_initialHealth;
	}

	public float GetDamagability()
	{
		float num = 1f - ThisHealth() / m_initialHealth;
		float num2 = ((!(m_damagePassthroughModifier == null)) ? (Mathf.Clamp01(num / m_reachMaxDamageabilityAt) * Mathf.Clamp01(m_damagePassthroughModifier.GetDamagePassthroughModifier())) : Mathf.Clamp01(num / m_reachMaxDamageabilityAt));
		return num2 * m_maxDamageability;
	}

	public override bool IsDamageBlocker()
	{
		return true;
	}

	public Collider GetCollider()
	{
		return m_collider;
	}

	public Vector3 GetRandomPositionOnCollider()
	{
		Vector3 position = base.transform.position + Random.onUnitSphere * 30f;
		return m_collider.ClosestPointOnBounds(position);
	}

	public void SetInvincible(bool invincible)
	{
		m_invincible = invincible;
	}

	public override float DamageGetPassthrough(float amt, DamageSource ds)
	{
		if (ds.m_damageType != DamageSource.DamageType.Fire && OnDamage != null)
		{
			OnDamage(ds, amt);
		}
		if (ds.m_damageType != DamageSource.DamageType.Fire)
		{
			Singleton<DboxInMissionController>.Instance.DoDamageEffect(ds.m_position, amt, Singleton<BomberSpawn>.Instance.GetBomberSystems().transform);
		}
		m_damageEffects.DoDamageEffect(m_collider, ds, amt);
		float num = ThisHealth() / m_initialHealth;
		float num2 = Mathf.Max(ThisHealth() - amt, 0f) / m_initialHealth;
		if (ds.m_incendiaryEffect)
		{
			float num3 = Random.Range(0f, 1f);
			if (num3 < GetDamagability() * 0.33f)
			{
				m_bomberSystems.GetFireOverview().StartFire(ds.m_position);
			}
		}
		if (ds.m_damageType == DamageSource.DamageType.Fire)
		{
			amt = 0f;
		}
		if (!m_invincible || ds.m_damageType == DamageSource.DamageType.GroundImpact)
		{
			ModifyHealth(0f - amt);
		}
		if (m_absolutePassthrough)
		{
			return (GetDamagability() != 1f) ? 0f : 1f;
		}
		return Mathf.Clamp01(GetDamagability());
	}
}
