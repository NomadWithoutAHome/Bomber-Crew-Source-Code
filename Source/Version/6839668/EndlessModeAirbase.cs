using BomberCrewCommon;
using UnityEngine;

public class EndlessModeAirbase : Singleton<EndlessModeAirbase>
{
	[SerializeField]
	private AirbasePersistentCrew m_persistentCrew;

	private bool m_isPaused;

	private void OnEnable()
	{
		Singleton<EndlessModeGameFlow>.Instance.ResetEndlessMode();
	}

	private void Update()
	{
		bool flag = Singleton<GameFlow>.Instance.IsPaused();
		if (flag == m_isPaused)
		{
			return;
		}
		m_isPaused = flag;
		if (flag)
		{
			EndlessModeVariant currentEndlessMode = Singleton<EndlessModeGameFlow>.Instance.GetCurrentEndlessMode();
			int currentCrewCount = Singleton<CrewContainer>.Instance.GetCurrentCrewCount();
			Crewman[] array = new Crewman[currentCrewCount];
			for (int i = 0; i < currentCrewCount; i++)
			{
				array[i] = Singleton<CrewContainer>.Instance.GetCrewman(i);
			}
			EndlessModeVariant.LoadoutJS loadout = new EndlessModeVariant.LoadoutJS(Singleton<BomberContainer>.Instance.GetCurrentConfig(), array, Singleton<BomberContainer>.Instance.GetCurrentConfig().GetName());
			Singleton<SaveDataContainer>.Instance.Get().SetLastSeenEndlessLoadout(currentEndlessMode.name, loadout);
		}
	}

	public AirbasePersistentCrew GetPersistentCrew()
	{
		return m_persistentCrew;
	}
}
