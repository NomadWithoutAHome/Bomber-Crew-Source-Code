using BomberCrewCommon;
using UnityEngine;

public class NewSkillSummaryDisplay : MonoBehaviour
{
	[SerializeField]
	private TextSetter m_textSetter;

	[SerializeField]
	private TextSetter m_descriptionTextSetter;

	[SerializeField]
	private GameObject m_starIcon;

	[SerializeField]
	private tk2dSprite m_skillSprite;

	public void SetUp(CrewmanSkillAbilityBool skillAbility, Crewman.SpecialisationSkill fromSkill)
	{
		m_textSetter.SetText(skillAbility.GetTitleText());
		m_descriptionTextSetter.SetText(skillAbility.GetDescriptionText());
		m_starIcon.SetActive(skillAbility.IsStarredSkill());
		m_skillSprite.SetSprite(Singleton<EnumToIconMapping>.Instance.GetIconName(fromSkill));
	}
}
