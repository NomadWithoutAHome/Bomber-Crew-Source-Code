using BomberCrewCommon;
using UnityEngine;

public class SkyboxDayNight : Singleton<SkyboxDayNight>
{
	[SerializeField]
	private DayNightCycle m_dayNightCycle;

	[SerializeField]
	private Animation m_dayNightAnim;

	[SerializeField]
	private Renderer m_skySphereRenderer;

	private Texture2D m_skySphereTexture;

	[SerializeField]
	private Renderer m_cloudsRenderer;

	private Color m_cloudsCol = Color.white;

	private Color m_ambientColor = Color.white;

	[SerializeField]
	private RadialFogDepthPostProcess m_fogPostProcess;

	[SerializeField]
	private Light m_sunLight;

	private void Start()
	{
		m_skySphereTexture = (Texture2D)m_skySphereRenderer.material.mainTexture;
		m_dayNightAnim.Play();
		m_dayNightAnim[m_dayNightAnim.clip.name].speed = 0f;
	}

	public Color GetAmbientColor()
	{
		return m_ambientColor;
	}

	private void Update()
	{
		m_dayNightAnim[m_dayNightAnim.clip.name].normalizedTime = m_dayNightCycle.GetDayNightProgress();
		m_cloudsCol = m_skySphereTexture.GetPixel(Mathf.RoundToInt((float)m_skySphereTexture.width * m_skySphereRenderer.material.mainTextureOffset.x), 13);
		m_cloudsRenderer.material.SetColor("_Tint", m_cloudsCol);
		m_ambientColor = m_skySphereTexture.GetPixel(Mathf.RoundToInt((float)m_skySphereTexture.width * m_skySphereRenderer.material.mainTextureOffset.x), 9);
		RenderSettings.ambientLight = m_ambientColor * 0.85f;
	}

	public Color GetFogColor()
	{
		return m_cloudsCol;
	}
}
