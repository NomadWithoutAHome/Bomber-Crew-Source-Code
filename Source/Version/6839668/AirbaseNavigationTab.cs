using UnityEngine;

public class AirbaseNavigationTab : MonoBehaviour
{
	[SerializeField]
	private TextSetter m_areaLabel;

	[SerializeField]
	private tk2dUIItem m_uiItem;

	[SerializeField]
	private AirbaseNotificationDisplay m_notification;

	private bool m_notificationsActive;

	public void SetUp(AirbaseAreaScreen navArea)
	{
		m_areaLabel.SetTextFromLanguageString(navArea.GetNamedTextReference());
		m_notification.SetUp(navArea.GetUIScreen().GetComponent<NotificationProvider>());
	}
}
