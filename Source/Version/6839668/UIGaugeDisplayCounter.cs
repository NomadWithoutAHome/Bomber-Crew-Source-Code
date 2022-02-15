using UnityEngine;

public class UIGaugeDisplayCounter : UIGaugeDisplay
{
	[SerializeField]
	private tk2dTextMesh m_counterText;

	public override void DisplayValue(float value)
	{
		m_counterText.text = Mathf.RoundToInt(value).ToString();
	}
}
