using System;
using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;
using WingroveAudio;

public class StationGunner : Station
{
	[SerializeField]
	private float m_maxRotationY;

	[SerializeField]
	private float m_maxRotationX;

	[SerializeField]
	private float m_minRotationY;

	[SerializeField]
	private float m_minRotationX;

	[SerializeField]
	private Transform m_rotationNodeY;

	[SerializeField]
	private Transform m_rotationNodeX;

	[SerializeField]
	private float m_aimSpeedDegrees = 90f;

	[SerializeField]
	private float m_aimAcceleration = 180f;

	[SerializeField]
	private float m_damping = 10f;

	[SerializeField]
	private float m_maxFireDistance = 1000f;

	[SerializeField]
	private float m_minDistance;

	[SerializeField]
	private float m_damageRate = 50f;

	[SerializeField]
	private int m_gunViewResolution;

	[SerializeField]
	private Transform m_UITrackingTransform;

	[SerializeField]
	private Transform m_baseOfGun;

	[SerializeField]
	private Transform m_forwardNode;

	[SerializeField]
	private float m_fovDegrees;

	[SerializeField]
	private float m_angleX;

	[SerializeField]
	private float m_angleY;

	[SerializeField]
	private float m_aimAheadFactor = 0.75f;

	[SerializeField]
	private float m_burstOnTime = 1.5f;

	[SerializeField]
	private float m_burstOffTime = 0.4f;

	[SerializeField]
	private float m_testFireDuration = 1f;

	[SerializeField]
	private float m_reloadDuration = 3f;

	[SerializeField]
	private float m_minimumAimAssist;

	[SerializeField]
	private float m_maximumAimAssist = 10f;

	[SerializeField]
	private float m_minimumDamageMultiplier = 1f;

	[SerializeField]
	private float m_maximumDamageMultiplier = 1.5f;

	[SerializeField]
	private CrewmanSkillAbilityFloat m_gunAimingSkill;

	[SerializeField]
	private AmmoFeed m_ammoFeed;

	[SerializeField]
	private float m_ammoConserveAimMin = 0.85f;

	[SerializeField]
	private float m_ammoConserveAimMax = 0.95f;

	[SerializeField]
	private SkillRefreshTimer m_skillTimer;

	[SerializeField]
	private CrewmanSkillAbilityBool m_focusSkill;

	[SerializeField]
	private CrewmanSkillAbilityBool m_defensiveFireSkill;

	[SerializeField]
	private CrewmanSkillAbilityBool m_incendiaryAmmoSkill;

	[SerializeField]
	private CrewmanSkillAbilityBool m_armourPiercingSkill;

	[SerializeField]
	private CrewmanSkillAbilityBool m_highExplosiveSkill;

	[SerializeField]
	private BomberSystemUniqueId m_thisSystem;

	[SerializeField]
	private HydraulicsTank m_hydraulics;

	[SerializeField]
	private GameObject m_abilityActiveHierarchy;

	[SerializeField]
	private float m_inaccuracyAimAheadLvl1 = 3f;

	[SerializeField]
	private float m_inaccuracyAimAheadLvl12;

	[SerializeField]
	private float m_inaccuracyFrequency = 0.2f;

	[SerializeField]
	private BomberMissileLauncher m_missileLauncher;

	[SerializeField]
	private bool m_noFireZone;

	[SerializeField]
	private float m_noFireMinX;

	[SerializeField]
	private float m_noFireMaxX;

	[SerializeField]
	private float m_noFireMinY;

	[SerializeField]
	private float m_noFireMaxY;

	[SerializeField]
	private bool m_hydraulicsNotRequired;

	private bool m_isActive;

	private FighterPlane m_previousTarget;

	private FighterCoordinator m_fighterCoordinator;

	private RenderTexture m_cameraTexture;

	private float m_burstTimer;

	private bool m_burstOnState;

	private float m_xRotateVelocity;

	private float m_yRotateVelocity;

