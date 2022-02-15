using System;
using System.Collections;
using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class ExtraObjective : MonoBehaviour
{
	[SerializeField]
	private MissionPlaceableObject m_placeableObject;

	private string m_objectiveType;

	private int m_value;

	private ObjectiveManager.Objective m_thisObjective;

	private MissionLog.LogObjective m_missionObjective;

	private string m_failTrigger;

	private string m_failTrigger2;

	private string m_triggerOnComplete;

	private void Start()
	{
		m_objectiveType = m_placeableObject.GetParameter("objectiveType");
		m_value = 1;
		int.TryParse(m_placeableObject.GetParameter("value"), out m_value);
		string parameter = m_placeableObject.GetParameter("namedText");
		string parameter2 = m_placeableObject.GetParameter("namedTextDebrief");
		bool flag = m_placeableObject.GetParameter("fullObjective") == "true";
		string parameter3 = m_placeableObject.GetParameter("failObjective");
		if (!string.IsNullOrEmpty(parameter3))
		{
			m_failTrigger = parameter3;
			m_failTrigger2 = m_placeableObject.GetParameter("failObjective2");
			MissionCoordinator instance = Singleton<MissionCoordinator>.Instance;
			instance.OnTrigger = (Action<string>)Delegate.Combine(instance.OnTrigger, new Action<string>(OnTriggerReceive));
		}
		m_triggerOnComplete = m_placeableObject.GetParameter("triggerOnComplete");
		switch (m_objectiveType)
		{
		case "TakePhoto":
			m_thisObjective = new ObjectiveManager.Objective();
			m_thisObjective.m_primary = true;
			m_thisObjective.m_countToComplete = m_value;
			m_thisObjective.m_objectiveType = ObjectiveManager.ObjectiveType.Other;
			m_thisObjective.m_objectiveTitle = ((!string.IsNullOrEmpty(parameter)) ? parameter : "mission_objective_take_photo");
			Singleton<ObjectiveManager>.Instance.RegisterObjective(m_thisObjective);
			if (flag)
			{
				m_missionObjective = new MissionLog.LogObjective();
				Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetMissionLog().LogObjectiveNew(m_missionObjective);
			}
			StartCoroutine(WaitForTakePhoto());
			break;
		case "ClearArea":
		{
			m_thisObjective = new ObjectiveManager.Objective();
			m_thisObjective.m_primary = true;
			m_thisObjective.m_countToComplete = m_value;
			if (m_thisObjective.m_countToComplete == 0)
			{
				m_thisObjective.m_countToComplete = 1;
			}
			m_thisObjective.m_objectiveType = ObjectiveManager.ObjectiveType.Other;
			m_thisObjective.m_objectiveTitle = ((!string.IsNullOrEmpty(parameter)) ? parameter : "mission_objective_clear_area");
			Singleton<ObjectiveManager>.Instance.RegisterObjective(m_thisObjective);
			if (flag)
			{
				m_missionObjective = new MissionLog.LogObjective();
				Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetMissionLog().LogObjectiveNew(m_missionObjective);
			}
			bool lightClear = m_placeableObject.GetParameter("lightClearObjective") == "true";
			StartCoroutine(WaitForClearArea(m_placeableObject.GetParameter("valueString"), lightClear));
			break;
		}
		case "V1sDontHit":
			m_thisObjective = new ObjectiveManager.Objective();
			m_thisObjective.m_primary = true;
			m_thisObjective.m_countIsFailure = true;
			m_thisObjective.m_countToComplete = 1;
			m_thisObjective.m_countToFail = m_value;
			m_thisObjective.m_objectiveType = ObjectiveManager.ObjectiveType.Other;
			m_thisObjective.m_objectiveTitle = ((!string.IsNullOrEmpty(parameter)) ? parameter : "mission_objective_stop_rockets");
			Singleton<ObjectiveManager>.Instance.RegisterObjective(m_thisObjective);
			if (flag)
			{
				m_missionObjective = new MissionLog.LogObjective();
				Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetMissionLog().LogObjectiveNew(m_missionObjective);
			}
			StartCoroutine(WaitForV1sHit());
			break;
		case "WaitForTrigger":
			m_thisObjective = new ObjectiveManager.Objective();
			m_thisObjective.m_primary = true;
			m_thisObjective.m_countToComplete = 1;
			m_thisObjective.m_objectiveType = ObjectiveManager.ObjectiveType.Other;
			m_thisObjective.m_objectiveTitle = ((!string.IsNullOrEmpty(parameter)) ? parameter : "mission_objective_misc");
			Singleton<ObjectiveManager>.Instance.RegisterObjective(m_thisObjective);
			if (flag)
			{
				m_missionObjective = new MissionLog.LogObjective();
				Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetMissionLog().LogObjectiveNew(m_missionObjective);
			}
			StartCoroutine(WaitForTrigger(m_placeableObject.GetParameter("valueString")));
			break;
		}
	}

	private void OnTriggerReceive(string trigger)
	{
		if ((trigger == m_failTrigger || trigger == m_failTrigger2) && m_thisObjective != null)
		{
			Singleton<ObjectiveManager>.Instance.FailObjective(m_thisObjective);
			StopAllCoroutines();
		}
	}

	private bool AreOtherObjectivesComplete()
	{
		bool result = true;
		foreach (ObjectiveManager.Objective currentObjective in Singleton<ObjectiveManager>.Instance.GetCurrentObjectives())
		{
			if (!currentObjective.m_complete && currentObjective != m_thisObjective && currentObjective.m_objectiveType != ObjectiveManager.ObjectiveType.Ace)
			{
				result = false;
			}
		}
		return result;
	}

	private IEnumerator WaitForClearArea(string waitForTrigger, bool lightClear)
	{
		bool triggerFired = false;
		if (string.IsNullOrEmpty(waitForTrigger))
		{
			triggerFired = true;
		}
		else
		{
			MissionCoordinator instance = Singleton<MissionCoordinator>.Instance;
			instance.OnTrigger = (Action<string>)Delegate.Combine(instance.OnTrigger, (Action<string>)delegate(string st)
			{
				if (st == waitForTrigger)
				{
					triggerFired = true;
				}
			});
		}
		bool isCompleted = false;
		while (!isCompleted)
		{
			MissionLog ml = Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetMissionLog();
			List<MissionLog.LogObjective> lo = ml.GetLogObjectives();
			if (!lightClear)
			{
				while (!AreOtherObjectivesComplete())
				{
					yield return null;
				}
			}
			float timeWithNoFighters = 0f;
			do
			{
				timeWithNoFighters = ((Singleton<FighterCoordinator>.Instance.AreAnyFightersEngaged() || !triggerFired) ? 0f : (timeWithNoFighters + Time.deltaTime));
				yield return null;
			}
			while (!(timeWithNoFighters > 6f));
			Singleton<ObjectiveManager>.Instance.CompleteObjective(m_thisObjective);
			if (m_thisObjective.m_complete && !m_thisObjective.m_failed)
			{
				if (!string.IsNullOrEmpty(m_triggerOnComplete))
				{
					Singleton<MissionCoordinator>.Instance.FireTrigger(m_triggerOnComplete);
				}
				isCompleted = true;
				if (m_missionObjective != null)
				{
					m_missionObjective.m_isComplete = true;
				}
			}
			yield return null;
		}
	}

	private IEnumerator WaitForTakePhoto()
	{
		bool isCompleted = false;
		while (!isCompleted)
		{
			while (Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetMissionLog().GetPhotosTaken() > m_thisObjective.m_countCompleted && m_thisObjective.m_countCompleted < m_thisObjective.m_countToComplete && !m_thisObjective.m_complete && !m_thisObjective.m_failed)
			{
				Singleton<ObjectiveManager>.Instance.CompleteObjective(m_thisObjective);
				if (m_thisObjective.m_complete && !m_thisObjective.m_failed)
				{
					if (!string.IsNullOrEmpty(m_triggerOnComplete))
					{
						Singleton<MissionCoordinator>.Instance.FireTrigger(m_triggerOnComplete);
					}
					isCompleted = true;
					if (m_missionObjective != null)
					{
						m_missionObjective.m_isComplete = true;
					}
					string parameter = m_placeableObject.GetParameter("valueString");
					if (!string.IsNullOrEmpty(parameter))
					{
						Singleton<MissionCoordinator>.Instance.FireTrigger(parameter);
					}
				}
				yield return null;
			}
			yield return null;
		}
	}

	private IEnumerator WaitForTrigger(string trigger)
	{
		bool isCompleted = false;
		MissionCoordinator instance = Singleton<MissionCoordinator>.Instance;
		instance.OnTrigger = (Action<string>)Delegate.Combine(instance.OnTrigger, (Action<string>)delegate(string tr)
		{
			if (tr == trigger)
			{
				isCompleted = true;
			}
		});
		while (!isCompleted)
		{
			yield return null;
		}
		Singleton<ObjectiveManager>.Instance.CompleteObjective(m_thisObjective);
		if (m_thisObjective.m_complete && !m_thisObjective.m_failed)
		{
			if (!string.IsNullOrEmpty(m_triggerOnComplete))
			{
				Singleton<MissionCoordinator>.Instance.FireTrigger(m_triggerOnComplete);
			}
			isCompleted = true;
			if (m_missionObjective != null)
			{
				m_missionObjective.m_isComplete = true;
			}
		}
	}

	private IEnumerator WaitForV1sHit()
	{
		bool mainObjectiveFailed = false;
		MissionCoordinator instance = Singleton<MissionCoordinator>.Instance;
		instance.OnTrigger = (Action<string>)Delegate.Combine(instance.OnTrigger, (Action<string>)delegate(string st)
		{
			if (st == "BOMBTARGETS_FAIL")
			{
				mainObjectiveFailed = true;
			}
		});
		int numFailed = 0;
		while (!AreOtherObjectivesComplete())
		{
			for (; Singleton<FighterCoordinator>.Instance.GetV1sTotal() > numFailed; numFailed++)
			{
				Singleton<ObjectiveManager>.Instance.FailObjectiveCount(m_thisObjective);
			}
			yield return null;
		}
		yield return null;
		if (m_thisObjective.m_complete)
		{
			yield break;
		}
		if (mainObjectiveFailed)
		{
			Singleton<ObjectiveManager>.Instance.FailObjective(m_thisObjective);
			yield break;
		}
		Singleton<ObjectiveManager>.Instance.CompleteObjective(m_thisObjective);
		if (m_thisObjective.m_complete && !m_thisObjective.m_failed)
		{
			if (!string.IsNullOrEmpty(m_triggerOnComplete))
			{
				Singleton<MissionCoordinator>.Instance.FireTrigger(m_triggerOnComplete);
			}
			if (m_missionObjective != null)
			{
				m_missionObjective.m_isComplete = true;
			}
		}
	}
}
