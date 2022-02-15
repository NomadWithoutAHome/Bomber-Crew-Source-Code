using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class EndlessModeGameFlow : Singleton<EndlessModeGameFlow>
{
	public class EndlessModeResult
	{
		public int m_waveReached;

		public int m_wavesActuallyCompleted;

		public int m_totalScore;
	}

	[SerializeField]
	private EndlessModeVariant m_defaultVariant;

	[SerializeField]
	private EndlessModeVariant[] m_allVariants;

	[SerializeField]
	private CrewmanTrait m_traitNone;

	private EndlessModeVariant m_currentVariant;

	private bool m_hasBeenResetToCurrentEndlessMode;

	private EndlessModeResult m_endlessModeResult = new EndlessModeResult();

	private void Awake()
	{
		m_currentVariant = m_defaultVariant;
	}

	public void SetEndlessModeVariant(EndlessModeVariant variant)
	{
		if (variant != m_currentVariant)
		{
			m_hasBeenResetToCurrentEndlessMode = false;
			m_currentVariant = variant;
		}
	}

	public EndlessModeVariant GetCurrentEndlessMode()
	{
		return m_currentVariant;
	}

	public void ClearEndlessModeScore()
	{
		m_endlessModeResult = new EndlessModeResult();
	}

	public EndlessModeResult GetEndlessModeResult()
	{
		return m_endlessModeResult;
	}

	public void AddScore(int score)
	{
		m_endlessModeResult.m_totalScore += score;
	}

	public void SetLoadout(EndlessModeVariant.LoadoutJS loadout, EndlessModeVariant.Loadout baseLoadout)
	{
		Singleton<SaveDataContainer>.Instance.Get().SetIntel(int.MaxValue);
		Singleton<SaveDataContainer>.Instance.Get().ResetToNewBomber();
		BomberUpgradeConfig currentBomber = Singleton<SaveDataContainer>.Instance.Get().GetCurrentBomber();
		BomberDefaults.RequirementDefault[] requirement = baseLoadout.m_bomber.m_requirement;
		foreach (BomberDefaults.RequirementDefault requirementDefault in requirement)
		{
			EquipmentUpgradeFittableBase equipment = requirementDefault.m_equipment;
			equipment = Singleton<BomberUpgradeCatalogueLoader>.Instance.GetCatalogue().GetByName(equipment.name);
			currentBomber.SetUpgrade(requirementDefault.m_requirementName, equipment);
		}
		for (int j = 0; j < Singleton<CrewContainer>.Instance.GetCurrentCrewCount(); j++)
		{
			CrewmanEquipmentPreset crewmanEquipmentPreset = baseLoadout.m_crewmanEquipmentPresets[j];
			CrewmanEquipmentBase[] allInPreset = crewmanEquipmentPreset.GetAllInPreset();
			foreach (CrewmanEquipmentBase crewmanEquipmentBase in allInPreset)
			{
				Crewman crewman = Singleton<CrewContainer>.Instance.GetCrewman(j);
				crewman.SetEquippedFor(crewmanEquipmentBase.GetGearType(), crewmanEquipmentBase);
			}
		}
		Singleton<SaveDataContainer>.Instance.Get().SetBalance(0);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		Dictionary<string, string> allUpgrades = loadout.m_bomber.GetAllUpgrades();
		foreach (KeyValuePair<string, string> item in allUpgrades)
		{
			dictionary.Add(item.Key, item.Value);
		}
		BomberDefaults.RequirementDefault[] requirement2 = baseLoadout.m_bomber.m_requirement;
		foreach (BomberDefaults.RequirementDefault requirementDefault2 in requirement2)
		{
			if (!dictionary.ContainsKey(requirementDefault2.m_requirementName))
			{
				dictionary[requirementDefault2.m_requirementName] = null;
			}
		}
		foreach (KeyValuePair<string, string> item2 in dictionary)
		{
			EquipmentUpgradeFittableBase byName = Singleton<BomberUpgradeCatalogueLoader>.Instance.GetCatalogue().GetByName(currentBomber.GetUpgradeFor(item2.Key));
			if (byName != null)
			{
				Singleton<SaveDataContainer>.Instance.Get().AddBalance(byName.GetCost());
			}
			if (item2.Value != null)
			{
				EquipmentUpgradeFittableBase byName2 = Singleton<BomberUpgradeCatalogueLoader>.Instance.GetCatalogue().GetByName(item2.Value);
				if (byName2 != null)
				{
					if (item2.Key == "fuselage_section_bay_doors")
					{
						byName2 = Singleton<BomberUpgradeCatalogueLoader>.Instance.GetCatalogue().GetByName("FuselageArmouredMk4");
					}
					currentBomber.SetUpgrade(item2.Key, byName2);
					Singleton<SaveDataContainer>.Instance.Get().AddBalance(-byName2.GetCost());
				}
				else
				{
					currentBomber.SetUpgrade(item2.Key, null);
				}
			}
			else
			{
				currentBomber.SetUpgrade(item2.Key, null);
			}
		}
		Dictionary<string, Dictionary<string, string>> allUpgradeDetails = loadout.m_bomber.GetAllUpgradeDetails();
		foreach (KeyValuePair<string, Dictionary<string, string>> item3 in allUpgradeDetails)
		{
			foreach (KeyValuePair<string, string> item4 in item3.Value)
			{
				currentBomber.SetUpgradeDetail(item3.Key, item4.Key, item4.Value);
			}
		}
		if (loadout.m_crewmanEquipmentPresetDictionary == null || loadout.m_crewmanEquipmentPresetDictionary.Count < Singleton<CrewContainer>.Instance.GetCurrentCrewCount())
		{
			DebugLogWrapper.LogError("[ERR] Trying to load old style loadout! Something odd might happen!");
			return;
		}
		for (int m = 0; m < Singleton<CrewContainer>.Instance.GetCurrentCrewCount(); m++)
		{
			List<string> list = loadout.m_crewmanEquipmentPresetDictionary[m];
			Crewman crewman2 = Singleton<CrewContainer>.Instance.GetCrewman(m);
			foreach (string item5 in list)
			{
				CrewmanEquipmentBase byName3 = Singleton<CrewmanGearCatalogueLoader>.Instance.GetCatalogue().GetByName(item5);
				CrewmanEquipmentBase equippedFor = crewman2.GetEquippedFor(byName3.GetGearType());
				Singleton<SaveDataContainer>.Instance.Get().AddBalance(equippedFor.GetCost());
				crewman2.SetEquippedFor(byName3.GetGearType(), byName3);
				Singleton<SaveDataContainer>.Instance.Get().AddBalance(-byName3.GetCost());
			}
		}
		if (string.IsNullOrEmpty(loadout.m_title))
		{
			string text = Singleton<LanguageProvider>.Instance.GetNamedTextGroupImmediate("bomber_name_default");
			int numPreviousBombersOfName = Singleton<SaveDataContainer>.Instance.Get().GetNumPreviousBombersOfName(text);
			if (numPreviousBombersOfName != 0)
			{
				text = text + " " + (numPreviousBombersOfName + 1);
			}
			currentBomber.SetName(text);
		}
		else
		{
			currentBomber.SetName(loadout.m_title);
		}
		Singleton<BomberContainer>.Instance.GetLivery().Refresh();
		if (Singleton<SaveDataContainer>.Instance.Get().GetBalance() < 0)
		{
			SetLoadout(m_currentVariant.GetPreset(0), m_currentVariant.GetPreset(0));
		}
	}

	public void SetLoadout(EndlessModeVariant.Loadout loadout, EndlessModeVariant.Loadout baseLoadout)
	{
		Singleton<SaveDataContainer>.Instance.Get().SetIntel(int.MaxValue);
		BomberUpgradeConfig currentBomber = Singleton<SaveDataContainer>.Instance.Get().GetCurrentBomber();
		string text = currentBomber.GetName();
		Singleton<SaveDataContainer>.Instance.Get().ResetToNewBomber();
		currentBomber = Singleton<SaveDataContainer>.Instance.Get().GetCurrentBomber();
		BomberDefaults.RequirementDefault[] requirement = baseLoadout.m_bomber.m_requirement;
		foreach (BomberDefaults.RequirementDefault requirementDefault in requirement)
		{
			EquipmentUpgradeFittableBase equipment = requirementDefault.m_equipment;
			equipment = Singleton<BomberUpgradeCatalogueLoader>.Instance.GetCatalogue().GetByName(equipment.name);
			currentBomber.SetUpgrade(requirementDefault.m_requirementName, equipment);
		}
		for (int j = 0; j < Singleton<CrewContainer>.Instance.GetCurrentCrewCount(); j++)
		{
			CrewmanEquipmentPreset crewmanEquipmentPreset = baseLoadout.m_crewmanEquipmentPresets[j];
			CrewmanEquipmentBase[] allInPreset = crewmanEquipmentPreset.GetAllInPreset();
			foreach (CrewmanEquipmentBase crewmanEquipmentBase in allInPreset)
			{
				Crewman crewman = Singleton<CrewContainer>.Instance.GetCrewman(j);
				crewman.SetEquippedFor(crewmanEquipmentBase.GetGearType(), crewmanEquipmentBase);
			}
		}
		Singleton<SaveDataContainer>.Instance.Get().SetBalance(0);
		BomberDefaults.RequirementDefault[] requirement2 = loadout.m_bomber.m_requirement;
		foreach (BomberDefaults.RequirementDefault requirementDefault2 in requirement2)
		{
			EquipmentUpgradeFittableBase byName = Singleton<BomberUpgradeCatalogueLoader>.Instance.GetCatalogue().GetByName(currentBomber.GetUpgradeFor(requirementDefault2.m_requirementName));
			if (byName != null)
			{
				Singleton<SaveDataContainer>.Instance.Get().AddBalance(byName.GetCost());
			}
			EquipmentUpgradeFittableBase equipment2 = requirementDefault2.m_equipment;
			equipment2 = Singleton<BomberUpgradeCatalogueLoader>.Instance.GetCatalogue().GetByName(equipment2.name);
			if (equipment2 != null)
			{
				currentBomber.SetUpgrade(requirementDefault2.m_requirementName, equipment2);
				Singleton<SaveDataContainer>.Instance.Get().AddBalance(-equipment2.GetCost());
			}
			else
			{
				currentBomber.SetUpgrade(requirementDefault2.m_requirementName, null);
			}
		}
		BomberDefaults.RequirementDetail[] details = loadout.m_bomber.m_details;
		foreach (BomberDefaults.RequirementDetail requirementDetail in details)
		{
			currentBomber.SetUpgradeDetail(requirementDetail.m_slotName, requirementDetail.m_detailName, requirementDetail.m_detailValue);
		}
		for (int n = 0; n < Singleton<CrewContainer>.Instance.GetCurrentCrewCount(); n++)
		{
			CrewmanEquipmentPreset crewmanEquipmentPreset2 = loadout.m_crewmanEquipmentPresets[n];
			Crewman crewman2 = Singleton<CrewContainer>.Instance.GetCrewman(n);
			CrewmanEquipmentBase[] allInPreset2 = crewmanEquipmentPreset2.GetAllInPreset();
			foreach (CrewmanEquipmentBase crewmanEquipmentBase2 in allInPreset2)
			{
				CrewmanEquipmentBase equippedFor = crewman2.GetEquippedFor(crewmanEquipmentBase2.GetGearType());
				Singleton<SaveDataContainer>.Instance.Get().AddBalance(equippedFor.GetCost());
				crewman2.SetEquippedFor(crewmanEquipmentBase2.GetGearType(), crewmanEquipmentBase2);
				Singleton<SaveDataContainer>.Instance.Get().AddBalance(-crewmanEquipmentBase2.GetCost());
			}
		}
		string text2 = Singleton<LanguageProvider>.Instance.GetNamedTextGroupImmediate("bomber_name_default");
		int numPreviousBombersOfName = Singleton<SaveDataContainer>.Instance.Get().GetNumPreviousBombersOfName(text2);
		if (numPreviousBombersOfName != 0)
		{
			text2 = text2 + " " + (numPreviousBombersOfName + 1);
		}
		currentBomber.SetName(text2);
		Singleton<BomberContainer>.Instance.GetLivery().Refresh();
	}

	public void ResetEndlessMode()
	{
		while (Singleton<CrewContainer>.Instance.GetCurrentCrewCount() < Singleton<GameFlow>.Instance.GetGameMode().GetMaxCrew())
		{
			Singleton<CrewContainer>.Instance.HireNewCrewmanAuto();
		}
		for (int i = 0; i < Singleton<CrewContainer>.Instance.GetCurrentCrewCount(); i++)
		{
			Singleton<CrewContainer>.Instance.GetCrewman(i).GetPrimarySkill().ResetXP();
			Singleton<CrewContainer>.Instance.GetCrewman(i).SetTrait(m_traitNone);
		}
		if (!m_hasBeenResetToCurrentEndlessMode)
		{
			Singleton<SaveDataContainer>.Instance.Get().SetBalance(0);
			Singleton<SaveDataContainer>.Instance.Get().SetIntel(int.MaxValue);
			EndlessModeVariant.LoadoutJS loadoutJS = Singleton<SaveDataContainer>.Instance.Get().LastSeenEndlessLoadout(m_currentVariant.name);
			if (loadoutJS == null)
			{
				SetLoadout(m_currentVariant.GetPreset(0), m_currentVariant.GetPreset(0));
			}
			else
			{
				SetLoadout(loadoutJS, m_currentVariant.GetPreset(0));
			}
		}
		m_hasBeenResetToCurrentEndlessMode = true;
	}
}
