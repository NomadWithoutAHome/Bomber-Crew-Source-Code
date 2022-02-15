using UnityEngine;

[CreateAssetMenu(menuName = "Bomber Crew/DLC Expansion")]
public class DLCExpansionCampaign : ScriptableObject
{
	[SerializeField]
	[NamedText]
	private string m_dlcPackName;

	[SerializeField]
	private string m_internalReference;

	[SerializeField]
	private CampaignStructure m_additionalMissions;

	[SerializeField]
	private EnemyFighterAceCatalogue m_enemyAces;

	[SerializeField]
	private MissionCoordinatorPrefabs m_extraPrefabs;

	[SerializeField]
	private FighterCoordinatorExtraData m_extraFighters;

	[SerializeField]
	private int m_recommendedIntel;

	[SerializeField]
	private EndlessModeVariant m_endlessVariant;

	[SerializeField]
	private GameMode m_gameMode;

	[SerializeField]
	private GameMode m_endlessGameMode;

	public EndlessModeVariant GetEndlessModeVariant()
	{
		return m_endlessVariant;
	}

	public GameMode GetGameMode()
	{
		return m_gameMode;
	}

	public GameMode GetEndlessGameMode()
	{
		return m_endlessGameMode;
	}

	public CampaignStructure GetCampaign()
	{
		return m_additionalMissions;
	}

	public int GetRecommendedIntel()
	{
		return m_recommendedIntel;
	}

	public string GetNamedTextReference()
	{
		return m_dlcPackName;
	}

	public FighterCoordinatorExtraData GetExtraFighters()
	{
		return m_extraFighters;
	}

	public MissionCoordinatorPrefabs GetExtraPrefabs()
	{
		return m_extraPrefabs;
	}

	public EnemyFighterAceCatalogue GetAceCatalogue()
	{
		return m_enemyAces;
	}

	public string GetInternalReference()
	{
		return m_internalReference;
	}
}
