using UnityEngine;

[CreateAssetMenu(menuName = "Bomber Crew/Bomber Upgrades/Hydraulic System")]
public class HydraulicSystemUpgrade : EquipmentUpgradeFittableBase
{
	[SerializeField]
	private float m_speed;

	public override BomberUpgradeType GetUpgradeType()
	{
		return BomberUpgradeType.Hyrdaulic;
	}

	public override StatHelper.StatInfo GetPrimaryStatInfo()
	{
		return StatHelper.StatInfo.Create(m_speed, "ui_bomber_upgrades_stat_hydraulic_power");
	}

	public override bool HasWeight()
	{
		return true;
	}

	public override bool HasArmour()
	{
		return true;
	}

	public override bool HasReliability()
	{
		return true;
	}

	public float GetSpeedMultiplier()
	{
		return m_speed;
	}
}
