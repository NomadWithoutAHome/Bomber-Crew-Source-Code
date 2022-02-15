using BomberCrewCommon;
using UnityEngine;

public class NavigatorStationPanel : StationPanel
{
	private BomberSystems m_bomberSystems;

	private BomberState m_bomber;

	[SerializeField]
	private tk2dRadialSprite m_navigationMarkerProgress;

	[SerializeField]
	private tk2dUIItem m_directionMarkerButton;

	[SerializeField]
	private tk2dUIItem m_clearMarkerButton;

	[SerializeField]
	private PanelToggleButton m_zoomMapToggleButton;

	[SerializeField]
	private GameObject m_customWaypointHoverActiveHierarchy;

	[SerializeField]
	private GameObject m_customWaypointActiveHierarchy;

	[SerializeField]
	private GameObject m_customWaypointPositionHover;

	[SerializeField]
	private GameObject m_customWaypointDirectionHover;

	[SerializeField]
	private GameObject m_customWaypointPosition;

	[SerializeField]
	private float m_customWaypointOuterCircleRadius;

	[SerializeField]
	private GameObject m_navigationUpdatePaused;

	[SerializeField]
	private GameObject m_navigationUpdateActive;

	[SerializeField]
	private GameObject m_navigationReadyLight;

	[SerializeField]
	private TextSetter m_aboveNavVisVal;

	[SerializeField]
	private TextSetter m_belowNavVisVal;

	[SerializeField]
	[NamedText]
	private string m_aboveLabelName;

	[SerializeField]
	[NamedText]
	private string m_belowLabelName;

	private string m_aboveLabelText;

	private string m_belowLabelText;

	[SerializeField]
	private GameObject m_aboveNavVisGreen;

	[SerializeField]
	private GameObject m_aboveNavVisRed;

	[SerializeField]
	private GameObject m_belowNavVisGreen;

	[SerializeField]
	private GameObject m_belowNavVisRed;

	[SerializeField]
	private GameObject m_mapClickEnabledHierarchy;

	[SerializeField]
	private GameObject[] m_systemAlertVisibility;

	[SerializeField]
	[NamedText]
	private string m_etaNamedText;

	[SerializeField]
	[NamedText]
	private string m_etaNaNamedText;

	[SerializeField]
	[NamedText]
	private string m_etaUnknownNamedText;

	[SerializeField]
	[NamedText]
	private string m_etaArrived;

	[SerializeField]
	private TextSetter m_notepadText;

	[SerializeField]
	private UISelectFinder m_navigationCustomWaypointFinder;

	[SerializeField]
	private UISelectFinder m_mainFinder;

	[SerializeField]
	private GameObject m_controlPromptCustomWaypoint;

	[SerializeField]
	private GameObject m_controlPromptSetHeading;

	[SerializeField]
	private UISelectorMovementTypeNavigationMap m_navigationMapMovement;

	[SerializeField]
	private Renderer m_mapRenderer;

	private int m_navCalcTimer;

	private bool m_inCustomNavigationArea;

	private void Awake()
	{
		m_directionMarkerButton.OnClick += OnDirectionMarkerClick;
		m_clearMarkerButton.OnClick += ClearMarkerClick;
		m_directionMarkerButton.OnHoverOver += OnDirectionMarkerHoverOver;
		m_directionMarkerButton.OnHoverOut += OnDirectionMarkerHoverOut;
		m_zoomMapToggleButton.OnClick += OnZoomMapClick;
		m_mapRenderer.sharedMaterial = Singleton<MissionMapController>.Instance.GetMissionMapMaterial();
	}

	private void OnEnable()
	{
		Singleton<MainActionButtonMonitor>.Instance.AddListener(ButtonListenerCallback);
	}

	private void OnDisable()
	{
		Singleton<MainActionButtonMonitor>.Instance.RemoveListener(ButtonListenerCallback, invalidateCurrentPress: false);
	}