	private InMissionNotification m_ammoNotification;

	private InMissionNotification m_ammoLowNotification;

	private InMissionNotification m_reloadingNotification;

	private BomberState m_bomberState;

	private SkillRefreshTimer.SkillTimer m_testFireTimer;

	private SkillRefreshTimer.SkillTimer m_reloadTimer;

	private SkillRefreshTimer.SkillTimer m_incendiaryTimer;

	private SkillRefreshTimer.SkillTimer m_armourPiercingTimer;

	private SkillRefreshTimer.SkillTimer m_highExplosiveTimer;

	private SkillRefreshTimer.SkillTimer m_focusTimer;

	private SkillRefreshTimer.SkillTimer m_defensiveFireTimer;

	private SkillRefreshTimer.SkillTimer m_fireMissileTimer;

	private Interaction m_replaceBeltsInteraction = new Interaction("replaceBelts", queryProgress: false, EnumToIconMapping.InteractionOrAlertType.Ammo);

	private RaycastGunTurret[] m_raycastGuns;

	private InMissionNotification m_hydraulicsNotification;

	private ProjectileType m_currentProjectileType;

	private GunTurretUpgrade m_gunTurretUpgrade;

	private float m_timeSinceLowAmmoComplaint;

	private float m_aimPhase;

	private float m_timeSinceFire;

	private bool m_canSuperScare;

	private string m_gunAimingSkillId;

	private bool m_abilityActiveIsActive;

	private Vector3 m_debugPos;

	public Transform GetUITrackingTransform()
	{
		return m_UITrackingTransform;
	}

	public RenderTexture GetCameraTexture()
	{
		return m_cameraTexture;
	}

	private void SetProjectileType()
	{
		ProjectileType projectileType = m_gunTurretUpgrade.GetProjectileType();
		ProjectileType projectileType2 = projectileType;
		if (m_armourPiercingTimer.IsActive())
		{
			projectileType2 = m_gunTurretUpgrade.GetProjectileTypeArmourPiercing();
		}
		if (m_incendiaryTimer.IsActive())
		{
			projectileType2 = m_gunTurretUpgrade.GetProjectileTypeIncendiary();
		}
		if (m_highExplosiveTimer.IsActive())
		{
			projectileType2 = m_gunTurretUpgrade.GetProjectileTypeExplosive();
		}
		if (projectileType2 != m_currentProjectileType)
		{
			m_currentProjectileType = projectileType2;
			RaycastGunTurret[] raycastGuns = m_raycastGuns;
			foreach (RaycastGunTurret raycastGunTurret in raycastGuns)
			{
				raycastGunTurret.SetProjectileType(m_currentProjectileType);
			}
			if (projectileType2 != projectileType)
			{
				m_ammoFeed.SetInfinite(infinite: true);
			}
			else
			{
				m_ammoFeed.SetInfinite(infinite: false);
			}
		}
	}

