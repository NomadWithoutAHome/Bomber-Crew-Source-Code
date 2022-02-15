using BomberCrewCommon;
using dbox;
using UnityEngine;
using WingroveAudio;

public class BomberState : MonoBehaviour, Shootable
{
	public enum FlightState
	{
		PreTakeOff,
		Takeoff,
		Normal,
		Corkscrew,
		EmergencyLanding,
		EmergencyDive,
		NormalLanding,
		NormalLandingPaused,
		Landed,
		EvadeFlak
	}

	[SerializeField]
	private AltitudeTarget[] m_altitudeTargets;

	[SerializeField]
	private StationPilot m_pilotStation;

	[SerializeField]
	private float m_evasiveActionDuration = 3f;

	[SerializeField]
	private float m_emergencyDiveDuration = 5f;

	[SerializeField]
	private float m_emergencyDiveMinAlt = 150f;

	[SerializeField]
	private float m_corkscrewMinAlt = 125f;

	[SerializeField]
	private float m_bomberHeightPixels;

	[SerializeField]
	private WindZone m_particleSystemWindZone;

	[SerializeField]
	private BomberFlightPhysicsModel m_physicsModel;

	[SerializeField]
	private BomberNavigation m_navigation;

	[SerializeField]
	private SkillRefreshTimer m_bomberTimers;

	[SerializeField]
	private BomberSystems m_bomberSystems;

	[SerializeField]
	private float[] m_corkscrewPitchYokeValues;

	[SerializeField]
	private float[] m_corkscrewBankYokeValues;

	[SerializeField]
	private WalkableArea[] m_externalWalkables;

	[SerializeField]
	private CrewmanSkillAbilityFloat m_pilotingSkill;

	[SerializeField]
	private GameObject[] m_trailObjects;

	[SerializeField]
	private BigTransform m_bigTransform;

	private FlightState m_currentFlightState;

	private float m_currentAltitude;

	private float m_currentAboveGroundAltitude;

	private int m_currentTarget;

	private int m_currentNearestAltitude = -1;

	private int m_currentFuelMix;

	private float m_landingBrakeAmount;

	private bool m_bailOutOrderGiven;

	private bool m_hasHitGround;

	private SkillRefreshTimer.SkillTimer m_emergencyDiveTimer;

	private SkillRefreshTimer.SkillTimer m_corkscrewTimer;

	private SkillRefreshTimer.SkillTimer m_evadeFlakTimer;

	private float m_timeWithoutPilot;

	private Vector3 m_previousDelta;

	private bool m_isAboveEngland;

	private Vector3 m_boundsMin;

	private Vector3 m_boundsMax;

	private bool m_boundsStrict;

	private float m_outOfBoundsTimer;

	private float m_timeSinceTakeOff;

	private bool m_debugInvincible;

	private float m_postManoeuvreReadjustTimer;

	private float m_evadeFlakProgress;

	private bool m_trailsEnabled;

	private bool m_hasDoneLandingSound;

	private float m_landingTimeEnginesOff;

	private string m_pilotingAbilityId;

	private RaycastHit[] buffer = new RaycastHit[128];

	private float m_threshold = 80f;

	private void Awake()
	{
		m_pilotingAbilityId = m_pilotingSkill.GetCachedName();
		m_currentAltitude = 0f;
		m_currentTarget = 0;
		m_currentFuelMix = 1;
		m_landingBrakeAmount = 1f;
		m_emergencyDiveTimer = m_bomberTimers.CreateNew();
		m_corkscrewTimer = m_bomberTimers.CreateNew();
		m_evadeFlakTimer = m_bomberTimers.CreateNew();
		SetBoundsDefaults();
		Singleton<ShootableCoordinator>.Instance.RegisterShootable(this);
		GameObject[] trailObjects = m_trailObjects;
		foreach (GameObject gameObject in trailObjects)
		{
			gameObject.SetActive(value: false);
		}
	}

	public BigTransform GetBigTransform()
	{
		return m_bigTransform;
	}

	public void SetBoundsDefaults()
	{
		if (Singleton<GameFlow>.Instance.GetGameMode().GetUseUSNaming())
		{
			SetBounds(new Vector3(-30800f, 0f, -18000f), new Vector3(12000f, 0f, 45000f));
		}
		else
		{
			SetBounds(new Vector3(-60000f, 0f, -28000f), new Vector3(73000f, 0f, 117000f));
		}
	}

	public void SetBoundsStrict()
	{
		m_boundsStrict = true;
	}

	public bool IsDestroyed()
	{
		return false;
	}

