using System;
using BomberCrewCommon;
using UnityEngine;

public class AATurret : MonoBehaviour
{
	[SerializeField]
	private Transform m_xzRotationTransform;

	[SerializeField]
	private Transform m_yRotationTransform;

	[SerializeField]
	private RaycastGunTurret[] m_guns;

	[SerializeField]
	private float m_yMin = 20f;

	[SerializeField]
	private float m_yMax = 90f;

	[SerializeField]
	private Transform m_aimNode;

	[SerializeField]
	private float m_maxFireRange = 1500f;

	[SerializeField]
	private float m_maxFireHeight = 300f;

	[SerializeField]
	private float m_maxTurnRate = 45f;

	[SerializeField]
	private float m_maxFireBurst = 3f;

	[SerializeField]
	private float m_fireBurstReloadTime = 5f;

	[SerializeField]
	private SmoothDamageable m_damageableToMonitor;

	[SerializeField]
	private float m_xMin = -360f;

	[SerializeField]
	private float m_xMax = 360f;

	[SerializeField]
	private ShootableType m_targetType;

	[SerializeField]
	private float m_aimPhaseFrequncy = 0.2f;

	[SerializeField]
	private MissionPlaceableObject m_placeable;

	private float m_targetReselect = 3f;

	private Transform m_aimTarget;

	private float m_curYAng = 30f;

	private float m_curXAng = 90f;

	private float m_firingTimeCurrent;

	private float m_reloadTimeCurrent;

	private float m_visibilityCounter;

	private Vector3 m_aimAheadPos;

	private float m_currentLockTimer;

	private float m_aimPhase;

	private Shootable m_currentTarget;

	private bool m_hasDoneMusicTrigger;

	private bool m_isDisabled;

	private float m_curXAngPrev = 99999f;

	private float m_curYAngPrev = 99999f;

	private float m_sleepTime;

	private void Start()
	{
		m_aimPhase = UnityEngine.Random.Range(0f, 1f);
		int num = 0;
		float num2 = Singleton<SaveDataContainer>.Instance.Get().PerkGetFighterDamageReduction();
		FighterDifficultySettings.DifficultySetting currentDifficulty = Singleton<MissionCoordinator>.Instance.GetFighterCoordinator().GetCurrentDifficulty();
		RaycastGunTurret[] guns = m_guns;
		foreach (RaycastGunTurret raycastGunTurret in guns)
		{
			raycastGunTurret.SetDamageMultiplier(num2 * currentDifficulty.m_damageMultiplier);
			raycastGunTurret.PoolProjectiles(2);
			num++;
		}
		if (!(m_placeable != null))
		{
			return;
		}
		string activateTrigger = m_placeable.GetParameter("activateTrigger");
		if (string.IsNullOrEmpty(activateTrigger))
		{
			return;
		}
		m_isDisabled = true;
		MissionCoordinator instance = Singleton<MissionCoordinator>.Instance;
		instance.OnTrigger = (Action<string>)Delegate.Combine(instance.OnTrigger, (Action<string>)delegate(string st)
		{
			if (st == activateTrigger)
			{
				m_isDisabled = false;
			}
		});
	}

	private void UpdateAngles()
	{
		m_curYAng = Mathf.Clamp(m_curYAng, m_yMin, m_yMax);
		m_curXAng = Mathf.Clamp(m_curXAng, m_xMin, m_xMax);
		if (m_curXAng != m_curXAngPrev)
		{
			m_xzRotationTransform.localEulerAngles = new Vector3(0f, m_curXAng, 0f);
		}
		if (m_curYAng != m_curYAngPrev)
		{
			m_yRotationTransform.localEulerAngles = new Vector3(0f - m_curYAng, 0f, 0f);
		}
		m_curXAngPrev = m_curXAng;
		m_curYAngPrev = m_curYAng;
	}

