using BomberCrewCommon;
using UnityEngine;

public class FighterAI_AceHo229 : FighterAI
{
	[SerializeField]
	private bool m_alwaysSelectUnder;

	[SerializeField]
	private float m_approachFromBehindDistance = 30f;

	[SerializeField]
	private float m_formPositionLength = 30f;

	[SerializeField]
	private float m_rocketFireAttackChance = 0.4f;

	[SerializeField]
	private FighterRocketLauncher m_rocketLauncherStandard;

	[SerializeField]
	private FighterRocketLauncher[] m_rocketLauncherHoming;

	[SerializeField]
	private Animation m_trickAnimationController;

	[SerializeField]
	private AnimationClip[] m_chargingAnimation;

	[SerializeField]
	private AnimationClip[] m_trickAnims;

	[SerializeField]
	private float m_trickTimerMin;

	[SerializeField]
	private float m_trickTimerMax;

	[SerializeField]
	private AceFighterInMission m_aceDetails;

	[SerializeField]
	private float m_minTimeBeforeHomingLaunch = 30f;

	private float m_forwardMovement;

	private bool m_inAttackFormation;

	private bool m_doingRocketAttack;

	private bool m_rightHanded;

	private float m_alignStability;

	private float m_underneathTimer;

	private float m_trickTimer;

	private bool m_hasStartedRocketCharge;

	private int m_homingRocketLauncherIndex;

	private bool m_isDoingMissileAttack;

	private float m_rocketAttackChargeTime;

	private float m_timer;

	protected override bool CanAttack()
	{
		return true;
	}

	protected override void StopAttack()
	{
	}

	private void TryDoTrick(AnimationClip[] animList, bool force)
	{
		if (!m_trickAnimationController.isPlaying)
		{
			m_trickTimer -= Time.deltaTime;
			if (m_trickTimer < 0f || force)
			{
				m_trickTimer = Random.Range(m_trickTimerMin, m_trickTimerMax);
				m_trickAnimationController.Play(animList[Random.Range(0, animList.Length)].name);
			}
		}
	}

	public override void UpdateWithState(FighterState fs)
	{
		if (fs != 0)
		{
			m_timer += Time.deltaTime;
		}
		if (fs == FighterState.AttackFormation || fs == FighterState.BreakFormation)
		{
			if (!m_inAttackFormation)
			{
				m_inAttackFormation = true;
				m_hasStartedRocketCharge = false;
				m_rocketLauncherStandard.ResetDidFireTrigger();
				m_doingRocketAttack = Random.Range(0f, 1f) < m_rocketFireAttackChance;
				m_rocketAttackChargeTime = 0f;
				if (m_doingRocketAttack)
				{
					TryLockPosition();
					if (!m_hasCurrentlyLockedPosition)
					{
						m_doingRocketAttack = false;
					}
				}
				else
				{
					m_isDoingMissileAttack = !m_isDoingMissileAttack;
				}
			}
			if (m_doingRocketAttack)
			{
				MatchSpeedFlySmart();
			}
			else
			{
				m_currentState = FighterState.BreakFormation;
				base.UpdateWithState(m_currentState);
				if (m_isDoingMissileAttack)
				{
					CheckFireMissile();
					m_fighterPlane.StopFiringGuns();
				}
				if (m_currentState == FighterState.Leaving)
				{
					m_leftBecauseOfTimeOut = false;
					m_currentState = FighterState.BreakFormation;
				}
			}
			if (m_fighterPlane.IsScared())
			{
				m_currentState = FighterState.ReturningToDistance;
				m_rocketLauncherStandard.SetCharging(charging: false);
				m_currentTimer = 0f;
			}
			return;
		}
		m_rocketLauncherStandard.SetCharging(charging: false);
		FighterRocketLauncher[] rocketLauncherHoming = m_rocketLauncherHoming;
		foreach (FighterRocketLauncher fighterRocketLauncher in rocketLauncherHoming)
		{
			if (fighterRocketLauncher.DidFire())
			{
				Singleton<MissionCoordinator>.Instance.FireTrigger("ACE_HOMING_MISSILE");
				m_homingRocketLauncherIndex++;
			}
			fighterRocketLauncher.SetCharging(charging: false);
			fighterRocketLauncher.ResetDidFireTrigger();
		}
		ResetLockPosition();
		m_inAttackFormation = false;
		base.UpdateWithState(fs);
	}

	public void TryLockPosition()
	{
		if (Singleton<FighterCoordinator>.Instance.IsAttackPositionAvailable(FighterCoordinator.AttackPositions.Tailing))
		{
			SetLockPosition(new Vector3(0f, 0f, -3f), FighterCoordinator.AttackPositions.Tailing);
		}
	}

	private float Magnitude2D(Vector3 d)
	{
		return new Vector3(d.x, 0f, d.z).magnitude;
	}

	private void CheckFireMissile()
	{
		if (m_timer > m_minTimeBeforeHomingLaunch)
		{
			BomberState bomberState = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState();
			FighterRocketLauncher fighterRocketLauncher = m_rocketLauncherHoming[m_homingRocketLauncherIndex % m_rocketLauncherHoming.Length];
			Vector3 vector = bomberState.transform.position - base.transform.position;
			if (vector.magnitude > 400f && vector.magnitude < 1500f && !fighterRocketLauncher.DidFire())
			{
				fighterRocketLauncher.SetCharging(charging: true);
			}
		}
	}

