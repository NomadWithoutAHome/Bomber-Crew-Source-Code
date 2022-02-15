using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class CrewQuartersSlotCategoryButton : NotificationProvider
{
	[SerializeField]
	private TextSetter m_slotName;

	[SerializeField]
	private TextSetter m_equippedName;

	[SerializeField]
	private SelectableFilterButton m_filterButton;

	[SerializeField]
	private GameObject m_isPresetDisableNode;

	[SerializeField]
	private GameObject m_isPresetEnableNode;

	[SerializeField]
	private AirbaseNotificationDisplay m_notificationDisplay;

	private Crewman m_crewman;

	private CrewQuartersScreenController.GearTypeDisplay m_gearType;

	private bool m_isPreset;

	private void Awake()
	{
		m_filterButton.OnRefresh += Refresh;
	}

	public void SetUp(Crewman forCrewman, CrewQuartersScreenController.GearTypeDisplay gearType)
	{
		m_crewman = forCrewman;
		m_gearType = gearType;
		m_isPreset = gearType.m_isPresets;
		if (m_notificationDisplay != null)
		{
			m_notificationDisplay.SetUp(this);
		}
		Refresh();
	}

	public override List<Notification> GetNotifications()
	{
		CrewmanGearCatalogueLoader.CrewmanGearCatalogueInternal catalogue = Singleton<CrewmanGearCatalogueLoader>.Instance.GetCatalogue();
		int intel = Singleton<SaveDataContainer>.Instance.Get().GetIntel();
		List<Notification> list = new List<Notification>();
		if (m_isPreset)
		{
			List<CrewmanEquipmentPreset> allPresets = catalogue.GetAllPresets();
			{
				foreach (CrewmanEquipmentPreset item in allPresets)
				{
					if (intel >= item.GetIntelUnlockRequirement() && item.IsAvailableForCrewman(m_crewman) && !Singleton<SaveDataContainer>.Instance.Get().HasBeenViewed(item.name))
					{
						list.Add(new Notification(NotificationType.New));
						return list;
					}
				}
				return list;
			}
		}
		List<CrewmanEquipmentBase> all = catalogue.GetAll();
		foreach (CrewmanEquipmentBase item2 in all)
		{
			if (intel >= item2.GetIntelUnlockRequirement() && item2.GetGearType() == m_gearType.m_gearType && item2.IsAvailableForCrewman(m_crewman) && !Singleton<SaveDataContainer>.Instance.Get().HasBeenViewed(item2.name))
			{
				list.Add(new Notification(NotificationType.New));
				return list;
			}
		}
		return list;
	}

	private void Refresh()
	{
		m_slotName.SetTextFromLanguageString(m_gearType.m_namedText);
		if (!m_isPreset)
		{
			CrewmanEquipmentBase equippedFor = m_crewman.GetEquippedFor(m_gearType.m_gearType);
			m_equippedName.SetText(equippedFor.GetNamedTextTranslated());
			if (m_isPresetDisableNode != null)
			{
				m_isPresetDisableNode.SetActive(value: true);
			}
			if (m_isPresetEnableNode != null)
			{
				m_isPresetEnableNode.SetActive(value: false);
			}
		}
		else
		{
			if (m_isPresetDisableNode != null)
			{
				m_isPresetDisableNode.SetActive(value: false);
			}
			if (m_isPresetEnableNode != null)
			{
				m_isPresetEnableNode.SetActive(value: true);
			}
		}
	}
}
