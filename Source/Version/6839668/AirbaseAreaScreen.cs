using System;
using BomberCrewCommon;
using UnityEngine;

public class AirbaseAreaScreen : MonoBehaviour
{
	[SerializeField]
	private AirbaseCameraNode m_cameraLocation;

	[SerializeField]
	[NamedText]
	private string m_namedTextReference;

	[SerializeField]
	private UIScreen m_uiScreen;

	[SerializeField]
	private bool m_showInDemo;

	[SerializeField]
	private bool m_showOnConsole;

	[SerializeField]
	private bool m_hideIfNoDLC;

	[SerializeField]
	private UISelectFinder m_defaultFinder;

	public event Action<bool> OnAcceptButton;

	public event Action OnBackButton;

	public event Action<bool> OnAction1;

	public event Action<bool> OnAction2;

	public event Action<bool> OnAction3;

	private void OnEnable()
	{
		int currentCrewCount = Singleton<CrewContainer>.Instance.GetCurrentCrewCount();
		for (int i = 0; i < currentCrewCount; i++)
		{
			Singleton<CrewContainer>.Instance.GetCrewman(i).GetPortraitPictureTexture();
		}
	}

	private void OnDisable()
	{
		Singleton<MainActionButtonMonitor>.Instance.RemoveListener(ScreenHandlers, invalidateCurrentPress: false);
	}

	public void SetDefaultSelection()
	{
		Singleton<UISelector>.Instance.SetFinder(m_defaultFinder);
		Singleton<MainActionButtonMonitor>.Instance.AddListener(ScreenHandlers);
	}

	private bool ScreenHandlers(MainActionButtonMonitor.ButtonPress bp)
	{
		switch (bp)
		{
		case MainActionButtonMonitor.ButtonPress.Back:
			if (this.OnBackButton != null)
			{
				this.OnBackButton();
			}
			return true;
		case MainActionButtonMonitor.ButtonPress.Confirm:
			if (this.OnAcceptButton != null)
			{
				this.OnAcceptButton(obj: false);
			}
			return true;
		case MainActionButtonMonitor.ButtonPress.ConfirmDown:
			if (this.OnAcceptButton != null)
			{
				this.OnAcceptButton(obj: true);
			}
			return true;
		case MainActionButtonMonitor.ButtonPress.LeftAction:
			if (this.OnAction1 != null)
			{
				this.OnAction1(obj: false);
			}
			return true;
		case MainActionButtonMonitor.ButtonPress.LeftActionDown:
			if (this.OnAction1 != null)
			{
				this.OnAction1(obj: true);
			}
			return true;
		case MainActionButtonMonitor.ButtonPress.TopAction:
			if (this.OnAction2 != null)
			{
				this.OnAction2(obj: false);
			}
			return true;
		case MainActionButtonMonitor.ButtonPress.TopActionDown:
			if (this.OnAction2 != null)
			{
				this.OnAction2(obj: true);
			}
			return true;
		case MainActionButtonMonitor.ButtonPress.LeftStart:
			if (this.OnAction3 != null)
			{
				this.OnAction3(obj: false);
			}
			return true;
		default:
			return false;
		}
	}

	public bool GetShowInDemo()
	{
		return m_showInDemo;
	}

	public bool ShouldShow()
	{
		return true;
	}

	public string GetNamedTextReference()
	{
		return m_namedTextReference;
	}

	public AirbaseCameraNode GetAssociatedCameraNode()
	{
		return m_cameraLocation;
	}

	public UIScreen GetUIScreen()
	{
		return m_uiScreen;
	}
}
