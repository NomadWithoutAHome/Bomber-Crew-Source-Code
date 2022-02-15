using BomberCrewCommon;
using UnityEngine;

public class InMissionNotificationSingleDisplay : MonoBehaviour
{
	[SerializeField]
	private GameObject m_enableHierarchy;

	[SerializeField]
	private GameObject m_redTriangle;

	[SerializeField]
	private GameObject m_yellow;

	[SerializeField]
	private tk2dRadialSprite m_radialProgress;

	[SerializeField]
	private tk2dSprite[] m_sprites;

	[SerializeField]
	private WorldToScreenTracker m_tracker;

	private InMissionNotificationProvider m_provider;

	private bool m_hideIfCrewmanSelected;

	public void SetUp(InMissionNotificationProvider s, Transform trackingTransform, bool hideIfCrewmanSelected)
	{
		m_hideIfCrewmanSelected = hideIfCrewmanSelected;
		m_provider = s;
		m_tracker.SetTracking(trackingTransform, tk2dCamera.CameraForLayer(base.gameObject.layer));
		Refresh();
	}

	private void Update()
	{
		Refresh();
	}

	private void Refresh()
	{
		bool flag = true;
		if (m_hideIfCrewmanSelected && Singleton<ContextControl>.Instance.GetCurrentlySelected() != null)
		{
			flag = false;
		}
		if (Singleton<BomberCamera>.Instance.ShouldShowStationOverlays() && flag)
		{
			InMissionNotification inMissionNotification = null;
			foreach (InMissionNotification notification in m_provider.GetNotifications())
			{
				if (inMissionNotification == null || notification.m_urgency == StationNotificationUrgency.Red)
				{
					inMissionNotification = notification;
				}
			}
			if (inMissionNotification != null)
			{
				m_enableHierarchy.SetActive(value: true);
				m_tracker.enabled = true;
				if (inMissionNotification.m_progress == 0f && inMissionNotification.m_urgency == StationNotificationUrgency.Red)
				{
					m_redTriangle.SetActive(value: true);
					m_yellow.SetActive(value: false);
				}
				else
				{
					m_redTriangle.SetActive(value: false);
					m_yellow.SetActive(value: true);
				}
				m_radialProgress.SetValue(1f - inMissionNotification.m_progress);
				tk2dSprite[] sprites = m_sprites;
				foreach (tk2dSprite tk2dSprite2 in sprites)
				{
					tk2dSprite2.SetSprite(Singleton<EnumToIconMapping>.Instance.GetIconName(inMissionNotification.m_iconType));
				}
			}
			else
			{
				m_enableHierarchy.SetActive(value: false);
				m_tracker.enabled = false;
			}
		}
		else
		{
			m_enableHierarchy.SetActive(value: false);
			m_tracker.enabled = false;
		}
	}
}
