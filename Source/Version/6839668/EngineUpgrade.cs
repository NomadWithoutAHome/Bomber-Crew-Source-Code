using UnityEngine;

[CreateAssetMenu(menuName = "Bomber Crew/Bomber Upgrades/Engine")]
public class EngineUpgrade : EquipmentUpgradeFittableBase
{
	[SerializeField]
	private int m_additionalWeightLimit;

	public override BomberUpgradeType GetUpgradeType()
	{
		return BomberUpgradeType.Engine;
	}

	public int GetAdditionalWeightLimit()
	{
		return m_additionalWeightLimit;
	}

	public override StatHelper.StatInfo GetPrimaryStatInfo()
	{
		return StatHelper.StatInfo.Create(m_additionalWeightLimit, "ui_bomber_upgrades_stat_weightlimit_raise");
	}

	public override bool HasArmour()
	{
		return true;
	}
}
