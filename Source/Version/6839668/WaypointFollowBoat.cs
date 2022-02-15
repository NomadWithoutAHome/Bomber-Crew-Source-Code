using System;
using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;
using WingroveAudio;

public class WaypointFollowBoat : MonoBehaviour
{
	[SerializeField]
	private float m_startingHealth;

	[SerializeField]
	private float m_movementSpeed;

	[SerializeField]
	private MissionPlaceableObject m_placeableObject;

	[SerializeField]
	private float m_rotateSpeed;

	[SerializeField]
	[AudioEventName]
	private string m_audioStart;

	[SerializeField]
	[AudioEventName]
	private string m_audioEnd;

	[SerializeField]
	private SmoothDamageable m_damageable;

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

	private string m_startMovementTrigger;

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
		m_startMovementTrigger = m_placeableObject.GetParameter("startTrigger");
	}

	private void OnTrigger(string trigger)
	{
		if (trigger == m_startMovementTrigger && m_waypoints != null)
		{
			m_isStarted = true;
		}
	}

	private void Update()
	{
		if (m_isStarted && m_damageable.GetHealthNormalised() > 0f)
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

	private void SetAnimatorIsMovingBool(bool isMoving)
	{
		if (m_useAnimator)
		{
			m_animator.SetBool(m_isMovingBoolName, isMoving);
		}
	}
}
