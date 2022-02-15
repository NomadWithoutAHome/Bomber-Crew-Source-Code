using System.Collections;
using System.Collections.Generic;
using BomberCrewCommon;
using Common;
using UnityEngine;

public class ShowGameCompleteMissionCompletedInfo : MonoBehaviour
{
	[SerializeField]
	private TextSetter m_missionName;

	[SerializeField]
	private TextSetter m_bomberName;

	[SerializeField]
	private Renderer m_bomberPortraitRenderer;

	[SerializeField]
	private GameObject m_bomberActivateHierarchy;

	[SerializeField]
	private LayoutGrid[] m_layoutGrids;

	[SerializeField]
	private int[] m_crewPerLayoutGrid;

	[SerializeField]
	private GameObject m_disableLayout;

	[SerializeField]
	[NamedText]
	private string m_firstCrewText;

	[SerializeField]
	private GameObject m_crewmanDisplay;

	[SerializeField]
	private float m_crewDisplayInterval = 0.1f;

	[SerializeField]
	private float m_showBomberDelay = 1f;

	[SerializeField]
	private float m_showCrewDelay = 0.2f;

	public void SetUpFor(CampaignStructure cs, SaveData.CrewCompletedMission ccm)
	{
		StartCoroutine(SetUpForCo(cs, ccm));
	}

	private IEnumerator SetUpForCo(CampaignStructure cs, SaveData.CrewCompletedMission ccm)
	{
		m_disableLayout.SetActive(value: false);
		m_bomberActivateHierarchy.SetActive(value: false);
		m_bomberName.SetText(ccm.m_bomberUpgradeConfig.GetName());
		if (ccm.m_missionReference != "BombRunTraining")
		{
			CampaignStructure.CampaignMission[] allMissions = cs.GetAllMissions();
			CampaignStructure.CampaignMission[] array = allMissions;
			foreach (CampaignStructure.CampaignMission campaignMission in array)
			{
				if (campaignMission.m_missionReferenceName == ccm.m_missionReference)
				{
					m_missionName.SetTextFromLanguageString(campaignMission.m_titleNamedText);
					break;
				}
			}
		}
		else
		{
			m_missionName.SetTextFromLanguageString(m_firstCrewText);
		}
		BomberUpgradeConfig buc = ccm.m_bomberUpgradeConfig;
		RenderTexture rt = Singleton<BomberPhotoBooth>.Instance.RenderForBomber(buc);
		m_bomberPortraitRenderer.material.mainTexture = rt;
		while (Singleton<BomberPhotoBooth>.Instance.IsProcessing())
		{
			yield return null;
		}
		int layoutIndex = 0;
		int subIndex = 0;
		List<ShowGameCompleteCrewmanInfo> cinfo = new List<ShowGameCompleteCrewmanInfo>();
		Crewman[] crewCompletedWith = ccm.m_crewCompletedWith;
		foreach (Crewman up in crewCompletedWith)
		{
			GameObject gameObject = Object.Instantiate(m_crewmanDisplay);
			gameObject.transform.parent = m_layoutGrids[layoutIndex].transform;
			ShowGameCompleteCrewmanInfo component = gameObject.GetComponent<ShowGameCompleteCrewmanInfo>();
			component.SetUp(up);
			cinfo.Add(component);
			subIndex++;
			if (subIndex == m_crewPerLayoutGrid[layoutIndex])
			{
				layoutIndex++;
				subIndex = 0;
			}
			gameObject.SetActive(value: false);
		}
		LayoutGrid[] layoutGrids = m_layoutGrids;
		foreach (LayoutGrid layoutGrid in layoutGrids)
		{
			layoutGrid.RepositionChildren();
		}
		while (Singleton<CrewmanPhotoBooth>.Instance.IsProcessing())
		{
			yield return null;
		}
		m_disableLayout.SetActive(value: true);
		yield return new WaitForSeconds(m_showBomberDelay);
		m_bomberActivateHierarchy.CustomActivate(active: true);
		yield return new WaitForSeconds(m_showCrewDelay);
		for (int l = 0; l < cinfo.Count; l++)
		{
			cinfo[l].gameObject.CustomActivate(active: true, m_crewDisplayInterval * (float)l);
		}
	}
}
