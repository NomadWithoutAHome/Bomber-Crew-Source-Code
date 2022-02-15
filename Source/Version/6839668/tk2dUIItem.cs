using System;
using UnityEngine;

[AddComponentMenu("2D Toolkit/UI/Core/tk2dUIItem")]
public class tk2dUIItem : MonoBehaviour
{
	public GameObject sendMessageTarget;

	public string SendMessageOnDownMethodName = string.Empty;

	public string SendMessageOnUpMethodName = string.Empty;

	public string SendMessageOnClickMethodName = string.Empty;

	public string SendMessageOnReleaseMethodName = string.Empty;

	[SerializeField]
	private bool isChildOfAnotherUIItem;

	public bool registerPressFromChildren;

	public bool isHoverEnabled;

	public Transform[] editorExtraBounds = new Transform[0];

	public Transform[] editorIgnoreBounds = new Transform[0];

	private bool isPressed;

	private bool isHoverOver;

	private tk2dUITouch touch;

	private tk2dUIItem parentUIItem;

	public bool IsPressed => isPressed;

	public tk2dUITouch Touch => touch;

	public tk2dUIItem ParentUIItem => parentUIItem;

	public event Action OnDown;

	public event Action OnUp;

	public event Action OnClick;

	public event Action OnRelease;

	public event Action OnHoverOver;

	public event Action OnHoverOut;

	public event Action<tk2dUIItem> OnDownUIItem;

	public event Action<tk2dUIItem> OnUpUIItem;

	public event Action<tk2dUIItem> OnClickUIItem;

	public event Action<tk2dUIItem> OnReleaseUIItem;

	public event Action<tk2dUIItem> OnHoverOverUIItem;

	public event Action<tk2dUIItem> OnHoverOutUIItem;

	private void Awake()
	{
		if (isChildOfAnotherUIItem)
		{
			UpdateParent();
		}
	}

	private void Start()
	{
		if (tk2dUIManager.Instance == null)
		{
			DebugLogWrapper.LogError("Unable to find tk2dUIManager. Please create a tk2dUIManager in the scene before proceeding.");
		}
		if (isChildOfAnotherUIItem && parentUIItem == null)
		{
			UpdateParent();
		}
	}

	public void UpdateParent()
	{
		parentUIItem = GetParentUIItem();
	}

	public void ManuallySetParent(tk2dUIItem newParentUIItem)
	{
		parentUIItem = newParentUIItem;
	}

	public void RemoveParent()
	{
		parentUIItem = null;
	}

	public bool Press(tk2dUITouch touch)
	{
		return Press(touch, null);
	}

	public bool Press(tk2dUITouch touch, tk2dUIItem sentFromChild)
	{
		if (isPressed)
		{
			return false;
		}
		if (!isPressed)
		{
			this.touch = touch;
			if ((registerPressFromChildren || sentFromChild == null) && base.enabled)
			{
				isPressed = true;
				if (this.OnDown != null)
				{
					this.OnDown();
				}
				if (this.OnDownUIItem != null)
				{
					this.OnDownUIItem(this);
				}
				DoSendMessage(SendMessageOnDownMethodName);
			}
			if (parentUIItem != null)
			{
				parentUIItem.Press(touch, this);
			}
		}
		return true;
	}

	public void UpdateTouch(tk2dUITouch touch)
	{
		this.touch = touch;
		if (parentUIItem != null)
		{
			parentUIItem.UpdateTouch(touch);
		}
	}

	private void DoSendMessage(string methodName)
	{
		if (sendMessageTarget != null && methodName.Length > 0)
		{
			sendMessageTarget.SendMessage(methodName, this, SendMessageOptions.RequireReceiver);
		}
	}

	public void Release()
	{
		if (isPressed)
		{
			isPressed = false;
			if (this.OnUp != null)
			{
				this.OnUp();
			}
			if (this.OnUpUIItem != null)
			{
				this.OnUpUIItem(this);
			}
			DoSendMessage(SendMessageOnUpMethodName);
			if (this.OnClick != null)
			{
				this.OnClick();
			}
			if (this.OnClickUIItem != null)
			{
				this.OnClickUIItem(this);
			}
			DoSendMessage(SendMessageOnClickMethodName);
		}
		if (this.OnRelease != null)
		{
			this.OnRelease();
		}
		if (this.OnReleaseUIItem != null)
		{
			this.OnReleaseUIItem(this);
		}
		DoSendMessage(SendMessageOnReleaseMethodName);
		if (parentUIItem != null)
		{
			parentUIItem.Release();
		}
	}

	public void CurrentOverUIItem(tk2dUIItem overUIItem)
	{
		if (!(overUIItem != this))
		{
			return;
		}
		if (isPressed)
		{
			if (!CheckIsUIItemChildOfMe(overUIItem))
			{
				Exit();
				if (parentUIItem != null)
				{
					parentUIItem.CurrentOverUIItem(overUIItem);
				}
			}
		}
		else if (parentUIItem != null)
		{
			parentUIItem.CurrentOverUIItem(overUIItem);
		}
	}

