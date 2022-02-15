using BomberCrewCommon;
using UnityEngine;

public class CrewmanSkillDisplay : MonoBehaviour
{
	[SerializeField]
	private TextSetter m_skillLevelNumber;

	[SerializeField]
	private Animation m_skillLevelNumberAnim;

	[SerializeField]
	private tk2dSprite m_skillSprite;

	[SerializeField]
	private bool m_useSmallIcon;

	[SerializeField]
	private GameObject m_enableHierarchy;

	[SerializeField]
	private tk2dClippedSprite m_clippedSprite;

	[SerializeField]
	private TextSetter m_xpProgressValue;

	[SerializeField]
	private string m_xpProgressFormat;

	[SerializeField]
	private GameObject m_lockedHierarchy;

	[SerializeField]
	private TextSetter m_lockedLevelText;

	private Crewman m_crewman;

	private int m_skillIndex;

	private int m_currentLevel;

	public void SetUp(Crewman crewmanToDisplay, int skillIndex)
	{
		m_crewman = crewmanToDisplay;
		m_skillIndex = skillIndex;
		Refresh();
	}

	public void Refresh()
	{
		Crewman.Skill skill = null;
		skill = ((m_skillIndex != 0) ? m_crewman.GetSecondarySkill() : m_crewman.GetPrimarySkill());
		if (skill == null)
		{
			if (m_enableHierarchy != null)
			{
				m_enableHierarchy.SetActive(value: false);
			}
			if (m_lockedHierarchy != null)
			{
				m_lockedHierarchy.SetActive(value: true);
			}
			if (m_lockedLevelText != null)
			{
				int secondarySkillUnlocksAt = Singleton<CrewmanSkillUpgradeInfo>.Instance.GetSecondarySkillUnlocksAt(m_crewman.GetPrimarySkill().GetSkill());
				m_lockedLevelText.SetText(secondarySkillUnlocksAt.ToString());
			}
			return;
		}
		if (m_currentLevel == 0)
		{
			m_currentLevel = skill.GetLevel();
		}
		if (m_lockedHierarchy != null)
		{
			m_lockedHierarchy.SetActive(value: false);
		}
		if (m_enableHierarchy != null)
		{
			m_enableHierarchy.SetActive(value: true);
		}
		if (m_skillLevelNumber != null)
		{
			m_skillLevelNumber.SetText(skill.GetLevel().ToString());
		}
		if (m_useSmallIcon)
		{
			m_skillSprite.SetSprite(Singleton<EnumToIconMapping>.Instance.GetIconNameSmall(skill.GetSkill()));
		}
		else
		{
			m_skillSprite.SetSprite(Singleton<EnumToIconMapping>.Instance.GetIconName(skill.GetSkill()));
		}
		if (m_clippedSprite != null)
		{
			Rect clipRect = m_clippedSprite.ClipRect;
			clipRect.height = skill.GetXPNormalised();
			m_clippedSprite.ClipRect = clipRect;
		}
		if (m_xpProgressValue != null)
		{
			if (skill.GetLevel() < skill.GetMaxLevel())
			{
				m_xpProgressValue.SetText(string.Format(m_xpProgressFormat, skill.GetXP(), Singleton<XPRequirements>.Instance.GetXPRequiredForLevel(skill.GetLevel())));
			}
			else
			{
				m_xpProgressValue.SetText(string.Empty);
			}
		}
		if (m_skillLevelNumberAnim != null && skill.GetLevel() != m_currentLevel)
		{
			PlayLevelUpAnim();
			m_currentLevel = skill.GetLevel();
		}
	}

	public void PlayLevelUpAnim()
	{
		m_skillLevelNumberAnim.Play();
	}
}
