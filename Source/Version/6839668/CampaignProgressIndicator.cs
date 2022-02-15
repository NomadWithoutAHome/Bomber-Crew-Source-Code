using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class CampaignProgressIndicator : MonoBehaviour
{
	[SerializeField]
	private CampaignProgressSection[] m_allProgressSections;

	[SerializeField]
	private string[] m_keysToLookFor;

	[SerializeField]
	private bool m_ignoreBrt;

	private void Start()
	{
		Refresh();
	}

	public void Refresh()
	{
		SaveData saveData = Singleton<SaveDataContainer>.Instance.Get();
		List<string> keysUnlocked = saveData.GetKeysUnlocked(Singleton<GameFlow>.Instance.GetCampaign());
		bool flag = keysUnlocked.Contains("BRT") || m_ignoreBrt;
		CampaignStructure.CampaignMission[] allMissions = Singleton<GameFlow>.Instance.GetCampaign().GetAllMissions();
		int num = 0;
		CampaignStructure.CampaignMission[] array = allMissions;
		foreach (CampaignStructure.CampaignMission campaignMission in array)
		{
			if (campaignMission.m_isKeyMission && campaignMission.m_showKeyMissionTitle && keysUnlocked.Contains(campaignMission.m_tagRequired) && campaignMission.m_minIntelRequired <= Singleton<SaveDataContainer>.Instance.Get().GetIntel())
			{
				num++;
			}
		}
		for (int j = 0; j < m_keysToLookFor.Length; j++)
		{
			if (keysUnlocked.Contains(m_keysToLookFor[j]))
			{
				m_allProgressSections[j].SetUp(inProgress: false, complete: true, keyAvailable: true);
				flag = true;
			}
			else if (flag)
			{
				m_allProgressSections[j].SetUp(inProgress: true, complete: false, j < num);
				flag = false;
			}
			else
			{
				m_allProgressSections[j].SetUp(inProgress: false, complete: false, keyAvailable: false);
			}
		}
	}
}
