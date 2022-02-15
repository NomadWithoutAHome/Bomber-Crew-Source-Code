using UnityEngine;

public class HighlightOnOver : MonoBehaviour
{
	[SerializeField]
	private tk2dSpriteAnimator m_highlightAnimator;

	[SerializeField]
	private tk2dUIItem m_uiItem;

	[SerializeField]
	private string m_hoverAnimName = "Hover01";

	[SerializeField]
	private string m_offAnimName = "Off";

	private bool m_forcedOn;

	private void Start()
	{
		if (m_uiItem.GetComponent<Collider>().enabled)
		{
			m_highlightAnimator.Play(m_offAnimName);
		}
		m_uiItem.OnHoverOver += m_uiItem_OnHoverOver;
		m_uiItem.OnHoverOut += m_uiItem_OnHoverOut;
	}

	public void SetHoverColour(int crewmanIndex)
	{
		m_hoverAnimName = "Hover0" + crewmanIndex;
	}

	private void m_uiItem_OnHoverOut()
	{
		if (!m_forcedOn)
		{
			m_highlightAnimator.Play(m_offAnimName);
		}
	}

	public void SetOn()
	{
		m_forcedOn = true;
		m_highlightAnimator.Play(m_hoverAnimName);
	}

	private void m_uiItem_OnHoverOver()
	{
		m_highlightAnimator.Play(m_hoverAnimName);
	}
}
