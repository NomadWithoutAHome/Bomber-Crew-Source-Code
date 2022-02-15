using System.Collections.Generic;
using UnityEngine;

namespace WingroveAudio;

[AddComponentMenu("WingroveAudio/Mix Bus")]
public class WingroveMixBus : MonoBehaviour
{
	[SerializeField]
	[FaderInterface]
	private float m_volumeMult = 1f;

	[SerializeField]
	private int m_importance;

	private float m_mixedVol = 1f;

	private WingroveMixBus m_parent;

	private int m_totalImportance;

	private List<BaseAutomaticDuck> m_duckList = new List<BaseAutomaticDuck>();

	private List<BaseWingroveAudioSource> m_audioSources = new List<BaseWingroveAudioSource>();

	private List<WingroveMixBus> m_childMixBuses = new List<WingroveMixBus>();

	private bool m_hasParent;

	public float Volume
	{
		get
		{
			return m_volumeMult;
		}
		set
		{
			m_volumeMult = value;
		}
	}

	private void Awake()
	{
		m_parent = FindParentMixBus(base.transform.parent);
		if (m_parent != null)
		{
			m_hasParent = true;
			m_parent.RegisterMixBus(this);
		}
		WingroveRoot.Instance.RegisterMixBus(this);
	}

	private void OnDestroy()
	{
		if (m_parent != null)
		{
			m_parent.UnregisterMixBus(this);
		}
	}

	public void RegisterMixBus(WingroveMixBus mixBus)
	{
		m_childMixBuses.Add(mixBus);
	}

	public void UnregisterMixBus(WingroveMixBus mixBus)
	{
		m_childMixBuses.Remove(mixBus);
	}

	public void AddDuck(BaseAutomaticDuck duck)
	{
		m_duckList.Add(duck);
	}

	public void RegisterSource(BaseWingroveAudioSource source)
	{
		m_audioSources.Add(source);
	}

	public void RemoveSource(BaseWingroveAudioSource source)
	{
		m_audioSources.Remove(source);
	}

	public float GetRMS()
	{
		float num = 0f;
		foreach (BaseWingroveAudioSource audioSource in m_audioSources)
		{
			float rMS = audioSource.GetRMS();
			num += rMS * rMS;
		}
		foreach (WingroveMixBus childMixBuse in m_childMixBuses)
		{
			float rMS2 = childMixBuse.GetRMS();
			num += rMS2 * rMS2;
		}
		return Mathf.Sqrt(num);
	}

	public static WingroveMixBus FindParentMixBus(Transform t)
	{
		if (t == null)
		{
			return null;
		}
		WingroveMixBus component = t.GetComponent<WingroveMixBus>();
		if (component == null)
		{
			return FindParentMixBus(t.parent);
		}
		return component;
	}

	public static InstanceLimiter FindParentLimiter(Transform t)
	{
		if (t == null)
		{
			return null;
		}
		InstanceLimiter component = t.GetComponent<InstanceLimiter>();
		if (component == null)
		{
			return FindParentLimiter(t.parent);
		}
		return component;
	}

	public void DoUpdate()
	{
		if (!m_hasParent)
		{
			m_mixedVol = m_volumeMult;
			m_totalImportance = m_importance;
		}
		else
		{
			m_mixedVol = m_parent.GetMixedVol() * m_volumeMult;
			m_totalImportance = m_parent.GetImportance() + m_totalImportance;
		}
		List<BaseAutomaticDuck>.Enumerator enumerator = m_duckList.GetEnumerator();
		while (enumerator.MoveNext())
		{
			m_mixedVol *= enumerator.Current.GetDuckVol();
		}
	}

	public List<BaseAutomaticDuck> GetDuckList()
	{
		return m_duckList;
	}

	public float GetMixedVol()
	{
		return m_mixedVol;
	}

	public int GetImportance()
	{
		return m_totalImportance;
	}
}
