using System;
using System.Collections;
using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;
using WingroveAudio;

public class AirbaseNavigation : Singleton<AirbaseNavigation>
{
	[SerializeField]
	private AirbaseAreaScreen[] m_navigationAreas;

	[SerializeField]
	private GameObject m_tabPrefab;

	[SerializeField]
	private UISelectFinder m_selectAreaFinder;

	[SerializeField]
	private UISelectorMovementType m_autopressMovementType;

	[SerializeField]
	private UISelectFinder m_selectNoneFinder;

	[SerializeField]
	private LayoutGrid m_layoutGrid;

	[SerializeField]
	private GameObject m_controlPromptPrev;

	[SerializeField]
	private GameObject m_controlPromptNext;

	[SerializeField]
	private GameObject m_enableNavigationHierarchy;

	[SerializeField]
	private AirbaseAreaScreen m_debriefScreen;

	[SerializeField]
	private AirbaseAreaScreen m_overviewScreen;

	[SerializeField]
	private AirbasePersistentCrew[] m_crewAvatars;

	[SerializeField]
	private AirbaseCameraNode m_startNode;

	[SerializeField]
	private tk2dCamera m_uiCamera;

	[SerializeField]
	private AirbasePersistentCrew m_persistentCrew;

	[SerializeField]
	private AirbaseAreaScreen m_defaultScreen;

	[SerializeField]
	private bool m_autoRefreshCrewPhoto;

	private List<SelectableFilterButton> m_allFilterTabs = new List<SelectableFilterButton>();

	private bool m_impatienceDetected;

	private AirbaseAreaScreen m_onRouteTo;

	private AirbaseAreaScreen m_currentArea;

	private bool m_contentUpdateAllowed;

	private RenderTexture m_currentCrewPhoto;

	private bool m_crewPhotoRequiresRefresh;

	private bool m_savedCrewPhotoSinceLastRefresh;

	public event Action OnChangeArea;

	public AirbasePersistentCrew GetPersistentCrew()
	{
		return m_persistentCrew;
	}

