using System.Collections;
using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;
using WingroveAudio;

public class ObjectiveManager : Singleton<ObjectiveManager>
{
	public enum ObjectiveType
	{
		None,
		BombTarget,
		Tutorial,
		ReturnHome,
		Other,
		Ace,
		NonBombTarget,
		TagOnly
	}

	public class Objective
	{
		public string m_objectiveTitle;

		public string m_objectiveGroup;

		public int m_countToComplete;

		public int m_countToFail;

		public bool m_countIsFailure;

		public int m_countCompleted;

		public int m_countFailed;

		public bool m_primary;

		public string m_substitutionString;

		public bool m_hidden;

		public bool m_complete;

		public bool m_failed;

		public AceFighterInMission m_aceFighter;

		public ObjectiveType m_objectiveType;

		public GameObject m_prefabOverride;
	}

	private List<Objective> m_objectives = new List<Objective>();

	private bool m_returnJourneyDisabled;

	public void SetReturnJourneyDisabled()
	{
		m_returnJourneyDisabled = true;
	}

	public Objective RegisterObjective(Objective o)
	{
		bool flag = false;
		Objective result = o;
		if (!string.IsNullOrEmpty(o.m_objectiveGroup))
		{
			foreach (Objective objective in m_objectives)
			{
				if (objective.m_objectiveGroup == o.m_objectiveGroup)
				{
					objective.m_countToComplete++;
					result = objective;
					flag = true;
					break;
				}
			}
		}
		if (!flag)
		{
			m_objectives.Add(o);
		}
		return result;
	}

	public void RemoveObjective(Objective o)
	{
		m_objectives.Remove(o);
	}

	public void RemoveAllObjectives()
	{
		m_objectives.Clear();
	}

	public void CompleteObjective(Objective o)
	{
		if (o.m_failed || o.m_complete)
		{
			return;
		}
		o.m_countCompleted++;
		if (o.m_countToComplete == o.m_countCompleted)
		{
			WingroveRoot.Instance.PostEvent("OBJECTIVE_COMPLETE");
			o.m_complete = true;
			if (o.m_objectiveType == ObjectiveType.BombTarget)
			{
				Singleton<MissionCoordinator>.Instance.FireTrigger("BOMBTARGETS_FINISHED");
				Singleton<MissionCoordinator>.Instance.FireTrigger("BOMBTARGETS_COMPLETE");
			}
			if (o.m_objectiveType == ObjectiveType.Other || o.m_objectiveType == ObjectiveType.NonBombTarget)
			{
				Singleton<MissionCoordinator>.Instance.FireTrigger("OTHEROBJECTIVE_FINISHED");
				Singleton<MissionCoordinator>.Instance.FireTrigger("OTHEROBJECTIVE_COMPLETE");
			}
		}
		CheckAllComplete();
	}

	private void CheckAllComplete()
	{
		if (this != null)
		{
			StartCoroutine(WaitAndComplete());
		}
	}

	public IEnumerator WaitAndComplete()
	{
		yield return new WaitForSeconds(0.5f);
		bool allComplete = true;
		bool anyFailed = false;
		foreach (Objective objective in m_objectives)
		{
			if (!objective.m_complete && objective.m_primary)
			{
				allComplete = false;
			}
			if (objective.m_failed)
			{
				anyFailed = true;
			}
		}
		if (allComplete && !m_returnJourneyDisabled)
		{
			if (anyFailed)
			{
				Singleton<MusicSelectionRules>.Instance.TriggerTimed(MusicSelectionRules.MusicTriggerEvents.ObjectiveFailed, 10f);
				Singleton<MusicSelectionRules>.Instance.Trigger(MusicSelectionRules.MusicTriggerEvents.ObjectiveFailedEver);
			}
			else
			{
				Singleton<MusicSelectionRules>.Instance.TriggerTimed(MusicSelectionRules.MusicTriggerEvents.ObjectiveComplete, 10f);
				Singleton<MusicSelectionRules>.Instance.Trigger(MusicSelectionRules.MusicTriggerEvents.ObjectiveCompleteEver);
			}
		}
	}

	public void FailObjectiveCount(Objective o)
	{
		if (o.m_complete)
		{
			return;
		}
		o.m_countFailed++;
		if (o.m_countFailed == o.m_countToFail)
		{
			o.m_failed = true;
			o.m_complete = true;
			if (o.m_objectiveType == ObjectiveType.BombTarget || o.m_objectiveType == ObjectiveType.NonBombTarget)
			{
				Singleton<MissionCoordinator>.Instance.FireTrigger("BOMBTARGETS_FINISHED");
				Singleton<MissionCoordinator>.Instance.FireTrigger("BOMBTARGETS_FAIL");
			}
			if (o.m_objectiveType == ObjectiveType.Other || o.m_objectiveType == ObjectiveType.NonBombTarget)
			{
				Singleton<MissionCoordinator>.Instance.FireTrigger("OTHEROBJECTIVE_FINISHED");
				Singleton<MissionCoordinator>.Instance.FireTrigger("OTHEROBJECTIVE_FAIL");
			}
			CheckAllComplete();
		}
	}

	public void FailObjective(Objective o)
	{
		if (!o.m_complete)
		{
			o.m_failed = true;
			o.m_complete = true;
			if (o.m_objectiveType == ObjectiveType.BombTarget || o.m_objectiveType == ObjectiveType.NonBombTarget)
			{
				Singleton<MissionCoordinator>.Instance.FireTrigger("BOMBTARGETS_FINISHED");
				Singleton<MissionCoordinator>.Instance.FireTrigger("BOMBTARGETS_FAIL");
			}
			if (o.m_objectiveType == ObjectiveType.Other || o.m_objectiveType == ObjectiveType.NonBombTarget)
			{
				Singleton<MissionCoordinator>.Instance.FireTrigger("OTHEROBJECTIVE_FINISHED");
				Singleton<MissionCoordinator>.Instance.FireTrigger("OTHEROBJECTIVE_FAIL");
			}
			CheckAllComplete();
		}
	}

	public void FailObjectivesOfType(ObjectiveType ot)
	{
		foreach (Objective objective in m_objectives)
		{
			if (!objective.m_complete && objective.m_objectiveType == ot)
			{
				FailObjective(objective);
			}
		}
	}

	public void CompleteObjectivesOfType(ObjectiveType ot)
	{
		foreach (Objective objective in m_objectives)
		{
			if (!objective.m_complete && objective.m_objectiveType == ot)
			{
				objective.m_complete = true;
			}
		}
	}

	public bool ObjectivesOfTypeRemain(ObjectiveType ot)
	{
		foreach (Objective objective in m_objectives)
		{
			if (!objective.m_complete && objective.m_objectiveType == ot)
			{
				return true;
			}
		}
		return false;
	}

	public bool PrimaryNonReturnObjectivesRemain()
	{
		foreach (Objective objective in m_objectives)
		{
			if (!objective.m_complete && objective.m_primary && objective.m_objectiveType != ObjectiveType.ReturnHome)
			{
				return true;
			}
		}
		return false;
	}

	public bool PrimaryNonReturnObjectivesAllComplete()
	{
		foreach (Objective objective in m_objectives)
		{
			if (objective.m_primary && objective.m_objectiveType != ObjectiveType.ReturnHome)
			{
				if (!objective.m_complete)
				{
					return false;
				}
				if (objective.m_failed)
				{
					return false;
				}
			}
		}
		return true;
	}

	public List<Objective> GetCurrentObjectives()
	{
		return m_objectives;
	}
}
