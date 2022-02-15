using UnityEngine;

[ExecuteInEditMode]
public class SkyboxCamera : MonoBehaviour
{
	[SerializeField]
	private RadialFogDepthPostProcess m_fogPostProcess;

	private RenderTexture m_renderTexture;

	private void Start()
	{
		SetRT();
	}

	private void SetRT()
	{
		if (m_renderTexture != null)
		{
			m_renderTexture.Release();
			if (Application.isPlaying)
			{
				Object.Destroy(m_renderTexture);
			}
		}
		m_renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
		m_renderTexture.name = "SKYBOX_PP";
		GetComponent<Camera>().targetTexture = m_renderTexture;
		GetComponent<Camera>().enabled = true;
		m_fogPostProcess.SetRenderTexture(m_renderTexture);
	}

	public void LateUpdate()
	{
		base.transform.rotation = Camera.main.transform.rotation;
		GetComponent<Camera>().fieldOfView = Camera.main.fieldOfView;
		if (m_renderTexture.width != Screen.width || m_renderTexture.height != Screen.height)
		{
			SetRT();
		}
	}
}
