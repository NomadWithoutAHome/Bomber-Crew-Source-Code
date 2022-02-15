using System;
using System.Globalization;
using BomberCrewCommon;
using UnityEngine;

public class MissionObjectiveTimedTrigger : MonoBehaviour
{
	[SerializeField]
	private MissionPlaceableObject m_placeable;

	private float m_countdown;

	private bool m_enabled;

	private string m_completeObjectiveTrigger;

	private string m_completeForwardTrigger;

	private ObjectiveManager.Objective m_thisObjective;

	private MissionLog.LogObjective m_fullObjective;

	private void Start()
	{
		m_countdown = float.Parse(m_placeable.GetParameter("countdown"), CultureInfo.InvariantCulture);
		m_enabled = true;
		m_completeObjectiveTrigger = m_placeable.GetParameter("completeTrigger");
		m_completeForwardTrigger = m_placeable.GetParameter("completeForward");
		if (!string.IsNullOrEmpty(m_completeObjectiveTrigger))
		{
			MissionCoordinator instance = Singleton<MissionCoordinator>.Instance;
			instance.OnTrigger = (Action<string>)Delegate.Combine(instance.OnTrigger, new Action<string>(WaitForTrigger));
		}
		if (m_placeable.GetParameter("hasObjective") == "true")
		{
			ObjectiveManager.Objective objective = new ObjectiveManager.Objective();
			objective.m_countToComplete = 1;
			objective.m_objectiveTitle = m_placeable.GetParameter("objectiveText");
			objective.m_primary = true;
			objective.m_objectiveType = ObjectiveManager.ObjectiveType.BombTarget;
			m_thisObjective = Singleton<ObjectiveManager>.Instance.RegisterObjective(objective);
			if (m_placeable.GetParameter("fullObjective") == "true")
			{
				m_fullObjective = new MissionLog.LogObjective();
				Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetMissionLog().LogObjectiveNew(m_fullObjective);
			}
			if (m_placeable.GetParameter("failIfNoBombs") == "true" && Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBombBay().GetBombLoad() != null && Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBombBay().GetBombLoad()
				.HasDropped())
			{
				Singleton<ObjectiveManager>.Instance.FailObjective(m_thisObjective);
			}
		}
	}

	private void WaitForTrigger(string t)
	{
		if (t == m_completeObjectiveTrigger && m_enabled)
		{
			if (!string.IsNullOrEmpty(m_completeForwardTrigger))
			{
				Singleton<MissionCoordinator>.Instance.FireTrigger(m_completeForwardTrigger);
			}
			if (m_fullObjective != null)
			{
				m_fullObjective.m_isComplete = true;
			}
			m_enabled = false;
			if (m_thisObjective != null)
			{
				Singleton<ObjectiveManager>.Instance.CompleteObjective(m_thisObjective);
			}
		}
	}

	private void Update()
	{
		if (m_thisObjective != null)
		{
			int num = (int)Mathf.Max(m_countdown, 0f);
			int num2 = Mathf.FloorToInt(m_countdown / 60f);
			int num3 = num - num2 * 60;
			m_thisObjective.m_substitutionString = $"{num2}:{num3:00}";
		}
		if (!m_enabled)
		{
			return;
		}
		m_countdown -= Time.deltaTime;
		if (m_countdown < 0f)
		{
			m_countdown = 0f;
			m_enabled = false;
			if (m_thisObjective != null)
			{
				Singleton<ObjectiveManager>.Instance.FailObjective(m_thisObjective);
			}
			Singleton<MissionCoordinator>.Instance.FireTrigger(m_placeable.GetParameter("triggerToFire"));
		}
	}
}
