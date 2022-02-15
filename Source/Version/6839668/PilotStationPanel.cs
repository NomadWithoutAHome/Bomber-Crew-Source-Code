using BomberCrewCommon;
using Common;
using UnityEngine;
using WingroveAudio;

public class PilotStationPanel : StationPanel
{
	[SerializeField]
	private PanelMultiSelect m_targetAltitudeMultiSelect;

	[SerializeField]
	private PanelMultiSelect m_landingGearMultiSelect;

	[SerializeField]
	private PanelToggleButton m_emergencyLandToggle;

	[SerializeField]
	private PanelToggleButton m_bailOutOrderToggle;

	[SerializeField]
	private PanelToggleButton m_abortMissionToggle;

	[SerializeField]
	private PanelCooldownButton m_corkscrewButton;

	[SerializeField]
	private PanelCooldownButton m_emergencyDiveButton;

	[SerializeField]
	private PanelCooldownButton m_flakEvadeButton;

	[SerializeField]
	private tk2dUIItem m_takeOffButton;

	[SerializeField]
	private CrewmanSkillAbilityBool m_emergencyDiveSkill;

	[SerializeField]
	private CrewmanSkillAbilityBool m_emergencyDivePlusSkill;

	[SerializeField]
	private CrewmanSkillAbilityBool m_corkscrewSkill;

	[SerializeField]
	private CrewmanSkillAbilityBool m_corkscrewPlusSkill;

	[SerializeField]
	private CrewmanSkillAbilityBool m_flakEvadeSkill;

	[SerializeField]
	private CrewmanSkillAbilityBool m_flakEvadePlusSkill;

	[SerializeField]
	private float m_atAltitudeThreshold = 25f;

	[SerializeField]
	private GameObject m_systemAlertHydraulics;

	[SerializeField]
	private GameObject m_enableWhenTakenOff;

	[SerializeField]
	private GameObject m_disableWhenTakenOff;

	[SerializeField]
	private PanelToggleButton m_lockToggleButton;

	[SerializeField]
	private Animation m_lockFlashAnimation;

	[SerializeField]
	private Transform m_mainOffset;

	[SerializeField]
	private Vector3 m_offsetWhenController;

	[SerializeField]
	private Vector3 m_offsetNoEmergency;

	[SerializeField]
	private Vector3 m_offsetMouseNoEmergency = new Vector3(116f, 0f, 0f);

	[SerializeField]
	private GameObject[] m_emergencyControls;

	private Camera m_stationCamera;

	private BomberSystems m_bomberSystems;

	private BomberState m_bomber;

	private bool m_isHydraulicsAlertActive;

	private bool m_areAllControlsActive;

	protected override void SetUpStation()
	{
		m_bomberSystems = Singleton<BomberSpawn>.Instance.GetBomberSystems();
		m_bomber = m_bomberSystems.GetBomberState();
		m_targetAltitudeMultiSelect.OnSelectIndex += AltitudeIndexClick;
		m_landingGearMultiSelect.OnSelectIndex += LandingGearClick;
		m_takeOffButton.OnClick += TakeOffClick;
		m_corkscrewButton.OnClick += CorkscrewClick;
		m_emergencyDiveButton.OnClick += EmergencyDiveClick;
		m_emergencyLandToggle.OnClick += EmergencyLandClick;
		m_bailOutOrderToggle.OnClick += BailOutOrderClick;
		m_abortMissionToggle.OnClick += AbortMissionClick;
		m_flakEvadeButton.OnClick += FlakEvadeClick;
		if (m_lockToggleButton != null)
		{
			m_lockToggleButton.OnClick += ToggleLock;
		}
		m_takeOffButton.gameObject.SetActive(value: false);
		m_takeOffButton.gameObject.CustomActivate(m_bomberSystems.GetBomberState().IsPreTakeOff());
		m_enableWhenTakenOff.SetActive(value: true);
		m_disableWhenTakenOff.SetActive(value: false);
		m_areAllControlsActive = true;
		if (Singleton<GameFlow>.Instance.GetGameMode().UseEndlessDifficulty())
		{
			GameObject[] emergencyControls = m_emergencyControls;
			foreach (GameObject gameObject in emergencyControls)
			{
				gameObject.SetActive(value: false);
			}
		}
		Refresh();
	}

