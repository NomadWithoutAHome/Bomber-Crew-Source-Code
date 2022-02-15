public abstract class SmoothDamageableRepairable : SmoothDamageable
{
	public abstract bool CanBeRepaired();

	public abstract bool NeedsRepair();

	public abstract bool IsUnreliable();

	public abstract bool IsBroken();

	public abstract void Repair();

	public virtual void StartRepair()
	{
	}

	public virtual void AbandonRepair()
	{
	}
}
