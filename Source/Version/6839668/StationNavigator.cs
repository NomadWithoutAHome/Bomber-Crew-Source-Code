using BomberCrewCommon;
using UnityEngine;

public class StationNavigator : Station
{
	[SerializeField]
	private GameObject m_navigationPointPrefab;

	[SerializeField]
	private GameObject m_navigationPointCustomPrefab;

	[SerializeField]
	private float m_minDistFromTarget;

	[SerializeField]
	private BomberNavigation m_bomberNavigation;

	[SerializeField]
	private BomberState m_bomberState;

	[SerializeField]
	private float m_maxDistance;

	[SerializeField]
	private float m_minDistance;

	[SerializeField]
	private float m_minCalculationTime = 5f;

	[SerializeField]
	private float m_maxCalcuationTime = 15f;

	[SerializeField]
	private float m_minInaccuracy = 100f;

	[SerializeField]
	private float m_maxInaccuracy = 400f;

	[SerializeField]
	private CrewmanSkillAbilityFloat m_navigationSkill;

	[SerializeField]
	private float m_navigationHintRange = 4000f;

	[SerializeField]
	private CrewmanSkillAbilityBool m_navigateByStarsSkill;

	[SerializeField]
	private float m_accuracyLossPerAltitudeDayMinSkill = 0.001f;

	[SerializeField]
	private float m_accuracyLossPerAltitudeNightMinSkill = 0.0015f;

	[SerializeField]
	private float m_accuracyLossPerAltitudeDayMaxSkill = 0.0005f;

	[SerializeField]
	private float m_accuracyLossPerAltitudeNightMaxSkill = 0.001f;

	[SerializeField]
	private float m_altitudeAccuracyBuffer = 140f;

	private float m_calculationTimer;

	private GameObject m_createdNavigationPointObject;

	private Vector3 m_currentInaccuracy;

	private GameObject m_createdCustomNavigationPointObject;

	private Vector3 m_currentCustomNavigationPointDirection;

	private float m_customNavigationPointCountdownTimer;

	private Vector3 m_currentCustomInaccuracy;

	private float m_navigationHintVisibility;

	private float m_navigationQualityStars = 1f;

	private float m_navigationQualityGround = 1f;

	private float m_currentEta;

	private bool m_currentEtaIsNA;

	private InMissionNotification m_noVisibilityNotification;

	private float m_timeSinceCityAnnounce = 30f;

	private float m_timeSinceNavLowAnnounce = 30f;

	private CityNameLocation m_lastAnnouncedCity;

	private float m_timeInStation;

	private bool m_inactiveLocked;

	private string m_navigationSkillId;

	public void SetInactiveLocked(bool inactiveLocked)
	{
		m_inactiveLocked = inactiveLocked;
		if (m_inactiveLocked && m_createdNavigationPointObject != null)
		{
			m_calculationTimer = 0f;
			Object.Destroy(m_createdNavigationPointObject);
		}
	}

	public override void Start()
	{
		base.Start();
		m_calculationTimer = 0f;
		m_navigationSkillId = m_navigationSkill.GetCachedName();
	}

	public bool HasCustomNavigationPointObject()
	{
		return m_createdCustomNavigationPointObject != null && !m_createdCustomNavigationPointObject.GetComponent<TaggableItem>().IsFullyTagged();
	}

	public Vector3 GetCustomNavigationPointDirection()
	{
		return m_currentCustomNavigationPointDirection;
	}

