using UnityEngine;

public class AnimateDetailAlpha : MonoBehaviour
{
	[SerializeField]
	private float m_alpha = 1f;

	[SerializeField]
	private tk2dBaseSprite[] m_sprites;

	[SerializeField]
	private tk2dTextMesh[] m_textMeshes;

	[SerializeField]
	private Renderer[] m_tintRenderers;

	private float m_detailAlpha = 1f;

	private float m_startAlpha;

	private float m_alphaLastFrame;

	private void Awake()
	{
		m_startAlpha = m_alpha;
	}

	private void OnEnable()
	{
		m_alpha = m_startAlpha;
		m_alphaLastFrame = -999f;
		Update();
	}

	private void Update()
	{
		if (m_alpha * m_detailAlpha != m_alphaLastFrame)
		{
			for (int i = 0; i < m_sprites.Length; i++)
			{
				if (m_sprites[i] != null && m_sprites[i].color.a != m_alpha * m_detailAlpha)
				{
					Color color = m_sprites[i].color;
					color.a = m_alpha * m_detailAlpha;
					m_sprites[i].color = color;
				}
			}
			for (int j = 0; j < m_textMeshes.Length; j++)
			{
				if (m_textMeshes[j] != null)
				{
					Color color2 = m_textMeshes[j].color;
					color2.a = m_alpha * m_detailAlpha;
					m_textMeshes[j].color = color2;
				}
			}
			for (int k = 0; k < m_tintRenderers.Length; k++)
			{
				if (m_tintRenderers[k] != null)
				{
					Color color3 = m_tintRenderers[k].sharedMaterial.GetColor("_Tint");
					color3.a = m_alpha * m_detailAlpha;
					m_tintRenderers[k].material.SetColor("_Tint", color3);
				}
			}
		}
		m_alphaLastFrame = m_alpha * m_detailAlpha;
	}

	public void SetDetailAlpha(float alpha)
	{
		m_detailAlpha = alpha;
		Update();
	}

	public void SetAlpha(float alpha)
	{
		m_alpha = alpha;
		Update();
	}
}
