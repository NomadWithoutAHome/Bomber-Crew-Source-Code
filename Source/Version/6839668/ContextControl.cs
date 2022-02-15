using System;
using System.Collections.Generic;
using BomberCrewCommon;
using Common;
using dbox;
using Rewired;
using UnityEngine;
using WingroveAudio;

public class ContextControl : Singleton<ContextControl>
{
	public class RememberedSelectionPositions
	{
		public string m_buttonParentName;

		public string m_buttonName;
	}

	[SerializeField]
	private GameObject m_optionsPrefab;

	[SerializeField]
	private Transform m_uiRoot;

	[SerializeField]
	private CrewSpawner m_spawner;

	[SerializeField]
	private bool m_useWaypoints;

	[SerializeField]
	private BomberCamera m_bomberCamera;

	[SerializeField]
	private tk2dCamera m_uiCamera;

	[SerializeField]
	private Transform m_uiTransform;

	[SerializeField]
	private Transform m_stationPanelUINode;

	[SerializeField]
	private UISelectFinder m_crewmenOnlyFinder;

	[SerializeField]
	private UISelectFinder m_noneFinder;

	[SerializeField]
	private UISelectFinderUsableInteractivesOnly m_interactivesOnlyFinder;

	[SerializeField]
	private UISelectorMovementTypeCrewman m_crewmanSelectMovementType;

	[SerializeField]
	private UISelectorMovementType m_normalSelectMovementType;

	[SerializeField]
	private UISelectFinderUnderHierarchy m_panelHierarchyFinder;

	[SerializeField]
	private GameObject m_targetingModeHierarchy;

	[SerializeField]
	private GameObject m_regularModeHierarchy;

	[SerializeField]
	private GameObject m_walkPreviewPrefab;

	[SerializeField]
	private GameObject m_panelLockedOverridePrefab;

	[SerializeField]
	private GameObject m_selectCrewPrompt;

	[SerializeField]
	private GameObject m_deselectCrewPrompt;

	[SerializeField]
	private GameObject m_moveCrewPrompt;

	[SerializeField]
	private GameObject m_actionPrompt1;

	[SerializeField]
	private GameObject m_actionPrompt2;

	[SerializeField]
	private GameObject m_zoomPrompt;

	[SerializeField]
	private GameObject m_targetingPrompt;

	[SerializeField]
	private GameObject m_holdingSelectNoneSelected;

	[SerializeField]
	private Animator m_crewSelectPanelAnimator;

	[SerializeField]
	private Animator m_zoomStateAnimator;

	[SerializeField]
	private Animator m_zoomLevelAnimator;

	private bool m_crewmanHeldPreviously;

	private bool m_crewSelectedPreviously;

	private GameObject m_currentWalkPreview;

	private WalkableArea m_currentWalkableArea;

	private CrewmanAvatar m_currentlySelectedCrewman;

	private GameObject m_currentlyCreatedOptions;

	public Action<CrewmanAvatar> OnContextChange;

	public Action<bool, UIInteractionMarker> OnMarkerExpanded;

	private GameObject m_currentUIPanel;

	private GameObject m_currentUIPanelPrefab;

	private Station m_currentUIPanelStation;

	private float m_rightClickMovement;

	private Vector3 m_lastMousePos;

	private Vector3 m_cachedMousePosPreTargeting;

	private bool m_isInTargetingMode;

	private bool m_isInMovementMode;

	private bool m_aiDebug;

	private InteractiveItem m_currentlyHoveredInteractiveItem;

	private int m_inputLockedCounter;

	private bool m_targetingAllowed = true;

	private List<UIInteractionMarker> m_allInteractionMarkers = new List<UIInteractionMarker>(64);

	private List<WalkableArea> m_allWalkableAreas = new List<WalkableArea>();

	private bool m_zoomedSinceZoomHeld;

	private bool m_isInZoomMode;

	private float m_zoomCameraDisplayTimer;

	private bool m_selectCrewPromptShown;

	private bool m_deselectCrewPromptShown;

	private bool m_moveCrewPromptShown;

	private bool m_actionPromptShown;

	private bool m_zoomPromptShown;

	private bool m_targetingModePromptShown;

