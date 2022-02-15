using System;
using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class SelectableFilterTracker : Singleton<SelectableFilterTracker>
{
	private List<SelectableFilterButton> m_currentlySelected = new List<SelectableFilterButton>();

	private List<SelectableFilterButton> m_allActiveButtons = new List<SelectableFilterButton>();

	private bool m_updateRequired;

	public event Action OnSelectionsChanged;

	public List<SelectableFilterButton> GetAllSelectedButtons()
	{
		return m_currentlySelected;
	}

	public void RegisterAsActive(SelectableFilterButton sfb)
	{
		m_allActiveButtons.Add(sfb);
		m_updateRequired = true;
	}

	public void DeRegisterAsActive(SelectableFilterButton sfb)
	{
		m_allActiveButtons.Remove(sfb);
		m_updateRequired = true;
	}

	public void DeRegisterAsSelected(SelectableFilterButton sfb)
	{
		m_currentlySelected.Remove(sfb);
		m_updateRequired = true;
	}

	public void RegisterAsSelected(SelectableFilterButton sfb)
	{
		if (!m_currentlySelected.Contains(sfb))
		{
			m_currentlySelected.Add(sfb);
		}
		m_updateRequired = true;
	}

	public int GetHighestSelected()
	{
		int result = -1;
		while (m_currentlySelected.Contains(null))
		{
			m_currentlySelected.Remove(null);
		}
		foreach (SelectableFilterButton item in m_currentlySelected)
		{
			if (item != null)
			{
				result = Mathf.Max(item.GetLayerDepth());
			}
		}
		return result;
	}

	private void LateUpdate()
	{
		if (!m_updateRequired)
		{
			return;
		}
		foreach (SelectableFilterButton allActiveButton in m_allActiveButtons)
		{
			allActiveButton.RefreshLayers();
		}
		m_updateRequired = false;
		if (this.OnSelectionsChanged != null)
		{
			this.OnSelectionsChanged();
		}
	}
}
