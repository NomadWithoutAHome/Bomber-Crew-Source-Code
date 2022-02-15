using UnityEngine;

[CreateAssetMenu(menuName = "Bomber Crew/Bomber Upgrades/Extinguisher")]
public class ExtinguisherSystemUpgrade : EquipmentUpgradeFittableBase
{
	[SerializeField]
	private int m_maxUses;

	public override BomberUpgradeType GetUpgradeType()
	{
		return BomberUpgradeType.Extinguisher;
	}

	public int GetCapacity()
	{
		return m_maxUses;
	}

	public override StatHelper.StatInfo GetPrimaryStatInfo()
	{
		return StatHelper.StatInfo.Create(m_maxUses, "ui_bomber_upgrades_stat_extinguish_capacity");
	}

	public override bool HasWeight()
	{
		return true;
	}
}