	private float m_zoomHeldTime;

	private bool m_requiresForceSelect;

	private tk2dUIItem m_previouslySelectedMainPanelItem;

	private bool m_hidePromptsSelect;

	private bool m_hidePromptsMove;

	private bool m_hidePromptsZoom;

	private bool m_hidePromptsTagging;

	private bool m_selectCrewBlocked;

	public Dictionary<GameObject, RememberedSelectionPositions> m_rememberedSelectionPositions = new Dictionary<GameObject, RememberedSelectionPositions>();

	private tk2dUICamera[] m_allCameras;

	private void Start()
	{
		m_allCameras = UnityEngine.Object.FindObjectsOfType<tk2dUICamera>();
	}

	public void RegisterUIInteractionMarker(UIInteractionMarker uim)
	{
		m_allInteractionMarkers.Add(uim);
	}

	public void DeRegisterUIInteractionMarker(UIInteractionMarker uim)
	{
		m_allInteractionMarkers.Remove(uim);
	}

	public List<UIInteractionMarker> GetAllInteractionMarkers()
	{
		return m_allInteractionMarkers;
	}

	public void RegisterWalkableArea(WalkableArea wa)
	{
		m_allWalkableAreas.Add(wa);
	}

	public void SetSelectCrewBlocked(bool blocked)
	{
		m_selectCrewBlocked = blocked;
	}

	public void DeRegisterWalkableArea(WalkableArea wa)
	{
		m_allWalkableAreas.Remove(wa);
	}

	public List<WalkableArea> GetAllWalkableAreas()
	{
		return m_allWalkableAreas;
	}

	public void LockInput()
	{
		m_inputLockedCounter++;
	}

	public void UnlockInput()
	{
		m_inputLockedCounter--;
	}

	public void SetTargetingAllowed(bool allowed)
	{
		m_targetingAllowed = allowed;
	}

	public void HidePrompts(bool select, bool move, bool tagging, bool zoom)
	{
		m_hidePromptsMove = move;
		m_hidePromptsSelect = select;
		m_hidePromptsTagging = tagging;
		m_hidePromptsZoom = zoom;
	}

	private void OnEnable()
	{
		Singleton<MainActionButtonMonitor>.Instance.AddListener(PauseListener);
		m_holdingSelectNoneSelected.SetActive(value: false);
		m_selectCrewPrompt.SetActive(value: true);
		m_selectCrewPromptShown = true;
		m_deselectCrewPrompt.SetActive(value: false);
		m_moveCrewPrompt.SetActive(value: false);
		m_actionPrompt1.SetActive(value: false);
		m_actionPrompt2.SetActive(value: false);
		m_zoomPrompt.SetActive(value: true);
		m_zoomPromptShown = true;
		m_targetingPrompt.SetActive(value: true);
		m_targetingModePromptShown = true;
	}

	public bool IsCrewSelectMode()
	{
		return m_crewmanHeldPreviously;
	}

	private bool PauseListener(MainActionButtonMonitor.ButtonPress bp)
	{
		if (bp == MainActionButtonMonitor.ButtonPress.Start)
		{
			TryPause();
		}
		return true;
	}

	public void TryPause()
	{
		if (m_inputLockedCounter == 0 && !Singleton<GameFlow>.Instance.IsPaused())
		{
			ToggleZoomMode(zoomMode: false, zoomButton: true);
			Singleton<UIPopupManager>.Instance.DisplayPopup(Singleton<GameFlow>.Instance.GetPauseMenuPrefab());
		}
	}

	private void OnDisable()
	{
		Singleton<MainActionButtonMonitor>.Instance.RemoveListener(PauseListener, invalidateCurrentPress: false);
		ToggleZoomMode(zoomMode: false, zoomButton: true);
		RefreshCameraSettingsForMenu();
	}

	private void ShowDebugPanel()
	{
		if (m_currentlySelectedCrewman != null)
		{
			GUILayout.Label("Add XP to Current Crewman:");
			if (GUILayout.Button("XP +10000"))
			{
				m_currentlySelectedCrewman.GetCrewman().GetPrimarySkill().AddXP(10000);
			}
		}
		bool activeSelf = m_uiCamera.gameObject.activeSelf;
		activeSelf = GUILayout.Toggle(activeSelf, "Show/hide UI");
		if (activeSelf != m_uiCamera.gameObject.activeSelf)
		{
			m_uiCamera.gameObject.SetActive(activeSelf);
		}
	}