	public override void Start()
	{
		base.Start();
		m_gunAimingSkillId = m_gunAimingSkill.GetCachedName();
		m_fighterCoordinator = Singleton<FighterCoordinator>.Instance;
		m_replaceBeltsInteraction.SetMaterialIndex(1);
		m_aimPhase = UnityEngine.Random.Range(0f, 1f);
		GameObject prefabSpawendRoot = m_thisSystem.GetPrefabSpawendRoot();
		if (prefabSpawendRoot != null)
		{
			m_raycastGuns = prefabSpawendRoot.GetComponentsInChildren<RaycastGunTurret>();
			OutlineMesh[] componentsInChildren = prefabSpawendRoot.GetComponentsInChildren<OutlineMesh>();
			RegisterAdditionalOutlines(componentsInChildren);
		}
		m_testFireTimer = m_skillTimer.CreateNew();
		m_reloadTimer = m_skillTimer.CreateNew();
		m_incendiaryTimer = m_skillTimer.CreateNew();
		m_armourPiercingTimer = m_skillTimer.CreateNew();
		m_highExplosiveTimer = m_skillTimer.CreateNew();
		m_focusTimer = m_skillTimer.CreateNew();
		m_defensiveFireTimer = m_skillTimer.CreateNew();
		m_fireMissileTimer = m_skillTimer.CreateNew();
		GunTurretUpgrade gunTurretUpgrade = (m_gunTurretUpgrade = (GunTurretUpgrade)m_thisSystem.GetUpgrade());
		RaycastGunTurret[] raycastGuns = m_raycastGuns;
		foreach (RaycastGunTurret raycastGunTurret in raycastGuns)
		{
			raycastGunTurret.SetFiring(firing: false, Vector3.zero);
			raycastGunTurret.SetAmmoFeed(m_ammoFeed);
			raycastGunTurret.SetProjectileType(gunTurretUpgrade.GetProjectileType());
			raycastGunTurret.SetSendsDBoxEvents();
		}
		SetProjectileType();
		m_ammoFeed.SetAmmoPerBelt(gunTurretUpgrade.GetAmmoPerReload(), gunTurretUpgrade.HasAmmoFeed());
		m_bomberState = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState();
		m_abilityActiveHierarchy.SetActive(value: false);
	}

	public override Interaction GetInteractionOptions(CrewmanAvatar crewman)
	{
		if (crewman.GetCarriedItem() != null && crewman.GetCarriedItem().GetComponent<AmmoBelt>() != null)
		{
			return m_replaceBeltsInteraction;
		}
		return base.GetInteractionOptions(crewman);
	}

	protected override void FinishInteraction(Interaction interaction, CrewmanAvatar crewman)
	{
		if (interaction == m_sitInteraction)
		{
			base.FinishInteraction(interaction, crewman);
		}
		else if (interaction == m_replaceBeltsInteraction)
		{
			m_ammoFeed.ReplaceBelts();
			crewman.GetCarriedItem().GetComponent<AmmoBelt>().Use();
			WingroveRoot.Instance.PostEventGO("STATION_GUNNER_REPLACE_BELTS", base.gameObject);
			if (GetCurrentCrewman() == null)
			{
				SetCrewman(crewman);
			}
			if (m_ammoFeed.GetAmmo() == 0)
			{
				RequestReload();
			}
		}
	}

	public bool IsFiring()
	{
		return m_raycastGuns[0].DidFire();
	}

	public BomberMissileLauncher GetMissileLauncher()
	{
		return m_missileLauncher;
	}

	public void FireMissile()
	{
		if (m_fireMissileTimer.CanStart() && m_missileLauncher.FireMissile(this, m_bomberState.GetVelocity(), m_previousTarget))
		{
			m_fireMissileTimer.Start(5f, m_missileLauncher.GetRechargeTime());
		}
	}

