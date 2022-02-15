using System;
using System.Collections.Generic;
using BomberCrewCommon;
using Rewired;
using Rewired.ControllerExtensions;
using UnityEngine;

public class UISelector : Singleton<UISelector>
{
	[SerializeField]
	private tk2dCamera m_uiCamera;

	[SerializeField]
	private float m_initialRepressSpeed = 0.5f;

	[SerializeField]
	private float m_minRepressSpeed = 0.3f;

	[SerializeField]
	private float m_repressModifier;

	[SerializeField]
	private UISelectFinder m_defaultFinder;

	[SerializeField]
	private UISelectorMovementType m_defaultMovementType;

	[SerializeField]
	private bool m_pcCanSwitchControlType;

	private bool m_isPrimary;

	private GameObject m_currentlySpawnedWidget;

	private Vector2 m_lastMove;

	private float m_repressCtr;

	private float m_repressCurrentTarg;

	private tk2dUIItem m_currentlyDownItem;

	private Vector3 m_lastMousePosition;

	private UISelectFinder m_currentFinder;

	private UISelectorMovementType m_currentMovementTracker;

	private Dictionary<string, UISelectorPointingHint> m_allHints = new Dictionary<string, UISelectorPointingHint>();

	private int m_pauseCount;

	private static bool s_previousPrimary;

	private Controller m_lastCachedController;

	private ControlPromptDisplayHelpers.ControllerMapSpriteType m_spriteType;

	public event Action OnFilterChange;

	private void Awake()
	{
		SetMovementTracker(m_defaultMovementType);
		SetFinder(m_defaultFinder);
		m_lastMousePosition = Input.mousePosition;
	}

	private void Start()
	{
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		SetPrimary(s_previousPrimary);
	}

	private void OnDisable()
	{
		if (m_isPrimary && m_currentlySpawnedWidget != null)
		{
			m_currentlySpawnedWidget.SetActive(value: false);
		}
	}

	public void RegisterHint(UISelectorPointingHint ph, string hName)
	{
		m_allHints[hName] = ph;
	}

	public void DeregisterHint(UISelectorPointingHint ph, string hName)
	{
		m_allHints.Remove(hName);
	}

	public UISelectorPointingHint GetHintForName(string hName)
	{
		UISelectorPointingHint value = null;
		m_allHints.TryGetValue(hName, out value);
		return value;
	}

	public void Pause()
	{
		m_pauseCount++;
	}

	public void Resume()
	{
		m_pauseCount--;
	}

	private void OnEnable()
	{
		if (m_isPrimary && m_currentlySpawnedWidget != null)
		{
			m_currentlySpawnedWidget.SetActive(value: true);
		}
	}

	public bool IsPrimary()
	{
		return m_isPrimary;
	}

	public Vector2 GetScreenPosition()
	{
		if (m_isPrimary)
		{
			return m_currentMovementTracker.GetCurrentScreenSpacePointerPosition();
		}
		return Input.mousePosition;
	}

	public UISelectFinder GetCurrentFinder()
	{
		return m_currentFinder;
	}

	public UISelectorMovementType GetCurrentMovementType()
	{
		return m_currentMovementTracker;
	}

	public bool IsValid(tk2dUIItem item)
	{
		return item.enabled && item.gameObject.activeInHierarchy && item.GetComponent<Collider>() != null && item.GetComponent<Collider>().enabled;
	}

