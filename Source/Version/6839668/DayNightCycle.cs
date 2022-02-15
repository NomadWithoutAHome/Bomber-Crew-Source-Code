using AudioNames;
using BomberCrewCommon;
using UnityEngine;
using WingroveAudio;

public class DayNightCycle : MonoBehaviour
{
	private float m_dayNightProgress;

	private float m_dayNightProgressionSpeedMultiplier = 1f;

	[SerializeField]
	private float m_progressRate = 0.001f;

	private bool m_hasStarted;

	private bool m_isPaused;

	private void Start()
	{
		if (Singleton<MissionCoordinator>.Instance != null)
		{
			MissionPlaceableObject objectByType = Singleton<MissionCoordinator>.Instance.GetObjectByType("MissionConfig");
			if (objectByType != null)
			{
				float startTime = objectByType.GetComponent<MissionConfig>().GetStartTime();
				SetDayNight(startTime, 1f);
				WingroveRoot.Instance.SetParameterGlobal(MusicEvents.Parameters.CacheVal_MusicDaynight(), Singleton<VisibilityHelpers>.Instance.GetNightFactor());
			}
		}
	}

	private void OnEnable()
	{
	}

	private void OnDisable()
	{
	}

	private void DebugMenu()
	{
		GUILayout.Label("DAY/NIGHT SLIDER:");
		m_dayNightProgress = GUILayout.HorizontalSlider(m_dayNightProgress, 0f, 1f);
		m_isPaused = GUILayout.Toggle(m_isPaused, "Pause Day/Night");
	}

	public void SetDayNight(float startTime, float multiplier)
	{
		m_dayNightProgress = startTime;
		m_dayNightProgressionSpeedMultiplier = multiplier;
		Singleton<VisibilityHelpers>.Instance.UpdateCachedValues();
		WingroveRoot.Instance.SetParameterGlobal(MusicEvents.Parameters.CacheVal_MusicDaynight(), Singleton<VisibilityHelpers>.Instance.GetNightFactor());
		Shader.SetGlobalFloat("RD_DayNightProgression", m_dayNightProgress);
	}

	public void StartDayNight()
	{
		m_hasStarted = true;
	}

	private void Update()
	{
		if (m_hasStarted && !m_isPaused)
		{
			m_dayNightProgress = Mathf.Repeat(m_dayNightProgress += m_progressRate * Time.deltaTime * m_dayNightProgressionSpeedMultiplier, 1f);
			Shader.SetGlobalFloat("RD_DayNightProgression", m_dayNightProgress);
		}
	}

	public float GetDayNightProgress()
	{
		return m_dayNightProgress;
	}
}
