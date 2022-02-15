using System.Collections.Generic;
using UnityEngine;

namespace WingroveAudio;

[RequireComponent(typeof(WingroveMixBus))]
[AddComponentMenu("WingroveAudio/Mix Bus Ducking/Level Based Automatic Duck")]
public class DuckMixBusLevelBased : BaseAutomaticDuck
{
	[SerializeField]
	private WingroveMixBus m_mixBusToMonitor;

	[SerializeField]
	[FaderInterface]
	private float m_threshold = 0.5f;

	[SerializeField]
	[FaderInterface]
	private float m_duckingMixAmount = 1f;

	[SerializeField]
	private float m_ratio = 4f;

	[SerializeField]
	private float m_attack;

	[SerializeField]
	private float m_release;

	private float m_fadeT;

	private WingroveMixBus m_mixBus;

	private void Awake()
	{
		m_mixBus = GetComponent<WingroveMixBus>();
		m_mixBus.AddDuck(this);
	}

	public WingroveMixBus GetMixBusToMonitor()
	{
		return m_mixBusToMonitor;
	}

	private void Update()
	{
		if (m_ratio != 0f)
		{
			float num = 0f;
			float num2 = m_mixBusToMonitor.GetRMS() / 0.707f;
			float value = Mathf.Max(0f, num2 - m_threshold) * m_ratio;
			num = Mathf.Clamp01(value);
			float num3 = ((!(num > m_fadeT)) ? (1f / Mathf.Max(m_release, 0.01f)) : (1f / Mathf.Max(m_attack, 0.01f)));
			m_fadeT += Mathf.Min(Mathf.Abs(num3 * WingroveRoot.GetDeltaTime()), Mathf.Abs(num - m_fadeT)) * Mathf.Sign(num - m_fadeT);
		}
	}

	public override float GetDuckVol()
	{
		return Mathf.Clamp01(1f * (1f - m_fadeT) + m_duckingMixAmount * m_fadeT);
	}

	public override string GetGroupName()
	{
		return m_mixBusToMonitor.name;
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
