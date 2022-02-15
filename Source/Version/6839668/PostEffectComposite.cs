using UnityEngine;

public class PostEffectComposite : MonoBehaviour
{
	[SerializeField]
	private Material m_combineMaterial;

	private RenderTexture m_overlayRenderTexture;

	public void SetRenderTexture(RenderTexture overlayRenderTexture)
	{
		m_overlayRenderTexture = overlayRenderTexture;
	}

	public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		m_combineMaterial.SetTexture("_OverlayTex", m_overlayRenderTexture);
		Graphics.Blit(source, destination, m_combineMaterial);
	}
}
