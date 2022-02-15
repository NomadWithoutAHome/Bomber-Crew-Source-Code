using UnityEngine;

public class UISelectFinderList : UISelectFinder
{
	[SerializeField]
	private tk2dUIItem[] m_list;

	public override bool DoesItemMatch(tk2dUIItem toMatch)
	{
		tk2dUIItem[] allItems = GetAllItems();
		tk2dUIItem[] array = allItems;
		foreach (tk2dUIItem tk2dUIItem2 in array)
		{
			if (tk2dUIItem2 == toMatch)
			{
				return true;
			}
		}
		return false;
	}

	public override tk2dUIItem[] GetAllItems()
	{
		return m_list;
	}
}
