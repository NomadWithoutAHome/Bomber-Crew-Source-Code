using System;
using System.Collections;
using BomberCrewCommon;
using UnityEngine;

public class Key04MissionObjectives : MonoBehaviour
{
	private ObjectiveManager.Objective m_destroyV2MissileObjective;

	private void Start()
	{
		MissionCoordinator instance = Singleton<MissionCoordinator>.Instance;
		instance.OnTrigger = (Action<string>)Delegate.Combine(instance.OnTrigger, new Action<string>(OnTrigger));
		TopBarInfoQueue.TopBarRequest tbr = TopBarInfoQueue.TopBarRequest.Speech("mission_speech_radar_site_intro", Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(Singleton<TopBarInfoQueue>.Instance.GetCommandNamedText()), Singleton<TopBarInfoQueue>.Instance.GetCommandPortrait(), Singleton<TopBarInfoQueue>.Instance.GetCommandJabberText(), isGoodGuy: true);
		Singleton<TopBarInfoQueue>.Instance.RegisterRequest(tbr);
		StartCoroutine(DoDelayedInfo());
	}

	private IEnumerator DoDelayedInfo()
	{
		TopBarInfoQueue.TopBarRequest intro1 = TopBarInfoQueue.TopBarRequest.Standard("mission_tutorial_radar_site_intro_info", null, Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.Navigator), 15f, 15f, 0f, 1);
		Singleton<TopBarInfoQueue>.Instance.RegisterRequest(intro1);
		yield return new WaitForSeconds(15f);
		Singleton<TopBarInfoQueue>.Instance.RemoveRequest(intro1);
	}

	private void OnTrigger(string trigger)
	{
		if (trigger.StartsWith("RADARSITEA"))
		{
			TopBarInfoQueue.TopBarRequest tbr = TopBarInfoQueue.TopBarRequest.Speech("mission_speech_radar_site_spot", Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(Singleton<TopBarInfoQueue>.Instance.GetCommandNamedText()), Singleton<TopBarInfoQueue>.Instance.GetCommandPortrait(), Singleton<TopBarInfoQueue>.Instance.GetCommandJabberText(), isGoodGuy: true);
			Singleton<TopBarInfoQueue>.Instance.RegisterRequest(tbr);
			return;
		}
		switch (trigger)
		{
		case "RADARSITEB":
		{
			TopBarInfoQueue.TopBarRequest tbr5 = TopBarInfoQueue.TopBarRequest.Speech("mission_speech_radar_site_spot_second", Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(Singleton<TopBarInfoQueue>.Instance.GetCommandNamedText()), Singleton<TopBarInfoQueue>.Instance.GetCommandPortrait(), Singleton<TopBarInfoQueue>.Instance.GetCommandJabberText(), isGoodGuy: true);
			Singleton<TopBarInfoQueue>.Instance.RegisterRequest(tbr5);
			break;
		}
		case "RADARSTART":
		{
			TopBarInfoQueue.TopBarRequest tbr4 = TopBarInfoQueue.TopBarRequest.Speech("mission_speech_radar_site_spot_warn", Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(Singleton<TopBarInfoQueue>.Instance.GetCommandNamedText()), Singleton<TopBarInfoQueue>.Instance.GetCommandPortrait(), Singleton<TopBarInfoQueue>.Instance.GetCommandJabberText(), isGoodGuy: true);
			Singleton<TopBarInfoQueue>.Instance.RegisterRequest(tbr4);
			break;
		}
		case "RADARSTARTNR":
		{
			TopBarInfoQueue.TopBarRequest tbr3 = TopBarInfoQueue.TopBarRequest.Speech("mission_speech_radar_site_spot_warn_close", Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(Singleton<TopBarInfoQueue>.Instance.GetCommandNamedText()), Singleton<TopBarInfoQueue>.Instance.GetCommandPortrait(), Singleton<TopBarInfoQueue>.Instance.GetCommandJabberText(), isGoodGuy: true);
			Singleton<TopBarInfoQueue>.Instance.RegisterRequest(tbr3);
			break;
		}
		case "KEY04ESCAPE":
		{
			TopBarInfoQueue.TopBarRequest tbr2 = TopBarInfoQueue.TopBarRequest.Speech("mission_speech_key04_officerfled", Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(Singleton<TopBarInfoQueue>.Instance.GetCommandNamedText()), Singleton<TopBarInfoQueue>.Instance.GetCommandPortrait(), Singleton<TopBarInfoQueue>.Instance.GetCommandJabberText(), isGoodGuy: true);
			Singleton<TopBarInfoQueue>.Instance.RegisterRequest(tbr2);
			break;
		}
		}
	}
}