	private void MatchSpeedFlySmart()
	{
		BomberState bomberState = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState();
		Vector3 velocity = bomberState.GetPhysicsModel().GetVelocity();
		Vector3 vector = velocity;
		vector.y = 0f;
		Vector3 position = bomberState.transform.position;
		float num = Vector3.Dot(m_fighterPlane.GetCachedVelocity().normalized, velocity.normalized);
		float num2 = num * 0.5f + 0.5f;
		Vector3 vector2 = base.transform.position + m_fighterPlane.GetCachedVelocity().normalized * 2f;
		float formPositionLength = m_formPositionLength;
		if (!m_hasCurrentlyLockedPosition)
		{
			TryLockPosition();
		}
		if (m_hasCurrentlyLockedPosition)
		{
			Vector3 currentlyLockedPositionVector = m_currentlyLockedPositionVector;
			Vector3 vector3 = currentlyLockedPositionVector.x * bomberState.transform.forward + currentlyLockedPositionVector.y * Vector3.up + currentlyLockedPositionVector.z * bomberState.transform.right;
			Vector3 vector4 = position + vector3 * formPositionLength;
			Vector3 vector5 = vector4 - vector.normalized * m_approachFromBehindDistance;
			float num3 = Vector3.Dot((vector4 - base.transform.position).normalized, m_fighterPlane.GetCachedVelocity().normalized);
			float num4 = Magnitude2D(vector4 - base.transform.position);
			float maxVelocity = m_controls.GetMaxVelocity();
			float t = 0f;
			float num5 = 1f;
			float catchUpHack = Mathf.Clamp01((num4 - 50f) / 75f);
			if (num > 0.9f)
			{
				m_alignStability += Time.deltaTime;
			}
			else
			{
				m_alignStability = 0f;
			}
			bool flag = false;
			Vector3 vector6;
			if (num4 > m_approachFromBehindDistance * 1.5f || num < 0.25f)
			{
				vector6 = vector5;
				catchUpHack = 1f;
			}
			else
			{
				Vector3 rhs = base.transform.position + m_fighterPlane.GetCachedVelocity() * 0.5f - vector5;
				float num6 = Vector3.Dot(vector.normalized, rhs);
				Vector3 vector7 = vector5 - (1f - num6) * vector.normalized;
				if (!(num4 < 50f))
				{
					vector6 = ((!(num3 > 0.5f) || !(num > 0.25f)) ? vector5 : vector7);
				}
				else if (num > 0.5f)
				{
					vector6 = vector7;
					if (num3 > 0f)
					{
						t = 1f;
						num5 = 1f;
						catchUpHack = 0f;
						Vector3 vector8 = vector4 - base.transform.position;
						num5 = ((!(m_alignStability > 1f)) ? (1f + Mathf.Abs(vector8.magnitude - 3f) * 0.3f) : (1f + Mathf.Abs(vector8.magnitude - 3f) * 0.5f));
						num5 += 0.02f;
					}
					else
					{
						t = 1f;
						num5 = 1f;
						catchUpHack = 0f;
						if (m_alignStability > 1f)
						{
							num5 = 1f - Mathf.Abs((vector4 - base.transform.position).magnitude - 6f) * 0.05f;
						}
					}
					if (m_alignStability > 1f)
					{
						flag = true;
					}
				}
				else
				{
					vector6 = vector5;
				}
			}
			m_controls.SetHeading((vector6 - base.transform.position).normalized, 1f, catchUpHack);
			m_controls.SetExactVelocity(Mathf.Lerp(maxVelocity, velocity.magnitude * num5, t));
			m_debugMarker1.transform.position = vector6;
			m_debugMarker2.transform.position = vector4;
			if (flag)
			{
				m_rocketAttackChargeTime += Time.deltaTime;
				m_underneathTimer += Time.deltaTime;
				if (m_rocketLauncherStandard.DidFire())
				{
					if (m_underneathTimer < 25f)
					{
						m_underneathTimer = 25f;
					}
				}
				else
				{
					if (!m_hasStartedRocketCharge)
					{
						TryDoTrick(m_chargingAnimation, force: true);
						m_hasStartedRocketCharge = true;
					}
					m_rocketLauncherStandard.SetCharging(charging: true);
					m_rocketLauncherStandard.SetInheritedVelocity(m_fighterPlane.GetCachedVelocity());
				}
				if (m_underneathTimer > 30f)
				{
					m_hasStartedRocketCharge = false;
					m_rocketLauncherStandard.ResetDidFireTrigger();
					m_rocketLauncherStandard.SetCharging(charging: false);
					m_underneathTimer = 0f;
					ResetLockPosition();
					m_currentState = FighterState.ReturningToDistance;
				}
			}
			else if (m_rocketAttackChargeTime > 4f)
			{
				m_underneathTimer = 0f;
				ResetLockPosition();
				m_inAttackFormation = false;
				m_currentState = FighterState.ReturningToDistance;
			}
			else
			{
				TryDoTrick(m_trickAnims, force: false);
				m_hasStartedRocketCharge = false;
				m_rocketLauncherStandard.SetCharging(charging: false);
				m_underneathTimer -= Time.deltaTime * 0.5f;
				if (m_underneathTimer < 0f)
				{
					m_underneathTimer = 0f;
				}
			}
		}
		else
		{
			m_underneathTimer = 0f;
			ResetLockPosition();
			m_inAttackFormation = false;
			m_currentState = FighterState.ReturningToDistance;
		}
	}
}
