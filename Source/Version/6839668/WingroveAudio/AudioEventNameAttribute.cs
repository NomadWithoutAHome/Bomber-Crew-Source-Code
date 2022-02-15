using UnityEngine;

namespace WingroveAudio;

public class AudioEventNameAttribute : PropertyAttribute
{
	private bool m_compact;

	private bool m_onOffSwitch;

	public AudioEventNameAttribute()
	{
		m_compact = false;
		m_onOffSwitch = false;
	}

	public AudioEventNameAttribute(bool compact, bool onOffSwitch)
	{
		m_compact = compact;
		m_onOffSwitch = onOffSwitch;
	}

	public bool IsCompact()
	{
		return m_compact;
	}

	public bool IsOnOff()
	{
		return m_onOffSwitch;
	}
}
