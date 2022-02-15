using BomberCrewCommon;
using UnityEngine;

public class AirbaseEnvironmentSetup : MonoBehaviour
{
	[SerializeField]
	private RadialFogDepthPostProcess m_radialFogPostProcess;

	[SerializeField]
	private float m_fogStart;

	[SerializeField]
	private float m_fogEnd = 7000f;

	[SerializeField]
	private float m_skyStart = 6500f;

	[SerializeField]
	private float m_skyEnd = 7000f;

	[SerializeField]
	private float m_skyVisibility = 1f;

	[SerializeField]
	private GameObject[] m_winterObjects;

	private void Start()
	{
		bool flag = Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetMissionLog() != null && Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetMissionLog().GetWinterEnvironment();
		if (Singleton<EnvironmentMaterialSetup>.Instance != null)
		{
			Singleton<EnvironmentMaterialSetup>.Instance.SetWinter(flag);
		}
		GameObject[] winterObjects = m_winterObjects;
		foreach (GameObject gameObject in winterObjects)
		{
			gameObject.SetActive(flag);
		}
	}

	private void Update()
	{
		m_radialFogPostProcess.SetDistances(m_fogStart, m_fogEnd, m_skyStart, m_skyEnd);
		m_radialFogPostProcess.SetFogColor(Singleton<SkyboxDayNight>.Instance.GetFogColor(), m_skyVisibility);
	}
}
