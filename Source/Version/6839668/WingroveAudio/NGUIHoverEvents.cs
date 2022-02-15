using UnityEngine;

namespace WingroveAudio;

[AddComponentMenu("WingroveAudio/Event Triggers/NGUI Hover Events Trigger")]
public class NGUIHoverEvents : MonoBehaviour
{
	[SerializeField]
	private bool m_fireEventOnHover;

	[SerializeField]
	[AudioEventName]
	private string m_hoverEvent;

	[SerializeField]
	private bool m_fireEventOnStopHover;

	[SerializeField]
	[AudioEventName]
	private string m_stopHoverEvent;

	private void OnHover(bool hover)
	{
		if (!(WingroveRoot.Instance != null))
		{
			return;
		}
		if (hover)
		{
			if (m_fireEventOnHover)
			{
				WingroveRoot.Instance.PostEvent(m_hoverEvent);
			}
		}
		else if (m_fireEventOnStopHover)
		{
			WingroveRoot.Instance.PostEvent(m_stopHoverEvent);
		}
	}
}
