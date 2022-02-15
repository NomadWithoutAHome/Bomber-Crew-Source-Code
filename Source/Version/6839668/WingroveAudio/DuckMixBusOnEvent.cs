using System.Collections.Generic;
using UnityEngine;

namespace WingroveAudio;

[RequireComponent(typeof(WingroveMixBus))]
[AddComponentMenu("WingroveAudio/Mix Bus Ducking/Duck on Event")]
public class DuckMixBusOnEvent : BaseAutomaticDuck
{
	private enum DuckState
	{
		Inactive,
		Attack,
		Hold,
		Release
	}

	[SerializeField]
	[AudioEventName]
	private string m_startDuckEvent = string.Empty;

	[SerializeField]
	[AudioEventName]
	private string m_endDuckEvent = string.Empty;

	[SerializeField]
	private float m_attack;

	[SerializeField]
	private float m_release;

	[SerializeField]
	[FaderInterface]
	private float m_duckingMixAmount = 1f;

	private bool m_isActive;

	private DuckState m_currentState;

	private float m_fadeT;

	private float m_faderSpeed;

	private WingroveMixBus m_mixBus;

	private void Awake()
	{
		m_mixBus = GetComponent<WingroveMixBus>();
		m_mixBus.AddDuck(this);
	}

	private void Update()
	{
		if (m_isActive)
		{
			if (m_currentState == DuckState.Inactive || m_currentState == DuckState.Release)
			{
				if (m_attack == 0f)
				{
					m_currentState = DuckState.Hold;
					m_fadeT = 1f;
				}
				else
				{
					m_currentState = DuckState.Attack;
					m_faderSpeed = 1f / m_attack;
				}
			}
		}
		else if (m_currentState != 0)
		{
			if (m_release == 0f)
			{
				m_currentState = DuckState.Inactive;
				m_fadeT = 0f;
			}
			else
			{
				m_currentState = DuckState.Release;
				m_faderSpeed = 1f / m_release;
			}
		}
		switch (m_currentState)
		{
		case DuckState.Attack:
			m_fadeT += m_faderSpeed * WingroveRoot.GetDeltaTime();
			if (m_fadeT >= 1f)
			{
				m_fadeT = 1f;
				m_currentState = DuckState.Hold;
			}
			break;
		case DuckState.Hold:
			m_fadeT = 1f;
			break;
		case DuckState.Release:
			m_fadeT -= m_faderSpeed * WingroveRoot.GetDeltaTime();
			if (m_fadeT <= 0f)
			{
				m_fadeT = 0f;
				m_currentState = DuckState.Inactive;
			}
			break;
		case DuckState.Inactive:
			break;
		}
	}

	public override string GetGroupName()
	{
		return base.name;
	}

	public override float GetDuckVol()
	{
		return 1f * (1f - m_fadeT) + m_duckingMixAmount * m_fadeT;
	}

	public override string[] GetEvents()
	{
		return new string[2] { m_startDuckEvent, m_endDuckEvent };
	}

	private void PerformInternal(string eventName)
	{
		if (eventName == m_startDuckEvent)
		{
			m_isActive = true;
			if (m_attack == 0f)
			{
				m_currentState = DuckState.Hold;
				m_fadeT = 1f;
			}
		}
		else if (eventName == m_endDuckEvent)
		{
			m_isActive = false;
			if (m_release == 0f)
			{
				m_currentState = DuckState.Inactive;
				m_fadeT = 0f;
			}
		}
	}

	public override void PerformAction(string eventName, GameObject targetObject, List<ActiveCue> cuesOut)
	{
		PerformInternal(eventName);
	}

	public override void PerformAction(string eventName, List<ActiveCue> cuesIn, List<ActiveCue> cuesOut)
	{
		PerformInternal(eventName);
	}

	public override void PerformAction(string eventName, GameObject targetObject, AudioArea aa, List<ActiveCue> cuesOut)
	{
		PerformInternal(eventName);
	}
}
