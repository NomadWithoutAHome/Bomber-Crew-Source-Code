using BomberCrewCommon;
using UnityEngine;

public class StationMedicalBed : Station
{
	[SerializeField]
	private tk2dSpriteAnimator m_animator;

	[SerializeField]
	private float m_startUpTime = 5f;

	[SerializeField]
	private float m_healUnitTime = 3f;

	[SerializeField]
	private float m_healUnit = 0.1f;

	private float m_healProgress;

	private bool m_hasStartedUp;

	private InMissionNotification m_healingNotification;

	private FlashManager.ActiveFlash m_healFlash;

	private bool m_isComplete;

	private Station m_initialStation;

	private Interaction m_sitInteractionLocal = new Interaction("Sit", queryProgress: false, EnumToIconMapping.InteractionOrAlertType.Main);

	public override void Start()
	{
		base.Start();
		if (Singleton<MissionCoordinator>.Instance == null)
		{
			base.enabled = false;
		}
	}

	private void OnDestroy()
	{
	}

	protected override void FinishInteraction(Interaction interaction, CrewmanAvatar crewman)
	{
		InteractionContract contractFor = GetContractFor(crewman);
		if (contractFor != null)
		{
			m_initialStation = contractFor.m_startingFromStation;
		}
		else
		{
			m_initialStation = null;
		}
		base.FinishInteraction(interaction, crewman);
	}

	public override Interaction GetInteractionOptions(CrewmanAvatar crewman)
	{
		if (GetCurrentCrewman() == null && !m_locked && crewman != null && crewman.GetHealthState().GetPhysicalHealthN() < 1f)
		{
			return m_sitInteractionLocal;
		}
		return null;
	}

	private void Update()
	{
		if (GetCurrentCrewman() != null)
		{
			CrewmanAvatar currentCrewman = GetCurrentCrewman();
			m_healFlash = GetCurrentCrewman().GetCrewmanGraphics().GetFlashManager().AddOrUpdateFlash(0.1f, 2f, 1f, 255, 1f, Color.green, m_healFlash);
			m_healProgress += Time.deltaTime;
			if (m_hasStartedUp)
			{
				if (m_healProgress > m_healUnitTime)
				{
					m_healProgress -= m_healUnitTime;
					currentCrewman.HealFractional(currentCrewman.GetCrewman(), m_healUnit);
				}
			}
			else if (m_healProgress > m_startUpTime)
			{
				m_hasStartedUp = true;
				m_healProgress -= m_startUpTime;
			}
			if (currentCrewman.GetHealthState().GetPhysicalHealthN() >= 1f && !m_isComplete)
			{
				m_isComplete = true;
				bool flag = false;
				if (m_initialStation != null)
				{
					Interaction interactionOptionsPublic = m_initialStation.GetInteractionOptionsPublic(currentCrewman, skipNullCheck: true);
					if (interactionOptionsPublic != null)
					{
						currentCrewman.QueueAction(andCancelExisting: false, m_initialStation, interactionOptionsPublic);
						flag = true;
					}
				}
				else
				{
					GetCurrentCrewman().SetStationSlow(null);
				}
				if (flag)
				{
					Singleton<SpeechPrioritiser>.Instance.RequestNewSpeech(StaticSpeechSets.InjuriesHealedReturned, currentCrewman);
				}
				else
				{
					Singleton<SpeechPrioritiser>.Instance.RequestNewSpeech(StaticSpeechSets.InjuriesHealed, currentCrewman);
				}
			}
			if (m_healingNotification == null)
			{
				m_healingNotification = new InMissionNotification(string.Empty, StationNotificationUrgency.Yellow, EnumToIconMapping.InteractionOrAlertType.Heal);
				GetNotifications().RegisterNotification(m_healingNotification);
			}
			m_healingNotification.SetProgress(currentCrewman.GetHealthState().GetPhysicalHealthN());
		}
		else
		{
			m_isComplete = false;
			m_hasStartedUp = false;
			if (m_healingNotification != null)
			{
				GetNotifications().RemoveNotification(m_healingNotification);
				m_healingNotification = null;
			}
			m_healProgress = 0f;
		}
	}
}
