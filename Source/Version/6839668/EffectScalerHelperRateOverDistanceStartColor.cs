using UnityEngine;

public class EffectScalerHelperRateOverDistanceStartColor : EffectScalerHelper
{
	[SerializeField]
	private AnimationCurve m_rateOverDistance;

	[SerializeField]
	private Gradient m_startColor;

	public override void SetEffectScale(float effectScale)
	{
		if (m_isSetup)
		{
			m_particleSystemEmission.rateOverDistance = m_rateOverDistance.Evaluate(effectScale);
			m_particleSystemMain.startColor = m_startColor.Evaluate(effectScale);
		}
	}
}
