using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("2D Toolkit/UI/tk2dUIScrollableArea")]
public class tk2dUIScrollableArea : MonoBehaviour
{
	public enum Axes
	{
		XAxis,
		YAxis
	}

	[SerializeField]
	private float contentLength = 1f;

	[SerializeField]
	private float visibleAreaLength = 1f;

	public GameObject contentContainer;

	public tk2dUIScrollbar scrollBar;

	public tk2dUIItem backgroundUIItem;

	public Axes scrollAxes = Axes.YAxis;

	public bool allowSwipeScrolling = true;

	public bool allowScrollWheel = true;

	[SerializeField]
	[HideInInspector]
	private tk2dUILayout backgroundLayoutItem;

	[SerializeField]
	[HideInInspector]
	private tk2dUILayoutContainer contentLayoutContainer;

	private bool isBackgroundButtonDown;

	private bool isBackgroundButtonOver;

	private Vector3 swipeScrollingPressDownStartLocalPos = Vector3.zero;

	private Vector3 swipeScrollingContentStartLocalPos = Vector3.zero;

	private Vector3 swipeScrollingContentDestLocalPos = Vector3.zero;

	private bool isSwipeScrollingInProgress;

	private const float SWIPE_SCROLLING_FIRST_SCROLL_THRESHOLD = 0.02f;

	private const float WITHOUT_SCROLLBAR_FIXED_SCROLL_WHEEL_PERCENT = 0.1f;

	private Vector3 swipePrevScrollingContentPressLocalPos = Vector3.zero;

	private float swipeCurrVelocity;

	private float snapBackVelocity;

	public string SendMessageOnScrollMethodName = string.Empty;

	private float percent;

	private static readonly Vector3[] boxExtents = new Vector3[8]
	{
		new Vector3(-1f, -1f, -1f),
		new Vector3(1f, -1f, -1f),
		new Vector3(-1f, 1f, -1f),
		new Vector3(1f, 1f, -1f),
		new Vector3(-1f, -1f, 1f),
		new Vector3(1f, -1f, 1f),
		new Vector3(-1f, 1f, 1f),
		new Vector3(1f, 1f, 1f)
	};

	public float ContentLength
	{
		get
		{
			return contentLength;
		}
		set
		{
			ContentLengthVisibleAreaLengthChange(contentLength, value, visibleAreaLength, visibleAreaLength);
		}
	}

	public float VisibleAreaLength
	{
		get
		{
			return visibleAreaLength;
		}
		set
		{
			ContentLengthVisibleAreaLengthChange(contentLength, contentLength, visibleAreaLength, value);
		}
	}

	public tk2dUILayout BackgroundLayoutItem
	{
		get
		{
			return backgroundLayoutItem;
		}
		set
		{
			if (backgroundLayoutItem != value)
			{
				if (backgroundLayoutItem != null)
				{
					backgroundLayoutItem.OnReshape -= LayoutReshaped;
				}
				backgroundLayoutItem = value;
				if (backgroundLayoutItem != null)
				{
					backgroundLayoutItem.OnReshape += LayoutReshaped;
				}
			}
		}
	}

	public tk2dUILayoutContainer ContentLayoutContainer
	{
		get
		{
			return contentLayoutContainer;
		}
		set
		{
			if (contentLayoutContainer != value)
			{
				if (contentLayoutContainer != null)
				{
					contentLayoutContainer.OnChangeContent -= ContentLayoutChangeCallback;
				}
				contentLayoutContainer = value;
				if (contentLayoutContainer != null)
				{
					contentLayoutContainer.OnChangeContent += ContentLayoutChangeCallback;
				}
			}
		}
	}

	public GameObject SendMessageTarget
	{
		get
		{
			if (backgroundUIItem != null)
			{
				return backgroundUIItem.sendMessageTarget;
			}
			return null;
		}
		set
		{
			if (backgroundUIItem != null && backgroundUIItem.sendMessageTarget != value)
			{
				backgroundUIItem.sendMessageTarget = value;
			}
		}
	}

