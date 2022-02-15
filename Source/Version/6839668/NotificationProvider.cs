using System.Collections.Generic;
using UnityEngine;

public abstract class NotificationProvider : MonoBehaviour
{
	public enum NotificationType
	{
		New,
		Yellow,
		Red
	}

	public class Notification
	{
		public string m_text;

		public NotificationType m_urgency;

		public Notification(NotificationType urgency)
		{
			m_urgency = urgency;
		}
	}

	public abstract List<Notification> GetNotifications();

	public bool HasNotifications(out NotificationType highestUrgency)
	{
		List<Notification> notifications = GetNotifications();
		bool result = false;
		highestUrgency = NotificationType.New;
		foreach (Notification item in notifications)
		{
			result = true;
			highestUrgency = (NotificationType)Mathf.Max((int)item.m_urgency, (int)highestUrgency);
		}
		return result;
	}
}
