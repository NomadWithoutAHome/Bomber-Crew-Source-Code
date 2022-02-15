using BomberCrewCommon;
using UnityEngine;

public class EnemyFighterAcesBoardScreen : MonoBehaviour
{
	[SerializeField]
	private tk2dUIItem m_toCampaignMapButton;

	[SerializeField]
	private UISelectFinder m_finder;

	[SerializeField]
	private AirbaseAreaScreen m_briefingScreen;

	private void Awake()
	{
		m_toCampaignMapButton.OnClick += ToCampaignMap;
	}

	private void ToCampaignMap()
	{
		Singleton<AirbaseNavigation>.Instance.SetSelectingArea(m_briefingScreen);
	}
}