	public void SetCustomNavigationPoint(Vector3 inDirection)
	{
		if (m_createdCustomNavigationPointObject != null)
		{
			Object.Destroy(m_createdCustomNavigationPointObject);
		}
		else
		{
			float proficiency = Singleton<CrewmanSkillUpgradeInfo>.Instance.GetProficiency(m_navigationSkillId, GetCurrentCrewman().GetCrewman());
			float num = Mathf.Lerp(m_maxInaccuracy, m_minInaccuracy, proficiency);
			Vector3 normalized = Vector3.Cross(inDirection, Vector3.up).normalized;
			m_currentCustomInaccuracy = normalized * Random.Range(0f - num, num);
		}
		m_currentCustomNavigationPointDirection = inDirection.normalized;
		m_customNavigationPointCountdownTimer = 30f;
		m_createdCustomNavigationPointObject = Object.Instantiate(m_navigationPointCustomPrefab);
		inDirection.y = 0f;
		Vector3d vector3d = m_bomberState.GetBigTransform().position + new Vector3d(m_currentCustomNavigationPointDirection * 2000f);
		m_createdCustomNavigationPointObject.btransform().position = vector3d + new Vector3d(m_currentCustomInaccuracy);
		Singleton<SpeechPrioritiser>.Instance.RequestNewSpeech(StaticSpeechSets.NavigatorNewMarkerCustom, GetCurrentCrewman());
	}

	public void ClearCustomNavigationPoint()
	{
		if (m_createdCustomNavigationPointObject != null)
		{
			Object.Destroy(m_createdCustomNavigationPointObject);
		}
	}

	public bool CanSeeGround(out float cloudiness)
	{
		Vector3 position = m_bomberState.transform.position;
		Vector3 endPos = position;
		BomberSystems bomberSystems = Singleton<BomberSpawn>.Instance.GetBomberSystems();
		endPos.y = 0f;
		CrewmanAvatar currentCrewman = GetCurrentCrewman();
		if (currentCrewman != null && m_bomberState.GetAltitudeNormalised() < 0.8f && Singleton<VisibilityHelpers>.Instance.IsVisibleHumanPlayer(position, endPos, bomberSystems, isRadarObject: false, isNavigationObject: false))
		{
			cloudiness = Mathf.Pow(Singleton<VisibilityHelpers>.Instance.GetCloudinessPlayer(position, endPos), 1.125f);
			float proficiency = Singleton<CrewmanSkillUpgradeInfo>.Instance.GetProficiency(m_navigationSkillId, currentCrewman.GetCrewman());
			float a = Mathf.Lerp(m_accuracyLossPerAltitudeDayMinSkill, m_accuracyLossPerAltitudeDayMaxSkill, proficiency);
			float b = Mathf.Lerp(m_accuracyLossPerAltitudeNightMinSkill, m_accuracyLossPerAltitudeNightMaxSkill, proficiency);
			float num = Mathf.Lerp(a, b, Singleton<VisibilityHelpers>.Instance.GetNightFactor());
			float num2 = Mathf.Max(m_bomberState.GetAltitudeAboveGround() - m_altitudeAccuracyBuffer, 0f);
			cloudiness += num * num2;
			cloudiness = Mathf.Clamp01(cloudiness);
			return true;
		}
		cloudiness = 1f;
		return false;
	}

	public bool CanSeeStars(out float cloudiness)
	{
		if (Singleton<VisibilityHelpers>.Instance.GetNightFactor() > 0.5f && m_bomberState.GetAltitudeNormalised() >= 0.5f && Singleton<CrewmanSkillUpgradeInfo>.Instance.IsUnlocked(m_navigateByStarsSkill, GetCurrentCrewman().GetCrewman()))
		{
			Vector3 position = m_bomberState.transform.position;
			Vector3 endPos = position;
			BomberSystems bomberSystems = Singleton<BomberSpawn>.Instance.GetBomberSystems();
			endPos.y = Mathf.Max(endPos.y + 200f, 1800f);
			if (Singleton<VisibilityHelpers>.Instance.IsVisibleHumanPlayer(position, endPos, bomberSystems, isRadarObject: false, isNavigationObject: false))
			{
				cloudiness = Singleton<VisibilityHelpers>.Instance.GetCloudinessPlayer(position, endPos);
				cloudiness = Mathf.Clamp01(cloudiness);
				return true;
			}
		}
		cloudiness = 1f;
		return false;
	}

