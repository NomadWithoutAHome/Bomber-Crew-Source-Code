using BomberCrewCommon;
using UnityEngine;

public class EngineerStationPanel : StationPanel
{
	[SerializeField]
	private PanelMultiSelect m_fuelMixMultiSelect;

	[SerializeField]
	private PanelToggleButton[] m_fireExtinguisherButtons;

	[SerializeField]
	private GameObject m_extinguishersNotInstalled;

	[SerializeField]
	private GameObject m_extinguishersInstalled;

	[SerializeField]
	private GameObject[] m_lightFlashingWarning;

	[SerializeField]
	private PanelToggleButton m_transferLtoRButton;

	[SerializeField]
	private PanelToggleButton m_transferRtoLButton;

	[SerializeField]
	private PanelCooldownButton m_richBoostButton;

	[SerializeField]
	private PanelCooldownButton m_leanBoostButton;

	[SerializeField]
	private float m_fuelTransferRate = 1000f;

	[SerializeField]
	private TextSetter m_notepadSetter;

	[SerializeField]
	[NamedText]
	private string m_fuelRemainingText;

	[SerializeField]
	private TextSetter m_extinguishesLeftText;

	[SerializeField]
	[NamedText]
	private string m_extinguishesLeftStringFormat;

	private Camera m_stationCamera;

	private BomberSystems m_bomberSystems;

	private BomberState m_bomber;

	private bool m_transferLtoR;

	private bool m_transferRtoL;

	private int m_fuelUsageTimer;

	private bool m_hasExtinguisher;

	private StationFlightEngineer m_stationFlightEngineer;

	protected override void SetUpStation()
	{
		m_bomberSystems = Singleton<BomberSpawn>.Instance.GetBomberSystems();
		m_bomber = m_bomberSystems.GetBomberState();
		m_fuelMixMultiSelect.OnSelectIndex += OnFuelMixChange;
		m_leanBoostButton.OnClick += FuelBoostLean;
		m_richBoostButton.OnClick += FuelBoostRich;
		for (int i = 0; i < 4; i++)
		{
			int v = i;
			m_fireExtinguisherButtons[i].OnClick += delegate
			{
				ToggleExtinguisher(v);
			};
		}
		m_stationFlightEngineer = (StationFlightEngineer)GetStation();
		m_transferLtoRButton.OnClick += TransferLtoRButton_OnClick;
		m_transferRtoLButton.OnClick += TransferRtoLButton_OnClick;
		Refresh();
	}

	private void ToggleExtinguisher(int index)
	{
		if (m_stationFlightEngineer.GetExtinguishesLeft() > 0)
		{
			EngineExtinguisher engineExtinguisher = m_bomberSystems.GetEngineExtinguisher(index);
			if (!engineExtinguisher.IsEnabled())
			{
				engineExtinguisher.SetEnabled(enabled: true);
				m_stationFlightEngineer.UseExtinguish();
			}
		}
	}

	private void TransferLtoRButton_OnClick()
	{
		m_transferLtoR = !m_transferLtoR;
		m_transferRtoL = false;
	}

	private void TransferRtoLButton_OnClick()
	{
		m_transferRtoL = !m_transferRtoL;
		m_transferLtoR = false;
	}

