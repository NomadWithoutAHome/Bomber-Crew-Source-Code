using UnityEngine;

public class UIGaugeDisplayRadial : UIGaugeDisplay
{
	[SerializeField]
	private UIInstrumentHelper.MinMax m_minMaxEulerAngles;

	[SerializeField]
	private Transform m_dialTransform;

	[SerializeField]
	private bool m_setRadialFill;

	[SerializeField]
	private tk2dRadialSprite m_radialFillSprite;

	[SerializeField]
	private bool m_setDefaultValue;

	[SerializeField]
	private float m_defaultFillValue = 0.25f;

	public void OnEnable()
	{
		if (m_setDefaultValue)
		{
			SetRadialFill(m_defaultFillValue);
		}
	}

	public override void DisplayValue(float value)
	{
		m_dialTransform.localEulerAngles = new Vector3(0f, 0f, Mathf.Lerp(m_minMaxEulerAngles.min, m_minMaxEulerAngles.max, value));
	}

	public void SetRadialFill(float fillValue)
	{
		m_radialFillSprite.SetValue(fillValue);
	}
}
