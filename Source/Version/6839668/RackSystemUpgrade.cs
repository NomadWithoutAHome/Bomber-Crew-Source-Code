using UnityEngine;

[CreateAssetMenu(menuName = "Bomber Crew/Bomber Upgrades/Rack")]
public class RackSystemUpgrade : EquipmentUpgradeFittableBase
{
	[SerializeField]
	private int m_numberOfSpaces;

	public override BomberUpgradeType GetUpgradeType()
	{
		return BomberUpgradeType.EquipmentRack;
	}

	public int GetNumberOfSpaces()
	{
		return m_numberOfSpaces;
	}

	public override StatHelper.StatInfo GetPrimaryStatInfo()
	{
		return StatHelper.StatInfo.Create(m_numberOfSpaces, "ui_bomber_upgrades_stat_rack_spaces");
	}

	public override bool HasWeight()
	{
		return true;
	}
}
