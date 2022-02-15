using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

[CreateAssetMenu(menuName = "Bomber Crew/Equipment Preset")]
public class CrewmanEquipmentPreset : ScriptableObject
{
	[SerializeField]
	[NamedText]
	private string m_presetName;

	[SerializeField]
	private CrewmanEquipmentBase[] m_allItemsInPreset;

	[SerializeField]
	private Crewman.SpecialisationSkill[] m_lockedToSkills;

	[SerializeField]
	[NamedText]
	private string m_presetDescription;

	[SerializeField]
	private bool m_isDLC;

	[SerializeField]
	private string m_DLCIconName;

	[SerializeField]
	private string m_DLCIconNameConsole;

	[SerializeField]
	private bool m_isGiftIcon;

	public bool IsAvailableForCrewman(Crewman cr)
	{
		if (IsAvailableForSkill(cr.GetPrimarySkill().GetSkill()))
		{
			return true;
		}
		if (cr.GetSecondarySkill() != null && IsAvailableForSkill(cr.GetSecondarySkill().GetSkill()))
		{
			return true;
		}
		return false;
	}

	public bool IsDLC()
	{
		return m_isDLC;
	}

	public string GetDLCIconName()
	{
		return m_DLCIconName;
	}

	public bool IsGiftIcon()
	{
		return m_isGiftIcon;
	}

	public CrewmanEquipmentBase[] GetAllInPreset()
	{
		return m_allItemsInPreset;
	}

	public string GetPresetName()
	{
		return m_presetName;
	}

	public string GetDescriptionName()
	{
		return m_presetDescription;
	}

	public int GetCost(Crewman forCrewman, bool useRefund, Dictionary<string, int> stockUsed)
	{
		int num = 0;
		CrewmanEquipmentBase[] allItemsInPreset = m_allItemsInPreset;
		foreach (CrewmanEquipmentBase crewmanEquipmentBase in allItemsInPreset)
		{
			if (useRefund)
			{
				if (forCrewman != null)
				{
					num -= forCrewman.GetEquippedFor(crewmanEquipmentBase.GetGearType()).GetCost();
				}
				num += crewmanEquipmentBase.GetCost();
				continue;
			}
			int stockForCrewGear = Singleton<SaveDataContainer>.Instance.Get().GetStockForCrewGear(crewmanEquipmentBase);
			if (forCrewman == null)
			{
				num += crewmanEquipmentBase.GetCost();
			}
			else if (forCrewman.GetEquippedFor(crewmanEquipmentBase.GetGearType()) != crewmanEquipmentBase)
			{
				int value = 0;
				stockUsed?.TryGetValue(crewmanEquipmentBase.name, out value);
				if (stockForCrewGear - value == 0)
				{
					num += crewmanEquipmentBase.GetCost();
				}
				else if (stockUsed != null)
				{
					stockUsed[crewmanEquipmentBase.name] = value + 1;
				}
			}
		}
		return num;
	}

	public bool IsAvailableForAllSkills()
	{
		CrewmanEquipmentBase[] allItemsInPreset = m_allItemsInPreset;
		foreach (CrewmanEquipmentBase crewmanEquipmentBase in allItemsInPreset)
		{
			if (!crewmanEquipmentBase.IsAvailableForAllSkills())
			{
				return false;
			}
		}
		return true;
	}

	public int GetIntelShowRequirement()
	{
		int num = 0;
		CrewmanEquipmentBase[] allItemsInPreset = m_allItemsInPreset;
		foreach (CrewmanEquipmentBase crewmanEquipmentBase in allItemsInPreset)
		{
			num = Mathf.Max(num, crewmanEquipmentBase.GetIntelShowRequirement());
		}
		return num;
	}

	public int GetIntelUnlockRequirement()
	{
		int num = 0;
		CrewmanEquipmentBase[] allItemsInPreset = m_allItemsInPreset;
		foreach (CrewmanEquipmentBase crewmanEquipmentBase in allItemsInPreset)
		{
			num = Mathf.Max(num, crewmanEquipmentBase.GetIntelUnlockRequirement());
		}
		return num;
	}

	public bool IsAvailableForSkill(Crewman.SpecialisationSkill sk)
	{
		if (m_lockedToSkills == null || m_lockedToSkills.Length == 0)
		{
			return true;
		}
		Crewman.SpecialisationSkill[] lockedToSkills = m_lockedToSkills;
		foreach (Crewman.SpecialisationSkill specialisationSkill in lockedToSkills)
		{
			if (sk == specialisationSkill)
			{
				return true;
			}
		}
		return false;
	}
}
