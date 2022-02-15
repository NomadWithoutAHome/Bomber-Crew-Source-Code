using System;
using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class CrewContainer : Singleton<CrewContainer>
{
	[Serializable]
	public class RecruitmentInfo
	{
		[SerializeField]
		public int m_minIntel;

		[SerializeField]
		public int m_minXP;

		[SerializeField]
		public int m_maxXP;
	}

	[SerializeField]
	private NameAndBackstoryGenerator m_nameGenerator;

	[SerializeField]
	private RecruitmentInfo[] m_recruitmentInfo;

	public event Action OnNewCrewman;

	public event Action OnCrewmanRemoved;

	public int GetCurrentCrewCount()
	{
		return Singleton<SaveDataContainer>.Instance.Get().GetActiveCrewmen().Count;
	}

	public Crewman GetCrewman(int index)
	{
		return Singleton<SaveDataContainer>.Instance.Get().GetCrewmanByIndex(index);
	}

	public RecruitmentInfo GetCurrentRecruitmentInfo()
	{
		int num = 0;
		RecruitmentInfo result = null;
		RecruitmentInfo[] recruitmentInfo = m_recruitmentInfo;
		foreach (RecruitmentInfo recruitmentInfo2 in recruitmentInfo)
		{
			if (recruitmentInfo2.m_minIntel > num && recruitmentInfo2.m_minIntel <= Singleton<SaveDataContainer>.Instance.Get().GetIntel())
			{
				result = recruitmentInfo2;
				num = recruitmentInfo2.m_minIntel;
			}
		}
		return result;
	}

	public void HireNewCrewmanAuto()
	{
		List<Crewman> activeCrewmen = Singleton<SaveDataContainer>.Instance.Get().GetActiveCrewmen();
		if (activeCrewmen.Count < Singleton<GameFlow>.Instance.GetGameMode().GetMaxCrew())
		{
			List<Crewman.SpecialisationSkill> list = new List<Crewman.SpecialisationSkill>();
			list.AddRange(Singleton<GameFlow>.Instance.GetGameMode().GetCrewRequirements());
			foreach (Crewman item in activeCrewmen)
			{
				list.Remove(item.GetPrimarySkill().GetSkill());
			}
			Crewman.Skill primarySkill = new Crewman.Skill(list[0], 1);
			Crewman crewman = new Crewman(primarySkill, null);
			CrewmanEquipmentPreset crewmanEquipmentPreset = null;
			CrewmanEquipmentPreset[] equipmentPresets = Singleton<GameFlow>.Instance.GetGameMode().GetEquipmentPresets();
			foreach (CrewmanEquipmentPreset crewmanEquipmentPreset2 in equipmentPresets)
			{
				if (crewmanEquipmentPreset2.IsAvailableForSkill(list[0]))
				{
					crewmanEquipmentPreset = crewmanEquipmentPreset2;
				}
			}
			CrewmanEquipmentBase[] allInPreset = crewmanEquipmentPreset.GetAllInPreset();
			foreach (CrewmanEquipmentBase crewmanEquipmentBase in allInPreset)
			{
				crewman.SetEquippedFor(crewmanEquipmentBase.GetGearType(), crewmanEquipmentBase);
			}
			Singleton<SaveDataContainer>.Instance.Get().AddNewCrewman(crewman);
		}
		SortBySpecialisation();
		if (this.OnNewCrewman != null)
		{
			this.OnNewCrewman();
		}
	}

	public void GiveDefaultEquipment()
	{
		int currentCrewCount = GetCurrentCrewCount();
		for (int i = 0; i < currentCrewCount; i++)
		{
			Crewman crewman = GetCrewman(i);
			CrewmanEquipmentPreset crewmanEquipmentPreset = null;
			CrewmanEquipmentPreset[] equipmentPresets = Singleton<GameFlow>.Instance.GetGameMode().GetEquipmentPresets();
			foreach (CrewmanEquipmentPreset crewmanEquipmentPreset2 in equipmentPresets)
			{
				if (crewmanEquipmentPreset2.IsAvailableForSkill(crewman.GetPrimarySkill().GetSkill()))
				{
					crewmanEquipmentPreset = crewmanEquipmentPreset2;
				}
			}
			CrewmanEquipmentBase[] allInPreset = crewmanEquipmentPreset.GetAllInPreset();
			foreach (CrewmanEquipmentBase crewmanEquipmentBase in allInPreset)
			{
				crewman.SetEquippedFor(crewmanEquipmentBase.GetGearType(), crewmanEquipmentBase);
			}
		}
	}

	public void SetDefaultEquipmentTo(Crewman cm)
	{
		CrewmanEquipmentPreset crewmanEquipmentPreset = null;
		CrewmanEquipmentPreset[] equipmentPresets = Singleton<GameFlow>.Instance.GetGameMode().GetEquipmentPresets();
		foreach (CrewmanEquipmentPreset crewmanEquipmentPreset2 in equipmentPresets)
		{
			if (crewmanEquipmentPreset2.IsAvailableForSkill(cm.GetPrimarySkill().GetSkill()))
			{
				crewmanEquipmentPreset = crewmanEquipmentPreset2;
			}
		}
		CrewmanEquipmentBase[] allInPreset = crewmanEquipmentPreset.GetAllInPreset();
		foreach (CrewmanEquipmentBase crewmanEquipmentBase in allInPreset)
		{
			cm.SetEquippedFor(crewmanEquipmentBase.GetGearType(), crewmanEquipmentBase);
		}
	}

	public void HireNewCrewman(Crewman crewman)
	{
		List<Crewman> activeCrewmen = Singleton<SaveDataContainer>.Instance.Get().GetActiveCrewmen();
		if (activeCrewmen.Count < Singleton<GameFlow>.Instance.GetGameMode().GetMaxCrew())
		{
			Singleton<SaveDataContainer>.Instance.Get().AddNewCrewman(crewman);
		}
		crewman.RefreshPortrait();
		SortBySpecialisation();
		if (this.OnNewCrewman != null)
		{
			this.OnNewCrewman();
		}
	}

	public void SetCrewUpdated()
	{
		if (this.OnCrewmanRemoved != null)
		{
			this.OnCrewmanRemoved();
		}
	}

	private void SortBySpecialisation()
	{
		List<Crewman> list = new List<Crewman>();
		List<Crewman> list2 = new List<Crewman>();
		list.AddRange(Singleton<SaveDataContainer>.Instance.Get().GetActiveCrewmen());
		Crewman.SpecialisationSkill[] crewRequirements = Singleton<GameFlow>.Instance.GetGameMode().GetCrewRequirements();
		foreach (Crewman.SpecialisationSkill specialisationSkill in crewRequirements)
		{
			Crewman crewman = null;
			foreach (Crewman item in list)
			{
				if (item.GetPrimarySkill().GetSkill() == specialisationSkill)
				{
					crewman = item;
					break;
				}
			}
			if (crewman != null)
			{
				list.Remove(crewman);
				list2.Add(crewman);
			}
		}
		Singleton<SaveDataContainer>.Instance.Get().GetActiveCrewmen().Clear();
		Singleton<SaveDataContainer>.Instance.Get().GetActiveCrewmen().AddRange(list2);
	}

	public void RemoveDeadCrewmen()
	{
		List<Crewman> list = new List<Crewman>();
		List<Crewman> activeCrewmen = Singleton<SaveDataContainer>.Instance.Get().GetActiveCrewmen();
		foreach (Crewman item in activeCrewmen)
		{
			if (item.IsDead())
			{
				list.Add(item);
			}
		}
		foreach (Crewman item2 in list)
		{
			Singleton<SaveDataContainer>.Instance.Get().MoveCrewmenToPast(item2);
		}
		SortBySpecialisation();
		if (this.OnCrewmanRemoved != null)
		{
			this.OnCrewmanRemoved();
		}
	}

	public void EndlessResurrectCrewmen()
	{
		List<Crewman> activeCrewmen = Singleton<SaveDataContainer>.Instance.Get().GetActiveCrewmen();
		foreach (Crewman item in activeCrewmen)
		{
			if (item.IsDead())
			{
				item.MagicallyResurrect();
			}
		}
	}

	private void Start()
	{
	}

	private void CrewDebug()
	{
	}
}
