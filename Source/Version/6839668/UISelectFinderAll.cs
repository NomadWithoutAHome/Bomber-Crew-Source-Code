using UnityEngine;

public class UISelectFinderAll : UISelectFinder
{
	public override bool DoesItemMatch(tk2dUIItem toMatch)
	{
		return true;
	}

	public override tk2dUIItem[] GetAllItems()
	{
		return Object.FindObjectsOfType<tk2dUIItem>();
	}
}
