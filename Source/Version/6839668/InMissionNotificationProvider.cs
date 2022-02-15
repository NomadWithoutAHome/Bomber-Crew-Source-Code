using System.Collections.Generic;
using UnityEngine;

public class InMissionNotificationProvider : MonoBehaviour
{
	[SerializeField]
	private GameObject m_alertTrackerPrefab;

	[SerializeField]
	private Transform m_notificationTrackingTransform;

	[SerializeField]
	private bool m_dontShowIfCrewmanSelected;

	private List<InMissionNotification> m_notifications = new List<InMissionNotification>();

	private int m_notificationUpdateCount;

	private GameObject m_alertTracker;

	public void RegisterNotification(InMissionNotification sn)
	{
		m_notifications.Add(sn);
		m_notificationUpdateCount++;
	}

	public void RemoveNotification(InMissionNotification sn)
	{
		m_notifications.Remove(sn);
		m_notificationUpdateCount++;
	}

	public List<InMissionNotification> GetNotifications()
	{
		return m_notifications;
	}

	public int GetNotificationUpdateCounter()
	{
		return m_notificationUpdateCount;
	}

	public virtual void Awake()
	{
		m_alertTracker = Object.Instantiate(m_alertTrackerPrefab);
		m_alertTracker.GetComponent<InMissionNotificationSingleDisplay>().SetUp(this, (!(m_notificationTrackingTransform == null)) ? m_notificationTrackingTransform : base.transform, m_dontShowIfCrewmanSelected);
	}
}
