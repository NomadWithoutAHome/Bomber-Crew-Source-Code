using System;
using System.Globalization;
using BomberCrewCommon;
using UnityEngine;

public class RadarSiteTrigger : MonoBehaviour
{
	[SerializeField]
	private MissionPlaceableObject m_placeable;

	[SerializeField]
	private GameObject m_hazardWarningPrefab;

	[SerializeField]
	private float m_countdownTimerStart;

	[SerializeField]
	private float m_showUnder;

	[SerializeField]
	private MapNavigatorDisplayable m_mapDisplay;

	[SerializeField]
	private SmoothDamageable m_damageable;

	[SerializeField]
	private GameObject m_alertedEffect;

	private GameObject m_hazardWarning;

	private float m_timer;

	private float m_radius;

	private string m_trigger;

	private string m_startTrigger;

	private string m_disableTrigger;

	private bool m_hasFiredTrigger;

	private bool m_hasFiredStartTrigger;

	private bool m_hasDamageable;

	private bool m_hasDoneMusicTrigger;

	private bool m_hasSwitchedOffNavigationMarker;

	private void Start()
	{
		m_hasDamageable = m_damageable != null;
		m_radius = (float)Convert.ToDouble(m_placeable.GetParameter("radius"), CultureInfo.InvariantCulture);
		m_mapDisplay.SetRadius(m_radius);
		m_startTrigger = m_placeable.GetParameter("startTrigger");
		m_trigger = m_placeable.GetParameter("trigger");
		m_disableTrigger = m_placeable.GetParameter("deactivate");
		if (!string.IsNullOrEmpty(m_disableTrigger))
		{
			MissionCoordinator instance = Singleton<MissionCoordinator>.Instance;
			instance.OnTrigger = (Action<string>)Delegate.Combine(instance.OnTrigger, new Action<string>(DisableTrigger));
		}
		m_hazardWarning = UnityEngine.Object.Instantiate(m_hazardWarningPrefab);
		m_hazardWarning.GetComponent<HazardPreviewWarning>().SetTracker(Singleton<TagManager>.Instance.GetUICamera(), base.gameObject, 200f);
		m_hazardWarning.SetActive(value: false);
		m_timer = m_countdownTimerStart;
		m_alertedEffect.SetActive(value: false);
	}

	private void DisableTrigger(string trigger)
	{
		if (trigger == m_disableTrigger)
		{
			m_hasFiredTrigger = true;
			m_alertedEffect.SetActive(value: false);
		}
	}

	private void Update()
	{
		Vector3d position = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBigTransform().position;
		Vector3d vector3d = base.gameObject.btransform().position - position;
		vector3d.y = 0.0;
		if (vector3d.magnitude < (double)(m_radius * 1.5f))
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
		if (!m_hasFiredTrigger)
		{
			if (m_hasDamageable && m_damageable.GetHealthNormalised() == 0f)
			{
				m_hazardWarning.SetActive(value: false);
				m_alertedEffect.SetActive(value: false);
			}
			else if (vector3d.magnitude < (double)m_radius)
			{
				if (m_timer < m_showUnder)
				{
					if (!m_hasFiredStartTrigger)
					{
						if (!string.IsNullOrEmpty(m_startTrigger))
						{
							Singleton<MissionCoordinator>.Instance.FireTrigger(m_startTrigger);
						}
						m_hasFiredStartTrigger = true;
						Singleton<CrewMiscSpeechTriggers>.Instance.DoExternalTrigger(CrewMiscSpeechTriggers.SpeechExternalTrigger.RadarSiteStart, null);
					}
					m_hazardWarning.SetActive(value: true);
					m_hazardWarning.GetComponent<HazardPreviewWarning>().SetCountdown(m_timer / m_countdownTimerStart, m_timer);
				}
				else
				{
					m_hazardWarning.SetActive(value: false);
				}
				m_timer -= Time.deltaTime;
				if (m_timer < 0f)
				{
					m_hasFiredTrigger = true;
					Singleton<MissionCoordinator>.Instance.FireTrigger(m_trigger);
					Singleton<CrewMiscSpeechTriggers>.Instance.DoExternalTrigger(CrewMiscSpeechTriggers.SpeechExternalTrigger.RadarSiteFinish, null);
					m_alertedEffect.SetActive(value: true);
				}
			}
			else
			{
				m_timer = m_countdownTimerStart;
				m_hazardWarning.SetActive(value: false);
			}
		}
		else
		{
			m_hazardWarning.SetActive(value: false);
			if (!m_hasSwitchedOffNavigationMarker)
			{
				m_hasSwitchedOffNavigationMarker = true;
				GetComponent<MapNavigatorDisplayable>().SetShouldDisplay(shouldDisplay: false);
			}
		}
	}
}
