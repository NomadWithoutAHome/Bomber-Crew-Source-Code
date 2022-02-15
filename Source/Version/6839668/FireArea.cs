using AudioNames;
using BomberCrewCommon;
using UnityEngine;
using WingroveAudio;

public class FireArea : MonoBehaviour, FireOverview.Extinguishable
{
	[SerializeField]
	private FireArea[] m_neighbours;

	[SerializeField]
	private float m_catchChance = 0.4f;

	[SerializeField]
	private float m_catchIntensityReset = 0.75f;

	[SerializeField]
	private float m_spreadSpeedFactor = 0.5f;

	[SerializeField]
	private float m_intensityRampUp = 0.2f;

	[SerializeField]
	private GameObject m_fireFxPrefab;

	[SerializeField]
	private float m_airFireReduceRate = 0.05f;

	[SerializeField]
	private float m_damagePerSecond = 5f;

	[SerializeField]
	private float m_damageRadius = 1f;

	[SerializeField]
	private float m_fireExpandTime = 20f;

	[SerializeField]
	private float m_normalisedAltitudePutFiresOutAbove;

	[SerializeField]
	private float m_normalisedAltitudeFirePutOutRate = 20f;

	private float m_fireIntensity;

	private float m_catchIntensity;

	private bool m_isOnFire;

	private int m_fireCentreNumber;

	private bool m_putOutLastFrame;

	private float m_timeAtNonCriticalLevel;

	private GameObject m_fireFxObject;

	private EffectScaler m_effectScaler;

	private int m_cachedGameObjectId;

	private void Awake()
	{
		m_cachedGameObjectId = base.gameObject.GetInstanceID();
	}

	private void Start()
	{
		m_fireFxObject = Object.Instantiate(m_fireFxPrefab);
		m_fireFxObject.transform.parent = base.transform;
		m_fireFxObject.transform.localScale = Vector3.one;
		m_fireFxObject.transform.localPosition = Vector3.zero;
		m_effectScaler = m_fireFxObject.GetComponent<EffectScaler>();
	}

	public void SpreadFire(float amt, int fireCentreNumber)
	{
		if (m_isOnFire)
		{
			return;
		}
		m_catchIntensity += amt;
		if (m_catchIntensity > 1f)
		{
			if (Random.Range(0f, 1f) < m_catchChance)
			{
				SetFireEnabled(enabled: true, fireCentreNumber - 1);
			}
			m_catchChance = m_catchIntensityReset;
		}
	}

	public Transform GetPutOutPositionOverride()
	{
		return null;
	}

	public bool Exists()
	{
		return true;
	}

	public Transform GetTransform()
	{
		return base.transform;
	}

	private void SetFireEnabled(bool enabled, int fireCentreSize)
	{
		if (enabled && !m_isOnFire)
		{
			WingroveRoot.Instance.PostEventGO("FIRE_START", base.gameObject);
		}
		else if (!enabled && m_isOnFire)
		{
			WingroveRoot.Instance.PostEventGO("FIRE_STOP", base.gameObject);
		}
		m_isOnFire = enabled;
		if (!enabled)
		{
			m_fireCentreNumber = 0;
			return;
		}
		m_fireCentreNumber = fireCentreSize;
		StopAllCoroutines();
	}

	public float GetFireIntensityNormalised()
	{
		if (m_isOnFire)
		{
			return m_fireIntensity;
		}
		return 0f;
	}

	private void Update()
	{
		if (m_isOnFire)
		{
			if (!m_putOutLastFrame)
			{
				if (m_fireCentreNumber > 0)
				{
					FireArea[] neighbours = m_neighbours;
					foreach (FireArea fireArea in neighbours)
					{
						fireArea.SpreadFire(m_spreadSpeedFactor * m_fireIntensity * Time.deltaTime, m_fireCentreNumber);
					}
				}
				m_fireIntensity += Time.deltaTime * m_intensityRampUp;
				if (m_fireIntensity > 0.5f && m_timeAtNonCriticalLevel < m_fireExpandTime)
				{
					m_fireIntensity = 0.5f;
					m_timeAtNonCriticalLevel += Time.deltaTime;
					if (m_timeAtNonCriticalLevel > m_fireExpandTime)
					{
						WingroveRoot.Instance.PostEventGO("FIRE_EXPAND", base.gameObject);
					}
				}
				if (m_fireIntensity > 1f)
				{
					m_fireIntensity = 1f;
				}
				if (m_fireIntensity > 0.5f)
				{
					DamageUtils.DoDamageSlowWithBlock(base.transform.position, 0f, m_damagePerSecond * Time.deltaTime * m_fireIntensity, m_damageRadius, DamageSource.DamageType.Fire);
				}
				WingroveRoot.Instance.SetParameterForObject(GameEvents.Parameters.CacheVal_FireSize(), m_cachedGameObjectId, base.gameObject, m_fireIntensity);
			}
			else
			{
				m_putOutLastFrame = false;
			}
			BomberState bomberState = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState();
			float num = Mathf.Clamp(bomberState.GetAltitudeNormalised(), 0.65f, 1f) - 0.15f;
			float num2 = Mathf.Max(0f - bomberState.GetPhysicsModel().GetVelocity().y, 0f);
			float num3 = Mathf.Clamp01((bomberState.GetAltitudeNormalised() - m_normalisedAltitudePutFiresOutAbove) / (1f - m_normalisedAltitudePutFiresOutAbove)) * m_normalisedAltitudeFirePutOutRate;
			float num4 = num * num2 + num3;
			PutOutFire(num4 * m_airFireReduceRate * Time.deltaTime);
			m_effectScaler.SetScale(Mathf.Pow(m_fireIntensity, 4f));
		}
		else
		{
			m_effectScaler.SetScale(0f);
			m_timeAtNonCriticalLevel = 0f;
		}
	}

	public bool IsOnFire()
	{
		return m_isOnFire;
	}

	public void StartFire(int fireSize)
	{
		SetFireEnabled(enabled: true, fireSize);
	}

	public void PutOutFire(float amt)
	{
		if (m_isOnFire)
		{
			m_fireIntensity -= amt;
			if (m_fireIntensity < 0.25f)
			{
				m_timeAtNonCriticalLevel = 0f;
			}
			if (m_fireIntensity < 0f)
			{
				m_fireIntensity = 0f;
				SetFireEnabled(enabled: false, 0);
				m_catchIntensity = 0f;
			}
			if (amt > 0.05f)
			{
				m_putOutLastFrame = true;
			}
		}
	}

	public bool SearchableForNext()
	{
		return true;
	}

	public InteractiveItem NextInteractive()
	{
		return null;
	}
}