	private void UpdateAim(float aimXTarget, float aimYTarget)
	{
		if (!m_hydraulics.IsBroken() || m_hydraulicsNotRequired)
		{
			if (m_hydraulicsNotification != null)
			{
				GetNotifications().RemoveNotification(m_hydraulicsNotification);
				m_hydraulicsNotification = null;
			}
			float num = aimYTarget - m_angleY;
			if (num > 180f)
			{
				num -= 360f;
			}
			else if (num < -180f)
			{
				num += 360f;
			}
			m_yRotateVelocity += Time.deltaTime * m_aimAcceleration * (num / 90f);
			m_yRotateVelocity = Mathf.Clamp(m_yRotateVelocity, 0f - m_aimSpeedDegrees, m_aimSpeedDegrees);
			m_angleY += m_yRotateVelocity * Time.deltaTime;
			float num2 = aimXTarget - m_angleX;
			if (num2 > 180f)
			{
				num2 -= 360f;
			}
			else if (num2 < -180f)
			{
				num2 += 360f;
			}
			m_xRotateVelocity += Time.deltaTime * m_aimAcceleration * (num2 / 90f);
			m_xRotateVelocity = Mathf.Clamp(m_xRotateVelocity, 0f - m_aimSpeedDegrees, m_aimSpeedDegrees);
			m_angleX += m_xRotateVelocity * Time.deltaTime;
			m_xRotateVelocity -= Time.deltaTime * m_xRotateVelocity * m_damping;
			m_yRotateVelocity -= Time.deltaTime * m_yRotateVelocity * m_damping;
			if (m_angleY > 180f)
			{
				m_angleY -= 360f;
			}
			if (m_angleY < -180f)
			{
				m_angleY += 360f;
			}
			if (m_angleX > 180f)
			{
				m_angleX -= 360f;
			}
			if (m_angleX < -180f)
			{
				m_angleX += 360f;
			}
			CrewmanAvatar currentCrewman = GetCurrentCrewman();
			if (currentCrewman != null)
			{
				float proficiency = Singleton<CrewmanSkillUpgradeInfo>.Instance.GetProficiency(m_gunAimingSkillId, currentCrewman.GetCrewman().GetPrimarySkill(), currentCrewman.GetCrewman().GetSecondarySkill());
				float num3 = Mathf.Lerp(m_minimumAimAssist, m_maximumAimAssist, proficiency);
				float num4 = aimXTarget - m_angleX;
				float num5 = aimYTarget - m_angleY;
				if (num4 > 180f)
				{
					num4 -= 360f;
				}
				else if (num4 < -180f)
				{
					num4 += 360f;
				}
				if (num5 > 180f)
				{
					num5 -= 360f;
				}
				else if (num5 < -180f)
				{
					num5 += 360f;
				}
				m_angleX += num4 * Mathf.Clamp01(num3 * Time.deltaTime);
				m_angleY += num5 * Mathf.Clamp01(num3 * Time.deltaTime);
				if (m_focusTimer.IsActive())
				{
					m_angleX = aimXTarget;
					m_angleY = aimYTarget;
				}
			}
		}
		else if (m_hydraulicsNotification == null)
		{
			m_hydraulicsNotification = new InMissionNotification("ui_notification_hydraulics", StationNotificationUrgency.Red, EnumToIconMapping.InteractionOrAlertType.Hydraulics);
			GetNotifications().RegisterNotification(m_hydraulicsNotification);
		}
		m_angleX = Mathf.Clamp(m_angleX, m_minRotationX, m_maxRotationX);
		m_angleY = Mathf.Clamp(m_angleY, m_minRotationY, m_maxRotationY);
		m_rotationNodeY.localRotation = Quaternion.Euler(m_angleY, 0f, 0f);
		m_rotationNodeX.localRotation = Quaternion.Euler(0f, m_angleX, 0f);
	}

	private void OnDrawGizmos()
	{
		if (m_debugPos.sqrMagnitude != 0f)
		{
			Gizmos.DrawSphere(m_debugPos, 1f);
		}
	}

	public AmmoFeed GetAmmoFeed()
	{
		return m_ammoFeed;
	}

