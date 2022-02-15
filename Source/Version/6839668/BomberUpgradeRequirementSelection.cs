using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class BomberUpgradeRequirementSelection : NotificationProvider
{
	[SerializeField]
	protected GameObject m_isEmptyHierarchy;

	[SerializeField]
	protected GameObject m_notEmptyHierarchy;

	[SerializeField]
	protected TextSetter m_currentlyInstalledText;

	[SerializeField]
	private SelectableFilterButton m_filterButton;

	[SerializeField]
	private TextSetter m_requirementTitleText;

	[SerializeField]
	private AirbaseNotificationDisplay m_notificationDisplay;

	protected BomberRequirements.BomberEquipmentRequirement m_currentRequirement;

	protected BomberUpgradeConfig m_config;

	private EquipmentUpgradeFittableBase.AircraftUpgradeType m_filterType;

	public virtual void SetUp(BomberRequirements.BomberEquipmentRequirement bomberRequirement, BomberUpgradeConfig config, EquipmentUpgradeFittableBase.AircraftUpgradeType filterType)
	{
		m_filterType = filterType;
		m_currentRequirement = bomberRequirement;
		m_config = config;
		m_filterButton.OnRefresh += Refresh;
		m_notificationDisplay.SetUp(this);
		Refresh();
	}

	public virtual void Refresh()
	{
		EquipmentUpgradeFittableBase upgradeFor = m_config.GetUpgradeFor(m_currentRequirement);
		m_requirementTitleText.SetTextFromLanguageString(m_currentRequirement.GetNamedText());
		if (upgradeFor == null)
		{
			m_isEmptyHierarchy.SetActive(value: true);
			m_notEmptyHierarchy.SetActive(value: false);
			return;
		}
		m_isEmptyHierarchy.SetActive(value: false);
		m_notEmptyHierarchy.SetActive(value: true);
		if (m_currentlyInstalledText != null)
		{
			m_currentlyInstalledText.SetText(upgradeFor.GetNameTranslated());
		}
	}

	public override List<Notification> GetNotifications()
	{
		List<Notification> list = new List<Notification>();
		int intel = Singleton<SaveDataContainer>.Instance.Get().GetIntel();
		BomberUpgradeType upgradeConfig = m_currentRequirement.GetUpgradeConfig();
		List<EquipmentUpgradeFittableBase> allOfType = Singleton<BomberUpgradeCatalogueLoader>.Instance.GetCatalogue().GetAllOfType(upgradeConfig);
		foreach (EquipmentUpgradeFittableBase item in allOfType)
		{
			if (intel >= item.GetIntelUnlockRequirement() && item.GetIntelUnlockRequirement() != 0 && item.IsDisplayableFor(m_currentRequirement) && (item.GetAircraftUpgradeType() == m_filterType || m_filterType == EquipmentUpgradeFittableBase.AircraftUpgradeType.AllAircraft) && !Singleton<SaveDataContainer>.Instance.Get().HasBeenViewed(item.name))
			{
				list.Add(new Notification(NotificationType.New));
				return list;
			}
		}
		return list;
	}
}