	public float Value
	{
		get
		{
			return Mathf.Clamp01(percent);
		}
		set
		{
			value = Mathf.Clamp(value, 0f, 1f);
			if (value != percent)
			{
				UnpressAllUIItemChildren();
				percent = value;
				if (this.OnScroll != null)
				{
					this.OnScroll(this);
				}
				if (isBackgroundButtonDown || isSwipeScrollingInProgress)
				{
					if (tk2dUIManager.Instance__NoCreate != null)
					{
						tk2dUIManager.Instance.OnInputUpdate -= BackgroundOverUpdate;
					}
					isBackgroundButtonDown = false;
					isSwipeScrollingInProgress = false;
				}
				TargetOnScrollCallback();
			}
			if (scrollBar != null)
			{
				scrollBar.SetScrollPercentWithoutEvent(percent);
			}
			SetContentPosition();
		}
	}

	private Vector3 ContentContainerOffset
	{
		get
		{
			return Vector3.Scale(new Vector3(-1f, 1f, 1f), contentContainer.transform.localPosition);
		}
		set
		{
			contentContainer.transform.localPosition = Vector3.Scale(new Vector3(-1f, 1f, 1f), value);
		}
	}

	public event Action<tk2dUIScrollableArea> OnScroll;

	public void SetScrollPercentWithoutEvent(float newScrollPercent)
	{
		percent = Mathf.Clamp(newScrollPercent, 0f, 1f);
		UnpressAllUIItemChildren();
		if (scrollBar != null)
		{
			scrollBar.SetScrollPercentWithoutEvent(percent);
		}
		SetContentPosition();
	}

	public float MeasureContentLength()
	{
		Vector3 vector = new Vector3(float.MinValue, float.MinValue, float.MinValue);
		Vector3 vector2 = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
		Vector3[] array = new Vector3[2] { vector2, vector };
		Transform transform = contentContainer.transform;
		GetRendererBoundsInChildren(transform.worldToLocalMatrix, array, transform);
		if (array[0] != vector2 && array[1] != vector)
		{
			ref Vector3 reference = ref array[0];
			reference = Vector3.Min(array[0], Vector3.zero);
			ref Vector3 reference2 = ref array[1];
			reference2 = Vector3.Max(array[1], Vector3.zero);
			return (scrollAxes != Axes.YAxis) ? (array[1].x - array[0].x) : (array[1].y - array[0].y);
		}
		DebugLogWrapper.LogError("Unable to measure content length");
		return VisibleAreaLength * 0.9f;
	}

	private void OnEnable()
	{
		if (scrollBar != null)
		{
			scrollBar.OnScroll += ScrollBarMove;
		}
		if (backgroundUIItem != null)
		{
			backgroundUIItem.OnDown += BackgroundButtonDown;
			backgroundUIItem.OnRelease += BackgroundButtonRelease;
			backgroundUIItem.OnHoverOver += BackgroundButtonHoverOver;
			backgroundUIItem.OnHoverOut += BackgroundButtonHoverOut;
		}
		if (backgroundLayoutItem != null)
		{
			backgroundLayoutItem.OnReshape += LayoutReshaped;
		}
		if (contentLayoutContainer != null)
		{
			contentLayoutContainer.OnChangeContent += ContentLayoutChangeCallback;
		}
	}

	private void OnDisable()
	{
		if (scrollBar != null)
		{
			scrollBar.OnScroll -= ScrollBarMove;
		}
		if (backgroundUIItem != null)
		{
			backgroundUIItem.OnDown -= BackgroundButtonDown;
			backgroundUIItem.OnRelease -= BackgroundButtonRelease;
			backgroundUIItem.OnHoverOver -= BackgroundButtonHoverOver;
			backgroundUIItem.OnHoverOut -= BackgroundButtonHoverOut;
		}
		if (isBackgroundButtonOver)
		{
			if (tk2dUIManager.Instance__NoCreate != null)
			{
				tk2dUIManager.Instance.OnScrollWheelChange -= BackgroundHoverOverScrollWheelChange;
			}
			isBackgroundButtonOver = false;
		}
		if (isBackgroundButtonDown || isSwipeScrollingInProgress)
		{
			if (tk2dUIManager.Instance__NoCreate != null)
			{
				tk2dUIManager.Instance.OnInputUpdate -= BackgroundOverUpdate;
			}
			isBackgroundButtonDown = false;
			isSwipeScrollingInProgress = false;
		}
		if (backgroundLayoutItem != null)
		{
			backgroundLayoutItem.OnReshape -= LayoutReshaped;
		}
		if (contentLayoutContainer != null)
		{
			contentLayoutContainer.OnChangeContent -= ContentLayoutChangeCallback;
		}
		swipeCurrVelocity = 0f;
	}

