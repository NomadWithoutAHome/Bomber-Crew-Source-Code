using UnityEngine;

public class UISelectFinderUnderHierarchy : UISelectFinder
{
	[SerializeField]
	private Transform m_hierarchy;

	public void SetHierarchy(Transform t)
	{
		m_hierarchy = t;
	}

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
		if (m_hierarchy != null)
		{
			return m_hierarchy.GetComponentsInChildren<tk2dUIItem>();
		}
		return new tk2dUIItem[0];
	}
}
