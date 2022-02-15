using BomberCrewCommon;
using UnityEngine;

public class StationPilot : Station
{
	[SerializeField]
	private BomberNavigation m_navigation;

	[SerializeField]
	private float m_pilotNavigationTimeMax;

	[SerializeField]
	private GameObject m_abilityActiveHierarchy;

	[SerializeField]
	private BomberState m_bomberState;

	private float m_pilotNavigationTimer;

	private bool m_hasSpoken;

	private InMissionNotification m_needNavigationNotification;

	private InMissionNotification m_needPilotNotification;

	private bool m_hasAnnouncedReturn;

	private float m_announceReturnDelay = 10f;

	private InMissionNotification m_hydraulicsNotification;

	private bool m_inactiveLocked;

	public void SetInactiveLocked(bool inactiveLocked)
	{
		m_inactiveLocked = inactiveLocked;
	}

	private void Update()
	{
		CrewmanAvatar currentCrewman = GetCurrentCrewman();
		if (currentCrewman != null)
		{
			if (m_needPilotNotification != null)
			{
				GetNotifications().RemoveNotification(m_needPilotNotification);
				m_needPilotNotification = null;
			}
			if (m_navigation.GetNextNavigationPoint() == null && !m_inactiveLocked)
			{
				m_pilotNavigationTimer += Time.deltaTime;
			}
			else
			{
				m_pilotNavigationTimer = 0f;
			}
			BomberSystems bomberSystems = Singleton<BomberSpawn>.Instance.GetBomberSystems();
			if (bomberSystems.GetLandingGear().IsAffectedByHydraulics() || bomberSystems.GetBombBayDoors().IsAffectedByHydraulics())
			{
				if (m_hydraulicsNotification == null)
				{
					m_hydraulicsNotification = new InMissionNotification("notification_hydraulics_out", StationNotificationUrgency.Red, EnumToIconMapping.InteractionOrAlertType.Hydraulics);
					GetNotifications().RegisterNotification(m_hydraulicsNotification);
					if (bomberSystems.GetLandingGear().IsAffectedByHydraulics())
					{
						Singleton<SpeechPrioritiser>.Instance.RequestNewSpeech(StaticSpeechSets.LandingGearHydraulicsOut, currentCrewman);
					}
					else
					{
						Singleton<SpeechPrioritiser>.Instance.RequestNewSpeech(StaticSpeechSets.BayDoorsHydraulicsOut, currentCrewman);
					}
				}
			}
			else if (m_hydraulicsNotification != null)
			{
				GetNotifications().RemoveNotification(m_hydraulicsNotification);
				m_hydraulicsNotification = null;
			}
			if (m_pilotNavigationTimer > m_pilotNavigationTimeMax)
			{
				if (!m_hasSpoken && currentCrewman != null)
				{
					Singleton<SpeechPrioritiser>.Instance.RequestNewSpeech(StaticSpeechSets.PilotNavigationRequired, currentCrewman);
					m_hasSpoken = true;
				}
				if (m_needNavigationNotification == null)
				{
					m_needNavigationNotification = new InMissionNotification("station_notification_navigation", StationNotificationUrgency.Red, EnumToIconMapping.InteractionOrAlertType.Navigate);
					GetNotifications().RegisterNotification(m_needNavigationNotification);
				}
			}
			else
			{
				m_hasSpoken = false;
				if (m_needNavigationNotification != null)
				{
					GetNotifications().RemoveNotification(m_needNavigationNotification);
					m_needNavigationNotification = null;
				}
			}
			if (!Singleton<MissionCoordinator>.Instance.IsOutwardJourney())
			{
				if (m_hasAnnouncedReturn)
				{
				}
			}
			else if (m_hasAnnouncedReturn)
			{
				Singleton<SpeechPrioritiser>.Instance.RequestNewSpeech(StaticSpeechSets.ContinuingMission, currentCrewman);
				m_hasAnnouncedReturn = false;
			}
			if (m_bomberState.IsAbilityActive())
			{
				m_abilityActiveHierarchy.SetActive(value: true);
			}
			else
			{
				m_abilityActiveHierarchy.SetActive(value: false);
			}
		}
		else
		{
			m_abilityActiveHierarchy.SetActive(value: false);
			m_hasSpoken = false;
			if (m_needNavigationNotification != null)
			{
				GetNotifications().RemoveNotification(m_needNavigationNotification);
				m_needNavigationNotification = null;
			}
			if (m_needPilotNotification == null)
			{
				m_needPilotNotification = new InMissionNotification("station_notification_pilot", StationNotificationUrgency.Red, EnumToIconMapping.InteractionOrAlertType.Pilot);
				GetNotifications().RegisterNotification(m_needPilotNotification);
			}
		}
	}
}
