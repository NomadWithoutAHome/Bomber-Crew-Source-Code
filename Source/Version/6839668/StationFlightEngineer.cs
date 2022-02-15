using BomberCrewCommon;
using UnityEngine;

public class StationFlightEngineer : Station
{
	[SerializeField]
	private SkillRefreshTimer m_skillRefreshTimer;

	[SerializeField]
	private CrewmanSkillAbilityBool m_richBoostSkill;

	[SerializeField]
	private CrewmanSkillAbilityBool m_leanBoostSkill;

	[SerializeField]
	private CrewmanSkillAbilityBool m_richBoostPlusSkill;

	[SerializeField]
	private CrewmanSkillAbilityBool m_leanBoostPlusSkill;

	[SerializeField]
	private FuelTank[] m_fuelTanks;

	[SerializeField]
	private BomberSystemUniqueId m_extinguisherSystem;

	[SerializeField]
	private GameObject[] m_boostVisualObjects;

	private Interaction m_sitInteractionLocal = new Interaction("Sit", queryProgress: false, EnumToIconMapping.InteractionOrAlertType.Main);

	private SkillRefreshTimer.SkillTimer m_leanBoostTimer;

	private SkillRefreshTimer.SkillTimer m_richBoostTimer;

	private int m_currentFuelMix = 1;

	private int m_returnToFuelMix = 1;

	private bool m_hasWarnedHalfwayFuel;

	private bool m_hasWarnedCriticalFuel;

	private float m_fuelInefficiencyTimer;

	private InMissionNotification m_criticalFuelNotification;

	private InMissionNotification m_fuelUseInefficientNotification;

	private BomberState m_bomberState;

	private bool m_isBoostShowing;

	private float m_timeSinceFuelWarning;

	private int m_numExtinguishesLeft;

	public override void Start()
	{
		base.Start();
		m_leanBoostTimer = m_skillRefreshTimer.CreateNew();
		m_richBoostTimer = m_skillRefreshTimer.CreateNew();
		m_currentFuelMix = 1;
		m_bomberState = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState();
		GameObject[] boostVisualObjects = m_boostVisualObjects;
		foreach (GameObject gameObject in boostVisualObjects)
		{
			gameObject.SetActive(value: false);
		}
		m_numExtinguishesLeft = ((!(GetExtinguisherUpgrade() == null)) ? GetExtinguisherUpgrade().GetCapacity() : 0);
	}

	public int GetExtinguishesLeft()
	{
		return m_numExtinguishesLeft;
	}

	public void UseExtinguish()
	{
		m_numExtinguishesLeft--;
	}

	public ExtinguisherSystemUpgrade GetExtinguisherUpgrade()
	{
		return (ExtinguisherSystemUpgrade)m_extinguisherSystem.GetUpgrade();
	}

	public void SetFuelMixRich()
	{
		m_currentFuelMix = 2;
		m_returnToFuelMix = 2;
		m_leanBoostTimer.FinishEarly();
		m_richBoostTimer.FinishEarly();
	}

	public void SetFuelMixCorrect()
	{
		m_currentFuelMix = 1;
		m_returnToFuelMix = 1;
		m_leanBoostTimer.FinishEarly();
		m_richBoostTimer.FinishEarly();
	}

	public void DoRichBoost()
	{
		if (m_richBoostTimer.CanStart())
		{
			m_leanBoostTimer.FinishEarly();
			Crewman crewman = GetCurrentCrewman().GetCrewman();
			bool flag = Singleton<CrewmanSkillUpgradeInfo>.Instance.IsUnlocked(m_richBoostPlusSkill, crewman.GetPrimarySkill(), crewman.GetSecondarySkill());
			m_richBoostTimer.Start((!flag) ? m_richBoostSkill.GetDurationTimer() : m_richBoostPlusSkill.GetDurationTimer(), (!flag) ? m_richBoostSkill.GetCooldownTimer() : m_richBoostPlusSkill.GetCooldownTimer());
			m_currentFuelMix = 3;
		}
	}

	public void DoLeanBoost()
	{
		if (m_leanBoostTimer.CanStart())
		{
			m_richBoostTimer.FinishEarly();
			Crewman crewman = GetCurrentCrewman().GetCrewman();
			bool flag = Singleton<CrewmanSkillUpgradeInfo>.Instance.IsUnlocked(m_leanBoostPlusSkill, crewman.GetPrimarySkill(), crewman.GetSecondarySkill());
			m_leanBoostTimer.Start((!flag) ? m_leanBoostSkill.GetDurationTimer() : m_leanBoostPlusSkill.GetDurationTimer(), (!flag) ? m_leanBoostSkill.GetCooldownTimer() : m_leanBoostPlusSkill.GetCooldownTimer());
			m_currentFuelMix = 0;
		}
	}

