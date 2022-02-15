using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class BombAimerStationPanel : StationPanel
{
	[SerializeField]
	private tk2dUIItem m_bombReleaseButton;

	[SerializeField]
	private PanelToggleButton m_bombReleaseToggleButton;

	[SerializeField]
	private PanelMultiSelect m_bombBayMultiSelect;

	[SerializeField]
	private PanelCooldownButton m_takePhotoButton;

	[SerializeField]
	private GameObject m_defaultBombLoadPrefab;

	[SerializeField]
	private GameObject m_systemAlertHydraulics;

	[SerializeField]
	private Transform m_selectorSwitchesRoot;

	[SerializeField]
	private CrewmanSkillAbilityBool m_takePhotoSkill;

	[SerializeField]
	private TextSetter m_visibilityTextSetter;

	[SerializeField]
	[NamedText]
	private string m_visibilityFormatString;

	[SerializeField]
	private GameObject m_visibilityGood;

	[SerializeField]
	private GameObject m_visibilityMedium;

	[SerializeField]
	private GameObject m_visibilityPoor;

	[SerializeField]
	private GameObject m_doorsHierarchy;

	[SerializeField]
	private GameObject m_bombSelectHierarchy;

	[SerializeField]
	private UISelectorPointingHint[] m_pointAtSelectAll;

	[SerializeField]
	private UISelectorPointingHint[] m_pointAtSelectAllL;

	[SerializeField]
	private tk2dUIItem m_leftFromSelectAll;

	[SerializeField]
	private tk2dUIItem m_rightFromSelectAll;

	private PanelToggleButton m_selectAllBombsToggleButton;

	private BomberSystems m_bomberSystems;

	private BomberState m_bomber;

	private List<PanelToggleButton> m_bombArmButtons = new List<PanelToggleButton>();

	private bool m_unarmedBombsAvailable;

	private bool m_allBombsDropped = true;

	private GameObject m_currentBombSwitches;

	private void OnEnable()
	{
		Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBombBay().OnBombLoadTypeChange += ChangeBombSwitches;
	}

	private void OnDisable()
	{
		if (Singleton<BomberSpawn>.Instance != null)
		{
			Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBombBay().OnBombLoadTypeChange -= ChangeBombSwitches;
		}
	}

	private void ChangeBombSwitches()
	{
		if (m_currentBombSwitches != null)
		{
			Object.DestroyImmediate(m_currentBombSwitches);
		}
		CampaignStructure.CampaignMission currentlySelectedMissionDetails = Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetCurrentlySelectedMissionDetails();
		GameObject gameObject = ((!(currentlySelectedMissionDetails.m_bombLoadConfig == null)) ? currentlySelectedMissionDetails.m_bombLoadConfig.GetUiSelectorSwitchesPrefab() : null);
		if (gameObject == null)
		{
			gameObject = m_defaultBombLoadPrefab;
		}
		GameObject gameObject2 = Object.Instantiate(gameObject);
		BombAimerSelectorSwitches component = gameObject2.GetComponent<BombAimerSelectorSwitches>();
		component.transform.parent = m_selectorSwitchesRoot;
		component.transform.localPosition = Vector3.zero;
		m_currentBombSwitches = gameObject2;
		m_selectAllBombsToggleButton = component.GetSelectAllToggleButton();
		if (m_selectAllBombsToggleButton != null)
		{
			m_selectAllBombsToggleButton.OnClick += OnSelectAllBombsClick;
			UISelectorPointingHint[] pointAtSelectAll = m_pointAtSelectAll;
			foreach (UISelectorPointingHint uISelectorPointingHint in pointAtSelectAll)
			{
				uISelectorPointingHint.SetRightLink(m_selectAllBombsToggleButton.GetUIItem());
			}
			UISelectorPointingHint[] pointAtSelectAllL = m_pointAtSelectAllL;
			foreach (UISelectorPointingHint uISelectorPointingHint2 in pointAtSelectAllL)
			{
				uISelectorPointingHint2.SetLeftLink(m_selectAllBombsToggleButton.GetUIItem());
			}
			m_selectAllBombsToggleButton.GetUIItem().GetComponent<UISelectorPointingHint>().SetLeftLink(m_leftFromSelectAll);
			m_selectAllBombsToggleButton.GetUIItem().GetComponent<UISelectorPointingHint>().SetRightLink(m_rightFromSelectAll);
		}
		List<BombLoad.BombRackState> bombRackStates = m_bomberSystems.GetBombLoad().GetBombRackStates();
		int num = 0;
		foreach (BombLoad.BombRackState item in bombRackStates)
		{
			PanelToggleButton selectorSwitch = component.GetSelectorSwitch(num);
			int currIndex = num;
			selectorSwitch.OnClick += delegate
			{
				m_bomberSystems.GetBombLoad().ToggleArm(currIndex);
				Refresh();
			};
			m_bombArmButtons.Add(selectorSwitch);
			num++;
		}
		if (m_bomberSystems.GetBombLoad().ShouldRemoveBayDoors() || m_bomberSystems.GetBombLoad().IgnoresBombBayDoors())
		{
			m_doorsHierarchy.SetActive(value: false);
			m_bombSelectHierarchy.transform.localPosition = new Vector3(0f, m_bombSelectHierarchy.transform.localPosition.y, m_bombSelectHierarchy.transform.localPosition.z);
		}
		else
		{
			m_doorsHierarchy.SetActive(value: true);
			m_bombSelectHierarchy.transform.localPosition = new Vector3(106f, m_bombSelectHierarchy.transform.localPosition.y, m_bombSelectHierarchy.transform.localPosition.z);
		}
	}

	protected override void SetUpStation()
	{
		m_bomberSystems = Singleton<BomberSpawn>.Instance.GetBomberSystems();
		m_bombBayMultiSelect.OnSelectIndex += BombBayDoorClick;
		m_bomber = m_bomberSystems.GetBomberState();
		m_bombReleaseButton.OnClick += m_bombReleaseButton_OnClick;
		m_takePhotoButton.OnClick += OnPhotoClick;
		ChangeBombSwitches();
		Refresh();
	}

	private void OnPhotoClick()
	{
		StationBombAimer stationBombAimer = (StationBombAimer)GetStation();
		SkillRefreshTimer.SkillTimer photoTimer = stationBombAimer.GetPhotoTimer();
		if (photoTimer.CanStart())
		{
			m_bomberSystems.GetPhotoCamera().TakePhoto(GetStation().GetCurrentCrewman());
			photoTimer.Start(m_takePhotoSkill.GetDurationTimer(), m_takePhotoSkill.GetCooldownTimer());
		}
	}

	private void m_bombReleaseButton_OnClick()
	{
		if (m_bomberSystems.GetBombBayDoors().IsFullyOpen() || m_bomberSystems.GetBombLoad().IgnoresBombBayDoors())
		{
			m_bomberSystems.GetBombLoad().DropBombs();
		}
	}

	private void OnSelectAllBombsClick()
	{
		List<BombLoad.BombRackState> bombRackStates = m_bomberSystems.GetBombLoad().GetBombRackStates();
		int num = 0;
		for (int i = 0; i < bombRackStates.Count; i++)
		{
			if (m_unarmedBombsAvailable)
			{
				if (bombRackStates[i] == BombLoad.BombRackState.Unarmed)
				{
					m_bomberSystems.GetBombLoad().ToggleArm(num);
				}
			}
			else if (bombRackStates[i] == BombLoad.BombRackState.Armed)
			{
				m_bomberSystems.GetBombLoad().ToggleArm(num);
			}
			num++;
		}
	}

	private void BombBayDoorClick(int index)
	{
		if (index == 0)
		{
			m_bomberSystems.GetBombBayDoors().Close();
		}
		else
		{
			m_bomberSystems.GetBombBayDoors().Open();
		}
	}

	private void Refresh()
	{
		switch (m_bomberSystems.GetBombBayDoors().GetState())
		{
		case BombBayDoors.DoorState.Closed:
			m_bombBayMultiSelect.SetSelected(0);
			m_bombBayMultiSelect.SetInProgress(inProgress: false);
			break;
		case BombBayDoors.DoorState.Open:
			m_bombBayMultiSelect.SetSelected(1);
			m_bombBayMultiSelect.SetInProgress(inProgress: false);
			break;
		case BombBayDoors.DoorState.Closing:
			m_bombBayMultiSelect.SetSelected(0);
			m_bombBayMultiSelect.SetInProgress(inProgress: true);
			break;
		case BombBayDoors.DoorState.Opening:
			m_bombBayMultiSelect.SetSelected(1);
			m_bombBayMultiSelect.SetInProgress(inProgress: true);
			break;
		}
		List<BombLoad.BombRackState> bombRackStates = m_bomberSystems.GetBombLoad().GetBombRackStates();
		bool flag = false;
		m_unarmedBombsAvailable = false;
		m_allBombsDropped = true;
		int num = 0;
		for (int i = 0; i < bombRackStates.Count; i++)
		{
			switch (bombRackStates[i])
			{
			case BombLoad.BombRackState.Armed:
				m_bombArmButtons[num].SetState(on: true);
				m_bombArmButtons[num].SetDisabled(disabled: false);
				flag = true;
				m_allBombsDropped = false;
				break;
			case BombLoad.BombRackState.Unarmed:
				m_bombArmButtons[num].SetState(on: false);
				m_bombArmButtons[num].SetDisabled(disabled: false);
				m_unarmedBombsAvailable = true;
				m_allBombsDropped = false;
				break;
			case BombLoad.BombRackState.Dropped:
				m_bombArmButtons[num].SetDisabled(disabled: true);
				break;
			}
			num++;
		}
		m_bombReleaseToggleButton.SetState(flag && (m_bomberSystems.GetBombBayDoors().IsFullyOpen() || m_bomberSystems.GetBombLoad().IgnoresBombBayDoors()));
		if (m_selectAllBombsToggleButton != null)
		{
			if (m_allBombsDropped)
			{
				m_selectAllBombsToggleButton.SetDisabled(disabled: true);
			}
			else
			{
				m_selectAllBombsToggleButton.SetState(!m_unarmedBombsAvailable);
				m_selectAllBombsToggleButton.SetDisabled(disabled: false);
			}
		}
		StationBombAimer stationBombAimer = (StationBombAimer)GetStation();
		SkillRefreshTimer.SkillTimer photoTimer = stationBombAimer.GetPhotoTimer();
		m_takePhotoButton.SetStatus(photoTimer.IsActive(), photoTimer.CanStart(), isDisabled: false, photoTimer.GetRechargeNormalised(), photoTimer.GetProgressNormalised());
		float currentPhotoVisibility = m_bomberSystems.GetPhotoCamera().GetCurrentPhotoVisibility(GetStation().GetCurrentCrewman());
		if (currentPhotoVisibility > 0.8f)
		{
			m_visibilityGood.SetActive(value: true);
			m_visibilityMedium.SetActive(value: false);
			m_visibilityPoor.SetActive(value: false);
		}
		else if (currentPhotoVisibility > 0.5f)
		{
			m_visibilityGood.SetActive(value: false);
			m_visibilityMedium.SetActive(value: true);
			m_visibilityPoor.SetActive(value: false);
		}
		else
		{
			m_visibilityGood.SetActive(value: false);
			m_visibilityMedium.SetActive(value: false);
			m_visibilityPoor.SetActive(value: true);
		}
		m_visibilityTextSetter.SetText(string.Format(Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_visibilityFormatString), Mathf.RoundToInt(currentPhotoVisibility * 100f)));
		m_systemAlertHydraulics.SetActive(m_bomberSystems.GetHydraulics().IsBroken());
	}

	private void Update()
	{
		Refresh();
	}
}
