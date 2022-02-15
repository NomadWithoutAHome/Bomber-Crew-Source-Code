using BomberCrewCommon;
using UnityEngine;

public class PauseMenuControlsPage : MonoBehaviour
{
	[SerializeField]
	private PanelToggleButton m_invertYButton;

	[SerializeField]
	private SliderBar m_sliderTargeting;

	[SerializeField]
	private SliderBar m_sliderCamera;

	[SerializeField]
	private PanelToggleButton m_invertHScrollZoomed;

	[SerializeField]
	private PanelToggleButton m_showHints;

	[SerializeField]
	private PanelToggleButton m_allowSlowTime;

	[SerializeField]
	private PanelToggleButton m_allowVibration;

	[SerializeField]
	private PanelToggleButton m_allowMotionAiming;

	[SerializeField]
	private PanelToggleButton m_invertYMotionAiming;

	[SerializeField]
	private GameObject[] m_motionAimHierarchy;

	[SerializeField]
	private LayoutGrid m_layoutGrid;

	[SerializeField]
	private int m_noMotionAimGridRowPadding;

	[SerializeField]
	private tk2dSlicedSprite m_controlsPanel;

	private void Start()
	{
		m_invertYButton.OnClick += ToggleInvertY;
		m_invertYButton.SetState(Singleton<SystemDataContainer>.Instance.Get().GetInvert());
		m_sliderTargeting.SetValue(Singleton<SystemDataContainer>.Instance.Get().GetSensitivityTargeting());
		m_sliderCamera.SetValue(Singleton<SystemDataContainer>.Instance.Get().GetSensitivityCamera());
		m_invertHScrollZoomed.OnClick += ToggleHorizontalZoomed;
		m_invertHScrollZoomed.SetState(Singleton<SystemDataContainer>.Instance.Get().GetHorizontalZoomedFlip());
		m_showHints.OnClick += ToggleShowHints;
		m_showHints.SetState(!Singleton<SystemDataContainer>.Instance.Get().BlockHints());
		m_allowSlowTime.OnClick += ToggleAllowSlowTime;
		m_allowSlowTime.SetState(!Singleton<SystemDataContainer>.Instance.Get().BlockSlowDown());
		m_allowVibration.OnClick += ToggleVibration;
		m_allowVibration.SetState(Singleton<SystemDataContainer>.Instance.Get().AllowVibration());
		GameObject[] motionAimHierarchy = m_motionAimHierarchy;
		foreach (GameObject gameObject in motionAimHierarchy)
		{
			gameObject.SetActive(value: false);
		}
		m_layoutGrid.PaddingBetweenRows = m_noMotionAimGridRowPadding;
		m_layoutGrid.RepositionChildren();
	}

	private void Update()
	{
		Singleton<SystemDataContainer>.Instance.Get().SetSensitivityCamera(m_sliderCamera.GetValue());
		Singleton<SystemDataContainer>.Instance.Get().SetSensitivityTargeting(m_sliderTargeting.GetValue());
		Singleton<SystemDataContainer>.Instance.Get().SetHorizontalZoomedFlip(m_invertHScrollZoomed.GetState());
	}

	private void ToggleMotionAiming()
	{
		Singleton<SystemDataContainer>.Instance.Get().SetGyroAim(!Singleton<SystemDataContainer>.Instance.Get().UseGyroAim());
		m_allowMotionAiming.SetState(Singleton<SystemDataContainer>.Instance.Get().UseGyroAim());
		Singleton<SystemDataContainer>.Instance.Save();
	}

	private void ToggleInvertYMotionAiming()
	{
		Singleton<SystemDataContainer>.Instance.Get().SetInvertYMotionAiming(!Singleton<SystemDataContainer>.Instance.Get().GetInvertYMotionAiming());
		m_invertYMotionAiming.SetState(Singleton<SystemDataContainer>.Instance.Get().GetInvertYMotionAiming());
		Singleton<SystemDataContainer>.Instance.Save();
	}

	private void ToggleHorizontalZoomed()
	{
		Singleton<SystemDataContainer>.Instance.Get().SetHorizontalZoomedFlip(!Singleton<SystemDataContainer>.Instance.Get().GetHorizontalZoomedFlip());
		m_invertHScrollZoomed.SetState(Singleton<SystemDataContainer>.Instance.Get().GetHorizontalZoomedFlip());
		Singleton<SystemDataContainer>.Instance.Save();
	}

	private void ToggleInvertY()
	{
		Singleton<SystemDataContainer>.Instance.Get().SetInvert(!Singleton<SystemDataContainer>.Instance.Get().GetInvert());
		m_invertYButton.SetState(Singleton<SystemDataContainer>.Instance.Get().GetInvert());
		Singleton<SystemDataContainer>.Instance.Save();
	}

	private void ToggleVibration()
	{
		Singleton<SystemDataContainer>.Instance.Get().SetAllowVibration(!Singleton<SystemDataContainer>.Instance.Get().AllowVibration());
		m_allowVibration.SetState(Singleton<SystemDataContainer>.Instance.Get().AllowVibration());
		Singleton<SystemDataContainer>.Instance.Save();
	}

	private void ToggleShowHints()
	{
		Singleton<SystemDataContainer>.Instance.Get().SetHintsAllowed(Singleton<SystemDataContainer>.Instance.Get().BlockHints());
		m_showHints.SetState(!Singleton<SystemDataContainer>.Instance.Get().BlockHints());
	}

	private void ToggleAllowSlowTime()
	{
		Singleton<SystemDataContainer>.Instance.Get().SetSlowdownAllowed(Singleton<SystemDataContainer>.Instance.Get().BlockSlowDown());
		m_allowSlowTime.SetState(!Singleton<SystemDataContainer>.Instance.Get().BlockSlowDown());
	}
}
