using UnityEngine;

public class OutlineOnOver : MonoBehaviour
{
	[SerializeField]
	private FlashManager m_outlineManager;

	[SerializeField]
	private tk2dUIItem m_uiItem;

	[SerializeField]
	private Color m_outlineColor;

	private FlashManager.ActiveFlash m_outlineFlash;

	private void OnEnable()
	{
		m_uiItem.OnHoverOver += m_uiItem_OnHoverOver;
		m_uiItem.OnHoverOut += m_uiItem_OnHoverOut;
	}

	private void OnDisable()
	{
		if (m_uiItem != null)
		{
			m_uiItem.OnHoverOver -= m_uiItem_OnHoverOver;
			m_uiItem.OnHoverOut -= m_uiItem_OnHoverOut;
		}
	}

	private void m_uiItem_OnHoverOut()
	{
		if (m_outlineFlash != null)
		{
			m_outlineManager.RemoveFlash(m_outlineFlash);
		}
	}

	public void SetOutlineManager(FlashManager outlineManager)
	{
		m_outlineManager = outlineManager;
	}

	private void m_uiItem_OnHoverOver()
	{
		m_outlineFlash = m_outlineManager.AddOrUpdateFlash(0f, 0f, 0f, 1, 1f, m_outlineColor, m_outlineFlash);
	}
}
