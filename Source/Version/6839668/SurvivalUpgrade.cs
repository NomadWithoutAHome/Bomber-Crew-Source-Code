using UnityEngine;

[CreateAssetMenu(menuName = "Bomber Crew/Bomber Upgrades/Survival")]
public class SurvivalUpgrade : EquipmentUpgradeFittableBase
{
	[SerializeField]
	private BomberUpgradeType m_upgradeType;

	public override BomberUpgradeType GetUpgradeType()
	{
		return m_upgradeType;
	}

	public override bool HasSurvival()
	{
		return true;
	}

	public override bool HasWeight()
	{
		return true;
	}
}