	private void Update()
	{
		//Discarded unreachable code: IL_01fa
		m_timeSinceFire += Time.deltaTime;
		CrewmanAvatar currentCrewman = GetCurrentCrewman();
		bool flag = false;
		if (HasCrewman())
		{
			m_aimPhase += Time.deltaTime * m_inaccuracyFrequency;
			if (m_aimPhase > 1f)
			{
				m_aimPhase -= 1f;
			}
			SetProjectileType();
			if (m_focusTimer.IsActive() || m_defensiveFireTimer.IsActive())
			{
				flag = true;
			}
			if (currentCrewman.CanDoActions())
			{
				List<FighterPlane> allFighters = m_fighterCoordinator.GetAllFighters();
				bool foundFighter = false;
				FighterPlane fighterPlane = Singleton<FighterCoordinator>.Instance.GetCurrentTarget(m_previousTarget, m_minDistance, m_baseOfGun, m_fovDegrees, out foundFighter);
				float proficiency = Singleton<CrewmanSkillUpgradeInfo>.Instance.GetProficiency(m_gunAimingSkillId, currentCrewman.GetCrewman().GetPrimarySkill(), currentCrewman.GetCrewman().GetSecondarySkill());
				if (m_reloadTimer.IsActive())
				{
					foundFighter = false;
					fighterPlane = null;
				}
				if (!foundFighter)
				{
					m_previousTarget = null;
					if (m_defensiveFireTimer.IsActive())
					{
						UpdateAim(Mathf.Sin(m_aimPhase * (float)Math.PI * 2f) * 30f, 0f);
						RaycastGunTurret[] raycastGuns = m_raycastGuns;
						foreach (RaycastGunTurret raycastGunTurret in raycastGuns)
						{
							raycastGunTurret.SetFiring(firing: true, Vector3.zero);
						}
					}
					else
					{
						UpdateAim(0f, 0f);
						RaycastGunTurret[] raycastGuns2 = m_raycastGuns;
						foreach (RaycastGunTurret raycastGunTurret2 in raycastGuns2)
						{
							raycastGunTurret2.SetFiring(firing: false, Vector3.zero);
						}
					}
				}
				else
				{
					float bulletSpeed = 0f;
					RaycastGunTurret[] raycastGuns3 = m_raycastGuns;
					int num = 0;
					if (num < raycastGuns3.Length)
					{
						RaycastGunTurret raycastGunTurret3 = raycastGuns3[num];
						bulletSpeed = raycastGunTurret3.GetMuzzleVelocity();
					}
					float num2 = Mathf.Lerp(m_inaccuracyAimAheadLvl1, m_inaccuracyAimAheadLvl12, proficiency);
					if (m_focusTimer.IsActive())
					{
						num2 *= 0.5f;
					}
					num2 += Singleton<VisibilityHelpers>.Instance.GetNightFactor() * 1f;
					float inaccuracyFactor = AimingUtils.GetInaccuracyFactor(num2, m_aimPhase, 0f);
					if ((fighterPlane.GetAimAtPosition() - m_forwardNode.position).magnitude < 50f)
					{
						inaccuracyFactor = 0f;
					}
					Vector3 vector = AimingUtils.GetAimAheadTarget(m_forwardNode.position, fighterPlane.GetAimAtPosition(), fighterPlane.GetCachedVelocity() - m_bomberState.GetPhysicsModel().GetVelocity(), bulletSpeed, inaccuracyFactor, isGroundBasedOrFriendly: true);
					if (m_defensiveFireTimer.IsActive())
					{
						vector = fighterPlane.GetAimAtPosition();
					}
					Vector3 vector2 = vector - m_forwardNode.position;
					if (m_previousTarget != fighterPlane)
					{
						m_previousTarget = fighterPlane;
					}
					bool flag2 = false;
					Vector3 v = vector2;
					Vector3 vector3 = base.transform.worldToLocalMatrix.MultiplyVector(v);
					float num3 = Mathf.Atan2(0f - vector3.y, new Vector3(vector3.x, 0f, vector3.z).magnitude) * 57.29578f;
					float num4 = Mathf.Atan2(0f - vector3.z, vector3.x) * 57.29578f;
					if (m_defensiveFireTimer.IsActive())
					{
						num4 += Mathf.Sin(m_aimPhase * (float)Math.PI * 2f) * 30f;
					}
					if (!float.IsNaN(num4) && !float.IsNaN(num3))
					{
						UpdateAim(num4, num3);
					}
					float num5 = Mathf.Lerp(m_ammoConserveAimMin, m_ammoConserveAimMax, proficiency);
					if (m_defensiveFireTimer.IsActive())
					{
						flag2 = true;
					}
					if (vector2.magnitude < m_maxFireDistance && Vector3.Dot(vector2.normalized, m_forwardNode.forward) > num5)
					{
						flag2 = true;
					}
					if (m_noFireZone && m_angleX > m_noFireMinX && m_angleX < m_noFireMaxX && m_angleY > m_noFireMinY && m_angleY < m_noFireMaxY)
					{
						flag2 = false;
					}
					if (!m_burstOnState || flag2)
					{
						m_burstTimer -= Time.deltaTime;
					}
					if (m_burstTimer < 0f)
					{
						m_burstOnState = !m_burstOnState;
						if (m_burstOnState)
						{
							m_burstTimer = m_burstOnTime;
						}
						else
						{
							m_burstTimer = m_burstOffTime;
						}
					}
					if (m_focusTimer.IsActive() || m_defensiveFireTimer.IsActive())
					{
						m_burstOnState = true;
					}
					if (flag2 && m_burstOnState && !m_reloadTimer.IsActive())
					{
						if (m_defensiveFireTimer.IsActive() && flag2 && m_ammoFeed.GetAmmo() > 0)
						{
							fighterPlane.Scare(m_canSuperScare);
							if (fighterPlane.CanSuperScare())
							{
								m_canSuperScare = false;
							}
						}
						RaycastGunTurret[] raycastGuns4 = m_raycastGuns;
						foreach (RaycastGunTurret raycastGunTurret4 in raycastGuns4)
						{
							raycastGunTurret4.SetRelatedObject(GetCurrentCrewman());
							raycastGunTurret4.SetFiring(firing: true, m_bomberState.GetPhysicsModel().GetVelocity());
						}
						m_timeSinceFire = 0f;
					}
					else
					{
						RaycastGunTurret[] raycastGuns5 = m_raycastGuns;
						foreach (RaycastGunTurret raycastGunTurret5 in raycastGuns5)
						{
							raycastGunTurret5.SetFiring(firing: false, Vector3.zero);
						}
					}
				}
				if (m_testFireTimer.IsActive() && m_testFireTimer.GetInUseNormalised() < 0.75f && !m_reloadTimer.IsActive())
				{
					RaycastGunTurret[] raycastGuns6 = m_raycastGuns;
					foreach (RaycastGunTurret raycastGunTurret6 in raycastGuns6)
					{
						raycastGunTurret6.SetRelatedObject(GetCurrentCrewman());
						raycastGunTurret6.SetFiring(firing: true, m_bomberState.GetPhysicsModel().GetVelocity());
					}
					m_timeSinceFire = 0f;
				}
				float damageMultiplier = Mathf.Lerp(m_minimumDamageMultiplier, m_maximumDamageMultiplier, proficiency) * ((GunTurretUpgrade)m_thisSystem.GetUpgrade()).GetDamageMultiplier();
				RaycastGunTurret[] raycastGuns7 = m_raycastGuns;
				foreach (RaycastGunTurret raycastGunTurret7 in raycastGuns7)
				{
					raycastGunTurret7.SetDamageMultiplier(damageMultiplier);
				}
				if (m_reloadTimer.IsDone())
				{
					WingroveRoot.Instance.PostEventGO("STATION_GUNNER_RELOAD_FINISH", base.gameObject);
					m_ammoFeed.ChangeBelt();
					m_reloadTimer.ResetDoneFlag();
				}
				if (m_ammoFeed.GetAmmo() == 0 && m_ammoFeed.GetBelts() != 0 && !m_reloadTimer.IsActive())
				{
					m_reloadTimer.Start(m_reloadDuration, 1f);
				}
				m_highExplosiveTimer.DoRechargedSpeech(m_highExplosiveSkill.GetTitleText(), GetCurrentCrewman(), m_highExplosiveSkill);
				m_armourPiercingTimer.DoRechargedSpeech(m_armourPiercingSkill.GetTitleText(), GetCurrentCrewman(), m_armourPiercingSkill);
				m_incendiaryTimer.DoRechargedSpeech(m_incendiaryAmmoSkill.GetTitleText(), GetCurrentCrewman(), m_incendiaryAmmoSkill);
				m_focusTimer.DoRechargedSpeech(m_focusSkill.GetTitleText(), GetCurrentCrewman(), m_focusSkill);
				m_defensiveFireTimer.DoRechargedSpeech(m_defensiveFireSkill.GetTitleText(), GetCurrentCrewman(), m_defensiveFireSkill);
			}
		}
		else
		{
			UpdateAim(0f, 0f);
			m_previousTarget = null;
			RaycastGunTurret[] raycastGuns8 = m_raycastGuns;
			foreach (RaycastGunTurret raycastGunTurret8 in raycastGuns8)
			{
				raycastGunTurret8.SetFiring(firing: false, Vector3.zero);
			}
			m_focusTimer.FinishEarly();
			m_defensiveFireTimer.FinishEarly();
			m_incendiaryTimer.FinishEarly();
			m_highExplosiveTimer.FinishEarly();
			m_armourPiercingTimer.FinishEarly();
			m_reloadTimer.FinishEarly();
		}
		if (flag != m_abilityActiveIsActive)
		{
			m_abilityActiveHierarchy.SetActive(flag);
			m_abilityActiveIsActive = flag;
		}
		bool flag3 = false;
		if (m_ammoFeed.GetAmmo() == 0 && m_ammoFeed.GetBelts() == 0)
		{
			flag3 = true;
		}
		if (m_reloadTimer.IsActive())
		{
			if (m_reloadingNotification == null)
			{
				m_reloadingNotification = new InMissionNotification(string.Empty, StationNotificationUrgency.Yellow, EnumToIconMapping.InteractionOrAlertType.Ammo);
				GetNotifications().RegisterNotification(m_reloadingNotification);
			}
			m_reloadingNotification.SetProgress(m_reloadTimer.GetProgressNormalised());
		}
		else if (m_reloadingNotification != null)
		{
			GetNotifications().RemoveNotification(m_reloadingNotification);
			m_reloadingNotification = null;
		}
		m_timeSinceLowAmmoComplaint += Time.deltaTime;
		if (flag3)
		{
			if (m_ammoNotification == null)
			{
				m_ammoNotification = new InMissionNotification("station_notification_out_of_ammo", StationNotificationUrgency.Red, EnumToIconMapping.InteractionOrAlertType.Ammo);
				GetNotifications().RegisterNotification(m_ammoNotification);
			}
			if (m_timeSinceLowAmmoComplaint > 20f)
			{
				Singleton<SpeechPrioritiser>.Instance.RequestNewSpeech(StaticSpeechSets.GunTurretOutOfAmmo, GetCurrentCrewman());
				m_timeSinceLowAmmoComplaint = 0f;
			}
			if (m_ammoLowNotification != null)
			{
				GetNotifications().RemoveNotification(m_ammoLowNotification);
			}
			return;
		}
		if (m_ammoFeed.GetBelts() == 0)
		{
			if (m_ammoLowNotification == null)
			{
				int ammo = m_ammoFeed.GetAmmo();
				m_ammoLowNotification = new InMissionNotification("station_notification_low_ammo", (ammo > 100) ? StationNotificationUrgency.Yellow : StationNotificationUrgency.Red, EnumToIconMapping.InteractionOrAlertType.Ammo);
				GetNotifications().RegisterNotification(m_ammoLowNotification);
				Singleton<SpeechPrioritiser>.Instance.RequestNewSpeech(StaticSpeechSets.GunTurretLowAmmo, GetCurrentCrewman());
			}
		}
		else if (m_ammoLowNotification != null)
		{
			GetNotifications().RemoveNotification(m_ammoLowNotification);
		}
		if (m_ammoNotification != null)
		{
			GetNotifications().RemoveNotification(m_ammoNotification);
			m_ammoNotification = null;
		}
	}

