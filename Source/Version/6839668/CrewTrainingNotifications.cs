using System.Collections.Generic;
using BomberCrewCommon;

public class CrewTrainingNotifications : NotificationProvider
{
	public override List<Notification> GetNotifications()
	{
		List<Notification> list = new List<Notification>();
		int currentCrewCount = Singleton<CrewContainer>.Instance.GetCurrentCrewCount();
		for (int i = 0; i < currentCrewCount; i++)
		{
			Crewman crewman = Singleton<CrewContainer>.Instance.GetCrewman(i);
			if (crewman.GetSecondarySkill() == null && crewman.GetPrimarySkill().GetLevel() >= Singleton<CrewmanSkillUpgradeInfo>.Instance.GetSecondarySkillUnlocksAt(crewman.GetPrimarySkill().GetSkill()))
			{
				list.Add(new Notification(NotificationType.New));
			}
		}
		return list;
	}
}
