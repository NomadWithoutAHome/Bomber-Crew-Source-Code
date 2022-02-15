using UnityEngine;

public class CameraFader : MonoBehaviour
{
	[SerializeField]
	private float m_opacity;

	[SerializeField]
	private Material m_material;

	[SerializeField]
	private string m_colorString;

	private RenderTexture m_rt;

	private Camera m_camera;

	private void OnEnable()
	{
		m_camera = base.gameObject.GetComponent<Camera>();
	}

	private void OnPreRender()
	{
		if (m_rt == null || m_rt.width != Screen.width || m_rt.height != Screen.height)
		{
			m_rt = new RenderTexture(Screen.width, Screen.height, 24);
			m_rt.Create();
			m_rt.DiscardContents(discardColor: true, discardDepth: true);
		}
		m_material.color = new Color(1f, 1f, 1f, m_opacity);
		if (!string.IsNullOrEmpty(m_colorString))
		{
			m_material.SetColor(m_colorString, new Color(1f, 1f, 1f, m_opacity));
		}
		m_camera.targetTexture = m_rt;
		Graphics.SetRenderTarget(m_rt);
		GL.Clear(clearDepth: true, clearColor: true, new Color(0f, 0f, 0f, 0f));
	}

	private void OnPostRender()
	{
		m_camera.targetTexture = null;
		Graphics.SetRenderTarget(null);
		m_rt.MarkRestoreExpected();
		Graphics.Blit(m_rt, m_material);
	}
}
