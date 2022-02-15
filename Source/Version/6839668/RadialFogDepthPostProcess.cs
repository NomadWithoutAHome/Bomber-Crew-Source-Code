using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class RadialFogDepthPostProcess : MonoBehaviour
{
	[SerializeField]
	private Material m_combineMaterial;

	private RenderTexture m_skyboxRenderTexture;

	public void SetRenderTexture(RenderTexture skybox)
	{
		m_skyboxRenderTexture = skybox;
		Shader.SetGlobalTexture("RD_SkyBoxRT", m_skyboxRenderTexture);
	}

	public void SetFogColor(Color fogColor, float skyVis)
	{
		Shader.SetGlobalColor("RD_FogColor", fogColor);
		Shader.SetGlobalFloat("RD_SkyVisibility", skyVis);
	}

	public void SetDistances(float nearDist, float farDist, float skyNear, float skyFar)
	{
		Shader.SetGlobalFloat("RD_FogStart", nearDist);
		Shader.SetGlobalFloat("RD_FogEnd", farDist);
		Shader.SetGlobalFloat("RD_SkyFogStart", skyNear);
		Shader.SetGlobalFloat("RD_SkyFogEnd", skyFar);
	}

	[ImageEffectOpaque]
	public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		Shader.SetGlobalFloat("RD_Fov", GetComponent<Camera>().fieldOfView);
		Graphics.Blit(source, destination, m_combineMaterial);
	}
}
