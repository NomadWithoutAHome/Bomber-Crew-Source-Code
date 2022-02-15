using UnityEngine;

public class AnimateTk2dTextMeshAlpha : MonoBehaviour
{
	[SerializeField]
	private tk2dTextMesh[] m_textMeshes;

	public float m_alphaMultiplier = 1f;

	private Color m_colorValue;

	private float m_alphaValue;

	private float m_alphaMultiplierLastFrame = 1f;

	[SerializeField]
	private float m_startAlphaMultiplier = 1f;

	private void Awake()
	{
		if (m_textMeshes.Length < 1)
		{
			m_textMeshes = GetComponentsInChildren<tk2dTextMesh>();
		}
		m_colorValue = m_textMeshes[0].color;
		m_alphaValue = m_colorValue.a;
		m_colorValue.a = m_startAlphaMultiplier;
		for (int i = 0; i < m_textMeshes.Length; i++)
		{
			m_textMeshes[i].color = m_colorValue;
		}
	}

	private void Update()
	{
		if (m_alphaMultiplier != m_alphaMultiplierLastFrame)
		{
			m_colorValue.a = m_alphaValue * m_alphaMultiplier;
			for (int i = 0; i < m_textMeshes.Length; i++)
			{
				m_textMeshes[i].color = m_colorValue;
				m_textMeshes[i].Commit();
			}
			m_alphaMultiplierLastFrame = m_alphaMultiplier;
		}
	}
}
