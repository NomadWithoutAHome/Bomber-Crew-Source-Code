using System.Collections.Generic;
using UnityEngine;

namespace WingroveAudio;

[RequireComponent(typeof(WingroveMixBus))]
[AddComponentMenu("WingroveAudio/Mix Bus Ducking/Simple Automatic Duck")]
public class DuckMixBusSimpleAutomatic : BaseAutomaticDuck
{
	private enum DuckState
	{
		Inactive,
		Attack,
		Hold,
		Release
	}

	[SerializeField]
	private GameObject m_groupToMonitor;

	[SerializeField]
	[FaderInterface]
	private float m_duckingMixAmount = 1f;

	[SerializeField]
	private float m_attack;

	[SerializeField]
	private float m_release;

	private BaseWingroveAudioSource[] m_audioSources;

	private DuckState m_currentState;

	private float m_fadeT;

	private float m_faderSpeed;

	private WingroveMixBus m_mixBus;

	private void Awake()
	{
		m_audioSources = m_groupToMonitor.GetComponentsInChildren<BaseWingroveAudioSource>();
		m_mixBus = GetComponent<WingroveMixBus>();
		m_mixBus.AddDuck(this);
	}

	private void Update()
	{
		bool flag = false;
		BaseWingroveAudioSource[] audioSources = m_audioSources;
		foreach (BaseWingroveAudioSource baseWingroveAudioSource in audioSources)
		{
			if (baseWingroveAudioSource.IsPlaying())
			{
				flag = true;
				break;
			}
		}
		if (flag)
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
			if (m_attack == 0f)
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

	public override float GetDuckVol()
	{
		return 1f * (1f - m_fadeT) + m_duckingMixAmount * m_fadeT;
	}

	public override string GetGroupName()
	{
		return m_groupToMonitor.name;
	}

	public override string[] GetEvents()
	{
		return null;
	}

	public override void PerformAction(string eventName, GameObject targetObject, List<ActiveCue> cuesOut)
	{
	}

	public override void PerformAction(string eventName, List<ActiveCue> cuesIn, List<ActiveCue> cuesOut)
	{
	}

	public override void PerformAction(string eventName, GameObject targetObject, AudioArea aa, List<ActiveCue> cuesOut)
	{
	}
}
