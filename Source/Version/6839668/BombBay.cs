using System;
using BomberCrewCommon;
using UnityEngine;

public class BombBay : MonoBehaviour
{
	[SerializeField]
	private GameObject m_defaultBombLoadPrefab;

	[SerializeField]
	private BombBayDoors m_doors;

	private BombLoad m_bombLoad;

	public event Action OnBombLoadTypeChange;

	private void Awake()
	{
		CampaignStructure.CampaignMission currentlySelectedMissionDetails = Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetCurrentlySelectedMissionDetails();
		GameObject gameObject = ((!(currentlySelectedMissionDetails.m_bombLoadConfig == null)) ? currentlySelectedMissionDetails.m_bombLoadConfig.GetBombLoadPrefab() : null);
		if (gameObject == null)
		{
			gameObject = m_defaultBombLoadPrefab;
		}
		GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject);
		m_bombLoad = gameObject2.GetComponent<BombLoad>();
		gameObject2.transform.parent = base.transform;
		gameObject2.transform.localPosition = Vector3.zero;
		gameObject2.transform.rotation = Quaternion.identity;
		if (m_bombLoad.ShouldRemoveBayDoors())
		{
			m_doors.RemoveDoors();
		}
	}

	public void Reset()
	{
		if (m_bombLoad != null)
		{
			UnityEngine.Object.Destroy(m_bombLoad.gameObject);
		}
		CampaignStructure.CampaignMission currentlySelectedMissionDetails = Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetCurrentlySelectedMissionDetails();
		GameObject gameObject = ((!(currentlySelectedMissionDetails.m_bombLoadConfig == null)) ? currentlySelectedMissionDetails.m_bombLoadConfig.GetBombLoadPrefab() : null);
		if (gameObject == null)
		{
			gameObject = m_defaultBombLoadPrefab;
		}
		GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject);
		m_bombLoad = gameObject2.GetComponent<BombLoad>();
		gameObject2.transform.parent = base.transform;
		gameObject2.transform.localPosition = Vector3.zero;
		gameObject2.transform.localRotation = Quaternion.identity;
		if (m_bombLoad.ShouldRemoveBayDoors())
		{
			m_doors.RemoveDoors();
		}
		else
		{
			m_doors.DontRemoveDoors();
		}
	}

	public void SetBombLoadType(BombLoadConfig newConfig)
	{
		CampaignStructure.CampaignMission currentlySelectedMissionDetails = Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetCurrentlySelectedMissionDetails();
		if (newConfig != currentlySelectedMissionDetails.m_bombLoadConfig)
		{
			currentlySelectedMissionDetails.m_bombLoadConfig = newConfig;
			Reset();
			if (this.OnBombLoadTypeChange != null)
			{
				this.OnBombLoadTypeChange();
			}
		}
		else
		{
			Reset();
		}
	}

	public BombLoad GetBombLoad()
	{
		return m_bombLoad;
	}
}
