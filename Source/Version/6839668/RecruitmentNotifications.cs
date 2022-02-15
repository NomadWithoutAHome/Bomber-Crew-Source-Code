using System.Collections.Generic;
using BomberCrewCommon;

public class RecruitmentNotifications : NotificationProvider
{
	public override List<Notification> GetNotifications()
	{
		List<Notification> list = new List<Notification>();
		if (Singleton<CrewContainer>.Instance.GetCurrentCrewCount() != Singleton<GameFlow>.Instance.GetGameMode().GetMaxCrew())
		{
			list.Add(new Notification(NotificationType.Red));
		}
		return list;
	}
}
