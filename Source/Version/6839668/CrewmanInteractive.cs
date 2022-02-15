using BomberCrewCommon;
using UnityEngine;

public class CrewmanInteractive : InteractiveItem
{
	[SerializeField]
	private CrewmanAvatar m_crewman;

	[SerializeField]
	private float m_healDuration = 5f;

	[SerializeField]
	private CrewmanGraphicsInstantiate m_crewmanGraphicsInstantiate;

	[SerializeField]
	private InMissionNotificationProvider m_notifications;

	[SerializeField]
	private GameObject m_disableIfCantHeal;

	[SerializeField]
	private CrewmanSkillAbilityBool m_healFirstAidSkill;

	[SerializeField]
	private CrewmanSkillAbilityBool m_healSlowSkill;

	private Interaction m_healInteraction;

	private Interaction m_healSlowSkillInteraction;

	private Interaction m_healFirstAidSkillInteraction;

	private CrewmanGraphics m_crewmanGraphics;

	private FlashManager.ActiveFlash m_healFlash;

	private float m_currentHealProgress;

	private bool m_isHealing;

	private InMissionNotification m_healingNotification;

	private bool m_isOutlined;

	private FlashManager.ActiveFlash m_healFlashOutline;

	private new void Start()
	{
		m_healInteraction = new Interaction("Heal", queryProgress: true, EnumToIconMapping.InteractionOrAlertType.Heal);
		m_healInteraction.SetMaterialIndex(2);
		m_healInteraction.SetInteractionAnimType(Interaction.InteractionAnimType.Healing);
		m_healInteraction.m_looseDistance = false;
		m_healSlowSkillInteraction = new Interaction("Heal", queryProgress: true, EnumToIconMapping.InteractionOrAlertType.Heal);
		m_healSlowSkillInteraction.SetMaterialIndex(2);
		m_healSlowSkillInteraction.SetInteractionAnimType(Interaction.InteractionAnimType.InteractingNormal);
		m_healSlowSkillInteraction.m_looseDistance = true;
		m_healFirstAidSkillInteraction = new Interaction("Heal", queryProgress: true, EnumToIconMapping.InteractionOrAlertType.Heal);
		m_healFirstAidSkillInteraction.SetMaterialIndex(2);
		m_healFirstAidSkillInteraction.SetInteractionAnimType(Interaction.InteractionAnimType.InteractingNormal);
		m_healFirstAidSkillInteraction.m_looseDistance = true;
		m_crewmanGraphics = m_crewmanGraphicsInstantiate.GetCrewmanGraphics();
	}

	public override Interaction GetInteractionOptions(CrewmanAvatar crewman)
	{
		bool flag = crewman.IsCarryingItem();
		if (flag && (bool)crewman.GetCarriedItem().GetComponent<MedKit>() && crewman != m_crewman && m_crewman.RequiresHealing())
		{
			return m_healInteraction;
		}
		if (crewman != m_crewman && !m_crewman.GetHealthState().IsDead() && !m_crewman.GetHealthState().IsCountingDown())
		{
			bool flag2 = flag && (bool)crewman.GetCarriedItem().GetComponent<MedKit>();
			bool flag3 = flag;
			if (m_crewman.GetHealthState().GetPhysicalHealthN() < 0.98f)
			{
				if (flag2)
				{
					if (Singleton<CrewmanSkillUpgradeInfo>.Instance.IsUnlocked(m_healFirstAidSkill, crewman.GetCrewman()))
					{
						return m_healFirstAidSkillInteraction;
					}
				}
				else if (!flag3 && Singleton<CrewmanSkillUpgradeInfo>.Instance.IsUnlocked(m_healSlowSkill, crewman.GetCrewman()))
				{
					return m_healSlowSkillInteraction;
				}
			}
		}
		return null;
	}

	private void Update()
	{
		m_disableIfCantHeal.SetActive(m_crewman.RequiresHealing());
		if (m_isOutlined != IsHovered())
		{
			m_isOutlined = IsHovered();
			if (m_isOutlined)
			{
				m_healFlashOutline = m_crewmanGraphics.GetOutlineFlash().AddOrUpdateFlash(0f, 0.5f, 0f, 255, 0.75f, Color.green, m_healFlashOutline);
			}
			else
			{
				m_crewmanGraphics.GetOutlineFlash().RemoveFlash(m_healFlashOutline);
			}
		}
	}