	private float GetCurrentAltitudeTargetHeight()
	{
		if (m_postManoeuvreReadjustTimer > 0f)
		{
			return m_altitudeTargets[2].GetHeight();
		}
		if (m_navigation.GetNextNavigationPoint() != null)
		{
			if (m_navigation.GetNextNavigationPoint().m_forceUltraLowAltitude)
			{
				return 50f;
			}
			if (m_navigation.GetNextNavigationPoint().m_forceUltraLowAltitude)
			{
				return m_altitudeTargets[0].GetHeight();
			}
		}
		return m_altitudeTargets[m_currentTarget].GetHeight();
	}

	public void LockWingWalking()
	{
		WalkableArea[] externalWalkables = m_externalWalkables;
		foreach (WalkableArea walkableArea in externalWalkables)
		{
			walkableArea.GetComponent<Collider>().enabled = false;
		}
	}

	public GameObject GetMainObject()
	{
		return base.gameObject;
	}

	public bool IsLitUp()
	{
		return m_bomberSystems.IsLitUp();
	}

	private void OnDestroy()
	{
		if (Singleton<ShootableCoordinator>.Instance != null)
		{
			Singleton<ShootableCoordinator>.Instance.RemoveShootable(this);
		}
	}

	public void SetBounds(Vector3 bMin, Vector3 bMax)
	{
		m_boundsMin = bMin;
		m_boundsMax = bMax;
		m_outOfBoundsTimer = 0f;
	}

	public bool IsAboveEngland()
	{
		return m_isAboveEngland;
	}

	public void EmergencyDive(bool superEmergencyDive, float rechargeTime)
	{
		if (IsEmergencyDivePossible())
		{
			if (m_emergencyDiveTimer.CanStart())
			{
				Singleton<CrewMiscSpeechTriggers>.Instance.DoExternalTrigger(CrewMiscSpeechTriggers.SpeechExternalTrigger.ManouevreStart, null);
				m_emergencyDiveTimer.Start(m_emergencyDiveDuration, rechargeTime);
				m_currentFlightState = FlightState.EmergencyDive;
				DboxInMissionController.DBoxCall(DboxSdkWrapper.PostDive);
			}
		}
		else if (m_currentAboveGroundAltitude <= m_emergencyDiveMinAlt || m_currentAltitude <= m_emergencyDiveMinAlt)
		{
			Singleton<SpeechPrioritiser>.Instance.RequestNewSpeech(StaticSpeechSets.AltitudeTooLowEmgDive, m_pilotStation.GetCurrentCrewman());
		}
	}

	public bool IsBailOutOrderActive()
	{
		return m_bailOutOrderGiven;
	}

	public void SetBailOutOrderActive(bool active)
	{
		m_bailOutOrderGiven = active;
	}

	public bool IsEmergencyDivePossible()
	{
		if ((m_currentFlightState == FlightState.Normal || m_currentFlightState == FlightState.EvadeFlak) && m_currentAboveGroundAltitude > m_emergencyDiveMinAlt && m_currentAltitude > m_emergencyDiveMinAlt)
		{
			return true;
		}
		return false;
	}

	public bool IsCorkscrewPossible()
	{
		if (m_currentAboveGroundAltitude > m_corkscrewMinAlt)
		{
			return m_currentFlightState == FlightState.Normal || m_currentFlightState == FlightState.EvadeFlak;
		}
		return false;
	}

	public bool IsEvadeFlakPossible()
	{
		return m_currentFlightState == FlightState.Normal;
	}

	public void DoEvadeFlak(float duration, float rechargeTime)
	{
		if (IsEvadeFlakPossible() && m_evadeFlakTimer.CanStart())
		{
			m_currentFlightState = FlightState.EvadeFlak;
			m_evadeFlakTimer.Start(duration, rechargeTime);
			m_evadeFlakProgress = 0f;
		}
	}

	public void DoCorkscrew(float rechargeTime)
	{
		if (IsCorkscrewPossible())
		{
			if (m_corkscrewTimer.CanStart())
			{
				Singleton<SaveDataContainer>.Instance.Get().SetHasCorkscrewed();
				Singleton<CrewMiscSpeechTriggers>.Instance.DoExternalTrigger(CrewMiscSpeechTriggers.SpeechExternalTrigger.ManouevreStart, null);
				m_currentFlightState = FlightState.Corkscrew;
				m_corkscrewTimer.Start(m_evasiveActionDuration, rechargeTime);
				DboxInMissionController.DBoxCall(DboxSdkWrapper.PostCorkscrew);
			}
		}
		else if (m_currentAboveGroundAltitude <= m_corkscrewMinAlt)
		{
			Singleton<SpeechPrioritiser>.Instance.RequestNewSpeech(StaticSpeechSets.AltitudeTooLowCorkscrew, m_pilotStation.GetCurrentCrewman());
		}
	}

