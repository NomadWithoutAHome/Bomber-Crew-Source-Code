using UnityEngine;

public class UIGaugeDisplayLinear : UIGaugeDisplay
{
	[SerializeField]
	private UIInstrumentHelper.MinMax m_minMaxPositionY;

	[SerializeField]
	private Transform m_dialTransform;

	public override void DisplayValue(float value)
	{
		m_dialTransform.localPosition = new Vector3(0f, Mathf.Lerp(m_minMaxPositionY.min, m_minMaxPositionY.max, value), 0f);
	}
}
