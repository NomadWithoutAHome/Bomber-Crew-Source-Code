using System;
using System.Globalization;
using BomberCrewCommon;
using dbox;
using UnityEngine;
using WingroveAudio;

public class FlakHazardArea : MonoBehaviour
{
	[SerializeField]
	private GameObject m_flakPrefab;

	[SerializeField]
	private float m_frequencyMin;

	[SerializeField]
	private float m_frequencyMax;

	[SerializeField]
	private float m_aroundBomberSphereStart = 50f;

	[SerializeField]
	private float m_aroundBomberSphereEnd = 25f;

	[SerializeField]
	private float m_sphereReduceTime;

	[SerializeField]
	private ParticleSystem[] m_systemsToScale;

	[SerializeField]
	private float m_activeFlakDepth = 15f;

	[SerializeField]
	private AudioArea m_audioArea;

	[SerializeField]
	private MissionPlaceableObject m_placeableObject;

	[SerializeField]
	private MapNavigatorDisplayable m_mapDisplayable;

	[SerializeField]
	private Transform m_flakAreaTransform;

	[SerializeField]
	private float m_phaseDuration = 120f;

	[SerializeField]
	private ParticleSystem m_muzzleFlashParticles;

	[SerializeField]
	private SmoothDamageable m_linkedToObject;

	private float m_time;

	private float m_radius;

	private float m_height;

	private float m_altitude;

	private float m_activeTime;

	private bool m_areaEnteredTrigger;

	private bool m_doingEnabledEffects;

	private bool m_currentlyEnabled = true;

	private bool m_switchedOff;

	private float m_flakParticleRate;

	private float m_pointInPhase;

	private float m_cutoffPoint;

	private Vector3 m_lastDelta;

	private float m_timeSinceSpeechTrigger;

	private bool m_hasDoneMusicTrigger;

	private int m_flakPrefabInstanceId;

	private void Start()
	{
		m_radius = (float)Convert.ToDouble(m_placeableObject.GetParameter("radius"), CultureInfo.InvariantCulture);
		m_height = (float)Convert.ToDouble(m_placeableObject.GetParameter("height"), CultureInfo.InvariantCulture);
		m_altitude = (float)Convert.ToDouble(m_placeableObject.GetParameter("altitude"), CultureInfo.InvariantCulture);
		m_flakAreaTransform.localPosition = new Vector3(0f, m_height + m_altitude, 0f);
		m_flakPrefabInstanceId = m_flakPrefab.GetInstanceID();
		m_flakParticleRate = m_radius * m_radius / 20000f;
		m_cutoffPoint = m_phaseDuration * Singleton<SaveDataContainer>.Instance.Get().PerkGetFlakUpTime();
		m_pointInPhase = UnityEngine.Random.Range(0f, m_phaseDuration);
		ParticleSystem[] systemsToScale = m_systemsToScale;
		foreach (ParticleSystem particleSystem in systemsToScale)
		{
			Vector3 localScale = particleSystem.transform.localScale;
			localScale = new Vector3(m_radius, m_height, m_radius);
			particleSystem.transform.localScale = localScale * 2f;
		}
		m_audioArea.SetSize(new Vector3(m_radius * 2f, m_height * 2f, m_radius * 2f));
		m_mapDisplayable.SetRadius(m_radius);
		if (!(m_placeableObject != null))
		{
			return;
		}
		string activateTrigger = m_placeableObject.GetParameter("activateTrigger");
		if (string.IsNullOrEmpty(activateTrigger))
		{
			return;
		}
		m_switchedOff = true;
		MissionCoordinator instance = Singleton<MissionCoordinator>.Instance;
		instance.OnTrigger = (Action<string>)Delegate.Combine(instance.OnTrigger, (Action<string>)delegate(string st)
		{
			if (st == activateTrigger)
			{
				m_switchedOff = false;
			}
		});
	}

	private void OnDestroy()
	{
		if (WingroveRoot.Instance != null && m_doingEnabledEffects)
		{
			WingroveRoot.Instance.PostEventGO("FLAK_AMBIENT_STOP", m_flakAreaTransform.gameObject);
		}
	}

	public void Update()
	{
		if (m_currentlyEnabled != m_doingEnabledEffects)
		{
			m_doingEnabledEffects = m_currentlyEnabled;
			if (m_currentlyEnabled)
			{
				WingroveRoot.Instance.PostEventGOAA("FLAK_AMBIENT_START", m_flakAreaTransform.gameObject, m_audioArea);
				if (m_muzzleFlashParticles != null)
				{
					m_muzzleFlashParticles.Play(withChildren: true);
				}
			}
			else
			{
				WingroveRoot.Instance.PostEventGO("FLAK_AMBIENT_STOP", m_flakAreaTransform.gameObject);
				if (m_muzzleFlashParticles != null)
				{
					m_muzzleFlashParticles.Stop(withChildren: true);
				}
			}
			ParticleSystem[] systemsToScale = m_systemsToScale;
			foreach (ParticleSystem particleSystem in systemsToScale)
			{
				Vector3 localScale = particleSystem.transform.localScale;
				localScale = new Vector3(m_radius, m_height, m_radius);
				particleSystem.transform.localScale = localScale * 2f;
				ParticleSystem.EmissionModule emission = particleSystem.emission;
				emission.rateOverTime = ((!m_currentlyEnabled) ? 0f : m_flakParticleRate);
			}
		}
		if (!m_switchedOff && m_linkedToObject != null && m_linkedToObject.GetHealthNormalised() == 0f)
		{
			m_switchedOff = true;
		}
		m_pointInPhase += Time.deltaTime;
		if (m_pointInPhase > m_phaseDuration)
		{
			m_pointInPhase -= m_phaseDuration;
		}
		m_currentlyEnabled = m_pointInPhase < m_cutoffPoint && !m_switchedOff;
		if (m_currentlyEnabled)
		{
			SpawnFlak();
		}
		else if (m_hasDoneMusicTrigger)
		{
			Singleton<MusicSelectionRules>.Instance.Untrigger(MusicSelectionRules.MusicTriggerEvents.HazardNearBy);
			m_hasDoneMusicTrigger = false;
		}
	}

