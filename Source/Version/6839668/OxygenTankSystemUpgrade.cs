using UnityEngine;

[CreateAssetMenu(menuName = "Bomber Crew/Bomber Upgrades/Oxygen Tank System")]
public class OxygenTankSystemUpgrade : EquipmentUpgradeFittableBase
{
	public override BomberUpgradeType GetUpgradeType()
	{
		return BomberUpgradeType.OxygenTank;
	}

	public override bool HasArmour()
	{
		return true;
	}

	public override bool HasReliability()
	{
		return true;
	}

	public override bool HasWeight()
	{
		return true;
	}
}
