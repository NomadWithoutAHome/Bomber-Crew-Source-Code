using UnityEngine;

namespace WingroveAudio;

public class SimpleVolumeParameter : ParameterModifierBase
{
	[SerializeField]
	[AudioParameterName]
	private string m_volumeParameter;

	[SerializeField]
	[FaderInterface(true)]
	private float m_maxGainReduction = 1f;

	[SerializeField]
	private bool m_smooth;

	[SerializeField]
	private bool m_logarithmicFade;

	[SerializeField]
	private bool m_parameterHighMeansReduceVolume;

	private int m_cachedParameterId;

	public override float GetVolumeMultiplier(int linkedObjectId)
	{
		if (m_cachedParameterId == 0)
		{
			m_cachedParameterId = WingroveRoot.Instance.GetParameterId(m_volumeParameter);
		}
		float num = WingroveRoot.Instance.GetParameterForGameObject(m_cachedParameterId, linkedObjectId);
		if (m_parameterHighMeansReduceVolume)
		{
			num = 1f - num;
		}
		num = Mathf.Clamp01(num);
		if (m_logarithmicFade)
		{
			num = Mathf.Pow(num, 2f);
		}
		if (!m_smooth)
		{
			return Mathf.Lerp(m_maxGainReduction, 1f, num);
		}
		return Mathf.SmoothStep(m_maxGainReduction, 1f, num);
	}
}
