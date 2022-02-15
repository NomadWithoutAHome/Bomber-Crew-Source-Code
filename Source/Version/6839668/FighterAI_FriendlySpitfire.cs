using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;
using WingroveAudio;

public class FighterAI_FriendlySpitfire : MonoBehaviour
{
	public enum FighterState
	{
		PatrolBomber,
		Attacking,
		ReturningToDistance,
		Leaving
	}

	[SerializeField]
	private float m_closestRange;

	[SerializeField]
	private float m_keepDistanceLength;

	[SerializeField]
	private float m_firingRange;

	[SerializeField]
	private float m_approachFromBehindDistance = 30f;

	[SerializeField]
	private float m_formPositionLength = 30f;

	[SerializeField]
	private float m_maxFireTime;

	[SerializeField]
	private float m_timeToReform;

	[SerializeField]
	protected FighterDynamics m_controls;

	[SerializeField]
	private float m_maxAttackTime;

	private float m_catchUpFactor = 0.25f;

	[SerializeField]
	protected Transform m_gunFiringNodeAverage;

	[SerializeField]
	protected bool m_alternateAimAhead;

	[SerializeField]
	protected float m_alternateAimAheadMultiplier = 1.5f;

	[SerializeField]
	private RaycastGunTurret m_referenceRGT;

	[SerializeField]
	private RaycastGunTurret[] m_allGuns;

	[SerializeField]
	private Texture2D m_jabberPortrait;

	[SerializeField]
	[NamedText]
	private string m_jabberName;

	[SerializeField]
	[AudioEventName]
	private string m_jabberAudio;

	[SerializeField]
	private Vector3[] m_normalisedPositions;

	protected FighterState m_currentState;

	protected float m_currentTimer;

	protected float m_t;

	protected Vector3d m_avoidTarget;

	protected float m_totalAttackingTime;

	private FighterPlane m_currentTarget;

	private bool m_hasStartedHello;

	private bool m_hasFinishedHello;

	private bool m_hasAnnouncedLeave;

	private bool m_isLeadPilot;

	private static float m_timeSinceFighterDestroyedAnnounce;

	private int m_pilotIndex;

	private float m_alignStability;

	private Vector3 m_normalisedMatchTargetPosition;

	private float m_slowestSpeed = 1f;

	public void SetIsLeadPilot()
	{
		m_timeSinceFighterDestroyedAnnounce = 0f;
		m_isLeadPilot = true;
	}

	public void SetPilotIndex(int i)
	{
		m_pilotIndex = i;
		m_normalisedMatchTargetPosition = m_normalisedPositions[i % m_normalisedPositions.Length];
	}

	protected FighterPlane GetFiringTarget()
	{
		List<FighterPlane> allFighters = Singleton<FighterCoordinator>.Instance.GetAllFighters();
		float num = float.MaxValue;
		FighterPlane result = null;
		foreach (FighterPlane item in allFighters)
		{
			if (item.GetAI().IsEngaged() && !item.IsDestroyed())
			{
				Vector3 vector = item.transform.position - base.transform.position;
				if (vector.magnitude < num)
				{
					num = vector.magnitude;
					result = item;
				}
			}
		}
		return result;
	}

	private void Start()
	{
		RaycastGunTurret[] allGuns = m_allGuns;
		foreach (RaycastGunTurret raycastGunTurret in allGuns)
		{
			raycastGunTurret.SetRelatedObject(this);
		}
	}

	public Vector3 GetNormalisedMatchTargetPosition()
	{
		return m_normalisedMatchTargetPosition;
	}

	private float Magnitude2D(Vector3 d)
	{
		return new Vector3(d.x, 0f, d.z).magnitude;
	}

