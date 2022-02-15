using System;
using UnityEngine;

public class PanelMultiSelect : MonoBehaviour
{
	[SerializeField]
	private PanelMultiSelectButtonItem[] m_buttons;

	[SerializeField]
	private bool m_autoSelect;

	private int m_currentlySelected = -1;

	private bool m_inProgress;

	private bool m_selectedEverSet;

	private bool m_inprogressEverSet;

	public event Action<int> OnSelectIndex;

	private void Start()
	{
		m_selectedEverSet = false;
		m_inprogressEverSet = false;
		for (int i = 0; i < m_buttons.Length; i++)
		{
			int value = i;
			m_buttons[i].OnClick += delegate
			{
				if (m_autoSelect)
				{
					SetSelected(value);
				}
				if (this.OnSelectIndex != null)
				{
					this.OnSelectIndex(value);
				}
			};
		}
	}

	public void SetSelected(int selected)
	{
		if (m_currentlySelected != selected || !m_selectedEverSet)
		{
			m_selectedEverSet = true;
			m_currentlySelected = selected;
			for (int i = 0; i < m_buttons.Length; i++)
			{
				m_buttons[i].SetState(m_currentlySelected == i, m_inProgress);
			}
		}
	}

	public void SetInProgress(bool inProgress)
	{
		if (m_inProgress != inProgress || !m_inprogressEverSet)
		{
			m_inprogressEverSet = true;
			m_inProgress = inProgress;
			for (int i = 0; i < m_buttons.Length; i++)
			{
				m_buttons[i].SetState(m_currentlySelected == i, m_inProgress);
			}
		}
	}

	public int GetSelected()
	{
		return m_currentlySelected;
	}
}
