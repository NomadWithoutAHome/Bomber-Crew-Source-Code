using UnityEngine;

[CreateAssetMenu(menuName = "Bomber Crew/Bomber Upgrade Catalogue")]
public class BomberUpgradeCatalogue : ScriptableObject
{
	[SerializeField]
	private EquipmentUpgradeFittableBase[] m_allUpgrades;

	[SerializeField]
	private BomberDefaults[] m_bomberDefaults;

	public EquipmentUpgradeFittableBase[] GetAll()
	{
		return m_allUpgrades;
	}

	public void SetAll(EquipmentUpgradeFittableBase[] list)
	{
		m_allUpgrades = list;
	}

	public BomberDefaults[] GetDefaultUpgrades()
	{
		return m_bomberDefaults;
	}
}
