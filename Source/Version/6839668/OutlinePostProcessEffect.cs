using UnityEngine;

[RequireComponent(typeof(Camera))]
public class OutlinePostProcessEffect : MonoBehaviour
{
	[SerializeField]
	private Material m_outlinePostEffectMaterial;

	[SerializeField]
	private PostEffectComposite m_compositor;

	[SerializeField]
	private PlatformRenderingOptions m_platRenderer;

	private RenderTexture m_renderTexture;

	private Camera m_camera;

	private void Start()
	{
		m_outlinePostEffectMaterial = Object.Instantiate(m_outlinePostEffectMaterial);
		m_camera = GetComponent<Camera>();
		m_camera.enabled = true;
	}

	private void LateUpdate()
	{
		m_renderTexture = m_platRenderer.GetOutlineRenderTexture();
		m_compositor.SetRenderTexture(m_renderTexture);
		m_camera.targetTexture = m_renderTexture;
	}

	public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		Vector4 value = new Vector4(1f / (float)m_platRenderer.GetWidth(), 1f / (float)m_platRenderer.GetHeight());
		m_outlinePostEffectMaterial.SetVector("_UVStepWidth", value);
		Graphics.Blit(source, destination, m_outlinePostEffectMaterial);
	}
}
