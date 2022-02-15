using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class CopyToTransparentBuffer : MonoBehaviour
{
	[SerializeField]
	private PlatformRenderingOptions m_platRenderer;

	public RenderTexture GetLastMainFrame()
	{
		return m_platRenderer.GetCopyTransparentRenderTexture();
	}

	public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		Graphics.Blit(source, m_platRenderer.GetCopyTransparentRenderTexture());
		Graphics.Blit(source, destination);
	}
}
