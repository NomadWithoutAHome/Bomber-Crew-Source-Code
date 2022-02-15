using AudioNames;
using BomberCrewCommon;
using UnityEngine;
using WingroveAudio;

public class RepairableInteractive : InteractiveItem
{
	[SerializeField]
	private SmoothDamageableRepairable m_damageable;

	[SerializeField]
	private FlashManager m_flashManager;

	[SerializeField]
	private float m_repairTimeMax = 40f;

	[SerializeField]
	private float m_repairTimeMin = 20f;

	[SerializeField]
	private InMissionNotificationProvider m_notifications;

	[SerializeField]
	private CrewmanSkillAbilityFloat m_repairSkill;

	[SerializeField]
	private Interaction.InteractionAnimType m_animType;

	[SerializeField]
	private GameObject m_enableIfBrokenOnly;

	private Interaction m_repairInteraction;

	private Interaction m_fireInteraction;

	private InMissionNotification m_repairNotification;

	private bool m_isRepairing;

	private bool m_isRepairAudioPaused;

	private bool m_isRepairPaused;

	private InteractionContract m_contract;

	private bool m_detached;

	private int m_cachedGameObjectId;

	private string m_repairSkillId;

	private void Awake()
	{
		m_repairSkillId = m_repairSkill.GetCachedName();
		m_cachedGameObjectId = base.gameObject.GetInstanceID();
		m_repairInteraction = new Interaction("Repair", m_repairTimeMax, EnumToIconMapping.InteractionOrAlertType.Repair);
		m_repairInteraction.SetMaterialIndex(1);
		m_repairInteraction.SetInteractionAnimType(m_animType);
		m_fireInteraction = new Interaction("Fire", 0f, EnumToIconMapping.InteractionOrAlertType.Extinguish);
		m_fireInteraction.SetMaterialIndex(3);
		m_fireInteraction.SetFinishRemotely();
		BomberDestroyableSection bomberDestroyableSection = BomberDestroyableSection.FindDestroyableSectionFor(base.transform);
		if (bomberDestroyableSection != null)
		{
			bomberDestroyableSection.OnSectionDestroy += OnSectionDestroy;
		}
	}

	private void OnSectionDestroy()
	{
		m_detached = true;
		if (m_repairNotification != null)
		{
			m_notifications.RemoveNotification(m_repairNotification);
			m_repairNotification = null;
		}
	}

	public override Interaction GetInteractionOptions(CrewmanAvatar crewman)
	{
		if (m_damageable is FireOverview.Extinguishable)
		{
			FireOverview.Extinguishable extinguishable = (FireOverview.Extinguishable)m_damageable;
			if (extinguishable.IsOnFire() && crewman.IsCarryingItem() && crewman.GetCarriedItem().GetComponent<FireExtinguisher>() != null)
			{
				return m_fireInteraction;
			}
		}
		if (m_damageable.CanBeRepaired())
		{
			float proficiency = Singleton<CrewmanSkillUpgradeInfo>.Instance.GetProficiency(m_repairSkillId, crewman.GetCrewman().GetPrimarySkill(), crewman.GetCrewman().GetSecondarySkill());
			float duration = Mathf.Lerp(m_repairTimeMax, m_repairTimeMin, proficiency);
			m_repairInteraction.m_duration = duration;
			m_repairInteraction.SetAnimationMultiplier(proficiency * 2f);
			return m_repairInteraction;
		}
		return null;
	}

	private void Update()
	{
		if (m_detached)
		{
			return;
		}
		if (m_isRepairing)
		{
			if (m_repairNotification == null)
			{
				m_repairNotification = new InMissionNotification(null, StationNotificationUrgency.Red, EnumToIconMapping.InteractionOrAlertType.Repairing);
				m_notifications.RegisterNotification(m_repairNotification);
			}
			m_repairInteraction.m_iconType = EnumToIconMapping.InteractionOrAlertType.Repairing;
			m_repairNotification.SetIcon(EnumToIconMapping.InteractionOrAlertType.Repairing);
			m_repairNotification.SetProgress(m_contract.GetValue());
			if (m_isRepairPaused)
			{
				if (!m_isRepairAudioPaused)
				{
					WingroveRoot.Instance.PostEventGO("REPAIR_CANCEL", base.gameObject);
					m_isRepairAudioPaused = true;
				}
				m_isRepairPaused = false;
			}
			else if (m_isRepairAudioPaused)
			{
				WingroveRoot.Instance.PostEventGO("REPAIR_START", base.gameObject);
				m_isRepairAudioPaused = false;
			}
			if (m_enableIfBrokenOnly != null)
			{
				m_enableIfBrokenOnly.SetActive(value: false);
			}
			return;
		}
		if (m_enableIfBrokenOnly != null)
		{
			if (m_damageable.CanBeRepaired())
			{
				m_enableIfBrokenOnly.SetActive(value: true);
			}
			else
			{
				m_enableIfBrokenOnly.SetActive(value: false);
			}
		}
		if (m_damageable.NeedsRepair())
		{
			if (m_repairNotification == null)
			{
				m_repairNotification = new InMissionNotification(null, StationNotificationUrgency.Red, EnumToIconMapping.InteractionOrAlertType.Repair);
				m_notifications.RegisterNotification(m_repairNotification);
			}
			m_repairNotification.SetIcon(EnumToIconMapping.InteractionOrAlertType.Repair);
			m_repairNotification.SetProgress(0f);
		}
		else if (m_repairInteraction != null)
		{
			m_notifications.RemoveNotification(m_repairNotification);
			m_repairNotification = null;
		}
		m_repairInteraction.m_iconType = EnumToIconMapping.InteractionOrAlertType.Repair;
	}

	protected override void AbandonInteraction(Interaction interaction, CrewmanAvatar crewman)
	{
		WingroveRoot.Instance.PostEventGO("REPAIR_CANCEL", base.gameObject);
		m_isRepairing = false;
		m_damageable.AbandonRepair();
	}

	public override void InteractionPaused(InteractionContract ic)
	{
		if (ic.m_interaction == m_repairInteraction)
		{
			m_isRepairPaused = true;
		}
	}

	protected override void BeginTimedInteraction(Interaction interaction, CrewmanAvatar crewman, InteractionContract contract)
	{
		if (interaction == m_repairInteraction)
		{
			float proficiency = Singleton<CrewmanSkillUpgradeInfo>.Instance.GetProficiency(m_repairSkillId, crewman.GetCrewman().GetPrimarySkill(), crewman.GetCrewman().GetSecondarySkill());
			WingroveRoot.Instance.PostEventGO("REPAIR_START", base.gameObject);
			WingroveRoot.Instance.SetParameterForObject(GameEvents.Parameters.CacheVal_RepairSkill(), m_cachedGameObjectId, base.gameObject, proficiency);
			m_isRepairing = true;
			m_contract = contract;
			m_damageable.StartRepair();
		}
		else
		{
			m_contract = null;
		}
	}

	protected override void FinishInteraction(Interaction interaction, CrewmanAvatar crewman)
	{
		if (interaction == m_repairInteraction)
		{
			WingroveRoot.Instance.PostEventGO("REPAIR_FINISH", base.gameObject);
			Singleton<SpeechPrioritiser>.Instance.RequestNewSpeech(StaticSpeechSets.RepairComplete, crewman);
			m_isRepairing = false;
			m_damageable.Repair();
		}
		else
		{
			crewman.QueueFireMode(andCancelOthers: true, (FireOverview.Extinguishable)m_damageable);
		}
	}
}
