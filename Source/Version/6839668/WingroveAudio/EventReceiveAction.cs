using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WingroveAudio;

[AddComponentMenu("WingroveAudio/Event Receive Action")]
public class EventReceiveAction : BaseEventReceiveAction
{
	public enum Actions
	{
		Play,
		PlayRandom,
		PlaySequence,
		Stop,
		Pause,
		UnPause,
		PlayRandomNoRepeats
	}

	[SerializeField]
	[AudioEventName]
	private string m_event = string.Empty;

	[SerializeField]
	private Actions m_action;

	[SerializeField]
	private float m_fadeLength;

	[SerializeField]
	private float m_delay;

	[SerializeField]
	private int m_noRepeatsMemory = 1;

	private BaseWingroveAudioSource[] m_audioSources;

	private int m_sequenceIndex;

	private Queue<int> m_previousRandoms = new Queue<int>();

	private List<int> m_availableRandoms = new List<int>();

	private void Awake()
	{
		m_audioSources = GetComponentsInChildren<BaseWingroveAudioSource>();
		for (int i = 0; i < m_audioSources.Length; i++)
		{
			m_availableRandoms.Add(i);
		}
	}

	public void CancelDelay()
	{
		StopAllCoroutines();
	}

	public override string[] GetEvents()
	{
		return new string[1] { m_event };
	}

	private IEnumerator PerformDelayedAction(string eventName, GameObject go, float delay)
	{
		yield return new WaitForSeconds(delay);
		PerformActionInternal(eventName, go, null);
	}

	private IEnumerator PerformDelayedAction(string eventName, GameObject go, AudioArea aa, float delay)
	{
		yield return new WaitForSeconds(delay);
		PerformActionInternal(eventName, go, aa, null);
	}

	private IEnumerator PerformDelayedAction(string eventName, List<ActiveCue> cuesIn, float delay)
	{
		yield return new WaitForSeconds(delay);
		PerformActionInternal(eventName, cuesIn, null);
	}

	public override void PerformAction(string eventName, GameObject targetObject, List<ActiveCue> cuesOut)
	{
		if (m_delay == 0f)
		{
			PerformActionInternal(eventName, targetObject, cuesOut);
		}
		else
		{
			StartCoroutine(PerformDelayedAction(eventName, targetObject, m_delay));
		}
	}

	public override void PerformAction(string eventName, GameObject targetObject, AudioArea aa, List<ActiveCue> cuesOut)
	{
		if (m_delay == 0f)
		{
			PerformActionInternal(eventName, targetObject, aa, cuesOut);
		}
		else
		{
			StartCoroutine(PerformDelayedAction(eventName, targetObject, aa, m_delay));
		}
	}

	public void PerformActionInternal(string eventName, GameObject targetObject, AudioArea aa, List<ActiveCue> cuesOut)
	{
		int num = Random.Range(0, m_audioSources.Length);
		bool flag = false;
		int num2 = 0;
		if (m_action == Actions.PlayRandomNoRepeats)
		{
			int index = Random.Range(0, m_availableRandoms.Count);
			num = m_availableRandoms[index];
			m_availableRandoms.Remove(num);
			m_previousRandoms.Enqueue(num);
			if (m_previousRandoms.Count > Mathf.Min(m_noRepeatsMemory, m_audioSources.Length - 1))
			{
				int item = m_previousRandoms.Dequeue();
				m_availableRandoms.Add(item);
			}
		}
		BaseWingroveAudioSource[] audioSources = m_audioSources;
		foreach (BaseWingroveAudioSource baseWingroveAudioSource in audioSources)
		{
			ActiveCue activeCue = baseWingroveAudioSource.GetCueForGameObject(targetObject);
			bool flag2 = true;
			if (targetObject != null && activeCue == null && (m_action == Actions.Pause || m_action == Actions.Stop || m_action == Actions.UnPause))
			{
				flag2 = false;
			}
			if (!flag2)
			{
				continue;
			}
			switch (m_action)
			{
			case Actions.Pause:
				activeCue = baseWingroveAudioSource.Pause(activeCue);
				break;
			case Actions.UnPause:
				activeCue = baseWingroveAudioSource.Unpause(activeCue);
				break;
			case Actions.Play:
				activeCue = baseWingroveAudioSource.Play(activeCue, m_fadeLength, targetObject, aa);
				break;
			case Actions.PlayRandom:
				if (num2 == num)
				{
					activeCue = baseWingroveAudioSource.Play(activeCue, m_fadeLength, targetObject, aa);
				}
				break;
			case Actions.PlaySequence:
				if (num2 == m_sequenceIndex)
				{
					activeCue = baseWingroveAudioSource.Play(activeCue, m_fadeLength, targetObject, aa);
					flag = true;
				}
				break;
			case Actions.Stop:
				activeCue = baseWingroveAudioSource.Stop(activeCue, m_fadeLength);
				break;
			case Actions.PlayRandomNoRepeats:
				if (num2 == num)
				{
					activeCue = baseWingroveAudioSource.Play(activeCue, m_fadeLength, targetObject, aa);
				}
				break;
			}
			cuesOut?.Add(activeCue);
			num2++;
		}
		if (flag)
		{
			m_sequenceIndex = (m_sequenceIndex + 1) % m_audioSources.Length;
		}
	}