	private void MatchSpeedFlySmart(float maxSpeed)
	{
		BomberState bomberState = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState();
		Vector3 velocity = bomberState.GetPhysicsModel().GetVelocity();
		Vector3 vector = velocity;
		vector.y = 0f;
		Vector3 position = bomberState.transform.position;
		Vector3 velocity2 = m_controls.GetVelocity();
		float num = Vector3.Dot(velocity2.normalized, velocity.normalized);
		float num2 = num * 0.5f + 0.5f;
		Vector3 vector2 = base.transform.position + velocity2.normalized * 2f;
		float num3 = 60f;
		Vector3 normalisedMatchTargetPosition = GetNormalisedMatchTargetPosition();
		Vector3 vector3 = normalisedMatchTargetPosition.x * bomberState.transform.forward + normalisedMatchTargetPosition.y * Vector3.up + normalisedMatchTargetPosition.z * bomberState.transform.right;
		Vector3 vector4 = position + vector3 * num3;
		Vector3 vector5 = vector4 - vector.normalized * m_approachFromBehindDistance;
		float num4 = Vector3.Dot((vector4 - base.transform.position).normalized, velocity2.normalized);
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
			Vector3 rhs = base.transform.position + velocity2 * 0.5f - vector5;
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
		m_controls.SetHeading((vector6 - base.transform.position).normalized, maxSpeed, catchUpHack);
		m_controls.SetExactVelocity(Mathf.Lerp(maxSpeed, velocity.magnitude * num6, t));
	}

