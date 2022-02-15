using System;
using BomberCrewCommon;
using UnityEngine;

public class SearchLight : MonoBehaviour
{
	[SerializeField]
	private Transform m_rotationNode;

	[SerializeField]
	private float m_angMax = 45f;

	[SerializeField]
	private Transform m_raycastForwardNode;

	[SerializeField]
	private Transform m_lengthScalerNode;

	[SerializeField]
	private LayerMask m_raycastLayers;

	[SerializeField]
	private float m_maxDistance;

	[SerializeField]
	private float m_guidanceDistanceXZ;

	[SerializeField]
	private float m_guidanceIncreaseSpeed;

	[SerializeField]
	private MissionPlaceableObject m_placeable;

	[SerializeField]
	private float m_timeToAlert = 10f;

	[SerializeField]
	private GameObject m_lightEnableNode;

	private BomberSystems m_bomber;

	private float m_seenBomberCountdown;

	private float m_joinTimer;

	private float m_t;

	private float m_guidance;

	private bool m_canSeeBomber;

	private float m_seenBomberFor;

	private bool m_hasTriggered;

	private string m_turnOffTrigger;

	private string m_turnOnTrigger;

	private bool m_turnedOff;

	private float m_lockRotationTimer;

	private bool m_hasDoneMusicTrigger;

	private void Start()
	{
		m_t = UnityEngine.Random.Range(0f, 100f);
		if (!(m_placeable != null))
		{
			return;
		}
		m_turnOffTrigger = m_placeable.GetParameter("switchOffTrigger");
		if (!string.IsNullOrEmpty(m_turnOffTrigger))
		{
			MissionCoordinator instance = Singleton<MissionCoordinator>.Instance;
			instance.OnTrigger = (Action<string>)Delegate.Combine(instance.OnTrigger, (Action<string>)delegate(string st)
			{
				if (st == m_turnOffTrigger)
				{
					m_turnedOff = true;
				}
			});
		}
		m_turnOnTrigger = m_placeable.GetParameter("switchOnTrigger");
		if (string.IsNullOrEmpty(m_turnOnTrigger))
		{
			return;
		}
		m_turnedOff = true;
		MissionCoordinator instance2 = Singleton<MissionCoordinator>.Instance;
		instance2.OnTrigger = (Action<string>)Delegate.Combine(instance2.OnTrigger, (Action<string>)delegate(string st)
		{
			if (st == m_turnOnTrigger)
			{
				m_turnedOff = false;
			}
		});
	}

