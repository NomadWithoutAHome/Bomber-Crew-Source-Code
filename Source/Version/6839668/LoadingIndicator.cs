using UnityEngine;

public class LoadingIndicator : MonoBehaviour
{
	[SerializeField]
	private Renderer m_faderRenderer;

	[SerializeField]
	private Renderer m_faderRendererZh;

	[SerializeField]
	private GameObject m_faderEnabled;

	[SerializeField]
	private Renderer[] m_otherFaderRenderers;

	private bool m_isLoading;

	private float m_transparency;

	public bool IsFullyFaded()
	{
		return m_transparency == 1f;
	}

	public bool IsFullyInvisible()
	{
		return m_transparency == 0f && !m_faderEnabled.activeInHierarchy;
	}

	public void SetIsLoading(bool loading)
	{
		m_isLoading = loading;
	}

	private void LateUpdate()
	{
		if (m_isLoading)
		{
			m_transparency += Time.unscaledDeltaTime * 4f;
		}
		else
		{
			m_transparency -= Time.unscaledDeltaTime * 2f;
		}
		m_transparency = Mathf.Clamp01(m_transparency);
		m_faderRenderer.material.SetColor("_Tint", new Color(1f, 1f, 1f, m_transparency));
		m_faderRendererZh.material.SetColor("_Tint", new Color(1f, 1f, 1f, m_transparency));
		Renderer[] otherFaderRenderers = m_otherFaderRenderers;
		foreach (Renderer renderer in otherFaderRenderers)
		{
			renderer.material.SetColor("_Tint", new Color(1f, 1f, 1f, m_transparency));
		}
		m_faderEnabled.SetActive(m_transparency != 0f);
	}
}