	public bool CheckIsUIItemChildOfMe(tk2dUIItem uiItem)
	{
		tk2dUIItem tk2dUIItem2 = null;
		bool result = false;
		if (uiItem != null)
		{
			tk2dUIItem2 = uiItem.parentUIItem;
		}
		while (tk2dUIItem2 != null)
		{
			if (tk2dUIItem2 == this)
			{
				result = true;
				break;
			}
			tk2dUIItem2 = tk2dUIItem2.parentUIItem;
		}
		return result;
	}

	public void Exit()
	{
		if (isPressed)
		{
			isPressed = false;
			if (this.OnUp != null)
			{
				this.OnUp();
			}
			if (this.OnUpUIItem != null)
			{
				this.OnUpUIItem(this);
			}
			DoSendMessage(SendMessageOnUpMethodName);
		}
	}

	public bool HoverOver(tk2dUIItem prevHover)
	{
		bool flag = false;
		tk2dUIItem tk2dUIItem2 = null;
		if (!isHoverOver)
		{
			if (this.OnHoverOver != null)
			{
				this.OnHoverOver();
			}
			if (this.OnHoverOverUIItem != null)
			{
				this.OnHoverOverUIItem(this);
			}
			isHoverOver = true;
		}
		if (prevHover == this)
		{
			flag = true;
		}
		if (parentUIItem != null && parentUIItem.isHoverEnabled)
		{
			tk2dUIItem2 = parentUIItem;
		}
		if (tk2dUIItem2 == null)
		{
			return flag;
		}
		return tk2dUIItem2.HoverOver(prevHover) || flag;
	}

	public void HoverOut(tk2dUIItem currHoverButton)
	{
		if (isHoverOver)
		{
			if (this.OnHoverOut != null)
			{
				this.OnHoverOut();
			}
			if (this.OnHoverOutUIItem != null)
			{
				this.OnHoverOutUIItem(this);
			}
			isHoverOver = false;
		}
		if (parentUIItem != null && parentUIItem.isHoverEnabled)
		{
			if (currHoverButton == null)
			{
				parentUIItem.HoverOut(currHoverButton);
			}
			else if (!parentUIItem.CheckIsUIItemChildOfMe(currHoverButton) && currHoverButton != parentUIItem)
			{
				parentUIItem.HoverOut(currHoverButton);
			}
		}
	}

	private tk2dUIItem GetParentUIItem()
	{
		Transform parent = base.transform.parent;
		while (parent != null)
		{
			tk2dUIItem component = parent.GetComponent<tk2dUIItem>();
			if (component != null)
			{
				return component;
			}
			parent = parent.parent;
		}
		return null;
	}

	public void SimulateClick()
	{
		if (this.OnDown != null)
		{
			this.OnDown();
		}
		if (this.OnDownUIItem != null)
		{
			this.OnDownUIItem(this);
		}
		DoSendMessage(SendMessageOnDownMethodName);
		if (this.OnUp != null)
		{
			this.OnUp();
		}
		if (this.OnUpUIItem != null)
		{
			this.OnUpUIItem(this);
		}
		DoSendMessage(SendMessageOnUpMethodName);
		if (this.OnClick != null)
		{
			this.OnClick();
		}
		if (this.OnClickUIItem != null)
		{
			this.OnClickUIItem(this);
		}
		DoSendMessage(SendMessageOnClickMethodName);
		if (this.OnRelease != null)
		{
			this.OnRelease();
		}
		if (this.OnReleaseUIItem != null)
		{
			this.OnReleaseUIItem(this);
		}
		DoSendMessage(SendMessageOnReleaseMethodName);
	}

	public void SimulateDown()
	{
		if (this.OnDown != null)
		{
			this.OnDown();
		}
		if (this.OnDownUIItem != null)
		{
			this.OnDownUIItem(this);
		}
		DoSendMessage(SendMessageOnDownMethodName);
		tk2dUIManager.Instance.SimulateAnyPress();
	}

	public void SimulateUpClick()
	{
		if (this.OnUp != null)
		{
			this.OnUp();
		}
		if (this.OnUpUIItem != null)
		{
			this.OnUpUIItem(this);
		}
		DoSendMessage(SendMessageOnUpMethodName);
		if (this.OnClick != null)
		{
			this.OnClick();
		}
		if (this.OnClickUIItem != null)
		{
			this.OnClickUIItem(this);
		}
		DoSendMessage(SendMessageOnClickMethodName);
		if (this.OnRelease != null)
		{
			this.OnRelease();
		}
		if (this.OnReleaseUIItem != null)
		{
			this.OnReleaseUIItem(this);
		}
		DoSendMessage(SendMessageOnReleaseMethodName);
	}

	public void SimulateRelease()
	{
		if (this.OnRelease != null)
		{
			this.OnRelease();
		}
		if (this.OnReleaseUIItem != null)
		{
			this.OnReleaseUIItem(this);
		}
		DoSendMessage(SendMessageOnReleaseMethodName);
	}

	public void InternalSetIsChildOfAnotherUIItem(bool state)
	{
		isChildOfAnotherUIItem = state;
	}

	public bool InternalGetIsChildOfAnotherUIItem()
	{
		return isChildOfAnotherUIItem;
	}
}
