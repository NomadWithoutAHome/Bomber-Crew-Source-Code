public class HiScoreProviderXboxOne : HiScoreProvider
{
	public override bool IsUpToDate(HiScoreTable hst, HiScoreTable.ScoreListType slt)
	{
		return false;
	}

	public override void AddHighScoreTable(HiScoreTable hst)
	{
	}

	public override void SetCheck()
	{
	}

	public override void SetUp()
	{
	}

	public override bool IsWorking()
	{
		return false;
	}

	public override string GetPlayerName()
	{
		return string.Empty;
	}
}
