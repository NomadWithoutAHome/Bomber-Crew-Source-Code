using BomberCrewCommon;
using UnityEngine;
using WingroveAudio;

public class StationRadioOperator : Station
{
	[SerializeField]
	private string m_morseCodeAudioEvent;

	[SerializeField]
	private tk2dSpriteAnimator m_animator;

	[SerializeField]
	private float m_discoveryRate = 0.2f;

	[SerializeField]
	private float m_discoveryDistance = 4500f;

	[SerializeField]
	private ElectricalSystem m_electricalSystem;

	[SerializeField]
	private SkillRefreshTimer m_skillTimer;

	[SerializeField]
	private CrewmanSkillAbilityBool m_autoTagSkill;

	[SerializeField]
	private CrewmanSkillAbilityBool m_getIntelSkill;

	[SerializeField]
	private CrewmanSkillAbilityBool m_spitfireSkill;

	[SerializeField]
	private CrewmanSkillAbilityBool m_spitfirePlusSkill;

	[SerializeField]
	private float m_autoTagSpeed = 2f;

	[SerializeField]
	private float m_intelGetRangeMin = 3000f;

	[SerializeField]
	private float m_intelGetRangeMax = 10000f;

	[SerializeField]
	private float m_intelGetAngle = 45f;

	[SerializeField]
	private int m_maxToAddMin = 2;

	[SerializeField]
	private int m_maxToAddMax = 10;

	[SerializeField]
	private GameObject m_spitfirePrefab;

	private RadarSystemUpgrade m_radarSystemUpgrade;

	private bool m_hasNewMessages;

	private float m_discoveryTime = 1f;

	private InMissionNotification m_electricsOutNotification;

	private int m_numFightersSeen;

	private int m_numGroundTargetsSeen;

	private SkillRefreshTimer.SkillTimer m_autoTagSkillTimer;

	private SkillRefreshTimer.SkillTimer m_getIntelSkillTimer;

	private SkillRefreshTimer.SkillTimer m_spitfireSkillTimer;

	private float m_autoTagTimer;

	private void OnDestroy()
	{
	}

	public bool IsRadarElectricsWorking()
	{
		return !m_electricalSystem.IsBroken();
	}

	public bool IsRadarManned()
	{
		return GetCurrentCrewman() != null;
	}

	public bool IsRadarActive()
	{
		return !m_electricalSystem.IsBroken() && GetCurrentCrewman() != null;
	}

	public override void Start()
	{
		m_radarSystemUpgrade = (RadarSystemUpgrade)GetComponent<BomberSystemUniqueId>().GetUpgrade();
		m_getIntelSkillTimer = m_skillTimer.CreateNew();
		m_autoTagSkillTimer = m_skillTimer.CreateNew();
		m_spitfireSkillTimer = m_skillTimer.CreateNew();
	}

	public RadarSystemUpgrade GetRadarSystem()
	{
		return m_radarSystemUpgrade;
	}

	public SkillRefreshTimer.SkillTimer GetAutoTagTimer()
	{
		return m_autoTagSkillTimer;
	}

	public SkillRefreshTimer.SkillTimer GetIntelTimer()
	{
		return m_getIntelSkillTimer;
	}

	public SkillRefreshTimer.SkillTimer GetSpitfireTimer()
	{
		return m_spitfireSkillTimer;
	}

	public void AutoTag()
	{
		if (m_autoTagSkillTimer.CanStart() && !m_electricalSystem.IsBroken())
		{
			m_autoTagSkillTimer.Start(m_autoTagSkill.GetDurationTimer(), m_autoTagSkill.GetCooldownTimer());
			m_autoTagTimer = 0f;
		}
	}

	public void GetIntel()
	{
		if (m_getIntelSkillTimer.CanStart() && !m_electricalSystem.IsBroken())
		{
			m_getIntelSkillTimer.Start(m_getIntelSkill.GetDurationTimer(), m_getIntelSkill.GetCooldownTimer());
		}
	}

