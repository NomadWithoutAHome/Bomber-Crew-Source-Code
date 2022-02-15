using UnityEngine;

public class PooledAudioSource : MonoBehaviour
{
	private AudioLowPassFilter m_lowPassFilter;

	private AudioHighPassFilter m_highPassFilter;

	private AudioReverbFilter m_reverbFilter;

	private int m_numLowPassFilters;

	private float m_lowPassResTotal;

	private float m_lowPassFreq;

	private int m_numHighPassFilters;

	private float m_highPassResTotal;

	private float m_highPassFreq;

	private bool m_previouslyWasEnabledHP;

	private bool m_previouslyWasEnabledLP;

	private float m_previousLowFreq = 999999f;

	private float m_previousHighFreq = 999999f;

	private float m_previousLowQ = 99999f;

	private float m_previousHighQ = 99999f;

	private void Awake()
	{
		m_lowPassFilter = base.gameObject.AddComponent<AudioLowPassFilter>();
		m_lowPassFilter.enabled = false;
		m_highPassFilter = base.gameObject.AddComponent<AudioHighPassFilter>();
		m_highPassFilter.enabled = false;
	}

	public void SetLowPassFilter(float freq, float res)
	{
		if (freq < 44000f)
		{
			m_lowPassFreq = Mathf.Min(freq, m_lowPassFreq);
			m_lowPassResTotal += res;
			m_numLowPassFilters++;
		}
	}

	public void SetHighPassFilter(float freq, float res)
	{
		if (freq > 100f)
		{
			m_highPassFreq = Mathf.Max(freq, m_highPassFreq);
			m_highPassResTotal += res;
			m_numHighPassFilters++;
		}
	}

	public void ResetFiltersForFrame()
	{
		m_lowPassFreq = 96000f;
		m_numLowPassFilters = 0;
		m_lowPassResTotal = 1f;
		m_highPassFreq = 0f;
		m_highPassResTotal = 1f;
		m_numHighPassFilters = 0;
	}

	public void CommitFiltersForFrame()
	{
		if (m_numLowPassFilters == 0)
		{
			if (m_previouslyWasEnabledLP)
			{
				m_lowPassFilter.enabled = false;
			}
			m_previouslyWasEnabledLP = false;
		}
		else
		{
			if (!m_previouslyWasEnabledLP)
			{
				m_previouslyWasEnabledLP = true;
			}
			m_lowPassFilter.enabled = true;
			if (m_previousLowFreq != m_lowPassFreq)
			{
				m_lowPassFilter.cutoffFrequency = m_lowPassFreq;
			}
			m_previousLowFreq = m_lowPassFreq;
			float num = m_lowPassResTotal / (float)m_numLowPassFilters;
			if (m_previousLowQ != num)
			{
				m_lowPassFilter.lowpassResonanceQ = num;
			}
			m_previousLowQ = num;
		}
		if (m_numHighPassFilters == 0)
		{
			if (m_previouslyWasEnabledHP)
			{
				m_highPassFilter.enabled = false;
			}
			m_previouslyWasEnabledHP = false;
			return;
		}
		if (!m_previouslyWasEnabledHP)
		{
			m_highPassFilter.enabled = true;
		}
		m_previouslyWasEnabledHP = true;
		if (m_previousHighFreq != m_highPassFreq)
		{
			m_highPassFilter.cutoffFrequency = m_highPassFreq;
		}
		m_previousHighFreq = m_highPassFreq;
		float num2 = m_highPassResTotal / (float)m_numHighPassFilters;
		if (m_previousHighQ != num2)
		{
			m_highPassFilter.highpassResonanceQ = num2;
		}
		m_previousHighQ = num2;
	}
}