	public SkillRefreshTimer.SkillTimer GetCorkscrewTimer()
	{
		return m_corkscrewTimer;
	}

	public SkillRefreshTimer.SkillTimer GetEvadeFlakTimer()
	{
		return m_evadeFlakTimer;
	}

	public float GetCorkscrewProgress()
	{
		return m_corkscrewTimer.GetProgressNormalised();
	}

	public float GetBomberHeightPixels()
	{
		return m_bomberHeightPixels;
	}

	public bool IsEvasive()
	{
		return m_currentFlightState == FlightState.Corkscrew;
	}

	public bool CanCrewOperate()
	{
		if (m_currentFlightState == FlightState.Corkscrew || m_currentFlightState == FlightState.EmergencyDive)
		{
			return false;
		}
		return true;
	}

	public void SetAltitudeTarget(int targetIndex)
	{
		m_currentTarget = targetIndex;
	}

	public int GetAltitudeTarget()
	{
		return m_currentTarget;
	}

	public float GetAltitude()
	{
		return m_currentAltitude;
	}

	public float GetAltitudeTargetForIndex(int index)
	{
		return m_altitudeTargets[index].GetHeight();
	}

	public float GetAltitudeAboveGround()
	{
		return m_currentAboveGroundAltitude;
	}

	public float GetTargetAltitudeAtIndex(int index)
	{
		return m_altitudeTargets[index].GetHeight();
	}

	public float GetAltitudeNormalised()
	{
		return m_currentAltitude / m_altitudeTargets[m_altitudeTargets.Length - 1].GetHeight();
	}

	public bool IsLanded()
	{
		return m_currentFlightState == FlightState.Landed;
	}

	public void SetFuelMix(int fuelMix)
	{
		m_currentFuelMix = fuelMix;
	}

	public int GetFuelMix()
	{
		return m_currentFuelMix;
	}

	public bool IsEvadingFlak()
	{
		return m_evadeFlakTimer.IsActive();
	}

	private float GetCurrentAboveGroundAltitude()
	{
		int num = Physics.RaycastNonAlloc(new Vector3(base.transform.position.x, 1000f, base.transform.position.z), Vector3.down, buffer, 1001f, 1 << LayerMask.NameToLayer("Environment"));
		float num2 = float.MaxValue;
		m_isAboveEngland = false;
		int num3 = 0;
		for (int i = 0; i < num; i++)
		{
			if (!buffer[i].collider.isTrigger)
			{
				num3++;
				num2 = Mathf.Min(buffer[i].distance, num2);
				if (buffer[i].collider.GetComponent<SafeLandmass>() != null)
				{
					m_isAboveEngland = true;
				}
			}
		}
		float num4 = 1000f - num2;
		if (num3 == 0)
		{
			return m_currentAltitude;
		}
		return m_currentAltitude - num4;
	}

	private void DoPilotlessUpdate()
	{
		m_timeWithoutPilot += Time.deltaTime;
		m_physicsModel.SetYoke(0.075f - m_timeWithoutPilot * 0.00208333344f);
	}

