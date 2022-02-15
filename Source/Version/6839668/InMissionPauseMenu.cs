using BomberCrewCommon;
using UnityEngine;

public class InMissionPauseMenu : MonoBehaviour
{
	[SerializeField]
	private SelectableFilterButton[] m_pageSelectButtons;

	[SerializeField]
	private GameObject[] m_pages;

	[SerializeField]
	private tk2dUIItem m_closeButton;

	[SerializeField]
	private LayoutGrid m_tabLayoutGrid;

	[SerializeField]
	private bool[] m_pagesEnabledInAirbase;

	[SerializeField]
	private PauseMenuHowToPlayPage m_manualPage;

	private UISelectFinder m_oldFinder;

	private int m_startingPage;

	public void SetShowManual(string manualRef)
	{
		m_startingPage = 3;
		m_manualPage.SetStartPage(manualRef);
	}

	private void Start()
	{
		m_closeButton.OnClick += Close;
		if (Singleton<MissionCoordinator>.Instance == null)
		{
			for (int i = 0; i < m_pageSelectButtons.Length; i++)
			{
				if (!m_pagesEnabledInAirbase[i])
				{
					m_pageSelectButtons[i].gameObject.SetActive(value: false);
				}
			}
		}
		m_tabLayoutGrid.RepositionChildren();
		for (int j = 0; j < m_pageSelectButtons.Length; j++)
		{
			int index = j;
			m_pageSelectButtons[j].OnClick += delegate
			{
				SelectPage(index);
			};
		}
		if (Singleton<ContextControl>.Instance != null)
		{
			Singleton<ContextControl>.Instance.RefreshCameraSettingsForMenu();
		}
		SelectPage(m_startingPage);
	}

	private void Close()
	{
		if (Singleton<ContextControl>.Instance != null)
		{
			Singleton<ContextControl>.Instance.RefreshCameraSettingsForTargeting();
		}
		Singleton<UIPopupManager>.Instance.DismissCurrentPopup();
		Singleton<SystemDataContainer>.Instance.Save();
	}

	private void SelectPage(int pageIndex)
	{
		for (int i = 0; i < m_pageSelectButtons.Length; i++)
		{
			if (i == pageIndex)
			{
				m_pages[i].SetActive(value: true);
				m_pageSelectButtons[i].SetSelected(selected: true);
			}
			else
			{
				m_pages[i].SetActive(value: false);
				m_pageSelectButtons[i].SetSelected(selected: false);
			}
		}
	}

	private bool PauseListener(MainActionButtonMonitor.ButtonPress bp)
	{
		switch (bp)
		{
		case MainActionButtonMonitor.ButtonPress.Start:
			Close();
			return true;
		case MainActionButtonMonitor.ButtonPress.Back:
			Close();
			return true;
		case MainActionButtonMonitor.ButtonPress.LeftStart:
			Close();
			return true;
		default:
			return true;
		}
	}

	private void OnEnable()
	{
		Singleton<GameFlow>.Instance.SetPaused(paused: true);
		m_oldFinder = Singleton<UISelector>.Instance.GetCurrentFinder();
		Singleton<MainActionButtonMonitor>.Instance.AddListener(PauseListener);
	}

	private void OnDisable()
	{
		Singleton<GameFlow>.Instance.SetPaused(paused: false);
		Singleton<UISelector>.Instance.SetFinder(m_oldFinder);
		Singleton<MainActionButtonMonitor>.Instance.RemoveListener(PauseListener, invalidateCurrentPress: true);
	}
}
