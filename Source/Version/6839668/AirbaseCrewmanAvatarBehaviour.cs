using System;
using BomberCrewCommon;
using UnityEngine;
using UnityEngine.AI;

public class AirbaseCrewmanAvatarBehaviour : MonoBehaviour
{
	[SerializeField]
	private GameObject m_navMeshAgentPrefab;

	[SerializeField]
	private float m_turnRate = 240f;

	[SerializeField]
	private float m_localiseLerpFactor = 10f;

	[SerializeField]
	private CrewmanGraphicsInstantiate m_crewmanGraphicsInstantiate;

	[SerializeField]
	private float m_walkSpeedBase = 5f;

	[SerializeField]
	private float m_walkAnimMultiplier = 0.5f;

	[SerializeField]
	private tk2dUIItem m_uiItem;

	private CrewmanGraphics m_crewmanGraphics;

	private Crewman m_crewman;

	private GameObject m_navMeshAgentToFollow;

	private bool m_isFollowing;

	private Action m_actionOnArrive;

	private Transform m_faceEndLerp;

	private float m_endLerpOverDistance;

	private float m_timeOutTime;

	private bool m_isInitialised;

	private bool m_localisingToTransform;

	private bool m_isSitting;

	private bool m_isWaiting;

	private float m_waitTime;

	public event Action<GameObject> OnClick;

	private void Start()
	{
		if (!m_isInitialised)
		{
			Init();
		}
	}

	public void SetCrewman(Crewman cm)
	{
		m_crewman = cm;
	}

	private void Init()
	{
		m_isInitialised = true;
		m_navMeshAgentToFollow = UnityEngine.Object.Instantiate(m_navMeshAgentPrefab);
		m_navMeshAgentToFollow.GetComponent<NavMeshAgent>().Warp(base.transform.position);
		m_navMeshAgentToFollow.transform.SetPositionAndRotation(base.transform.position, base.transform.rotation);
		m_crewmanGraphics = m_crewmanGraphicsInstantiate.GetCrewmanGraphics();
		if (m_uiItem != null)
		{
			m_uiItem.OnClick += ClickAirbaseCrewman;
		}
	}

	private void ClickAirbaseCrewman()
	{
		if (this.OnClick != null)
		{
			this.OnClick(base.gameObject);
		}
	}

	private void OnDestroy()
	{
		if (m_navMeshAgentToFollow != null)
		{
			UnityEngine.Object.Destroy(m_navMeshAgentToFollow);
		}
	}

	public void Talk()
	{
		Singleton<Jabberer>.Instance.StartJabber(m_crewman.GetJabberEvent(), null, base.gameObject, Jabberer.JabberSettings.Normal);
		m_crewmanGraphics.FacialAnimController.Talk(0.5f);
	}

