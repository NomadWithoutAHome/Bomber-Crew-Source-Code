using System.Collections.Generic;
using BomberCrewCommon;

public class UISelectFinderUsableInteractivesOnly : UISelectFinder
{
	public override bool DoesItemMatch(tk2dUIItem toMatch)
	{
		if (toMatch.transform.parent != null && toMatch.transform.parent.GetComponent<CrewmanAvatar>() != null)
		{
			return true;
		}
		return false;
	}

	public override tk2dUIItem[] GetAllItems()
	{
		CrewmanAvatar currentlySelected = Singleton<ContextControl>.Instance.GetCurrentlySelected();
		List<tk2dUIItem> list = new List<tk2dUIItem>();
		if (currentlySelected != null)
		{
			List<UIInteractionMarker> allInteractionMarkers = Singleton<ContextControl>.Instance.GetAllInteractionMarkers();
			foreach (UIInteractionMarker item in allInteractionMarkers)
			{
				if (item.GetRelatedItem().GetInteractionOptionsPublic(currentlySelected, skipNullCheck: true) != null)
				{
					list.Add(item.GetRelatedItem().GetInteractionItem());
					list.Add(item.GetTeardrop());
				}
			}
			List<WalkableArea> allWalkableAreas = Singleton<ContextControl>.Instance.GetAllWalkableAreas();
			foreach (WalkableArea item2 in allWalkableAreas)
			{
				list.Add(item2.GetUIItem());
			}
		}
		return list.ToArray();
	}
}