	private void Start()
	{
		UpdateScrollbarActiveState();
	}

	private void BackgroundHoverOverScrollWheelChange(float mouseWheelChange)
	{
		if (mouseWheelChange > 0f)
		{
			if ((bool)scrollBar)
			{
				scrollBar.ScrollUpFixed();
			}
			else
			{
				Value -= 0.1f;
			}
		}
		else if (mouseWheelChange < 0f)
		{
			if ((bool)scrollBar)
			{
				scrollBar.ScrollDownFixed();
			}
			else
			{
				Value += 0.1f;
			}
		}
	}

	private void ScrollBarMove(tk2dUIScrollbar scrollBar)
	{
		Value = scrollBar.Value;
		isSwipeScrollingInProgress = false;
		if (isBackgroundButtonDown)
		{
			BackgroundButtonRelease();
		}
	}

	private void SetContentPosition()
	{
		Vector3 contentContainerOffset = ContentContainerOffset;
		float num = (contentLength - visibleAreaLength) * Value;
		if (num < 0f)
		{
			num = 0f;
		}
		if (scrollAxes == Axes.XAxis)
		{
			contentContainerOffset.x = num;
		}
		else if (scrollAxes == Axes.YAxis)
		{
			contentContainerOffset.y = num;
		}
		ContentContainerOffset = contentContainerOffset;
	}

	private void BackgroundButtonDown()
	{
		if (allowSwipeScrolling && contentLength > visibleAreaLength)
		{
			if (!isBackgroundButtonDown && !isSwipeScrollingInProgress)
			{
				tk2dUIManager.Instance.OnInputUpdate += BackgroundOverUpdate;
			}
			swipeScrollingPressDownStartLocalPos = base.transform.InverseTransformPoint(CalculateClickWorldPos(backgroundUIItem));
			swipePrevScrollingContentPressLocalPos = swipeScrollingPressDownStartLocalPos;
			swipeScrollingContentStartLocalPos = ContentContainerOffset;
			swipeScrollingContentDestLocalPos = swipeScrollingContentStartLocalPos;
			isBackgroundButtonDown = true;
			swipeCurrVelocity = 0f;
		}
	}

