using System.Collections.Generic;
using UnityEngine;

public class MissionLog
{
	public enum LogObjectiveType
	{
		DestroyTarget
	}

	public class LogObjective
	{
		public bool m_isComplete;
	}

	public enum XpRewardType
	{
		BombTarget,
		Fighter,
		FighterAce,
		PhotoTarget
	}

	public class XpEvent
	{
		public int m_xpAMount;

		public XpRewardType m_rewardType;
	}

	public enum LandingState
	{
		AtBase,
		InEngland,
		InSea,
		Abroad
	}

	private int m_numFightersDestroyed;

	private int m_validPhotosTaken;

	private int m_intelRewardsTotal;

	private int m_moneyRewardsTotal;

	private int m_numTargetsDestroyed;

	private int m_numTargetsTotal;

	private List<XpEvent> m_xpRewards = new List<XpEvent>();

	private Dictionary<Crewman, Vector3> m_crewmenPositions = new Dictionary<Crewman, Vector3>();

	private Dictionary<Crewman, LandingState> m_crewmanStates = new Dictionary<Crewman, LandingState>();

	public List<LogObjective> m_logObjectives = new List<LogObjective>();

	private int m_xpTotal;

	private LandingState m_landingState;

	private Vector3 m_bomberEndPosition;

	private EnemyFighterAce m_aceEncountered;

	private bool m_aceDefeated;

	private bool m_aceDefeatedFirstTime;

	private bool m_usedSlowTime;

	private bool m_missionCompletedSimple;

	public static bool s_missionCompletedSimple;

	private float m_distanceFlown;

	private float m_damageTaken;

	private int m_fightersDestroyed;

	private bool m_bomberDestroyed;

	private bool m_isWinterEnvironment;

	public void SetMissionCompletedSimple()
	{
		m_missionCompletedSimple = true;
		s_missionCompletedSimple = true;
	}

	public void AddDistanceFlown(float dist)
	{
		m_distanceFlown += dist;
	}

	public float GetMilesFlown()
	{
		return m_distanceFlown * 0.014559811f;
	}

	public bool IsMissionCompletedSimple()
	{
		return s_missionCompletedSimple || m_missionCompletedSimple;
	}

	public void SetWinterEnvironment()
	{
		m_isWinterEnvironment = true;
	}

	public bool GetWinterEnvironment()
	{
		return m_isWinterEnvironment;
	}

	public void AddDamageTaken(float amt)
	{
		m_damageTaken += amt;
	}

	public void AddFighterDestroyed()
	{
		m_fightersDestroyed++;
	}

	public void SetUsedSlowTime()
	{
		m_usedSlowTime = true;
	}

	public bool DidUseSlowTime()
	{
		return m_usedSlowTime;
	}

	public void AddValidPhotoTaken(int xpAmount, int intelAmount, int moneyAmount)
	{
		m_validPhotosTaken++;
		AddXP(xpAmount, XpRewardType.PhotoTarget);
		m_intelRewardsTotal += intelAmount;
		m_moneyRewardsTotal += moneyAmount;
	}

	public float GetDamageTaken()
	{
		return m_damageTaken;
	}

	public int GetFightersDestroyed()
	{
		return m_fightersDestroyed;
	}

	public void SetAceEncountered(EnemyFighterAce ace)
	{
		m_aceEncountered = ace;
	}

	public void SetAceDefeated(bool isFirstDefeat)
	{
		m_aceDefeated = true;
		m_aceDefeatedFirstTime = isFirstDefeat;
	}

	public EnemyFighterAce GetAceEncountered()
	{
		return m_aceEncountered;
	}

	public bool GetAceDefeated()
	{
		return m_aceDefeated;
	}

	public bool GetAceDefeatedForFirstTime()
	{
		return m_aceDefeatedFirstTime;
	}

	public bool WasBomberDestroyed()
	{
		return m_bomberDestroyed;
	}

	public void SetBomberDestroyed()
	{
		m_bomberDestroyed = true;
	}

	public void LogObjectiveNew(LogObjective lo)
	{
		m_logObjectives.Add(lo);
	}

