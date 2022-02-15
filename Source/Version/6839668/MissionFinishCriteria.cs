using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class MissionFinishCriteria : MonoBehaviour
{
	[SerializeField]
	private LayerMask m_detectEndPositionMask;

	private float m_missionEndTimer = 5f;

	private bool m_hasMissionEnded;

	private RaycastHit[] buffer = new RaycastHit[128];

	private bool MissionCriteriaEnded()
	{
		if (Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState().IsLanded())
		{
			bool isOnGround = false;
			bool isOnRunway = false;
			bool isInSafeTerritory = false;
			DoRaycast(Singleton<BomberSpawn>.Instance.GetBomberSystems().transform.position, out isOnGround, out isOnRunway, out isInSafeTerritory);
			if (isOnGround && isOnRunway)
			{
				Singleton<ObjectiveManager>.Instance.CompleteObjectivesOfType(ObjectiveManager.ObjectiveType.ReturnHome);
			}
			else
			{
				Singleton<ObjectiveManager>.Instance.FailObjectivesOfType(ObjectiveManager.ObjectiveType.ReturnHome);
			}
			Singleton<ObjectiveManager>.Instance.FailObjectivesOfType(ObjectiveManager.ObjectiveType.BombTarget);
			Singleton<ObjectiveManager>.Instance.FailObjectivesOfType(ObjectiveManager.ObjectiveType.NonBombTarget);
			return true;
		}
		if (Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState().GetPhysicsModel()
			.IsOnGround())
		{
			bool flag = true;
			int engineCount = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetEngineCount();
			for (int i = 0; i < engineCount; i++)
			{
				Engine engine = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetEngine(i);
				if (engine != null)
				{
					flag = false;
				}
			}
			if (flag)
			{
				return true;
			}
			if (Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBombLoad() != null && Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBombLoad().HasDropped())
			{
				return true;
			}
			bool flag2 = true;
			foreach (CrewSpawner.CrewmanAvatarPairing item in Singleton<CrewSpawner>.Instance.GetAllCrew())
			{
				if (!item.m_spawnedAvatar.IsBailedOut() || item.m_spawnedAvatar.GetHealthState().IsDead())
				{
					flag2 = false;
					break;
				}
			}
			if (flag2)
			{
				return true;
			}
		}
		return false;
	}

	private void Awake()
	{
	}

	private void ShowDebug()
	{
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("End mission"))
		{
			EndMission();
		}
		if (GUILayout.Button("End mission (destroy target)"))
		{
			MissionLog missionLog = Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetMissionLog();
			foreach (MissionLog.LogObjective logObjective in Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetMissionLog().GetLogObjectives())
			{
				logObjective.m_isComplete = true;
			}
			missionLog.SetMissionCompletedSimple();
			EndMission();
		}
		GUILayout.EndHorizontal();
	}

	private void OnDestroy()
	{
	}

	private void DoRaycast(Vector3 atPosition, out bool isOnGround, out bool isOnRunway, out bool isInSafeTerritory)
	{
		isOnGround = false;
		isOnRunway = false;
		isInSafeTerritory = false;
		int num = Physics.RaycastNonAlloc(atPosition + Vector3.up * 50f, Vector3.down, buffer, 3000f, m_detectEndPositionMask);
		for (int i = 0; i < num; i++)
		{
			if (buffer[i].collider != null)
			{
				if (buffer[i].collider.gameObject.GetComponent<MissionLandingArea>() != null)
				{
					isOnGround = true;
					isOnRunway = true;
				}
				if (buffer[i].collider.gameObject.GetComponent<SafeLandmass>() != null)
				{
					isInSafeTerritory = true;
					isOnGround = true;
				}
				if (buffer[i].point.y > 0f)
				{
					isOnGround = true;
				}
			}
		}
	}

	private void EndMission()
	{
		MissionLog missionLog = Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetMissionLog();
		Vector3 endPosition = (Vector3)Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBigTransform().position;
		bool isOnGround = false;
		bool isOnRunway = false;
		bool isInSafeTerritory = false;
		DoRaycast(Singleton<BomberSpawn>.Instance.GetBomberSystems().transform.position, out isOnGround, out isOnRunway, out isInSafeTerritory);
		if (isOnGround && isOnRunway)
		{
			missionLog.SetLandingState(MissionLog.LandingState.AtBase, endPosition);
		}
		else if (isOnGround && isInSafeTerritory)
		{
			missionLog.SetLandingState(MissionLog.LandingState.InEngland, endPosition);
		}
		else if (isOnGround)
		{
			missionLog.SetLandingState(MissionLog.LandingState.Abroad, endPosition);
		}
		else
		{
			missionLog.SetLandingState(MissionLog.LandingState.InSea, endPosition);
		}
		bool flag = false;
		if (Singleton<BomberSpawn>.Instance.GetBomberSystems().GetCriticalFlash().IsDestroyed())
		{
			flag = true;
			missionLog.SetBomberDestroyed();
		}
		if (isOnGround && isOnRunway && !flag)
		{
			Singleton<AchievementMonitor>.Instance.CheckAchievementsOnEvent(AchievementMonitor.CheckType.OnLandBomberAtBase);
			if (missionLog.IsComplete())
			{
				Singleton<AchievementMonitor>.Instance.CheckAchievementsOnEvent(AchievementMonitor.CheckType.OnLandBomberAtBaseComplete);
			}
		}
		int currentCrewCount = Singleton<CrewContainer>.Instance.GetCurrentCrewCount();
		int num = 0;
		for (int i = 0; i < currentCrewCount; i++)
		{
			Crewman crewman = Singleton<CrewContainer>.Instance.GetCrewman(i);
			CrewmanAvatar avatarFor = Singleton<ContextControl>.Instance.GetAvatarFor(crewman);
			if (crewman.IsDead())
			{
				num++;
			}
			if (avatarFor.IsBailedOut())
			{
				DoRaycast(avatarFor.transform.position, out isOnGround, out isOnRunway, out isInSafeTerritory);
				if (avatarFor.GetComponent<BigTransform>() != null)
				{
					missionLog.SetCrewmanPosition(crewman, (Vector3)avatarFor.GetComponent<BigTransform>().position);
				}
				else
				{
					missionLog.SetCrewmanPosition(crewman, -(Vector3)Singleton<BigTransformCoordinator>.Instance.GetCurrentOffset() + avatarFor.transform.position);
				}
				if (isOnGround && isOnRunway)
				{
					missionLog.SetCrewmanState(crewman, MissionLog.LandingState.AtBase);
				}
				else if (isOnGround && isInSafeTerritory)
				{
					missionLog.SetCrewmanState(crewman, MissionLog.LandingState.InEngland);
				}
				else if (isOnGround)
				{
					missionLog.SetCrewmanState(crewman, MissionLog.LandingState.Abroad);
				}
				else
				{
					missionLog.SetCrewmanState(crewman, MissionLog.LandingState.InSea);
				}
			}
		}
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["MissionName"] = Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetCurrentlySelectedMission().name;
		dictionary["LandingState"] = missionLog.GetLandingState();
		dictionary["CrewLost"] = num;
		dictionary["XPGained"] = missionLog.GetTotalXP();
		dictionary["TargetsHit"] = missionLog.GetNumTargetsDestroyed();
		Singleton<GameFlow>.Instance.ToDebrief();
	}

	public void Update()
	{
		if (MissionCriteriaEnded())
		{
			m_missionEndTimer -= Time.deltaTime;
			if (m_missionEndTimer < 0f && !m_hasMissionEnded)
			{
				m_hasMissionEnded = true;
				EndMission();
			}
		}
		else
		{
			m_missionEndTimer = 5f;
		}
	}
}
