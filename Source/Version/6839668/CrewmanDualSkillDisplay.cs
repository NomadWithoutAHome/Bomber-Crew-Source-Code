using UnityEngine;

public class CrewmanDualSkillDisplay : MonoBehaviour
{
	[SerializeField]
	private CrewmanSkillDisplay m_primarySkill;

	[SerializeField]
	private CrewmanSkillDisplay m_secondarySkill;

	[SerializeField]
	private Transform m_primarySinglePosition;

	[SerializeField]
	private Transform m_primaryDoublePosition;

	[SerializeField]
	private bool m_hideSecondaryIfNotExists;

	public void SetUp(Crewman crewmanToDisplay)
	{
		m_primarySkill.SetUp(crewmanToDisplay, 0);
		m_secondarySkill.SetUp(crewmanToDisplay, 1);
		if (m_hideSecondaryIfNotExists)
		{
			if (crewmanToDisplay.GetSecondarySkill() == null)
			{
				m_secondarySkill.gameObject.SetActive(value: false);
				m_primarySkill.transform.localPosition = m_primarySinglePosition.localPosition;
			}
			else
			{
				m_secondarySkill.gameObject.SetActive(value: true);
				m_primarySkill.transform.localPosition = m_primaryDoublePosition.localPosition;
			}
		}
	}

	public void Refresh()
	{
		m_primarySkill.Refresh();
		m_secondarySkill.Refresh();
	}
}
