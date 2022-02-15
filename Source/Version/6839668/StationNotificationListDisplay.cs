using System.Collections.Generic;
using UnityEngine;

public class StationNotificationListDisplay : MonoBehaviour
{
	[SerializeField]
	private GameObject m_alertPrefab;

	[SerializeField]
	private LayoutGrid m_layoutGrid;

	private List<GameObject> m_spawnedItems = new List<GameObject>();

	private Station m_station;

	private int m_lastVersionUpdated;

	public void SetStation(Station s)
	{
		m_station = s;
		Refresh();
	}

	public void Update()
	{
		if (m_station != null)
		{
			int notificationUpdateCounter = m_station.GetNotifications().GetNotificationUpdateCounter();
			if (notificationUpdateCounter != m_lastVersionUpdated)
			{
				Refresh();
			}
		}
	}

	private void Refresh()
	{
		foreach (GameObject spawnedItem in m_spawnedItems)
		{
			Object.DestroyImmediate(spawnedItem);
		}
		m_spawnedItems.Clear();
		List<InMissionNotification> notifications = m_station.GetNotifications().GetNotifications();
		foreach (InMissionNotification item in notifications)
		{
			if (item.m_urgency == StationNotificationUrgency.Red)
			{
				GameObject gameObject = Object.Instantiate(m_alertPrefab);
				gameObject.transform.parent = base.transform;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localScale = Vector3.one;
				gameObject.GetComponent<NotificationDisplay>().SetUp(item);
				m_spawnedItems.Add(gameObject);
			}
		}
		m_layoutGrid.RepositionChildren();
		m_lastVersionUpdated = m_station.GetNotifications().GetNotificationUpdateCounter();
	}
}