	private void BackgroundOverUpdate()
	{
		if (isBackgroundButtonDown)
		{
			UpdateSwipeScrollDestintationPosition();
		}
		if (!isSwipeScrollingInProgress)
		{
			return;
		}
		float num = percent;
		float num2 = 0f;
		if (scrollAxes == Axes.XAxis)
		{
			num2 = swipeScrollingContentDestLocalPos.x;
		}
		else if (scrollAxes == Axes.YAxis)
		{
			num2 = swipeScrollingContentDestLocalPos.y;
		}
		float num3 = 0f;
		float num4 = contentLength - visibleAreaLength;
		if (isBackgroundButtonDown)
		{
			if (num2 < num3)
			{
				num2 += (0f - num2) / visibleAreaLength / 2f;
				if (num2 > num3)
				{
					num2 = num3;
				}
			}
			else if (num2 > num4)
			{
				num2 -= (num2 - num4) / visibleAreaLength / 2f;
				if (num2 < num4)
				{
					num2 = num4;
				}
			}
			if (scrollAxes == Axes.XAxis)
			{
				swipeScrollingContentDestLocalPos.x = num2;
			}
			else if (scrollAxes == Axes.YAxis)
			{
				swipeScrollingContentDestLocalPos.y = num2;
			}
			num = ((!(contentLength - visibleAreaLength > Mathf.Epsilon)) ? 0f : (num2 / (contentLength - visibleAreaLength)));
		}
		else
		{
			float num5 = visibleAreaLength * 0.001f;
			if (num2 < num3 || num2 > num4)
			{
				float num6 = ((!(num2 < num3)) ? num4 : num3);
				num2 = Mathf.SmoothDamp(num2, num6, ref snapBackVelocity, 0.05f, float.PositiveInfinity, tk2dUITime.deltaTime);
				if (Mathf.Abs(snapBackVelocity) < num5)
				{
					num2 = num6;
					snapBackVelocity = 0f;
				}
				swipeCurrVelocity = 0f;
			}
			else if (swipeCurrVelocity != 0f)
			{
				num2 += swipeCurrVelocity * tk2dUITime.deltaTime * 20f;
				if (swipeCurrVelocity > num5 || swipeCurrVelocity < 0f - num5)
				{
					swipeCurrVelocity = Mathf.Lerp(swipeCurrVelocity, 0f, tk2dUITime.deltaTime * 2.5f);
				}
				else
				{
					swipeCurrVelocity = 0f;
				}
			}
			else
			{
				isSwipeScrollingInProgress = false;
				tk2dUIManager.Instance.OnInputUpdate -= BackgroundOverUpdate;
			}
			if (scrollAxes == Axes.XAxis)
			{
				swipeScrollingContentDestLocalPos.x = num2;
			}
			else if (scrollAxes == Axes.YAxis)
			{
				swipeScrollingContentDestLocalPos.y = num2;
			}
			num = num2 / (contentLength - visibleAreaLength);
		}
		if (num != percent)
		{
			percent = num;
			ContentContainerOffset = swipeScrollingContentDestLocalPos;
			if (this.OnScroll != null)
			{
				this.OnScroll(this);
			}
			TargetOnScrollCallback();
		}
		if (scrollBar != null)
		{
			float scrollPercentWithoutEvent = percent;
			if (scrollAxes == Axes.XAxis)
			{
				scrollPercentWithoutEvent = ContentContainerOffset.x / (contentLength - visibleAreaLength);
			}
			else if (scrollAxes == Axes.YAxis)
			{
				scrollPercentWithoutEvent = ContentContainerOffset.y / (contentLength - visibleAreaLength);
			}
			scrollBar.SetScrollPercentWithoutEvent(scrollPercentWithoutEvent);
		}
	}

	private void UpdateSwipeScrollDestintationPosition()
	{
		Vector3 vector = base.transform.InverseTransformPoint(CalculateClickWorldPos(backgroundUIItem));
		Vector3 vector2 = vector - swipeScrollingPressDownStartLocalPos;
		vector2.x *= -1f;
		float f = 0f;
		if (scrollAxes == Axes.XAxis)
		{
			f = vector2.x;
			swipeCurrVelocity = 0f - (vector.x - swipePrevScrollingContentPressLocalPos.x);
		}
		else if (scrollAxes == Axes.YAxis)
		{
			f = vector2.y;
			swipeCurrVelocity = vector.y - swipePrevScrollingContentPressLocalPos.y;
		}
		if (!isSwipeScrollingInProgress && Mathf.Abs(f) > 0.02f)
		{
			isSwipeScrollingInProgress = true;
			tk2dUIManager.Instance.OverrideClearAllChildrenPresses(backgroundUIItem);
		}
		if (isSwipeScrollingInProgress)
		{
			Vector3 vector3 = swipeScrollingContentStartLocalPos + vector2;
			vector3.z = ContentContainerOffset.z;
			if (scrollAxes == Axes.XAxis)
			{
				vector3.y = ContentContainerOffset.y;
			}
			else if (scrollAxes == Axes.YAxis)
			{
				vector3.x = ContentContainerOffset.x;
			}
			vector3.z = ContentContainerOffset.z;
			swipeScrollingContentDestLocalPos = vector3;
			swipePrevScrollingContentPressLocalPos = vector;
		}
	}

