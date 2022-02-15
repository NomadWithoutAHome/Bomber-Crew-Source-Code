using UnityEngine;

public class BriefingMissionButton : MonoBehaviour
{
	[SerializeField]
	private TextSetter m_missionTitleTemp;

	[SerializeField]
	private GameObject m_keyMissionIndicator;

	[SerializeField]
	private TextSetter m_textNumbering;

	[SerializeField]
	private GameObject m_completedHierarchy;

	public void SetUp(CampaignStructure.CampaignMission cm, bool isComplete, int number)
	{
		if (m_missionTitleTemp != null)
		{
			m_missionTitleTemp.SetText(cm.m_missionReferenceName);
		}
		if (m_keyMissionIndicator != null)
		{
			m_keyMissionIndicator.SetActive(cm.m_isKeyMission);
		}
		if (m_completedHierarchy != null)
		{
			m_completedHierarchy.SetActive(isComplete);
		}
		if (m_textNumbering != null)
		{
			m_textNumbering.SetText(number.ToString());
		}
	}
}
