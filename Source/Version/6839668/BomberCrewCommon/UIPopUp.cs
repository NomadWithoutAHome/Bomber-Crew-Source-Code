using System;
using Common;
using UnityEngine;

namespace BomberCrewCommon;

public class UIPopUp : MonoBehaviour
{
	private UIPopupData m_data;

	public event Action<UIPopUp> OnPopupStart;

	public event Action<UIPopUp> OnPopupShown;

	public event Action<UIPopUp> OnPopupDismissed;

	public event Action<UIPopUp> OnPopupWillBeDismissed;

	public void LockInput(bool locked)
	{
		Collider[] componentsInChildren = GetComponentsInChildren<Collider>(includeInactive: true);
		Collider[] array = componentsInChildren;
		foreach (Collider c in array)
		{
			if (locked)
			{
				Singleton<InputLayerInterface>.Instance.DisableCollider(c);
			}
			else
			{
				Singleton<InputLayerInterface>.Instance.EnableCollider(c);
			}
		}
	}

	public virtual void SetupPopup(UIPopupData data)
	{
		m_data = data;
	}

	public virtual void ShowPopup()
	{
		base.gameObject.CustomActivate(active: true, 0f, PopupWasShown);
		if (this.OnPopupStart != null)
		{
			this.OnPopupStart(this);
		}
		if (m_data != null && m_data.PopupStartCallback != null)
		{
			m_data.PopupStartCallback(this);
		}
	}

	public virtual void DismissPopup()
	{
		if (this.OnPopupWillBeDismissed != null)
		{
			this.OnPopupWillBeDismissed(this);
		}
		if (m_data != null && m_data.PopupWillBeDismissedCallback != null)
		{
			m_data.PopupWillBeDismissedCallback(this);
		}
		StopAllCoroutines();
		base.gameObject.CustomActivate(active: false, 0f, PopupWasDismissed);
	}

	private void PopupWasShown()
	{
		if (this.OnPopupShown != null)
		{
			this.OnPopupShown(this);
		}
		if (m_data != null && m_data.PopupShownCallback != null)
		{
			m_data.PopupShownCallback(this);
		}
	}

	private void PopupWasDismissed()
	{
		UnityEngine.Object.Destroy(base.gameObject);
		if (this.OnPopupDismissed != null)
		{
			this.OnPopupDismissed(this);
		}
		if (m_data != null && m_data.PopupDismissedCallback != null)
		{
			m_data.PopupDismissedCallback(this);
		}
	}
}
