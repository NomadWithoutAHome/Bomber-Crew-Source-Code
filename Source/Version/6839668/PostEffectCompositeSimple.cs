using UnityEngine;

public class PostEffectCompositeSimple : MonoBehaviour
{
	[SerializeField]
	private Material m_combineMaterial;

	[SerializeField]
	private Camera m_overlayRenderTexture;

	public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		m_combineMaterial.SetTexture("_OverlayTex", m_overlayRenderTexture.targetTexture);
		Graphics.Blit(source, destination, m_combineMaterial);
	}
}
