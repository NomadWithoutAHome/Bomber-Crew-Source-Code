using UnityEngine;

public class EffectScalerHelperRateOverDistance : EffectScalerHelper
{
	[SerializeField]
	private AnimationCurve m_rateOverDistance;

	public override void SetEffectScale(float effectScale)
	{
		if (m_isSetup)
		{
			m_particleSystemEmission.rateOverDistance = m_rateOverDistance.Evaluate(effectScale);
		}
	}
}
