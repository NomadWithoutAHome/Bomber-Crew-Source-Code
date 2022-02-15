using System;
using BomberCrewCommon;
using UnityEngine;

public class SpeechTrigger : MonoBehaviour
{
	[SerializeField]
	private MissionPlaceableObject m_placeable;

	private bool m_hasTriggered;

	private void Start()
	{
		string triggerOnEvent = m_placeable.GetParameter("trigger");
		string triggerCancel = m_placeable.GetParameter("cancelTrigger");
		string text = m_placeable.GetParameter("text");
		MissionCoordinator instance = Singleton<MissionCoordinator>.Instance;
		instance.OnTrigger = (Action<string>)Delegate.Combine(instance.OnTrigger, (Action<string>)delegate(string st)
		{
			if (!m_hasTriggered)
			{
				if (!string.IsNullOrEmpty(triggerCancel) && st == triggerCancel)
				{
					m_hasTriggered = true;
				}
				if (st == triggerOnEvent)
				{
					m_hasTriggered = true;
					TopBarInfoQueue.TopBarRequest tbr = TopBarInfoQueue.TopBarRequest.Speech(text, Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(Singleton<TopBarInfoQueue>.Instance.GetCommandNamedText()), Singleton<TopBarInfoQueue>.Instance.GetCommandPortrait(), Singleton<TopBarInfoQueue>.Instance.GetCommandJabberText(), isGoodGuy: true);
					Singleton<TopBarInfoQueue>.Instance.RegisterRequest(tbr);
				}
			}
		});
	}
}
