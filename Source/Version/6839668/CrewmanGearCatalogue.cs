using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Bomber Crew/Crewman Gear Catalogue")]
public class CrewmanGearCatalogue : ScriptableObject
{
	[SerializeField]
	private CrewmanEquipmentBase[] m_allEquipment;

	[SerializeField]
	private CrewmanEquipmentBase[] m_defaultGear;

	[SerializeField]
	private CrewmanEquipmentPreset[] m_allPresets;

	public CrewmanEquipmentBase GetByName(string byName)
	{
		CrewmanEquipmentBase[] allEquipment = m_allEquipment;
		foreach (CrewmanEquipmentBase crewmanEquipmentBase in allEquipment)
		{
			if (string.Equals(crewmanEquipmentBase.name, byName))
			{
				return crewmanEquipmentBase;
			}
		}
		return null;
	}

	public CrewmanEquipmentBase[] GetAllDefaults()
	{
		return m_defaultGear;
	}

	public CrewmanEquipmentPreset[] GetAllPresets()
	{
		return m_allPresets;
	}

	public CrewmanEquipmentBase[] GetAll()
	{
		return m_allEquipment;
	}

	public void SetAll(CrewmanEquipmentBase[] list)
	{
		m_allEquipment = list;
	}

	public List<CrewmanEquipmentBase> GetByType(CrewmanGearType byType)
	{
		List<CrewmanEquipmentBase> list = new List<CrewmanEquipmentBase>();
		CrewmanEquipmentBase[] allEquipment = m_allEquipment;
		foreach (CrewmanEquipmentBase crewmanEquipmentBase in allEquipment)
		{
			if (crewmanEquipmentBase.GetGearType() == byType)
			{
				list.Add(crewmanEquipmentBase);
			}
		}
		return list;
	}

	public CrewmanEquipmentBase GetDefaultByType(CrewmanGearType byType)
	{
		List<CrewmanEquipmentBase> list = new List<CrewmanEquipmentBase>();
		CrewmanEquipmentBase[] defaultGear = m_defaultGear;
		foreach (CrewmanEquipmentBase crewmanEquipmentBase in defaultGear)
		{
			if (crewmanEquipmentBase.GetGearType() == byType)
			{
				return crewmanEquipmentBase;
			}
		}
		return null;
	}
}
