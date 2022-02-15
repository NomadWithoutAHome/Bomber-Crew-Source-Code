using BomberCrewCommon;
using UnityEngine;

public class FighterAI_MatchSpeedSmartSwapModes : FighterAI
{
	[SerializeField]
	private bool m_alwaysSelectUnder;

	[SerializeField]
	private bool m_alwaysSelectInFront;

	[SerializeField]
	private float m_approachFromBehindDistance = 30f;

	[SerializeField]
	private float m_formPositionLength = 30f;

	[SerializeField]
	private Vector3 m_dragStart;

	[SerializeField]
	private Vector3 m_dragEnd;

	private float m_forwardMovement;

	private bool m_rightHanded;

	private float m_alignStability;

	private float m_underneathTimer;

	private float m_underneathTimerT;

	private bool m_hasSelectedMode;

	private bool m_currentModeIsMatch;

	public override void UpdateWithState(FighterState fs)
	{
		if (fs == FighterState.AttackFormation || fs == FighterState.BreakFormation)
		{
			if (!m_hasSelectedMode)
			{
				m_hasSelectedMode = true;
				m_currentModeIsMatch = !m_currentModeIsMatch;
			}
			if (m_currentModeIsMatch)
			{
				MatchSpeedFlySmart();
				if (m_fighterPlane.IsScared())
				{
					m_currentState = FighterState.ReturningToDistance;
					m_currentTimer = 0f;
				}
				if (!m_hasCurrentlyLockedPosition)
				{
					m_currentTimer = 0f;
					m_currentModeIsMatch = false;
					m_currentState = FighterState.BreakFormation;
				}
			}
			else
			{
				ResetLockPosition();
				base.UpdateWithState(FighterState.BreakFormation);
			}
		}
		else
		{
			m_hasSelectedMode = false;
			ResetLockPosition();
			base.UpdateWithState(fs);
		}
	}

