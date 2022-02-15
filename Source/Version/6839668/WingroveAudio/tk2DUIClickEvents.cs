using UnityEngine;

namespace WingroveAudio;

[AddComponentMenu("WingroveAudio/Event Triggers/tk2D Click Events Trigger")]
public class tk2DUIClickEvents : MonoBehaviour
{
	public bool m_fireEventOnPress;

	[AudioEventName]
	public string m_onPressEvent;

	public bool m_fireEventOnClick;

	[AudioEventName]
	public string m_onClickEvent;

	public bool m_fireEventOnRelease;

	[AudioEventName]
	public string m_onReleaseEvent;

	public bool m_fireEventOnDoubleClick;

	[AudioEventName]
	public string m_onDoubleClickEvent;

	private void Awake()
	{
		tk2dUIItem component = GetComponent<tk2dUIItem>();
		if (component != null)
		{
			component.OnClick += OnClick;
			component.OnDown += OnPress;
			component.OnUp += OnRelease;
		}
	}

	private void OnPress()
	{
		if (WingroveRoot.Instance != null && m_fireEventOnPress)
		{
			WingroveRoot.Instance.PostEvent(m_onPressEvent);
		}
	}

	private void OnRelease()
	{
		if (WingroveRoot.Instance != null && m_fireEventOnRelease)
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
}
