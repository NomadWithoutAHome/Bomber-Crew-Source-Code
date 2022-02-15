using UnityEngine;

[CreateAssetMenu(menuName = "Bomber Crew/Bomber Upgrades/Fuel Tank")]
public class FuelTankUpgrade : EquipmentUpgradeFittableBase
{
	[SerializeField]
	private bool m_isSelfSealing;

	public override BomberUpgradeType GetUpgradeType()
	{
		return BomberUpgradeType.FuelTank;
	}

	public override bool HasArmour()
	{
		return true;
	}

	public override bool HasWeight()
	{
		return true;
	}
}
