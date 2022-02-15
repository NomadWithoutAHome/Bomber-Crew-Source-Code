using UnityEngine;

namespace WingroveAudio;

[AddComponentMenu("WingroveAudio/Event Triggers/Lifetime Event Trigger")]
public class LifetimeEventTrigger : MonoBehaviour
{
	[SerializeField]
	private bool m_fireEventOnStart;

	[SerializeField]
	[AudioEventName]
	private string m_startEvent = string.Empty;

	[SerializeField]
	private bool m_fireEventOnEnable;

	[SerializeField]
	[AudioEventName]
	private string m_onEnableEvent = string.Empty;

	[SerializeField]
	private bool m_fireEventOnDisable;

	[SerializeField]
	[AudioEventName]
	private string m_onDisableEvent = string.Empty;

	[SerializeField]
	private bool m_fireEventOnDestroy;

	[SerializeField]
	[AudioEventName]
	private string m_onDestroyEvent = string.Empty;

	[SerializeField]
	private bool m_dontPlayDestroyIfDisabled = true;

	[SerializeField]
	private bool m_linkToObject = true;

	private void Start()
	{
		if (WingroveRoot.Instance != null && m_fireEventOnStart)
		{
			if (m_linkToObject)
			{
				WingroveRoot.Instance.PostEventGO(m_startEvent, base.gameObject);
			}
			else
			{
				WingroveRoot.Instance.PostEvent(m_startEvent);
			}
		}
	}

	private void OnDisable()
	{
		if (WingroveRoot.Instance != null && m_fireEventOnDisable)
		{
			if (m_linkToObject)
			{
				WingroveRoot.Instance.PostEventGO(m_onDisableEvent, base.gameObject);
			}
			else
			{
				WingroveRoot.Instance.PostEvent(m_onDisableEvent);
			}
		}
	}

	private void OnEnable()
	{
		if (WingroveRoot.Instance != null && m_fireEventOnEnable)
		{
			if (m_linkToObject)
			{
				WingroveRoot.Instance.PostEventGO(m_onEnableEvent, base.gameObject);
			}
			else
			{
				WingroveRoot.Instance.PostEvent(m_onEnableEvent);
			}
		}
	}

	private void OnDestroy()
	{
		if (WingroveRoot.Instance != null && m_fireEventOnDestroy && (base.gameObject.activeInHierarchy || !m_dontPlayDestroyIfDisabled))
		{
			if (m_linkToObject)
			{
				WingroveRoot.Instance.PostEventGO(m_onDestroyEvent, base.gameObject);
			}
			else
			{
				WingroveRoot.Instance.PostEvent(m_onDestroyEvent);
			}
		}
	}
}
