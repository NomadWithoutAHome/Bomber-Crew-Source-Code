using UnityEngine;

public class UIMenuDisplay : MonoBehaviour
{
	[SerializeField]
	private BomberLoadout m_bomberLoadout;

	[SerializeField]
	private tk2dUIToggleButtonGroup m_CategoryToggleGroup;

	[SerializeField]
	private LayoutGrid m_CategoryLayoutGrid;

	[SerializeField]
	private GameObject m_categoryButton;

	[SerializeField]
	private tk2dUIToggleButtonGroup m_SlotsToggleGroup;

	[SerializeField]
	private LayoutGrid m_SlotsLayoutGrid;

	[SerializeField]
	private GameObject m_slotButton;

	private int m_selectedCategoryIndex;

	private void Start()
	{
		PopulateMenu(m_bomberLoadout);
	}

	private void PopulateMenu(BomberLoadout bomberLoadout)
	{
		m_bomberLoadout = bomberLoadout;
		m_CategoryLayoutGrid.ClearChildrenImmediate();
		tk2dUIToggleButton[] array = new tk2dUIToggleButton[m_bomberLoadout.GetCategories().Length];
		for (int i = 0; i < m_bomberLoadout.GetCategories().Length; i++)
		{
			BomberLoadout.Category category = m_bomberLoadout.GetCategories()[i];
			GameObject gameObject = Object.Instantiate(m_categoryButton);
			gameObject.transform.parent = m_CategoryLayoutGrid.transform;
			tk2dTextMesh[] componentsInChildren = gameObject.GetComponentsInChildren<tk2dTextMesh>(includeInactive: true);
			tk2dTextMesh[] array2 = componentsInChildren;
			foreach (tk2dTextMesh tk2dTextMesh2 in array2)
			{
				tk2dTextMesh2.text = category.GetName();
			}
			array[i] = gameObject.GetComponent<tk2dUIToggleButton>();
		}
		m_CategoryToggleGroup.AddNewToggleButtons(array);
		m_selectedCategoryIndex = 0;
		m_CategoryToggleGroup.SelectedIndex = m_selectedCategoryIndex;
		m_CategoryLayoutGrid.RepositionChildren();
		ShowCategorySlots(m_selectedCategoryIndex);
	}

	private void ShowCategorySlots(int index)
	{
		m_SlotsLayoutGrid.ClearChildrenImmediate();
		BomberLoadout.Category category = m_bomberLoadout.GetCategories()[index];
		tk2dUIToggleButton[] array = new tk2dUIToggleButton[category.GetSlots().Length];
		for (int i = 0; i < category.GetSlots().Length; i++)
		{
			GameObject gameObject = Object.Instantiate(m_slotButton);
			gameObject.transform.parent = m_SlotsLayoutGrid.transform;
			tk2dTextMesh[] componentsInChildren = gameObject.GetComponentsInChildren<tk2dTextMesh>(includeInactive: true);
			tk2dTextMesh[] array2 = componentsInChildren;
			foreach (tk2dTextMesh tk2dTextMesh2 in array2)
			{
				tk2dTextMesh2.text = category.GetSlots()[i];
			}
			array[i] = gameObject.GetComponent<tk2dUIToggleButton>();
		}
		m_SlotsToggleGroup.AddNewToggleButtons(array);
		m_SlotsLayoutGrid.RepositionChildren();
		m_SlotsToggleGroup.SelectedIndex = m_bomberLoadout.GetCategories()[index].LastSelectedSlotIndex;
		if (m_SlotsToggleGroup.SelectedIndex < 0)
		{
			m_SlotsToggleGroup.SelectedIndex = 0;
		}
		m_selectedCategoryIndex = index;
	}

	private void SelectCategory(tk2dUIToggleButtonGroup toggleButtonGroup)
	{
		m_bomberLoadout.GetCategories()[m_selectedCategoryIndex].LastSelectedSlotIndex = m_SlotsToggleGroup.SelectedIndex;
		ShowCategorySlots(toggleButtonGroup.SelectedIndex);
	}

	private void SelectSlot(tk2dUIToggleButtonGroup toggleButtonGroup)
	{
	}
}
