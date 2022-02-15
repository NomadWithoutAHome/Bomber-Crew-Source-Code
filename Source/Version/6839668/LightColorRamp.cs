using UnityEngine;

public class LightColorRamp : MonoBehaviour
{
	[SerializeField]
	private Light m_light;

	[SerializeField]
	private Texture2D m_rampTexture;

	[SerializeField]
	private int m_yIndex;

	private float m_time;

	public void SetTime(float time)
	{
		m_time = time;
		SetLightColor();
	}

	private void SetLightColor()
	{
		int x = Mathf.RoundToInt((float)m_rampTexture.width * m_time);
		Color pixel = m_rampTexture.GetPixel(x, m_yIndex);
		m_light.color = pixel;
	}
}
