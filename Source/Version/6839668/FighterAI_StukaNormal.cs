using BomberCrewCommon;
using UnityEngine;
using WingroveAudio;

public class FighterAI_StukaNormal : FighterAI
{
	[SerializeField]
	private float m_diveAttackDistance = 1000f;

	[SerializeField]
	private float m_diveAttackHeight = 400f;

	[SerializeField]
	private float m_breakAttackSpeedMultiplierStuka;

	[SerializeField]
	private float m_diveFireRange = 500f;

	[SerializeField]
	private float m_regularFireRange = 500f;

	[SerializeField]
	private GameObject m_diveBombPrefab;

	[SerializeField]
	private Transform m_bombTransform;

	[SerializeField]
	private GameObject m_hazardPreviewIndicatorPrefab;

	private bool m_isOnGround;

	private bool m_shouldChargeRockets;

	private float m_timeAfterRocketsGone;

	private Shootable m_currentTarget;

	private bool m_targetIsBomber;

	private bool m_hasDroppedBombs;

	private bool m_setUp;

	private bool m_isStukaAudioPlaying;

	private GameObject m_chargeIndicator;

	private bool m_chargeIsVisible;

	private bool m_forceToBomberTarget;

	private float m_giveUpTimer;

	public override void UpdateWithState(FighterState fs)
	{
		if (!m_setUp)
		{
			m_chargeIndicator = Object.Instantiate(m_hazardPreviewIndicatorPrefab);
			m_chargeIndicator.GetComponent<HazardPreviewWarning>().SetTracker(Singleton<TagManager>.Instance.GetUICamera(), base.gameObject, 15f);
			m_chargeIndicator.GetComponent<HazardPreviewWarning>().SetNoCountdown();
			m_chargeIndicator.SetActive(value: false);
			m_fighterPlane.OnDestroyed += OnDestroyed;
			m_setUp = true;
		}
		if (m_currentTarget == null)
		{
			m_currentFiringTarget = null;
			m_currentTarget = Singleton<ShootableCoordinator>.Instance.GetNearestShootableNotDestroyed(ShootableType.Defendable, base.transform.position);
			m_targetIsBomber = false;
			m_hasDroppedBombs = false;
			if (m_currentTarget == null || m_forceToBomberTarget)
			{
				m_currentTarget = Singleton<ShootableCoordinator>.Instance.GetNearestShootable(ShootableType.Player, base.transform.position);
				m_targetIsBomber = true;
			}
			m_giveUpTimer = 0f;
		}
		m_shouldChargeRockets = false;
		switch (fs)
		{
		case FighterState.KeepingRange:
		case FighterState.Reforming:
			if (m_targetIsBomber)
			{
				base.UpdateWithState(fs);
			}
			else
			{
				FlyAboveBomber();
			}
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
	}

	private void OnDestroyed()
	{
		if (m_chargeIndicator != null)
		{
			Object.Destroy(m_chargeIndicator);
		}
	}

	private void OnDestroy()
	{
		if (m_chargeIndicator != null)
		{
			Object.Destroy(m_chargeIndicator);
		}
	}

	public void AttackBrokenRocket()
	{
		bool flag = false;
		m_currentFiringTarget = m_currentTarget.GetCentreTransform();
		Vector3 vector = base.transform.position - m_currentFiringTarget.position;
		Vector3 vector2 = vector;
		if (m_alternateAimAhead)
		{
			Vector3 aimAheadTarget = AimingUtils.GetAimAheadTarget(m_gunFiringNodeAverage.position, m_currentFiringTarget.position, m_currentTarget.GetVelocity(), m_fighterPlane.GetMuzzleVelocity() + m_fighterPlane.GetCachedVelocity().magnitude, m_inaccuracyCached, isGroundBasedOrFriendly: false);
			aimAheadTarget += m_regularAimAheadVelFudge * m_currentTarget.GetVelocity();
			vector = base.transform.position - Vector3.LerpUnclamped(base.transform.position, aimAheadTarget, m_alternateAimAheadMultiplier);
			m_debugMarker1.position = m_currentFiringTarget.position;
			m_debugMarker2.position = aimAheadTarget;
		}
		else
		{
			Vector3 aimAheadTarget2 = AimingUtils.GetAimAheadTarget(m_gunFiringNodeAverage.transform.position, m_currentFiringTarget.position, m_currentTarget.GetVelocity() - m_fighterPlane.GetCachedVelocity(), m_fighterPlane.GetMuzzleVelocity(), m_inaccuracyCached, isGroundBasedOrFriendly: false);
			aimAheadTarget2 += m_regularAimAheadVelFudge * m_currentTarget.GetVelocity();
			vector = base.transform.position - aimAheadTarget2;
			vector2 = vector;
			m_debugMarker1.position = m_currentFiringTarget.position;
			m_debugMarker2.position = aimAheadTarget2;
		}
		Vector3 vector3 = -vector.normalized;
		if (m_targetIsBomber)
		{
			m_controls.SetHeading(vector3, m_breakAttackSpeedMultiplierStuka, 1f);
		}
		else
		{
			m_controls.SetHeading(vector3, m_breakAttackSpeedMultiplierStuka, 0f);
		}
		bool flag2 = m_currentTarget.IsEvasive();
		if (m_targetIsBomber)
		{
			if (Vector3.Dot(vector3, m_controls.GetCurrentHeading()) > 0.95f && vector2.magnitude < m_regularFireRange)
			{
				flag = true;
				m_currentTimer += Time.deltaTime;
			}
		}
		else
		{
			Vector3 vector4 = vector3;
			vector4.y = 0f;
			Vector3 currentHeading = m_controls.GetCurrentHeading();
			currentHeading.y = 0f;
			if (Vector3.Dot(vector4.normalized, currentHeading.normalized) > 0.95f && vector2.magnitude < m_regularFireRange)
			{
				flag = true;
				m_currentTimer += Time.deltaTime;
			}
		}
		bool flag3 = false;
		if (!m_targetIsBomber)
		{
			if (vector2.magnitude < m_diveFireRange && flag)
			{
				if (!m_hasDroppedBombs)
				{
					m_hasDroppedBombs = true;
					GameObject gameObject = Object.Instantiate(m_diveBombPrefab);
					gameObject.transform.position = m_bombTransform.position;
					gameObject.transform.rotation = m_bombTransform.rotation;
					gameObject.GetComponent<EnemyBomb>().Release(m_currentTarget, m_controls.GetVelocity().magnitude);
					if (m_isStukaAudioPlaying)
					{
						WingroveRoot.Instance.PostEventGO("STUKA_STOP", base.gameObject);
						m_isStukaAudioPlaying = false;
					}
				}
			}
			else if (!m_hasDroppedBombs && vector2.magnitude < m_diveFireRange * 3.5f)
			{
				flag3 = true;
			}
			if (vector2.magnitude < m_regularFireRange)
			{
				m_giveUpTimer += Time.deltaTime;
				if (!m_isStukaAudioPlaying)
				{
					WingroveRoot.Instance.PostEventGO("STUKA_START", base.gameObject);
					m_isStukaAudioPlaying = true;
				}
			}
			if (vector2.magnitude < 100f || m_hasDroppedBombs || m_giveUpTimer > 15f)
			{
				m_hasDroppedBombs = false;
				m_fighterPlane.StopFiringGuns();
				m_currentTimer = 0f;
				m_currentState = FighterState.ReturningToDistance;
				SetAvoidTarget();
				m_currentTarget = null;
				m_forceToBomberTarget = !m_forceToBomberTarget;
				if (m_isStukaAudioPlaying)
				{
					WingroveRoot.Instance.PostEventGO("STUKA_STOP", base.gameObject);
					m_isStukaAudioPlaying = false;
				}
				flag3 = false;
			}
		}
		else
		{
			if (flag)
			{
				m_fighterPlane.StartFiringGuns();
			}
			else
			{
				m_fighterPlane.StopFiringGuns();
			}
			if (vector2.magnitude < 300f)
			{
				m_fighterPlane.StopFiringGuns();
				m_currentTimer = 0f;
				m_currentState = FighterState.ReturningToDistance;
				SetAvoidTarget();
				m_currentTarget = null;
				m_forceToBomberTarget = !m_forceToBomberTarget;
			}
		}
		if (flag3 != m_chargeIsVisible)
		{
			m_chargeIndicator.SetActive(flag3);
			m_chargeIsVisible = flag3;
		}
	}

	private void FlyAboveBomber()
	{
		Vector3 position = m_currentTarget.GetCentreTransform().position;
		Vector3 vector = position - base.transform.position;
		Vector3 vector2 = vector;
		vector2.y = 0f;
		Vector3 vector3 = position - vector2.normalized * m_diveAttackDistance;
		vector3.y += m_diveAttackHeight;
		Vector3 vector4 = vector3 - base.transform.position;
		m_fighterPlane.GetDynamics().SetHeading(vector4.normalized, 1f, 1f);
		if (base.transform.position.y > vector3.y)
		{
			m_currentState = FighterState.BreakFormation;
		}
	}
}
