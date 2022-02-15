using System.Collections.Generic;
using UnityEngine;

public class CrewmenIconsDisplay : MonoBehaviour
{
	[SerializeField]
	private GameObject m_iconDisplayPrefab;

	[SerializeField]
	private LayoutGrid m_layoutGrid;

	private CrewmanAvatar m_forCrewman;

	private int m_currentVersion = -1;

	private List<GameObject> m_iconDisplays = new List<GameObject>();

	public void SetUp(CrewmanAvatar crewman)
	{
		m_forCrewman = crewman;
		Refresh();
	}

	private void Update()
	{
		Refresh();
	}

	private void Refresh()
	{
		int notificationsVersion = m_forCrewman.GetNotificationsVersion();
		if (notificationsVersion != m_currentVersion)
		{
			m_currentVersion = notificationsVersion;
			List<CrewmanAvatar.CrewmanNotification> allNotifications = m_forCrewman.GetAllNotifications();
			while (m_iconDisplays.Count < allNotifications.Count)
			{
				GameObject gameObject = Object.Instantiate(m_iconDisplayPrefab);
				gameObject.transform.parent = m_layoutGrid.transform;
				m_iconDisplays.Add(gameObject);
			}
			while (m_iconDisplays.Count > allNotifications.Count)
			{
				Object.DestroyImmediate(m_iconDisplays[0]);
				m_iconDisplays.RemoveAt(0);
			}
			for (int i = 0; i < allNotifications.Count; i++)
			{
				m_iconDisplays[0].GetComponent<CrewmanStatusAlert>().SetUp(allNotifications[i].m_iconName);
			}
			m_layoutGrid.RepositionChildren();
		}
	}
}