	public bool UseWaypoints()
	{
		return m_useWaypoints;
	}

	public bool ClickOnCrewman(CrewmanAvatar crewman)
	{
		bool result = false;
		if (m_currentlyCreatedOptions != null)
		{
			UnityEngine.Object.Destroy(m_currentlyCreatedOptions);
		}
		if (m_currentlySelectedCrewman != null)
		{
			m_currentlySelectedCrewman.Deselect();
		}
		if (crewman != null)
		{
			if (crewman.Select())
			{
				WingroveRoot.Instance.PostEvent("SELECT_CREWMAN");
				result = true;
				m_currentlySelectedCrewman = crewman;
				if (OnContextChange != null)
				{
					OnContextChange(crewman);
				}
			}
			else
			{
				m_currentlySelectedCrewman = null;
				if (OnContextChange != null)
				{
					OnContextChange(null);
				}
			}
		}
		else
		{
			if (m_currentWalkPreview != null)
			{
				UnityEngine.Object.Destroy(m_currentWalkPreview);
				m_currentWalkPreview = null;
			}
			if (m_currentlySelectedCrewman != null)
			{
				WingroveRoot.Instance.PostEvent("DESELECT_CREWMAN");
			}
			m_currentlySelectedCrewman = null;
			if (OnContextChange != null)
			{
				OnContextChange(null);
			}
		}
		return result;
	}

	public CrewmanAvatar GetCurrentlySelected()
	{
		return m_currentlySelectedCrewman;
	}

	public void SelectCrewman(Crewman crewman)
	{
		ClickOnCrewman(m_spawner.Find(crewman));
	}

	public CrewmanAvatar GetAvatarFor(Crewman crewman)
	{
		return m_spawner.Find(crewman);
	}

	public void ClickOnWalkableArea(Vector3 position, WalkableArea associatedWalkableArea, bool ignoreExternalZones)
	{
		if (!(m_currentlySelectedCrewman != null))
		{
			return;
		}
		if (!m_currentlySelectedCrewman.AreMoveOrdersBlocked())
		{
			if (m_currentlyCreatedOptions != null)
			{
				UnityEngine.Object.Destroy(m_currentlyCreatedOptions);
			}
			m_currentlySelectedCrewman.QueueAction(andCancelExisting: true, position, associatedWalkableArea, ignoreExternalZones);
			if (Singleton<UISelector>.Instance.IsPrimary())
			{
				m_currentlySelectedCrewman.FlashPath();
				Singleton<UISelector>.Instance.FlashCursor();
			}
			WingroveRoot.Instance.PostEvent("GIVE_ORDER");
		}
		else if (m_currentUIPanel != null)
		{
			m_currentUIPanel.GetComponent<StationPanel>().FlashLock();
		}
	}

	public void HoverWalkableArea(Vector3 position, WalkableArea wa, bool ignoreExternal)
	{
		if (m_currentlySelectedCrewman != null && !m_isInTargetingMode && !m_currentlySelectedCrewman.AreMoveOrdersBlocked() && !ReInput.players.GetPlayer(0).GetButton(33) && !Input.GetMouseButton(1))
		{
			if (m_currentWalkPreview == null)
			{
				m_currentWalkPreview = UnityEngine.Object.Instantiate(m_walkPreviewPrefab);
				m_currentWalkPreview.transform.parent = null;
			}
			BomberWalkZone bomberWalkZone = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetAllWalkZones().FindContainingWalkZone(position, ignoreExternal);
			Vector3 nearestPlanarPoint = bomberWalkZone.GetNearestPlanarPoint(position);
			m_currentWalkPreview.transform.position = nearestPlanarPoint;
			m_currentWalkPreview.transform.LookAt(m_currentWalkPreview.transform.position + Vector3.forward, bomberWalkZone.transform.up);
			m_currentWalkableArea = wa;
			m_currentlySelectedCrewman.HoverPath(wa.gameObject, bomberWalkZone, nearestPlanarPoint);
		}
	}