	public void RequestTestFire()
	{
		m_reloadTimer.FinishEarly();
		m_testFireTimer.Start(m_testFireDuration, 1f);
	}

	public void RequestIncendiaryAmmo()
	{
		if (m_incendiaryTimer.CanStart())
		{
			m_highExplosiveTimer.FinishEarly();
			m_armourPiercingTimer.FinishEarly();
			m_incendiaryTimer.Start(m_incendiaryAmmoSkill.GetDurationTimer(), m_incendiaryAmmoSkill.GetCooldownTimer());
		}
	}

	public void RequestHEAmmo()
	{
		if (m_highExplosiveTimer.CanStart())
		{
			m_incendiaryTimer.FinishEarly();
			m_armourPiercingTimer.FinishEarly();
			m_highExplosiveTimer.Start(m_highExplosiveSkill.GetDurationTimer(), m_highExplosiveSkill.GetCooldownTimer());
		}
	}

	public SkillRefreshTimer.SkillTimer GetMissileTimer()
	{
		return m_fireMissileTimer;
	}

	public void RequestAPAmmo()
	{
		if (m_armourPiercingTimer.CanStart())
		{
			m_highExplosiveTimer.FinishEarly();
			m_incendiaryTimer.FinishEarly();
			m_armourPiercingTimer.Start(m_armourPiercingSkill.GetDurationTimer(), m_armourPiercingSkill.GetCooldownTimer());
		}
	}

