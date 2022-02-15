using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class FighterAI : MonoBehaviour
{
	public enum FighterState
	{
		CircleSpawnPoint,
		KeepingRange,
		AttackFormation,
		BreakFormation,
		ReturningToDistance,
		Reforming,
		Leaving
	}

	[SerializeField]
	private float m_switchToEngagedRange;

	[SerializeField]
	private float m_maintainDistanceTime;

	[SerializeField]
	private float m_breakFormationRange;

	[SerializeField]
	private float m_closestRange;

	[SerializeField]
	private float m_keepDistanceLength;

	[SerializeField]
	private float m_firingRange;

	[SerializeField]
	private float m_avoidDistanceOffset;

	[SerializeField]
	private float m_avoidDistanceLength;

	[SerializeField]
	private float m_maxFireTime;

	[SerializeField]
	private float m_timeBeforeReform;

	[SerializeField]
	private float m_timeToReform;

	[SerializeField]
	protected FighterDynamics m_controls;

	[SerializeField]
	protected FighterPlane m_fighterPlane;

	[SerializeField]
	private Vector3[] m_formationOffsets;

	[SerializeField]
	private float m_patrolAreaRadius;

	[SerializeField]
	private float m_maxAttackTime;

	private float m_catchUpFactor = 0.25f;

	[SerializeField]
	protected Transform m_debugMarker1;

	[SerializeField]
	protected Transform m_debugMarker2;

	[SerializeField]
	protected Transform m_gunFiringNodeAverage;

	[SerializeField]
	private bool m_testAiFighter;

	[SerializeField]
	private bool m_testAiDontFire;

	[SerializeField]
	protected bool m_alternateAimAhead;

	[SerializeField]
	protected float m_alternateAimAheadMultiplier = 1.5f;

	[SerializeField]
	protected float m_regularAimAheadVelFudge;

	[SerializeField]
	private float m_breakAttackSpeedMultiplier = 1f;

	[SerializeField]
	private float m_aimAheadInaccuracyFrequency = 0.2f;

	[SerializeField]
	private float m_inaccuracyScaleMultiplier = 1f;

	[SerializeField]
	private bool m_alwaysAttackBroken;

	[SerializeField]
	private float m_alternateAimAheadUnits = 20f;

	[SerializeField]
	protected BigTransform m_bigTransform;

	protected FighterState m_currentState;

	protected Transform m_currentFiringTarget;

	protected float m_currentTimer;

	protected float m_t;

	protected Vector3d m_avoidTarget;

	protected float m_totalAttackingTime;

	private float m_visibilityCountdown;

	private float m_aimAheadPhase;

	protected float m_inaccuracyCached;

	protected FighterCoordinator.AttackPositions m_currentlyLockedPosition;

	protected Vector3 m_currentlyLockedPositionVector;

	protected bool m_hasCurrentlyLockedPosition;

	private float m_unlockTimer;

	protected bool m_leftBecauseOfTimeOut;

	private bool m_canAttack;

	protected void GetFiringTarget()
	{
		List<Transform> targetableAreas = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetTargetableAreas();
		m_currentFiringTarget = targetableAreas[Random.Range(0, targetableAreas.Count)];
	}

	protected void SetAvoidTarget()
	{
		BomberState bomberState = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState();
		Vector3d vector3d = bomberState.GetBigTransform().position - m_bigTransform.position;
		if (base.transform.position.y > 0f)
		{
			m_avoidTarget = m_bigTransform.position + new Vector3d(vector3d.x * (double)m_avoidDistanceLength, m_avoidDistanceOffset, vector3d.z * (double)m_avoidDistanceLength);
		}
		else if (base.transform.position.z > 0f)
		{
			m_avoidTarget = m_bigTransform.position + new Vector3d(vector3d.x * (double)m_avoidDistanceLength, vector3d.y * (double)m_avoidDistanceLength, m_avoidDistanceOffset);
		}
		else
		{
			m_avoidTarget = m_bigTransform.position + new Vector3d(vector3d.x * (double)m_avoidDistanceLength, vector3d.y * (double)m_avoidDistanceLength, 0f - m_avoidDistanceOffset);
		}
	}

	private void OnDisable()
	{
		StopAttack();
		ResetLockPosition();
	}

	private void OnDestroy()
	{
		StopAttack();
		ResetLockPosition();
	}

	protected virtual bool CanAttack()
	{
		if (m_canAttack)
		{
			return m_canAttack;
		}
		m_canAttack = Singleton<FighterCoordinator>.Instance.GetAllowedAttack(m_fighterPlane);
		return m_canAttack;
	}

	protected virtual void StopAttack()
	{
		if (Singleton<FighterCoordinator>.Instance != null)
		{
			Singleton<FighterCoordinator>.Instance.SetNoLongerAttacking(m_fighterPlane);
		}
		m_canAttack = false;
	}

	public bool IsEngaged()
	{
		if (m_fighterPlane.IsDestroyed())
		{
			return false;
		}
		FighterState currentState = m_currentState;
		if (currentState == FighterState.Leaving || currentState == FighterState.CircleSpawnPoint)
		{
			return false;
		}
		return true;
	}

	public float GetVisibilityCountdown()
	{
		return m_visibilityCountdown;
	}

	protected void SetLockPosition(Vector3 v, FighterCoordinator.AttackPositions ap)
	{
		ResetLockPosition();
		Singleton<FighterCoordinator>.Instance.RegisterForAttackPosition(ap);
		m_currentlyLockedPositionVector = v;
		m_currentlyLockedPosition = ap;
		m_hasCurrentlyLockedPosition = true;
		m_unlockTimer = 30f;
	}

	protected void ResetLockPosition()
	{
		if (m_hasCurrentlyLockedPosition)
		{
			Singleton<FighterCoordinator>.Instance.FreeUpAttackPosition(m_currentlyLockedPosition);
		}
		m_hasCurrentlyLockedPosition = false;
	}

	public virtual void SetWasLaunched()
	{
		m_currentState = FighterState.KeepingRange;
	}

	public virtual void UpdateWithState(FighterState fs)
	{
		if (m_alwaysAttackBroken)
		{
			m_fighterPlane.SetFromArea(null, m_fighterPlane.ShouldHunt());
		}
		BomberState bomberState = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState();
		Vector3d vector3d = m_bigTransform.position - bomberState.GetBigTransform().position;
		if (Singleton<VisibilityHelpers>.Instance.IsVisibleAILenient(base.transform.position, bomberState.transform.position, Singleton<BomberSpawn>.Instance.GetBomberSystems().IsLitUp()))
		{
			m_visibilityCountdown = 10f;
		}
		else
		{
			m_visibilityCountdown -= Time.deltaTime;
		}
		float num = (m_fighterPlane.HasWing() ? m_fighterPlane.GetFighterWing().GetVisibilityCountdown() : m_visibilityCountdown);
		switch (fs)
		{
		case FighterState.CircleSpawnPoint:
			Freeze();
			if (num > 0f)
			{
				m_currentState = FighterState.KeepingRange;
				m_currentTimer = 0f;
			}
			else if (m_fighterPlane.ShouldHunt() && vector3d.magnitude < (double)m_switchToEngagedRange)
			{
				m_currentState = FighterState.KeepingRange;
				m_currentTimer = 0f;
			}
			break;
		case FighterState.KeepingRange:
			MaintainDistance();
			m_currentTimer += Time.deltaTime;
			if (!(m_currentTimer > m_maintainDistanceTime))
			{
				break;
			}
			if (num > 0f || m_fighterPlane.ShouldHunt())
			{
				if (!m_fighterPlane.IsScared() && CanAttack())
				{
					m_currentState = FighterState.AttackFormation;
					m_currentTimer = 0f;
					GetFiringTarget();
				}
			}
			else
			{
				m_currentState = FighterState.CircleSpawnPoint;
				m_currentTimer = 0f;
			}
			break;
		case FighterState.AttackFormation:
			AttackFormed();
			if (vector3d.magnitude < (double)m_breakFormationRange)
			{
				m_currentState = FighterState.BreakFormation;
			}
			if (m_currentTimer > m_maxFireTime || m_fighterPlane.IsScared())
			{
				m_fighterPlane.StopFiringGuns();
				m_currentState = FighterState.ReturningToDistance;
				m_currentTimer = 0f;
				SetAvoidTarget();
			}
			break;
		case FighterState.BreakFormation:
			AttackBroken();
			if (m_currentTimer > m_maxFireTime || vector3d.magnitude < (double)m_closestRange || m_fighterPlane.IsScared())
			{
				m_fighterPlane.StopFiringGuns();
				m_currentState = FighterState.ReturningToDistance;
				m_currentTimer = 0f;
				SetAvoidTarget();
			}
			break;
		case FighterState.ReturningToDistance:
			ReturnToDistance();
			GetFiringTarget();
			m_currentTimer += Time.deltaTime;
			if (m_totalAttackingTime > m_maxAttackTime)
			{
				Leave(becauseOfTimeOut: true);
			}
			if (!(m_currentTimer > m_timeBeforeReform))
			{
				break;
			}
			if (num > 0f)
			{
				m_currentState = FighterState.Reforming;
				m_currentTimer = 0f;
				break;
			}
			if (m_fighterPlane.ShouldHunt())
			{
				m_currentState = FighterState.CircleSpawnPoint;
			}
			else
			{
				m_currentState = FighterState.KeepingRange;
			}
			m_currentTimer = 0f;
			break;
		case FighterState.Leaving:
			if (m_canAttack)
			{
				StopAttack();
			}
			ReturnToDistanceEurope();
			m_currentTimer += Time.deltaTime;
			break;
		case FighterState.Reforming:
		{
			Reform();
			m_currentTimer += Time.deltaTime;
			FighterWing fighterWing = m_fighterPlane.GetFighterWing();
			if (!m_fighterPlane.HasWing() || fighterWing.GetLeadFighter() == m_fighterPlane)
			{
				if (m_currentTimer > m_timeToReform && IsRoughlyFacingForAttack())
				{
					m_currentState = FighterState.AttackFormation;
					m_currentTimer = 0f;
				}
			}
			else if (!fighterWing.GetLeadFighter().GetAI().IsReforming())
			{
				m_currentState = FighterState.AttackFormation;
				m_currentTimer = 0f;
			}
			break;
		}
		}
	}

	public FighterState GetAIState()
	{
		return m_currentState;
	}

	private void Update()
	{
		BomberState bomberState = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState();
		m_aimAheadPhase += Time.deltaTime * m_aimAheadInaccuracyFrequency;
		if (m_aimAheadPhase > 1f)
		{
			m_aimAheadPhase -= 1f;
		}
		m_unlockTimer -= Time.deltaTime;
		if (m_unlockTimer < 0f)
		{
			ResetLockPosition();
		}
		m_inaccuracyCached = AimingUtils.GetInaccuracyFighter(Singleton<VisibilityHelpers>.Instance.GetNightFactor(), bomberState.IsEvasive(), bomberState.IsLitUp(), m_aimAheadPhase) * m_inaccuracyScaleMultiplier;
		if (m_debugMarker1 != null)
		{
			m_debugMarker1.localPosition = Vector3.zero;
			m_debugMarker2.localPosition = Vector3.zero;
		}
		if (m_currentState != 0 && m_currentState != FighterState.KeepingRange)
		{
			if (m_fighterPlane.CanSuperScare())
			{
				m_totalAttackingTime += Time.deltaTime;
			}
		}
		else if (m_currentState == FighterState.KeepingRange && m_fighterPlane.CanSuperScare())
		{
			m_totalAttackingTime += Time.deltaTime * 0.5f;
		}
		if (m_currentState != 0 && m_currentState != FighterState.KeepingRange && m_currentState != FighterState.Leaving)
		{
			if (bomberState.IsAboveEngland() && !m_testAiFighter)
			{
				m_totalAttackingTime += Time.deltaTime;
				if (!Singleton<MissionCoordinator>.Instance.IsOutwardJourney())
				{
					Leave(becauseOfTimeOut: false);
				}
			}
			if (m_fighterPlane.IsSuperScared())
			{
				Leave(becauseOfTimeOut: true);
			}
		}
		if (m_currentState == FighterState.Leaving && !m_leftBecauseOfTimeOut && Singleton<MissionCoordinator>.Instance.IsOutwardJourney())
		{
			m_currentState = FighterState.KeepingRange;
		}
		UpdateWithState(m_currentState);
	}

	public void Leave(bool becauseOfTimeOut)
	{
		if (!m_leftBecauseOfTimeOut)
		{
			m_leftBecauseOfTimeOut = becauseOfTimeOut;
		}
		StopAttack();
		m_currentState = FighterState.Leaving;
		m_currentTimer = 0f;
	}

	public bool IsReforming()
	{
		return m_currentState == FighterState.Reforming;
	}

	public bool IsRoughlyFacingForAttack()
	{
		BomberState bomberState = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState();
		if (Vector3.Dot(rhs: -(Vector3)(m_bigTransform.position - bomberState.GetBigTransform().position).normalized, lhs: m_fighterPlane.GetDynamics().GetCurrentHeading().normalized) > 0.5f)
		{
			return true;
		}
		return false;
	}

	public void FollowWing(float catchUp)
	{
		FighterWing fighterWing = m_fighterPlane.GetFighterWing();
		FighterPlane leadFighter = fighterWing.GetLeadFighter();
		int myFighterIndex = fighterWing.GetMyFighterIndex(m_fighterPlane);
		Vector3 vector = m_formationOffsets[(myFighterIndex - 1) % m_formationOffsets.Length] * (1 + myFighterIndex / m_formationOffsets.Length);
		Vector3 vector2 = Vector3.Cross(leadFighter.GetDynamics().GetCurrentHeading(), Vector3.up);
		float magnitude = (leadFighter.transform.position - base.transform.position).magnitude;
		float velocity = Mathf.Clamp(magnitude / 60f, 0.05f, 1f);
		m_controls.SetTargetPositionAndHeading(leadFighter.transform.position + vector2 * vector.x, leadFighter.GetDynamics().GetCurrentHeading(), velocity, catchUp);
	}

	public void Freeze()
	{
		m_controls.SetFrozen();
	}

	public void MaintainDistance()
	{
		FighterWing fighterWing = m_fighterPlane.GetFighterWing();
		if (!m_fighterPlane.HasWing() || fighterWing.GetLeadFighter() == m_fighterPlane || fighterWing.GetLeadFighter() == null || fighterWing.GetLeadFighter().IsDestroyed() || fighterWing.GetLeadFighter().GetAI().IsLeaving())
		{
			BomberState bomberState = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState();
			Vector3d vector3d = m_bigTransform.position - bomberState.GetBigTransform().position;
			Vector3 normalized = ((Vector3)(-vector3d.normalized) + bomberState.GetPhysicsModel().GetVelocity().normalized).normalized;
			Vector3 pos = bomberState.transform.position + (Vector3)(vector3d.normalized * m_keepDistanceLength);
			m_controls.SetTargetPositionAndHeading(pos, normalized, 1f, m_catchUpFactor);
		}
		else
		{
			FollowWing(m_catchUpFactor);
		}
	}

	public void Reform()
	{
		FighterWing fighterWing = m_fighterPlane.GetFighterWing();
		if (!m_fighterPlane.HasWing() || fighterWing.GetLeadFighter() == m_fighterPlane || fighterWing.GetLeadFighter() == null || fighterWing.GetLeadFighter().IsDestroyed() || fighterWing.GetLeadFighter().GetAI().IsLeaving())
		{
			BomberState bomberState = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState();
			Vector3d vector3d = m_bigTransform.position - bomberState.GetBigTransform().position;
			Vector3 normalized = ((Vector3)(-vector3d.normalized) + bomberState.GetPhysicsModel().GetVelocity().normalized).normalized;
			Vector3 pos = bomberState.transform.position + (Vector3)(vector3d.normalized * m_keepDistanceLength);
			m_controls.SetTargetPositionAndHeading(pos, normalized, 1f, m_catchUpFactor);
		}
		else
		{
			FollowWing(m_catchUpFactor);
		}
	}

	public void ReturnToDistance()
	{
		BomberState bomberState = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState();
		Vector3 heading = Vector3.Lerp((Vector3)(m_bigTransform.position - bomberState.GetBigTransform().position).normalized, bomberState.GetPhysicsModel().GetVelocity().normalized, 0.3f);
		m_controls.SetHeading(heading, 1f, m_catchUpFactor);
	}

	public void ReturnToDistanceEurope()
	{
		BomberState bomberState = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState();
		Vector3d vector3d = m_bigTransform.position - bomberState.GetBigTransform().position;
		Vector3 vector = Vector3.Lerp((Vector3)vector3d.normalized, bomberState.GetPhysicsModel().GetVelocity().normalized, 0.3f);
		vector.y = 0f;
		if (vector3d.magnitude > 100.0)
		{
			vector = new Vector3(1f, 0f, 1f).normalized;
		}
		m_controls.SetHeading(vector, 1f, Mathf.Clamp01(Vector3.Dot(bomberState.GetVelocity().normalized, vector)));
		if (vector3d.magnitude > 3500.0)
		{
			Object.Destroy(base.gameObject);
		}
	}

	public bool IsLeaving()
	{
		return m_currentState == FighterState.Leaving;
	}

	public void AttackFormed()
	{
		FighterWing fighterWing = m_fighterPlane.GetFighterWing();
		if (!m_fighterPlane.HasWing() || fighterWing.GetLeadFighter() == m_fighterPlane || fighterWing.GetLeadFighter() == null || fighterWing.GetLeadFighter().IsDestroyed() || fighterWing.GetLeadFighter().GetAI().IsLeaving() || m_alwaysAttackBroken)
		{
			AttackBroken();
			return;
		}
		FollowWing(1f);
		if (fighterWing.GetLeadFighter().GetComponent<FighterAI>().GetState() != FighterState.AttackFormation)
		{
			m_currentState = FighterState.BreakFormation;
		}
		BomberState bomberState = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState();
		Vector3 vector = base.transform.position - m_currentFiringTarget.position;
		Vector3 vector2 = vector;
		if (m_alternateAimAhead)
		{
			Vector3 aimAheadTarget = AimingUtils.GetAimAheadTarget(m_gunFiringNodeAverage.position, m_currentFiringTarget.position, bomberState.GetPhysicsModel().GetVelocity(), m_fighterPlane.GetMuzzleVelocity() + m_fighterPlane.GetCachedVelocity().magnitude, m_inaccuracyCached, isGroundBasedOrFriendly: false);
			aimAheadTarget += m_regularAimAheadVelFudge * bomberState.GetPhysicsModel().GetVelocity();
			m_debugMarker1.position = m_currentFiringTarget.position;
			m_debugMarker2.position = aimAheadTarget;
			vector = base.transform.position - Vector3.LerpUnclamped(base.transform.position, aimAheadTarget, m_alternateAimAheadMultiplier);
		}
		else
		{
			Vector3 aimAheadTarget2 = AimingUtils.GetAimAheadTarget(m_gunFiringNodeAverage.position, m_currentFiringTarget.position, bomberState.GetPhysicsModel().GetVelocity() - m_fighterPlane.GetCachedVelocity(), m_fighterPlane.GetMuzzleVelocity(), m_inaccuracyCached, isGroundBasedOrFriendly: false);
			aimAheadTarget2 += m_regularAimAheadVelFudge * bomberState.GetPhysicsModel().GetVelocity();
			m_debugMarker1.position = m_currentFiringTarget.position;
			m_debugMarker2.position = aimAheadTarget2;
			vector = base.transform.position - aimAheadTarget2;
			vector2 = vector;
		}
		Vector3 lhs = -vector.normalized;
		bool flag = bomberState.IsEvasive();
		if (Vector3.Dot(lhs, m_controls.GetCurrentHeading()) > 0.95f && vector2.magnitude < m_firingRange)
		{
			if (!m_testAiDontFire)
			{
				m_fighterPlane.StartFiringGuns();
			}
			else
			{
				m_fighterPlane.StopFiringGuns();
			}
			m_currentTimer += Time.deltaTime;
		}
		else
		{
			m_fighterPlane.StopFiringGuns();
		}
	}

	public void AttackBroken()
	{
		BomberState bomberState = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState();
		Vector3 vector = base.transform.position - m_currentFiringTarget.position;
		Vector3 vector2 = vector;
		if (m_alternateAimAhead)
		{
			Vector3 aimAheadTarget = AimingUtils.GetAimAheadTarget(m_gunFiringNodeAverage.position, m_currentFiringTarget.position, bomberState.GetPhysicsModel().GetVelocity(), m_fighterPlane.GetMuzzleVelocity() + m_fighterPlane.GetCachedVelocity().magnitude, m_inaccuracyCached, isGroundBasedOrFriendly: false);
			aimAheadTarget += m_regularAimAheadVelFudge * bomberState.GetPhysicsModel().GetVelocity();
			vector = base.transform.position - Vector3.LerpUnclamped(base.transform.position, aimAheadTarget, m_alternateAimAheadMultiplier);
			m_debugMarker1.position = m_currentFiringTarget.position;
			m_debugMarker2.position = aimAheadTarget;
		}
		else
		{
			Vector3 aimAheadTarget2 = AimingUtils.GetAimAheadTarget(m_gunFiringNodeAverage.transform.position, m_currentFiringTarget.position, bomberState.GetPhysicsModel().GetVelocity() - m_fighterPlane.GetCachedVelocity(), m_fighterPlane.GetMuzzleVelocity(), m_inaccuracyCached, isGroundBasedOrFriendly: false);
			aimAheadTarget2 += m_regularAimAheadVelFudge * bomberState.GetPhysicsModel().GetVelocity();
			vector = base.transform.position - aimAheadTarget2;
			vector2 = vector;
			m_debugMarker1.position = m_currentFiringTarget.position;
			m_debugMarker2.position = aimAheadTarget2;
		}
		Vector3 vector3 = -vector.normalized;
		m_controls.SetHeading(vector3, m_breakAttackSpeedMultiplier, 1f);
		bool flag = bomberState.IsEvasive();
		if (Vector3.Dot(vector3, m_controls.GetCurrentHeading()) > 0.95f && vector2.magnitude < m_firingRange)
		{
			if (!m_testAiDontFire)
			{
				m_fighterPlane.StartFiringGuns();
			}
			else
			{
				m_fighterPlane.StopFiringGuns();
			}
			m_currentTimer += Time.deltaTime;
		}
		else
		{
			m_fighterPlane.StopFiringGuns();
		}
	}

	public FighterState GetState()
	{
		return m_currentState;
	}
}
