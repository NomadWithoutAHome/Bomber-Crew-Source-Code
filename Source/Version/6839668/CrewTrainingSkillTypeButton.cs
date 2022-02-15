using BomberCrewCommon;
using UnityEngine;

public class CrewTrainingSkillTypeButton : MonoBehaviour
{
	[SerializeField]
	private tk2dSprite m_skillSprite;

	[SerializeField]
	private TextSetter m_skillNameText;

	public void SetUp(Crewman.SpecialisationSkill skillType)
	{
		m_skillSprite.SetSprite(Singleton<EnumToIconMapping>.Instance.GetIconName(skillType));
		m_skillNameText.SetTextFromLanguageString(Singleton<CrewmanSkillUpgradeInfo>.Instance.GetNamedText(skillType));
	}
}
