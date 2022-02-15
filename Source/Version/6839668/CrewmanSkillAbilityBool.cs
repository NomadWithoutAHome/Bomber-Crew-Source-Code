using BomberCrewCommon;
using UnityEngine;

[CreateAssetMenu(menuName = "Bomber Crew/Skill Abilities/Skill Ability Bool")]
public class CrewmanSkillAbilityBool : ScriptableObject
{
	[SerializeField]
	[NamedText]
	private string m_namedTextIdentifier;

	[SerializeField]
	[NamedText]
	private string m_descriptionTextIdentifier;

	[SerializeField]
	private bool m_useAlternativeUSText;

	[SerializeField]
	[NamedText]
	private string m_namedTextIdentifierUS;

	[SerializeField]
	[NamedText]
	private string m_descriptionTextIdentifierUS;

	[SerializeField]
	private float m_cooldownTimer;

	[SerializeField]
	private float m_durationTimer;

	[SerializeField]
	private bool m_isStarredSkill;

	private string m_cachedName;

	public float GetCooldownTimer()
	{
		return m_cooldownTimer;
	}

	public float GetDurationTimer()
	{
		return m_durationTimer;
	}

	public string GetCachedName()
	{
		if (string.IsNullOrEmpty(m_cachedName))
		{
			m_cachedName = base.name;
		}
		return m_cachedName;
	}

	public string GetDescriptionText()
	{
		if (m_useAlternativeUSText && Singleton<GameFlow>.Instance.GetGameMode().GetUseUSNaming())
		{
			if (!string.IsNullOrEmpty(m_descriptionTextIdentifierUS))
			{
				return Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_descriptionTextIdentifierUS);
			}
			return string.Empty;
		}
		if (!string.IsNullOrEmpty(m_descriptionTextIdentifier))
		{
			return Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_descriptionTextIdentifier);
		}
		return string.Empty;
	}

	public string GetTitleText()
	{
		if (m_useAlternativeUSText && Singleton<GameFlow>.Instance.GetGameMode().GetUseUSNaming())
		{
			return Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_namedTextIdentifierUS);
		}
		return Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_namedTextIdentifier);
	}

	public bool IsStarredSkill()
	{
		return m_isStarredSkill;
	}
}