	public void RequestReload()
	{
		m_testFireTimer.FinishEarly();
		if (!m_reloadTimer.IsActive())
		{
			m_reloadTimer.Start(m_reloadDuration, 1f);
		}
	}

	public void Focus()
	{
		if (m_focusTimer.CanStart())
		{
			Singleton<SaveDataContainer>.Instance.Get().SetHasFocused();
			m_defensiveFireTimer.FinishEarly();
			m_focusTimer.Start(m_focusSkill.GetDurationTimer(), m_focusSkill.GetCooldownTimer());
		}
	}

	public SkillRefreshTimer.SkillTimer GetFocusTimer()
	{
		return m_focusTimer;
	}

	public void DefensiveFire()
	{
		if (m_defensiveFireTimer.CanStart())
		{
			m_focusTimer.FinishEarly();
			m_canSuperScare = true;
			m_defensiveFireTimer.Start(m_focusSkill.GetDurationTimer(), m_focusSkill.GetCooldownTimer());
		}
	}

	public SkillRefreshTimer.SkillTimer GetDefensiveTimer()
	{
		return m_defensiveFireTimer;
	}

	public SkillRefreshTimer.SkillTimer GetIncendiaryTimer()
	{
		return m_incendiaryTimer;
	}

	public SkillRefreshTimer.SkillTimer GetArmourPiercingTimer()
	{
		return m_armourPiercingTimer;
	}

	public SkillRefreshTimer.SkillTimer GetHighExplosiveTimer()
	{
		return m_highExplosiveTimer;
	}

	public bool ComesFromAmmoFeed(AmmoFeed af)
	{
		RaycastGunTurret[] raycastGuns = m_raycastGuns;
		foreach (RaycastGunTurret raycastGunTurret in raycastGuns)
		{
			if (raycastGunTurret.GetAmmoFeed() == af)
			{
				return true;
			}
		}
		return false;
	}

	public override bool IsActivityInProgress(out float progress, out string iconType)
	{
		if (m_ammoFeed.IsInfinite() || m_ammoFeed.HasAmmoFeed())
		{
			iconType = string.Empty;
			progress = 1f;
			return false;
		}
		if (m_reloadTimer.IsActive())
		{
			iconType = "Icon_Ammo";
			progress = m_reloadTimer.GetProgressNormalised();
			return true;
		}
		iconType = string.Empty;
		progress = m_ammoFeed.GetAmmoNormalised();
		return m_timeSinceFire < 0.2f;
	}
}
