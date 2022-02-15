using BomberCrewCommon;
using UnityEngine;

public class PanelSkillLockableButton : MonoBehaviour
{
	[SerializeField]
	private GameObject m_lockedHierarchy;

	[SerializeField]
	private GameObject m_unlockedHierarchy;

	[SerializeField]
	private TextSetter m_lockedLevelText;

	[SerializeField]
	private TextSetter m_mainText;

	[SerializeField]
	private Crewman.SpecialisationSkill m_useSkillForReference;

	[SerializeField]
	private CrewmanSkillAbilityBool m_linkedSkill;

	[SerializeField]
	private CrewmanSkillAbilityBool m_linkedSkillStar;

	[SerializeField]
	[NamedText]
	private string m_languageSkillNamedTextFormat = "ui_crewman_panel_level_unlock";

	[SerializeField]
	private GameObject m_starSkillHierarchy;

	private int m_crewPrimarySkillLevelPrev = -1;

	private int m_crewSecondarySkillLevelPrev = -1;

	private bool m_hasEverRefreshed;

	private Crewman m_crewman;

	public void SetUp(Crewman crewman)
	{
		m_hasEverRefreshed = false;
		m_crewman = crewman;
		Refresh();
	}

	private void Update()
	{
		Refresh();
	}

	private void Refresh()
	{
		if (m_crewman == null)
		{
			return;
		}
		int level = m_crewman.GetPrimarySkill().GetLevel();
		bool flag = false;
		if (!m_hasEverRefreshed)
		{
			flag = true;
		}
		if (level != m_crewPrimarySkillLevelPrev)
		{
			flag = true;
		}
		if (!flag)
		{
			int num = ((m_crewman.GetSecondarySkill() != null) ? m_crewman.GetSecondarySkill().GetLevel() : (-1));
			if (num != m_crewSecondarySkillLevelPrev)
			{
				flag = true;
			}
		}
		if (!flag)
		{
			return;
		}
		int num2 = (m_crewSecondarySkillLevelPrev = ((m_crewman.GetSecondarySkill() != null) ? m_crewman.GetSecondarySkill().GetLevel() : (-1)));
		m_crewPrimarySkillLevelPrev = level;
		m_hasEverRefreshed = true;
		if (m_linkedSkill == null || Singleton<CrewmanSkillUpgradeInfo>.Instance.IsUnlocked(m_linkedSkill, m_crewman))
		{
			m_lockedHierarchy.SetActive(value: false);
			m_unlockedHierarchy.SetActive(value: true);
			bool flag2 = m_linkedSkillStar != null && Singleton<CrewmanSkillUpgradeInfo>.Instance.IsUnlocked(m_linkedSkillStar, m_crewman);
			if (m_starSkillHierarchy != null)
			{
				m_starSkillHierarchy.SetActive(value: false);
			}
			if (flag2)
			{
				if (m_mainText != null)
				{
					m_mainText.SetText(m_linkedSkillStar.GetTitleText());
				}
			}
			else if (m_mainText != null)
			{
				m_mainText.SetText(m_linkedSkill.GetTitleText());
			}
		}
		else
		{
			m_lockedHierarchy.SetActive(value: true);
			m_unlockedHierarchy.SetActive(value: false);
			int unlockLevel = Singleton<CrewmanSkillUpgradeInfo>.Instance.GetUnlockLevel(m_linkedSkill, m_useSkillForReference);
			m_lockedLevelText.SetText(string.Format(Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_languageSkillNamedTextFormat), unlockLevel));
		}
	}
}
