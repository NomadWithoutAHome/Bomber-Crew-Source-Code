using UnityEngine;

[CreateAssetMenu(menuName = "Bomber Crew/Bomber Upgrades/Missiles Upgrade")]
public class MissilesUpgrade : EquipmentUpgradeFittableBase
{
	[SerializeField]
	private int m_numMissiles;

	[SerializeField]
	private ProjectileType m_projectileToFire;

	[SerializeField]
	private float m_rechargeTime = 60f;

	public override BomberUpgradeType GetUpgradeType()
	{
		return BomberUpgradeType.Missiles;
	}

	public override bool HasArmour()
	{
		return false;
	}

	public ProjectileType GetProjectileType()
	{
		return m_projectileToFire;
	}

	public override bool HasSurvival()
	{
		return false;
	}

	public override bool HasWeight()
	{
		return true;
	}

	public override StatHelper.StatInfo GetPrimaryStatInfo()
	{
		return StatHelper.StatInfo.Create(m_numMissiles, "ui_bomber_upgrades_stat_num_missiles");
	}

	public int GetNumMissiles()
	{
		return m_numMissiles;
	}

	public float GetRechargeTime()
	{
		return m_rechargeTime;
	}
}