	private void DoPilotUpdate(float pilotingCapability)
	{
		m_timeWithoutPilot = 0f;
		LandingGear landingGear = m_bomberSystems.GetLandingGear();
		m_postManoeuvreReadjustTimer -= Time.deltaTime;
		if (m_currentFlightState == FlightState.PreTakeOff)
		{
			m_physicsModel.SetYoke(0f);
			m_landingBrakeAmount = 1f;
		}
		else if (m_currentFlightState == FlightState.Takeoff)
		{
			m_landingBrakeAmount -= Time.deltaTime * 0.125f;
			m_physicsModel.SetYoke(0.05f);
			if (m_currentAboveGroundAltitude > 50f)
			{
				m_landingBrakeAmount = 0f;
				m_currentFlightState = FlightState.Normal;
			}
		}
		else if (IsLanding() || m_currentFlightState == FlightState.Landed)
		{
			if (m_physicsModel.IsOnGround())
			{
				if (m_physicsModel.AllWheelsOnGround() && !m_hasDoneLandingSound)
				{
					m_physicsModel.SetDoLandingFX();
					m_hasDoneLandingSound = true;
					WingroveRoot.Instance.PostEventGO("LAND_SUCCESS", base.gameObject);
				}
				m_landingTimeEnginesOff += Time.deltaTime;
				if (m_landingTimeEnginesOff > 1f)
				{
					int engineCount = m_bomberSystems.GetEngineCount();
					for (int i = 0; i < engineCount; i++)
					{
						if (m_bomberSystems.GetEngine(i) != null)
						{
							m_bomberSystems.GetEngine(i).SwitchOff();
						}
					}
				}
				m_physicsModel.SetYoke(0f);
			}
			else
			{
				m_landingTimeEnginesOff = 0f;
				m_hasDoneLandingSound = false;
				float value = 0f - m_currentAboveGroundAltitude;
				Vector3 normalized = m_physicsModel.GetVelocity().normalized;
				Vector3 normalized2 = m_physicsModel.GetHeading().normalized;
				float num = Mathf.Clamp(value, -300f, 300f) / 200f;
				float num2 = num - normalized.y * 2.5f;
				float b = 10f;
				float a = 7f;
				float num3 = Mathf.Lerp(a, b, pilotingCapability);
				if (m_currentAboveGroundAltitude < num3 || m_physicsModel.GetPreviousEngineForce() == 0f)
				{
					float b2 = num2 - normalized2.y * 2.5f;
					float a2 = num2 - normalized2.y * 2f;
					float yoke = Mathf.Lerp(a2, b2, pilotingCapability);
					m_physicsModel.SetYoke(yoke);
				}
				else
				{
					m_physicsModel.SetYoke(num2);
				}
			}
		}
		else if (m_physicsModel.GetVelocity().normalized.y < 0f && m_physicsModel.GetHeading().normalized.y > 0f)
		{
			Vector3 normalized3 = m_physicsModel.GetHeading().normalized;
			m_physicsModel.SetYoke(0f - normalized3.y);
		}
		else
		{
			float num4 = GetCurrentAltitudeTargetHeight() - m_currentAltitude;
			if (num4 < 100f && num4 > -100f)
			{
				if (m_currentNearestAltitude != m_currentTarget)
				{
					if (m_currentTarget == 2)
					{
						Singleton<CrewMiscSpeechTriggers>.Instance.DoExternalTrigger(CrewMiscSpeechTriggers.SpeechExternalTrigger.ReachedAltitudeHigh, null);
					}
					else if (m_currentTarget == 1)
					{
						Singleton<CrewMiscSpeechTriggers>.Instance.DoExternalTrigger(CrewMiscSpeechTriggers.SpeechExternalTrigger.ReachedAltitudeMid, null);
					}
					else if (m_currentTarget == 0)
					{
						Singleton<CrewMiscSpeechTriggers>.Instance.DoExternalTrigger(CrewMiscSpeechTriggers.SpeechExternalTrigger.ReachedAltitudeLow, null);
					}
				}
				m_currentNearestAltitude = m_currentTarget;
			}
			Vector3 normalized4 = m_physicsModel.GetVelocity().normalized;
			float num5 = Mathf.Clamp(num4, -300f, 300f) / 450f;
			float b3 = num5 - normalized4.y * 4f;
			float a3 = num5 - normalized4.y * 5f;
			m_physicsModel.SetYoke(Mathf.Lerp(a3, b3, pilotingCapability));
			if (m_currentFlightState == FlightState.Corkscrew)
			{
				int num6 = (int)(GetCorkscrewProgress() * (float)m_corkscrewBankYokeValues.Length) % m_corkscrewBankYokeValues.Length;
				m_physicsModel.SetYoke(m_corkscrewPitchYokeValues[num6]);
				float yokeBank = m_corkscrewBankYokeValues[num6] - (m_physicsModel.GetTurningVelocity() * 2.5f + m_physicsModel.GetBanking() * 0.1f);
				m_physicsModel.SetYokeBank(yokeBank);
				if (m_corkscrewTimer.IsDone())
				{
					m_currentFlightState = FlightState.Normal;
				}
				m_postManoeuvreReadjustTimer = 5f;
			}
			else if (m_currentFlightState == FlightState.EmergencyDive)
			{
				m_physicsModel.SetYoke(-0.3f - normalized4.y * 0.25f);
				if (!m_emergencyDiveTimer.IsActive() || m_currentAboveGroundAltitude < m_emergencyDiveMinAlt || m_currentAltitude < m_emergencyDiveMinAlt)
				{
					m_emergencyDiveTimer.FinishEarly();
					m_currentFlightState = FlightState.Normal;
				}
				m_postManoeuvreReadjustTimer = 5f;
			}
			else if (m_currentFlightState == FlightState.EvadeFlak)
			{
				if (m_evadeFlakTimer.IsDone())
				{
					m_currentFlightState = FlightState.Normal;
				}
				m_evadeFlakProgress += Time.deltaTime;
			}
		}
		Vector3 normalized5 = m_physicsModel.GetVelocity().normalized;
		normalized5.y = 0f;
		normalized5 = normalized5.normalized;
		Vector3 lhs = normalized5.normalized;
		BomberNavigation.NavigationPoint nextNavigationPoint = m_navigation.GetNextNavigationPoint();
		if (nextNavigationPoint != null)
		{
			if (nextNavigationPoint.m_hasDirection)
			{
				if (nextNavigationPoint.m_holdSteady)
				{
					lhs = (Vector3)(nextNavigationPoint.m_position - m_bigTransform.position).normalized;
					lhs.y = 0f;
					if (Vector3.Dot(lhs, nextNavigationPoint.m_direction) < 0.95f)
					{
						lhs = nextNavigationPoint.m_direction;
					}
				}
				else
				{
					Vector3 heading = m_physicsModel.GetHeading();
					heading.y = 0f;
					float num7 = Vector3.Dot(heading, nextNavigationPoint.m_direction) * 0.5f + 0.5f;
					float num8 = Mathf.Pow(1f - num7, 0.5f);
					float num9 = Mathf.Clamp01(1f - num7 * 2f);
					float num10 = Mathf.Clamp01(((float)(nextNavigationPoint.m_position - m_bigTransform.position).magnitude - 100f) / 400f);
					float num11 = num8 * 0.75f + num10 * num8 * 0.25f;
					float num12 = Vector3.Dot(heading, nextNavigationPoint.m_direction);
					Vector3d vector3d = nextNavigationPoint.m_position + new Vector3d(nextNavigationPoint.m_direction * 250f * num12) - new Vector3d(nextNavigationPoint.m_direction * 200f * num10);
					Vector3 vector = (Vector3)(vector3d - m_bigTransform.position).normalized;
					Debug.DrawLine((Vector3)vector3d, (Vector3)nextNavigationPoint.m_position);
					lhs = vector;
				}
			}
			else
			{
				lhs = (Vector3)(nextNavigationPoint.m_position - m_bigTransform.position).normalized;
				lhs.y = 0f;
			}
			if (HasHitNavigationPoint(nextNavigationPoint))
			{
				m_navigation.MarkNavigationPointReached();
				if (nextNavigationPoint.m_switchToLandingState)
				{
					if (m_currentAltitude < m_altitudeTargets[0].GetHeight() + 200f && landingGear.IsFullyLowered())
					{
						m_currentFlightState = FlightState.NormalLanding;
						m_navigation.SetFinalLanding();
					}
					else
					{
						if (m_pilotStation.GetCurrentCrewman() != null)
						{
							if (!landingGear.IsFullyLowered())
							{
								Singleton<SpeechPrioritiser>.Instance.RequestNewSpeech(StaticSpeechSets.PilotLandingGearStillUp, m_pilotStation.GetCurrentCrewman());
							}
							else
							{
								Singleton<SpeechPrioritiser>.Instance.RequestNewSpeech(StaticSpeechSets.PilotLandingAltitudeTooHigh, m_pilotStation.GetCurrentCrewman());
							}
						}
						m_navigation.SetPreLanding(0);
					}
				}
				else if (nextNavigationPoint.m_goToNextPreLanding)
				{
					m_navigation.SetPreLanding(nextNavigationPoint.m_nextPreLanding);
				}
			}
			if (m_boundsStrict)
			{
				Vector3d position = m_bigTransform.position;
				if (position.x > (double)m_boundsMax.x || position.x < (double)m_boundsMin.x || position.z > (double)m_boundsMax.z || position.z < (double)m_boundsMin.z)
				{
					m_outOfBoundsTimer = 5f;
				}
				m_outOfBoundsTimer -= Time.deltaTime;
				if (m_outOfBoundsTimer > 0f)
				{
					Vector3d vector3d2 = new Vector3d((m_boundsMin + m_boundsMax) / 2f);
					lhs = (Vector3)(vector3d2 - position).normalized;
				}
			}
		}
		else
		{
			Vector3d position2 = m_bigTransform.position;
			if (position2.x > (double)m_boundsMax.x || position2.x < (double)m_boundsMin.x || position2.z > (double)m_boundsMax.z || position2.z < (double)m_boundsMin.z)
			{
				m_outOfBoundsTimer = 10f;
			}
			m_outOfBoundsTimer -= Time.deltaTime;
			if (m_outOfBoundsTimer > 0f)
			{
				Vector3d vector3d3 = new Vector3d((m_boundsMin + m_boundsMax) / 2f);
				lhs = (Vector3)(vector3d3 - position2).normalized;
			}
		}
		float num13 = Mathf.Atan2(normalized5.x, 0f - normalized5.z) * 57.29578f;
		float num14 = Mathf.Atan2(lhs.x, 0f - lhs.z) * 57.29578f;
		float num15 = num14 - num13;
		if (num15 > 180f)
		{
			num15 = num14 - 360f - num13;
		}
		else if (num15 <= -180f)
		{
			num15 = num14 + 360f - num13;
		}
		float num16 = Mathf.Clamp01(Mathf.Abs(m_physicsModel.GetPitch() / 45f));
		float num17 = 1f - num16 * 0.5f;
		float num18 = Mathf.Clamp(num15 / 30f, -3f, 3f) * num17;
		switch (m_currentFlightState)
		{
		case FlightState.PreTakeOff:
		case FlightState.Takeoff:
		case FlightState.EmergencyLanding:
		case FlightState.EmergencyDive:
		case FlightState.Landed:
			m_physicsModel.SetYokeBank(0f);
			break;
		case FlightState.EvadeFlak:
		{
			float num19 = Mathf.Sin(m_evadeFlakProgress * 0.5f) * 0.75f;
			float yokeBank2 = num19 - (m_physicsModel.GetTurningVelocity() * 2.5f + m_physicsModel.GetBanking() * 0.1f);
			m_physicsModel.SetYokeBank(yokeBank2);
			break;
		}
		case FlightState.NormalLanding:
		{
			float b5 = num18 - (m_physicsModel.GetTurningVelocity() * 2.5f + m_physicsModel.GetBanking() * 0.1f);
			float a5 = num18 - (m_physicsModel.GetTurningVelocity() * 1.5f + m_physicsModel.GetBanking() * 0.075f);
			if (m_currentAboveGroundAltitude > 50f)
			{
				m_physicsModel.SetYokeBank(Mathf.Lerp(a5, b5, pilotingCapability));
			}
			else
			{
				m_physicsModel.SetYokeBank(Mathf.Lerp(a5, b5, pilotingCapability) * Mathf.Clamp01((m_currentAboveGroundAltitude - 30f) / 20f));
			}
			break;
		}
		default:
		{
			float b4 = num18 - (m_physicsModel.GetTurningVelocity() * 2.5f + m_physicsModel.GetBanking() * 0.1f);
			float a4 = num18 - (m_physicsModel.GetTurningVelocity() * 1.5f + m_physicsModel.GetBanking() * 0.075f);
			m_physicsModel.SetYokeBank(Mathf.Lerp(a4, b4, pilotingCapability));
			break;
		}
		case FlightState.Corkscrew:
			break;
		}
		if (m_currentFlightState == FlightState.Corkscrew)
		{
			m_physicsModel.SetBankEffectMultiplier(3f);
			m_physicsModel.SetDoingManeuvre();
		}
		else
		{
			m_physicsModel.SetBankEffectMultiplier(1f);
		}
		if (m_currentFlightState == FlightState.EmergencyDive)
		{
			m_physicsModel.SetDoingManeuvre();
		}
		if (m_currentFlightState != 0)
		{
			m_timeSinceTakeOff += Time.deltaTime;
		}
		bool flag = false;
		FlightState currentFlightState = m_currentFlightState;
		if (currentFlightState == FlightState.Corkscrew || currentFlightState == FlightState.EmergencyDive || currentFlightState == FlightState.EvadeFlak)
		{
			flag = true;
		}
		if (m_trailsEnabled != flag)
		{
			m_trailsEnabled = flag;
			GameObject[] trailObjects = m_trailObjects;
			foreach (GameObject gameObject in trailObjects)
			{
				gameObject.SetActive(m_trailsEnabled);
			}
		}
	}

