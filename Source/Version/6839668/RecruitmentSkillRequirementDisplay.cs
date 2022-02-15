using BomberCrewCommon;
using Common;
using UnityEngine;

public class RecruitmentSkillRequirementDisplay : MonoBehaviour
{
	[SerializeField]
	private tk2dSprite m_iconSprite;

	[SerializeField]
	private GameObject m_existsHierarchy;

	[SerializeField]
	private GameObject m_emptyHierarchy;

	private Crewman.SpecialisationSkill m_requiredSkillType;

	private int m_numRequired;

	private bool m_activated;

	public void SetUp(Crewman.SpecialisationSkill requiredSkill, int numRequired)
	{
		m_numRequired = numRequired;
		m_requiredSkillType = requiredSkill;
		m_existsHierarchy.SetActive(value: false);
		m_emptyHierarchy.SetActive(value: true);
		Refresh();
	}

	public void Refresh()
	{
		m_iconSprite.SetSprite(Singleton<EnumToIconMapping>.Instance.GetIconName(m_requiredSkillType));
		int currentCrewCount = Singleton<CrewContainer>.Instance.GetCurrentCrewCount();
		int num = 0;
		for (int i = 0; i < currentCrewCount; i++)
		{
			Crewman crewman = Singleton<CrewContainer>.Instance.GetCrewman(i);
			if (crewman.GetPrimarySkill().GetSkill() == m_requiredSkillType)
			{
				num++;
			}
		}
		bool flag = num >= m_numRequired;
		if (flag != m_activated)
		{
			m_activated = flag;
			m_existsHierarchy.CustomActivate(flag);
			m_emptyHierarchy.CustomActivate(!flag);
		}
	}
}
