using UnityEngine;

public class BombAimerSelectorSwitches : MonoBehaviour
{
	[SerializeField]
	private PanelToggleButton m_selectAllButton;

	[SerializeField]
	private PanelToggleButton[] m_selectorSwitches;

	public PanelToggleButton GetSelectAllToggleButton()
	{
		return m_selectAllButton;
	}

	public PanelToggleButton GetSelectorSwitch(int index)
	{
		return m_selectorSwitches[index];
	}
}