	private void Update()
	{
		Refresh();
	}

	private void EmergencyLandClick()
	{
		if (!m_emergencyLandToggle.GetState())
		{
			if (GetStation().GetCurrentCrewman() != null)
			{
				Singleton<SpeechPrioritiser>.Instance.RequestNewSpeech(StaticSpeechSets.EmergencyLandingStart, GetStation().GetCurrentCrewman());
			}
			m_bomber.EmergencyLand();
		}
		else
		{
			m_bomber.CancelEmergencyLand();
		}
	}

	private void AbortMissionClick()
	{
		Singleton<MissionCoordinator>.Instance.SetAbort(!m_abortMissionToggle.GetState());
	}

	public void BailOutOrderClick()
	{
		if (!m_bailOutOrderToggle.GetState())
		{
			if (GetStation().GetCurrentCrewman() != null)
			{
				Singleton<SpeechPrioritiser>.Instance.RequestNewSpeech(StaticSpeechSets.OrderBailOut, GetStation().GetCurrentCrewman());
			}
			m_bomberSystems.GetBomberState().SetBailOutOrderActive(active: true);
		}
		else
		{
			m_bomberSystems.GetBomberState().SetBailOutOrderActive(active: false);
		}
	}

	private void EmergencyDiveClick()
	{
		bool flag = Singleton<CrewmanSkillUpgradeInfo>.Instance.IsUnlocked(m_emergencyDivePlusSkill, GetCrewman().GetPrimarySkill(), GetCrewman().GetSecondarySkill());
		m_bomberSystems.GetBomberState().EmergencyDive(flag, (!flag) ? m_emergencyDiveSkill.GetCooldownTimer() : m_emergencyDivePlusSkill.GetCooldownTimer());
	}

	private void FlakEvadeClick()
	{
		bool flag = Singleton<CrewmanSkillUpgradeInfo>.Instance.IsUnlocked(m_flakEvadePlusSkill, GetCrewman().GetPrimarySkill(), GetCrewman().GetSecondarySkill());
		m_bomberSystems.GetBomberState().DoEvadeFlak((!flag) ? m_flakEvadeSkill.GetDurationTimer() : m_flakEvadePlusSkill.GetDurationTimer(), (!flag) ? m_flakEvadeSkill.GetCooldownTimer() : m_flakEvadePlusSkill.GetCooldownTimer());
	}

	private void CorkscrewClick()
	{
		bool flag = Singleton<CrewmanSkillUpgradeInfo>.Instance.IsUnlocked(m_corkscrewPlusSkill, GetCrewman().GetPrimarySkill(), GetCrewman().GetSecondarySkill());
		m_bomber.DoCorkscrew((!flag) ? m_corkscrewSkill.GetCooldownTimer() : m_corkscrewPlusSkill.GetCooldownTimer());
	}

	private void TakeOffClick()
	{
		m_bomber.TakeOff();
		m_takeOffButton.gameObject.CustomActivate(active: false);
	}

