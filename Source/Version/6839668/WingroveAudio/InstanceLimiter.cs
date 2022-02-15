using System.Collections.Generic;
using UnityEngine;

namespace WingroveAudio;

[AddComponentMenu("WingroveAudio/Instance Limiter")]
public class InstanceLimiter : MonoBehaviour
{
	public enum LimitMethod
	{
		RemoveOldest,
		DontCreateNew
	}

	[SerializeField]
	private int m_instanceLimit = 1;

	[SerializeField]
	private LimitMethod m_limitMethod;

	[SerializeField]
	private float m_removedSourceFade;

	[SerializeField]
	private bool m_ignoreStopping = true;

	private List<ActiveCue> m_activeCues = new List<ActiveCue>();

	private bool m_requiresTidy;

	private int m_addedThisFrame;

	private List<GameObject> m_reusableLeysToRemove = new List<GameObject>(8);

	private void Start()
	{
		WingroveRoot.Instance.RegisterIL(this);
	}

	public void ResetFrameFlags()
	{
		m_addedThisFrame = 0;
		m_requiresTidy = true;
	}

	public bool CanPlay(GameObject attachedObject)
	{
		if (m_requiresTidy)
		{
			Tidy();
		}
		if (m_limitMethod == LimitMethod.DontCreateNew)
		{
			if (m_activeCues.Count >= m_instanceLimit)
			{
				return false;
			}
		}
		else if (m_addedThisFrame >= m_instanceLimit)
		{
			return false;
		}
		return true;
	}

	public void AddCue(ActiveCue cue, GameObject attachedObject)
	{
		m_addedThisFrame++;
		m_activeCues.Add(cue);
		Limit(attachedObject);
	}

	public void RemoveCue(ActiveCue cue)
	{
		if (m_addedThisFrame > 0)
		{
			m_addedThisFrame--;
		}
		m_activeCues.Remove(cue);
	}

	private void Tidy()
	{
		m_requiresTidy = false;
		List<ActiveCue> list = new List<ActiveCue>(8);
		list.Clear();
		List<ActiveCue>.Enumerator enumerator = m_activeCues.GetEnumerator();
		while (enumerator.MoveNext())
		{
			ActiveCue current = enumerator.Current;
			if (current == null || current.GetState() == ActiveCue.CueState.Stopped)
			{
				list.Add(current);
			}
			else if (m_ignoreStopping && current.GetState() == ActiveCue.CueState.PlayingFadeOut)
			{
				list.Add(current);
			}
		}
		enumerator = list.GetEnumerator();
		while (enumerator.MoveNext())
		{
			m_activeCues.Remove(enumerator.Current);
		}
	}

	private void Limit(GameObject attachedObject)
	{
		if (m_requiresTidy)
		{
			Tidy();
		}
		if (m_activeCues.Count > m_instanceLimit)
		{
			if (m_activeCues[0] != null)
			{
				m_activeCues[0].Stop(m_removedSourceFade);
			}
			m_activeCues.Remove(m_activeCues[0]);
		}
	}
}