	private void Update()
	{
		if (!m_isInitialised)
		{
			return;
		}
		if (m_isFollowing)
		{
			if (m_crewmanGraphics == null)
			{
				m_crewmanGraphics = m_crewmanGraphicsInstantiate.GetCrewmanGraphics();
			}
			m_crewmanGraphics.SetIsMoving(moving: true);
			NavMeshAgent component = m_navMeshAgentToFollow.GetComponent<NavMeshAgent>();
			m_crewmanGraphics.SetMoveSpeed(component.velocity.magnitude * m_walkAnimMultiplier);
			if (m_faceEndLerp == null)
			{
				base.transform.SetPositionAndRotation(m_navMeshAgentToFollow.transform.position, m_navMeshAgentToFollow.transform.rotation);
				base.transform.rotation = m_navMeshAgentToFollow.transform.rotation;
			}
			else
			{
				float t = Mathf.Clamp01(component.remainingDistance / m_endLerpOverDistance);
				base.transform.SetPositionAndRotation(m_navMeshAgentToFollow.transform.position, Quaternion.Lerp(m_faceEndLerp.rotation, m_navMeshAgentToFollow.transform.rotation, t));
			}
			m_timeOutTime -= Time.deltaTime;
			if ((!component.pathPending && component.remainingDistance < component.stoppingDistance && (!component.hasPath || component.velocity.magnitude < 0.1f)) || m_timeOutTime < 0f)
			{
				m_crewmanGraphics.SetIsMoving(moving: false);
				m_isFollowing = false;
				if (m_actionOnArrive != null)
				{
					Action actionOnArrive = m_actionOnArrive;
					m_actionOnArrive = null;
					actionOnArrive();
				}
			}
		}
		if (m_localisingToTransform)
		{
			m_crewmanGraphics.SetIsMoving(moving: false);
			base.transform.localPosition = Vector3.Lerp(base.transform.localPosition, Vector3.zero, m_localiseLerpFactor * Time.deltaTime);
			Quaternion localRotation = Quaternion.RotateTowards(base.transform.localRotation, Quaternion.identity, m_turnRate * Time.deltaTime);
			base.transform.localRotation = localRotation;
			float num = Vector3.Dot(base.transform.parent.forward, base.transform.forward);
			if (num > 0.98f && base.transform.localPosition.magnitude < 0.05f)
			{
				base.transform.localPosition = Vector3.zero;
				base.transform.localRotation = Quaternion.identity;
				m_localisingToTransform = false;
			}
		}
		if (m_isWaiting)
		{
			m_crewmanGraphics.SetIsMoving(moving: false);
			m_waitTime -= Time.deltaTime;
			if (m_waitTime < 0f)
			{
				m_isWaiting = false;
				if (m_actionOnArrive != null)
				{
					Action actionOnArrive2 = m_actionOnArrive;
					m_actionOnArrive = null;
					actionOnArrive2();
				}
			}
		}
		m_crewmanGraphics.SetIsAtStation(m_isSitting);
	}

	public void FaceDirection(Vector3 lookAt)
	{
		Vector3 view = lookAt - base.transform.position;
		view.y = 0f;
		base.transform.rotation.SetLookRotation(view, Vector3.up);
	}

	public void MoveToTransform(Transform t)
	{
		base.transform.parent = t;
		m_localisingToTransform = true;
	}

	public bool IsVisible()
	{
		return m_crewmanGraphics.IsVisibleRough(100f);
	}

	public void WarpTo(Vector3 worldPosition, Transform endDirection)
	{
		if (!m_isInitialised)
		{
			Init();
		}
		m_navMeshAgentToFollow.GetComponent<NavMeshAgent>().ResetPath();
		m_navMeshAgentToFollow.GetComponent<NavMeshAgent>().Warp(worldPosition);
		m_navMeshAgentToFollow.transform.SetPositionAndRotation(worldPosition, endDirection.rotation);
		base.transform.SetPositionAndRotation(worldPosition, endDirection.rotation);
		m_crewmanGraphics.SetIsMoving(moving: false);
		m_isFollowing = false;
		m_isWaiting = false;
		m_localisingToTransform = false;
		m_actionOnArrive = null;
		m_isSitting = false;
	}

	public void WalkTo(Vector3 worldPosition, Transform endDirection, float endLerpOverDist, float timeOut, Action andThenDo, float speedMultiplier)
	{
		if (!m_isInitialised)
		{
			Init();
		}
		m_navMeshAgentToFollow.GetComponent<NavMeshAgent>().ResetPath();
		m_navMeshAgentToFollow.GetComponent<NavMeshAgent>().Warp(base.transform.position);
		m_navMeshAgentToFollow.GetComponent<NavMeshAgent>().speed = speedMultiplier * m_walkSpeedBase;
		m_navMeshAgentToFollow.transform.SetPositionAndRotation(base.transform.position, base.transform.rotation);
		m_faceEndLerp = endDirection;
		m_endLerpOverDistance = endLerpOverDist;
		m_timeOutTime = timeOut;
		m_navMeshAgentToFollow.GetComponent<NavMeshAgent>().SetDestination(worldPosition);
		m_actionOnArrive = andThenDo;
		m_isFollowing = true;
		m_isSitting = false;
	}

	public void SetSitting()
	{
		m_isSitting = true;
	}

	public CrewmanGraphics GetCrewmanGraphics()
	{
		return m_crewmanGraphics;
	}

	public void Wait(float duration, Action andThenDo)
	{
		m_isSitting = false;
		m_isWaiting = true;
		m_waitTime = duration;
		m_actionOnArrive = andThenDo;
	}
}
