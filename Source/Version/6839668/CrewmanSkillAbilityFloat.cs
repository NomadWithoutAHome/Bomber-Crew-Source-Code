using UnityEngine;

[CreateAssetMenu(menuName = "Bomber Crew/Skill Abilities/Skill Ability Float")]
public class CrewmanSkillAbilityFloat : ScriptableObject
{
	[SerializeField]
	[NamedText]
	private string m_namedTextIdentifier;

	private string m_cachedName;

	public string GetCachedName()
	{
		if (string.IsNullOrEmpty(m_cachedName))
		{
			m_cachedName = base.name;
		}
		return m_cachedName;
	}
}
