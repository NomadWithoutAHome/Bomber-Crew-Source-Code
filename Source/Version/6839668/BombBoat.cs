using System;
using System.Collections;
using System.Collections.Generic;
using BomberCrewCommon;
using dbox;
using UnityEngine;
using WingroveAudio;

public class BombBoat : SmoothDamageable, Shootable
{
	[SerializeField]
	private float m_startingHealth;

	[SerializeField]
	private float m_movementSpeed;

	[SerializeField]
	private GameObject m_explosionPrefab;

	[SerializeField]
	private GameObject m_explosionPrefabSmall;

	[SerializeField]
	private Transform m_destroyDamageCentre;

	[SerializeField]
	private MissionPlaceableObject m_placeableObject;

	[SerializeField]
	private float m_rotateSpeed;

	[SerializeField]
	private ExternalDamageExplicit m_damage;

	[SerializeField]
	private Transform[] m_targetAreas;

	[SerializeField]
	private Transform m_centrePos;

	[SerializeField]
	private Collider m_collider;

	[SerializeField]
	private GameObject m_modelNormal;

	[SerializeField]
	private GameObject m_modelDestroyed;

	[SerializeField]
	private bool m_invincible;

	[SerializeField]
	private TaggableItem m_taggableItem;

	[SerializeField]
	private float m_detonateDelay = 4f;

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

	private WaypointPath m_waypoints;

	private int m_currentWaypointTarget;

	private bool m_isStarted;

	private float m_health;

	private bool m_isDestroyed;

	private bool m_isSetToDetonate;

	private float m_damageMultiplier = 0.1f;

	private bool m_audioIsPlaying;

	public override float DamageGetPassthrough(float amt, DamageSource damageSource)
	{
		if (!m_isDestroyed && !m_invincible)
		{
			m_damageEffects.DoDamageEffect(m_damageEffectsCollider, damageSource, amt);
			m_health -= amt;
			if (m_health < 0f)
			{
				Singleton<MissionCoordinator>.Instance.FireTrigger("BOMB_BOAT_DESTROY");
				Explode();
			}
		}
		return 0f;
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

	public bool IsDestroyed()
	{
		return m_isDestroyed;
	}

	public GameObject GetMainObject()
	{
		return base.gameObject;
	}

	public void Explode()
	{
		m_taggableItem.SetTaggable(taggable: false);
		GetComponent<NavigationPoint>().enabled = false;
		SetAudio(playing: false);
		DboxInMissionController.DBoxCall(DboxSdkWrapper.PostExplosion, m_destroyDamageCentre.position);
		GameObject fromPoolSlow = Singleton<PoolManager>.Instance.GetFromPoolSlow((!(m_damageMultiplier >= 1f)) ? m_explosionPrefabSmall : m_explosionPrefab);
		fromPoolSlow.btransform().SetFromCurrentPage(m_destroyDamageCentre.position);
		m_isDestroyed = true;
		Singleton<ShootableCoordinator>.Instance.RemoveShootable(this);
		StartCoroutine(DoExplosionDamage());
		m_modelDestroyed.SetActive(value: true);
		m_modelNormal.SetActive(value: false);
	}

	public override Vector3 GetDamagePosition(Vector3 inPos)
	{
		return m_collider.ClosestPointOnBounds(inPos);
	}

	private IEnumerator DoExplosionDamage()
	{
		yield return null;
		m_damage.DoDamage(m_damageMultiplier);
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
		MissionPlaceableObject placeableByRef = Singleton<MissionCoordinator>.Instance.GetPlaceableByRef(parameter);
		m_waypoints = placeableByRef.GetComponent<WaypointPath>();
		MissionCoordinator instance = Singleton<MissionCoordinator>.Instance;
		instance.OnTrigger = (Action<string>)Delegate.Combine(instance.OnTrigger, new Action<string>(OnTrigger));
		Singleton<ShootableCoordinator>.Instance.RegisterShootable(this);
	}

	private void OnTrigger(string trigger)
	{
		if (trigger == "BOMB_BOAT")
		{
			m_isStarted = true;
		}
	}

	private void Update()
	{
		if (m_isStarted && !m_isDestroyed)
		{
			List<MissionPlaceableObject> allWaypoints = m_waypoints.GetAllWaypoints();
			if (allWaypoints.Count > m_currentWaypointTarget)
			{
				SetAudio(playing: true);
				Vector3d vector3d = allWaypoints[m_currentWaypointTarget].gameObject.btransform().position - base.gameObject.btransform().position;
				vector3d.y = 0.0;
				Quaternion to = Quaternion.LookRotation((Vector3)vector3d.normalized);
				base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, to, m_rotateSpeed * Time.deltaTime);
				if (Vector3.Dot(base.transform.forward, (Vector3)vector3d.normalized) > 0.9f)
				{
					base.transform.position += base.transform.forward * Time.deltaTime * m_movementSpeed;
				}
				if (vector3d.magnitude < 100.0)
				{
					Singleton<MissionCoordinator>.Instance.FireTrigger("BOMBBOAT_" + m_currentWaypointTarget);
					m_currentWaypointTarget++;
				}
			}
			else
			{
				SetAudio(playing: false);
				Singleton<MissionCoordinator>.Instance.FireTrigger("BOMB_BOAT_SUCCESS");
				m_damageMultiplier = 1f;
				if (!m_isSetToDetonate)
				{
					StartCoroutine(DoExplodeAtTargetWithDelay());
				}
			}
		}
		else
		{
			SetAudio(playing: false);
		}
	}

	private IEnumerator DoExplodeAtTargetWithDelay()
	{
		m_invincible = true;
		m_isSetToDetonate = true;
		yield return new WaitForSeconds(m_detonateDelay);
		Explode();
		yield return null;
	}

	public Vector3 GetVelocity()
	{
		return base.transform.forward * m_movementSpeed;
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
}
