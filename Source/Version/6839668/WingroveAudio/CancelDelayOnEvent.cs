using System.Collections.Generic;
using UnityEngine;

namespace WingroveAudio;

[AddComponentMenu("WingroveAudio/Cancel Delay Event Action")]
public class CancelDelayOnEvent : BaseEventReceiveAction
{
	[SerializeField]
	[AudioEventName]
	private string m_event = string.Empty;

	[SerializeField]
	private EventReceiveAction[] m_toCancel;

	public override string[] GetEvents()
	{
		return new string[1] { m_event };
	}

	public override void PerformAction(string eventName, GameObject targetObject, List<ActiveCue> cuesOut)
	{
		EventReceiveAction[] toCancel = m_toCancel;
		foreach (EventReceiveAction eventReceiveAction in toCancel)
		{
			if (eventReceiveAction != null)
			{
				eventReceiveAction.CancelDelay();
			}
		}
	}

	public override void PerformAction(string eventName, List<ActiveCue> cuesIn, List<ActiveCue> cuesOut)
	{
		EventReceiveAction[] toCancel = m_toCancel;
		foreach (EventReceiveAction eventReceiveAction in toCancel)
		{
			if (eventReceiveAction != null)
			{
				eventReceiveAction.CancelDelay();
			}
		}
	}

	public override void PerformAction(string eventName, GameObject targetObject, AudioArea aa, List<ActiveCue> cuesOut)
	{
		EventReceiveAction[] toCancel = m_toCancel;
		foreach (EventReceiveAction eventReceiveAction in toCancel)
		{
			if (eventReceiveAction != null)
			{
				eventReceiveAction.CancelDelay();
			}
		}
	}
}
