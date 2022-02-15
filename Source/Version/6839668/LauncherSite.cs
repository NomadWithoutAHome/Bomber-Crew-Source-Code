using System;
using System.Collections.Generic;
using System.Globalization;
using BomberCrewCommon;
using UnityEngine;

public class LauncherSite : MonoBehaviour
{
	[SerializeField]
	private GameObject m_prefabToLaunch;

	[SerializeField]
	private Transform m_trackingHeirarchy;

	[SerializeField]
	private Animation m_animationToFire;

	[SerializeField]
	private float m_launchTimer;

	[SerializeField]
	private SmoothDamageable m_activeDamageable;

	[SerializeField]
	private MissionPlaceableObject m_placeable;

	[SerializeField]
	private float m_onlyFireWithinDistance;

	[SerializeField]
	private bool m_useFighterLookup;

	[SerializeField]
	private bool m_showWarningIcon;

	[SerializeField]
	private GameObject m_warningPrefab;

	[SerializeField]
	private float m_countdownUnderTimer;

	[SerializeField]
	private bool m_doSpeechEventStart;

	[SerializeField]
	private CrewMiscSpeechTriggers.SpeechExternalTrigger m_startTrigger;

	[SerializeField]
	private bool m_doSpeechEventLaunch;

	[SerializeField]
	private CrewMiscSpeechTriggers.SpeechExternalTrigger m_launchTrigger;

	[SerializeField]
	private bool m_onlyInRange;

	[SerializeField]
	private bool m_includeAnimTime;

	[SerializeField]
	private float m_animTimeTrim;

	private bool m_isLaunchInProgress;

	private float m_timerCountdown;

	private bool m_isStarted;

	private GameObject m_currentLaunchable;

	private GameObject m_warningInstance;

	private string m_waitForTrigger;

	private string m_triggerOnLaunch;

	private int m_launchNumber;

	private int m_maxToLaunch;

	private int m_maxAtOnce;

	private bool m_hasDoneStartSpeech;

	private bool m_hasDoneMusicTrigger;

	private List<GameObject> m_currentActiveObjects = new List<GameObject>(8);

	private void Start()
	{
		m_timerCountdown = m_launchTimer;
		m_waitForTrigger = m_placeable.GetParameter("launcherTrigger");
		m_triggerOnLaunch = m_placeable.GetParameter("triggerOnLaunch");
		m_maxToLaunch = 0;
		int.TryParse(m_placeable.GetParameter("maxToLaunch"), out m_maxToLaunch);
		m_maxAtOnce = 0;
		int.TryParse(m_placeable.GetParameter("maxAtOnce"), out m_maxAtOnce);
		string parameter = m_placeable.GetParameter("launcherTimer");
		if (!string.IsNullOrEmpty(parameter))
		{
			float num = float.Parse(parameter, CultureInfo.InvariantCulture);
			if (num != 0f)
			{
				m_launchTimer = num;
			}
		}
		if (string.IsNullOrEmpty(m_waitForTrigger))
		{
			m_isStarted = true;
		}
		else
		{
			MissionCoordinator instance = Singleton<MissionCoordinator>.Instance;
			instance.OnTrigger = (Action<string>)Delegate.Combine(instance.OnTrigger, (Action<string>)delegate(string v)
			{
				if (v == m_waitForTrigger)
				{
					m_isStarted = true;
				}
			});
		}
		if (m_showWarningIcon)
		{
			m_warningInstance = UnityEngine.Object.Instantiate(m_warningPrefab);
			m_warningInstance.GetComponent<HazardPreviewWarning>().SetTracker(Singleton<TagManager>.Instance.GetUICamera(), base.gameObject, 50f);
			m_warningInstance.SetActive(value: false);
		}
	}

	public void Stop()
	{
		m_isStarted = false;
	}

