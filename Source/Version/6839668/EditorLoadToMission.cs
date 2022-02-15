using UnityEngine;

public class EditorLoadToMission : MonoBehaviour
{
	[SerializeField]
	private LevelDescription m_missionToLoad;

	public LevelDescription GetMissionToLoad()
	{
		return m_missionToLoad;
	}

	public void SetMissionToLoad(LevelDescription missionToLoad)
	{
		m_missionToLoad = missionToLoad;
	}
}