	private void Update()
	{
		Vector3 position = base.transform.position;
		m_aimPhase += Time.deltaTime * m_aimPhaseFrequncy;
		if (m_aimPhase > 1f)
		{
			m_aimPhase -= 1f;
		}
		if (m_damageableToMonitor != null && m_damageableToMonitor.GetHealthNormalised() <= 0f)
		{
			RaycastGunTurret[] guns = m_guns;
			foreach (RaycastGunTurret raycastGunTurret in guns)
			{
				raycastGunTurret.SetFiring(firing: false, Vector3.zero);
			}
			if (m_hasDoneMusicTrigger)
			{
				Singleton<MusicSelectionRules>.Instance.Untrigger(MusicSelectionRules.MusicTriggerEvents.HazardNearBy);
				m_hasDoneMusicTrigger = false;
			}
			return;
		}
		if (m_isDisabled)
		{
			RaycastGunTurret[] guns2 = m_guns;
			foreach (RaycastGunTurret raycastGunTurret2 in guns2)
			{
				raycastGunTurret2.SetFiring(firing: false, Vector3.zero);
			}
			if (m_hasDoneMusicTrigger)
			{
				Singleton<MusicSelectionRules>.Instance.Untrigger(MusicSelectionRules.MusicTriggerEvents.HazardNearBy);
				m_hasDoneMusicTrigger = false;
			}
			return;
		}
		if (m_currentTarget == null || m_visibilityCounter < -10f)
		{
			m_currentTarget = Singleton<ShootableCoordinator>.Instance.GetNearestShootable(m_targetType, position);
			m_visibilityCounter = 0f;
		}
		if (m_currentTarget != null)
		{
			m_targetReselect -= Time.deltaTime;
			if (m_targetReselect < 0f)
			{
				m_targetReselect = 3f;
				m_aimTarget = null;
			}
			Transform centreTransform = m_currentTarget.GetCentreTransform();
			Vector3 position2 = centreTransform.position;
			Vector3 vector = position2 - position;
			if (m_sleepTime > 0f)
			{
				m_sleepTime -= Time.deltaTime;
				UpdateAngles();
				return;
			}
			if (m_aimTarget == null)
			{
				m_aimTarget = m_currentTarget.GetRandomTargetableArea();
			}
			if (Singleton<VisibilityHelpers>.Instance.IsVisibleAILenient(position, position2, m_currentTarget.IsLitUp()))
			{
				m_visibilityCounter = 5f;
			}
			if (vector.sqrMagnitude < m_maxFireRange * 1.5f * (m_maxFireRange * 1.5f))
			{
				if (!m_hasDoneMusicTrigger)
				{
					Singleton<MusicSelectionRules>.Instance.Trigger(MusicSelectionRules.MusicTriggerEvents.HazardNearBy);
					m_hasDoneMusicTrigger = true;
				}
			}
			else
			{
				if (m_hasDoneMusicTrigger)
				{
					Singleton<MusicSelectionRules>.Instance.Untrigger(MusicSelectionRules.MusicTriggerEvents.HazardNearBy);
					m_hasDoneMusicTrigger = false;
				}
				m_sleepTime = UnityEngine.Random.Range(3f, 5f);
			}
			m_visibilityCounter -= Time.deltaTime;
			bool flag = false;
			bool fireFake = false;
			if (m_visibilityCounter > 0f)
			{
				m_currentLockTimer += Time.deltaTime;
				float inaccuracyAATurret = AimingUtils.GetInaccuracyAATurret(Singleton<VisibilityHelpers>.Instance.GetNightFactor(), m_currentTarget.IsEvasive(), m_currentTarget.IsLitUp(), m_currentLockTimer, m_aimPhase);
				vector = (m_aimAheadPos = AimingUtils.GetAimAheadTarget(m_aimNode.position, position2, m_currentTarget.GetVelocity(), m_guns[0].GetMuzzleVelocity(), inaccuracyAATurret, isGroundBasedOrFriendly: true)) - m_aimNode.position;
				Vector3 v = vector;
				Vector3 vector2 = base.transform.worldToLocalMatrix.MultiplyVector(v);
				float num = Mathf.Atan2(vector2.y, new Vector3(vector2.x, 0f, vector2.z).magnitude) * 57.29578f;
				float num2 = Mathf.Atan2(vector2.x, vector2.z) * 57.29578f;
				float num3 = num2 - m_curXAng;
				if (num3 > 180f)
				{
					num3 = 360f - num3;
				}
				else if (num3 < -180f)
				{
					num3 = -360f - num3;
				}
				m_curXAng += Mathf.Clamp(num3, (0f - m_maxTurnRate) * Time.deltaTime, m_maxTurnRate * Time.deltaTime);
				num3 = num - m_curYAng;
				if (num3 > 180f)
				{
					num3 = 360f - num3;
				}
				else if (num3 < -180f)
				{
					num3 = -360f - num3;
				}
				m_curYAng += Mathf.Clamp(num3, (0f - m_maxTurnRate) * Time.deltaTime, m_maxTurnRate * Time.deltaTime);
				m_curXAng = num2;
				m_curYAng = num;
				if (0 == 0 && vector.sqrMagnitude < m_maxFireRange * m_maxFireRange && Mathf.Abs(vector.y) < m_maxFireHeight && Vector3.Dot(m_aimNode.forward, vector.normalized) > 0.9f)
				{
					if (Mathf.Abs(inaccuracyAATurret) > 2f)
					{
						fireFake = true;
					}
					flag = true;
				}
			}
			else
			{
				m_currentLockTimer = 0f;
			}
			if (m_reloadTimeCurrent <= 0f)
			{
				int num4 = 0;
				RaycastGunTurret[] guns3 = m_guns;
				foreach (RaycastGunTurret raycastGunTurret3 in guns3)
				{
					raycastGunTurret3.SetFiring(flag, Vector3.zero);
					raycastGunTurret3.SetFireFake(fireFake);
				}
				if (flag)
				{
					m_firingTimeCurrent += Time.deltaTime;
					if (m_firingTimeCurrent > m_maxFireBurst)
					{
						m_firingTimeCurrent = 0f;
						m_reloadTimeCurrent = m_fireBurstReloadTime;
					}
				}
			}
			else
			{
				int num5 = 0;
				RaycastGunTurret[] guns4 = m_guns;
				foreach (RaycastGunTurret raycastGunTurret4 in guns4)
				{
					raycastGunTurret4.SetFiring(firing: false, Vector3.zero);
				}
				m_reloadTimeCurrent -= Time.deltaTime;
				m_aimTarget = null;
			}
		}
		UpdateAngles();
	}

	private void OnDrawGizmos()
	{
		if (m_currentTarget != null)
		{
			Gizmos.color = Color.white;
			Gizmos.DrawLine(m_currentTarget.GetCentreTransform().position, m_aimNode.position);
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(m_aimAheadPos, m_aimNode.position);
		}
	}
}
