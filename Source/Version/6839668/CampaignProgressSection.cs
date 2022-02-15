using UnityEngine;

public class CampaignProgressSection : MonoBehaviour
{
	[SerializeField]
	private GameObject m_disabledHierarchy;

	[SerializeField]
	private GameObject m_inprogressHierarchy;

	[SerializeField]
	private GameObject m_completeHierarchy;

	[SerializeField]
	private GameObject m_keyMissionInProgressHierarchy;

	[SerializeField]
	private GameObject m_keyMissionCompleteHierarchy;

	[SerializeField]
	private GameObject m_keyMissionDisabledHierarchy;

	private bool m_isSetUp;

	public void SetUp(bool inProgress, bool complete, bool keyAvailable)
	{
		if (complete)
		{
			m_disabledHierarchy.SetActive(value: false);
			m_inprogressHierarchy.SetActive(value: false);
			m_completeHierarchy.SetActive(value: true);
			if (m_keyMissionInProgressHierarchy != null)
			{
				m_keyMissionInProgressHierarchy.SetActive(value: false);
			}
			if (m_keyMissionDisabledHierarchy != null)
			{
				m_keyMissionDisabledHierarchy.SetActive(value: false);
			}
			if (m_keyMissionCompleteHierarchy != null)
			{
				m_keyMissionCompleteHierarchy.SetActive(value: true);
			}
		}
		else if (inProgress)
		{
			m_disabledHierarchy.SetActive(value: false);
			m_inprogressHierarchy.SetActive(value: true);
			m_completeHierarchy.SetActive(value: false);
			if (m_keyMissionInProgressHierarchy != null)
			{
				m_keyMissionInProgressHierarchy.SetActive(keyAvailable);
			}
			if (m_keyMissionDisabledHierarchy != null)
			{
				m_keyMissionDisabledHierarchy.SetActive(!keyAvailable);
			}
			if (m_keyMissionCompleteHierarchy != null)
			{
				m_keyMissionCompleteHierarchy.SetActive(value: false);
			}
		}
		else
		{
			m_disabledHierarchy.SetActive(value: true);
			m_inprogressHierarchy.SetActive(value: false);
			m_completeHierarchy.SetActive(value: false);
			if (m_keyMissionInProgressHierarchy != null)
			{
				m_keyMissionInProgressHierarchy.SetActive(value: false);
			}
			if (m_keyMissionDisabledHierarchy != null)
			{
				m_keyMissionDisabledHierarchy.SetActive(value: true);
			}
			if (m_keyMissionCompleteHierarchy != null)
			{
				m_keyMissionCompleteHierarchy.SetActive(value: false);
			}
		}
		m_isSetUp = true;
	}
}
