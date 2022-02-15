using BomberCrewCommon;
using UnityEngine;

public class ObjectiveDisplayAce : ObjectiveDisplay
{
	[SerializeField]
	private GameObject m_healthBarHierarchy;

	[SerializeField]
	private tk2dUIProgressBar m_progressBar;

	private float m_failedCountdown;

	private bool m_completed;

	public override void SetUp(ObjectiveManager.Objective o)
	{
		m_objective = o;
		m_titleText.SetText($"{o.m_aceFighter.GetAceInformation().GetFirstName()} {o.m_aceFighter.GetAceInformation().GetSurname()}");
		m_progressBar.Value = 0f;
	}

	protected override void Refresh()
	{
		if (m_objective.m_aceFighter != null)
		{
			m_progressBar.Value = m_objective.m_aceFighter.GetComponent<FighterPlane>().GetHealthNormalised();
		}
		m_completed = m_objective.m_complete && !m_objective.m_failed;
		m_completeHierachy.SetActive(m_completed);
		m_healthBarHierarchy.SetActive(!m_completed);
		m_failedHierarchy.SetActive(m_objective.m_failed);
		if (m_objective.m_failed)
		{
			m_failedCountdown += Time.deltaTime;
			if (m_failedCountdown > 10f)
			{
				Singleton<ObjectiveManager>.Instance.RemoveObjective(m_objective);
			}
		}
	}
}