	public void TryLockPosition()
	{
		BomberSystems bomberSystems = Singleton<BomberSpawn>.Instance.GetBomberSystems();
		FighterCoordinator instance = Singleton<FighterCoordinator>.Instance;
		if (m_alwaysSelectUnder)
		{
			if (instance.IsAttackPositionAvailable(FighterCoordinator.AttackPositions.Below))
			{
				SetLockPosition(new Vector3(0f, -1f, 0f), FighterCoordinator.AttackPositions.Below);
			}
			else
			{
				ResetLockPosition();
			}
			return;
		}
		if (m_alwaysSelectInFront)
		{
			if (instance.IsAttackPositionAvailable(FighterCoordinator.AttackPositions.InFront))
			{
				float num = Random.Range(0.02f, 0.04f);
				num *= (float)((Random.Range(0, 2) != 1) ? 1 : (-1));
				SetLockPosition(new Vector3(num, 0.03f, 2.5f), FighterCoordinator.AttackPositions.InFront);
			}
			else
			{
				ResetLockPosition();
			}
			return;
		}
		bool flag = false;
		Station stationFor = bomberSystems.GetStationFor(BomberSystems.StationType.GunsNose);
		if (stationFor != null)
		{
			flag = stationFor.GetCurrentCrewman() != null;
		}
		bool flag2 = bomberSystems.GetStationFor(BomberSystems.StationType.GunsTail).GetCurrentCrewman() != null;
		bool flag3 = bomberSystems.GetStationFor(BomberSystems.StationType.GunsUpperDeck).GetCurrentCrewman() != null;
		bool flag4 = false;
		Station stationFor2 = bomberSystems.GetStationFor(BomberSystems.StationType.GunsLowerDeck);
		if (stationFor2 != null)
		{
			flag4 = stationFor2.GetCurrentCrewman() != null;
		}
		if (!flag2 && instance.IsAttackPositionAvailable(FighterCoordinator.AttackPositions.Tailing))
		{
			SetLockPosition(new Vector3(0f, 0f, -1f), FighterCoordinator.AttackPositions.Tailing);
		}
		else if (!flag && instance.IsAttackPositionAvailable(FighterCoordinator.AttackPositions.InFrontRight))
		{
			SetLockPosition(new Vector3(0.5f, -1f, 1.5f), FighterCoordinator.AttackPositions.InFrontRight);
		}
		else if (!flag && instance.IsAttackPositionAvailable(FighterCoordinator.AttackPositions.InFrontLeft))
		{
			SetLockPosition(new Vector3(-0.5f, -1f, 1.5f), FighterCoordinator.AttackPositions.InFrontLeft);
		}
		else if (!flag3 && instance.IsAttackPositionAvailable(FighterCoordinator.AttackPositions.AboveLeft))
		{
			SetLockPosition(new Vector3(-0.5f, 1f, 0.3f), FighterCoordinator.AttackPositions.AboveLeft);
		}
		else if (!flag3 && instance.IsAttackPositionAvailable(FighterCoordinator.AttackPositions.AboveRight))
		{
			SetLockPosition(new Vector3(0.5f, 1f, 0.3f), FighterCoordinator.AttackPositions.AboveRight);
		}
		else if (!flag4 && instance.IsAttackPositionAvailable(FighterCoordinator.AttackPositions.BelowLeft))
		{
			SetLockPosition(new Vector3(-0.5f, -1f, 0.3f), FighterCoordinator.AttackPositions.BelowLeft);
		}
		else if (!flag4 && instance.IsAttackPositionAvailable(FighterCoordinator.AttackPositions.BelowRight))
		{
			SetLockPosition(new Vector3(0.5f, -1f, 0.3f), FighterCoordinator.AttackPositions.BelowRight);
		}
		else if (instance.IsAttackPositionAvailable(FighterCoordinator.AttackPositions.Right))
		{
			SetLockPosition(new Vector3(1f, 0f, 0.2f), FighterCoordinator.AttackPositions.Right);
		}
		else if (instance.IsAttackPositionAvailable(FighterCoordinator.AttackPositions.Left))
		{
			SetLockPosition(new Vector3(-1f, 0f, 0.2f), FighterCoordinator.AttackPositions.Left);
		}
		if (instance.IsAttackPositionAvailable(FighterCoordinator.AttackPositions.Tailing))
		{
			SetLockPosition(new Vector3(0f, 0f, -1f), FighterCoordinator.AttackPositions.Tailing);
		}
		else if (instance.IsAttackPositionAvailable(FighterCoordinator.AttackPositions.InFrontRight))
		{
			SetLockPosition(new Vector3(0.5f, -1f, 1.5f), FighterCoordinator.AttackPositions.InFrontRight);
		}
		else if (instance.IsAttackPositionAvailable(FighterCoordinator.AttackPositions.InFrontLeft))
		{
			SetLockPosition(new Vector3(-0.5f, -1f, 1.5f), FighterCoordinator.AttackPositions.InFrontLeft);
		}
		else if (!instance.IsAttackPositionAvailable(FighterCoordinator.AttackPositions.AboveLeft))
		{
			SetLockPosition(new Vector3(-0.5f, 1f, 0.3f), FighterCoordinator.AttackPositions.AboveLeft);
		}
		else if (instance.IsAttackPositionAvailable(FighterCoordinator.AttackPositions.AboveRight))
		{
			SetLockPosition(new Vector3(0.5f, 1f, 0.3f), FighterCoordinator.AttackPositions.AboveRight);
		}
		else if (instance.IsAttackPositionAvailable(FighterCoordinator.AttackPositions.BelowLeft))
		{
			SetLockPosition(new Vector3(-0.5f, -1f, 0.3f), FighterCoordinator.AttackPositions.BelowLeft);
		}
		else if (instance.IsAttackPositionAvailable(FighterCoordinator.AttackPositions.BelowRight))
		{
			SetLockPosition(new Vector3(0.5f, -1f, 0.3f), FighterCoordinator.AttackPositions.BelowRight);
		}
		else
		{
			ResetLockPosition();
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
			m_underneathTimerT = Mathf.Max(m_underneathTimerT, Mathf.Clamp01(m_underneathTimer / m_formPositionLength));
			Vector3 vector3 = Vector3.Lerp(m_dragStart, m_dragEnd, m_underneathTimerT);
			Vector3 vector4 = m_currentlyLockedPositionVector + vector3;
			Vector3 vector5 = vector4.x * bomberState.transform.forward + vector4.y * Vector3.up + vector4.z * bomberState.transform.right;
			Vector3 vector6 = position + vector5 * num3;
			Vector3 vector7 = vector6 - vector.normalized * m_approachFromBehindDistance;
			float num4 = Vector3.Dot((vector6 - base.transform.position).normalized, m_fighterPlane.GetCachedVelocity().normalized);
			float num5 = Magnitude2D(vector6 - base.transform.position);
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
			Vector3 vector8;
			if (num5 > m_approachFromBehindDistance * 1.5f || num < 0.25f)
			{
				vector8 = vector7;
				catchUpHack = 1f;
			}
			else
			{
				Vector3 rhs = base.transform.position + m_fighterPlane.GetCachedVelocity() * 0.5f - vector7;
				float num7 = Vector3.Dot(vector.normalized, rhs);
				Vector3 vector9 = vector7 - (1f - num7) * vector.normalized;
				if (!(num5 < 50f))
				{
					vector8 = ((!(num4 > 0.5f) || !(num > 0.25f)) ? vector7 : vector9);
				}
				else if (num > 0.5f)
				{
					vector8 = vector9;
					if (num4 > 0f)
					{
						t = 1f;
						num6 = 1f;
						catchUpHack = 0f;
						Vector3 vector10 = vector6 - base.transform.position;
						num6 = ((!(m_alignStability > 1f)) ? (1f + Mathf.Abs(vector10.magnitude - 3f) * 0.3f) : (1f + Mathf.Abs(vector10.magnitude - 3f) * 0.5f));
						num6 += 0.02f;
					}
					else
					{
						t = 1f;
						num6 = 1f;
						catchUpHack = 0f;
						if (m_alignStability > 1f)
						{
							num6 = 1f - Mathf.Abs((vector6 - base.transform.position).magnitude - 6f) * 0.05f;
						}
					}
					if (m_alignStability > 1f)
					{
						flag = true;
					}
				}
				else
				{
					vector8 = vector7;
				}
			}
			m_controls.SetHeading((vector8 - base.transform.position).normalized, 1f, catchUpHack);
			m_controls.SetExactVelocity(Mathf.Lerp(maxVelocity, velocity.magnitude * num6, t));
			m_debugMarker1.transform.position = vector8;
			m_debugMarker2.transform.position = vector6;
			if (flag)
			{
				m_underneathTimer += Time.deltaTime;
				if (m_underneathTimer > m_formPositionLength)
				{
					m_underneathTimerT = 0f;
					m_underneathTimer = 0f;
					ResetLockPosition();
					m_currentState = FighterState.ReturningToDistance;
				}
			}
			else
			{
				m_underneathTimer -= Time.deltaTime * 0.5f;
				if (m_underneathTimer < 0f)
				{
					m_underneathTimer = 0f;
				}
			}
		}
		else
		{
			m_underneathTimerT = 0f;
			m_underneathTimer = 0f;
			ResetLockPosition();
			m_currentState = FighterState.ReturningToDistance;
		}
	}
}
