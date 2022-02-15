using System;
using System.Collections;
using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;
using WingroveAudio;

public class MissionCoordinator : Singleton<MissionCoordinator>
{
	[SerializeField]
	private FighterCoordinator m_fighterCoordinator;

	[SerializeField]
	private MissionCoordinatorPrefabs m_missionCoordinatorPrefabs;

	[SerializeField]
	private DayNightCycle m_dayNightCycle;

	[SerializeField]
	private GameObject m_keyMissionPrefab;

	private BomberState m_bomber;

	private Dictionary<string, List<MissionPlaceableObject>> m_allSpawnedAreas = new Dictionary<string, List<MissionPlaceableObject>>();

	private Dictionary<string, MissionPlaceableObject> m_referencedMissionPlaceables = new Dictionary<string, MissionPlaceableObject>();

	private bool m_canToggleReturnJourney;

	private ObjectiveManager.Objective m_returnHomeObjective;

	private bool m_abortedMission;

	private bool m_isReturnJourney;

	private bool m_outwardJourneyLockedByTutorialOrEvents;

	public Action<string> OnTrigger;

	private List<string> m_triggersFired = new List<string>();

	private List<string> m_playerKeys = new List<string>();

	private List<MissionCoordinatorPrefabs> m_extraPrefabLists = new List<MissionCoordinatorPrefabs>();

	private bool m_extraDLCInitialised;

	public void OnPlayerTakeOff()
	{
		m_dayNightCycle.StartDayNight();
	}

	private void InitDLC()
	{
		if (m_extraDLCInitialised)
		{
			return;
		}
		foreach (string currentInstalledDLC in Singleton<DLCManager>.Instance.GetCurrentInstalledDLCs())
		{
			DLCExpansionCampaign dLCExpansionCampaign = Singleton<DLCManager>.Instance.LoadAssetFromBundle<DLCExpansionCampaign>(currentInstalledDLC, "CAMPAIGN_" + currentInstalledDLC);
			if (dLCExpansionCampaign != null && dLCExpansionCampaign.GetExtraPrefabs() != null)
			{
				m_extraPrefabLists.Add(dLCExpansionCampaign.GetExtraPrefabs());
			}
		}
		m_extraDLCInitialised = true;
	}

	private IEnumerator Start()
	{
		m_bomber = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState();
		m_playerKeys = Singleton<SaveDataContainer>.Instance.Get().GetKeysUnlocked(Singleton<GameFlow>.Instance.GetCampaign());
		if (Singleton<SystemDataContainer>.Instance.Get().GetMusic())
		{
			WingroveRoot.Instance.PostEvent("MUSIC_UNMUTE");
		}
		else
		{
			WingroveRoot.Instance.PostEvent("MUSIC_MUTE");
		}
		MissionCoordinator missionCoordinator = this;
		missionCoordinator.OnTrigger = (Action<string>)Delegate.Combine(missionCoordinator.OnTrigger, new Action<string>(SpawnMission));
		yield return new WaitForSeconds(1f);
		if (Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetCurrentlySelectedMissionDetails().m_showKeyMissionTitle)
		{
			StartCoroutine(WaitAndShowKeyMission());
		}
		else if (Singleton<GameFlow>.Instance.GetGameMode().GetUseUSNaming())
		{
			WingroveRoot.Instance.PostEvent("START_MISSION_CAMERA_USAAF");
		}
		else
		{
			WingroveRoot.Instance.PostEvent("START_MISSION_CAMERA");
		}
	}

	private IEnumerator WaitAndShowKeyMission()
	{
		yield return new WaitForSeconds(1.5f);
		GameObject go = UnityEngine.Object.Instantiate(m_keyMissionPrefab);
		go.GetComponent<KeyMissionDisplay>().SetUp(Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetCurrentlySelectedMissionDetails());
	}

	private GameObject GetPrefabForObjectType(string stringRef)
	{
		MissionCoordinatorPrefabs.PrefabConnection[] allObjectTypes = m_missionCoordinatorPrefabs.GetAllObjectTypes();
		foreach (MissionCoordinatorPrefabs.PrefabConnection prefabConnection in allObjectTypes)
		{
			if (prefabConnection.m_missionObjectString == stringRef)
			{
				return prefabConnection.m_prefab;
			}
		}
		foreach (MissionCoordinatorPrefabs extraPrefabList in m_extraPrefabLists)
		{
			MissionCoordinatorPrefabs.PrefabConnection[] allObjectTypes2 = extraPrefabList.GetAllObjectTypes();
			foreach (MissionCoordinatorPrefabs.PrefabConnection prefabConnection2 in allObjectTypes2)
			{
				if (prefabConnection2.m_missionObjectString == stringRef)
				{
					return prefabConnection2.m_prefab;
				}
			}
		}
		return null;
	}

	public FighterCoordinator GetFighterCoordinator()
	{
		return m_fighterCoordinator;
	}

	public void SpawnMissionStart()
	{
		InitDLC();
		SpawnMission(null);
		int numberOfTimesPlayed = Singleton<SaveDataContainer>.Instance.Get().GetNumberOfTimesPlayed(Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetCurrentlySelectedMissionDetails().m_missionReferenceName);
		SpawnMission("PLAYEDN2_" + numberOfTimesPlayed % 2);
		SpawnMission("PLAYEDN3_" + numberOfTimesPlayed % 3);
	}

	public void SetOutwardLocked(bool locked)
	{
		m_outwardJourneyLockedByTutorialOrEvents = locked;
	}

