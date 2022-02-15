public class FighterAI_Null : FighterAI
{
	protected override bool CanAttack()
	{
		return true;
	}

	protected override void StopAttack()
	{
	}

	public override void UpdateWithState(FighterState fs)
	{
	}
}
