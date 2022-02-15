using System.Collections.Generic;
using BomberCrewCommon;

public class CrewQuartersNotifications : NotificationProvider
{
	public override List<Notification> GetNotifications()
	{
		List<Notification> list = new List<Notification>();
		int currentCrewCount = Singleton<CrewContainer>.Instance.GetCurrentCrewCount();
		bool flag = false;
		for (int i = 0; i < currentCrewCount; i++)
		{
			Crewman crewman = Singleton<CrewContainer>.Instance.GetCrewman(i);
			List<CrewmanEquipmentBase> allEquipment = crewman.GetAllEquipment();
			bool flag2 = true;
			foreach (CrewmanEquipmentBase item in allEquipment)
			{
				if (item != null && item != Singleton<CrewmanGearCatalogueLoader>.Instance.GetCatalogue().GetDefaultByType(item.GetGearType()))
				{
					flag2 = false;
					break;
				}
			}
			if (flag2)
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			list.Add(new Notification(NotificationType.Red));
		}
		CrewmanGearCatalogueLoader.CrewmanGearCatalogueInternal catalogue = Singleton<CrewmanGearCatalogueLoader>.Instance.GetCatalogue();
		List<CrewmanEquipmentBase> all = catalogue.GetAll();
		int intel = Singleton<SaveDataContainer>.Instance.Get().GetIntel();
		foreach (CrewmanEquipmentBase item2 in all)
		{
			if (intel >= item2.GetIntelUnlockRequirement() && !Singleton<SaveDataContainer>.Instance.Get().HasBeenViewed(item2.name))
			{
				list.Add(new Notification(NotificationType.New));
				return list;
			}
		}
		return list;
	}
}
