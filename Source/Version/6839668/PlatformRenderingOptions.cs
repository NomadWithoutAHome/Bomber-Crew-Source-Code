using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class PlatformRenderingOptions : MonoBehaviour
{
	[SerializeField]
	private MonoBehaviour[] m_postProcessToDisableOnSwitch;

	[SerializeField]
	private MonoBehaviour[] m_postProcessToDisableOnSwitchHandheld;

	[SerializeField]
	private MonoBehaviour[] m_postProcessToDisableXboxOne;

	[SerializeField]
	private MonoBehaviour[] m_postProcessToDisablePS4;

	[SerializeField]
	private Camera m_camera;

	[SerializeField]
	private int m_switchResolutionOffsetForScene = -1;

	[SerializeField]
	private DepthOfField m_depthOfField;

	[SerializeField]
	private DepthOfField.BlurSampleCount m_dofQualitySwitch;

	[SerializeField]
	private DepthOfField.BlurSampleCount m_dofQualitySwitchHandheld;

	[SerializeField]
	private DepthOfField.BlurSampleCount m_dofQualityXboxOne;

	[SerializeField]
	private DepthOfField.BlurSampleCount m_dofQualityPS4;

	[SerializeField]
	private DepthOfField.BlurSampleCount m_dofQualityStandalone;

	[SerializeField]
	private DepthOfField.BlurSampleCount m_dofQualityStandaloneLow;

	private RenderTexture m_outlineTexture;

	private RenderTexture m_transparentTexture;

	private void Awake()
	{
		CreateRT();
		if (QualitySettings.names[QualitySettings.GetQualityLevel()] == "Low")
		{
			m_depthOfField.blurSampleCount = m_dofQualityStandaloneLow;
		}
	}

	private void OnDestroy()
	{
		m_outlineTexture.Release();
		if (Application.isEditor)
		{
			Object.Destroy(m_outlineTexture);
		}
		m_transparentTexture.Release();
		if (Application.isEditor)
		{
			Object.Destroy(m_transparentTexture);
		}
	}

	private void CreateRT()
	{
		m_outlineTexture = new RenderTexture(Screen.width, Screen.height, 24);
		m_outlineTexture.filterMode = FilterMode.Point;
		m_transparentTexture = new RenderTexture(Screen.width, Screen.height, 24);
	}

	public int GetWidth()
	{
		return Screen.width;
	}

	public int GetHeight()
	{
		return Screen.height;
	}

	public RenderTexture GetOutlineRenderTexture()
	{
		return m_outlineTexture;
	}

	public RenderTexture GetCopyTransparentRenderTexture()
	{
		return m_transparentTexture;
	}
}