	public List<LogObjective> GetLogObjectives()
	{
		return m_logObjectives;
	}

	private void CheckConflict()
	{
		if (s_missionCompletedSimple != m_missionCompletedSimple)
		{
			DebugLogWrapper.LogError("[MISSIONLOGCONFLICT] static var isn't equal to local var... " + s_missionCompletedSimple + " " + m_missionCompletedSimple);
		}
		if ((s_missionCompletedSimple || m_missionCompletedSimple) && m_logObjectives.Count == 0)
		{
			int num = Object.FindObjectsOfType<GameFlow>().Length;
			DebugLogWrapper.LogError("[MISSIONLOGCONFLICT] no log objectives, but bool is complete... " + s_missionCompletedSimple + " " + m_missionCompletedSimple + " " + num);
		}
		if (!s_missionCompletedSimple && !m_missionCompletedSimple)
		{
			return;
		}
		bool flag = true;
		foreach (LogObjective logObjective in m_logObjectives)
		{
			if (!logObjective.m_isComplete)
			{
				flag = false;
			}
		}
		if (!flag)
		{
			DebugLogWrapper.LogError("[MISSIONLOGCONFLICT] objectives say false, but bool is complete... " + s_missionCompletedSimple + " " + m_missionCompletedSimple);
		}
	}

	public bool IsComplete()
	{
		try
		{
			CheckConflict();
		}
		catch
		{
			DebugLogWrapper.LogError("[MISSIONLOG] exception while checking for conflicts...");
		}
		if (s_missionCompletedSimple)
		{
			return true;
		}
		if (m_missionCompletedSimple)
		{
			return true;
		}
		if (m_logObjectives.Count == 0)
		{
			return false;
		}
		bool result = true;
		foreach (LogObjective logObjective in m_logObjectives)
		{
			if (!logObjective.m_isComplete)
			{
				result = false;
			}
		}
		return result;
	}

	public void SetCrewmanPosition(Crewman crewman, Vector3 pos)
	{
		m_crewmenPositions[crewman] = pos;
	}

	public bool IsBailedOut(Crewman cm)
	{
		return m_crewmenPositions.ContainsKey(cm);
	}

	public void SetCrewmanState(Crewman crewman, LandingState ls)
	{
		m_crewmanStates[crewman] = ls;
	}

	public LandingState GetCrewmanLandingState(Crewman cm)
	{
		if (m_crewmanStates.ContainsKey(cm))
		{
			return m_crewmanStates[cm];
		}
		return m_landingState;
	}

	public Vector3 GetCrewmanPosition(Crewman crewman)
	{
		return m_crewmenPositions[crewman];
	}

	private void AddXP(int xpReward, XpRewardType type)
	{
		XpEvent xpEvent = new XpEvent();
		xpEvent.m_xpAMount = xpReward;
		xpEvent.m_rewardType = type;
		m_xpRewards.Add(xpEvent);
		m_xpTotal += xpReward;
	}

	public void SetTargetDestroyed(int xpReward)
	{
		m_numTargetsDestroyed++;
		AddXP(xpReward, XpRewardType.BombTarget);
	}

	public void SetFighterDestroyed(int xpReward)
	{
		m_numFightersDestroyed++;
		AddXP(xpReward, XpRewardType.Fighter);
	}

	public void SetLandingState(LandingState ls, Vector3 endPosition)
	{
		m_landingState = ls;
		m_bomberEndPosition = endPosition;
	}

	public LandingState GetLandingState()
	{
		return m_landingState;
	}

	public Vector3 GetLandingPosition()
	{
		return m_bomberEndPosition;
	}

	public int GetNumTargetsDestroyed()
	{
		return m_numTargetsDestroyed;
	}

	public List<XpEvent> GetAllXPRewards()
	{
		return m_xpRewards;
	}

	public int GetTotalXP()
	{
		return m_xpTotal;
	}

	public int GetPhotosTaken()
	{
		return m_validPhotosTaken;
	}

	public int GetIntelFromPhotos()
	{
		return m_intelRewardsTotal;
	}

	public int GetMoneyFromPhotos()
	{
		return m_moneyRewardsTotal;
	}
}
