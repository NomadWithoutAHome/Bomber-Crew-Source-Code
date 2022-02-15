using System.Collections.Generic;
using BomberCrewCommon;
using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class BomberUpgradeConfig
{
	[JsonProperty]
	private Dictionary<string, string> m_upgradedTypes = new Dictionary<string, string>();

	[JsonProperty]
	private Dictionary<string, Dictionary<string, string>> m_upgradeDetails = new Dictionary<string, Dictionary<string, string>>();

	[JsonProperty]
	private string m_bomberName;

	public BomberUpgradeConfig()
	{
	}

	public BomberUpgradeConfig(BomberUpgradeConfig copyFrom)
	{
		m_upgradedTypes = new Dictionary<string, string>();
		foreach (KeyValuePair<string, string> upgradedType in copyFrom.m_upgradedTypes)
		{
			m_upgradedTypes.Add(upgradedType.Key, upgradedType.Value);
		}
		m_bomberName = copyFrom.m_bomberName;
		m_upgradeDetails = new Dictionary<string, Dictionary<string, string>>();
		foreach (KeyValuePair<string, Dictionary<string, string>> upgradeDetail in copyFrom.m_upgradeDetails)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (KeyValuePair<string, string> item in upgradeDetail.Value)
			{
				dictionary.Add(item.Key, item.Value);
			}
			m_upgradeDetails.Add(upgradeDetail.Key, dictionary);
		}
	}

	public string GetUpgradeFor(string upgradeableType)
	{
		if (m_upgradedTypes == null)
		{
			m_upgradedTypes = new Dictionary<string, string>();
		}
		string value = null;
		m_upgradedTypes.TryGetValue(upgradeableType, out value);
		return value;
	}

	public Dictionary<string, string> GetAllUpgrades()
	{
		return m_upgradedTypes;
	}

	public Dictionary<string, Dictionary<string, string>> GetAllUpgradeDetails()
	{
		return m_upgradeDetails;
	}

	public EquipmentUpgradeFittableBase GetUpgradeFor(BomberRequirements.BomberEquipmentRequirement req)
	{
		string upgradeFor = GetUpgradeFor(req.GetUniquePartId());
		if (!string.IsNullOrEmpty(upgradeFor))
		{
			return Singleton<BomberUpgradeCatalogueLoader>.Instance.GetCatalogue().GetByName(upgradeFor);
		}
		return null;
	}

	public bool ContainsUpgrade(string name)
	{
		foreach (string value in m_upgradedTypes.Values)
		{
			if (value == name)
			{
				return true;
			}
		}
		return false;
	}

	public void BuildFrom(BomberRequirements br, SaveData sd)
	{
		m_upgradedTypes = new Dictionary<string, string>();
		BomberRequirements.BomberEquipmentRequirement[] requirements = br.GetRequirements();
		BomberRequirements.BomberEquipmentRequirement[] array = requirements;
		foreach (BomberRequirements.BomberEquipmentRequirement bomberEquipmentRequirement in array)
		{
			string uniquePartId = bomberEquipmentRequirement.GetUniquePartId();
			if (bomberEquipmentRequirement.GetDefault() != null)
			{
				m_upgradedTypes[uniquePartId] = bomberEquipmentRequirement.GetDefault().name;
			}
		}
		string[] defaultItemDetails = br.GetDefaultItemDetails();
		string[] array2 = defaultItemDetails;
		foreach (string text in array2)
		{
			string[] array3 = text.Split(",".ToCharArray());
			SetUpgradeDetail(array3[0], array3[1], array3[2]);
		}
		if (string.IsNullOrEmpty(m_bomberName))
		{
			m_bomberName = Singleton<LanguageProvider>.Instance.GetNamedTextGroupImmediate("bomber_name_default");
			int numPreviousBombersOfName = sd.GetNumPreviousBombersOfName(m_bomberName);
			if (numPreviousBombersOfName != 0)
			{
				m_bomberName = m_bomberName + " " + (numPreviousBombersOfName + 1);
			}
		}
	}

	public void PatchFrom(BomberRequirements br, SaveData sd)
	{
		BomberRequirements.BomberEquipmentRequirement[] requirements = br.GetRequirements();
		BomberRequirements.BomberEquipmentRequirement[] array = requirements;
		foreach (BomberRequirements.BomberEquipmentRequirement bomberEquipmentRequirement in array)
		{
			string uniquePartId = bomberEquipmentRequirement.GetUniquePartId();
			if (!m_upgradedTypes.ContainsKey(uniquePartId))
			{
				if (bomberEquipmentRequirement.GetDefault() != null)
				{
					m_upgradedTypes[uniquePartId] = bomberEquipmentRequirement.GetDefault().name;
				}
			}
			else if (!string.IsNullOrEmpty(m_upgradedTypes[uniquePartId]) && Singleton<BomberUpgradeCatalogueLoader>.Instance.GetCatalogue().GetByName(m_upgradedTypes[uniquePartId]) == null)
			{
				if (bomberEquipmentRequirement.GetDefault() != null)
				{
					m_upgradedTypes[uniquePartId] = bomberEquipmentRequirement.GetDefault().name;
				}
				else
				{
					m_upgradeDetails.Remove(uniquePartId);
				}
			}
		}
		if (string.IsNullOrEmpty(m_bomberName))
		{
			m_bomberName = Singleton<LanguageProvider>.Instance.GetNamedTextGroupImmediate("bomber_name_default");
			int numPreviousBombersOfName = sd.GetNumPreviousBombersOfName(m_bomberName);
			if (numPreviousBombersOfName != 0)
			{
				m_bomberName = m_bomberName + " " + (numPreviousBombersOfName + 1);
			}
		}
	}

	public void SetUpgradeDebug(string slotId, string typeName)
	{
		m_upgradedTypes[slotId] = typeName;
	}

	public void SetUpgrade(string slotId, EquipmentUpgradeFittableBase fromType)
	{
		if (fromType == null)
		{
			m_upgradedTypes.Remove(slotId);
		}
		else
		{
			m_upgradedTypes[slotId] = fromType.name;
		}
	}

	public void SetUpgradeDetail(string slotId, string detail, string value)
	{
		Dictionary<string, string> value2 = null;
		m_upgradeDetails.TryGetValue(slotId, out value2);
		if (value2 == null)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			m_upgradeDetails[slotId] = dictionary;
			value2 = dictionary;
		}
		value2[detail] = value;
	}

	public Dictionary<string, string> GetUpgradeDetail(string slotId)
	{
		Dictionary<string, string> value = null;
		m_upgradeDetails.TryGetValue(slotId, out value);
		return value;
	}

	public string GetName()
	{
		return m_bomberName;
	}

	public void SetName(string nameIn)
	{
		m_bomberName = nameIn;
	}

	public string GetUpgradeDetail(string slotId, string detail)
	{
		string value = null;
		Dictionary<string, string> value2 = null;
		m_upgradeDetails.TryGetValue(slotId, out value2);
		value2?.TryGetValue(detail, out value);
		return value;
	}
}
