using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class CrewQuartersCrewmanNameButton : NotificationProvider
{
	[SerializeField]
	private TextSetter m_crewmanName;

	[SerializeField]
	private tk2dSprite m_roleSprite;

	[SerializeField]
	private AirbaseNotificationDisplay m_notificationDisplay;

	private Crewman m_crewman;

	public void SetUp(Crewman c)
	{
		m_crewman = c;
		if (m_notificationDisplay != null)
		{
			m_notificationDisplay.SetUp(this);
		}
		Refresh();
	}

	private void Update()
	{
		Refresh();
	}

	public void Refresh()
	{
		m_crewmanName.SetText(m_crewman.GetFirstName()[0] + " " + m_crewman.GetSurname());
		m_roleSprite.SetSprite(Singleton<EnumToIconMapping>.Instance.GetIconName(m_crewman.GetPrimarySkill().GetSkill()));
	}

	public override List<Notification> GetNotifications()
	{
		List<Notification> list = new List<Notification>();
		List<CrewmanEquipmentBase> allEquipment = m_crewman.GetAllEquipment();
		bool flag = true;
		foreach (CrewmanEquipmentBase item in allEquipment)
		{
			if (item != null && item != Singleton<CrewmanGearCatalogueLoader>.Instance.GetCatalogue().GetDefaultByType(item.GetGearType()))
			{
				flag = false;
				break;
			}
		}
		CrewmanGearCatalogueLoader.CrewmanGearCatalogueInternal catalogue = Singleton<CrewmanGearCatalogueLoader>.Instance.GetCatalogue();
		List<CrewmanEquipmentBase> all = catalogue.GetAll();
		int intel = Singleton<SaveDataContainer>.Instance.Get().GetIntel();
		foreach (CrewmanEquipmentBase item2 in all)
		{
			if (intel >= item2.GetIntelUnlockRequirement() && item2.IsAvailableForCrewman(m_crewman) && !Singleton<SaveDataContainer>.Instance.Get().HasBeenViewed(item2.name))
			{
				list.Add(new Notification(NotificationType.New));
				break;
			}
		}
		if (flag)
		{
			list.Add(new Notification(NotificationType.Red));
		}
		return list;
	}
}
