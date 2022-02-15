using UnityEngine;

[CreateAssetMenu(menuName = "Bomber Crew/Bomber Upgrades/Radar")]
public class RadarSystemUpgrade : EquipmentUpgradeFittableBase
{
	[SerializeField]
	private float m_sweepTime;

	[SerializeField]
	private float m_range;

	public override BomberUpgradeType GetUpgradeType()
	{
		return BomberUpgradeType.Radar;
	}

	public float GetSweepTime()
	{
		return m_sweepTime;
	}

	public float GetRange()
	{
		return m_range;
	}

	public override StatHelper.StatInfo GetPrimaryStatInfo()
	{
		return StatHelper.StatInfo.CreateSmallerBetter(m_sweepTime, "ui_bomber_upgrades_stat_radar_sweep_time");
	}

	public override StatHelper.StatInfo GetSecondaryStatInfo()
	{
		return StatHelper.StatInfo.Create(m_range, "ui_bomber_upgrades_stat_radar_range");
	}

	public override bool HasWeight()
	{
		return true;
	}
}
