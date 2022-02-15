using System;
using BomberCrewCommon;
using dbox;
using UnityEngine;

public class FighterPlane : Damageable
{
	public enum SpeechCategory
	{
		FighterNormal,
		EnemyAce,
		V1Rocket,
		V2Rocket,
		HomingRocket
	}

	[SerializeField]
	private float m_startingHealth;

	[SerializeField]
	private RaycastGunTurret[] m_forwardFacingGuns;

	[SerializeField]
	private GameObject m_explodeEffect;

	[SerializeField]
	private FighterDynamics m_dynamics;

	[SerializeField]
	private EffectScaler m_damageEffectScaler;

	[SerializeField]
	private TaggableItem m_taggableItem;

	[SerializeField]
	private FighterAI m_fighterAi;

	[SerializeField]
	private int m_xpReward;

	[SerializeField]
	private bool m_useDifficultySettings;

	[SerializeField]
	private float m_explodeChance;

	[SerializeField]
	private bool m_shouldHunt;

	[SerializeField]
	private RaycastGunTurret m_referenceRGT;

	[SerializeField]
	private bool m_dontCountTowardsAchievementStats;

	[SerializeField]
	private bool m_isV1Rocket;

	[SerializeField]
	private Transform m_aimAt;

	[SerializeField]
	private string m_triggerOnDestroy;

	[SerializeField]
	private float m_blendDifficultySettingsHealth;

	[SerializeField]
	private float m_blendDifficultySettingsDamage;

	[SerializeField]
	private float m_blendDifficultySettingsPhase;

	[SerializeField]
	private bool m_noSuperScare;

	[SerializeField]
	private float m_muzzleVelocityOverride;

	[SerializeField]
	private bool m_highPriority;

	private bool m_hasAimAtPosition;

	[SerializeField]
	private SpeechCategory m_speechCategory;

	private FighterWing m_wing;

	private float m_health;

	private bool m_destroyed;

	private bool m_exploded;

	private bool m_firing;

	private Vector3 m_lastVelocity;

	private bool m_isInvincible;

	private float m_scaredCountdown;

	private float m_scaledStartHealth;

	private bool m_hasRegisteredV1;

	private bool m_isSuperScared;

	private bool m_hasWing;

	public event Action OnDestroyed;

	public void SetInvincible(bool invincible)
	{
		m_isInvincible = invincible;
	}

	public bool IsV1()
	{
		return m_isV1Rocket;
	}

	public bool IsHighPriority()
	{
		return m_highPriority;
	}

	public FighterAI GetAI()
	{
		return m_fighterAi;
	}

	public bool IsTagged()
	{
		return m_taggableItem.IsFullyTagged();
	}

	public Vector3 GetAimAtPosition()
	{
		if (!m_hasAimAtPosition)
		{
			return base.transform.position;
		}
		return m_aimAt.position;
	}

	public void SetFromArea(FighterWing wing, bool shouldHunt)
	{
		m_wing = wing;
		m_shouldHunt = shouldHunt;
		if (m_wing != null)
		{
			m_hasWing = true;
			m_wing.AssignFighter(this);
		}
		else
		{
			m_hasWing = false;
		}
	}

	public bool HasWing()
	{
		return m_hasWing;
	}

	public void SetShouldHunt()
	{
		m_shouldHunt = true;
	}

	public bool ShouldHunt()
	{
		return m_shouldHunt;
	}

	public bool IgnoreForStats()
	{
		return m_dontCountTowardsAchievementStats;
	}

	private void OnDestroy()
	{
		if (m_hasWing)
		{
			m_wing.RemoveFighter(this);
		}
		if (Singleton<MissionCoordinator>.Instance != null)
		{
			Singleton<MissionCoordinator>.Instance.GetFighterCoordinator().DeregisterFighter(this);
		}
	}

