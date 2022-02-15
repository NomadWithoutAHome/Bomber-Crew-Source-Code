using BomberCrewCommon;
using UnityEngine;

public class ObjectiveDisplay : MonoBehaviour
{
	[SerializeField]
	protected GameObject m_completeHierachy;

	[SerializeField]
	protected GameObject m_failedHierarchy;

	[SerializeField]
	protected GameObject m_numericalHierarchy;

	[SerializeField]
	protected TextSetter m_titleText;

	[SerializeField]
	protected TextSetter m_counterText;

	protected ObjectiveManager.Objective m_objective;

	protected int m_currentCounter;

	public virtual void SetUp(ObjectiveManager.Objective o)
	{
		m_objective = o;
		if (!string.IsNullOrEmpty(o.m_substitutionString))
		{
			m_titleText.SetText(string.Format(Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(o.m_objectiveTitle), o.m_substitutionString));
		}
		else
		{
			m_titleText.SetText(Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(o.m_objectiveTitle));
		}
		m_currentCounter = 0;
		if (!m_objective.m_countIsFailure)
		{
			m_counterText.SetText($"{0}/{m_objective.m_countToComplete}");
		}
		else
		{
			m_counterText.SetText($"{m_objective.m_countToFail}/{m_objective.m_countToFail}");
		}
	}

	private void Update()
	{
		Refresh();
	}

	protected virtual void Refresh()
	{
		int num = 0;
		if (!m_objective.m_countIsFailure)
		{
			num = m_objective.m_countToComplete;
			if (m_currentCounter != m_objective.m_countCompleted)
			{
				m_currentCounter = m_objective.m_countCompleted;
				m_counterText.SetText($"{m_objective.m_countCompleted}/{m_objective.m_countToComplete}");
			}
		}
		else
		{
			num = m_objective.m_countToFail;
			if (m_currentCounter != m_objective.m_countFailed)
			{
				m_currentCounter = m_objective.m_countFailed;
				m_counterText.SetText($"{m_objective.m_countToFail - m_objective.m_countFailed}/{m_objective.m_countToFail}");
			}
		}
		if (!string.IsNullOrEmpty(m_objective.m_substitutionString))
		{
			m_titleText.SetText(string.Format(Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_objective.m_objectiveTitle), m_objective.m_substitutionString));
		}
		m_numericalHierarchy.SetActive(!m_objective.m_complete && num > 1);
		m_completeHierachy.SetActive(m_objective.m_complete && !m_objective.m_failed);
		m_failedHierarchy.SetActive(m_objective.m_failed);
	}
}