	private void Update()
	{
		if (m_pauseCount > 0)
		{
			return;
		}
		float num = ((Time.timeScale != 0f) ? (Time.deltaTime / Time.timeScale) : Mathf.Clamp(Time.unscaledDeltaTime, 0f, 0.1f));
		Vector2 normalized = ReInput.players.GetPlayer(0).GetAxis2D(1, 0).normalized;
		Vector2 tickMove = Vector3.zero;
		if (Vector3.Dot(normalized.normalized, m_lastMove.normalized) < 0.5f || normalized.magnitude < m_lastMove.magnitude * 0.8f)
		{
			tickMove = normalized;
			m_repressCtr = m_initialRepressSpeed;
			m_repressCurrentTarg = m_initialRepressSpeed;
		}
		else
		{
			m_repressCtr -= num;
			if (m_repressCtr < 0f)
			{
				m_repressCurrentTarg = (m_repressCtr = Mathf.Max(m_repressCurrentTarg - m_repressModifier, m_minRepressSpeed));
				tickMove = normalized;
			}
		}
		if (tickMove.magnitude > 0f && m_pcCanSwitchControlType && !m_isPrimary)
		{
			SetPrimary(isPrimary: true);
			return;
		}
		if (IsPrimary())
		{
			m_currentMovementTracker.DoMovement(normalized, tickMove);
		}
		if (ReInput.players.GetPlayer(0).GetButtonDown(2))
		{
			tk2dUIItem currentlyPointedAtItem = m_currentMovementTracker.GetCurrentlyPointedAtItem();
			if (currentlyPointedAtItem != null && IsValid(currentlyPointedAtItem))
			{
				if (m_pcCanSwitchControlType && !m_isPrimary)
				{
					SetPrimary(isPrimary: true);
					return;
				}
				m_currentlyDownItem = currentlyPointedAtItem;
				m_currentlyDownItem.SimulateDown();
			}
		}
		if (ReInput.players.GetPlayer(0).GetButtonUp(2) && m_currentlyDownItem != null && IsValid(m_currentlyDownItem))
		{
			if (m_pcCanSwitchControlType && !m_isPrimary)
			{
				SetPrimary(isPrimary: true);
				return;
			}
			m_currentlyDownItem.HoverOut(null);
			m_currentlyDownItem.SimulateUpClick();
			if (m_currentlyDownItem != null && m_currentlyDownItem == m_currentMovementTracker.GetCurrentlyPointedAtItem())
			{
				m_currentlyDownItem.HoverOver(null);
			}
			m_currentlyDownItem = null;
		}
		if (m_currentlyDownItem != null && (!IsValid(m_currentlyDownItem) || m_currentMovementTracker.GetCurrentlyPointedAtItem() != m_currentlyDownItem))
		{
			if (m_pcCanSwitchControlType && !m_isPrimary)
			{
				SetPrimary(isPrimary: true);
				return;
			}
			m_currentlyDownItem.SimulateRelease();
			m_currentlyDownItem = null;
		}
		if (m_isPrimary)
		{
			m_currentMovementTracker.UpdateLogic();
		}
		if (m_currentlySpawnedWidget != null)
		{
			m_currentlySpawnedWidget.GetComponent<PointingWidget>().SetPointedAt(m_currentMovementTracker, m_uiCamera);
		}
		if ((Input.GetMouseButton(0) || Input.GetMouseButton(1) || (Input.mousePosition - m_lastMousePosition).magnitude > 5f) && m_pcCanSwitchControlType)
		{
			SetPrimary(isPrimary: false);
		}
		m_lastMousePosition = Input.mousePosition;
		m_lastMove = normalized;
	}

	public void FlashCursor()
	{
		if (m_isPrimary && m_currentlySpawnedWidget != null)
		{
			m_currentlySpawnedWidget.GetComponent<PointingWidget>().Flash();
		}
	}

	private void SetPrimary(bool isPrimary)
	{
		if (isPrimary == m_isPrimary)
		{
			return;
		}
		m_isPrimary = isPrimary;
		s_previousPrimary = m_isPrimary;
		if (isPrimary)
		{
			Cursor.visible = false;
			if (m_currentlySpawnedWidget != null)
			{
				m_currentlySpawnedWidget.SetActive(value: true);
			}
			tk2dUIManager[] array = UnityEngine.Object.FindObjectsOfType<tk2dUIManager>();
			tk2dUIManager[] array2 = array;
			foreach (tk2dUIManager tk2dUIManager2 in array2)
			{
				tk2dUIManager2.ForceHoverOut();
				tk2dUIManager2.InputEnabled = false;
			}
			UISelectFinder currentFinder = m_currentFinder;
			SetFinder(null);
			SetFinder(currentFinder);
			if (currentFinder != null && currentFinder.GetMovementType().GetCurrentlyPointedAtItem() == null && currentFinder.GetMovementType() is UISelectorMovementTypeSnap)
			{
				UISelectorMovementTypeSnap uISelectorMovementTypeSnap = (UISelectorMovementTypeSnap)currentFinder.GetMovementType();
				if (uISelectorMovementTypeSnap != null)
				{
					uISelectorMovementTypeSnap.FindBest();
				}
			}
		}
		else
		{
			Cursor.visible = true;
			tk2dUIManager[] array3 = UnityEngine.Object.FindObjectsOfType<tk2dUIManager>();
			tk2dUIManager[] array4 = array3;
			foreach (tk2dUIManager tk2dUIManager3 in array4)
			{
				tk2dUIManager3.InputEnabled = true;
			}
			if (m_currentlySpawnedWidget != null)
			{
				m_currentlySpawnedWidget.SetActive(value: false);
			}
			if (m_currentlyDownItem != null)
			{
				m_currentlyDownItem.SimulateRelease();
				m_currentlyDownItem = null;
			}
		}
	}

