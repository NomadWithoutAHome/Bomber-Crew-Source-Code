using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class CrewTrainingCrewmanNameButton : NotificationProvider
{
	[SerializeField]
	private TextSetter m_crewmanName;

	[SerializeField]
	private tk2dSprite m_roleSprite;

	[SerializeField]
	private AirbaseNotificationDisplay m_notificationDisplay;

	private Crewman m_crewman;

	public void SetUp(Crewman c)
	{
		m_crewman = c;
		m_notificationDisplay.SetUp(this);
		Refresh();
	}

	public void Refresh()
	{
		m_crewmanName.SetText(m_crewman.GetFirstName()[0] + " " + m_crewman.GetSurname());
		m_roleSprite.SetSprite(Singleton<EnumToIconMapping>.Instance.GetIconName(m_crewman.GetPrimarySkill().GetSkill()));
	}

	public override List<Notification> GetNotifications()
	{
		List<Notification> list = new List<Notification>();
		if (m_crewman.GetSecondarySkill() == null && m_crewman.GetPrimarySkill().GetLevel() >= Singleton<CrewmanSkillUpgradeInfo>.Instance.GetSecondarySkillUnlocksAt(m_crewman.GetPrimarySkill().GetSkill()))
		{
			list.Add(new Notification(NotificationType.New));
		}
		return list;
	}
}
