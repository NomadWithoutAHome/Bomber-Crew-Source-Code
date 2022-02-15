using BomberCrewCommon;
using UnityEngine;

public class FighterAI_AceBasic : FighterAI
{
	[SerializeField]
	private Animation m_trickAnimationController;

	[SerializeField]
	private AnimationClip[] m_dodgeAnimations;

	[SerializeField]
	private AnimationClip[] m_trickAnimations;

	[SerializeField]
	private float m_trickTimerMin;

	[SerializeField]
	private float m_trickTimerMax;

	[SerializeField]
	private AceFighterInMission m_aceDetails;

	[SerializeField]
	private float m_aceFighterIntroRange = 400f;

	[SerializeField]
	private float m_aceFighterAttackRange = 500f;

	[SerializeField]
	private float m_aceFighterReturnToDistanceRange = 400f;

	[SerializeField]
	private float m_aceAttackSpeedMultiplier = 1f;

	[SerializeField]
	private float m_aceRetreatSpeedMultiplier = 1f;

	[SerializeField]
	private float m_aceDistanceTimerFirst = 15f;

	[SerializeField]
	private float m_aceDistanceTimerHeadstart = 10f;

	[SerializeField]
	private float m_maxTimeBeforeRunAway;

	[SerializeField]
	private ProjectileType[] m_cycleProjectileTypes;

	[SerializeField]
	private RaycastGunTurret[] m_gunsToCycle;

	private float m_aceFighterAITimer;

	private float m_trickTimer;

	private float m_totalTimer;

	private int m_currentProjectileCycleType;

	protected override bool CanAttack()
	{
		return true;
	}

	protected override void StopAttack()
	{
	}

	protected void DoRunAwayCheck()
	{
		if (m_fighterPlane.GetHealthNormalised() < 0.35f && !m_aceDetails.HasBeenEncounteredPreviously() && Singleton<GameFlow>.Instance.GetGameMode().AllowAceRunawayLowHealth())
		{
			m_leftBecauseOfTimeOut = true;
			m_currentState = FighterState.Leaving;
		}
		else if (m_totalTimer > m_maxTimeBeforeRunAway)
		{
			m_leftBecauseOfTimeOut = true;
			m_currentState = FighterState.Leaving;
		}
		else if (m_totalTimer > m_maxTimeBeforeRunAway / 2f && Singleton<DifficultyMagic>.Instance.GetAceShouldGo())
		{
			m_leftBecauseOfTimeOut = true;
			m_currentState = FighterState.Leaving;
		}
	}

	public override void UpdateWithState(FighterState fs)
	{
		if (m_currentFiringTarget == null)
		{
			GetFiringTarget();
		}
		BomberState bomberState = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState();
		Vector3 vector = base.transform.position - m_currentFiringTarget.position;
		Vector3 vector2 = vector;
		vector2.y = 0f;
		switch (fs)
		{
		case FighterState.KeepingRange:
			DoAceFighterIntroRange();
			m_aceFighterAITimer += Time.deltaTime;
			if (m_aceFighterAITimer > m_aceDistanceTimerFirst)
			{
				m_currentState = FighterState.AttackFormation;
				if (m_cycleProjectileTypes.Length > 0)
				{
					RaycastGunTurret[] gunsToCycle = m_gunsToCycle;
					foreach (RaycastGunTurret raycastGunTurret in gunsToCycle)
					{
						raycastGunTurret.SetProjectileType(m_cycleProjectileTypes[m_currentProjectileCycleType]);
					}
					m_currentProjectileCycleType++;
					if (m_currentProjectileCycleType == m_cycleProjectileTypes.Length)
					{
						m_currentProjectileCycleType = 0;
					}
				}
				m_aceFighterAITimer = 0f;
			}
			m_totalTimer += Time.deltaTime;
			break;
		case FighterState.AttackFormation:
		case FighterState.BreakFormation:
			m_totalTimer += Time.deltaTime;
			DoAceFighterAttack();
			if (vector.magnitude < 100f)
			{
				SetAvoidTarget();
				m_fighterPlane.StopFiringGuns();
				m_currentState = FighterState.ReturningToDistance;
				m_aceFighterAITimer = 0f;
				m_currentFiringTarget = null;
				DoRunAwayCheck();
			}
			break;
		case FighterState.ReturningToDistance:
			m_totalTimer += Time.deltaTime;
			DoAceReturnToDistance();
			if (vector2.magnitude > m_aceFighterReturnToDistanceRange)
			{
				m_currentFiringTarget = null;
				m_currentState = FighterState.KeepingRange;
				m_aceFighterAITimer = m_aceDistanceTimerHeadstart;
			}
			break;
		default:
			base.UpdateWithState(fs);
			break;
		}
	}

