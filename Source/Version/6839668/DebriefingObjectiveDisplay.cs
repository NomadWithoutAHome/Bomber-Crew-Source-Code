using BomberCrewCommon;
using Common;
using UnityEngine;

public class DebriefingObjectiveDisplay : MonoBehaviour
{
	[SerializeField]
	private GameObject m_targetDestroyedHierarchy;

	[SerializeField]
	private GameObject m_targetNotDestroyedHierarchy;

	[SerializeField]
	private TextSetter m_rewardText;

	[SerializeField]
	private TextSetter m_intelText;

	[SerializeField]
	private TextSetter m_objectiveNameText;

	[SerializeField]
	private GameObject[] m_subObjectiveHierarchies;

	[SerializeField]
	private GameObject[] m_normalHierarchies;

	public void Awake()
	{
		m_targetDestroyedHierarchy.SetActive(value: false);
		m_targetNotDestroyedHierarchy.SetActive(value: false);
		m_rewardText.SetText(string.Empty);
		m_intelText.SetText(string.Empty);
	}

	public void SetUp(string namedText)
	{
		if (!string.IsNullOrEmpty(namedText))
		{
			m_objectiveNameText.SetTextFromLanguageString(namedText);
		}
	}

	public void ShowObjectiveCompletion(bool destroyed, int moneyReward, int intelReward, bool isSubObjective)
	{
		if (isSubObjective)
		{
			GameObject[] subObjectiveHierarchies = m_subObjectiveHierarchies;
			foreach (GameObject gameObject in subObjectiveHierarchies)
			{
				gameObject.SetActive(value: true);
			}
			GameObject[] normalHierarchies = m_normalHierarchies;
			foreach (GameObject gameObject2 in normalHierarchies)
			{
				gameObject2.SetActive(value: false);
			}
		}
		m_rewardText.SetText(Singleton<GameFlow>.Instance.GetGameMode().ReplaceCurrencySymbol($"£{moneyReward:N0}"));
		m_intelText.SetText(intelReward.ToString());
		if (destroyed)
		{
			m_targetDestroyedHierarchy.CustomActivate(active: true);
		}
		else
		{
			m_targetNotDestroyedHierarchy.CustomActivate(active: true);
		}
	}
}