	public void ForceToFlying(bool setInvincible)
	{
		for (int i = 0; i < 4; i++)
		{
			m_bomberSystems.GetEngineOrdered(i).SwitchOn();
			if (setInvincible)
			{
				m_bomberSystems.GetEngineOrdered(i).SetInvincible(invincible: true);
			}
		}
		m_currentFlightState = FlightState.Normal;
		m_physicsModel.ForceVelocity(60f);
		m_landingBrakeAmount = 0f;
	}

	public bool HasHitNavigationPoint(BomberNavigation.NavigationPoint np)
	{
		Vector3d position = m_bigTransform.position;
		position.y = 0.0;
		Vector3d position2 = np.m_position;
		position2.y = 0.0;
		Vector3 normalized = m_physicsModel.GetHeading().normalized;
		normalized.y = 0f;
		Vector3 vector = (Vector3)(position - position2);
		Vector3 lhs = vector - m_previousDelta;
		bool flag = false;
		if (lhs.magnitude > 0f && m_previousDelta.magnitude > 0f && vector.magnitude < m_threshold * 4f && Mathf.Sign(Vector3.Dot(lhs, vector)) != Mathf.Sign(Vector3.Dot(lhs, m_previousDelta)))
		{
			flag = true;
		}
		m_previousDelta = (Vector3)(position - position2);
		if ((position - position2).magnitude < (double)m_threshold || flag)
		{
			if (np.m_hasDirection)
			{
				if (Vector3.Dot(normalized, np.m_direction) > 0f)
				{
					m_previousDelta = Vector3.zero;
					return true;
				}
				return false;
			}
			m_previousDelta = Vector3.zero;
			return true;
		}
		return false;
	}