	private void OnEnable()
	{
		Singleton<MissionCoordinator>.Instance.GetFighterCoordinator().RegisterFigher(this);
		if (m_isV1Rocket && !m_hasRegisteredV1)
		{
			Singleton<MissionCoordinator>.Instance.GetFighterCoordinator().RegisterV1Only();
			m_hasRegisteredV1 = true;
		}
		if (m_useDifficultySettings)
		{
			FighterDifficultySettings.DifficultySetting currentDifficulty = Singleton<MissionCoordinator>.Instance.GetFighterCoordinator().GetCurrentDifficulty();
			float num = Singleton<SaveDataContainer>.Instance.Get().PerkGetFighterHealthReduction();
			float num2 = Singleton<SaveDataContainer>.Instance.Get().PerkGetFighterDamageReduction();
			float num3 = 1f;
			AceFighterInMission component = GetComponent<AceFighterInMission>();
			if (component != null)
			{
				num3 = component.GetHealthMultiplier();
			}
			m_scaledStartHealth = (m_health = m_startingHealth * Mathf.Lerp(currentDifficulty.m_healthMultiplier, 1f, m_blendDifficultySettingsDamage) * num * num3);
			float firePhase = UnityEngine.Random.Range(0f, 10f);
			RaycastGunTurret[] componentsInChildren = GetComponentsInChildren<RaycastGunTurret>();
			foreach (RaycastGunTurret raycastGunTurret in componentsInChildren)
			{
				raycastGunTurret.SetDamageMultiplier(Mathf.Lerp(currentDifficulty.m_damageMultiplier, 1f, m_blendDifficultySettingsHealth) * num2);
				raycastGunTurret.SetFirePhaseCutoff(Mathf.Lerp(currentDifficulty.m_firePhaseMultiplier, 1f, m_blendDifficultySettingsPhase));
				raycastGunTurret.SetFirePhase(firePhase);
			}
		}
		else
		{
			m_scaledStartHealth = (m_health = m_startingHealth);
			float firePhase2 = UnityEngine.Random.Range(0f, 10f);
			RaycastGunTurret[] componentsInChildren2 = GetComponentsInChildren<RaycastGunTurret>();
			foreach (RaycastGunTurret raycastGunTurret2 in componentsInChildren2)
			{
				raycastGunTurret2.SetFirePhase(firePhase2);
			}
		}
		m_hasAimAtPosition = m_aimAt != null;
	}

	public void ResetHealth(float amt)
	{
		m_scaledStartHealth = (m_health = amt);
	}

	private void OnDisable()
	{
		if (Singleton<MissionCoordinator>.Instance != null)
		{
			Singleton<MissionCoordinator>.Instance.GetFighterCoordinator().DeregisterFighter(this);
		}
	}

	public Vector3 GetVelocity()
	{
		return m_dynamics.GetVelocity();
	}

	public Vector3 GetCachedVelocity()
	{
		return m_lastVelocity;
	}

	public FighterDynamics GetDynamics()
	{
		return m_dynamics;
	}

	public void SetLayer(Transform t, int layer)
	{
		t.gameObject.layer = layer;
		foreach (Transform item in t)
		{
			SetLayer(item, layer);
		}
	}

	public void Damage(float amt, object relatedObject)
	{
		m_health -= amt;
		if (m_isInvincible && m_health < m_scaledStartHealth * 0.5f)
		{
			m_health = m_scaledStartHealth * 0.5f;
		}
		m_damageEffectScaler.SetScale(1f - GetHealthNormalised());
		if (!(m_health < 0f) || m_destroyed)
		{
			return;
		}
		if (relatedObject != null)
		{
			if (relatedObject is CrewmanAvatar)
			{
				((CrewmanAvatar)relatedObject).FighterDestroyed(m_speechCategory);
			}
			else if (relatedObject is FighterAI_FriendlySpitfire && (m_speechCategory == SpeechCategory.FighterNormal || m_speechCategory == SpeechCategory.EnemyAce))
			{
				((FighterAI_FriendlySpitfire)relatedObject).FighterDestroyed();
			}
		}
		Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetMissionLog().AddFighterDestroyed();
		SetDestroyed(forceExplode: false, forceDontExplode: false, byPlayer: true);
	}