	public void UnhoverWalkableArea(WalkableArea wa)
	{
		if (m_currentWalkableArea == wa && m_currentWalkPreview != null)
		{
			UnityEngine.Object.Destroy(m_currentWalkPreview);
			m_currentWalkPreview = null;
			if (m_currentlySelectedCrewman != null)
			{
				m_currentlySelectedCrewman.UnhoverPath(wa.gameObject);
			}
		}
	}

	public void HoverInteractive(InteractiveItem ii)
	{
		m_currentlyHoveredInteractiveItem = ii;
		if (m_currentlySelectedCrewman != null)
		{
			m_currentWalkableArea = null;
			if (m_currentWalkPreview != null)
			{
				UnityEngine.Object.Destroy(m_currentWalkPreview);
				m_currentWalkPreview = null;
			}
			if (!ReInput.players.GetPlayer(0).GetButton(33) && !Input.GetMouseButton(1))
			{
				m_currentlySelectedCrewman.HoverPath(ii.gameObject, ii.GetContainerWalkZone(), ii.transform.position);
			}
		}
	}

	public void UnhoverInteractive(InteractiveItem ii)
	{
		if (m_currentlyHoveredInteractiveItem == ii)
		{
			if (m_currentlySelectedCrewman != null && ii != null)
			{
				m_currentlySelectedCrewman.UnhoverPath(ii.gameObject);
			}
			m_currentlyHoveredInteractiveItem = null;
		}
	}

	public void TemporarilyUnhover(InteractiveItem ii)
	{
		if (m_currentlyHoveredInteractiveItem == ii && m_currentlySelectedCrewman != null && ii != null)
		{
			m_currentlySelectedCrewman.UnhoverPath(ii.gameObject);
		}
	}

	public InteractiveItem.InteractionContract DoInteraction(InteractiveItem.Interaction interaction, InteractiveItem item)
	{
		if (m_currentlySelectedCrewman != null)
		{
			InteractiveItem.InteractionContract result = m_currentlySelectedCrewman.QueueAction(andCancelExisting: true, item, interaction);
			OnContextChange(m_currentlySelectedCrewman);
			if (Singleton<UISelector>.Instance.IsPrimary())
			{
				m_currentlySelectedCrewman.FlashPath();
				if (item.GetInteractionItem() != null)
				{
					Singleton<UISelector>.Instance.ForcePointAt(item.GetInteractionItem());
					HoverInteractive(item);
				}
				Singleton<UISelector>.Instance.FlashCursor();
			}
			WingroveRoot.Instance.PostEvent("GIVE_ORDER");
			return result;
		}
		return null;
	}

	public Transform GetUITransform()
	{
		return m_uiTransform;
	}

	public tk2dCamera GetUICamera()
	{
		return m_uiCamera;
	}

	public void GoToManual(string manualRef)
	{
		UIPopupData uIPopupData = new UIPopupData();
		uIPopupData.PopupStartCallback = (Action<UIPopUp>)Delegate.Combine(uIPopupData.PopupStartCallback, (Action<UIPopUp>)delegate(UIPopUp pop)
		{
			pop.GetComponent<InMissionPauseMenu>().SetShowManual(manualRef);
		});
		Singleton<UIPopupManager>.Instance.DisplayPopup(Singleton<GameFlow>.Instance.GetPauseMenuPrefab(), uIPopupData);
	}

	public bool IsMovementMode()
	{
		return m_isInMovementMode;
	}

