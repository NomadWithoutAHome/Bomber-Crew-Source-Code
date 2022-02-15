using System;
using UnityEngine;

public class MultiSelect : MonoBehaviour
{
	[SerializeField]
	private Animation[] m_highlights;

	[SerializeField]
	private AnimationClip m_enableAnim;

	[SerializeField]
	private AnimationClip m_disableAnim;

	[SerializeField]
	private AnimationClip m_blinkAnim;

	[SerializeField]
	private bool m_autoSelect;

	[SerializeField]
	private tk2dUIItem[] m_buttons;

	private int m_currentlySelected;

	public event Action<int> OnSelectIndex;

	private void Awake()
	{
		for (int i = 0; i < m_highlights.Length; i++)
		{
			m_buttons[i].OnClickUIItem += OnClick;
		}
	}

	private void OnClick(tk2dUIItem clickedItem)
	{
		for (int i = 0; i < m_highlights.Length; i++)
		{
			if (m_buttons[i] == clickedItem)
			{
				if (m_autoSelect)
				{
					SetSelected(i);
				}
				if (this.OnSelectIndex != null)
				{
					this.OnSelectIndex(i);
				}
				break;
			}
		}
	}

	public void SetSelected(int selected)
	{
		m_currentlySelected = selected;
		for (int i = 0; i < m_highlights.Length; i++)
		{
			if (i == m_currentlySelected)
			{
				m_highlights[i].Play(m_enableAnim.name);
			}
			else
			{
				m_highlights[i].Play(m_disableAnim.name);
			}
		}
	}

	public void SetInProgress(bool inProgress)
	{
	}

	public int GetSelected()
	{
		return m_currentlySelected;
	}
}