	private void SpawnFlak()
	{
		float t = Mathf.Clamp01(m_activeTime / m_sphereReduceTime);
		float num = Mathf.Lerp(m_aroundBomberSphereStart, m_aroundBomberSphereEnd, t);
		Vector3 position = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState().transform.position;
		Vector3d position2 = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState().GetBigTransform()
			.position;
		Vector3 vector = position + UnityEngine.Random.insideUnitSphere * num;
		Vector3 lastDelta = position - m_flakAreaTransform.position;
		Vector3 vector2 = vector - m_flakAreaTransform.position;
		m_time -= Time.deltaTime;
		bool flag = false;
		if (lastDelta.y < m_height && lastDelta.y > 0f - m_height && lastDelta.x < m_radius && lastDelta.x > 0f - m_radius && lastDelta.z < m_radius && lastDelta.z > 0f - m_radius)
		{
			flag = true;
		}
		if (flag)
		{
			if (!m_hasDoneMusicTrigger)
			{
				Singleton<MusicSelectionRules>.Instance.Trigger(MusicSelectionRules.MusicTriggerEvents.HazardNearBy);
				m_hasDoneMusicTrigger = true;
			}
		}
		else if (m_hasDoneMusicTrigger)
		{
			Singleton<MusicSelectionRules>.Instance.Untrigger(MusicSelectionRules.MusicTriggerEvents.HazardNearBy);
			m_hasDoneMusicTrigger = false;
		}
		if (!flag && lastDelta.sqrMagnitude < m_lastDelta.sqrMagnitude && m_timeSinceSpeechTrigger > 30f && Mathf.Abs(lastDelta.x) < m_radius * 1.2f && Mathf.Abs(lastDelta.z) < m_radius * 1.2f)
		{
			m_timeSinceSpeechTrigger = 0f;
			if (lastDelta.y > m_height * 1.1f)
			{
				Singleton<CrewMiscSpeechTriggers>.Instance.DoExternalTrigger(CrewMiscSpeechTriggers.SpeechExternalTrigger.FlakAheadBelow, null);
			}
			else if (m_altitude > 300f)
			{
				Singleton<CrewMiscSpeechTriggers>.Instance.DoExternalTrigger(CrewMiscSpeechTriggers.SpeechExternalTrigger.FlakAheadMidAltitude, null);
			}
			else
			{
				Singleton<CrewMiscSpeechTriggers>.Instance.DoExternalTrigger(CrewMiscSpeechTriggers.SpeechExternalTrigger.FlakAheadLowAltitude, null);
			}
		}
		m_timeSinceSpeechTrigger += Time.deltaTime;
		m_lastDelta = lastDelta;
		if (flag)
		{
			if (m_time < 0f)
			{
				m_time = UnityEngine.Random.Range(m_frequencyMin, m_frequencyMax);
				GameObject fromPool = Singleton<PoolManager>.Instance.GetFromPool(m_flakPrefab, m_flakPrefabInstanceId);
				fromPool.transform.localScale = m_flakPrefab.transform.localScale;
				fromPool.gameObject.btransform().SetFromCurrentPage(vector);
				DboxInMissionController.DBoxCall(DboxSdkWrapper.PostFlak, vector);
			}
			if (Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState().IsEvadingFlak())
			{
				m_activeTime -= Time.deltaTime;
				if (m_activeTime < 0f)
				{
					m_activeTime = 0f;
				}
			}
			else
			{
				m_activeTime += Time.deltaTime;
			}
			if (!m_areaEnteredTrigger)
			{
				Singleton<MusicSelectionRules>.Instance.Trigger(MusicSelectionRules.MusicTriggerEvents.HazardNearBy);
				m_areaEnteredTrigger = true;
			}
		}
		else
		{
			m_activeTime -= Time.deltaTime;
			if (m_activeTime < 0f)
			{
				m_activeTime = 0f;
			}
			if (m_areaEnteredTrigger)
			{
				Singleton<MusicSelectionRules>.Instance.Untrigger(MusicSelectionRules.MusicTriggerEvents.HazardNearBy);
				m_areaEnteredTrigger = false;
			}
		}
	}
}
