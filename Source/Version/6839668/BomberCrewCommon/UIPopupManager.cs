using System.Collections.Generic;
using UnityEngine;

namespace BomberCrewCommon;

public class UIPopupManager : Singleton<UIPopupManager>
{
	private bool m_canDismissPopup = true;

	private UIPopUp m_currentPopup;

	private LinkedList<KeyValuePair<UIPopUp, UIPopupData>> m_popupQueue;

	[SerializeField]
	private Transform m_popupParentTransform;

	public UIPopUp ActivePopup => m_currentPopup;

	public bool IsCurrentlyShowingPopup => m_currentPopup != null || m_popupQueue.Count > 0;

	public event PopupEventDelegate PopupWasShown;

	public event PopupEventDelegate PopupWasDismissed;

	public void CanDismissPopup(bool enabled)
	{
		m_canDismissPopup = enabled;
	}

	public void DisplayPopup(GameObject popupPrefab)
	{
		UIPopupData data = new UIPopupData();
		DisplayPopup(popupPrefab, data);
	}

	public void AddPopupToFrontOfQueue(GameObject popupPrefab)
	{
		UIPopupData data = new UIPopupData();
		AddPopupToFrontOfQueue(popupPrefab, data);
	}

	public void DisplayPopup(GameObject popupPrefab, UIPopupData data)
	{
		if (popupPrefab.GetComponent<UIPopUp>() != null)
		{
			KeyValuePair<UIPopUp, UIPopupData> value = CreatePopupEntry(popupPrefab, data);
			m_popupQueue.AddLast(value);
			ShowNextPopup();
		}
	}

	public void AddPopupToFrontOfQueue(GameObject popupPrefab, UIPopupData data)
	{
		if (popupPrefab.GetComponent<UIPopUp>() != null)
		{
			KeyValuePair<UIPopUp, UIPopupData> value = CreatePopupEntry(popupPrefab, data);
			m_popupQueue.AddFirst(value);
			ShowNextPopup();
		}
	}

	private KeyValuePair<UIPopUp, UIPopupData> CreatePopupEntry(GameObject popupPrefab, UIPopupData data)
	{
		data.Prefab = popupPrefab;
		GameObject gameObject = Object.Instantiate(popupPrefab);
		gameObject.transform.parent = m_popupParentTransform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.SetActive(value: false);
		UIPopUp component = gameObject.GetComponent<UIPopUp>();
		component.OnPopupDismissed += OnDismissCurrentPopup;
		return new KeyValuePair<UIPopUp, UIPopupData>(component, data);
	}

	private void ShowNextPopup()
	{
		if (m_currentPopup == null && m_popupQueue.Count > 0)
		{
			KeyValuePair<UIPopUp, UIPopupData> value = m_popupQueue.First.Value;
			m_popupQueue.RemoveFirst();
			UIPopUp key = value.Key;
			UIPopupData value2 = value.Value;
			m_currentPopup = key;
			m_currentPopup.SetupPopup(value2);
			m_currentPopup.ShowPopup();
			if (this.PopupWasShown != null)
			{
				this.PopupWasShown(m_currentPopup);
			}
		}
	}

	public void DismissCurrentPopup()
	{
		if (m_currentPopup != null)
		{
			m_currentPopup.DismissPopup();
		}
	}

	private void OnDismissCurrentPopup(UIPopUp currentPopup)
	{
		if (this.PopupWasDismissed != null && m_currentPopup != null)
		{
			this.PopupWasDismissed(m_currentPopup);
		}
		m_currentPopup = null;
		ShowNextPopup();
	}

	public void LockCurrentPopupInput(bool state)
	{
		if ((bool)m_currentPopup)
		{
			m_currentPopup.LockInput(state);
		}
	}

	private void Awake()
	{
		m_popupQueue = new LinkedList<KeyValuePair<UIPopUp, UIPopupData>>();
	}
}