	public void DoSpitfiresNoTimer()
	{
		Vector3d position = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBigTransform().position;
		Vector3d vector3d = -position;
		vector3d.y = 0.0;
		vector3d = ((vector3d.magnitude == 0.0) ? new Vector3d(-1f, 0f, 0f) : vector3d.normalized);
		Vector3d vector3d2 = position + vector3d * 2500.0;
		if (vector3d2.y < 100.0)
		{
			vector3d2.y = 100.0;
		}
		for (int i = 0; i < 3; i++)
		{
			Vector3d position2 = vector3d2 + new Vector3d(150 * i, 0f, 150 * i);
			GameObject original = m_spitfirePrefab;
			if (Singleton<GameFlow>.Instance.GetGameMode().HasSpitfireOverride())
			{
				original = Singleton<GameFlow>.Instance.GetGameMode().GetSpitfireOverride();
			}
			GameObject gameObject = Object.Instantiate(original);
			gameObject.transform.parent = null;
			gameObject.transform.localScale = gameObject.transform.localScale;
			gameObject.transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360f), 0f));
			gameObject.btransform().position = position2;
			if (i == 0)
			{
				gameObject.GetComponent<FighterAI_FriendlySpitfire>().SetIsLeadPilot();
			}
			gameObject.GetComponent<FighterAI_FriendlySpitfire>().SetPilotIndex(i);
		}
	}

	public void DoSpitfires()
	{
		if (m_spitfireSkillTimer.CanStart() && !m_electricalSystem.IsBroken())
		{
			Crewman crewman = GetCurrentCrewman().GetCrewman();
			bool flag = Singleton<CrewmanSkillUpgradeInfo>.Instance.IsUnlocked(m_spitfirePlusSkill, crewman.GetPrimarySkill(), crewman.GetSecondarySkill());
			m_spitfireSkillTimer.Start((!flag) ? m_spitfireSkill.GetDurationTimer() : m_spitfirePlusSkill.GetDurationTimer(), (!flag) ? m_spitfireSkill.GetCooldownTimer() : m_spitfirePlusSkill.GetCooldownTimer());
			DoSpitfiresNoTimer();
		}
	}

	private void Update()
	{
		if (m_electricalSystem.IsBroken())
		{
			if (m_electricsOutNotification == null)
			{
				m_electricsOutNotification = new InMissionNotification("station_notification_electrical", StationNotificationUrgency.Red, EnumToIconMapping.InteractionOrAlertType.Electrics);
				GetNotifications().RegisterNotification(m_electricsOutNotification);
				if (GetCurrentCrewman() != null)
				{
					Singleton<SpeechPrioritiser>.Instance.RequestNewSpeech(StaticSpeechSets.RadioOpElectricOut, GetCurrentCrewman());
				}
			}
			if (m_getIntelSkillTimer.IsActive())
			{
				m_getIntelSkillTimer.FinishEarly();
				m_getIntelSkillTimer.ResetDoneFlag();
			}
		}
		else
		{
			if (m_electricsOutNotification != null)
			{
				GetNotifications().RemoveNotification(m_electricsOutNotification);
				m_electricsOutNotification = null;
			}
			if (GetCurrentCrewman() != null)
			{
				m_autoTagSkillTimer.DoRechargedSpeech(m_autoTagSkill.GetTitleText(), GetCurrentCrewman(), m_autoTagSkill);
				m_getIntelSkillTimer.DoRechargedSpeech(m_getIntelSkill.GetTitleText(), GetCurrentCrewman(), m_getIntelSkill);
				m_spitfireSkillTimer.DoRechargedSpeech(m_spitfireSkill.GetTitleText(), GetCurrentCrewman(), m_spitfireSkill);
				int numFighters = Singleton<FishpondRadar>.Instance.GetNumFighters();
				if (numFighters > m_numFightersSeen)
				{
					Singleton<SpeechPrioritiser>.Instance.RequestNewSpeech(StaticSpeechSets.RadioOpFightersSpotted, GetCurrentCrewman());
				}
				m_numFightersSeen = numFighters;
				numFighters = Singleton<FishpondRadar>.Instance.GetNumGroundTargets();
				if (numFighters > m_numGroundTargetsSeen)
				{
					Singleton<SpeechPrioritiser>.Instance.RequestNewSpeech(StaticSpeechSets.RadioOpGroundTargetsSpotted, GetCurrentCrewman());
				}
				m_numGroundTargetsSeen = numFighters;
				if (m_autoTagSkillTimer.IsActive())
				{
					m_autoTagTimer += Time.deltaTime * m_autoTagSpeed;
					if (m_autoTagTimer >= 1f)
					{
						m_autoTagTimer = 0f;
						foreach (FighterPlane allFighter in Singleton<FighterCoordinator>.Instance.GetAllFighters())
						{
							if (!allFighter.IsTagged() && allFighter.GetComponent<TaggableFighter>().IsTaggable())
							{
								allFighter.GetComponent<TaggableItem>().SetTagComplete();
								break;
							}
						}
					}
				}
				if (m_getIntelSkillTimer.IsDone())
				{
					m_getIntelSkillTimer.ResetDoneFlag();
					int num = Singleton<MissionMapController>.Instance.SpotFurther(m_intelGetRangeMax, m_intelGetAngle, m_maxToAddMax);
					WingroveRoot.Instance.PostEvent("MORSE_CODE_INCOMING");
					if (num != 0)
					{
						Singleton<SpeechPrioritiser>.Instance.RequestNewSpeech(StaticSpeechSets.RadioOp_IntelFound, GetCurrentCrewman());
					}
					else
					{
						Singleton<SpeechPrioritiser>.Instance.RequestNewSpeech(StaticSpeechSets.RadioOp_NoIntelFound, GetCurrentCrewman());
					}
				}
			}
		}
		if (GetCurrentCrewman() == null)
		{
			if (m_getIntelSkillTimer.IsActive())
			{
				m_getIntelSkillTimer.FinishEarly();
				m_getIntelSkillTimer.ResetDoneFlag();
			}
			if (m_autoTagSkillTimer.IsActive())
			{
				m_autoTagSkillTimer.FinishEarly();
			}
		}
	}
}
