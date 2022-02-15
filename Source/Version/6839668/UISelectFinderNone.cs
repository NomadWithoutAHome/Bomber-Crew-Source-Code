public class UISelectFinderNone : UISelectFinder
{
	public override bool DoesItemMatch(tk2dUIItem toMatch)
	{
		return false;
	}

	public override tk2dUIItem[] GetAllItems()
	{
		return new tk2dUIItem[0];
	}
}
