using UnityEngine;

public class CurrentMission : MonoBehaviour
{
	private MissionLog m_missionLog;

	private CampaignStructure.CampaignMission m_currentlySelectedMission;

	public LevelDescription GetCurrentlySelectedMission()
	{
		return m_currentlySelectedMission.m_level;
	}

	public CampaignStructure.CampaignMission GetCurrentlySelectedMissionDetails()
	{
		return m_currentlySelectedMission;
	}

	public void StartNewMission(CampaignStructure.CampaignMission cm)
	{
		m_currentlySelectedMission = cm;
		m_missionLog = new MissionLog();
	}

	public void SetCleared()
	{
		m_currentlySelectedMission = null;
		m_missionLog = null;
	}

	public MissionLog GetMissionLog()
	{
		return m_missionLog;
	}
}
