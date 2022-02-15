using System;
using Common;
using UnityEngine;

public class PanelToggleButton : MonoBehaviour
{
	[SerializeField]
	private GameObject m_stateOffZeroHierarchy;

	[SerializeField]
	private GameObject m_stateOnOneHierarchy;

	[SerializeField]
	private GameObject[] m_additionalOffHierarchies;

	[SerializeField]
	private GameObject[] m_additionalOnHierarchies;

	[SerializeField]
	private GameObject m_disabledHierarchy;

	[SerializeField]
	private tk2dUIItem m_pressButton;

	private bool m_on;

	private bool m_disabled;

	private bool m_hasDEverBeenSet;

	private bool m_hasSEverBeenSet;

	public event Action OnClick;

	private void Awake()
	{
		m_pressButton.OnClick += OnClickButton;
		if (m_stateOffZeroHierarchy != null)
		{
			m_stateOffZeroHierarchy.CustomActivate(active: true);
		}
		if (m_stateOnOneHierarchy != null)
		{
			m_stateOnOneHierarchy.CustomActivate(active: false);
		}
		GameObject[] additionalOffHierarchies = m_additionalOffHierarchies;
		foreach (GameObject g in additionalOffHierarchies)
		{
			g.CustomActivate(active: true);
		}
		GameObject[] additionalOnHierarchies = m_additionalOnHierarchies;
		foreach (GameObject g2 in additionalOnHierarchies)
		{
			g2.CustomActivate(active: false);
		}
		m_disabled = false;
		if (m_disabledHierarchy != null)
		{
			m_disabledHierarchy.SetActive(value: false);
		}
		m_hasDEverBeenSet = false;
		m_hasSEverBeenSet = false;
	}

	public tk2dUIItem GetUIItem()
	{
		return m_pressButton;
	}

	private void OnClickButton()
	{
		if (this.OnClick != null)
		{
			this.OnClick();
		}
	}

	public void SetState(int stateIndex)
	{
		SetState(stateIndex != 0);
	}

	public void SetDisabled(bool disabled)
	{
		if (disabled != m_disabled || !m_hasDEverBeenSet)
		{
			m_hasDEverBeenSet = true;
			m_disabled = disabled;
			SetState(on: false);
			if (m_disabledHierarchy != null)
			{
				m_disabledHierarchy.CustomActivate(m_disabled);
			}
		}
	}

	public void SetState(bool on)
	{
		if (m_on != on || !m_hasSEverBeenSet)
		{
			m_hasSEverBeenSet = true;
			if (m_stateOffZeroHierarchy != null)
			{
				m_stateOffZeroHierarchy.CustomActivate(!on);
			}
			if (m_stateOnOneHierarchy != null)
			{
				m_stateOnOneHierarchy.CustomActivate(on);
			}
			GameObject[] additionalOffHierarchies = m_additionalOffHierarchies;
			foreach (GameObject g in additionalOffHierarchies)
			{
				g.CustomActivate(!on);
			}
			GameObject[] additionalOnHierarchies = m_additionalOnHierarchies;
			foreach (GameObject g2 in additionalOnHierarchies)
			{
				g2.CustomActivate(on);
			}
			m_on = on;
		}
	}

	public bool GetState()
	{
		return m_on;
	}
}
