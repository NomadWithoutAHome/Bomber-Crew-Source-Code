using Common;
using UnityEngine;

public class AirbaseNotificationDisplay : MonoBehaviour
{
	[SerializeField]
	private GameObject m_alertIcon;

	[SerializeField]
	private GameObject m_newIcon;

	[SerializeField]
	private GameObject m_yellowIcon;

	[SerializeField]
	private SelectableFilterButton m_filterButton;

	private NotificationProvider m_notifications;

	private bool m_notificationsActive;

	private NotificationProvider.NotificationType m_currentShownType;

	private void Awake()
	{
		m_filterButton.OnRefresh += Refresh;
	}

	private void Refresh()
	{
		NotificationProvider.NotificationType highestUrgency = m_currentShownType;
		bool flag = !(m_notifications == null) && m_notifications.HasNotifications(out highestUrgency);
		if (m_notificationsActive != flag || highestUrgency != m_currentShownType)
		{
			m_notificationsActive = flag;
			m_currentShownType = highestUrgency;
			if (m_alertIcon != null)
			{
				m_alertIcon.CustomActivate(m_notificationsActive && m_currentShownType == NotificationProvider.NotificationType.Red);
			}
			if (m_newIcon != null)
			{
				m_newIcon.CustomActivate(m_notificationsActive && m_currentShownType == NotificationProvider.NotificationType.New);
			}
		}
	}

	public void SetUp(NotificationProvider notificationProvider)
	{
		m_notifications = notificationProvider;
		Refresh();
	}
}