	private void Refresh()
	{
		StationFlightEngineer stationFlightEngineer = (StationFlightEngineer)GetStation();
		int currentFuelMix = stationFlightEngineer.GetCurrentFuelMix();
		m_fuelMixMultiSelect.SetSelected(currentFuelMix - 1);
		SkillRefreshTimer.SkillTimer leanBoostInfo = stationFlightEngineer.GetLeanBoostInfo();
		SkillRefreshTimer.SkillTimer richBoostInfo = stationFlightEngineer.GetRichBoostInfo();
		m_leanBoostButton.SetStatus(leanBoostInfo.IsActive(), leanBoostInfo.CanStart(), isDisabled: false, leanBoostInfo.GetRechargeNormalised(), leanBoostInfo.GetInUseNormalised());
		m_richBoostButton.SetStatus(richBoostInfo.IsActive(), richBoostInfo.CanStart(), isDisabled: false, richBoostInfo.GetRechargeNormalised(), richBoostInfo.GetInUseNormalised());
		m_transferLtoRButton.SetState(m_transferLtoR);
		m_transferRtoLButton.SetState(m_transferRtoL);
		bool flag = m_bomberSystems.GetEngineExtinguisher(0).IsInstalled();
		m_extinguishersInstalled.SetActive(flag);
		m_extinguishersNotInstalled.SetActive(!flag);
		if (m_fuelUsageTimer == 0)
		{
			float fuelUsageAverage = m_bomber.GetPhysicsModel().GetFuelUsageAverage();
			if (fuelUsageAverage > 0f && !m_bomber.HasNotCompletedTakeOff())
			{
				float num = m_bomberSystems.GetFuelTank(0).GetFuel() + m_bomberSystems.GetFuelTank(1).GetFuel();
				if (num >= 0f)
				{
					float num2 = num / fuelUsageAverage;
					int num3 = Mathf.FloorToInt(num2 / 60f);
					int num4 = (int)num2 - num3 * 60;
					if (num3 < 60)
					{
						m_notepadSetter.SetText(string.Format(Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_fuelRemainingText), num3, num4));
					}
					else
					{
						m_notepadSetter.SetText(string.Empty);
					}
				}
				else
				{
					m_notepadSetter.SetText(string.Empty);
				}
			}
			else
			{
				m_notepadSetter.SetText(string.Empty);
			}
		}
		m_fuelUsageTimer = (m_fuelUsageTimer + 1) % 5;
		if (!flag)
		{
			return;
		}
		m_extinguishesLeftText.SetText(string.Format(Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_extinguishesLeftStringFormat), m_stationFlightEngineer.GetExtinguishesLeft()));
		int engineCount = m_bomberSystems.GetEngineCount();
		for (int i = 0; i < engineCount; i++)
		{
			Engine engineOrdered = m_bomberSystems.GetEngineOrdered(i);
			if (engineOrdered == null || engineOrdered.IsDestroyed())
			{
				m_lightFlashingWarning[i].SetActive(value: false);
			}
			else if (engineOrdered.IsOnFire())
			{
				m_lightFlashingWarning[i].SetActive(value: true);
			}
			else
			{
				m_lightFlashingWarning[i].SetActive(value: false);
			}
			m_fireExtinguisherButtons[i].SetState(m_bomberSystems.GetEngineExtinguisher(i).IsEnabled());
			m_fireExtinguisherButtons[i].SetDisabled(!m_bomberSystems.GetEngineExtinguisher(i).CanBeActivated() || m_stationFlightEngineer.GetExtinguishesLeft() == 0);
		}
	}

	private void Update()
	{
		Refresh();
		if (m_transferLtoR || m_transferRtoL)
		{
			int index = 0;
			int index2 = 1;
			if (m_transferRtoL)
			{
				index = 1;
				index2 = 0;
			}
			if (m_bomberSystems.GetFuelTank(index).GetFuelNormalised() > 0f && m_bomberSystems.GetFuelTank(index2).GetFuelNormalised() < 1f)
			{
				float num = m_fuelTransferRate * Time.deltaTime;
				float num2 = m_bomberSystems.GetFuelTank(index).UseFuel(num);
				float amt = m_bomberSystems.GetFuelTank(index2).PumpFuelIn(num - num2);
				m_bomberSystems.GetFuelTank(index).PumpFuelIn(amt);
			}
		}
	}

	private void OnFuelMixChange(int obj)
	{
		StationFlightEngineer stationFlightEngineer = (StationFlightEngineer)GetStation();
		if (obj == 0)
		{
			stationFlightEngineer.SetFuelMixCorrect();
		}
		else
		{
			stationFlightEngineer.SetFuelMixRich();
		}
	}

	private void FuelBoostLean()
	{
		StationFlightEngineer stationFlightEngineer = (StationFlightEngineer)GetStation();
		stationFlightEngineer.DoLeanBoost();
	}

	private void FuelBoostRich()
	{
		StationFlightEngineer stationFlightEngineer = (StationFlightEngineer)GetStation();
		stationFlightEngineer.DoRichBoost();
	}
}
