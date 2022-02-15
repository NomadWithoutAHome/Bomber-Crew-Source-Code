using BomberCrewCommon;
using UnityEngine;

public class LevelUpNewAbilityDisplay : MonoBehaviour
{
	[SerializeField]
	private TextSetter m_textSetter;

	[SerializeField]
	private GameObject m_starIcon;

	[SerializeField]
	private tk2dSprite m_skillSprite;

	public void SetFromSkill(CrewmanSkillAbilityBool skill, Crewman.SpecialisationSkill fromSkill)
	{
		m_textSetter.SetText(skill.GetTitleText());
		m_starIcon.SetActive(skill.IsStarredSkill());
		m_skillSprite.SetSprite(Singleton<EnumToIconMapping>.Instance.GetIconName(fromSkill));
	}
}
