using UnityEngine;

namespace WingroveAudio;

[AddComponentMenu("WingroveAudio/Event Triggers/NGUI Click Events Trigger")]
public class NGUIClickEvents : MonoBehaviour
{
	[SerializeField]
	public bool m_fireEventOnPress;

	[SerializeField]
	[AudioEventName]
	public string m_onPressEvent;

	[SerializeField]
	public bool m_fireEventOnClick;

	[SerializeField]
	[AudioEventName]
	public string m_onClickEvent;

	[SerializeField]
	public bool m_fireEventOnRelease;

	[SerializeField]
	[AudioEventName]
	public string m_onReleaseEvent;

	[SerializeField]
	public bool m_fireEventOnDoubleClick;

	[SerializeField]
	[AudioEventName]
	public string m_onDoubleClickEvent;

	private void OnPress(bool pressed)
	{
		if (pressed)
		{
			if (WingroveRoot.Instance != null && m_fireEventOnPress)
			{
				WingroveRoot.Instance.PostEvent(m_onPressEvent);
			}
		}
		else if (WingroveRoot.Instance != null && m_fireEventOnRelease)
		{
			WingroveRoot.Instance.PostEvent(m_onReleaseEvent);
		}
	}

	private void OnClick()
	{
		if (WingroveRoot.Instance != null && m_fireEventOnClick)
		{
			WingroveRoot.Instance.PostEvent(m_onClickEvent);
		}
	}

	private void OnDoubleClick()
	{
		if (WingroveRoot.Instance != null && m_fireEventOnDoubleClick)
		{
			WingroveRoot.Instance.PostEvent(m_onDoubleClickEvent);
		}
	}
}