	private void Update()
	{
		if (Singleton<GameFlow>.Instance.IsPaused())
		{
			return;
		}
		tk2dUIItem currentlyPointedAtItem = m_normalSelectMovementType.GetCurrentlyPointedAtItem();
		if (currentlyPointedAtItem != null)
		{
			m_previouslySelectedMainPanelItem = currentlyPointedAtItem;
		}
		float num = ((!(Time.timeScale > 0f)) ? 0f : (Time.deltaTime / Time.timeScale));
		if (m_currentlySelectedCrewman != null && m_currentlyHoveredInteractiveItem != null && !ReInput.players.GetPlayer(0).GetButton(33) && !Input.GetMouseButton(1))
		{
			m_currentlySelectedCrewman.HoverPath(m_currentlyHoveredInteractiveItem.gameObject, m_currentlyHoveredInteractiveItem.GetContainerWalkZone(), m_currentlyHoveredInteractiveItem.transform.position);
		}
		bool flag = m_inputLockedCounter > 0;
		if (!flag)
		{
			if (ReInput.players.GetPlayer(0).GetButtonDown(33) || Input.GetMouseButtonDown(1))
			{
				m_rightClickMovement = 0f;
			}
			if (ReInput.players.GetPlayer(0).GetButton(33) || Input.GetMouseButton(1))
			{
				UnhoverWalkableArea(m_currentWalkableArea);
				TemporarilyUnhover(m_currentlyHoveredInteractiveItem);
				m_rightClickMovement += (Input.mousePosition - m_lastMousePos).magnitude;
			}
			if ((ReInput.players.GetPlayer(0).GetButtonUp(33) || Input.GetMouseButtonUp(1)) && m_rightClickMovement < 24f)
			{
				SelectCrewman(null);
			}
		}
		bool flag2 = false;
		if (!flag && !m_selectCrewBlocked)
		{
			if (ReInput.players.GetPlayer(0).GetButton(3))
			{
				Singleton<UISelector>.Instance.SetFinder(m_crewmenOnlyFinder);
				flag2 = true;
				Singleton<MissionSpeedControls>.Instance.SetIsSlowedByControls();
			}
			if (ReInput.players.GetPlayer(0).GetButtonUp(3))
			{
				if (!m_crewmanSelectMovementType.WasMoved())
				{
					SelectCrewman(null);
				}
				Singleton<UISelector>.Instance.SetFinder(m_noneFinder);
			}
		}
		bool flag3 = true;
		bool flag4 = false;
		bool flag5 = false;
		bool flag6 = false;
		bool flag7 = true;
		bool flag8 = true;
		bool flag9 = m_currentlySelectedCrewman != null;
		if (flag2)
		{
			if (m_crewmanHeldPreviously != flag2 || m_crewSelectedPreviously != flag9)
			{
				if (!flag9)
				{
					m_holdingSelectNoneSelected.CustomActivate(active: true);
				}
				else
				{
					m_holdingSelectNoneSelected.CustomActivate(active: false);
				}
			}
			flag3 = false;
			flag5 = false;
			flag4 = false;
		}
		else
		{
			if (m_crewmanHeldPreviously != flag2 || m_crewSelectedPreviously != flag9)
			{
				m_holdingSelectNoneSelected.CustomActivate(active: false);
			}
			if (!flag9)
			{
				flag3 = true;
				flag4 = false;
				flag5 = false;
			}
			else
			{
				flag3 = false;
				flag4 = true;
				flag5 = true;
			}
		}
		m_crewSelectedPreviously = flag9;
		m_crewmanHeldPreviously = flag2;
		if (m_crewSelectPanelAnimator.isActiveAndEnabled)
		{
			m_crewSelectPanelAnimator.SetBool("FocusOn", flag2);
		}
		bool flag10 = ReInput.players.GetPlayer(0).GetButton(17) && !ReInput.players.GetPlayer(0).GetButton(16) && !flag2;
		bool flag11 = flag10 && ReInput.players.GetPlayer(0).GetButtonDown(17);
		bool flag12 = ReInput.players.GetPlayer(0).GetButton(48) && !flag10 && !m_isInTargetingMode;
		if (flag12 != m_isInZoomMode)
		{
			ToggleZoomMode(flag12, ReInput.players.GetPlayer(0).GetButton(48));
		}
		if (m_isInZoomMode)
		{
			m_zoomHeldTime += Time.deltaTime;
			m_bomberCamera.SetZoomDirection(ReInput.players.GetPlayer(0).GetAxis(0));
			if (m_bomberCamera.HasZoomedRecently())
			{
				m_zoomedSinceZoomHeld = true;
			}
		}
		else
		{
			m_zoomHeldTime = 0f;
			m_bomberCamera.SetZoomDirection(0f);
		}
		if (m_zoomStateAnimator.isActiveAndEnabled)
		{
			m_zoomStateAnimator.SetBool("FocusOn", m_isInZoomMode || m_zoomCameraDisplayTimer > 0f);
		}
		if (m_isInZoomMode)
		{
			m_zoomCameraDisplayTimer = 1f;
		}
		m_zoomCameraDisplayTimer -= num;
		if (m_zoomLevelAnimator.isActiveAndEnabled)
		{
			m_zoomLevelAnimator.SetInteger("Level", m_bomberCamera.GetZoomLevelForUIWidget());
		}
		if (flag)
		{
			flag10 = false;
			flag11 = false;
		}
		bool flag13 = false;
		if (ReInput.players.GetPlayer(0).GetButtonDown(34))
		{
			m_bomberCamera.ReCentre();
			flag13 = true;
		}
		if (ReInput.players.GetPlayer(0).GetButton(34))
		{
			flag13 = true;
		}
		GameObject gameObject = null;
		Station station = null;
		m_isInMovementMode = false;
		if (m_currentlySelectedCrewman != null)
		{
			station = m_currentlySelectedCrewman.GetStation();
			if (station != null)
			{
				gameObject = station.GetUIPanelPrefab();
				if (m_currentlySelectedCrewman.AreOrdersBlocked())
				{
					gameObject = m_panelLockedOverridePrefab;
				}
			}
			if (flag10)
			{
				m_isInMovementMode = true;
				Singleton<MissionSpeedControls>.Instance.SetIsSlowedByControls();
				UISelectorMovementTypeFree uISelectorMovementTypeFree = (UISelectorMovementTypeFree)m_interactivesOnlyFinder.GetMovementType();
				Singleton<UISelector>.Instance.SetFinder(m_interactivesOnlyFinder);
				if (flag11)
				{
					m_interactivesOnlyFinder.GetMovementType().ForcePointAt(m_currentlySelectedCrewman.gameObject, LayerMask.NameToLayer("_UIWorld"));
					m_bomberCamera.ToggleZoomInternalIfNotInternal(uISelectorMovementTypeFree.GetRoughWorldPosition(m_bomberCamera.GetCamera(), m_bomberCamera.GetDistance()));
				}
				if (m_bomberCamera.ToggleZoomWhileInteracting(0f - ReInput.players.GetPlayer(0).GetAxis(7), ReInput.players.GetPlayer(0).GetButtonUp(48), uISelectorMovementTypeFree.GetRoughWorldPosition(m_bomberCamera.GetCamera(), m_bomberCamera.GetDistance())))
				{
					uISelectorMovementTypeFree.SetCameraZoomHappened();
				}
				switch (m_bomberCamera.GetZoomLevel())
				{
				case 0:
				{
					if (uISelectorMovementTypeFree.DidSnap())
					{
						m_bomberCamera.DoCentralisingActions(uISelectorMovementTypeFree.GetRoughWorldPosition(m_bomberCamera.GetCamera(), m_bomberCamera.GetDistance()));
						uISelectorMovementTypeFree.SetCameraZoomHappened();
					}
					float scrollHint = uISelectorMovementTypeFree.GetScrollHint();
					m_bomberCamera.NudgeScroll(scrollHint * Time.deltaTime * 2f);
					break;
				}
				case 1:
					m_bomberCamera.ReCentre();
					break;
				}
				flag13 = true;
				flag6 = true;
				flag4 = false;
				flag5 = false;
				flag3 = false;
			}
			else
			{
				m_panelHierarchyFinder.SetHierarchy(m_stationPanelUINode);
				if (!flag2)
				{
					UISelectFinder finder = m_panelHierarchyFinder;
					if (m_currentUIPanel != null)
					{
						UISelectFinder overrideFinder = m_currentUIPanel.GetComponent<StationPanel>().GetOverrideFinder();
						if (overrideFinder != null)
						{
							finder = overrideFinder;
						}
					}
					Singleton<UISelector>.Instance.SetFinder(finder);
					if (m_requiresForceSelect && m_currentUIPanelPrefab != null)
					{
						RememberedSelectionPositions value = null;
						m_rememberedSelectionPositions.TryGetValue(m_currentUIPanelPrefab, out value);
						if (value != null)
						{
							tk2dUIItem tk2dUIItem2 = null;
							tk2dUIItem[] allItems = m_panelHierarchyFinder.GetAllItems();
							tk2dUIItem[] array = allItems;
							foreach (tk2dUIItem tk2dUIItem3 in array)
							{
								if (tk2dUIItem3.name == value.m_buttonName && tk2dUIItem3.transform.parent.name == value.m_buttonParentName)
								{
									tk2dUIItem2 = tk2dUIItem3;
								}
							}
							if (tk2dUIItem2 != null)
							{
								m_normalSelectMovementType.ForcePointAt(tk2dUIItem2);
							}
						}
						m_requiresForceSelect = false;
						m_previouslySelectedMainPanelItem = null;
					}
				}
			}
		}
		else if (!flag2)
		{
			Singleton<UISelector>.Instance.SetFinder(m_noneFinder);
		}
		if (flag13)
		{
			m_bomberCamera.SetLockedAtZoomMax(1);
		}
		else
		{
			m_bomberCamera.SetNotLockedAtMax();
		}
		if (gameObject != m_currentUIPanelPrefab || m_currentUIPanelStation != station)
		{
			m_currentUIPanelStation = station;
			m_requiresForceSelect = true;
			if (m_currentUIPanel != null)
			{
				RememberedSelectionPositions rememberedSelectionPositions = new RememberedSelectionPositions();
				if (m_previouslySelectedMainPanelItem != null)
				{
					rememberedSelectionPositions.m_buttonName = m_previouslySelectedMainPanelItem.name;
					rememberedSelectionPositions.m_buttonParentName = m_previouslySelectedMainPanelItem.transform.parent.name;
					m_rememberedSelectionPositions[m_currentUIPanelPrefab] = rememberedSelectionPositions;
					m_previouslySelectedMainPanelItem = null;
				}
				else
				{
					m_rememberedSelectionPositions[m_currentUIPanelPrefab] = null;
				}
				UnityEngine.Object.Destroy(m_currentUIPanel);
			}
			m_currentUIPanelPrefab = gameObject;
			if (gameObject != null)
			{
				m_currentUIPanel = UnityEngine.Object.Instantiate(gameObject);
				m_currentUIPanel.transform.parent = m_stationPanelUINode;
				m_currentUIPanel.transform.localPosition = Vector3.zero;
				StationPanel component = m_currentUIPanel.GetComponent<StationPanel>();
				if (component != null && m_currentlySelectedCrewman != null && station != null)
				{
					component.SetUpStationView(station, m_currentlySelectedCrewman.GetCrewman());
				}
			}
		}
		bool flag14 = ReInput.players.GetPlayer(0).GetButtonUp(12);
		if (flag || !m_targetingAllowed)
		{
			flag14 = false;
		}
		else if (!m_isInTargetingMode)
		{
			if (ReInput.players.GetPlayer(0).GetButtonDown(16))
			{
				flag14 = true;
			}
		}
		else if (ReInput.players.GetPlayer(0).GetButtonUp(16))
		{
			flag14 = true;
		}
		if (flag14)
		{
			ToggleTargetingMode();
		}
		m_lastMousePos = Input.mousePosition;
		if (m_hidePromptsZoom)
		{
			flag7 = false;
		}
		if (m_hidePromptsTagging)
		{
			flag8 = false;
		}
		if (m_hidePromptsSelect)
		{
			flag3 = false;
		}
		if (m_hidePromptsMove)
		{
			flag5 = false;
		}
		if (!Singleton<UISelector>.Instance.IsPrimary())
		{
			flag6 = false;
			flag4 = false;
			flag5 = false;
			flag3 = false;
			flag8 = false;
			flag7 = false;
		}
		if (m_selectCrewPromptShown != flag3)
		{
			m_selectCrewPromptShown = flag3;
			m_selectCrewPrompt.CustomActivate(flag3);
		}
		if (m_deselectCrewPromptShown != flag4)
		{
			m_deselectCrewPromptShown = flag4;
			m_deselectCrewPrompt.CustomActivate(flag4);
		}
		if (m_moveCrewPromptShown != flag5)
		{
			m_moveCrewPromptShown = flag5;
			m_moveCrewPrompt.CustomActivate(flag5);
		}
		if (m_zoomPromptShown != flag7)
		{
			m_zoomPromptShown = flag7;
			m_zoomPrompt.CustomActivate(flag7);
		}
		if (m_actionPromptShown != flag6)
		{
			m_actionPromptShown = flag6;
			m_actionPrompt1.CustomActivate(flag6);
			m_actionPrompt2.CustomActivate(flag6);
		}
		if (m_targetingModePromptShown != flag8)
		{
			m_targetingModePromptShown = flag8;
			m_targetingPrompt.CustomActivate(flag8);
		}
	}

