using System;
using BomberCrewCommon;
using UnityEngine;

public class Key02MissionObjectives : MonoBehaviour
{
	private ObjectiveManager.Objective m_destroyV2MissileObjective;

	private void Start()
	{
		MissionCoordinator instance = Singleton<MissionCoordinator>.Instance;
		instance.OnTrigger = (Action<string>)Delegate.Combine(instance.OnTrigger, new Action<string>(OnTrigger));
	}

	private void OnTrigger(string trigger)
	{
		if (trigger == "REACH_V2_ANNOUNCE")
		{
			m_destroyV2MissileObjective = new ObjectiveManager.Objective();
			m_destroyV2MissileObjective.m_objectiveTitle = "mission_objective_destroy_V2_missile";
			m_destroyV2MissileObjective.m_primary = false;
			m_destroyV2MissileObjective.m_countToComplete = 1;
			m_destroyV2MissileObjective.m_objectiveType = ObjectiveManager.ObjectiveType.Other;
			Singleton<ObjectiveManager>.Instance.RegisterObjective(m_destroyV2MissileObjective);
			TopBarInfoQueue.TopBarRequest tbr = TopBarInfoQueue.TopBarRequest.Speech("mission_speech_v2_launch_stop", Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(Singleton<TopBarInfoQueue>.Instance.GetCommandNamedText()), Singleton<TopBarInfoQueue>.Instance.GetCommandPortrait(), Singleton<TopBarInfoQueue>.Instance.GetCommandJabberText(), isGoodGuy: true);
			Singleton<TopBarInfoQueue>.Instance.RegisterRequest(tbr);
		}
		switch (trigger)
		{
		case "REACH_V2_START":
		{
			TopBarInfoQueue.TopBarRequest tbr3 = TopBarInfoQueue.TopBarRequest.Speech("mission_speech_v2_launch_imminent", Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(Singleton<TopBarInfoQueue>.Instance.GetCommandNamedText()), Singleton<TopBarInfoQueue>.Instance.GetCommandPortrait(), Singleton<TopBarInfoQueue>.Instance.GetCommandJabberText(), isGoodGuy: true);
			Singleton<TopBarInfoQueue>.Instance.RegisterRequest(tbr3);
			break;
		}
		case "V2_ESCAPED":
		{
			if (m_destroyV2MissileObjective != null)
			{
				Singleton<ObjectiveManager>.Instance.FailObjective(m_destroyV2MissileObjective);
			}
			Singleton<MissionCoordinator>.Instance.FireTrigger("REACH_V2_END");
			TopBarInfoQueue.TopBarRequest tbr4 = TopBarInfoQueue.TopBarRequest.Speech("mission_speech_v2_launch_failed", Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(Singleton<TopBarInfoQueue>.Instance.GetCommandNamedText()), Singleton<TopBarInfoQueue>.Instance.GetCommandPortrait(), Singleton<TopBarInfoQueue>.Instance.GetCommandJabberText(), isGoodGuy: true);
			Singleton<TopBarInfoQueue>.Instance.RegisterRequest(tbr4);
			break;
		}
		case "V2_DESTROYED":
		{
			if (m_destroyV2MissileObjective != null)
			{
				Singleton<ObjectiveManager>.Instance.CompleteObjective(m_destroyV2MissileObjective);
			}
			Singleton<MissionCoordinator>.Instance.FireTrigger("REACH_V2_END");
			TopBarInfoQueue.TopBarRequest tbr2 = TopBarInfoQueue.TopBarRequest.Speech("mission_speech_v2_launch_success", Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(Singleton<TopBarInfoQueue>.Instance.GetCommandNamedText()), Singleton<TopBarInfoQueue>.Instance.GetCommandPortrait(), Singleton<TopBarInfoQueue>.Instance.GetCommandJabberText(), isGoodGuy: true);
			Singleton<TopBarInfoQueue>.Instance.RegisterRequest(tbr2);
			break;
		}
		}
	}
}