	private void BackgroundButtonRelease()
	{
		if (allowSwipeScrolling)
		{
			if (isBackgroundButtonDown && !isSwipeScrollingInProgress)
			{
				tk2dUIManager.Instance.OnInputUpdate -= BackgroundOverUpdate;
			}
			isBackgroundButtonDown = false;
		}
	}

	private void BackgroundButtonHoverOver()
	{
		if (allowScrollWheel)
		{
			if (!isBackgroundButtonOver)
			{
				tk2dUIManager.Instance.OnScrollWheelChange += BackgroundHoverOverScrollWheelChange;
			}
			isBackgroundButtonOver = true;
		}
	}

	private void BackgroundButtonHoverOut()
	{
		if (isBackgroundButtonOver)
		{
			tk2dUIManager.Instance.OnScrollWheelChange -= BackgroundHoverOverScrollWheelChange;
		}
		isBackgroundButtonOver = false;
	}

	private Vector3 CalculateClickWorldPos(tk2dUIItem btn)
	{
		Vector2 position = btn.Touch.position;
		Camera uICameraForControl = tk2dUIManager.Instance.GetUICameraForControl(base.gameObject);
		Vector3 result = uICameraForControl.ScreenToWorldPoint(new Vector3(position.x, position.y, btn.transform.position.z - uICameraForControl.transform.position.z));
		result.z = btn.transform.position.z;
		return result;
	}

	private void UpdateScrollbarActiveState()
	{
		bool flag = contentLength > visibleAreaLength;
		if (scrollBar != null && scrollBar.gameObject.activeSelf != flag)
		{
			tk2dUIBaseItemControl.ChangeGameObjectActiveState(scrollBar.gameObject, flag);
		}
	}

	private void ContentLengthVisibleAreaLengthChange(float prevContentLength, float newContentLength, float prevVisibleAreaLength, float newVisibleAreaLength)
	{
		float value = ((newContentLength - visibleAreaLength == 0f) ? 0f : ((prevContentLength - prevVisibleAreaLength) * Value / (newContentLength - newVisibleAreaLength)));
		contentLength = newContentLength;
		visibleAreaLength = newVisibleAreaLength;
		UpdateScrollbarActiveState();
		Value = value;
	}

	private void UnpressAllUIItemChildren()
	{
	}

	private void TargetOnScrollCallback()
	{
		if (SendMessageTarget != null && SendMessageOnScrollMethodName.Length > 0)
		{
			SendMessageTarget.SendMessage(SendMessageOnScrollMethodName, this, SendMessageOptions.RequireReceiver);
		}
	}

	private static void GetRendererBoundsInChildren(Matrix4x4 rootWorldToLocal, Vector3[] minMax, Transform t)
	{
		MeshFilter component = t.GetComponent<MeshFilter>();
		if (component != null && component.sharedMesh != null)
		{
			Bounds bounds = component.sharedMesh.bounds;
			Matrix4x4 matrix4x = rootWorldToLocal * t.localToWorldMatrix;
			for (int i = 0; i < 8; i++)
			{
				Vector3 v = bounds.center + Vector3.Scale(bounds.extents, boxExtents[i]);
				Vector3 rhs = matrix4x.MultiplyPoint(v);
				ref Vector3 reference = ref minMax[0];
				reference = Vector3.Min(minMax[0], rhs);
				ref Vector3 reference2 = ref minMax[1];
				reference2 = Vector3.Max(minMax[1], rhs);
			}
		}
		int childCount = t.childCount;
		for (int j = 0; j < childCount; j++)
		{
			Transform child = t.GetChild(j);
			if (t.gameObject.activeSelf)
			{
				GetRendererBoundsInChildren(rootWorldToLocal, minMax, child);
			}
		}
	}

	private void LayoutReshaped(Vector3 dMin, Vector3 dMax)
	{
		VisibleAreaLength += ((scrollAxes != 0) ? (dMax.y - dMin.y) : (dMax.x - dMin.x));
	}

	private void ContentLayoutChangeCallback()
	{
		if (contentLayoutContainer != null)
		{
			Vector2 innerSize = contentLayoutContainer.GetInnerSize();
			ContentLength = ((scrollAxes != 0) ? innerSize.y : innerSize.x);
		}
	}
}