	public float GetNavigationRange()
	{
		return m_navigationHintRange;
	}

	public void SetNavigationProgressForce(float fAmt)
	{
		m_calculationTimer = fAmt;
	}

	public float GetNavigationHintVisibility()
	{
		return m_navigationHintVisibility;
	}

	private void Update()
	{
		if (m_inactiveLocked)
		{
			m_calculationTimer = 0f;
			m_navigationHintVisibility = 1f;
			m_navigationQualityGround = 1f;
			if (m_noVisibilityNotification != null)
			{
				GetNotifications().RemoveNotification(m_noVisibilityNotification);
				m_noVisibilityNotification = null;
			}
			return;
		}
		if (GetCurrentCrewman() == null || !GetCurrentCrewman().CanDoActions())
		{
			m_timeInStation = 0f;
			m_calculationTimer = 0f;
			if (m_createdNavigationPointObject != null)
			{
				Object.Destroy(m_createdNavigationPointObject);
			}
			m_navigationHintVisibility = 0f;
			m_navigationQualityGround = 0f;
			m_navigationQualityStars = 0f;
			if (m_noVisibilityNotification == null)
			{
				m_noVisibilityNotification = new InMissionNotification("notification_navigator_no_visibility", StationNotificationUrgency.Red, EnumToIconMapping.InteractionOrAlertType.Navigate);
				GetNotifications().RegisterNotification(m_noVisibilityNotification);
			}
			goto IL_050a;
		}
		float cloudiness = 0f;
		bool flag = CanSeeGround(out cloudiness);
		float cloudiness2 = 0f;
		bool flag2 = CanSeeStars(out cloudiness2);
		float num = 1f - cloudiness - m_navigationQualityGround;
		if (Mathf.Abs(num) < 0.01f)
		{
			m_navigationQualityGround = 1f - cloudiness;
		}
		else
		{
			m_navigationQualityGround += num * Time.deltaTime;
		}
		float num2 = 1f - cloudiness2 - m_navigationQualityStars;
		if (Mathf.Abs(num2) < 0.01f)
		{
			m_navigationQualityStars = 1f - cloudiness2;
		}
		else
		{
			m_navigationQualityStars += num2 * Time.deltaTime;
		}
		float num3 = Mathf.Max(m_navigationQualityGround, m_navigationQualityStars);
		if (num3 > 0f)
		{
			m_navigationHintVisibility += Time.deltaTime;
			if (m_navigationHintVisibility > 1f)
			{
				m_navigationHintVisibility = 1f;
			}
			if (m_noVisibilityNotification != null)
			{
				GetNotifications().RemoveNotification(m_noVisibilityNotification);
				m_noVisibilityNotification = null;
			}
		}
		else
		{
			if (m_noVisibilityNotification == null)
			{
				m_noVisibilityNotification = new InMissionNotification("notification_navigator_no_visibility", StationNotificationUrgency.Red, EnumToIconMapping.InteractionOrAlertType.Visibility);
				GetNotifications().RegisterNotification(m_noVisibilityNotification);
			}
			m_calculationTimer -= Time.deltaTime;
			if (m_calculationTimer < 0f)
			{
				m_calculationTimer = 0f;
			}
			m_navigationHintVisibility -= Time.deltaTime;
			if (m_navigationHintVisibility < 0f)
			{
				m_navigationHintVisibility = 0f;
			}
		}
		if (num3 > 0.5f && m_timeSinceCityAnnounce > 60f && !Singleton<GameFlow>.Instance.GetGameMode().UseEndlessDifficulty())
		{
			float nearestCityDistance = Singleton<CityNameProximityDetection>.Instance.GetNearestCityDistance();
			if (nearestCityDistance < 2250f)
			{
				CityNameLocation nearestCity = Singleton<CityNameProximityDetection>.Instance.GetNearestCity();
				if (nearestCity != m_lastAnnouncedCity)
				{
					Vector3 normalized = (nearestCity.transform.position - base.transform.position).normalized;
					float num4 = Vector3.Dot(m_bomberState.GetVelocity().normalized, normalized);
					if (num4 > 0.5f)
					{
						Singleton<SpeechPrioritiser>.Instance.RequestNewSpeech(StaticSpeechSets.ApproachingCity, GetCurrentCrewman(), nearestCity.GetTranslatedTextName());
						m_timeSinceCityAnnounce = 0f;
						m_lastAnnouncedCity = nearestCity;
					}
					else if (num4 > -0.1f)
					{
						Singleton<SpeechPrioritiser>.Instance.RequestNewSpeech(StaticSpeechSets.PassingCity, GetCurrentCrewman(), nearestCity.GetTranslatedTextName());
						m_timeSinceCityAnnounce = 0f;
						m_lastAnnouncedCity = nearestCity;
					}
				}
			}
		}
		if (m_bomberNavigation.GetNextNavigationPoint() != null)
		{
			BomberNavigation.NavigationPointType? currentType = m_bomberNavigation.GetCurrentType();
			if (currentType.GetValueOrDefault() == BomberNavigation.NavigationPointType.Navigation && currentType.HasValue)
			{
				goto IL_0472;
			}
		}
		float proficiency = Singleton<CrewmanSkillUpgradeInfo>.Instance.GetProficiency(m_navigationSkillId, GetCurrentCrewman().GetCrewman());
		float num5 = Mathf.Lerp(m_maxCalcuationTime, m_minCalculationTime, proficiency);
		m_calculationTimer += Time.deltaTime * (1f / num5) * Mathf.Clamp01(num3);
		if (m_calculationTimer > 1f)
		{
			m_calculationTimer = 1f;
		}
		goto IL_0472;
		IL_050a:
		m_timeSinceCityAnnounce += Time.deltaTime;
		if (m_bomberNavigation.GetNextNavigationPoint() == null || m_bomberNavigation.GetCurrentType() == BomberNavigation.NavigationPointType.Detour)
		{
			if (m_calculationTimer >= 1f)
			{
				MissionPlaceableObject missionPlaceableObject = null;
				missionPlaceableObject = ((!Singleton<MissionCoordinator>.Instance.IsOutwardJourney()) ? Singleton<MissionCoordinator>.Instance.GetObjectByType("MissionStart") : Singleton<MissionCoordinator>.Instance.GetObjectByType("MissionTarget"));
				if (missionPlaceableObject == null)
				{
					missionPlaceableObject = Singleton<MissionCoordinator>.Instance.GetObjectByType("MissionStart");
				}
				Vector3d vector3d = missionPlaceableObject.gameObject.btransform().position - m_bomberState.gameObject.btransform().position;
				vector3d.y = 0.0;
				if (vector3d.magnitude > (double)m_minDistFromTarget)
				{
					float b = Mathf.Min((float)(vector3d.magnitude - (double)m_minDistFromTarget), m_maxDistance);
					float num6 = Mathf.Max(m_minDistance, b);
					if (m_createdNavigationPointObject == null)
					{
						m_createdNavigationPointObject = Object.Instantiate(m_navigationPointPrefab);
						float proficiency2 = Singleton<CrewmanSkillUpgradeInfo>.Instance.GetProficiency(m_navigationSkillId, GetCurrentCrewman().GetCrewman());
						float num7 = Mathf.Lerp(m_maxInaccuracy, m_minInaccuracy, proficiency2);
						Vector3 normalized2 = Vector3.Cross((Vector3)vector3d, Vector3.up).normalized;
						m_currentInaccuracy = normalized2 * Random.Range(0f - num7, num7);
						Singleton<SpeechPrioritiser>.Instance.RequestNewSpeech(StaticSpeechSets.NavigatorNewMarker, GetCurrentCrewman());
					}
					Vector3d vector3d2 = m_bomberState.gameObject.btransform().position + vector3d.normalized * num6;
					m_createdNavigationPointObject.btransform().position = vector3d2 + new Vector3d(m_currentInaccuracy);
				}
				else if (m_createdNavigationPointObject != null)
				{
					m_calculationTimer = 0f;
					Object.Destroy(m_createdNavigationPointObject);
				}
			}
		}
		else if (m_createdNavigationPointObject != null)
		{
			m_calculationTimer = 0f;
			Object.Destroy(m_createdNavigationPointObject);
		}
		if (m_createdCustomNavigationPointObject != null)
		{
			Vector3d vector3d3 = m_bomberState.gameObject.btransform().position + new Vector3d(m_currentCustomNavigationPointDirection * 2000f);
			m_createdCustomNavigationPointObject.btransform().position = vector3d3 + new Vector3d(m_currentCustomInaccuracy);
			m_customNavigationPointCountdownTimer -= Time.deltaTime;
			if (m_customNavigationPointCountdownTimer < 0f && !Singleton<ContextControl>.Instance.IsTargetingMode())
			{
				Object.Destroy(m_createdCustomNavigationPointObject);
			}
		}
		return;
		IL_0472:
		if (num3 < 0.25f && m_timeSinceNavLowAnnounce > 60f && m_timeInStation > 10f)
		{
			if (num3 == 0f)
			{
				Singleton<SpeechPrioritiser>.Instance.RequestNewSpeech(StaticSpeechSets.NavigationNone, GetCurrentCrewman());
			}
			else
			{
				Singleton<SpeechPrioritiser>.Instance.RequestNewSpeech(StaticSpeechSets.NavigationPoorQuality, GetCurrentCrewman());
			}
			m_timeSinceNavLowAnnounce = 0f;
		}
		m_timeInStation += Time.deltaTime;
		m_timeSinceNavLowAnnounce += Time.deltaTime;
		goto IL_050a;
	}

