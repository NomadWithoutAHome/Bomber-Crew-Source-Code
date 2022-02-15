public class InMissionNotification
{
	public string m_text;

	public StationNotificationUrgency m_urgency;

	public float m_progress;

	public EnumToIconMapping.InteractionOrAlertType m_iconType;

	public InMissionNotification(string languageString, StationNotificationUrgency urgency, EnumToIconMapping.InteractionOrAlertType iconType)
	{
		m_text = languageString;
		m_urgency = urgency;
		m_iconType = iconType;
	}

	public void SetIcon(EnumToIconMapping.InteractionOrAlertType iconType)
	{
		m_iconType = iconType;
	}

	public void SetProgress(float progress)
	{
		m_progress = progress;
	}
}
