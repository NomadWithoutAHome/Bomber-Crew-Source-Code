using System.Collections.Generic;
using BomberCrewCommon;

public class UISelectFinderCrewmenOnly : UISelectFinder
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
		List<tk2dUIItem> list = new List<tk2dUIItem>();
		int currentCrewCount = Singleton<CrewContainer>.Instance.GetCurrentCrewCount();
		for (int i = 0; i < currentCrewCount; i++)
		{
			CrewmanAvatar avatarFor = Singleton<ContextControl>.Instance.GetAvatarFor(Singleton<CrewContainer>.Instance.GetCrewman(i));
			list.Add(avatarFor.GetComponentInChildren<tk2dUIItem>());
		}
		return list.ToArray();
	}
}
