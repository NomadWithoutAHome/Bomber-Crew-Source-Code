using UnityEngine;

[CreateAssetMenu(menuName = "Bomber Crew/Bomber Upgrades/Gun Turret")]
public class GunTurretUpgrade : EquipmentUpgradeFittableBase
{
	public enum GunTurretUpgradeFilterType
	{
		Standard,
		Window,
		Ball
	}

	[SerializeField]
	private float m_damageMultiplier;

	[SerializeField]
	private int m_numGuns;

	[SerializeField]
	private int m_ammoPerReload;

	[SerializeField]
	private ProjectileType m_projectileTypeBase;

	[SerializeField]
	private ProjectileType m_projectileTypeIncendiary;

	[SerializeField]
	private ProjectileType m_projectileTypeArmourPiercing;

	[SerializeField]
	private ProjectileType m_projectileTypeExplosive;

	[SerializeField]
	private bool m_hasAmmoFeed;

	[SerializeField]
	private GunTurretUpgradeFilterType m_upgradeFilterType;

	public override BomberUpgradeType GetUpgradeType()
	{
		return BomberUpgradeType.GunTurret;
	}

	public ProjectileType GetProjectileType()
	{
		return m_projectileTypeBase;
	}

	public ProjectileType GetProjectileTypeIncendiary()
	{
		return m_projectileTypeIncendiary;
	}

	public ProjectileType GetProjectileTypeExplosive()
	{
		return m_projectileTypeExplosive;
	}

	public ProjectileType GetProjectileTypeArmourPiercing()
	{
		return m_projectileTypeArmourPiercing;
	}

	public float GetDamageTotal()
	{
		return m_damageMultiplier * m_projectileTypeBase.GetDamage();
	}

	public bool HasAmmoFeed()
	{
		return m_hasAmmoFeed;
	}

	public float GetDamageMultiplier()
	{
		return m_damageMultiplier;
	}

	public int GetAmmoPerReload()
	{
		return m_ammoPerReload;
	}

	public override bool IsDisplayableFor(BomberRequirements.BomberEquipmentRequirement requirement)
	{
		if (requirement.GetFilteringFlags() == (int)m_upgradeFilterType)
		{
			return true;
		}
		return false;
	}

	public override StatHelper.StatInfo GetPrimaryStatInfo()
	{
		float num = 1f / m_projectileTypeBase.GetFireInterval();
		return StatHelper.StatInfo.Create(GetDamageTotal() * num * (float)m_numGuns, "ui_bomber_upgrades_stat_damage_per_second");
	}

	public override StatHelper.StatInfo GetSecondaryStatInfo()
	{
		return StatHelper.StatInfo.Create(m_ammoPerReload, "ui_bomber_upgrades_stat_ammo_per_reload");
	}

	public override bool HasWeight()
	{
		return true;
	}
}
