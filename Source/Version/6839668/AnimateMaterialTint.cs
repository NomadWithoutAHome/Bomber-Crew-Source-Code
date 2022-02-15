using UnityEngine;

public class AnimateMaterialTint : MonoBehaviour
{
	[SerializeField]
	private string m_propertyName = "_TintColor";

	[SerializeField]
	private Renderer m_renderer;

	public float m_alphaMultiplier = 1f;

	private Color m_colorValue = Color.white;

	private float m_alphaValue;

	private float m_alphaMultiplierLastFrame = 1f;

	[SerializeField]
	private float m_alphaStartValue = 1f;

	private void OnEnable()
	{
		m_colorValue = m_renderer.sharedMaterial.GetColor(m_propertyName);
		if (m_alphaStartValue != 1f)
		{
			m_colorValue.a = m_alphaStartValue;
			m_renderer.sharedMaterial.SetColor(m_propertyName, m_colorValue);
		}
	}

	private void Update()
	{
		if (m_alphaMultiplier != m_alphaMultiplierLastFrame)
		{
			m_colorValue.a = m_alphaMultiplier;
			m_renderer.sharedMaterial.SetColor(m_propertyName, m_colorValue);
			m_alphaMultiplierLastFrame = m_alphaMultiplier;
		}
	}
}
