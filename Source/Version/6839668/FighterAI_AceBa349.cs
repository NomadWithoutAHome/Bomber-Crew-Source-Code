using BomberCrewCommon;
using UnityEngine;

public class FighterAI_AceBa349 : FighterAI
{
	[SerializeField]
	private float m_rocketAttackDistance = 1000f;

	[SerializeField]
	private float m_rocketAttackHeight = 400f;

	[SerializeField]
	private FighterRocketLauncher[] m_rocketLaunchers;

	[SerializeField]
	private float m_breakAttackSpeedMultiplierBa349;

	[SerializeField]
	private float m_rocketFireRange = 500f;

	[SerializeField]
	private int m_maxAttacks = 10;

	private bool m_isOnGround;

	private bool m_shouldChargeRockets;

	private float m_timeAfterRocketsGone;

	private int m_attackCounter;

	public override void UpdateWithState(FighterState fs)
	{
		m_shouldChargeRockets = false;
		switch (fs)
		{
		case FighterState.KeepingRange:
			FlyAboveBomber();
			break;
		case FighterState.AttackFormation:
		case FighterState.BreakFormation:
			AttackBrokenRocket();
			break;
		default:
			ResetLockPosition();
			base.UpdateWithState(fs);
			break;
		}
		bool flag = true;
		FighterRocketLauncher[] rocketLaunchers = m_rocketLaunchers;
		foreach (FighterRocketLauncher fighterRocketLauncher in rocketLaunchers)
		{
			if (!fighterRocketLauncher.DidFire())
			{
				fighterRocketLauncher.SetInheritedVelocity(m_fighterPlane.GetCachedVelocity());
				fighterRocketLauncher.SetCharging(m_shouldChargeRockets);
				flag = false;
			}
		}
		if (flag)
		{
			m_timeAfterRocketsGone += Time.deltaTime;
		}
		if (m_timeAfterRocketsGone > 15f && m_currentState == FighterState.BreakFormation)
		{
			m_attackCounter++;
			m_currentTimer = 0f;
			m_currentState = FighterState.ReturningToDistance;
			SetAvoidTarget();
			m_timeAfterRocketsGone = 0f;
			FighterRocketLauncher[] rocketLaunchers2 = m_rocketLaunchers;
			foreach (FighterRocketLauncher fighterRocketLauncher2 in rocketLaunchers2)
			{
				fighterRocketLauncher2.SetCharging(charging: false);
				fighterRocketLauncher2.ResetDidFireTrigger();
			}
			if (m_attackCounter == m_maxAttacks)
			{
				Leave(becauseOfTimeOut: true);
			}
		}
	}

	public void AttackBrokenRocket()
	{
		BomberState bomberState = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState();
		if (m_currentFiringTarget == null)
		{
			GetFiringTarget();
		}
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
		m_controls.SetHeading(vector3, m_breakAttackSpeedMultiplierBa349, 1f);
		bool flag = bomberState.IsEvasive();
		if (Vector3.Dot(vector3, m_controls.GetCurrentHeading()) > 0.95f && vector2.magnitude < m_rocketFireRange)
		{
			m_shouldChargeRockets = true;
			m_currentTimer += Time.deltaTime;
		}
		if (vector2.magnitude < 100f)
		{
			m_attackCounter++;
			FighterRocketLauncher[] rocketLaunchers = m_rocketLaunchers;
			foreach (FighterRocketLauncher fighterRocketLauncher in rocketLaunchers)
			{
				fighterRocketLauncher.SetCharging(charging: false);
				fighterRocketLauncher.ResetDidFireTrigger();
			}
			m_currentTimer = 0f;
			m_currentState = FighterState.ReturningToDistance;
			SetAvoidTarget();
		}
	}

	private void FlyAboveBomber()
	{
		Vector3 position = Singleton<BomberSpawn>.Instance.GetBomberSystems().transform.position;
		Vector3 vector = position - base.transform.position;
		Vector3 vector2 = vector;
		vector2.y = 0f;
		Vector3 vector3 = position - vector2.normalized * m_rocketAttackDistance;
		vector3.y += m_rocketAttackHeight;
		Vector3 vector4 = vector3 - base.transform.position;
		m_fighterPlane.GetDynamics().SetHeading(vector4.normalized, 1f, 1f);
		if (base.transform.position.y > vector3.y)
		{
			m_currentState = FighterState.BreakFormation;
		}
	}
}
