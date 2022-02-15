using UnityEngine;

[CreateAssetMenu(menuName = "Bomber Crew/Game Mode")]
public class GameMode : ScriptableObject
{
	[SerializeField]
	private string m_saveGamePrefix;

	[SerializeField]
	private string m_airbaseScene;

	[SerializeField]
	private string m_missionScene;

	[SerializeField]
	private bool m_shouldSaveToNormalSlots;

	[SerializeField]
	private bool m_demoFlow;

	[SerializeField]
	private bool m_useEndlessDifficulty;

	[SerializeField]
	private bool m_allowAceRunawayLowHealth;

	[SerializeField]
	private GameObject m_playerBomberPrefab;

	[SerializeField]
	private GameObject m_playerAirbaseOrPhotoBomberPrefab;

	[SerializeField]
	private BomberRequirements m_bomberRequirements;

	[SerializeField]
	private int m_crewSize = 7;

	[SerializeField]
	private Crewman.SpecialisationSkill[] m_specialisationsRequired;

	[SerializeField]
	private CrewmanEquipmentPreset[] m_presetsToUse;

	[SerializeField]
	private bool m_useAddOnCampaignsTab;

	[SerializeField]
	private CampaignStructure m_mainCampaignStructure;

	[SerializeField]
	private float m_xpMultiplier = 1f;

	[SerializeField]
	private bool m_useUSNaming;

	[SerializeField]
	private bool m_hasSpitfireOverride;

	[SerializeField]
	private GameObject m_spitfireOverridePrefab;

	[SerializeField]
	private EnemyFighterAceCatalogue m_overrideAcesForGameMode;

	[SerializeField]
	private bool m_enemyAceOverride;

	[SerializeField]
	private bool m_hasTutorial;

	[SerializeField]
	private int m_startingMoney;

	[SerializeField]
	private int m_startingIntel;

	[SerializeField]
	private Texture2D m_overrideBaseTextureLivery;

	[SerializeField]
	private LiveryRequirements m_overrideLiveryRequirements;

	[SerializeField]
	private Texture2D m_environmentTextureGrass;

	[SerializeField]
	private Texture2D m_environmentTextureFields;

	public int GetStartingMoney()
	{
		return m_startingMoney;
	}

	public Texture2D GetOverrideBaseLiveryTexture()
	{
		return m_overrideBaseTextureLivery;
	}

	public LiveryRequirements GetOverrideLiveryRequirements()
	{
		return m_overrideLiveryRequirements;
	}

	public int GetStartingIntel()
	{
		return m_startingIntel;
	}

	public string GetAirbaseScene()
	{
		return m_airbaseScene;
	}

	public bool HasTutorial()
	{
		return m_hasTutorial;
	}

	public bool ShouldOverrideAceCatalogue()
	{
		return m_enemyAceOverride;
	}

	public string ReplaceCurrencySymbol(string inString)
	{
		if (m_useUSNaming)
		{
			return inString.Replace("£", "$");
		}
		return inString;
	}

	public EnemyFighterAceCatalogue GetAces()
	{
		return m_overrideAcesForGameMode;
	}

	public bool HasSpitfireOverride()
	{
		return m_hasSpitfireOverride;
	}

	public GameObject GetSpitfireOverride()
	{
		return m_spitfireOverridePrefab;
	}

	public bool GetUseUSNaming()
	{
		return m_useUSNaming;
	}

	public bool UseAddOnCampaignsTab()
	{
		return m_useAddOnCampaignsTab;
	}

	public CampaignStructure GetCampaign()
	{
		return m_mainCampaignStructure;
	}

	public float GetXPMultiplier()
	{
		return m_xpMultiplier;
	}

	public string GetMissionScene()
	{
		return m_missionScene;
	}

	public GameObject GetBomberPrefab()
	{
		return m_playerBomberPrefab;
	}

	public GameObject GetAirbaseBomberPrefab()
	{
		return m_playerAirbaseOrPhotoBomberPrefab;
	}

	public BomberRequirements GetBomberRequirements()
	{
		return m_bomberRequirements;
	}

	public string GetSaveDataPrefix()
	{
		return m_saveGamePrefix;
	}

	public int GetMaxCrew()
	{
		return m_crewSize;
	}

	public Crewman.SpecialisationSkill[] GetCrewRequirements()
	{
		return m_specialisationsRequired;
	}

	public CrewmanEquipmentPreset[] GetEquipmentPresets()
	{
		return m_presetsToUse;
	}

	public bool UseEndlessDifficulty()
	{
		return m_useEndlessDifficulty;
	}

	public bool AllowAceRunawayLowHealth()
	{
		return m_allowAceRunawayLowHealth;
	}

	public Texture2D GetEnvironmentGrassTexture()
	{
		return m_environmentTextureGrass;
	}

	public Texture2D GetEnvironmentFieldsTexture()
	{
		return m_environmentTextureFields;
	}
}
