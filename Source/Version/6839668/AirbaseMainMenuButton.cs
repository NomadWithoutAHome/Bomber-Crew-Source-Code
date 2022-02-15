using BomberCrewCommon;
using UnityEngine;

public class AirbaseMainMenuButton : MonoBehaviour
{
	[SerializeField]
	private tk2dUIItem m_button;

	private static bool m_blockPause;

	private void Awake()
	{
		m_button.OnClick += ClickMainMenu;
		Singleton<MainActionButtonMonitor>.Instance.AddListener(PauseListener);
	}

	public static void SetPauseBlocked(bool blocked)
	{
		m_blockPause = blocked;
	}

	private void OnDestroy()
	{
		if (Singleton<MainActionButtonMonitor>.Instance != null)
		{
			Singleton<MainActionButtonMonitor>.Instance.RemoveListener(PauseListener, invalidateCurrentPress: false);
		}
	}

	private bool PauseListener(MainActionButtonMonitor.ButtonPress bp)
	{
		if (bp == MainActionButtonMonitor.ButtonPress.Start)
		{
			if (!m_blockPause)
			{
				Singleton<AirbaseNavigation>.Instance.RefreshCrewPhoto();
				Singleton<UIPopupManager>.Instance.DisplayPopup(Singleton<GameFlow>.Instance.GetPauseMenuPrefab());
			}
			return true;
		}
		return false;
	}

	private void ClickMainMenu()
	{
		if (!m_blockPause)
		{
			Singleton<AirbaseNavigation>.Instance.RefreshCrewPhoto();
			Singleton<UIPopupManager>.Instance.DisplayPopup(Singleton<GameFlow>.Instance.GetPauseMenuPrefab());
		}
	}
}