	public void SetDestroyed(bool forceExplode, bool forceDontExplode, bool byPlayer)
	{
		if (!m_destroyed)
		{
			if (!string.IsNullOrEmpty(m_triggerOnDestroy))
			{
				Singleton<MissionCoordinator>.Instance.FireTrigger(m_triggerOnDestroy);
			}
			RaycastGunTurret[] forwardFacingGuns = m_forwardFacingGuns;
			foreach (RaycastGunTurret raycastGunTurret in forwardFacingGuns)
			{
				raycastGunTurret.SetFiring(firing: false, Vector3.zero);
			}
			m_destroyed = true;
			if (byPlayer)
			{
				Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetMissionLog().SetFighterDestroyed(m_xpReward);
				Singleton<EndlessModeGameFlow>.Instance.GetEndlessModeResult().m_totalScore += m_xpReward;
				Singleton<MissionXPCounter>.Instance.AddXP(m_xpReward, base.transform, base.transform.position);
				if (m_isV1Rocket)
				{
					AchievementSystem.Achievement achievement = Singleton<AchievementSystem>.Instance.GetAchievement("V1Destroyed");
					if (achievement != null)
					{
						Singleton<AchievementSystem>.Instance.AwardAchievement(achievement);
					}
				}
			}
			if (Singleton<MissionCoordinator>.Instance != null)
			{
				Singleton<MissionCoordinator>.Instance.GetFighterCoordinator().DeregisterFighter(this);
			}
			m_dynamics.SetDestroyed();
			if (!m_dontCountTowardsAchievementStats)
			{
				Singleton<AchievementSystem>.Instance.GetUserStat("FightersDestroyed")?.ModifyValue(1);
			}
			if (this.OnDestroyed != null)
			{
				this.OnDestroyed();
			}
		}
		if ((UnityEngine.Random.Range(0f, 1f) <= m_explodeChance || forceExplode) && !forceDontExplode)
		{
			Explode();
		}
	}

	public bool IsDestroyed()
	{
		return m_destroyed;
	}

	private void Explode()
	{
		if (!m_exploded)
		{
			DboxInMissionController.DBoxCall(DboxSdkWrapper.PostExplosion, base.transform.position);
			GameObject fromPoolSlow = Singleton<PoolManager>.Instance.GetFromPoolSlow(m_explodeEffect);
			fromPoolSlow.btransform().SetFromCurrentPage(base.transform.position);
			m_exploded = true;
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void Update()
	{
		if (m_destroyed)
		{
			float magnitude = (Singleton<BomberSpawn>.Instance.GetBomberSystems().transform.position - base.transform.position).magnitude;
			if (magnitude < 60f)
			{
				Explode();
			}
			if (base.transform.position.y < 0f)
			{
				Explode();
			}
		}
		m_scaredCountdown -= Time.deltaTime;
		if (m_scaredCountdown < 0f)
		{
			m_scaredCountdown = 0f;
		}
		m_lastVelocity = m_dynamics.GetVelocity();
	}

	public void StartFiringGuns()
	{
		if (!m_destroyed)
		{
			RaycastGunTurret[] forwardFacingGuns = m_forwardFacingGuns;
			foreach (RaycastGunTurret raycastGunTurret in forwardFacingGuns)
			{
				raycastGunTurret.SetFiring(firing: true, m_dynamics.GetVelocity());
			}
			m_firing = true;
		}
	}

	public void StopFiringGuns()
	{
		RaycastGunTurret[] forwardFacingGuns = m_forwardFacingGuns;
		foreach (RaycastGunTurret raycastGunTurret in forwardFacingGuns)
		{
			raycastGunTurret.SetFiring(firing: false, Vector3.zero);
		}
		m_firing = false;
	}

	public float GetHealthNormalised()
	{
		return m_health / m_scaledStartHealth;
	}

	public override bool IsDamageBlocker()
	{
		return false;
	}

	public override float DamageGetPassthrough(float amt, DamageSource ds)
	{
		if (ds.m_damageShapeEffect == DamageSource.DamageShape.Line)
		{
			Singleton<CommonEffectManager>.Instance.EffectHit(ds.m_raycastInfo.point, Quaternion.LookRotation(ds.m_raycastInfo.normal), base.transform, CommonEffectManager.AudioHitType.Fuselage, ds.m_fromProjectile);
		}
		Damage(amt, ds.m_relatedObject);
		return 0f;
	}

	public float GetMuzzleVelocity()
	{
		if (m_forwardFacingGuns.Length > 0)
		{
			return m_forwardFacingGuns[0].GetMuzzleVelocity();
		}
		if (m_muzzleVelocityOverride == 0f)
		{
			return m_referenceRGT.GetMuzzleVelocity();
		}
		return m_muzzleVelocityOverride;
	}

	public FighterWing GetFighterWing()
	{
		return m_wing;
	}

	public void Scare(bool canSuperScare)
	{
		if (canSuperScare && UnityEngine.Random.Range(0f, 100f) > 80f && !m_noSuperScare)
		{
			m_isSuperScared = true;
		}
		m_scaredCountdown = 3f;
	}

	public bool CanSuperScare()
	{
		return !m_noSuperScare;
	}

	public bool IsSuperScared()
	{
		return m_isSuperScared;
	}

	public bool IsScared()
	{
		return m_scaredCountdown > 0f;
	}
}
