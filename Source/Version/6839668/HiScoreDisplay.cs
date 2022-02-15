using UnityEngine;

public class HiScoreDisplay : MonoBehaviour
{
	[SerializeField]
	private TextSetter m_rank;

	[SerializeField]
	private TextSetter m_name;

	[SerializeField]
	private TextSetter m_score;

	[SerializeField]
	private GameObject m_isPlayerScoreHierarchy;

	[SerializeField]
	private GameObject m_isNotPlayerScoreHierarchy;

	public void SetUp(int rank, string playerName, int score, bool isPlayer)
	{
		m_isPlayerScoreHierarchy.SetActive(isPlayer);
		m_isNotPlayerScoreHierarchy.SetActive(!isPlayer);
		if (rank == -1)
		{
			m_rank.SetText("-");
		}
		else
		{
			m_rank.SetText($"{rank}.");
		}
		m_name.SetText(playerName);
		m_score.SetText($"{score:N0}");
	}
}
