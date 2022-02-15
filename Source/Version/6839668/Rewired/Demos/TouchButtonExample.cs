using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Rewired.Demos;

[AddComponentMenu("")]
[RequireComponent(typeof(Image))]
public class TouchButtonExample : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler
{
	public bool allowMouseControl = true;

	public bool isPressed { get; private set; }

	private void Awake()
	{
		if (SystemInfo.deviceType == DeviceType.Handheld)
		{
			allowMouseControl = false;
		}
	}

	private void Restart()
	{
		isPressed = false;
	}

	void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
	{
		if (allowMouseControl || !IsMousePointerId(eventData.pointerId))
		{
			isPressed = true;
		}
	}

	void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
	{
		if (allowMouseControl || !IsMousePointerId(eventData.pointerId))
		{
			isPressed = false;
		}
	}

	private static bool IsMousePointerId(int id)
	{
		return id == -1 || id == -2 || id == -3;
	}
}
