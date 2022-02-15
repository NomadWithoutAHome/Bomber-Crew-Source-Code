using UnityEngine;

public class EffectScalerHelperRate : EffectScalerHelper
{
	[SerializeField]
	private AnimationCurve m_rate;

	public override void SetEffectScale(float effectScale)
	{
		if (m_isSetup)
		{
			m_particleSystemEmission.rateOverTime = m_rate.Evaluate(effectScale);
		}
	}
}
