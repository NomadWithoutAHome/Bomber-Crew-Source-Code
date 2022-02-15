using UnityEngine;

public class TextureRenderCamera : MonoBehaviour
{
	[SerializeField]
	private Camera m_camera;

	[SerializeField]
	private bool m_generateRenderTexture;

	[SerializeField]
	private Vector2 m_generateSize;

	[SerializeField]
	private Vector2 m_generateSizeSwitch;

	[SerializeField]
	private bool m_noShadowsOnSwitch;

	private int m_registeredUsers;

	private RenderTexture m_renderTexture;

	private ShadowResolution m_prevShadowQuality;

	private void Awake()
	{
		m_camera.enabled = false;
		if (m_generateRenderTexture)
		{
			m_renderTexture = new RenderTexture((int)m_generateSize.x, (int)m_generateSize.y, 24);
			m_camera.targetTexture = m_renderTexture;
		}
		else
		{
			m_renderTexture = m_camera.targetTexture;
		}
	}

	private void OnDestroy()
	{
		if (m_generateRenderTexture && m_renderTexture != null)
		{
			m_renderTexture.Release();
		}
	}

	private void Start()
	{
		if (!m_generateRenderTexture)
		{
			m_renderTexture = m_camera.targetTexture;
		}
	}

	public RenderTexture RegisterUse()
	{
		m_camera.enabled = true;
		m_registeredUsers++;
		return m_renderTexture;
	}

	public void DeregisterUse()
	{
		m_registeredUsers--;
		if (m_registeredUsers == 0 && m_camera != null)
		{
			m_camera.enabled = false;
		}
	}
}
