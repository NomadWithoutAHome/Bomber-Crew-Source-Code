using UnityEngine;

namespace WingroveAudio;

public class WingroveFilterLowPass : FilterApplicationBase
{
	[SerializeField]
	[AudioParameterName]
	private string m_filterParameterController;

	[SerializeField]
	private float m_filterLowCutParameterAtZero = 5000f;

	[SerializeField]
	private float m_filterLowCutParameterAtOne = 5000f;

	[SerializeField]
	private float m_resonanceAtZero = 1f;

	[SerializeField]
	private float m_resonanceAtOne = 1f;

	[SerializeField]
	private bool m_smoothStep = true;

	private int m_cachedParameterValue;

	private WingroveRoot.CachedParameterValue m_cachedParameterValueActual;

	private bool m_hasEverCalculated;

	private float m_previousDt;

	private float m_previousFilter;

	private float m_previousResonance;

	public override void UpdateFor(PooledAudioSource playingSource, int linkedObjectId)
	{
		if (m_cachedParameterValue == 0)
		{
			m_cachedParameterValue = WingroveRoot.Instance.GetParameterId(m_filterParameterController);
		}
		else if (m_cachedParameterValueActual == null)
		{
			m_cachedParameterValueActual = WingroveRoot.Instance.GetParameter(m_cachedParameterValue);
		}
		float num = m_previousFilter;
		float num2 = m_previousResonance;
		float value = 0f;
		if (m_cachedParameterValueActual != null)
		{
			if (m_cachedParameterValueActual.m_isGlobalValue)
			{
				value = m_cachedParameterValueActual.m_valueNull;
			}
			else
			{
				m_cachedParameterValueActual.m_valueObject.TryGetValue(linkedObjectId, out value);
			}
		}
		if (value != m_previousDt || !m_hasEverCalculated)
		{
			if (m_smoothStep)
			{
				num = Mathf.SmoothStep(m_filterLowCutParameterAtZero, m_filterLowCutParameterAtOne, value);
				num2 = Mathf.SmoothStep(m_resonanceAtZero, m_resonanceAtOne, value);
			}
			else
			{
				num = Mathf.Lerp(m_filterLowCutParameterAtZero, m_filterLowCutParameterAtOne, value);
				num2 = Mathf.Lerp(m_resonanceAtZero, m_resonanceAtOne, value);
			}
			m_hasEverCalculated = true;
			m_previousFilter = num;
			m_previousResonance = num2;
			m_previousDt = value;
		}
		playingSource.SetLowPassFilter(num, num2);
	}
}
