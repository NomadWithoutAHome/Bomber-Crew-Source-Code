using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class BomberUpgradeNotificationProvider : NotificationProvider
{
	[SerializeField]
	private EquipmentUpgradeFittableBase.AircraftUpgradeType m_filterType;

	public override List<Notification> GetNotifications()
	{
		BomberUpgradeCatalogueLoader.BomberUpgradeCatalogueInternal catalogue = Singleton<BomberUpgradeCatalogueLoader>.Instance.GetCatalogue();
		List<EquipmentUpgradeFittableBase> all = catalogue.GetAll();
		int intel = Singleton<SaveDataContainer>.Instance.Get().GetIntel();
		List<Notification> list = new List<Notification>();
		foreach (EquipmentUpgradeFittableBase item in all)
		{
			if (intel >= item.GetIntelUnlockRequirement() && (item.GetAircraftUpgradeType() == m_filterType || m_filterType == EquipmentUpgradeFittableBase.AircraftUpgradeType.AllAircraft) && item.GetIntelUnlockRequirement() != 0 && !Singleton<SaveDataContainer>.Instance.Get().HasBeenViewed(item.name))
			{
				list.Add(new Notification(NotificationType.New));
				return list;
			}
		}
		return list;
	}
}