	private void ToggleZoomMode(bool zoomMode, bool zoomButton)
	{
		if (zoomMode != m_isInZoomMode)
		{
			m_isInZoomMode = zoomMode;
			if (zoomButton == m_isInZoomMode && !m_isInZoomMode && !m_zoomedSinceZoomHeld && m_zoomHeldTime < 0.75f)
			{
				m_bomberCamera.ToggleZoomInternal();
			}
			if (m_isInZoomMode)
			{
				Singleton<UISelector>.Instance.Pause();
				Singleton<InputLayerInterface>.Instance.DisableAllLayers();
				m_bomberCamera.ResetZoomedRecently();
			}
			else
			{
				Singleton<UISelector>.Instance.Resume();
				Singleton<InputLayerInterface>.Instance.EnableAllLayers();
			}
			m_zoomedSinceZoomHeld = false;
		}
	}

	private void SetTargetingModeEnabled(bool targetingMode)
	{
		if (targetingMode != m_isInTargetingMode)
		{
			m_isInTargetingMode = targetingMode;
			if (m_isInTargetingMode)
			{
				DboxInMissionController.DBoxCall(DboxSdkWrapper.PostTarget);
			}
			else
			{
				DboxInMissionController.DBoxCall(DboxSdkWrapper.PostTargetCancel);
			}
			if (m_isInTargetingMode)
			{
				WingroveRoot.Instance.PostEvent("TARGETING_MODE_ON");
				UnhoverWalkableArea(m_currentWalkableArea);
				Singleton<InputLayerInterface>.Instance.DisableLayerInput("_UIWorld");
			}
			else
			{
				WingroveRoot.Instance.PostEvent("TARGETING_MODE_OFF");
				Singleton<InputLayerInterface>.Instance.EnableLayerInput("_UIWorld");
			}
			if (m_targetingModeHierarchy != null)
			{
				m_targetingModeHierarchy.CustomActivate(m_isInTargetingMode);
			}
			if (m_regularModeHierarchy != null)
			{
				m_regularModeHierarchy.CustomActivate(!m_isInTargetingMode);
			}
		}
		RefreshCameraSettingsForTargeting();
	}

	public void RefreshCameraSettingsForTargeting()
	{
		Singleton<BomberCamera>.Instance.SetTargetingMode(m_isInTargetingMode);
		Singleton<UISelector>.Instance.gameObject.SetActive(!m_isInTargetingMode);
		tk2dUICamera[] allCameras = m_allCameras;
		foreach (tk2dUICamera tk2dUICamera2 in allCameras)
		{
			if (tk2dUICamera2 != null)
			{
				tk2dUICamera2.enabled = !m_isInTargetingMode;
			}
		}
	}

	public void RefreshCameraSettingsForMenu()
	{
		Singleton<BomberCamera>.Instance.SetTargetingMode(targetingMode: false);
		Singleton<UISelector>.Instance.gameObject.SetActive(value: true);
		tk2dUICamera[] allCameras = m_allCameras;
		foreach (tk2dUICamera tk2dUICamera2 in allCameras)
		{
			if (tk2dUICamera2 != null)
			{
				tk2dUICamera2.enabled = true;
			}
		}
	}

	private void ToggleTargetingMode()
	{
		SetTargetingModeEnabled(!m_isInTargetingMode);
	}

	public bool IsTargetingMode()
	{
		return m_isInTargetingMode;
	}
}
