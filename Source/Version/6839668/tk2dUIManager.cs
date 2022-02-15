using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("2D Toolkit/UI/Core/tk2dUIManager")]
public class tk2dUIManager : MonoBehaviour
{
	public static double version = 1.0;

	public static int releaseId = 0;

	private static tk2dUIManager instance;

	[SerializeField]
	private Camera uiCamera;

	private static List<tk2dUICamera> allCameras = new List<tk2dUICamera>();

	private List<tk2dUICamera> sortedCameras = new List<tk2dUICamera>();

	public LayerMask raycastLayerMask = -1;

	private bool inputEnabled = true;

	public bool areHoverEventsTracked = true;

	private tk2dUIItem pressedUIItem;

	private tk2dUIItem overUIItem;

	private tk2dUITouch firstPressedUIItemTouch;

	private bool checkForHovers = true;

	[SerializeField]
	private bool useMultiTouch;

	private const int MAX_MULTI_TOUCH_COUNT = 5;

	private tk2dUITouch[] allTouches = new tk2dUITouch[5];

	private List<tk2dUIItem> prevPressedUIItemList = new List<tk2dUIItem>();

	private tk2dUIItem[] pressedUIItems = new tk2dUIItem[5];

	private int touchCounter;

	private Vector2 mouseDownFirstPos = Vector2.zero;

	private const string MOUSE_WHEEL_AXES_NAME = "Mouse ScrollWheel";

	private tk2dUITouch primaryTouch = default(tk2dUITouch);

	private tk2dUITouch secondaryTouch = default(tk2dUITouch);

	private tk2dUITouch resultTouch = default(tk2dUITouch);

	private tk2dUIItem hitUIItem;

	private RaycastHit hit;

	private Ray ray;

	private tk2dUITouch currTouch;

	private tk2dUIItem currPressedItem;

	private tk2dUIItem prevPressedItem;

