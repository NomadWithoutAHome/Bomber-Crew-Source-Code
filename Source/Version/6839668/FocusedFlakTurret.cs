using System;
using BomberCrewCommon;
using UnityEngine;
using WingroveAudio;

public class FocusedFlakTurret : MonoBehaviour
{
	[SerializeField]
	private float m_maxRange;

	[SerializeField]
	private GameObject m_flakTargetPreviewSpherePrefab;

	[SerializeField]
	private float m_timeFromSpotToFlakExplode;

	[SerializeField]
	private float m_refireTime;

	[SerializeField]
	private float m_maxAltitude;

	[SerializeField]
	private float m_minAltitude;

	[SerializeField]
	private ParticleSystem m_muzzleFlash;

	[SerializeField]
	private MissionPlaceableObject m_placeableObject;

	private BomberSystems m_bomber;

	private float m_rechargeTimer;

	private Vector3 m_stableVelocity;

	private float m_stableTime;

	private bool m_areaEnteredTrigger;

	private bool m_switchedOff;

	private void Start()
	{
		m_bomber = Singleton<BomberSpawn>.Instance.GetBomberSystems();
		m_stableVelocity = Vector3.up;
		m_rechargeTimer = m_refireTime;
		if (m_placeableObject != null)
		{
			string activateTrigger = m_placeableObject.GetParameter("activateTrigger");
			if (!string.IsNullOrEmpty(activateTrigger))
			{
				m_switchedOff = true;
				MissionCoordinator instance = Singleton<MissionCoordinator>.Instance;
				instance.OnTrigger = (Action<string>)Delegate.Combine(instance.OnTrigger, (Action<string>)delegate(string st)
				{
					if (st == activateTrigger)
					{
						m_switchedOff = false;
					}
				});
			}
		}
		m_muzzleFlash.Stop(withChildren: true);
	}

	private void Update()
	{
		m_rechargeTimer -= Time.deltaTime;
		if (Singleton<VisibilityHelpers>.Instance.IsVisibleGeneric(base.transform.position, m_bomber.transform.position) && m_bomber.transform.position.y > base.transform.position.y + m_minAltitude && m_bomber.transform.position.y < base.transform.position.y + m_maxAltitude && (m_bomber.transform.position - base.transform.position).magnitude < m_maxRange && !m_switchedOff)
		{
			Vector3 velocity = m_bomber.GetBomberState().GetPhysicsModel().GetVelocity();
			if (Vector3.Dot(m_stableVelocity.normalized, velocity.normalized) > 0.99f)
			{
				m_stableTime += Time.deltaTime;
			}
			else
			{
				m_stableTime = 0f;
				m_stableVelocity = velocity;
			}
			if (m_stableTime > 8f && m_rechargeTimer < 0f)
			{
				m_rechargeTimer = m_refireTime;
				Vector3 fromCurrentPage = m_bomber.transform.position + velocity * m_timeFromSpotToFlakExplode;
				GameObject gameObject = UnityEngine.Object.Instantiate(m_flakTargetPreviewSpherePrefab);
				gameObject.btransform().SetFromCurrentPage(fromCurrentPage);
				m_muzzleFlash.Play(withChildren: true);
				WingroveRoot.Instance.PostEventGO("FLAK_EXPLODE", base.gameObject);
				Singleton<CrewMiscSpeechTriggers>.Instance.DoExternalTrigger(CrewMiscSpeechTriggers.SpeechExternalTrigger.FlakTargetedAppear, null);
				gameObject.GetComponent<FocusedFlakTimedExplosion>().SetUp(m_timeFromSpotToFlakExplode);
				m_stableTime = 0f;
			}
			if (!m_areaEnteredTrigger)
			{
				Singleton<MusicSelectionRules>.Instance.Trigger(MusicSelectionRules.MusicTriggerEvents.HazardNearBy);
				m_areaEnteredTrigger = true;
			}
		}
		else if (m_areaEnteredTrigger)
		{
			Singleton<MusicSelectionRules>.Instance.Untrigger(MusicSelectionRules.MusicTriggerEvents.HazardNearBy);
			m_areaEnteredTrigger = false;
		}
	}
}