	public virtual void UpdateWithState(FighterState fs)
	{
		BomberState bomberState = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState();
		Vector3d vector3d = base.gameObject.btransform().position - bomberState.gameObject.btransform().position;
		if (vector3d.magnitude < 30.0 && m_currentState == FighterState.Attacking)
		{
			m_currentState = FighterState.PatrolBomber;
			if (m_hasFinishedHello)
			{
				m_currentTimer = 5f;
			}
			else
			{
				m_currentTimer = 0f;
			}
		}
		if (m_currentTarget == null || m_currentTarget.IsDestroyed() || !m_currentTarget.GetAI().IsEngaged())
		{
			m_currentTarget = GetFiringTarget();
		}
		if (m_currentTarget == null && m_currentState != FighterState.Leaving && m_currentState != 0)
		{
			m_currentTimer = 0f;
			m_currentState = FighterState.PatrolBomber;
		}
		bool firing = false;
		float num = 1f;
		if (m_currentState == FighterState.PatrolBomber)
		{
			if (vector3d.magnitude < 1250.0)
			{
				num = 0.5f;
			}
			if (vector3d.magnitude < 1000.0)
			{
				num = 0.3f;
			}
			if (vector3d.magnitude < 500.0)
			{
				num = 0.1f;
			}
		}
		else
		{
			num = 0.25f;
		}
		if (num < m_slowestSpeed)
		{
			m_slowestSpeed = num;
			if (m_slowestSpeed < 0.25f)
			{
				m_slowestSpeed = 0.25f;
			}
		}
		if (m_slowestSpeed < num)
		{
			num = m_slowestSpeed;
		}
		switch (m_currentState)
		{
		case FighterState.PatrolBomber:
			MatchSpeedFlySmart(num);
			if (!(vector3d.magnitude < 600.0))
			{
				break;
			}
			m_currentTimer += Time.deltaTime;
			if (m_currentTimer > 2.5f && !m_hasStartedHello && m_isLeadPilot)
			{
				if (Singleton<GameFlow>.Instance.GetGameMode().GetUseUSNaming())
				{
					Singleton<TopBarInfoQueue>.Instance.RegisterRequest(TopBarInfoQueue.TopBarRequest.Speech("mustang_pilot_arrive", Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_jabberName), m_jabberPortrait, m_jabberAudio, isGoodGuy: true));
				}
				else
				{
					Singleton<TopBarInfoQueue>.Instance.RegisterRequest(TopBarInfoQueue.TopBarRequest.Speech("spitfire_pilot_arrive", Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_jabberName), m_jabberPortrait, m_jabberAudio, isGoodGuy: true));
				}
				m_hasStartedHello = true;
			}
			if (m_currentTimer > 5f && m_currentTarget != null)
			{
				m_currentTimer = Random.Range(-3f, 0f);
				m_currentState = FighterState.Attacking;
				m_hasFinishedHello = true;
			}
			break;
		case FighterState.Leaving:
		{
			if (!m_hasAnnouncedLeave && m_isLeadPilot)
			{
				Singleton<TopBarInfoQueue>.Instance.RegisterRequest(TopBarInfoQueue.TopBarRequest.Speech("spitfire_pilot_leave", Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_jabberName), m_jabberPortrait, m_jabberAudio, isGoodGuy: true));
				m_hasAnnouncedLeave = true;
			}
			Vector3d vector3d2 = new Vector3d(-2500f, 0f, -2500f);
			Vector3d vector3d3 = vector3d2 - base.gameObject.btransform().position;
			if (vector3d.magnitude < 50.0)
			{
				m_controls.SetHeading((Vector3)vector3d.normalized, num, 0f);
			}
			else if (vector3d3.magnitude > 0.0)
			{
				m_controls.SetHeading((Vector3)vector3d3.normalized, num, 0f);
			}
			if (vector3d.magnitude > 3000.0)
			{
				Object.Destroy(base.gameObject);
			}
			break;
		}
		case FighterState.Attacking:
			if (m_currentTarget != null)
			{
				Vector3 aimAheadTarget = AimingUtils.GetAimAheadTarget(m_gunFiringNodeAverage.position, m_currentTarget.transform.position, m_currentTarget.GetCachedVelocity() - m_controls.GetVelocity(), m_referenceRGT.GetMuzzleVelocity());
				Vector3 vector4 = Vector3.LerpUnclamped(m_currentTarget.transform.position, aimAheadTarget, 1.2f);
				Vector3 vector5 = vector4 - base.transform.position;
				if (vector5.magnitude < m_closestRange || m_currentTimer > m_maxFireTime)
				{
					m_currentTimer = Random.Range(-3f, 0f);
					m_currentState = FighterState.ReturningToDistance;
				}
				if (vector5.magnitude < m_firingRange)
				{
					num = 0.35f;
					if (vector5.magnitude < m_firingRange / 2f)
					{
						num = 0.2f;
					}
					if (Vector3.Dot(vector5.normalized, m_controls.GetVelocity().normalized) > 0.95f)
					{
						m_currentTimer += Time.deltaTime;
						firing = true;
					}
				}
				else
				{
					if (vector5.magnitude < m_firingRange * 2f && Vector3.Dot(vector5.normalized, m_controls.GetVelocity().normalized) > 0.98f)
					{
						firing = true;
					}
					num = Mathf.Clamp01(vector5.magnitude / m_firingRange * 0.5f);
				}
				m_controls.SetHeading(vector5.normalized, num, 0f);
			}
			else
			{
				m_currentTimer = 0f;
				m_currentState = FighterState.PatrolBomber;
			}
			break;
		case FighterState.ReturningToDistance:
			m_currentTimer += Time.deltaTime;
			if (m_currentTarget != null)
			{
				Vector3 vector = bomberState.transform.position - base.transform.position;
				vector.y = 0f;
				vector = vector.normalized;
				vector.y = 0.1f;
				Vector3 vector2 = bomberState.transform.position + -vector.normalized * m_keepDistanceLength;
				Vector3 vector3 = vector2 - base.transform.position;
				m_controls.SetHeading(vector3.normalized, num, 1f);
				if (m_currentTimer > m_timeToReform)
				{
					m_currentTimer = Random.Range(-3f, 0f);
					m_currentState = FighterState.Attacking;
				}
			}
			else
			{
				m_currentTimer = 0f;
				m_currentState = FighterState.PatrolBomber;
			}
			break;
		}
		RaycastGunTurret[] allGuns = m_allGuns;
		foreach (RaycastGunTurret raycastGunTurret in allGuns)
		{
			raycastGunTurret.SetFiring(firing, m_controls.GetVelocity());
		}
		if (m_isLeadPilot)
		{
			m_timeSinceFighterDestroyedAnnounce += Time.deltaTime;
		}
	}

	public void FighterDestroyed()
	{
		if (m_timeSinceFighterDestroyedAnnounce > 15f)
		{
			Singleton<TopBarInfoQueue>.Instance.RegisterRequest(TopBarInfoQueue.TopBarRequest.Speech("spitfire_pilot_get_fighter", Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_jabberName), m_jabberPortrait, m_jabberAudio, isGoodGuy: true));
			m_timeSinceFighterDestroyedAnnounce = 0f;
		}
	}

	public FighterState GetAIState()
	{
		return m_currentState;
	}

	private void Update()
	{
		BomberState bomberState = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState();
		if ((base.gameObject.btransform().position - bomberState.gameObject.btransform().position).magnitude < 2000.0 || m_hasStartedHello)
		{
			m_totalAttackingTime += Time.deltaTime;
		}
		if (m_totalAttackingTime > m_maxAttackTime)
		{
			m_currentState = FighterState.Leaving;
			m_currentTimer = 0f;
		}
		UpdateWithState(m_currentState);
	}

	public FighterState GetState()
	{
		return m_currentState;
	}
}
