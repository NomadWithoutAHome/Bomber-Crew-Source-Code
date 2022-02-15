using UnityEngine;

public class PulseAlphaAnimation : MonoBehaviour
{
	[SerializeField]
	private tk2dBaseSprite[] m_sprites;

	[SerializeField]
	private tk2dTextMesh[] m_textMeshes;

	[SerializeField]
	private float m_flashRate = 4f;

	[SerializeField]
	private bool m_useUnscaledTime;

	private Color[] m_spriteColors;

	private Color[] m_textMeshColors;

	private float m_alphaMod;

	private void OnEnable()
	{
		if (m_sprites.Length > 0)
		{
			m_spriteColors = new Color[m_sprites.Length];
			for (int i = 0; i < m_sprites.Length; i++)
			{
				ref Color reference = ref m_spriteColors[i];
				reference = m_sprites[i].color;
			}
		}
		if (m_textMeshes.Length > 0)
		{
			m_textMeshColors = new Color[m_textMeshes.Length];
			for (int j = 0; j < m_textMeshes.Length; j++)
			{
				ref Color reference2 = ref m_textMeshColors[j];
				reference2 = m_textMeshes[j].color;
			}
		}
	}

	private void Update()
	{
		if (m_useUnscaledTime)
		{
			m_alphaMod = 0.5f + Mathf.Sin(Time.unscaledTime * m_flashRate) * 0.5f * 0.7f;
		}
		else
		{
			m_alphaMod = 0.5f + Mathf.Sin(Time.time * m_flashRate) * 0.5f * 0.7f;
		}
		if (m_sprites.Length > 0)
		{
			for (int i = 0; i < m_sprites.Length; i++)
			{
				m_spriteColors[i].a = m_alphaMod;
				m_sprites[i].color = m_spriteColors[i];
			}
		}
		if (m_textMeshes.Length > 0)
		{
			for (int j = 0; j < m_textMeshes.Length; j++)
			{
				m_textMeshColors[j].a = m_alphaMod;
				m_textMeshes[j].color = m_textMeshColors[j];
			}
		}
	}
}
