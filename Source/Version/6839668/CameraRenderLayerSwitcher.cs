using BomberCrewCommon;
using UnityEngine;

public class CameraRenderLayerSwitcher : MonoBehaviour
{
	[SerializeField]
	private Camera m_camera;

	[SerializeField]
	private LayerMask m_layerMaskLeft;

	[SerializeField]
	private LayerMask m_layerMaskRight;

	[SerializeField]
	private LayerMask m_layerMaskInterior;

	[SerializeField]
	private LayerMask m_layerMaskExterior;

	private bool m_rightSideIsShowing;

	private bool m_leftSideIsShowing;

	private BomberState m_bomberState;

	private void Start()
	{
		m_bomberState = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState();
	}

	private void Update()
	{
		if (Singleton<BomberCamera>.Instance != null)
		{
			ShowSide(Singleton<BomberCamera>.Instance.ShouldShowSides(), bomberIsFacingRight: true);
		}
		else
		{
			ShowSide(show: true, bomberIsFacingRight: true);
		}
	}

	public void ShowSide(bool show, bool bomberIsFacingRight)
	{
		bool flag = !bomberIsFacingRight || show;
		bool flag2 = bomberIsFacingRight || show;
		if (m_rightSideIsShowing != flag)
		{
			if (flag)
			{
				ShowLayers(m_layerMaskRight, show: true);
				ShowLayers(m_layerMaskInterior, show: false);
				ShowLayers(m_layerMaskExterior, show: true);
			}
			else
			{
				ShowLayers(m_layerMaskRight, show: false);
				ShowLayers(m_layerMaskInterior, show: true);
				ShowLayers(m_layerMaskExterior, show: false);
			}
			m_rightSideIsShowing = flag;
		}
		if (m_leftSideIsShowing != flag2)
		{
			if (flag2)
			{
				ShowLayers(m_layerMaskLeft, show: true);
				ShowLayers(m_layerMaskInterior, show: false);
				ShowLayers(m_layerMaskExterior, show: true);
			}
			else
			{
				ShowLayers(m_layerMaskLeft, show: false);
				ShowLayers(m_layerMaskInterior, show: true);
				ShowLayers(m_layerMaskExterior, show: false);
			}
			m_leftSideIsShowing = flag2;
		}
	}

	private void ShowLayers(LayerMask layerMask, bool show)
	{
		if (show)
		{
			m_camera.cullingMask |= layerMask;
		}
		else
		{
			m_camera.cullingMask &= ~(int)layerMask;
		}
	}
}
