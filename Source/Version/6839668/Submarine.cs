using System;
using BomberCrewCommon;
using UnityEngine;

public class Submarine : MonoBehaviour
{
	[SerializeField]
	private MonoBehaviour[] m_toDisableWhenSubmergedBehaviours;

	[SerializeField]
	private Collider[] m_toDisableWhenSubmergedColliders;

	[SerializeField]
	private GameObject m_submergeYMovementHierarchy;

	[SerializeField]
	private float m_submergeDelayTimeMin;

	[SerializeField]
	private float m_submergeDelayTimeMax;

	[SerializeField]
	private float m_maxRepositionRangeFromOriginalPosition;

	[SerializeField]
	private float m_submergeUnderWaterTimeMin;

	[SerializeField]
	private float m_submergeUnderWaterTimeMax;

	[SerializeField]
	private float m_submergeSpeed;

	[SerializeField]
	private float m_submergedDistance;

	[SerializeField]
	private TaggableItem m_taggableItem;

	[SerializeField]
	private SmoothDamageable m_damageableToMonitor;

	[SerializeField]
	private Animator m_animator;

	[SerializeField]
	private ParticleSystem m_submergeParticles;

	private ParticleSystem.EmissionModule m_submergeParticlesEmission;

	[SerializeField]
	private MissionPlaceableObject m_placeable;

	private float m_currentCountdown;

	private bool m_isSubmerged;

	private float m_submergeT;

	private bool m_isFullyAtState;

	private Vector3d m_startingPosition;

	private Vector3d m_targetPos;

	private bool m_wantsToLeave;

	private bool m_submergeBlocked;

	private bool m_neverComeBack;

	private void Start()
	{
		m_isFullyAtState = true;
		m_startingPosition = base.gameObject.btransform().position;
		m_submergeParticlesEmission = m_submergeParticles.emission;
		m_submergeParticlesEmission.enabled = false;
		string dontComeBackTrigger = m_placeable.GetParameter("leaveTrigger");
		if (string.IsNullOrEmpty(dontComeBackTrigger))
		{
			return;
		}
		MissionCoordinator instance = Singleton<MissionCoordinator>.Instance;
		instance.OnTrigger = (Action<string>)Delegate.Combine(instance.OnTrigger, (Action<string>)delegate(string st)
		{
			if (st == dontComeBackTrigger)
			{
				m_currentCountdown = Mathf.Min(m_currentCountdown, 1f);
				m_wantsToLeave = true;
				m_neverComeBack = true;
			}
		});
	}

	private void SetThingsActive(bool a)
	{
		MonoBehaviour[] toDisableWhenSubmergedBehaviours = m_toDisableWhenSubmergedBehaviours;
		foreach (MonoBehaviour monoBehaviour in toDisableWhenSubmergedBehaviours)
		{
			monoBehaviour.enabled = a;
		}
		m_taggableItem.SetTaggable(a);
		if (!a)
		{
			m_taggableItem.SetTagIncomplete();
		}
	}

	private void SetThingsActiveFull(bool a)
	{
		Collider[] toDisableWhenSubmergedColliders = m_toDisableWhenSubmergedColliders;
		foreach (Collider collider in toDisableWhenSubmergedColliders)
		{
			collider.enabled = a;
		}
	}

	public bool WantsToLeave()
	{
		return m_wantsToLeave;
	}

	public void SetSubmergeBlocked(bool blocked)
	{
		if (!blocked && m_submergeBlocked)
		{
			m_currentCountdown = Mathf.Max(m_currentCountdown, 5f);
		}
		m_submergeBlocked = blocked;
	}

	private void Update()
	{
		if (!(m_damageableToMonitor == null) && !(m_damageableToMonitor.GetHealthNormalised() > 0f))
		{
			return;
		}
		if (!m_isFullyAtState)
		{
			if (m_isSubmerged)
			{
				m_submergeT += Time.deltaTime * m_submergeSpeed;
				if (m_submergeT >= 1f)
				{
					m_animator.SetBool("Submerge", value: false);
					m_submergeParticlesEmission.enabled = false;
					m_submergeT = 1f;
					m_isFullyAtState = true;
					SetThingsActiveFull(a: false);
				}
				m_submergeYMovementHierarchy.transform.localPosition = new Vector3(0f, (0f - m_submergedDistance) * m_submergeT, 0f);
			}
			else
			{
				m_submergeT -= Time.deltaTime * m_submergeSpeed;
				if (m_submergeT <= 0f)
				{
					m_animator.SetBool("Surface", value: false);
					m_submergeParticlesEmission.enabled = false;
					m_submergeT = 0f;
					m_isFullyAtState = true;
					SetThingsActive(a: true);
					SetThingsActiveFull(a: true);
				}
				m_submergeYMovementHierarchy.transform.localPosition = new Vector3(0f, (0f - m_submergedDistance) * m_submergeT, 0f);
			}
		}
		if (!m_isFullyAtState)
		{
			return;
		}
		if (!m_isSubmerged)
		{
			if (!m_submergeBlocked)
			{
				m_currentCountdown -= Time.deltaTime;
			}
			if (m_currentCountdown < 0f && !m_submergeBlocked)
			{
				if (m_taggableItem.IsFullyTagged())
				{
					Singleton<CrewMiscSpeechTriggers>.Instance.DoExternalTrigger(CrewMiscSpeechTriggers.SpeechExternalTrigger.SubmarineSubmergeTagged, null);
				}
				m_animator.SetBool("Submerge", value: true);
				m_submergeParticlesEmission.enabled = true;
				m_isSubmerged = true;
				m_isFullyAtState = false;
				m_submergeT = 0f;
				Vector2 insideUnitCircle = UnityEngine.Random.insideUnitCircle;
				m_targetPos = m_startingPosition + new Vector3d(insideUnitCircle.x * m_maxRepositionRangeFromOriginalPosition, 0f, insideUnitCircle.y * m_maxRepositionRangeFromOriginalPosition);
				SetThingsActive(a: false);
				m_currentCountdown = UnityEngine.Random.Range(m_submergeUnderWaterTimeMin, m_submergeUnderWaterTimeMax);
			}
		}
		else if (!m_neverComeBack)
		{
			if (!m_submergeBlocked)
			{
				m_currentCountdown -= Time.deltaTime;
			}
			if (m_currentCountdown < 0f && !m_submergeBlocked)
			{
				m_animator.SetBool("Surface", value: true);
				m_submergeParticlesEmission.enabled = true;
				m_isSubmerged = false;
				m_isFullyAtState = false;
				m_submergeT = 1f;
				base.gameObject.btransform().position = m_targetPos;
				m_currentCountdown = UnityEngine.Random.Range(m_submergeDelayTimeMin, m_submergeDelayTimeMax);
			}
		}
	}
}
