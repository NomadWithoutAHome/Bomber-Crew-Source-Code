using UnityEngine;

public class FuelTankCombinationSystemDamage : SmoothDamageableRepairable
{
	[SerializeField]
	private FuelTank[] m_tanks;

	public override bool CanBeRepaired()
	{
		return IsBroken() || IsUnreliable();
	}

	public override float DamageGetPassthrough(float amt, DamageSource damageSource)
	{
		return 0f;
	}

	public override float GetHealthNormalised()
	{
		return 1f;
	}

	public override bool IsBroken()
	{
		float num = 0f;
		FuelTank[] tanks = m_tanks;
		foreach (FuelTank fuelTank in tanks)
		{
			num += fuelTank.GetFuelNormalised();
		}
		return num <= 0f;
	}

	public override bool IsDamageBlocker()
	{
		return false;
	}

	public override bool IsUnreliable()
	{
		bool flag = false;
		FuelTank[] tanks = m_tanks;
		foreach (FuelTank fuelTank in tanks)
		{
			if (fuelTank.IsLeaking())
			{
				flag = true;
			}
		}
		float num = 0f;
		FuelTank[] tanks2 = m_tanks;
		foreach (FuelTank fuelTank2 in tanks2)
		{
			num += fuelTank2.GetFuelNormalised();
		}
		return flag || num < 0.25f;
	}

	public override bool NeedsRepair()
	{
		return IsUnreliable() || IsBroken();
	}

	public override void Repair()
	{
	}
}
