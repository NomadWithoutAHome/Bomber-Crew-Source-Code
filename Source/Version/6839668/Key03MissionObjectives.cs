using System;
using System.Collections;
using BomberCrewCommon;
using UnityEngine;

public class Key03MissionObjectives : MonoBehaviour
{
	private ObjectiveManager.Objective m_destroyV2MissileObjective;

	private void Start()
	{
		MissionCoordinator instance = Singleton<MissionCoordinator>.Instance;
		instance.OnTrigger = (Action<string>)Delegate.Combine(instance.OnTrigger, new Action<string>(OnTrigger));
		TopBarInfoQueue.TopBarRequest tbr = TopBarInfoQueue.TopBarRequest.Speech("mission_speech_dam_intro", Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(Singleton<TopBarInfoQueue>.Instance.GetCommandNamedText()), Singleton<TopBarInfoQueue>.Instance.GetCommandPortrait(), Singleton<TopBarInfoQueue>.Instance.GetCommandJabberText(), isGoodGuy: true);
		Singleton<TopBarInfoQueue>.Instance.RegisterRequest(tbr);
	}

	private void OnTrigger(string trigger)
	{
		if (trigger == "REACH_DAM_ANNOUNCE")
		{
			StartCoroutine(DoDamSpeech());
		}
		else if (trigger == "DAM_DESTROYED")
		{
			StopAllCoroutines();
			TopBarInfoQueue.TopBarRequest tbr = TopBarInfoQueue.TopBarRequest.Speech("mission_speech_dam_bounce_correct", Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(Singleton<TopBarInfoQueue>.Instance.GetCommandNamedText()), Singleton<TopBarInfoQueue>.Instance.GetCommandPortrait(), Singleton<TopBarInfoQueue>.Instance.GetCommandJabberText(), isGoodGuy: true);
			Singleton<TopBarInfoQueue>.Instance.RegisterRequest(tbr);
		}
	}

	private IEnumerator DoDamSpeech()
	{
		TopBarInfoQueue.TopBarRequest intro1 = TopBarInfoQueue.TopBarRequest.Speech("mission_speech_dam_near_1", Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(Singleton<TopBarInfoQueue>.Instance.GetCommandNamedText()), Singleton<TopBarInfoQueue>.Instance.GetCommandPortrait(), Singleton<TopBarInfoQueue>.Instance.GetCommandJabberText(), isGoodGuy: true);
		Singleton<TopBarInfoQueue>.Instance.RegisterRequest(intro1);
		yield return new WaitForSeconds(12.5f);
		TopBarInfoQueue.TopBarRequest intro2 = TopBarInfoQueue.TopBarRequest.Speech("mission_speech_dam_near_2", Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(Singleton<TopBarInfoQueue>.Instance.GetCommandNamedText()), Singleton<TopBarInfoQueue>.Instance.GetCommandPortrait(), Singleton<TopBarInfoQueue>.Instance.GetCommandJabberText(), isGoodGuy: true);
		Singleton<TopBarInfoQueue>.Instance.RegisterRequest(intro2);
	}
}
