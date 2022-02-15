using BomberCrewCommon;
using UnityEngine;

public class AbilityLockUnlockDisplay : MonoBehaviour
{
	[SerializeField]
	private TextSetter m_textSetter;

	[SerializeField]
	private GameObject m_starIcon;

	[SerializeField]
	private tk2dSprite m_skillSprite;

	[SerializeField]
	private GameObject m_lockedHierarchy;

	[SerializeField]
	private GameObject m_unlockedHierarchy;

	private Crewman m_currentCrewman;

	private int m_levelUnlockedAt;

	private Crewman.SpecialisationSkill m_skillToCheck;

	public void SetFromSkill(CrewmanSkillAbilityBool skill, Crewman.SpecialisationSkill fromSkill, bool unlocked)
	{
		m_textSetter.SetText(skill.GetTitleText().ToUpper());
		m_starIcon.SetActive(skill.IsStarredSkill());
		m_skillSprite.SetSprite(Singleton<EnumToIconMapping>.Instance.GetIconName(fromSkill));
		m_lockedHierarchy.SetActive(!unlocked);
		m_unlockedHierarchy.SetActive(unlocked);
	}
}
