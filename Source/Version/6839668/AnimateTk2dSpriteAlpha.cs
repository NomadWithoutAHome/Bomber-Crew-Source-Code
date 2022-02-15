using UnityEngine;

public class AnimateTk2dSpriteAlpha : MonoBehaviour
{
	[SerializeField]
	private tk2dBaseSprite[] m_uiSprites;

	public float m_alphaMultiplier = 1f;

	private Color m_colorValue;

	private float[] m_alphaValues;

	[SerializeField]
	private bool m_startWithZeroAlpha;

	private float m_alphaMultiplierLastFrame = 1f;

	[SerializeField]
	private float m_startAlphaMultiplier = 1f;

	private void Awake()
	{
		if (m_uiSprites.Length < 1)
		{
			m_uiSprites = GetComponentsInChildren<tk2dBaseSprite>();
		}
		m_alphaValues = new float[m_uiSprites.Length];
		for (int i = 0; i < m_uiSprites.Length; i++)
		{
			m_alphaValues[i] = m_uiSprites[i].color.a;
			if (m_startWithZeroAlpha)
			{
				m_uiSprites[i].color = new Color(m_uiSprites[i].color.r, m_uiSprites[i].color.g, m_uiSprites[i].color.b, 0f);
			}
		}
	}

	private void Update()
	{
		if (m_alphaMultiplier != m_alphaMultiplierLastFrame)
		{
			for (int i = 0; i < m_uiSprites.Length; i++)
			{
				m_colorValue = m_uiSprites[i].color;
				m_colorValue.a = m_alphaValues[i] * m_alphaMultiplier;
				m_uiSprites[i].color = m_colorValue;
			}
			m_alphaMultiplierLastFrame = m_alphaMultiplier;
		}
	}
}
