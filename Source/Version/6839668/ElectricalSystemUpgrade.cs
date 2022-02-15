using UnityEngine;

[CreateAssetMenu(menuName = "Bomber Crew/Bomber Upgrades/Electrical System")]
public class ElectricalSystemUpgrade : EquipmentUpgradeFittableBase
{
	[SerializeField]
	private int m_voltage;

	public override BomberUpgradeType GetUpgradeType()
	{
		return BomberUpgradeType.Electrical;
	}

	public override StatHelper.StatInfo GetPrimaryStatInfo()
	{
		return StatHelper.StatInfo.CreateUnitType(m_voltage, "ui_postfix_voltage", biggerIsBetter: true, "ui_bomber_upgrades_stat_electricalvoltage");
	}

	public override bool HasArmour()
	{
		return true;
	}

	public override bool HasWeight()
	{
		return true;
	}

	public override bool HasReliability()
	{
		return true;
	}
}