	private void Update()
	{
		if (m_isStarted && (m_launchNumber < m_maxToLaunch || m_maxToLaunch == 0) && (m_activeDamageable == null || m_activeDamageable.GetHealthNormalised() > 0f))
		{
			Vector3 position = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState().transform.position;
			if ((position - base.transform.position).magnitude < 2750f)
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
		}
		else if (m_hasDoneMusicTrigger)
		{
			Singleton<MusicSelectionRules>.Instance.Untrigger(MusicSelectionRules.MusicTriggerEvents.StealthHazardNearish);
			m_hasDoneMusicTrigger = false;
		}
		bool active = false;
		if (!m_isLaunchInProgress)
		{
			if (m_isStarted && CanLaunch() && (m_activeDamageable == null || m_activeDamageable.GetHealthNormalised() > 0f))
			{
				m_timerCountdown -= Time.deltaTime;
				if (m_timerCountdown < m_countdownUnderTimer && !m_hasDoneStartSpeech && m_doSpeechEventStart)
				{
					Singleton<CrewMiscSpeechTriggers>.Instance.DoExternalTrigger(m_startTrigger, null);
					m_hasDoneStartSpeech = true;
				}
				float num = ((!m_includeAnimTime) ? m_timerCountdown : (m_animationToFire[m_animationToFire.clip.name].length - m_animationToFire[m_animationToFire.clip.name].time + m_timerCountdown - m_animTimeTrim));
				if (num < m_countdownUnderTimer && m_showWarningIcon)
				{
					active = true;
					m_warningInstance.GetComponent<HazardPreviewWarning>().SetCountdown(num / m_countdownUnderTimer, num);
				}
				if (m_timerCountdown < 0f)
				{
					if (!string.IsNullOrEmpty(m_triggerOnLaunch))
					{
						Singleton<MissionCoordinator>.Instance.FireTrigger(m_triggerOnLaunch);
						m_triggerOnLaunch = null;
					}
					GameObject gameObject = null;
					if (m_useFighterLookup)
					{
						string parameter = m_placeable.GetParameter("groupType");
						string[] array = ((!string.IsNullOrEmpty(parameter)) ? parameter.Split(",".ToCharArray()) : new string[1] { Singleton<FighterCoordinator>.Instance.GetDefaultFighterId() });
						gameObject = Singleton<FighterCoordinator>.Instance.GetFighterForId(array[m_launchNumber % array.Length]);
					}
					else
					{
						gameObject = m_prefabToLaunch;
					}
					m_currentLaunchable = UnityEngine.Object.Instantiate(gameObject);
					m_currentLaunchable.transform.parent = m_trackingHeirarchy;
					m_currentLaunchable.transform.localPosition = Vector3.zero;
					m_currentLaunchable.transform.localRotation = Quaternion.identity;
					m_currentLaunchable.GetComponent<Launchable>().SetLaunched(launched: false);
					FighterPlane component = m_currentLaunchable.GetComponent<FighterPlane>();
					if (component != null)
					{
						component.SetShouldHunt();
					}
					m_animationToFire.Play();
					if (m_doSpeechEventLaunch)
					{
						Singleton<CrewMiscSpeechTriggers>.Instance.DoExternalTrigger(m_launchTrigger, null);
					}
					m_launchNumber++;
					m_timerCountdown = m_launchTimer;
					m_hasDoneStartSpeech = false;
					m_isLaunchInProgress = true;
					m_currentActiveObjects.Add(m_currentLaunchable);
				}
			}
		}
		else if (!m_animationToFire.isPlaying)
		{
			if (m_currentLaunchable != null)
			{
				m_currentLaunchable.transform.parent = null;
				m_currentLaunchable.GetComponent<Launchable>().SetLaunched(launched: true);
				m_currentLaunchable = null;
			}
			m_isLaunchInProgress = false;
		}
		else if (m_currentLaunchable != null)
		{
			float num2 = m_animationToFire[m_animationToFire.clip.name].length - m_animationToFire[m_animationToFire.clip.name].time - m_animTimeTrim;
			if (num2 > 0f && num2 < m_countdownUnderTimer && m_showWarningIcon)
			{
				active = m_includeAnimTime;
				m_warningInstance.GetComponent<HazardPreviewWarning>().SetCountdown(num2 / m_countdownUnderTimer, num2);
			}
		}
		if (m_showWarningIcon)
		{
			m_warningInstance.SetActive(active);
		}
	}

	private bool CanLaunch()
	{
		while (m_currentActiveObjects.Contains(null))
		{
			m_currentActiveObjects.Remove(null);
		}
		if ((m_maxToLaunch == 0 || m_launchNumber < m_maxToLaunch) && (m_maxAtOnce == 0 || m_currentActiveObjects.Count < m_maxAtOnce))
		{
			if (!m_onlyInRange)
			{
				return true;
			}
			float num = ((m_onlyFireWithinDistance != 0f) ? m_onlyFireWithinDistance : 4000f);
			Vector3 vector = Singleton<BomberSpawn>.Instance.GetBomberSystems().transform.position - base.transform.position;
			vector.y = 0f;
			if (vector.magnitude < num)
			{
				return true;
			}
		}
		return false;
	}
}
