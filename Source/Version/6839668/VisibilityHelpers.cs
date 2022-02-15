using BomberCrewCommon;
using UnityEngine;

public class VisibilityHelpers : Singleton<VisibilityHelpers>
{
	[SerializeField]
	private LayerMask m_visibilityLayers;

	[SerializeField]
	private AnimationCurve m_timeOfDayVisibilityMultiplier;

	[SerializeField]
	private DayNightCycle m_dayNightCycle;

	[SerializeField]
	private RadialFogDepthPostProcess m_radialFogPostProcess;

	[SerializeField]
	private float m_maxEyeDistanceDay = 2500f;

	[SerializeField]
	private float m_maxEyeDistanceNight = 1250f;

	[SerializeField]
	private float m_maxEyeDistanceDayLitUp = 3000f;

	[SerializeField]
	private float m_fogMaxDistanceAdditionDay = 500f;

	[SerializeField]
	private float m_fogMaxDistanceAdditionNight = 500f;

	[SerializeField]
	private float m_fogDistanceNearDay;

	[SerializeField]
	private float m_fogDistanceNearNight = 500f;

	[SerializeField]
	private float m_skyBlendDistance;

	[SerializeField]
	private CloudLayers m_cloudLayers;

	private BomberSystems m_bomberSystems;

	private bool m_isSetUp;

	private float m_near;

	private float m_far;

	private Color m_fogColor;

	private float m_skyVisibility = 1f;

	private float m_currentVisDistanceCached;

	private float m_currentNightFactorCached;

	private void Start()
	{
		if (Singleton<BomberSpawn>.Instance != null)
		{
			m_bomberSystems = Singleton<BomberSpawn>.Instance.GetBomberSystems();
		}
	}

	public float GetNightFactor()
	{
		return m_currentNightFactorCached;
	}

	public void UpdateCachedValues()
	{
		m_currentNightFactorCached = 1f - m_timeOfDayVisibilityMultiplier.Evaluate(m_dayNightCycle.GetDayNightProgress());
		m_currentVisDistanceCached = Mathf.Lerp(m_maxEyeDistanceDay, m_maxEyeDistanceNight, GetNightFactor());
	}

	public float GetCurrentVisDistance()
	{
		return m_currentVisDistanceCached;
	}

	public bool IsVisibleGeneric(Vector3 startPos, Vector3 endPos)
	{
		Vector3 vector = endPos - startPos;
		float currentVisDistance = GetCurrentVisDistance();
		if (IsVisible(startPos, endPos, currentVisDistance, lenientCloudModel: false))
		{
			return true;
		}
		return false;
	}

	public bool IsVisibleGenericLenient(Vector3 startPos, Vector3 endPos)
	{
		Vector3 vector = endPos - startPos;
		float maxDistance = GetCurrentVisDistance() * 1.25f;
		if (IsVisible(startPos, endPos, maxDistance, lenientCloudModel: true))
		{
			return true;
		}
		return false;
	}

	public bool IsVisibleHumanPlayer(Vector3 startPos, Vector3 endPos, BomberSystems bSystems, bool isRadarObject, bool isNavigationObject)
	{
		bool flag = false;
		float sqrMagnitude = (endPos - startPos).sqrMagnitude;
		if (isRadarObject)
		{
			StationRadioOperator stationRadioOperator = (StationRadioOperator)bSystems.GetStationFor(BomberSystems.StationType.RadioOperator);
			if (stationRadioOperator.IsRadarActive() && sqrMagnitude < stationRadioOperator.GetRadarSystem().GetRange() * stationRadioOperator.GetRadarSystem().GetRange())
			{
				flag = true;
			}
		}
		if (isNavigationObject)
		{
			StationNavigator stationNavigator = (StationNavigator)bSystems.GetStationFor(BomberSystems.StationType.Navigation);
			if (stationNavigator.GetNavigationHintVisibility() > 0f && sqrMagnitude < stationNavigator.GetNavigationRange() * stationNavigator.GetNavigationRange())
			{
				flag = true;
			}
		}
		if (!flag)
		{
			float maxDistance = GetCurrentVisDistance();
			if (m_bomberSystems.GetWeatherSensor().GetCloudFog())
			{
				maxDistance = 300f;
			}
			if (IsVisible(startPos, endPos, maxDistance, lenientCloudModel: false))
			{
				flag = true;
			}
		}
		return flag;
	}

