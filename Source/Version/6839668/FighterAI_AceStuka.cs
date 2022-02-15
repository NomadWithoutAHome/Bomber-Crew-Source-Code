using BomberCrewCommon;
using UnityEngine;
using WingroveAudio;

public class FighterAI_AceStuka : FighterAI
{
	[SerializeField]
	private float m_attackDistance = 1000f;

	[SerializeField]
	private float m_attackHeight = 400f;

	[SerializeField]
	private float m_breakAttackSpeedMultiplierStuka;

	[SerializeField]
	private float m_fireRange = 500f;

	[SerializeField]
	private int m_maxAttacks = 10;

	private bool m_isOnGround;

	private bool m_shouldChargeRockets;

	private float m_timeAfterRocketsGone;

	private int m_attackCounter;

	private bool m_isPlayingDive;

	private bool m_shouldPlayDive;

	public override void UpdateWithState(FighterState fs)
	{
		m_shouldPlayDive = false;
		m_shouldChargeRockets = false;
		switch (fs)
		{
		case FighterState.KeepingRange:
		case FighterState.Reforming:
			FlyAboveBomber();
			break;
		case FighterState.AttackFormation:
		case FighterState.BreakFormation:
			AttackBrokenHigh();
			break;
		default:
			ResetLockPosition();
			base.UpdateWithState(fs);
			break;
		}
		if (m_shouldPlayDive != m_isPlayingDive)
		{
			m_isPlayingDive = m_shouldPlayDive;
			if (m_shouldPlayDive)
			{
				WingroveRoot.Instance.PostEventGO("STUKA_START", base.gameObject);
			}
			else
			{
				WingroveRoot.Instance.PostEventGO("STUKA_STOP", base.gameObject);
			}
		}
	}

	public void AttackBrokenHigh()
	{
		bool flag = false;
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
		m_controls.SetHeading(vector3, m_breakAttackSpeedMultiplierStuka, 1f);
		bool flag2 = bomberState.IsEvasive();
		if (Vector3.Dot(vector3, m_controls.GetCurrentHeading()) > 0.95f && vector2.magnitude < m_fireRange)
		{
			flag = true;
			m_currentTimer += Time.deltaTime;
		}
		if (vector2.magnitude < 100f)
		{
			m_attackCounter++;
			m_currentTimer = 0f;
			m_currentState = FighterState.ReturningToDistance;
			SetAvoidTarget();
		}
		if (flag)
		{
			if (vector3.y < 0.2f)
			{
				m_shouldPlayDive = true;
			}
			m_fighterPlane.StartFiringGuns();
		}
		else
		{
			m_fighterPlane.StopFiringGuns();
		}
	}

	private void FlyAboveBomber()
	{
		Vector3 position = Singleton<BomberSpawn>.Instance.GetBomberSystems().transform.position;
		Vector3 vector = position - base.transform.position;
		Vector3 vector2 = vector;
		vector2.y = 0f;
		Vector3 vector3 = position - vector2.normalized * m_attackDistance;
		vector3.y += m_attackHeight;
		Vector3 vector4 = vector3 - base.transform.position;
		m_fighterPlane.GetDynamics().SetHeading(vector4.normalized, 1f, 1f);
		if (base.transform.position.y > vector3.y)
		{
			m_currentState = FighterState.BreakFormation;
		}
	}
}
