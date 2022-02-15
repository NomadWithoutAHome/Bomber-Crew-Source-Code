using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class BomberUpgradeCategorySelection : NotificationProvider
{
	[SerializeField]
	private TextSetter m_nameText;

	[SerializeField]
	private AirbaseNotificationDisplay m_notificationDisplay;

	private BomberUpgradeType[] m_filters;

	private EquipmentUpgradeFittableBase.AircraftUpgradeType m_upgradeType;

	public void SetUp(string categoryNamedText, BomberUpgradeType[] filters, EquipmentUpgradeFittableBase.AircraftUpgradeType upgradeType)
	{
		m_upgradeType = upgradeType;
		m_filters = filters;
		m_nameText.SetTextFromLanguageString(categoryNamedText);
		m_notificationDisplay.SetUp(this);
	}

	public override List<Notification> GetNotifications()
	{
		List<Notification> list = new List<Notification>();
		int intel = Singleton<SaveDataContainer>.Instance.Get().GetIntel();
		BomberUpgradeType[] filters = m_filters;
		foreach (BomberUpgradeType bomberUpgradeType in filters)
		{
			List<EquipmentUpgradeFittableBase> all = Singleton<BomberUpgradeCatalogueLoader>.Instance.GetCatalogue().GetAll();
			foreach (EquipmentUpgradeFittableBase item in all)
			{
				if (item.GetUpgradeType() == bomberUpgradeType && item.GetIntelUnlockRequirement() != 0 && intel >= item.GetIntelUnlockRequirement() && (item.GetAircraftUpgradeType() == m_upgradeType || m_upgradeType == EquipmentUpgradeFittableBase.AircraftUpgradeType.AllAircraft) && !Singleton<SaveDataContainer>.Instance.Get().HasBeenViewed(item.name))
				{
					list.Add(new Notification(NotificationType.New));
					break;
				}
			}
		}
		return list;
	}
}