	public bool UseVirtualKeyboard()
	{
		return false;
	}

	private void SetMovementTracker(UISelectorMovementType tracker)
	{
		if (tracker == null)
		{
			tracker = m_defaultMovementType;
		}
		if (tracker != m_currentMovementTracker)
		{
			m_currentMovementTracker = tracker;
			if (m_currentFinder != null && m_isPrimary)
			{
				m_currentMovementTracker.SetUp(m_currentFinder);
			}
		}
	}

	public void SetFinder(UISelectFinder finder)
	{
		if (finder == null)
		{
			finder = m_defaultFinder;
		}
		if (finder != m_currentFinder)
		{
			if (m_currentMovementTracker != null)
			{
				m_currentMovementTracker.DeSelect();
			}
			m_currentFinder = finder;
			if (m_currentlySpawnedWidget != null)
			{
				UnityEngine.Object.Destroy(m_currentlySpawnedWidget);
			}
			GameObject pointingWidgetPrefab = finder.GetPointingWidgetPrefab();
			if (pointingWidgetPrefab != null)
			{
				m_currentlySpawnedWidget = UnityEngine.Object.Instantiate(pointingWidgetPrefab);
				m_currentlySpawnedWidget.SetActive(m_isPrimary);
			}
			if (m_isPrimary && m_currentlyDownItem != null && !MatchesFilter(m_currentlyDownItem))
			{
				m_currentlyDownItem.SimulateRelease();
				m_currentlyDownItem = null;
			}
			SetMovementTracker(finder.GetMovementType());
			if (m_isPrimary)
			{
				m_currentMovementTracker.SetUp(m_currentFinder);
			}
			if (m_currentlySpawnedWidget != null)
			{
				m_currentlySpawnedWidget.GetComponent<PointingWidget>().SetPointedAt(m_currentMovementTracker, m_uiCamera);
			}
			if (this.OnFilterChange != null)
			{
				this.OnFilterChange();
			}
		}
	}

	public bool MatchesFilter(tk2dUIItem item)
	{
		return m_isPrimary && m_currentFinder != null && m_currentFinder.DoesItemMatch(item);
	}

	public void Reselect()
	{
		m_currentMovementTracker.DoMovement(Vector3.zero, Vector3.zero);
	}

	public bool IsPaused()
	{
		return m_pauseCount > 0;
	}

	public ControlPromptDisplayHelpers.ControllerMapSpriteType GetControllerType()
	{
		if (!Singleton<UISelector>.Instance.IsPrimary())
		{
			return ControlPromptDisplayHelpers.ControllerMapSpriteType.STEAM;
		}
		Controller lastActiveController = ReInput.players.GetPlayer(0).controllers.GetLastActiveController(ControllerType.Joystick);
		if (lastActiveController != null)
		{
			if (lastActiveController != m_lastCachedController)
			{
				DualShock4Extension extension = lastActiveController.GetExtension<DualShock4Extension>();
				if (extension == null)
				{
					m_spriteType = ControlPromptDisplayHelpers.ControllerMapSpriteType.STEAM;
				}
				else
				{
					m_spriteType = ControlPromptDisplayHelpers.ControllerMapSpriteType.PS4;
				}
				m_lastCachedController = lastActiveController;
			}
			return m_spriteType;
		}
		return ControlPromptDisplayHelpers.ControllerMapSpriteType.STEAM;
	}

	public void ForcePointAt(tk2dUIItem uiItem)
	{
		if (uiItem != null)
		{
			if (m_currentlyDownItem != null)
			{
				m_currentlyDownItem.SimulateRelease();
				m_currentlyDownItem = null;
			}
			m_currentMovementTracker.ForcePointAt(uiItem);
		}
	}
}