	private void Awake()
	{
		AirbaseAreaScreen[] navigationAreas = m_navigationAreas;
		foreach (AirbaseAreaScreen na in navigationAreas)
		{
			bool flag = na.ShouldShow();
			if (Singleton<GameFlow>.Instance.IsDemoMode())
			{
				flag &= na.GetShowInDemo();
			}
			if (!flag)
			{
				continue;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(m_tabPrefab);
			gameObject.transform.parent = m_layoutGrid.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.GetComponent<AirbaseNavigationTab>().SetUp(na);
			SelectableFilterButton ft = gameObject.GetComponent<SelectableFilterButton>();
			m_allFilterTabs.Add(ft);
			ft.OnClick += delegate
			{
				foreach (SelectableFilterButton allFilterTab in m_allFilterTabs)
				{
					allFilterTab.SetSelected(allFilterTab == ft);
				}
				if (m_onRouteTo == na)
				{
					m_impatienceDetected = true;
				}
				else
				{
					SetSelectingArea(na);
				}
			};
		}
		m_layoutGrid.RepositionChildren();
		m_controlPromptPrev.transform.localPosition = m_layoutGrid.transform.GetChild(0).localPosition;
		m_controlPromptNext.transform.localPosition = m_layoutGrid.transform.GetChild(m_layoutGrid.transform.childCount - 1).localPosition;
		GoToDefaultScreen(jump: true);
		Singleton<CrewContainer>.Instance.OnCrewmanRemoved += SetCrewPhotoRequiresRefresh;
		Singleton<CrewContainer>.Instance.OnNewCrewman += SetCrewPhotoRequiresRefresh;
	}

	private void Start()
	{
		m_crewPhotoRequiresRefresh = true;
		RefreshCrewPhoto();
		if (Singleton<SystemDataContainer>.Instance.Get().GetMusic())
		{
			WingroveRoot.Instance.PostEvent("MUSIC_UNMUTE");
		}
		else
		{
			WingroveRoot.Instance.PostEvent("MUSIC_MUTE");
		}
	}

	public SelectableFilterButton GetButtonFor(AirbaseAreaScreen aas)
	{
		int num = 0;
		AirbaseAreaScreen[] navigationAreas = m_navigationAreas;
		foreach (AirbaseAreaScreen airbaseAreaScreen in navigationAreas)
		{
			if (airbaseAreaScreen.GetShowInDemo() || !Singleton<GameFlow>.Instance.IsDemoMode())
			{
				if (airbaseAreaScreen == aas)
				{
					return m_allFilterTabs[num];
				}
				num++;
			}
		}
		return null;
	}

	public void RefreshCrewPhoto()
	{
		if (m_crewPhotoRequiresRefresh && m_autoRefreshCrewPhoto)
		{
			m_crewPhotoRequiresRefresh = false;
			int currentCrewCount = Singleton<CrewContainer>.Instance.GetCurrentCrewCount();
			Crewman[] array = new Crewman[currentCrewCount];
			for (int i = 0; i < currentCrewCount; i++)
			{
				array[i] = Singleton<CrewContainer>.Instance.GetCrewman(i);
			}
			m_currentCrewPhoto = Singleton<CrewAndBomberPhotoBooth>.Instance.RenderForCrewAndBomber(array, Singleton<BomberContainer>.Instance.GetCurrentConfig());
			m_savedCrewPhotoSinceLastRefresh = false;
		}
	}

	public void SetCrewPhotoRequiresRefresh()
	{
		m_crewPhotoRequiresRefresh = true;
	}

	private IEnumerator GenerateAndSaveCrewPhoto()
	{
		RefreshCrewPhoto();
		while (Singleton<CrewAndBomberPhotoBooth>.Instance.IsProcessing())
		{
			yield return null;
		}
		SaveCrewPhoto(instant: true);
	}

	public void SaveCrewPhoto(bool instant)
	{
		if (!m_autoRefreshCrewPhoto)
		{
			return;
		}
		if (instant)
		{
			if (!m_savedCrewPhotoSinceLastRefresh)
			{
				m_savedCrewPhotoSinceLastRefresh = true;
				RenderTexture.active = m_currentCrewPhoto;
				Texture2D texture2D = new Texture2D(m_currentCrewPhoto.width, m_currentCrewPhoto.height, TextureFormat.RGB24, mipmap: false);
				texture2D.ReadPixels(new Rect(0f, 0f, m_currentCrewPhoto.width, m_currentCrewPhoto.height), 0, 0);
				texture2D.Apply();
				Singleton<SaveDataContainer>.Instance.SaveCrewPhoto(texture2D);
			}
		}
		else
		{
			StartCoroutine(GenerateAndSaveCrewPhoto());
		}
	}

	public void GoToDefaultScreen(bool jump)
	{
		int num = 0;
		int num2 = 0;
		AirbaseAreaScreen[] navigationAreas = m_navigationAreas;
		foreach (AirbaseAreaScreen airbaseAreaScreen in navigationAreas)
		{
			if (airbaseAreaScreen == m_defaultScreen)
			{
				num2 = num;
			}
			num++;
		}
		num = 0;
		foreach (SelectableFilterButton allFilterTab in m_allFilterTabs)
		{
			allFilterTab.SetSelected(num == num2);
			num++;
		}
		m_impatienceDetected = false;
		SetSelectingArea(m_defaultScreen, jump);
	}

	public void SetSelectingArea(AirbaseAreaScreen area, bool force = false)
	{
		if (!(area != m_currentArea) && !force)
		{
			return;
		}
		StopAllCoroutines();
		Singleton<UIScreenManager>.Instance.ShowScreen(null, showNavBarButtons: true);
		Singleton<MainActionButtonMonitor>.Instance.RemoveListener(TravellingToLocationHandler, invalidateCurrentPress: false);
		if (this.OnChangeArea != null)
		{
			this.OnChangeArea();
		}
		m_onRouteTo = area;
		m_currentArea = null;
		Singleton<UISelector>.Instance.SetFinder(m_selectAreaFinder);
		if (area == null)
		{
			foreach (SelectableFilterButton allFilterTab in m_allFilterTabs)
			{
				if (allFilterTab.IsSelected())
				{
					Singleton<UISelector>.Instance.ForcePointAt(allFilterTab.GetUIItem());
				}
				allFilterTab.SetSelected(selected: false);
			}
			Singleton<AirbaseCameraController>.Instance.MoveCameraToLocation(m_startNode);
		}
		else
		{
			StartCoroutine(MoveToScreenAndActivate(area));
		}
	}

	private IEnumerator MoveToScreenAndActivate(AirbaseAreaScreen area)
	{
		m_impatienceDetected = false;
		Singleton<AirbaseCameraController>.Instance.MoveCameraToLocation(area.GetAssociatedCameraNode());
		bool hasAddedImpatienceDetector = false;
		while (Singleton<AirbaseCameraController>.Instance.IsMoving())
		{
			if (m_impatienceDetected)
			{
				AirbasePersistentCrew[] crewAvatars = m_crewAvatars;
				foreach (AirbasePersistentCrew airbasePersistentCrew in crewAvatars)
				{
					airbasePersistentCrew.SetIsImpatient();
				}
				Singleton<AirbaseCameraController>.Instance.MoveCameraToLocationInstant(area.GetAssociatedCameraNode());
			}
			yield return null;
			if (!hasAddedImpatienceDetector)
			{
				Singleton<MainActionButtonMonitor>.Instance.AddListener(TravellingToLocationHandler);
				hasAddedImpatienceDetector = true;
			}
		}
		Singleton<MainActionButtonMonitor>.Instance.RemoveListener(TravellingToLocationHandler, invalidateCurrentPress: false);
		m_onRouteTo = null;
		m_currentArea = area;
		m_currentArea.SetDefaultSelection();
		Singleton<UIScreenManager>.Instance.ShowScreen(area.GetUIScreen().name, showNavBarButtons: true);
	}

	private bool TravellingToLocationHandler(MainActionButtonMonitor.ButtonPress bp)
	{
		if (bp == MainActionButtonMonitor.ButtonPress.Confirm)
		{
			m_impatienceDetected = true;
			return true;
		}
		return false;
	}

	public void GoToDebrief()
	{
		SetSelectingArea(m_debriefScreen);
	}

	public void SetContentUpdateAllowed()
	{
		m_contentUpdateAllowed = true;
	}

	public void GoToNormal()
	{
		AirbaseMainMenuButton.SetPauseBlocked(blocked: false);
		m_persistentCrew.InitialisePersistentCrew();
		SetSelectingArea(m_overviewScreen);
		m_contentUpdateAllowed = true;
	}

	public void Refresh()
	{
		foreach (SelectableFilterButton allFilterTab in m_allFilterTabs)
		{
			allFilterTab.Refresh();
		}
	}

	private void Update()
	{
		if (m_contentUpdateAllowed)
		{
			Singleton<DLCManager>.Instance.DoDLCReloadCheck();
		}
	}
}
