using UnityEngine;

namespace WingroveAudio;

public class WingroveFilterHighPass : FilterApplicationBase
{
	[SerializeField]
	[AudioParameterName]
	private string m_filterParameterController;

	[SerializeField]
	private float m_filterHighCutParameterAtZero = 5000f;

	[SerializeField]
	private float m_filterHighCutParameterAtOne = 5000f;

	[SerializeField]
	private float m_resonanceAtZero = 1f;

	[SerializeField]
	private float m_resonanceAtOne = 1f;

	[SerializeField]
	private bool m_smoothStep = true;

	private int m_cachedParameterId;

	public override void UpdateFor(PooledAudioSource playingSource, int linkedObjectId)
	{
		if (m_cachedParameterId == 0)
		{
			m_cachedParameterId = WingroveRoot.Instance.GetParameterId(m_filterParameterController);
		}
		float t = Mathf.Clamp01(WingroveRoot.Instance.GetParameterForGameObject(m_cachedParameterId, linkedObjectId));
		float num = 0f;
		float num2 = 0f;
		if (m_smoothStep)
		{
			num = Mathf.SmoothStep(m_filterHighCutParameterAtZero, m_filterHighCutParameterAtOne, t);
			num2 = Mathf.SmoothStep(m_resonanceAtZero, m_resonanceAtOne, t);
		}
		else
		{
			num = Mathf.Lerp(m_filterHighCutParameterAtZero, m_filterHighCutParameterAtOne, t);
			num2 = Mathf.Lerp(m_resonanceAtZero, m_resonanceAtOne, t);
		}
		playingSource.SetHighPassFilter(num, num2);
	}
}