	private void DoAceReturnToDistance()
	{
		BomberState bomberState = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState();
		Vector3 heading = Vector3.Lerp((Vector3)(m_bigTransform.position - bomberState.GetBigTransform().position).normalized, bomberState.GetPhysicsModel().GetVelocity().normalized, 0.3f);
		if (base.transform.position.y < 200f)
		{
			heading.y = 1f;
			heading = heading.normalized;
		}
		TryDoTrick(m_trickAnimations);
		m_controls.SetHeading(heading, m_aceRetreatSpeedMultiplier, 0.25f);
	}

	private void TryDoTrick(AnimationClip[] animList)
	{
		if (!m_trickAnimationController.isPlaying)
		{
			m_trickTimer -= Time.deltaTime;
			if (m_trickTimer < 0f)
			{
				m_trickTimer = Random.Range(m_trickTimerMin, m_trickTimerMax);
				m_trickAnimationController.Play(animList[Random.Range(0, animList.Length)].name);
			}
		}
	}

	private void DoAceFighterAttack()
	{
		BomberState bomberState = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState();
		Vector3 vector = base.transform.position - m_currentFiringTarget.position;
		if (m_alternateAimAhead)
		{
			Vector3 aimAheadTarget = AimingUtils.GetAimAheadTarget(m_gunFiringNodeAverage.position, m_currentFiringTarget.position, bomberState.GetPhysicsModel().GetVelocity(), m_fighterPlane.GetMuzzleVelocity() + m_fighterPlane.GetVelocity().magnitude);
			aimAheadTarget += m_regularAimAheadVelFudge * bomberState.GetPhysicsModel().GetVelocity();
			m_debugMarker1.position = m_currentFiringTarget.position;
			m_debugMarker2.position = aimAheadTarget;
			if (aimAheadTarget.y < 120f)
			{
				aimAheadTarget.y = 120f;
			}
			vector = base.transform.position - Vector3.LerpUnclamped(base.transform.position, aimAheadTarget, m_alternateAimAheadMultiplier);
		}
		else
		{
			Vector3 aimAheadTarget2 = AimingUtils.GetAimAheadTarget(m_gunFiringNodeAverage.position, m_currentFiringTarget.position, bomberState.GetPhysicsModel().GetVelocity() - m_fighterPlane.GetCachedVelocity(), m_fighterPlane.GetMuzzleVelocity());
			aimAheadTarget2 += m_regularAimAheadVelFudge * bomberState.GetPhysicsModel().GetVelocity();
			if (aimAheadTarget2.y < 120f)
			{
				aimAheadTarget2.y = 120f;
			}
			m_debugMarker1.position = m_currentFiringTarget.position;
			m_debugMarker2.position = aimAheadTarget2;
			vector = base.transform.position - aimAheadTarget2;
		}
		TryDoTrick(m_dodgeAnimations);
		Vector3 vector2 = -vector.normalized;
		m_controls.SetHeading(vector2, m_aceAttackSpeedMultiplier, 1f);
		if (Vector3.Dot(vector2, m_controls.GetCurrentHeading()) > 0.95f && vector.magnitude < m_aceFighterAttackRange)
		{
			m_fighterPlane.StartFiringGuns();
			m_currentTimer += Time.deltaTime;
		}
		else
		{
			m_fighterPlane.StopFiringGuns();
		}
	}

	private void DoAceFighterIntroRange()
	{
		BomberState bomberState = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState();
		Vector3d vector3d = m_bigTransform.position - bomberState.GetBigTransform().position;
		Vector3 normalized = ((Vector3)(-vector3d.normalized) + bomberState.GetPhysicsModel().GetVelocity().normalized).normalized;
		Vector3 pos = bomberState.transform.position + (Vector3)(vector3d.normalized * m_aceFighterIntroRange);
		pos.y = Mathf.Max(pos.y, 200f);
		TryDoTrick(m_trickAnimations);
		m_controls.SetTargetPositionAndHeading(pos, normalized, m_aceRetreatSpeedMultiplier, 0.25f);
	}
}