	private void LateUpdate()
	{
		bool flag = (double)Singleton<VisibilityHelpers>.Instance.GetNightFactor() > 0.75 && !m_turnedOff;
		m_lightEnableNode.SetActive(flag);
		if (m_bomber == null)
		{
			m_bomber = Singleton<BomberSpawn>.Instance.GetBomberSystems();
		}
		float num = m_maxDistance;
		if (Physics.Raycast(m_raycastForwardNode.transform.position, m_raycastForwardNode.forward, out var hitInfo, m_maxDistance, m_raycastLayers))
		{
			num = hitInfo.distance;
		}
		Quaternion identity = Quaternion.identity;
		m_t += Time.deltaTime;
		m_seenBomberCountdown -= Time.deltaTime;
		Vector3 vector = m_bomber.transform.position - m_raycastForwardNode.position;
		if (Vector3.Dot(vector.normalized, m_raycastForwardNode.forward) > 0.975f && vector.magnitude < num + 15f)
		{
			m_seenBomberCountdown = 3f;
		}
		if (vector.magnitude < m_maxDistance * 1.5f && flag)
		{
			if (!m_hasDoneMusicTrigger)
			{
				Singleton<MusicSelectionRules>.Instance.Trigger(MusicSelectionRules.MusicTriggerEvents.StealthHazardNearish);
				m_hasDoneMusicTrigger = true;
			}
		}
		else if (m_hasDoneMusicTrigger)
		{
			Singleton<MusicSelectionRules>.Instance.Untrigger(MusicSelectionRules.MusicTriggerEvents.StealthHazardNearish);
			m_hasDoneMusicTrigger = false;
		}
		bool flag2 = m_seenBomberCountdown > 0f;
		if (!m_canSeeBomber && !m_bomber.GetBomberState().IsEvasive())
		{
			if (!m_bomber.IsLitUp() || !(vector.magnitude < m_maxDistance))
			{
				m_joinTimer = 0f;
			}
			else if (m_bomber.GetLitUpCounter() < 3)
			{
				m_joinTimer += Time.deltaTime;
				if (m_joinTimer > 3f)
				{
					m_seenBomberCountdown = 3f;
					flag2 = true;
				}
			}
		}
		if (!flag)
		{
			flag2 = false;
		}
		if (m_lockRotationTimer > 6f)
		{
			flag2 = false;
		}
		if (m_canSeeBomber != flag2)
		{
			m_canSeeBomber = flag2;
			if (flag2)
			{
				m_bomber.AddSearchLight();
			}
			else
			{
				m_bomber.RemoveSearchLight();
			}
		}
		Vector3 vector2 = vector;
		vector2.y = 0f;
		if (vector2.magnitude < m_guidanceDistanceXZ && m_lockRotationTimer <= 0f)
		{
			m_guidance += Time.deltaTime * m_guidanceIncreaseSpeed;
		}
		else
		{
			m_guidance -= Time.deltaTime;
			if (m_guidance < 0f)
			{
				m_guidance = 0f;
			}
		}
		if (m_bomber.GetBomberState().IsEvasive())
		{
			m_guidance = 0f;
			m_joinTimer = 0f;
			m_lockRotationTimer += Time.deltaTime * 4f;
			if (m_lockRotationTimer > 12f)
			{
				m_lockRotationTimer = 12f;
			}
		}
		m_lockRotationTimer -= Time.deltaTime;
		if (m_lockRotationTimer < 0f)
		{
			m_lockRotationTimer = 0f;
		}
		if (!m_canSeeBomber)
		{
			identity = Quaternion.LookRotation(new Vector3(Mathf.Sin(m_t * 0.5f), 2f + Mathf.Sin(m_t * 0.2f), Mathf.Cos(m_t * 0.5f)));
			if (!m_bomber.GetBomberState().IsEvasive())
			{
				Quaternion b = Quaternion.LookRotation(vector);
				identity = Quaternion.Lerp(identity, b, m_guidance);
			}
			m_seenBomberFor = 0f;
		}
		else
		{
			identity = Quaternion.LookRotation(vector);
			if (m_bomber.GetBomberState().IsEvasive())
			{
				Quaternion b2 = Quaternion.LookRotation(new Vector3(0f - vector.x, vector.y, 0f - vector.z));
				identity = Quaternion.Lerp(identity, b2, 0.125f);
				m_seenBomberCountdown = 0f;
			}
			if (Vector3.Dot(vector.normalized, m_raycastForwardNode.forward) > 0.975f && vector.magnitude < m_maxDistance)
			{
				num = vector.magnitude;
			}
			Singleton<BomberEffectLight>.Instance.AddLighting(1.5f, Color.white, base.transform);
			if (m_seenBomberFor < m_timeToAlert && !m_hasTriggered)
			{
				m_seenBomberFor += Time.deltaTime;
				if (m_seenBomberFor > m_timeToAlert)
				{
					string parameter = m_placeable.GetParameter("triggerOnAlert");
					if (!string.IsNullOrEmpty(parameter))
					{
						Singleton<MissionCoordinator>.Instance.FireTrigger(parameter);
					}
					m_hasTriggered = true;
				}
			}
		}
		float t = Mathf.Clamp01(1f - m_lockRotationTimer / 12f);
		m_rotationNode.rotation = Quaternion.RotateTowards(m_rotationNode.rotation, identity, Time.deltaTime * Mathf.Lerp(4f, 20f, t));
		m_lengthScalerNode.transform.localScale = new Vector3(1f, 1f, 15f + num * 0.5f);
	}

	public bool CanSeeBomber()
	{
		return m_canSeeBomber;
	}
}
