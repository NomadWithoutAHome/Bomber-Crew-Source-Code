using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Bomber Crew/Skill Abilities/Skill Upgrade Paths")]
public class CrewmanSkillUpgradeInfoData : ScriptableObject
{
	[Serializable]
	public class SkillPath
	{
		[SerializeField]
		public Crewman.SpecialisationSkill m_selectedSkill;

		[SerializeField]
		public BoolAbilityAtSkillLevel[] m_abilitiesToUnlock;

		[SerializeField]
		public FloatAbilityBetweenSkillLevels[] m_abilitiesToScale;

		[SerializeField]
		public string m_crewmanRankNamedTextPrefix;

		[SerializeField]
		public int[] m_crewmanRankAtLevel;

		[SerializeField]
		public int m_unlockSecondarySkillAt;

		[SerializeField]
		public Crewman.SpecialisationSkill[] m_secondarySkillsAvailable;

		[SerializeField]
		[NamedText]
		public string m_namedText;
	}

	[Serializable]
	public class BoolAbilityAtSkillLevel
	{
		[SerializeField]
		public CrewmanSkillAbilityBool m_skill;

		[SerializeField]
		public int m_level;
	}

	[Serializable]
	public class FloatAbilityBetweenSkillLevels
	{
		[SerializeField]
		public CrewmanSkillAbilityFloat m_skill;

		[SerializeField]
		public int m_levelAtZero;

		[SerializeField]
		public int m_levelAtOne;

		[SerializeField]
		public float m_min;
	}

	[SerializeField]
	private SkillPath[] m_allSkillPaths;

	public SkillPath[] GetAllSkillPaths()
	{
		return m_allSkillPaths;
	}
}