	private void Refresh()
	{
		if (Singleton<UISelector>.Instance.IsPrimary())
		{
			if (Singleton<GameFlow>.Instance.GetGameMode().UseEndlessDifficulty())
			{
				m_mainOffset.localPosition = m_offsetNoEmergency + -m_offsetWhenController;
			}
			else
			{
				m_mainOffset.localPosition = m_offsetWhenController;
			}
		}
		else if (Singleton<GameFlow>.Instance.GetGameMode().UseEndlessDifficulty())
		{
			m_mainOffset.localPosition = m_offsetMouseNoEmergency;
		}
		else
		{
			m_mainOffset.localPosition = Vector3.zero;
		}
		int altitudeTarget = m_bomber.GetAltitudeTarget();
		m_targetAltitudeMultiSelect.SetSelected(altitudeTarget);
		float altitude = m_bomber.GetAltitude();
		float targetAltitudeAtIndex = m_bomber.GetTargetAltitudeAtIndex(altitudeTarget);
		float num = Mathf.Abs(altitude - targetAltitudeAtIndex);
		m_targetAltitudeMultiSelect.SetInProgress(num > m_atAltitudeThreshold);
		switch (m_bomberSystems.GetLandingGear().GetState())
		{
		case LandingGear.GearState.Lowered:
			m_landingGearMultiSelect.SetSelected(0);
			m_landingGearMultiSelect.SetInProgress(inProgress: false);
			break;
		case LandingGear.GearState.Raised:
			m_landingGearMultiSelect.SetSelected(1);
			m_landingGearMultiSelect.SetInProgress(inProgress: false);
			break;
		case LandingGear.GearState.Lowering:
			m_landingGearMultiSelect.SetSelected(0);
			m_landingGearMultiSelect.SetInProgress(inProgress: true);
			break;
		case LandingGear.GearState.Raising:
			m_landingGearMultiSelect.SetSelected(1);
			m_landingGearMultiSelect.SetInProgress(inProgress: true);
			break;
		}
		SkillRefreshTimer.SkillTimer emergencyDiveTimer = m_bomber.GetEmergencyDiveTimer();
		m_emergencyDiveButton.SetStatus(emergencyDiveTimer.IsActive(), emergencyDiveTimer.CanStart(), !m_bomber.IsEmergencyDivePossible(), emergencyDiveTimer.GetRechargeNormalised(), emergencyDiveTimer.GetInUseNormalised());
		emergencyDiveTimer = m_bomber.GetCorkscrewTimer();
		m_corkscrewButton.SetStatus(emergencyDiveTimer.IsActive(), emergencyDiveTimer.CanStart(), !m_bomber.IsCorkscrewPossible(), emergencyDiveTimer.GetRechargeNormalised(), emergencyDiveTimer.GetInUseNormalised());
		emergencyDiveTimer = m_bomber.GetEvadeFlakTimer();
		m_flakEvadeButton.SetStatus(emergencyDiveTimer.IsActive(), emergencyDiveTimer.CanStart(), !m_bomber.IsEvadeFlakPossible(), emergencyDiveTimer.GetRechargeNormalised(), emergencyDiveTimer.GetInUseNormalised());
		m_emergencyLandToggle.SetState(m_bomberSystems.GetBomberState().IsDoingEmergencyLand());
		m_emergencyLandToggle.SetDisabled(!m_bomberSystems.GetBomberState().CanToggleEmergencyLand());
		m_bailOutOrderToggle.SetState(m_bomberSystems.GetBomberState().IsBailOutOrderActive());
		m_abortMissionToggle.SetState(!Singleton<MissionCoordinator>.Instance.IsOutwardJourney());
		m_abortMissionToggle.SetDisabled(!Singleton<MissionCoordinator>.Instance.CanToggleReturnJourney());
		if (m_isHydraulicsAlertActive != m_bomberSystems.GetHydraulics().IsBroken())
		{
			m_isHydraulicsAlertActive = m_bomberSystems.GetHydraulics().IsBroken();
			m_systemAlertHydraulics.SetActive(m_isHydraulicsAlertActive);
		}
		m_lockToggleButton.SetState(GetStation().IsLockedInStation());
		bool flag = m_bomber.ShouldEnableAllControls();
		if (flag != m_areAllControlsActive)
		{
			m_areAllControlsActive = flag;
			m_enableWhenTakenOff.SetActive(flag);
			m_disableWhenTakenOff.SetActive(!flag);
		}
	}

	private void ToggleLock()
	{
		GetStation().SetLockedInStation(!GetStation().IsLockedInStation());
		m_lockToggleButton.SetState(GetStation().IsLockedInStation());
	}

	public override void FlashLock()
	{
		if (GetStation().IsLockedInStation() && m_lockFlashAnimation != null)
		{
			WingroveRoot.Instance.PostEvent("BAD_CLICK");
			m_lockFlashAnimation.Play();
		}
	}

	private void AltitudeIndexClick(int index)
	{
		m_bomber.SetAltitudeTarget(index);
	}

	private void LandingGearClick(int index)
	{
		if (index == 0)
		{
			m_bomberSystems.GetLandingGear().Lower();
		}
		else
		{
			m_bomberSystems.GetLandingGear().Raise();
		}
	}
}