	public override float QueryProgress(Interaction interaction, CrewmanAvatar crewman)
	{
		if (interaction == m_healInteraction)
		{
			MedKit component = crewman.GetCarriedItem().GetComponent<MedKit>();
			if (component != null && !m_crewman.GetHealthState().IsDead())
			{
				m_healFlash = m_crewmanGraphics.GetFlashManager().AddOrUpdateFlash(0.1f, 2f, 1f, 255, 1f, Color.green, m_healFlash);
				m_crewman.GetHealthState().SetHealing();
				m_currentHealProgress += Time.deltaTime;
				float num = Mathf.Clamp01(1f - m_currentHealProgress / m_healDuration);
				if (m_healingNotification != null)
				{
					m_healingNotification.SetProgress(1f - num);
				}
				if (num <= 0f)
				{
					m_crewman.Heal(crewman.GetCrewman());
					component.Use();
				}
				return num;
			}
			return 0f;
		}
		if (interaction == m_healFirstAidSkillInteraction)
		{
			MedKit component2 = crewman.GetCarriedItem().GetComponent<MedKit>();
			if (component2 != null && !m_crewman.GetHealthState().IsDead())
			{
				if (m_crewman.GetHealthState().IsCountingDown())
				{
					m_healFirstAidSkillInteraction.SetInteractionAnimType(Interaction.InteractionAnimType.Healing);
					m_healFirstAidSkillInteraction.m_looseDistance = false;
				}
				else
				{
					m_healFirstAidSkillInteraction.SetInteractionAnimType(Interaction.InteractionAnimType.InteractingNormal);
					m_healFirstAidSkillInteraction.m_looseDistance = m_crewman.GetStation() == null;
				}
				m_healFlash = m_crewmanGraphics.GetFlashManager().AddOrUpdateFlash(0.1f, 2f, 1f, 255, 1f, Color.green, m_healFlash);
				m_crewman.GetHealthState().SetHealing();
				m_currentHealProgress += Time.deltaTime;
				float num2 = Mathf.Clamp01(1f - m_currentHealProgress / m_healDuration);
				if (m_healingNotification != null)
				{
					m_healingNotification.SetProgress(1f - num2);
				}
				if (num2 <= 0f)
				{
					m_crewman.Heal(crewman.GetCrewman());
					component2.Use();
				}
				return num2;
			}
			return 0f;
		}
		if (interaction == m_healSlowSkillInteraction)
		{
			if (!m_crewman.GetHealthState().IsDead() && !m_crewman.GetHealthState().IsCountingDown())
			{
				m_healSlowSkillInteraction.m_looseDistance = m_crewman.GetStation() == null;
				m_healFlash = m_crewmanGraphics.GetFlashManager().AddOrUpdateFlash(0.1f, 2f, 1f, 255, 1f, Color.green, m_healFlash);
				m_crewman.GetHealthState().SetHealing();
				float num3 = Mathf.Clamp01(1f - m_crewman.GetHealthState().GetPhysicalHealthN());
				m_crewman.HealFractional(crewman.GetCrewman(), Time.deltaTime * 0.0125f);
				if (m_healingNotification != null)
				{
					m_healingNotification.SetProgress(1f - num3);
				}
				return num3;
			}
			return 0f;
		}
		return 0f;
	}

	protected override void FinishInteraction(Interaction interaction, CrewmanAvatar crewman)
	{
		if (m_healingNotification != null)
		{
			m_notifications.RemoveNotification(m_healingNotification);
		}
	}

	protected override void AbandonInteraction(Interaction interaction, CrewmanAvatar crewman)
	{
		if (m_healingNotification != null)
		{
			m_notifications.RemoveNotification(m_healingNotification);
		}
	}

	protected override void BeginTimedInteraction(Interaction interaction, CrewmanAvatar crewman, InteractionContract contract)
	{
		m_currentHealProgress = 0f;
		if (m_healingNotification == null)
		{
			m_healingNotification = new InMissionNotification(string.Empty, StationNotificationUrgency.Yellow, EnumToIconMapping.InteractionOrAlertType.Heal);
			m_notifications.RegisterNotification(m_healingNotification);
			m_healingNotification.SetProgress(0f);
		}
	}
}
