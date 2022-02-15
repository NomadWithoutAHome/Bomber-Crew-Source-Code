using BomberCrewCommon;
using UnityEngine;

public class RadioOpStationPanel : StationPanel
{
	[SerializeField]
	private PanelCooldownButton m_getIntelButton;

	[SerializeField]
	private PanelCooldownButton m_autoTagButton;

	[SerializeField]
	private PanelCooldownButton m_spitfireSupportButton;

	[SerializeField]
	private GameObject m_systemAlertElectrics;

	private BomberSystems m_bomberSystems;

	private ElectricalSystem m_electricalSystem;

	protected override void SetUpStation()
	{
		m_bomberSystems = Singleton<BomberSpawn>.Instance.GetBomberSystems();
		m_electricalSystem = m_bomberSystems.GetElectricalSystem();
		m_getIntelButton.OnClick += GetIntel;
		m_autoTagButton.OnClick += AutoTag;
		m_spitfireSupportButton.OnClick += SpitfireSupport;
		Refresh();
	}

	private void Update()
	{
		Refresh();
	}

	private void GetIntel()
	{
		StationRadioOperator stationRadioOperator = (StationRadioOperator)GetStation();
		stationRadioOperator.GetIntel();
	}

	private void AutoTag()
	{
		StationRadioOperator stationRadioOperator = (StationRadioOperator)GetStation();
		stationRadioOperator.AutoTag();
	}

	private void SpitfireSupport()
	{
		StationRadioOperator stationRadioOperator = (StationRadioOperator)GetStation();
		stationRadioOperator.DoSpitfires();
	}

	private void Refresh()
	{
		StationRadioOperator stationRadioOperator = (StationRadioOperator)GetStation();
		m_autoTagButton.SetStatus(stationRadioOperator.GetAutoTagTimer().IsActive(), stationRadioOperator.GetAutoTagTimer().CanStart(), m_electricalSystem.IsBroken(), stationRadioOperator.GetAutoTagTimer().GetRechargeNormalised(), stationRadioOperator.GetAutoTagTimer().GetInUseNormalised());
		m_getIntelButton.SetStatus(stationRadioOperator.GetIntelTimer().IsActive(), stationRadioOperator.GetIntelTimer().CanStart(), m_electricalSystem.IsBroken(), stationRadioOperator.GetIntelTimer().GetRechargeNormalised(), stationRadioOperator.GetIntelTimer().GetInUseNormalised());
		m_spitfireSupportButton.SetStatus(stationRadioOperator.GetSpitfireTimer().IsActive(), stationRadioOperator.GetSpitfireTimer().CanStart(), m_electricalSystem.IsBroken(), stationRadioOperator.GetSpitfireTimer().GetRechargeNormalised(), stationRadioOperator.GetSpitfireTimer().GetInUseNormalised());
		m_systemAlertElectrics.SetActive(m_bomberSystems.GetElectricalSystem().IsBroken());
	}
}
