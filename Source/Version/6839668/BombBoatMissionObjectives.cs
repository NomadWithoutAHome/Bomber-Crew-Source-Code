using System;
using System.Collections;
using BomberCrewCommon;
using UnityEngine;

public class BombBoatMissionObjectives : MonoBehaviour
{
	[SerializeField]
	private MissionPlaceableObject m_placeable;

	private ObjectiveManager.Objective m_objectiveRendezvous;

	private ObjectiveManager.Objective m_objectiveDestroy;

	private MissionLog.LogObjective m_mainObjectiveLog;

	private IEnumerator Start()
	{
		m_objectiveRendezvous = new ObjectiveManager.Objective();
		m_objectiveRendezvous.m_objectiveTitle = "mission_objective_meet_bombboat";
		m_objectiveRendezvous.m_primary = true;
		m_objectiveRendezvous.m_countToComplete = 1;
		m_objectiveRendezvous.m_objectiveType = ObjectiveManager.ObjectiveType.Other;
		Singleton<ObjectiveManager>.Instance.RegisterObjective(m_objectiveRendezvous);
		m_objectiveDestroy = new ObjectiveManager.Objective();
		m_objectiveDestroy.m_objectiveTitle = "mission_objective_escort_bombboat";
		m_objectiveDestroy.m_primary = true;
		m_objectiveDestroy.m_countToComplete = 1;
		m_objectiveDestroy.m_objectiveType = ObjectiveManager.ObjectiveType.Other;
		Singleton<ObjectiveManager>.Instance.RegisterObjective(m_objectiveDestroy);
		MissionCoordinator instance = Singleton<MissionCoordinator>.Instance;
		instance.OnTrigger = (Action<string>)Delegate.Combine(instance.OnTrigger, new Action<string>(OnTrigger));
		StartCoroutine(WaitForMainTargetDestroyed());
		m_mainObjectiveLog = new MissionLog.LogObjective();
		Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetMissionLog().LogObjectiveNew(m_mainObjectiveLog);
		yield return new WaitForSeconds(5f);
		TopBarInfoQueue.TopBarRequest intro1 = TopBarInfoQueue.TopBarRequest.Speech("mission_speech_bomb_boat_initial", Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(Singleton<TopBarInfoQueue>.Instance.GetCommandNamedText()), Singleton<TopBarInfoQueue>.Instance.GetCommandPortrait(), Singleton<TopBarInfoQueue>.Instance.GetCommandJabberText(), isGoodGuy: true);
		Singleton<TopBarInfoQueue>.Instance.RegisterRequest(intro1);
	}

	private void OnTrigger(string trigger)
	{
		switch (trigger)
		{
		case "BOMB_BOAT_DESTROY":
			StartCoroutine(WaitForConfirmAndFail());
			break;
		case "BOMB_BOAT":
		{
			Singleton<ObjectiveManager>.Instance.CompleteObjective(m_objectiveRendezvous);
			TopBarInfoQueue.TopBarRequest tbr2 = TopBarInfoQueue.TopBarRequest.Speech("mission_speech_bomb_boat_escort", Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(Singleton<TopBarInfoQueue>.Instance.GetCommandNamedText()), Singleton<TopBarInfoQueue>.Instance.GetCommandPortrait(), Singleton<TopBarInfoQueue>.Instance.GetCommandJabberText(), isGoodGuy: true);
			Singleton<TopBarInfoQueue>.Instance.RegisterRequest(tbr2);
			break;
		}
		case "BOMBBOAT_2":
		{
			TopBarInfoQueue.TopBarRequest tbr = TopBarInfoQueue.TopBarRequest.Speech("mission_speech_bomb_boat_turrets", Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(Singleton<TopBarInfoQueue>.Instance.GetCommandNamedText()), Singleton<TopBarInfoQueue>.Instance.GetCommandPortrait(), Singleton<TopBarInfoQueue>.Instance.GetCommandJabberText(), isGoodGuy: true);
			Singleton<TopBarInfoQueue>.Instance.RegisterRequest(tbr);
			break;
		}
		}
	}

	private IEnumerator WaitForMainTargetDestroyed()
	{
		yield return null;
		string mainTargetName = m_placeable.GetParameter("mainTarget");
		MissionPlaceableObject mpo = Singleton<MissionCoordinator>.Instance.GetPlaceableByRef(mainTargetName);
		SmoothDamageable d = mpo.GetComponent<SmoothDamageable>();
		while (d.GetHealthNormalised() > 0f)
		{
			yield return null;
		}
		m_mainObjectiveLog.m_isComplete = true;
		Singleton<ObjectiveManager>.Instance.CompleteObjective(m_objectiveDestroy);
	}

	private IEnumerator WaitForConfirmAndFail()
	{
		yield return new WaitForSeconds(5f);
		Singleton<ObjectiveManager>.Instance.FailObjective(m_objectiveDestroy);
		Singleton<ObjectiveManager>.Instance.FailObjective(m_objectiveRendezvous);
	}
}