	private void SpawnMission(string fromTrigger)
	{
		if (string.IsNullOrEmpty(fromTrigger))
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["MissionName"] = Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetCurrentlySelectedMission().name;
			dictionary["NumMissionsPlayed"] = Singleton<SaveDataContainer>.Instance.Get().GetNumMissionsPlayed();
		}
		LevelDescription currentlySelectedMission = Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetCurrentlySelectedMission();
		List<LevelDescription.LevelItemSerialized> levelItems = currentlySelectedMission.m_levelItems;
		foreach (LevelDescription.LevelItemSerialized item in levelItems)
		{
			if (!(item.m_spawnTrigger == fromTrigger) && (!string.IsNullOrEmpty(item.m_spawnTrigger) || !string.IsNullOrEmpty(fromTrigger)))
			{
				continue;
			}
			bool flag = true;
			if (!string.IsNullOrEmpty(item.m_missionKeyRequired) && !m_playerKeys.Contains(item.m_missionKeyRequired))
			{
				flag = false;
			}
			if (!string.IsNullOrEmpty(item.m_missionKeySuppress) && m_playerKeys.Contains(item.m_missionKeySuppress))
			{
				flag = false;
			}
			if (!flag)
			{
				continue;
			}
			GameObject prefabForObjectType = GetPrefabForObjectType(item.m_type);
			if (prefabForObjectType != null)
			{
				Vector3d position = new Vector3d(item.m_position);
				GameObject gameObject = UnityEngine.Object.Instantiate(prefabForObjectType);
				gameObject.transform.parent = base.transform;
				gameObject.GetComponent<MissionPlaceableObject>().SetPosition(position, item.m_size, item.m_levelParameters, item.m_rotation);
				List<MissionPlaceableObject> value = null;
				string text = item.m_typeBase;
				if (string.IsNullOrEmpty(text))
				{
					text = item.m_type;
				}
				m_allSpawnedAreas.TryGetValue(text, out value);
				if (value == null)
				{
					value = new List<MissionPlaceableObject>();
					m_allSpawnedAreas[text] = value;
				}
				value.Add(gameObject.GetComponent<MissionPlaceableObject>());
				if (!string.IsNullOrEmpty(item.m_ref))
				{
					m_referencedMissionPlaceables[item.m_ref] = gameObject.GetComponent<MissionPlaceableObject>();
				}
			}
		}
	}

	public MissionPlaceableObject GetPlaceableByRef(string refString)
	{
		return m_referencedMissionPlaceables[refString];
	}

	public void FireTrigger(string trigger)
	{
		if (!string.IsNullOrEmpty(trigger) && !m_triggersFired.Contains(trigger))
		{
			m_triggersFired.Add(trigger);
			if (OnTrigger != null)
			{
				OnTrigger(trigger);
			}
		}
	}

	public List<MissionPlaceableObject> GetObjectsByType(string missionObjectType)
	{
		List<MissionPlaceableObject> value = null;
		m_allSpawnedAreas.TryGetValue(missionObjectType, out value);
		return value;
	}

	public MissionPlaceableObject GetObjectByType(string missionObjectType)
	{
		List<MissionPlaceableObject> value = null;
		m_allSpawnedAreas.TryGetValue(missionObjectType, out value);
		if (value != null && value.Count > 0)
		{
			return value[value.Count - 1];
		}
		return null;
	}

	public void RegisterExternallySpawnedMissionPlaceableObject(GameObject createdObject, string typeName)
	{
		List<MissionPlaceableObject> list = null;
		if (list == null)
		{
			list = new List<MissionPlaceableObject>();
			m_allSpawnedAreas[typeName] = list;
		}
		list.Add(createdObject.GetComponent<MissionPlaceableObject>());
	}

	private void Update()
	{
		m_isReturnJourney = m_abortedMission;
		m_canToggleReturnJourney = true;
		if (!Singleton<ObjectiveManager>.Instance.PrimaryNonReturnObjectivesRemain() && !m_outwardJourneyLockedByTutorialOrEvents)
		{
			m_canToggleReturnJourney = false;
			m_isReturnJourney = true;
		}
		if (m_isReturnJourney && !Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetMissionLog().IsMissionCompletedSimple() && Singleton<ObjectiveManager>.Instance.PrimaryNonReturnObjectivesAllComplete() && !Singleton<ObjectiveManager>.Instance.PrimaryNonReturnObjectivesRemain())
		{
			Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetMissionLog().SetMissionCompletedSimple();
		}
		if (m_isReturnJourney && m_returnHomeObjective == null)
		{
			m_returnHomeObjective = new ObjectiveManager.Objective();
			m_returnHomeObjective.m_countToComplete = 1;
			m_returnHomeObjective.m_objectiveTitle = "mission_objective_return_to_base";
			m_returnHomeObjective.m_objectiveType = ObjectiveManager.ObjectiveType.ReturnHome;
			m_returnHomeObjective = Singleton<ObjectiveManager>.Instance.RegisterObjective(m_returnHomeObjective);
		}
		else if (!m_isReturnJourney && m_returnHomeObjective != null)
		{
			Singleton<ObjectiveManager>.Instance.RemoveObjective(m_returnHomeObjective);
			m_returnHomeObjective = null;
		}
	}

	public void SetAbort(bool abort)
	{
		m_abortedMission = abort;
	}

	public bool CanToggleReturnJourney()
	{
		return m_canToggleReturnJourney;
	}

	public bool IsOutwardJourney()
	{
		return !m_isReturnJourney;
	}
}