	public SkillRefreshTimer.SkillTimer GetLeanBoostInfo()
	{
		return m_leanBoostTimer;
	}

	public SkillRefreshTimer.SkillTimer GetRichBoostInfo()
	{
		return m_richBoostTimer;
	}

	private void Update()
	{
		if (GetCurrentCrewman() == null)
		{
			m_leanBoostTimer.FinishEarly();
			m_richBoostTimer.FinishEarly();
			m_currentFuelMix = m_returnToFuelMix;
		}
		else
		{
			m_leanBoostTimer.DoRechargedSpeech(m_leanBoostSkill.GetTitleText(), GetCurrentCrewman(), m_leanBoostSkill);
			m_richBoostTimer.DoRechargedSpeech(m_richBoostSkill.GetTitleText(), GetCurrentCrewman(), m_richBoostSkill);
		}
		if (!m_leanBoostTimer.IsActive() && m_currentFuelMix == 0)
		{
			m_currentFuelMix = m_returnToFuelMix;
		}
		if (!m_richBoostTimer.IsActive() && m_currentFuelMix == 3)
		{
			m_currentFuelMix = m_returnToFuelMix;
		}
		m_bomberState.SetFuelMix(m_currentFuelMix);
		float num = 0f;
		FuelTank[] fuelTanks = m_fuelTanks;
		foreach (FuelTank fuelTank in fuelTanks)
		{
			num += fuelTank.GetFuelNormalised();
		}
		num /= (float)m_fuelTanks.Length;
		if (num < 0.2f && num > 0f)
		{
			if (GetCurrentCrewman() != null && m_timeSinceFuelWarning > 30f)
			{
				m_hasWarnedCriticalFuel = true;
				m_hasWarnedHalfwayFuel = true;
				Singleton<SpeechPrioritiser>.Instance.RequestNewSpeech(StaticSpeechSets.FuelCriticalLow, GetCurrentCrewman());
				m_timeSinceFuelWarning = 0f;
			}
			m_timeSinceFuelWarning += Time.deltaTime;
		}
		if (num < 0.25f)
		{
			if (!m_hasWarnedCriticalFuel && GetCurrentCrewman() != null)
			{
				m_hasWarnedCriticalFuel = true;
				m_hasWarnedHalfwayFuel = true;
				Singleton<SpeechPrioritiser>.Instance.RequestNewSpeech(StaticSpeechSets.FuelCriticalLow, GetCurrentCrewman());
			}
			if (m_criticalFuelNotification == null)
			{
				m_criticalFuelNotification = new InMissionNotification("notification_fuel_critical", StationNotificationUrgency.Red, EnumToIconMapping.InteractionOrAlertType.Fuel);
				GetNotifications().RegisterNotification(m_criticalFuelNotification);
			}
		}
		if (num < 0.5f)
		{
			if (!m_hasWarnedHalfwayFuel && GetCurrentCrewman() != null)
			{
				m_hasWarnedHalfwayFuel = true;
				Singleton<SpeechPrioritiser>.Instance.RequestNewSpeech(StaticSpeechSets.FuelUnderHalf, GetCurrentCrewman());
			}
			if (Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState().GetPhysicsModel()
				.GetFuelUsage() > 1.5f)
			{
				if (m_fuelUseInefficientNotification == null)
				{
					m_fuelUseInefficientNotification = new InMissionNotification("notification_fuel_usage_high", StationNotificationUrgency.Yellow, EnumToIconMapping.InteractionOrAlertType.Fuel);
					GetNotifications().RegisterNotification(m_fuelUseInefficientNotification);
				}
				m_fuelInefficiencyTimer += Time.deltaTime;
				if (m_fuelInefficiencyTimer > 10f && GetCurrentCrewman() != null)
				{
					m_fuelInefficiencyTimer = -20f;
					Singleton<SpeechPrioritiser>.Instance.RequestNewSpeech(StaticSpeechSets.FuelUseInefficient, GetCurrentCrewman());
				}
			}
			else
			{
				m_fuelInefficiencyTimer = Mathf.Min(0f, m_fuelInefficiencyTimer);
				if (m_fuelUseInefficientNotification != null)
				{
					GetNotifications().RemoveNotification(m_fuelUseInefficientNotification);
				}
			}
		}
		if (m_richBoostTimer.IsActive() != m_isBoostShowing)
		{
			m_isBoostShowing = m_richBoostTimer.IsActive();
			GameObject[] boostVisualObjects = m_boostVisualObjects;
			foreach (GameObject gameObject in boostVisualObjects)
			{
				gameObject.SetActive(m_isBoostShowing);
			}
		}
	}

	public int GetCurrentFuelMix()
	{
		return m_currentFuelMix;
	}
}