	public bool GetEta(out float etaTime, out bool isUnknown)
	{
		if (m_navigationHintVisibility < 1f)
		{
			isUnknown = true;
			etaTime = 0f;
			return false;
		}
		MissionPlaceableObject missionPlaceableObject = null;
		missionPlaceableObject = ((!Singleton<MissionCoordinator>.Instance.IsOutwardJourney()) ? Singleton<MissionCoordinator>.Instance.GetObjectByType("MissionStart") : Singleton<MissionCoordinator>.Instance.GetObjectByType("MissionTarget"));
		if (missionPlaceableObject == null)
		{
			missionPlaceableObject = Singleton<MissionCoordinator>.Instance.GetObjectByType("MissionStart");
		}
		Vector3d vector3d = missionPlaceableObject.gameObject.btransform().position - m_bomberState.gameObject.btransform().position;
		vector3d.y = 0.0;
		m_currentEta = (float)vector3d.magnitude / m_bomberState.GetVelocity().magnitude * 1.05f;
		if (missionPlaceableObject.GetParameter("naEta") == "true")
		{
			m_currentEtaIsNA = true;
		}
		else
		{
			m_currentEtaIsNA = false;
		}
		etaTime = m_currentEta;
		if (m_bomberState.GetVelocity().magnitude < 1f || m_bomberState.HasNotCompletedTakeOff())
		{
			isUnknown = true;
			return false;
		}
		isUnknown = false;
		return !m_currentEtaIsNA;
	}

	public float GetStarsVisValue()
	{
		return m_navigationQualityStars;
	}

	public float GetGroundVisValue()
	{
		return m_navigationQualityGround;
	}

	public float GetNavigationCalculationTimer()
	{
		if (m_createdNavigationPointObject == null || GetCurrentCrewman() == null)
		{
			return m_calculationTimer;
		}
		return 1f;
	}

	public override bool IsActivityInProgress(out float progress, out string iconType)
	{
		iconType = string.Empty;
		progress = GetNavigationCalculationTimer();
		if (progress > 0f && progress < 1f)
		{
			return true;
		}
		return false;
	}
}