	public bool IsVisibleAI(Vector3 startPos, Vector3 endPos, bool isTargetLitUp)
	{
		if (isTargetLitUp && (startPos - endPos).magnitude < m_maxEyeDistanceDayLitUp)
		{
			return true;
		}
		return IsVisibleGeneric(startPos, endPos);
	}

	public bool IsVisibleAILenient(Vector3 startPos, Vector3 endPos, bool isTargetLitUp)
	{
		if (isTargetLitUp && (startPos - endPos).magnitude < m_maxEyeDistanceDayLitUp)
		{
			return true;
		}
		return IsVisibleGenericLenient(startPos, endPos);
	}

	public bool IsVisible(Vector3 startPos, Vector3 endPos, float maxDistance, bool lenientCloudModel)
	{
		float sqrMagnitude = (startPos - endPos).sqrMagnitude;
		if (sqrMagnitude > maxDistance * maxDistance)
		{
			return false;
		}
		return true;
	}

	public float GetCloudinessPlayer(Vector3 startPos, Vector3 endPos)
	{
		if (m_bomberSystems.GetWeatherSensor().GetCloudFog())
		{
			return 1f;
		}
		float num = 0f;
		if (startPos.y > endPos.y)
		{
			float lowerY = m_cloudLayers.GetLowerY();
			if (startPos.y > lowerY && endPos.y < lowerY)
			{
				num += m_cloudLayers.GetLowerDensity();
			}
			float upperY = m_cloudLayers.GetUpperY();
			if (startPos.y > upperY && endPos.y < upperY)
			{
				num += 0.4f;
				if (endPos.y < lowerY)
				{
					num += 0.3f;
				}
			}
		}
		else
		{
			float lowerY2 = m_cloudLayers.GetLowerY();
			if (startPos.y < lowerY2 && endPos.y > lowerY2)
			{
				num += m_cloudLayers.GetLowerDensity();
			}
			float upperY2 = m_cloudLayers.GetUpperY();
			if (startPos.y < upperY2 && endPos.y > upperY2)
			{
				num += 0.4f;
				if (startPos.y < lowerY2)
				{
					num += 0.3f;
				}
			}
		}
		return Mathf.Clamp01(num);
	}

	private void Update()
	{
		if (m_dayNightCycle != null)
		{
			float t = 1f - m_timeOfDayVisibilityMultiplier.Evaluate(m_dayNightCycle.GetDayNightProgress());
			float num = Mathf.Lerp(m_fogDistanceNearDay, m_fogDistanceNearNight, t);
			float num2 = Mathf.Lerp(m_maxEyeDistanceDay + m_fogMaxDistanceAdditionDay, m_maxEyeDistanceNight + m_fogMaxDistanceAdditionNight, t);
			float skyNear = num2 - m_skyBlendDistance;
			float skyFar = num2;
			float num3 = 1f;
			Color color = Singleton<SkyboxDayNight>.Instance.GetFogColor();
			if (m_bomberSystems != null && m_bomberSystems.GetWeatherSensor().GetCloudFog())
			{
				color = Singleton<SkyboxDayNight>.Instance.GetAmbientColor() * 0.5f;
				num = 0f;
				num2 = 500f;
				num3 = 0f;
			}
			if (!m_isSetUp)
			{
				m_isSetUp = true;
				m_near = num;
				m_far = num2;
				m_fogColor = color;
				m_skyVisibility = num3;
			}
			else
			{
				m_near = Mathf.Lerp(m_near, num, Mathf.Clamp01(Time.deltaTime * 5f));
				m_far = Mathf.Lerp(m_far, num2, Mathf.Clamp01(Time.deltaTime * 5f));
				m_fogColor = Color.Lerp(m_fogColor, color, Mathf.Clamp01(Time.deltaTime * 5f));
				m_skyVisibility = Mathf.Lerp(m_skyVisibility, num3, Mathf.Clamp01(Time.deltaTime * 5f));
			}
			m_radialFogPostProcess.SetDistances(m_near, m_far, skyNear, skyFar);
			m_radialFogPostProcess.SetFogColor(m_fogColor, m_skyVisibility);
			UpdateCachedValues();
		}
	}
}
