using BomberCrewCommon;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class TransparentPostProcessEffect : MonoBehaviour
{
	[SerializeField]
	private Material m_material;

	[SerializeField]
	private CopyToTransparentBuffer m_transparentBuffer;

	[SerializeField]
	private float m_alphaMin = 0.75f;

	[SerializeField]
	private float m_alphaMax = 1f;

	[SerializeField]
	private float m_speed = 3f;

	[SerializeField]
	private bool m_useShouldBeOpen;

	[SerializeField]
	private float m_alphaOnOpen = 0.25f;

	private float m_t;

	private void Start()
	{
		m_material = Object.Instantiate(m_material);
	}

	private void Update()
	{
		if (!m_useShouldBeOpen)
		{
			m_t += Time.deltaTime;
			float t = Mathf.Sin(m_speed * m_t) * 0.5f + 0.5f;
			float a = Mathf.Lerp(m_alphaMin, m_alphaMax, t);
			m_material.SetColor("_Tint", new Color(1f, 1f, 1f, a));
			return;
		}
		float num = ((!Singleton<BomberCamera>.Instance.ShouldShowSides()) ? m_alphaOnOpen : 1f);
		float num2 = num - m_t;
		m_t += num2 * Mathf.Clamp01(Time.deltaTime * 30f);
		if (m_t > 0.95f)
		{
			m_t = 1f;
		}
		m_material.SetColor("_Tint", new Color(1f, 1f, 1f, m_t));
	}

	public float GetCurrentT()
	{
		return m_t;
	}

	public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		m_material.SetTexture("_BackgroundTexture", m_transparentBuffer.GetLastMainFrame());
		Graphics.Blit(source, destination, m_material);
	}
}