	public bool IsLanding()
	{
		return m_currentFlightState == FlightState.NormalLanding || m_currentFlightState == FlightState.EmergencyLanding;
	}

	public bool IsPreLandingClose()
	{
		BomberNavigation.NavigationPoint nextNavigationPoint = m_navigation.GetNextNavigationPoint();
		if (nextNavigationPoint != null)
		{
			if (nextNavigationPoint.m_goToNextPreLanding)
			{
				if (nextNavigationPoint.m_nextPreLanding > 1)
				{
					if (Singleton<BomberSpawn>.Instance.GetBomberSystems().GetLandingGear().IsFullyLowered() && m_currentAltitude < m_altitudeTargets[0].GetHeight() + 200f)
					{
						return true;
					}
				}
				else if ((nextNavigationPoint.m_position - m_bigTransform.position).magnitude < 500.0 && Singleton<BomberSpawn>.Instance.GetBomberSystems().GetLandingGear().IsFullyLowered() && m_currentAltitude < m_altitudeTargets[0].GetHeight() + 200f)
				{
					return true;
				}
			}
			else if (nextNavigationPoint.m_switchToLandingState && Singleton<BomberSpawn>.Instance.GetBomberSystems().GetLandingGear().IsFullyLowered() && m_currentAltitude < m_altitudeTargets[0].GetHeight() + 200f)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsPreLanding()
	{
		BomberNavigation.NavigationPoint nextNavigationPoint = m_navigation.GetNextNavigationPoint();
		if (nextNavigationPoint != null && (nextNavigationPoint.m_goToNextPreLanding || nextNavigationPoint.m_switchToLandingState) && Singleton<BomberSpawn>.Instance.GetBomberSystems().GetLandingGear().IsFullyLowered() && m_currentAltitude < m_altitudeTargets[0].GetHeight() + 200f)
		{
			return true;
		}
		return false;
	}

	public bool HasNotCompletedTakeOff()
	{
		return m_currentFlightState == FlightState.PreTakeOff || m_currentFlightState == FlightState.Takeoff;
	}

	public bool ShouldEnableAllControls()
	{
		if (HasNotCompletedTakeOff())
		{
			return m_currentFlightState != 0 && m_currentAboveGroundAltitude > 5f;
		}
		return true;
	}

	public bool HasNotStartedTakeOff()
	{
		return m_currentFlightState == FlightState.PreTakeOff;
	}

	public bool IsAbilityActive()
	{
		return m_corkscrewTimer.IsActive() || m_emergencyDiveTimer.IsActive();
	}

	private void DoCommonUpdate()
	{
		m_currentAltitude = base.transform.position.y;
		m_currentAboveGroundAltitude = GetCurrentAboveGroundAltitude();
		m_physicsModel.SetHorizontalForces((!IsLanding()) ? m_currentFuelMix : 0, m_landingBrakeAmount);
	}

	public BomberFlightPhysicsModel GetPhysicsModel()
	{
		return m_physicsModel;
	}

	private void Update()
	{
		if (m_pilotStation.GetCurrentCrewman() == null)
		{
			DoPilotlessUpdate();
		}
		else
		{
			float proficiency = Singleton<CrewmanSkillUpgradeInfo>.Instance.GetProficiency(m_pilotingAbilityId, m_pilotStation.GetCurrentCrewman().GetCrewman());
			DoPilotUpdate(proficiency);
		}
		DoCommonUpdate();
		m_particleSystemWindZone.windMain = Mathf.Abs(m_physicsModel.GetVelocity().x * 30f);
		Vector3 velocity = m_physicsModel.GetVelocity();
		velocity.y = 0f;
		Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetMissionLog().AddDistanceFlown(velocity.magnitude * Time.deltaTime);
		if (m_currentFlightState == FlightState.PreTakeOff || !m_physicsModel.IsOnGround())
		{
			return;
		}
		bool flag = false;
		if (m_timeSinceTakeOff > 15f && m_physicsModel.GetVelocity().magnitude < 30f)
		{
			flag = true;
		}
		if (!flag)
		{
			return;
		}
		m_currentFlightState = FlightState.Landed;
		if (!(m_pilotStation.GetCurrentCrewman() == null))
		{
			return;
		}
		int engineCount = m_bomberSystems.GetEngineCount();
		for (int i = 0; i < engineCount; i++)
		{
			if (m_bomberSystems.GetEngine(i) != null)
			{
				m_bomberSystems.GetEngine(i).SwitchOff();
			}
		}
	}

	public void TakeOff()
	{
		if (m_currentFlightState != 0)
		{
			return;
		}
		DboxInMissionController.DBoxCall(DboxSdkWrapper.PostTakeOff);
		Singleton<MissionCoordinator>.Instance.OnPlayerTakeOff();
		int engineCount = m_bomberSystems.GetEngineCount();
		for (int i = 0; i < engineCount; i++)
		{
			if (m_bomberSystems.GetEngine(i) != null)
			{
				m_bomberSystems.GetEngine(i).SwitchOn();
			}
		}
		m_currentFlightState = FlightState.Takeoff;
	}

	public void EmergencyLand()
	{
		if (m_currentFlightState == FlightState.Normal)
		{
			m_currentFlightState = FlightState.EmergencyLanding;
		}
	}

	public void CancelEmergencyLand()
	{
		if (m_currentFlightState == FlightState.EmergencyLanding)
		{
			m_currentFlightState = FlightState.Normal;
		}
	}

	public bool CanToggleEmergencyLand()
	{
		return m_currentFlightState == FlightState.EmergencyLanding || m_currentFlightState == FlightState.Normal;
	}

	public bool IsDoingEmergencyLand()
	{
		return m_currentFlightState == FlightState.EmergencyLanding;
	}

	public SkillRefreshTimer.SkillTimer GetEmergencyDiveTimer()
	{
		return m_emergencyDiveTimer;
	}

	public bool IsAltitudeLowEnoughToland()
	{
		return GetAltitudeNormalised() < 0.1f;
	}

	public bool IsPreTakeOff()
	{
		return m_currentFlightState == FlightState.PreTakeOff;
	}

	private void SetInvincible(bool invincible)
	{
		if (invincible == m_debugInvincible)
		{
			return;
		}
		m_debugInvincible = invincible;
		BomberFuselageSection[] componentsInChildren = GetComponentsInChildren<BomberFuselageSection>();
		BomberFuselageSection[] array = componentsInChildren;
		foreach (BomberFuselageSection bomberFuselageSection in array)
		{
			bomberFuselageSection.SetInvincible(m_debugInvincible);
		}
		for (int j = 0; j < m_bomberSystems.GetEngineCount(); j++)
		{
			Engine engine = m_bomberSystems.GetEngine(j);
			if (engine != null)
			{
				engine.SetInvincible(m_debugInvincible);
			}
		}
	}

	private void DebugInfo()
	{
		BigTransform bigTransform = m_bigTransform;
		string text = $"Bomber State: {m_currentFlightState} X: {bigTransform.position.x:0.00} Y: {bigTransform.position.y:0.00} Z: {bigTransform.position.z:0.00}";
		GUILayout.Label(text);
		bool invincible = GUILayout.Toggle(m_debugInvincible, "Bomber Mostly Invincible");
		SetInvincible(invincible);
	}

	public Vector3 GetVelocity()
	{
		return m_physicsModel.GetVelocity();
	}

	public Transform GetRandomTargetableArea()
	{
		return m_bomberSystems.GetTargetableAreas()[Random.Range(0, m_bomberSystems.GetTargetableAreas().Count)];
	}

	public Transform GetCentreTransform()
	{
		return base.transform;
	}

	public ShootableType GetShootableType()
	{
		return ShootableType.Player;
	}
}
