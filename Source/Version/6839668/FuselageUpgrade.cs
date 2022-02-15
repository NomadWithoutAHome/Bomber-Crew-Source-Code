using UnityEngine;

[CreateAssetMenu(menuName = "Bomber Crew/Bomber Upgrades/Fuselage")]
public class FuselageUpgrade : EquipmentUpgradeFittableBase
{
	public override BomberUpgradeType GetUpgradeType()
	{
		return BomberUpgradeType.FuselageMain;
	}

	public override bool HasWeight()
	{
		return true;
	}

	public override bool HasArmour()
	{
		return true;
	}
}