	public void PerformActionInternal(string eventName, GameObject targetObject, List<ActiveCue> cuesOut)
	{
		int num = Random.Range(0, m_audioSources.Length);
		bool flag = false;
		int num2 = 0;
		if (m_action == Actions.PlayRandomNoRepeats)
		{
			int index = Random.Range(0, m_availableRandoms.Count);
			num = m_availableRandoms[index];
			m_availableRandoms.Remove(num);
			m_previousRandoms.Enqueue(num);
			if (m_previousRandoms.Count > Mathf.Min(m_noRepeatsMemory, m_audioSources.Length - 1))
			{
				int item = m_previousRandoms.Dequeue();
				m_availableRandoms.Add(item);
			}
		}
		BaseWingroveAudioSource[] audioSources = m_audioSources;
		foreach (BaseWingroveAudioSource baseWingroveAudioSource in audioSources)
		{
			ActiveCue activeCue = baseWingroveAudioSource.GetCueForGameObject(targetObject);
			bool flag2 = true;
			if (targetObject != null && activeCue == null && (m_action == Actions.Pause || m_action == Actions.Stop || m_action == Actions.UnPause))
			{
				flag2 = false;
			}
			if (!flag2)
			{
				continue;
			}
			switch (m_action)
			{
			case Actions.Pause:
				activeCue = baseWingroveAudioSource.Pause(activeCue);
				break;
			case Actions.UnPause:
				activeCue = baseWingroveAudioSource.Unpause(activeCue);
				break;
			case Actions.Play:
				activeCue = baseWingroveAudioSource.Play(activeCue, m_fadeLength, targetObject);
				break;
			case Actions.PlayRandom:
				if (num2 == num)
				{
					activeCue = baseWingroveAudioSource.Play(activeCue, m_fadeLength, targetObject);
				}
				break;
			case Actions.PlaySequence:
				if (num2 == m_sequenceIndex)
				{
					activeCue = baseWingroveAudioSource.Play(activeCue, m_fadeLength, targetObject);
					flag = true;
				}
				break;
			case Actions.Stop:
				activeCue = baseWingroveAudioSource.Stop(activeCue, m_fadeLength);
				break;
			case Actions.PlayRandomNoRepeats:
				if (num2 == num)
				{
					activeCue = baseWingroveAudioSource.Play(activeCue, m_fadeLength, targetObject);
				}
				break;
			}
			cuesOut?.Add(activeCue);
			num2++;
		}
		if (flag)
		{
			m_sequenceIndex = (m_sequenceIndex + 1) % m_audioSources.Length;
		}
	}

	public override void PerformAction(string eventName, List<ActiveCue> cuesIn, List<ActiveCue> cuesOut)
	{
		if (m_delay == 0f)
		{
			PerformActionInternal(eventName, cuesIn, cuesOut);
		}
		else
		{
			StartCoroutine(PerformDelayedAction(eventName, cuesIn, m_delay));
		}
	}

	public void PerformActionInternal(string eventName, List<ActiveCue> cuesIn, List<ActiveCue> cuesOut)
	{
		bool flag = false;
		BaseWingroveAudioSource[] audioSources = m_audioSources;
		foreach (BaseWingroveAudioSource baseWingroveAudioSource in audioSources)
		{
			if (cuesIn == null)
			{
				continue;
			}
			foreach (ActiveCue item2 in cuesIn)
			{
				if (item2.GetOriginatorSource() == baseWingroveAudioSource.gameObject)
				{
					flag = true;
				}
			}
		}
		int num = Random.Range(0, m_audioSources.Length);
		bool flag2 = false;
		int num2 = 0;
		if (m_action == Actions.PlayRandomNoRepeats)
		{
			int index = Random.Range(0, m_availableRandoms.Count);
			num = m_availableRandoms[index];
			m_availableRandoms.Remove(num);
			m_previousRandoms.Enqueue(num);
			if (m_previousRandoms.Count > Mathf.Min(m_noRepeatsMemory, m_audioSources.Length - 1))
			{
				int item = m_previousRandoms.Dequeue();
				m_availableRandoms.Add(item);
			}
		}
		BaseWingroveAudioSource[] audioSources2 = m_audioSources;
		foreach (BaseWingroveAudioSource baseWingroveAudioSource2 in audioSources2)
		{
			ActiveCue activeCue = null;
			if (cuesIn != null)
			{
				foreach (ActiveCue item3 in cuesIn)
				{
					if (item3.GetOriginatorSource() == baseWingroveAudioSource2.gameObject)
					{
						activeCue = item3;
						break;
					}
				}
			}
			if (!flag || activeCue != null)
			{
				switch (m_action)
				{
				case Actions.Pause:
					activeCue = baseWingroveAudioSource2.Pause(activeCue);
					break;
				case Actions.UnPause:
					activeCue = baseWingroveAudioSource2.Unpause(activeCue);
					break;
				case Actions.Play:
					activeCue = baseWingroveAudioSource2.Play(activeCue, m_fadeLength, null);
					break;
				case Actions.PlayRandom:
					if (num2 == num)
					{
						activeCue = baseWingroveAudioSource2.Play(activeCue, m_fadeLength, null);
					}
					break;
				case Actions.PlaySequence:
					if (num2 == m_sequenceIndex)
					{
						activeCue = baseWingroveAudioSource2.Play(activeCue, m_fadeLength, null);
						flag2 = true;
					}
					break;
				case Actions.Stop:
					activeCue = baseWingroveAudioSource2.Stop(activeCue, m_fadeLength);
					break;
				case Actions.PlayRandomNoRepeats:
					if (num2 == num)
					{
						activeCue = baseWingroveAudioSource2.Play(activeCue, m_fadeLength, null);
					}
					break;
				}
			}
			cuesOut?.Add(activeCue);
			num2++;
		}
		if (flag2)
		{
			m_sequenceIndex = (m_sequenceIndex + 1) % m_audioSources.Length;
		}
	}
}
