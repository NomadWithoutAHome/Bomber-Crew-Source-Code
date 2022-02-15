using UnityEngine;

namespace BomberCrewCommon;

public class PopupLockColliderController : MonoBehaviour
{
	[SerializeField]
	private bool m_debug;

	private bool m_wasLocked;

	private void Awake()
	{
		m_wasLocked = false;
		if (Singleton<UIPopupManager>.Instance != null)
		{
			Singleton<UIPopupManager>.Instance.PopupWasShown += HandlePopupWasShown;
			Singleton<UIPopupManager>.Instance.PopupWasDismissed += HandlePopupWasDismissed;
		}
	}

	private void OnDestroy()
	{
		if (Singleton<UIPopupManager>.Instance != null)
		{
			Singleton<UIPopupManager>.Instance.PopupWasShown -= HandlePopupWasShown;
			Singleton<UIPopupManager>.Instance.PopupWasDismissed -= HandlePopupWasDismissed;
		}
	}

	private void HandlePopupWasShown(UIPopUp popup)
	{
		LockInput(locked: true);
	}

	private void HandlePopupWasDismissed(UIPopUp popup)
	{
		LockInput(locked: false);
	}

	private void LockInput(bool locked)
	{
		if (m_wasLocked == locked)
		{
			return;
		}
		Collider[] componentsInChildren = GetComponentsInChildren<Collider>(includeInactive: true);
		if (locked)
		{
			Collider[] array = componentsInChildren;
			foreach (Collider c in array)
			{
				Singleton<InputLayerInterface>.Instance.DisableCollider(c, m_debug);
			}
		}
		else
		{
			Collider[] array2 = componentsInChildren;
			foreach (Collider c2 in array2)
			{
				Singleton<InputLayerInterface>.Instance.EnableCollider(c2, m_debug);
			}
		}
		m_wasLocked = locked;
	}
}
