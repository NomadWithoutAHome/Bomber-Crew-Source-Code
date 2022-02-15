using UnityEngine;

[AddComponentMenu("2D Toolkit/UI/tk2dUIBaseItemControl")]
public abstract class tk2dUIBaseItemControl : MonoBehaviour
{
	public tk2dUIItem uiItem;

	public GameObject SendMessageTarget
	{
		get
		{
			if (uiItem != null)
			{
				return uiItem.sendMessageTarget;
			}
			return null;
		}
		set
		{
			if (uiItem != null)
			{
				uiItem.sendMessageTarget = value;
			}
		}
	}

	public static void ChangeGameObjectActiveState(GameObject go, bool isActive)
	{
		go.SetActive(isActive);
	}

	public static void ChangeGameObjectActiveStateWithNullCheck(GameObject go, bool isActive)
	{
		if (go != null)
		{
			ChangeGameObjectActiveState(go, isActive);
		}
	}

	protected void DoSendMessage(string methodName, object parameter)
	{
		if (SendMessageTarget != null && methodName.Length > 0)
		{
			SendMessageTarget.SendMessage(methodName, parameter, SendMessageOptions.RequireReceiver);
		}
	}
}
