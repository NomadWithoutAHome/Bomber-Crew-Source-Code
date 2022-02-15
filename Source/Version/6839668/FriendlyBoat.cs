using System;
using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;
using WingroveAudio;

public class FriendlyBoat : SmoothDamageable, Shootable
{
	[SerializeField]
	private float m_startingHealth;

	[SerializeField]
	private float m_movementSpeed;

	[SerializeField]
	private GameObject m_explosionPrefab;

	[SerializeField]
	private Transform m_destroyDamageCentre;

	[SerializeField]
	private MissionPlaceableObject m_placeableObject;

	[SerializeField]
	private float m_rotateSpeed;

	[SerializeField]
	private Transform[] m_targetAreas;

	[SerializeField]
	private Transform m_centrePos;

	[SerializeField]
	private Collider m_collider;

	[SerializeField]
	private GameObject m_model;

	[SerializeField]
	private GameObject m_modelDestroyed;

	[SerializeField]
	[AudioEventName]
	private string m_audioStart;

	[SerializeField]
	[AudioEventName]
	private string m_audioEnd;

	[SerializeField]
	private DamageEffects m_damageEffects;

	[SerializeField]
	private Collider m_damageEffectsCollider;

	[SerializeField]
	private bool m_doDamageEffects;

	[SerializeField]
	private bool m_useAnimator;

	[SerializeField]
	private Animator m_animator;

	[SerializeField]
	private string m_isMovingBoolName = "IsMoving";

	private WaypointPath m_waypoints;

	private int m_currentWaypointTarget;

	private bool m_isStarted;

	private float m_health;

	private bool m_isDestroyed;

	private bool m_audioIsPlaying;

	public override float DamageGetPassthrough(float amt, DamageSource damageSource)
	{
		if (!m_isDestroyed)
		{
			if (m_doDamageEffects && m_damageEffects != null)
			{
				m_damageEffects.DoDamageEffect(m_damageEffectsCollider, damageSource, amt);
			}
			m_health -= amt;
			if (m_health < 0f)
			{
				Explode();
			}
		}
		return 0f;
	}

	public GameObject GetMainObject()
	{
		return base.gameObject;
	}

	public float GetAimAtAccuracy()
	{
		return 1f;
	}

	private void SetAudio(bool playing)
	{
		if (playing != m_audioIsPlaying)
		{
			if (playing)
			{
				WingroveRoot.Instance.PostEventGO(m_audioStart, base.gameObject);
			}
			else
			{
				WingroveRoot.Instance.PostEventGO(m_audioEnd, base.gameObject);
			}
			m_audioIsPlaying = playing;
		}
	}

	public void Explode()
	{
		SetAudio(playing: false);
		m_isDestroyed = true;
		Singleton<ShootableCoordinator>.Instance.RemoveShootable(this);
		GetComponent<TaggableItem>().SetTaggable(taggable: false);
		GetComponent<NavigationPoint>().enabled = false;
		if (m_useAnimator)
		{
			m_animator.SetBool("IsSunk", value: true);
		}
		else
		{
			m_model.SetActive(value: false);
			m_modelDestroyed.SetActive(value: true);
		}
		string parameter = m_placeableObject.GetParameter("triggerOnDestroy");
		if (!string.IsNullOrEmpty(parameter))
		{
			Singleton<MissionCoordinator>.Instance.FireTrigger(parameter);
		}
	}

	public bool IsDestroyed()
	{
		return m_isDestroyed;
	}

	public override Vector3 GetDamagePosition(Vector3 inPos)
	{
		return m_collider.ClosestPointOnBounds(inPos);
	}

	public override float GetHealthNormalised()
	{
		return m_health / m_startingHealth;
	}

	public override bool IsDamageBlocker()
	{
		return true;
	}

	private void Start()
	{
		m_health = m_startingHealth;
		string parameter = m_placeableObject.GetParameter("waypoints");
		if (!string.IsNullOrEmpty(parameter) && parameter != "REF_-1")
		{
			MissionPlaceableObject placeableByRef = Singleton<MissionCoordinator>.Instance.GetPlaceableByRef(parameter);
			m_waypoints = placeableByRef.GetComponent<WaypointPath>();
		}
		MissionCoordinator instance = Singleton<MissionCoordinator>.Instance;
		instance.OnTrigger = (Action<string>)Delegate.Combine(instance.OnTrigger, new Action<string>(OnTrigger));
		Singleton<ShootableCoordinator>.Instance.RegisterShootable(this);
	}

	private void OnTrigger(string trigger)
	{
		if (trigger == "BOAT" && m_waypoints != null)
		{
			m_isStarted = true;
		}
	}

	private void Update()
	{
		if (m_isStarted && !m_isDestroyed && m_waypoints != null)
		{
			List<MissionPlaceableObject> allWaypoints = m_waypoints.GetAllWaypoints();
			if (allWaypoints.Count > m_currentWaypointTarget)
			{
				SetAudio(playing: true);
				SetAnimatorIsMovingBool(isMoving: true);
				Vector3d vector3d = allWaypoints[m_currentWaypointTarget].gameObject.btransform().position - base.gameObject.btransform().position;
				vector3d.y = 0.0;
				if (vector3d.magnitude > 0.0)
				{
					Quaternion to = Quaternion.LookRotation((Vector3)vector3d.normalized);
					base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, to, m_rotateSpeed * Time.deltaTime);
					if (Vector3.Dot(base.transform.forward, (Vector3)vector3d.normalized) > 0.9f)
					{
						base.transform.position += base.transform.forward * Time.deltaTime * m_movementSpeed;
					}
				}
				if (vector3d.magnitude < 100.0)
				{
					Singleton<MissionCoordinator>.Instance.FireTrigger("BOAT_" + m_currentWaypointTarget);
					m_currentWaypointTarget++;
				}
			}
			else
			{
				SetAudio(playing: false);
				SetAnimatorIsMovingBool(isMoving: false);
			}
		}
		else
		{
			SetAudio(playing: false);
			SetAnimatorIsMovingBool(isMoving: false);
		}
	}

	public Vector3 GetVelocity()
	{
		if (m_waypoints != null)
		{
			List<MissionPlaceableObject> allWaypoints = m_waypoints.GetAllWaypoints();
			if (allWaypoints.Count > m_currentWaypointTarget)
			{
				return base.transform.forward * m_movementSpeed;
			}
			return Vector3.zero;
		}
		return Vector3.zero;
	}

	public Transform GetRandomTargetableArea()
	{
		return m_targetAreas[UnityEngine.Random.Range(0, m_targetAreas.Length)];
	}

	public Transform GetCentreTransform()
	{
		return m_centrePos;
	}

	public ShootableType GetShootableType()
	{
		return ShootableType.Defendable;
	}

	public bool IsLitUp()
	{
		return true;
	}

	public bool IsEvasive()
	{
		return false;
	}

	private void SetAnimatorIsMovingBool(bool isMoving)
	{
		if (m_useAnimator)
		{
			m_animator.SetBool(m_isMovingBoolName, isMoving);
		}
	}
}
