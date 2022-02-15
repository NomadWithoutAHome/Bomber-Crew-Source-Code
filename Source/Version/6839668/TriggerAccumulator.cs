using System;
using System.Collections.Generic;
using System.Globalization;
using BomberCrewCommon;
using UnityEngine;

public class TriggerAccumulator : MonoBehaviour
{
	[SerializeField]
	private MissionPlaceableObject m_placeable;

	private int m_currentCount;

	private string m_triggerToFire;

	private int m_maxRequired;

	private List<string> m_triggers = new List<string>();

	private string m_failOnTrigger;

	private bool m_hasFired;

	private ObjectiveManager.Objective m_thisObjective;

	private MissionLog.LogObjective m_thisLogObjective;

	private void Start()
	{
		int num = Convert.ToInt32(m_placeable.GetParameter("numTriggersTotal"), CultureInfo.InvariantCulture);
		for (int i = 0; i < num; i++)
		{
			m_triggers.Add(m_placeable.GetParameter("trigger" + i));
		}
		m_triggerToFire = m_placeable.GetParameter("triggerToFire");
		m_maxRequired = Convert.ToInt32(m_placeable.GetParameter("maxRequired"), CultureInfo.InvariantCulture);
		m_failOnTrigger = m_placeable.GetParameter("failOnTrigger");
		bool flag = m_placeable.GetParameter("hasVisibleObjective") == "true";
		bool flag2 = m_placeable.GetParameter("hasRequiredObjective") == "true";
		if (flag)
		{
			ObjectiveManager.Objective objective = new ObjectiveManager.Objective();
			objective.m_countToComplete = m_maxRequired;
			objective.m_objectiveTitle = m_placeable.GetParameter("objectiveName");
			objective.m_primary = flag2;
			objective.m_objectiveGroup = "ACCUMULATOR_" + base.name + m_placeable.GetInstanceID();
			objective.m_objectiveType = ObjectiveManager.ObjectiveType.BombTarget;
			m_thisObjective = Singleton<ObjectiveManager>.Instance.RegisterObjective(objective);
			if (m_placeable.GetParameter("failIfNoBombs") == "true" && Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBombBay().GetBombLoad() != null && Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBombBay().GetBombLoad()
				.HasDropped())
			{
				Singleton<ObjectiveManager>.Instance.FailObjective(m_thisObjective);
			}
		}
		if (flag2)
		{
			m_thisLogObjective = new MissionLog.LogObjective();
			Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetMissionLog().LogObjectiveNew(m_thisLogObjective);
		}
		MissionCoordinator instance = Singleton<MissionCoordinator>.Instance;
		instance.OnTrigger = (Action<string>)Delegate.Combine(instance.OnTrigger, new Action<string>(OnTriggerReceive));
	}

	private void OnTriggerReceive(string trigger)
	{
		if (m_hasFired)
		{
			return;
		}
		if (m_triggers.Contains(trigger))
		{
			if (m_thisObjective != null)
			{
				Singleton<ObjectiveManager>.Instance.CompleteObjective(m_thisObjective);
			}
			m_currentCount++;
			if (m_currentCount == m_maxRequired)
			{
				if (m_thisLogObjective != null)
				{
					m_thisLogObjective.m_isComplete = true;
				}
				Singleton<MissionCoordinator>.Instance.FireTrigger(m_triggerToFire);
				m_hasFired = true;
			}
		}
		if (!string.IsNullOrEmpty(m_failOnTrigger) && m_failOnTrigger == trigger)
		{
			if (m_thisObjective != null)
			{
				Singleton<ObjectiveManager>.Instance.FailObjective(m_thisObjective);
			}
			m_hasFired = true;
		}
	}
}