	public override UISelectFinder GetOverrideFinder()
	{
		if (m_inCustomNavigationArea)
		{
			return m_navigationCustomWaypointFinder;
		}
		return m_mainFinder;
	}

	private bool ButtonListenerCallback(MainActionButtonMonitor.ButtonPress bp)
	{
		if (bp == MainActionButtonMonitor.ButtonPress.TopActionDown && !m_inCustomNavigationArea && m_mapClickEnabledHierarchy.activeInHierarchy)
		{
			Singleton<MissionMapController>.Instance.SetZoom(zoomedIn: true);
			m_inCustomNavigationArea = true;
		}
		if (bp == MainActionButtonMonitor.ButtonPress.TopAction)
		{
			if (m_navigationMapMovement.IsDirectionPressed() && m_inCustomNavigationArea && m_mapClickEnabledHierarchy.activeInHierarchy)
			{
				OnDirectionMarkerClick();
			}
			m_inCustomNavigationArea = false;
		}
		return false;
	}

	public void OnDirectionMarkerHoverOver()
	{
		m_customWaypointHoverActiveHierarchy.SetActive(value: true);
	}

	public void OnDirectionMarkerHoverOut()
	{
		m_customWaypointHoverActiveHierarchy.SetActive(value: false);
	}

	public void OnDirectionMarkerClick()
	{
		Vector2 position = m_directionMarkerButton.Touch.position;
		Camera uICameraForControl = tk2dUIManager.Instance.GetUICameraForControl(m_directionMarkerButton.gameObject);
		Vector3 position2 = Singleton<UISelector>.Instance.GetScreenPosition();
		Vector3 vector = uICameraForControl.ScreenToWorldPoint(position2);
		Vector3 vector2 = vector - m_directionMarkerButton.transform.position;
		if (vector2.magnitude > 0f)
		{
			StationNavigator stationNavigator = (StationNavigator)GetStation();
			stationNavigator.SetCustomNavigationPoint(ToWorldDirection(vector2.normalized));
			m_customWaypointPosition.SetActive(value: false);
			m_customWaypointPosition.SetActive(value: true);
		}
		m_inCustomNavigationArea = false;
	}

	public void ClearMarkerClick()
	{
		StationNavigator stationNavigator = (StationNavigator)GetStation();
		stationNavigator.ClearCustomNavigationPoint();
	}

	public void OnZoomMapClick()
	{
		Singleton<MissionMapController>.Instance.ToggleZoom();
	}