	public static tk2dUIManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = UnityEngine.Object.FindObjectOfType(typeof(tk2dUIManager)) as tk2dUIManager;
				if (instance == null)
				{
					GameObject gameObject = new GameObject("tk2dUIManager");
					instance = gameObject.AddComponent<tk2dUIManager>();
				}
			}
			return instance;
		}
	}

	public static tk2dUIManager Instance__NoCreate => instance;

	public Camera UICamera
	{
		get
		{
			return uiCamera;
		}
		set
		{
			uiCamera = value;
		}
	}

	public bool InputEnabled
	{
		get
		{
			return inputEnabled;
		}
		set
		{
			if (inputEnabled && !value)
			{
				SortCameras();
				inputEnabled = value;
				if (useMultiTouch)
				{
					CheckMultiTouchInputs();
				}
				else
				{
					CheckInputs();
				}
			}
			else
			{
				inputEnabled = value;
			}
		}
	}

	public tk2dUIItem PressedUIItem
	{
		get
		{
			if (useMultiTouch)
			{
				if (pressedUIItems.Length > 0)
				{
					return pressedUIItems[pressedUIItems.Length - 1];
				}
				return null;
			}
			return pressedUIItem;
		}
	}

	public tk2dUIItem[] PressedUIItems => pressedUIItems;

	public bool UseMultiTouch
	{
		get
		{
			return useMultiTouch;
		}
		set
		{
			if (useMultiTouch != value && inputEnabled)
			{
				InputEnabled = false;
				useMultiTouch = value;
				InputEnabled = true;
			}
			else
			{
				useMultiTouch = value;
			}
		}
	}

	public event Action OnAnyPress;

	public event Action OnInputUpdate;

	public event Action<float> OnScrollWheelChange;

	public Camera GetUICameraForControl(GameObject go)
	{
		int num = 1 << go.layer;
		int count = allCameras.Count;
		for (int i = 0; i < count; i++)
		{
			tk2dUICamera tk2dUICamera2 = allCameras[i];
			if (((int)tk2dUICamera2.FilteredMask & num) != 0)
			{
				return tk2dUICamera2.HostCamera;
			}
		}
		return null;
	}

	public static void RegisterCamera(tk2dUICamera cam)
	{
		allCameras.Add(cam);
	}

	public static void UnregisterCamera(tk2dUICamera cam)
	{
		allCameras.Remove(cam);
	}

	private void SortCameras()
	{
		sortedCameras.Clear();
		int count = allCameras.Count;
		for (int i = 0; i < count; i++)
		{
			tk2dUICamera tk2dUICamera2 = allCameras[i];
			if (tk2dUICamera2 != null)
			{
				sortedCameras.Add(tk2dUICamera2);
			}
		}
		sortedCameras.Sort((tk2dUICamera a, tk2dUICamera b) => b.GetComponent<Camera>().depth.CompareTo(a.GetComponent<Camera>().depth));
	}

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			if (instance.transform.childCount != 0)
			{
				DebugLogWrapper.LogError("You should not attach anything to the tk2dUIManager object. The tk2dUIManager will not get destroyed between scene switches and any children will persist as well.");
			}
			if (Application.isPlaying)
			{
				UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			}
		}
		else if (instance != this)
		{
			if (uiCamera != null)
			{
				HookUpLegacyCamera(uiCamera);
				uiCamera = null;
			}
			UnityEngine.Object.Destroy(this);
			return;
		}
		tk2dUITime.Init();
		Setup();
	}

	private void HookUpLegacyCamera(Camera cam)
	{
		if (cam.GetComponent<tk2dUICamera>() == null)
		{
			tk2dUICamera tk2dUICamera2 = cam.gameObject.AddComponent<tk2dUICamera>();
			tk2dUICamera2.AssignRaycastLayerMask(raycastLayerMask);
		}
	}

	private void Start()
	{
		if (uiCamera != null)
		{
			HookUpLegacyCamera(uiCamera);
			uiCamera = null;
		}
		if (allCameras.Count == 0)
		{
			DebugLogWrapper.LogError("Unable to find any tk2dUICameras, and no cameras are connected to the tk2dUIManager. You will not be able to interact with the UI.");
		}
	}

	private void Setup()
	{
		if (!areHoverEventsTracked)
		{
			checkForHovers = false;
		}
	}

	private void Update()
	{
		tk2dUITime.Update();
		if (inputEnabled)
		{
			SortCameras();
			if (useMultiTouch)
			{
				CheckMultiTouchInputs();
			}
			else
			{
				CheckInputs();
			}
			if (this.OnInputUpdate != null)
			{
				this.OnInputUpdate();
			}
			if (this.OnScrollWheelChange != null)
			{
				float axis = Input.GetAxis("Mouse ScrollWheel");
				if (axis != 0f)
				{
					this.OnScrollWheelChange(axis);
				}
			}
		}
		else if (this.OnInputUpdate != null)
		{
			this.OnInputUpdate();
		}
	}

	private void CheckInputs()
	{
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		primaryTouch = default(tk2dUITouch);
		secondaryTouch = default(tk2dUITouch);
		resultTouch = default(tk2dUITouch);
		hitUIItem = null;
		if (inputEnabled)
		{
			int touchCount = Input.touchCount;
			if (Input.touchCount > 0)
			{
				for (int i = 0; i < touchCount; i++)
				{
					Touch touch = Input.GetTouch(i);
					if (touch.phase == TouchPhase.Began)
					{
						primaryTouch = new tk2dUITouch(touch);
						flag = true;
						flag3 = true;
					}
					else if (pressedUIItem != null && touch.fingerId == firstPressedUIItemTouch.fingerId)
					{
						secondaryTouch = new tk2dUITouch(touch);
						flag2 = true;
					}
				}
				checkForHovers = false;
			}
			else if (Input.GetMouseButtonDown(0))
			{
				primaryTouch = new tk2dUITouch(TouchPhase.Began, 9999, Input.mousePosition, Vector2.zero, 0f);
				flag = true;
				flag3 = true;
			}
			else if (Input.GetMouseButton(0) || Input.GetMouseButtonUp(0))
			{
				Vector2 vector = Vector2.zero;
				TouchPhase phase = TouchPhase.Moved;
				if (pressedUIItem != null)
				{
					vector = firstPressedUIItemTouch.position - new Vector2(Input.mousePosition.x, Input.mousePosition.y);
				}
				if (Input.GetMouseButtonUp(0))
				{
					phase = TouchPhase.Ended;
				}
				else if (vector == Vector2.zero)
				{
					phase = TouchPhase.Stationary;
				}
				secondaryTouch = new tk2dUITouch(phase, 9999, Input.mousePosition, vector, tk2dUITime.deltaTime);
				flag2 = true;
			}
		}
		if (flag)
		{
			resultTouch = primaryTouch;
		}
		else if (flag2)
		{
			resultTouch = secondaryTouch;
		}
		if (flag || flag2)
		{
			hitUIItem = RaycastForUIItem(resultTouch.position);
			if (resultTouch.phase == TouchPhase.Began)
			{
				if (pressedUIItem != null)
				{
					pressedUIItem.CurrentOverUIItem(hitUIItem);
					if (pressedUIItem != hitUIItem)
					{
						pressedUIItem.Release();
						pressedUIItem = null;
					}
					else
					{
						firstPressedUIItemTouch = resultTouch;
					}
				}
				if (hitUIItem != null)
				{
					hitUIItem.Press(resultTouch);
				}
				pressedUIItem = hitUIItem;
				firstPressedUIItemTouch = resultTouch;
			}
			else if (resultTouch.phase == TouchPhase.Ended)
			{
				if (pressedUIItem != null)
				{
					pressedUIItem.CurrentOverUIItem(hitUIItem);
					pressedUIItem.UpdateTouch(resultTouch);
					pressedUIItem.Release();
					pressedUIItem = null;
				}
			}
			else if (pressedUIItem != null)
			{
				pressedUIItem.CurrentOverUIItem(hitUIItem);
				pressedUIItem.UpdateTouch(resultTouch);
			}
		}
		else if (pressedUIItem != null)
		{
			pressedUIItem.CurrentOverUIItem(null);
			pressedUIItem.Release();
			pressedUIItem = null;
		}
		if (checkForHovers)
		{
			if (inputEnabled)
			{
				if (!flag && !flag2 && hitUIItem == null && !Input.GetMouseButton(0))
				{
					hitUIItem = RaycastForUIItem(Input.mousePosition);
				}
				else if (Input.GetMouseButton(0))
				{
					hitUIItem = null;
				}
			}
			if (hitUIItem != null)
			{
				if (hitUIItem.isHoverEnabled)
				{
					if (!hitUIItem.HoverOver(overUIItem) && overUIItem != null)
					{
						overUIItem.HoverOut(hitUIItem);
					}
					overUIItem = hitUIItem;
				}
				else if (overUIItem != null)
				{
					overUIItem.HoverOut(null);
					overUIItem = null;
				}
			}
			else if (overUIItem != null)
			{
				overUIItem.HoverOut(null);
				overUIItem = null;
			}
		}
		if (flag3 && this.OnAnyPress != null)
		{
			this.OnAnyPress();
		}
	}

	public void SimulateAnyPress()
	{
		if (this.OnAnyPress != null)
		{
			this.OnAnyPress();
		}
	}

	public void ForceHoverOut()
	{
		if (overUIItem != null)
		{
			overUIItem.HoverOut(null);
			overUIItem = null;
		}
	}

	private void CheckMultiTouchInputs()
	{
		bool flag = false;
		int num = -1;
		bool flag2 = false;
		bool flag3 = false;
		touchCounter = 0;
		if (inputEnabled)
		{
			if (Input.touchCount > 0)
			{
				Touch[] touches = Input.touches;
				foreach (Touch touch in touches)
				{
					if (touchCounter < 5)
					{
						ref tk2dUITouch reference = ref allTouches[touchCounter];
						reference = new tk2dUITouch(touch);
						touchCounter++;
						continue;
					}
					break;
				}
			}
			else if (Input.GetMouseButtonDown(0))
			{
				ref tk2dUITouch reference2 = ref allTouches[touchCounter];
				reference2 = new tk2dUITouch(TouchPhase.Began, 9999, Input.mousePosition, Vector2.zero, 0f);
				mouseDownFirstPos = Input.mousePosition;
				touchCounter++;
			}
			else if (Input.GetMouseButton(0) || Input.GetMouseButtonUp(0))
			{
				Vector2 vector = mouseDownFirstPos - new Vector2(Input.mousePosition.x, Input.mousePosition.y);
				TouchPhase phase = TouchPhase.Moved;
				if (Input.GetMouseButtonUp(0))
				{
					phase = TouchPhase.Ended;
				}
				else if (vector == Vector2.zero)
				{
					phase = TouchPhase.Stationary;
				}
				ref tk2dUITouch reference3 = ref allTouches[touchCounter];
				reference3 = new tk2dUITouch(phase, 9999, Input.mousePosition, vector, tk2dUITime.deltaTime);
				touchCounter++;
			}
		}
		for (int j = 0; j < touchCounter; j++)
		{
			pressedUIItems[j] = RaycastForUIItem(allTouches[j].position);
		}
		for (int k = 0; k < prevPressedUIItemList.Count; k++)
		{
			prevPressedItem = prevPressedUIItemList[k];
			if (!(prevPressedItem != null))
			{
				continue;
			}
			num = prevPressedItem.Touch.fingerId;
			flag2 = false;
			for (int l = 0; l < touchCounter; l++)
			{
				currTouch = allTouches[l];
				if (currTouch.fingerId != num)
				{
					continue;
				}
				flag2 = true;
				currPressedItem = pressedUIItems[l];
				if (currTouch.phase == TouchPhase.Began)
				{
					prevPressedItem.CurrentOverUIItem(currPressedItem);
					if (prevPressedItem != currPressedItem)
					{
						prevPressedItem.Release();
						prevPressedUIItemList.RemoveAt(k);
						k--;
					}
				}
				else if (currTouch.phase == TouchPhase.Ended)
				{
					prevPressedItem.CurrentOverUIItem(currPressedItem);
					prevPressedItem.UpdateTouch(currTouch);
					prevPressedItem.Release();
					prevPressedUIItemList.RemoveAt(k);
					k--;
				}
				else
				{
					prevPressedItem.CurrentOverUIItem(currPressedItem);
					prevPressedItem.UpdateTouch(currTouch);
				}
				break;
			}
			if (!flag2)
			{
				prevPressedItem.CurrentOverUIItem(null);
				prevPressedItem.Release();
				prevPressedUIItemList.RemoveAt(k);
				k--;
			}
		}
		for (int m = 0; m < touchCounter; m++)
		{
			currPressedItem = pressedUIItems[m];
			currTouch = allTouches[m];
			if (currTouch.phase == TouchPhase.Began)
			{
				if (currPressedItem != null && currPressedItem.Press(currTouch))
				{
					prevPressedUIItemList.Add(currPressedItem);
				}
				flag = true;
			}
		}
		if (flag && this.OnAnyPress != null)
		{
			this.OnAnyPress();
		}
	}

	private tk2dUIItem RaycastForUIItem(Vector2 screenPos)
	{
		int count = sortedCameras.Count;
		for (int i = 0; i < count; i++)
		{
			tk2dUICamera tk2dUICamera2 = sortedCameras[i];
			if (tk2dUICamera2.RaycastType == tk2dUICamera.tk2dRaycastType.Physics3D)
			{
				ray = tk2dUICamera2.HostCamera.ScreenPointToRay(screenPos);
				if (Physics.Raycast(ray, out hit, tk2dUICamera2.HostCamera.farClipPlane - tk2dUICamera2.HostCamera.nearClipPlane, tk2dUICamera2.FilteredMask))
				{
					return hit.collider.GetComponent<tk2dUIItem>();
				}
			}
			else if (tk2dUICamera2.RaycastType == tk2dUICamera.tk2dRaycastType.Physics2D)
			{
				Collider2D collider2D = Physics2D.OverlapPoint(tk2dUICamera2.HostCamera.ScreenToWorldPoint(screenPos), tk2dUICamera2.FilteredMask);
				if (collider2D != null)
				{
					return collider2D.GetComponent<tk2dUIItem>();
				}
			}
		}
		return null;
	}

	public void OverrideClearAllChildrenPresses(tk2dUIItem item)
	{
		if (useMultiTouch)
		{
			for (int i = 0; i < pressedUIItems.Length; i++)
			{
				tk2dUIItem tk2dUIItem2 = pressedUIItems[i];
				if (tk2dUIItem2 != null && item.CheckIsUIItemChildOfMe(tk2dUIItem2))
				{
					tk2dUIItem2.CurrentOverUIItem(item);
				}
			}
		}
		else if (pressedUIItem != null && item.CheckIsUIItemChildOfMe(pressedUIItem))
		{
			pressedUIItem.CurrentOverUIItem(item);
		}
	}
}
