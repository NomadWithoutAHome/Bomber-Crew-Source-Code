using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class CrewmanSkillUpgradeInfo : Singleton<CrewmanSkillUpgradeInfo>
{
	[SerializeField]
	private CrewmanSkillUpgradeInfoData m_data;

	private Dictionary<int, Dictionary<string, int>> m_boolSkillLookupTable = new Dictionary<int, Dictionary<string, int>>();

	private Dictionary<int, Dictionary<string, List<float>>> m_floatSkillLookupTable = new Dictionary<int, Dictionary<string, List<float>>>();

	private Dictionary<int, Dictionary<CrewmanSkillAbilityBool, int>> m_boolSkillLookupTableRaw = new Dictionary<int, Dictionary<CrewmanSkillAbilityBool, int>>();

	private Dictionary<int, string> m_namedTextForSkill = new Dictionary<int, string>();

	private void Awake()
	{
		CrewmanSkillUpgradeInfoData.SkillPath[] allSkillPaths = m_data.GetAllSkillPaths();
		foreach (CrewmanSkillUpgradeInfoData.SkillPath skillPath in allSkillPaths)
		{
			m_boolSkillLookupTable[(int)skillPath.m_selectedSkill] = new Dictionary<string, int>();
			m_boolSkillLookupTableRaw[(int)skillPath.m_selectedSkill] = new Dictionary<CrewmanSkillAbilityBool, int>();
			m_floatSkillLookupTable[(int)skillPath.m_selectedSkill] = new Dictionary<string, List<float>>();
			m_namedTextForSkill[(int)skillPath.m_selectedSkill] = skillPath.m_namedText;
			CrewmanSkillUpgradeInfoData.BoolAbilityAtSkillLevel[] abilitiesToUnlock = skillPath.m_abilitiesToUnlock;
			foreach (CrewmanSkillUpgradeInfoData.BoolAbilityAtSkillLevel boolAbilityAtSkillLevel in abilitiesToUnlock)
			{
				m_boolSkillLookupTable[(int)skillPath.m_selectedSkill][boolAbilityAtSkillLevel.m_skill.GetCachedName()] = boolAbilityAtSkillLevel.m_level;
				m_boolSkillLookupTableRaw[(int)skillPath.m_selectedSkill][boolAbilityAtSkillLevel.m_skill] = boolAbilityAtSkillLevel.m_level;
			}
			CrewmanSkillUpgradeInfoData.FloatAbilityBetweenSkillLevels[] abilitiesToScale = skillPath.m_abilitiesToScale;
			foreach (CrewmanSkillUpgradeInfoData.FloatAbilityBetweenSkillLevels floatAbilityBetweenSkillLevels in abilitiesToScale)
			{
				string text = floatAbilityBetweenSkillLevels.m_skill.name;
				m_floatSkillLookupTable[(int)skillPath.m_selectedSkill][floatAbilityBetweenSkillLevels.m_skill.GetCachedName()] = new List<float>();
				for (int l = 0; l < floatAbilityBetweenSkillLevels.m_levelAtZero; l++)
				{
					m_floatSkillLookupTable[(int)skillPath.m_selectedSkill][floatAbilityBetweenSkillLevels.m_skill.GetCachedName()].Add(0f);
				}
				for (int m = floatAbilityBetweenSkillLevels.m_levelAtZero; m <= floatAbilityBetweenSkillLevels.m_levelAtOne; m++)
				{
					float t = (float)(m - floatAbilityBetweenSkillLevels.m_levelAtZero) / (float)(floatAbilityBetweenSkillLevels.m_levelAtOne - floatAbilityBetweenSkillLevels.m_levelAtZero);
					float item = Mathf.Lerp(floatAbilityBetweenSkillLevels.m_min, 1f, t);
					m_floatSkillLookupTable[(int)skillPath.m_selectedSkill][floatAbilityBetweenSkillLevels.m_skill.GetCachedName()].Add(item);
				}
			}
		}
	}

	public string GetNamedText(Crewman.SpecialisationSkill skill)
	{
		return m_namedTextForSkill[(int)skill];
	}

	public bool IsUnlocked(CrewmanSkillAbilityBool ability, Crewman crewman)
	{
		return IsUnlocked(ability, crewman.GetPrimarySkill()) || IsUnlocked(ability, crewman.GetSecondarySkill());
	}

	public bool IsUnlocked(CrewmanSkillAbilityBool ability, Crewman.Skill primary, Crewman.Skill secondary)
	{
		return IsUnlocked(ability, primary) || IsUnlocked(ability, secondary);
	}

	public int GetUnlockLevel(CrewmanSkillAbilityBool ability, Crewman.SpecialisationSkill skillType)
	{
		Dictionary<string, int> value = null;
		m_boolSkillLookupTable.TryGetValue((int)skillType, out value);
		if (value != null)
		{
			int value2 = -1;
			if (value.TryGetValue(ability.GetCachedName(), out value2))
			{
				return value2;
			}
		}
		return -1;
	}

	public int GetSecondarySkillUnlocksAt(Crewman.SpecialisationSkill primarySkill)
	{
		CrewmanSkillUpgradeInfoData.SkillPath[] allSkillPaths = m_data.GetAllSkillPaths();
		CrewmanSkillUpgradeInfoData.SkillPath[] array = allSkillPaths;
		foreach (CrewmanSkillUpgradeInfoData.SkillPath skillPath in array)
		{
			if (skillPath.m_selectedSkill == primarySkill)
			{
				return skillPath.m_unlockSecondarySkillAt;
			}
		}
		return -1;
	}

	public Crewman.SpecialisationSkill[] SecondarySkillsAvailable(Crewman.SpecialisationSkill primarySkill)
	{
		CrewmanSkillUpgradeInfoData.SkillPath[] allSkillPaths = m_data.GetAllSkillPaths();
		CrewmanSkillUpgradeInfoData.SkillPath[] array = allSkillPaths;
		foreach (CrewmanSkillUpgradeInfoData.SkillPath skillPath in array)
		{
			if (skillPath.m_selectedSkill == primarySkill)
			{
				return skillPath.m_secondarySkillsAvailable;
			}
		}
		return null;
	}

	public int GetRank(Crewman cm)
	{
		CrewmanSkillUpgradeInfoData.SkillPath[] allSkillPaths = m_data.GetAllSkillPaths();
		CrewmanSkillUpgradeInfoData.SkillPath[] array = allSkillPaths;
		foreach (CrewmanSkillUpgradeInfoData.SkillPath skillPath in array)
		{
			if (skillPath.m_selectedSkill == cm.GetPrimarySkill().GetSkill())
			{
				if (skillPath.m_crewmanRankAtLevel.Length >= cm.GetPrimarySkill().GetLevel())
				{
					return skillPath.m_crewmanRankAtLevel[cm.GetPrimarySkill().GetLevel() - 1];
				}
				return 0;
			}
		}
		return 0;
	}

	public string GetRankTranslated(SystemData.CrewmanLite cm)
	{
		CrewmanSkillUpgradeInfoData.SkillPath[] allSkillPaths = m_data.GetAllSkillPaths();
		CrewmanSkillUpgradeInfoData.SkillPath[] array = allSkillPaths;
		foreach (CrewmanSkillUpgradeInfoData.SkillPath skillPath in array)
		{
			if (skillPath.m_selectedSkill == cm.GetPrimarySkill().GetSkill())
			{
				if (skillPath.m_crewmanRankNamedTextPrefix != null && skillPath.m_crewmanRankAtLevel.Length >= cm.GetPrimarySkill().GetLevel())
				{
					string key = skillPath.m_crewmanRankNamedTextPrefix + skillPath.m_crewmanRankAtLevel[cm.GetPrimarySkill().GetLevel() - 1];
					return Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(key);
				}
				return "*UNSET_RANK*";
			}
		}
		return "*UNSET_RANK*";
	}

	public string GetRankTranslated(Crewman cm)
	{
		CrewmanSkillUpgradeInfoData.SkillPath[] allSkillPaths = m_data.GetAllSkillPaths();
		CrewmanSkillUpgradeInfoData.SkillPath[] array = allSkillPaths;
		foreach (CrewmanSkillUpgradeInfoData.SkillPath skillPath in array)
		{
			if (skillPath.m_selectedSkill == cm.GetPrimarySkill().GetSkill())
			{
				if (skillPath.m_crewmanRankNamedTextPrefix != null && skillPath.m_crewmanRankAtLevel.Length >= cm.GetPrimarySkill().GetLevel())
				{
					string key = skillPath.m_crewmanRankNamedTextPrefix + skillPath.m_crewmanRankAtLevel[cm.GetPrimarySkill().GetLevel() - 1];
					return Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(key);
				}
				return "*UNSET_RANK*";
			}
		}
		return "*UNSET_RANK*";
	}

	private bool IsUnlocked(CrewmanSkillAbilityBool ability, Crewman.Skill forSkill)
	{
		bool result = false;
		if (forSkill != null)
		{
			Dictionary<string, int> value = null;
			m_boolSkillLookupTable.TryGetValue((int)forSkill.GetSkill(), out value);
			if (value != null)
			{
				int value2 = -1;
				if (value.TryGetValue(ability.GetCachedName(), out value2))
				{
					result = value2 <= forSkill.GetLevel();
				}
			}
		}
		return result;
	}

	public List<CrewmanSkillAbilityBool> GetSkillsUnlockedExactlyAt(Crewman.Skill forSkill)
	{
		List<CrewmanSkillAbilityBool> list = new List<CrewmanSkillAbilityBool>();
		if (forSkill != null)
		{
			Dictionary<CrewmanSkillAbilityBool, int> value = null;
			m_boolSkillLookupTableRaw.TryGetValue((int)forSkill.GetSkill(), out value);
			if (value != null)
			{
				foreach (KeyValuePair<CrewmanSkillAbilityBool, int> item in value)
				{
					if (item.Value == forSkill.GetLevel())
					{
						list.Add(item.Key);
					}
				}
				return list;
			}
		}
		return list;
	}

	public Dictionary<CrewmanSkillAbilityBool, int> GetAllSkills(Crewman.Skill forSkill)
	{
		Dictionary<CrewmanSkillAbilityBool, int> dictionary = new Dictionary<CrewmanSkillAbilityBool, int>();
		if (forSkill != null)
		{
			Dictionary<CrewmanSkillAbilityBool, int> value = null;
			m_boolSkillLookupTableRaw.TryGetValue((int)forSkill.GetSkill(), out value);
			if (value != null)
			{
				foreach (KeyValuePair<CrewmanSkillAbilityBool, int> item in value)
				{
					dictionary.Add(item.Key, item.Value);
				}
				return dictionary;
			}
		}
		return dictionary;
	}

	public float GetProficiencySlow(CrewmanSkillAbilityFloat ability, Crewman crewman)
	{
		return GetProficiencySlow(ability, crewman.GetPrimarySkill(), crewman.GetSecondarySkill());
	}

	public float GetProficiencySlow(CrewmanSkillAbilityFloat ability, Crewman.Skill primary, Crewman.Skill secondary)
	{
		return Mathf.Max(GetProficiencySlow(ability, primary), GetProficiencySlow(ability, secondary));
	}

	public float GetProficiency(string abilityIdName, Crewman crewman)
	{
		return GetProficiency(abilityIdName, crewman.GetPrimarySkill(), crewman.GetSecondarySkill());
	}

	public float GetProficiency(string abilityIdName, Crewman.Skill primary, Crewman.Skill secondary)
	{
		return Mathf.Max(GetProficiency(abilityIdName, primary), GetProficiency(abilityIdName, secondary));
	}

	private float GetProficiency(string abilityIdName, Crewman.Skill forSkill)
	{
		float result = 0f;
		if (forSkill != null)
		{
			Dictionary<string, List<float>> value = null;
			m_floatSkillLookupTable.TryGetValue((int)forSkill.GetSkill(), out value);
			if (value != null)
			{
				List<float> value2 = null;
				value.TryGetValue(abilityIdName, out value2);
				if (value2 != null && value2.Count >= 1)
				{
					int index = Mathf.Max(forSkill.GetLevel(), value.Count - 1);
					result = value2[index];
				}
			}
		}
		return result;
	}

	private float GetProficiencySlow(CrewmanSkillAbilityFloat ability, Crewman.Skill forSkill)
	{
		float result = 0f;
		if (forSkill != null)
		{
			Dictionary<string, List<float>> value = null;
			m_floatSkillLookupTable.TryGetValue((int)forSkill.GetSkill(), out value);
			if (value != null)
			{
				List<float> value2 = null;
				value.TryGetValue(ability.GetCachedName(), out value2);
				if (value2 != null && value2.Count >= 1)
				{
					int index = Mathf.Max(forSkill.GetLevel(), value.Count - 1);
					result = value2[index];
				}
			}
		}
		return result;
	}
}