	protected override void SetUpStation()
	{
		m_bomberSystems = Singleton<BomberSpawn>.Instance.GetBomberSystems();
		m_bomber = m_bomberSystems.GetBomberState();
		m_aboveLabelText = Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_aboveLabelName);
		m_belowLabelText = Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_belowLabelName);
		Refresh();
	}

	private Vector3 ToMapDirection(Vector3 worldDirection)
	{
		return new Vector3(worldDirection.z, 0f - worldDirection.x, 0f);
	}

	private Vector3 ToWorldDirection(Vector3 mapDirection)
	{
		return new Vector3(0f - mapDirection.y, 0f, mapDirection.x);
	}

	private void Refresh()
	{
		StationNavigator stationNavigator = (StationNavigator)GetStation();
		m_navigationMarkerProgress.SetValue(Mathf.Clamp01(1f - stationNavigator.GetNavigationCalculationTimer()));
		m_navigationReadyLight.SetActive(stationNavigator.GetNavigationCalculationTimer() == 1f);
		if (stationNavigator.HasCustomNavigationPointObject())
		{
			m_customWaypointActiveHierarchy.SetActive(value: true);
			Vector3 customNavigationPointDirection = stationNavigator.GetCustomNavigationPointDirection();
			m_customWaypointPosition.transform.localPosition = ToMapDirection(customNavigationPointDirection.normalized) * m_customWaypointOuterCircleRadius;
		}
		else
		{
			m_customWaypointActiveHierarchy.SetActive(value: false);
		}
		Camera uICameraForControl = tk2dUIManager.Instance.GetUICameraForControl(m_directionMarkerButton.gameObject);
		if (uICameraForControl != null)
		{
			Vector3 vector = uICameraForControl.ScreenToWorldPoint(Singleton<UISelector>.Instance.GetScreenPosition());
			Vector3 vector2 = vector - m_directionMarkerButton.transform.position;
			vector2.z = 0f;
			if (vector2.magnitude > 0f)
			{
				m_customWaypointPositionHover.transform.localPosition = vector2.normalized * m_customWaypointOuterCircleRadius;
				float num = Mathf.Atan2(vector2.y, vector2.x);
				m_customWaypointDirectionHover.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 57.29578f * num));
			}
		}
		float groundVisValue = stationNavigator.GetGroundVisValue();
		float starsVisValue = stationNavigator.GetStarsVisValue();
		m_aboveNavVisVal.SetText(string.Format(m_aboveLabelText + "{0}%", (int)(starsVisValue * 100f)));
		m_belowNavVisVal.SetText(string.Format(m_belowLabelText + "{0}%", (int)(groundVisValue * 100f)));
		m_belowNavVisGreen.SetActive(groundVisValue > 0f);
		m_belowNavVisRed.SetActive(groundVisValue <= 0f);
		m_aboveNavVisGreen.SetActive(starsVisValue > 0f);
		m_aboveNavVisRed.SetActive(starsVisValue <= 0f);
		m_navigationUpdateActive.SetActive(starsVisValue + groundVisValue > 0f);
		m_navigationUpdatePaused.SetActive(starsVisValue + groundVisValue <= 0f);
		m_mapClickEnabledHierarchy.SetActive(starsVisValue + groundVisValue > 0f && Singleton<MissionMapController>.Instance.GetIsMapZoomedIn());
		Singleton<MissionMapController>.Instance.SetFogSetting(Mathf.Clamp01(1f - (starsVisValue + groundVisValue)));
		m_zoomMapToggleButton.SetState(Singleton<MissionMapController>.Instance.GetIsMapZoomedIn());
		GameObject[] systemAlertVisibility = m_systemAlertVisibility;
		foreach (GameObject gameObject in systemAlertVisibility)
		{
			gameObject.SetActive(starsVisValue + groundVisValue == 0f);
		}
		m_inCustomNavigationArea &= m_mapClickEnabledHierarchy.activeInHierarchy;
		if (Singleton<UISelector>.Instance.IsPrimary())
		{
			m_customWaypointHoverActiveHierarchy.SetActive(m_inCustomNavigationArea && m_navigationMapMovement.IsDirectionPressed());
			m_controlPromptCustomWaypoint.SetActive(!m_inCustomNavigationArea);
			m_controlPromptSetHeading.SetActive(m_inCustomNavigationArea);
		}
		if (m_navCalcTimer == 0)
		{
			float etaTime = 0f;
			bool isUnknown = false;
			if (stationNavigator.GetEta(out etaTime, out isUnknown))
			{
				if (etaTime > 10f)
				{
					int num2 = Mathf.FloorToInt(etaTime / 60f);
					int num3 = (int)etaTime - num2 * 60;
					num3 = Mathf.FloorToInt((float)num3 / 10f) * 10;
					m_notepadText.SetText(string.Format(Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_etaNamedText), num2, num3));
				}
				else
				{
					m_notepadText.SetTextFromLanguageString(m_etaArrived);
				}
			}
			else if (isUnknown)
			{
				m_notepadText.SetTextFromLanguageString(m_etaUnknownNamedText);
			}
			else
			{
				m_notepadText.SetTextFromLanguageString(m_etaNaNamedText);
			}
		}
		m_navCalcTimer = (m_navCalcTimer + 1) % 5;
	}

	private void Update()
	{
		Refresh();
	}
}
