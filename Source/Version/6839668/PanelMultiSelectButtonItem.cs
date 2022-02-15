using System;
using Common;
using UnityEngine;

public class PanelMultiSelectButtonItem : MonoBehaviour
{
	[SerializeField]
	private GameObject m_defaultHierarchy;

	[SerializeField]
	private GameObject m_activeHierarchy;

	[SerializeField]
	private GameObject m_activeInProgressHierarchy;

	[SerializeField]
	private tk2dUIItem m_uiItem;

	private bool m_active;

	private bool m_inProgress;

	private bool m_everSet;

	public event Action OnClick;

	private void Awake()
	{
		m_active = false;
		m_inProgress = false;
		m_defaultHierarchy.SetActive(value: true);
		m_activeHierarchy.SetActive(value: false);
		m_activeInProgressHierarchy.SetActive(value: false);
		m_everSet = false;
		m_uiItem.OnClick += OnClickButton;
	}

	private void OnClickButton()
	{
		if (this.OnClick != null)
		{
			this.OnClick();
		}
	}

	public void SetState(bool active, bool inProgress)
	{
		if (active != m_active || m_inProgress != inProgress || !m_everSet)
		{
			m_active = active;
			m_inProgress = inProgress;
			m_everSet = true;
			m_defaultHierarchy.CustomActivate(!m_active);
			m_activeHierarchy.CustomActivate(m_active && !m_inProgress);
			m_activeInProgressHierarchy.CustomActivate(m_active && m_inProgress);
		}
	}
}
