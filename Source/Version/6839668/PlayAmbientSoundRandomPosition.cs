using System.Collections.Generic;
using UnityEngine;
using WingroveAudio;

public class PlayAmbientSoundRandomPosition : MonoBehaviour
{
	[SerializeField]
	[AudioEventName]
	private string m_audioNameStart;

	[SerializeField]
	[AudioEventName]
	private string m_audioNameStop;

	private void OnEnable()
	{
		List<ActiveCue> list = new List<ActiveCue>();
		if (!(WingroveRoot.Instance != null))
		{
			return;
		}
		WingroveRoot.Instance.PostEventCL(m_audioNameStart, null, list);
		foreach (ActiveCue item in list)
		{
			item.SetTime(Random.Range(0f, item.GetTimeUntilFinished(WingroveGroupInformation.HandleRepeatingAudio.GiveTimeUntilLoop)));
		}
	}

	private void OnDisable()
	{
		if (WingroveRoot.Instance != null)
		{
			WingroveRoot.Instance.PostEvent(m_audioNameStop);
		}
	}
}
