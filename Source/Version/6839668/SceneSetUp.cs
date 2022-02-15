using BomberCrewCommon;
using UnityEngine;

public class SceneSetUp : Singleton<SceneSetUp>
{
	[SerializeField]
	private string m_scriptableObjectExtraPostfix;

	[SerializeField]
	private SceneExportSettings m_sceneExportSettings;

	[SerializeField]
	private MissionCoordinatorPrefabs m_missionPrefabs;

	public SceneExportSettings GetExportSettings()
	{
		return m_sceneExportSettings;
	}

	public MissionCoordinatorPrefabs GetPrefabs()
	{
		return m_missionPrefabs;
	}

	private void Awake()
	{
	}
}
