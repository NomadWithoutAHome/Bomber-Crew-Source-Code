using BomberCrewCommon;
using UnityEngine;

public class FighterAI_AceMatchSpeedSmartRocket : FighterAI
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
	private FighterRocketLauncher[] m_rocketLaunchers;

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

	private float m_forwardMovement;

	private bool m_inAttackFormation;

	private bool m_doingRocketAttack;

	private bool m_rightHanded;

	private float m_alignStability;

	private float m_underneathTimer;

	private float m_trickTimer;

	private FighterRocketLauncher m_currentRocketLauncher;

	private bool m_hasStartedRocketCharge;

	protected override bool CanAttack()
	{
		return true;
	}

	protected override void StopAttack()
	{
	}

	private FighterRocketLauncher GetNextAvailable()
	{
		FighterRocketLauncher[] rocketLaunchers = m_rocketLaunchers;
		foreach (FighterRocketLauncher fighterRocketLauncher in rocketLaunchers)
		{
			if (fighterRocketLauncher.HasAmmo())
			{
				return fighterRocketLauncher;
			}
		}
		return null;
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
		if (fs == FighterState.AttackFormation || fs == FighterState.BreakFormation)
		{
			if (!m_inAttackFormation)
			{
				m_inAttackFormation = true;
				m_currentRocketLauncher = GetNextAvailable();
				m_hasStartedRocketCharge = false;
				if (m_currentRocketLauncher != null)
				{
					m_currentRocketLauncher.ResetDidFireTrigger();
					m_doingRocketAttack = Random.Range(0f, 1f) < m_rocketFireAttackChance;
					if (m_doingRocketAttack)
					{
						TryLockPosition();
						if (!m_hasCurrentlyLockedPosition)
						{
							m_doingRocketAttack = false;
						}
					}
				}
				else
				{
					m_doingRocketAttack = false;
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
				if (m_currentState == FighterState.Leaving)
				{
					m_leftBecauseOfTimeOut = false;
					m_currentState = FighterState.BreakFormation;
				}
			}
			if (m_fighterPlane.IsScared())
			{
				m_currentState = FighterState.ReturningToDistance;
				if (m_currentRocketLauncher != null)
				{
					m_currentRocketLauncher.SetCharging(charging: false);
				}
				m_currentTimer = 0f;
			}
		}
		else
		{
			ResetLockPosition();
			m_inAttackFormation = false;
			base.UpdateWithState(fs);
		}
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
		float num3 = 40f;
		if (!m_hasCurrentlyLockedPosition)
		{
			TryLockPosition();
		}
		if (m_hasCurrentlyLockedPosition)
		{
			Vector3 currentlyLockedPositionVector = m_currentlyLockedPositionVector;
			Vector3 vector3 = currentlyLockedPositionVector.x * bomberState.transform.forward + currentlyLockedPositionVector.y * Vector3.up + currentlyLockedPositionVector.z * bomberState.transform.right;
			Vector3 vector4 = position + vector3 * num3;
			Vector3 vector5 = vector4 - vector.normalized * m_approachFromBehindDistance;
			float num4 = Vector3.Dot((vector4 - base.transform.position).normalized, m_fighterPlane.GetCachedVelocity().normalized);
			float num5 = Magnitude2D(vector4 - base.transform.position);
			float maxVelocity = m_controls.GetMaxVelocity();
			float t = 0f;
			float num6 = 1f;
			float catchUpHack = Mathf.Clamp01((num5 - 50f) / 75f);
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
			if (num5 > m_approachFromBehindDistance * 1.5f || num < 0.25f)
			{
				vector6 = vector5;
				catchUpHack = 1f;
			}
			else
			{
				Vector3 rhs = base.transform.position + m_fighterPlane.GetCachedVelocity() * 0.5f - vector5;
				float num7 = Vector3.Dot(vector.normalized, rhs);
				Vector3 vector7 = vector5 - (1f - num7) * vector.normalized;
				if (!(num5 < 50f))
				{
					vector6 = ((!(num4 > 0.5f) || !(num > 0.25f)) ? vector5 : vector7);
				}
				else if (num > 0.5f)
				{
					vector6 = vector7;
					if (num4 > 0f)
					{
						t = 1f;
						num6 = 1f;
						catchUpHack = 0f;
						Vector3 vector8 = vector4 - base.transform.position;
						num6 = ((!(m_alignStability > 1f)) ? (1f + Mathf.Abs(vector8.magnitude - 3f) * 0.3f) : (1f + Mathf.Abs(vector8.magnitude - 3f) * 0.5f));
						num6 += 0.02f;
					}
					else
					{
						t = 1f;
						num6 = 1f;
						catchUpHack = 0f;
						if (m_alignStability > 1f)
						{
							num6 = 1f - Mathf.Abs((vector4 - base.transform.position).magnitude - 6f) * 0.05f;
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
			m_controls.SetExactVelocity(Mathf.Lerp(maxVelocity, velocity.magnitude * num6, t));
			m_debugMarker1.transform.position = vector6;
			m_debugMarker2.transform.position = vector4;
			if (flag)
			{
				m_underneathTimer += Time.deltaTime;
				if (m_currentRocketLauncher.DidFire())
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
					m_currentRocketLauncher.SetCharging(charging: true);
					m_currentRocketLauncher.SetInheritedVelocity(m_fighterPlane.GetCachedVelocity());
				}
				if (m_underneathTimer > 30f)
				{
					m_hasStartedRocketCharge = false;
					m_currentRocketLauncher.ResetDidFireTrigger();
					m_currentRocketLauncher.SetCharging(charging: false);
					m_currentRocketLauncher = null;
					m_underneathTimer = 0f;
					ResetLockPosition();
					m_currentState = FighterState.ReturningToDistance;
				}
			}
			else
			{
				TryDoTrick(m_trickAnims, force: false);
				m_hasStartedRocketCharge = false;
				m_currentRocketLauncher.SetCharging(charging: false);
				m_underneathTimer -= Time.deltaTime * 0.5f;
				if (m_underneathTimer < 0f)
				{
					m_underneathTimer = 0f;
				}
			}
			return;
		}
		m_underneathTimer = 0f;
		ResetLockPosition();
		m_currentState = FighterState.ReturningToDistance;
		if (Singleton<GameFlow>.Instance.GetGameMode().AllowAceRunawayLowHealth())
		{
			if (m_fighterPlane.GetHealthNormalised() < 0.35f && !m_aceDetails.HasBeenEncounteredPreviously())
			{
				m_leftBecauseOfTimeOut = true;
				m_currentState = FighterState.Leaving;
			}
			else if (m_fighterPlane.GetHealthNormalised() < 0.25f && m_aceDetails.GetNumEncounters() < 3 && Random.Range(0, 4) == 0)
			{
				m_leftBecauseOfTimeOut = true;
				m_currentState = FighterState.Leaving;
			}
		}
	}
}
