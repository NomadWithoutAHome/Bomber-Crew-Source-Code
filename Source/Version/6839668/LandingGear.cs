using BomberCrewCommon;
using dbox;
using UnityEngine;
using WingroveAudio;

public class LandingGear : MonoBehaviour
{
	public enum GearState
	{
		Raised,
		Lowering,
		Lowered,
		Raising,
		Disabled
	}

	[SerializeField]
	private float m_loweringDuration = 5f;

	[SerializeField]
	private float m_raisingDuration = 5f;

	[SerializeField]
	private HydraulicsTank m_hydraulicsTank;

	[SerializeField]
	private LandingGearManualWinch m_manualWinch;

	[SerializeField]
	private AeroSurface[] m_fuelEffiencySettings;

	[SerializeField]
	private BomberSystemUniqueId m_upgradeSystem;

	private GearState m_currentDoorState;

	private float m_openness;

	private float m_power = 1f;

	private bool m_shouldDoDboxEvent;

	private BomberSystems m_bomberSystems;

	private void Awake()
	{
		m_openness = 1f;
		m_currentDoorState = GearState.Lowered;
	}

	private void Start()
	{
		m_bomberSystems = Singleton<BomberSpawn>.Instance.GetBomberSystems();
		EquipmentUpgradeFittableBase upgrade = m_upgradeSystem.GetUpgrade();
		HydraulicSystemUpgrade hydraulicSystemUpgrade = (HydraulicSystemUpgrade)upgrade;
		m_power = hydraulicSystemUpgrade.GetSpeedMultiplier();
	}

	private void Update()
	{
		LandingGearSingle[] individualLandingGears = m_bomberSystems.GetIndividualLandingGears();
		LandingGearSingle[] array = individualLandingGears;
		foreach (LandingGearSingle landingGearSingle in array)
		{
			if (landingGearSingle != null)
			{
				landingGearSingle.UpdateFromState(m_openness);
			}
		}
		float num = ((!m_hydraulicsTank.IsBroken()) ? 1f : ((!(m_manualWinch != null) || !m_manualWinch.IsWinching()) ? 0f : 0.3f));
		num *= Mathf.Max(m_power, 1f);
		switch (m_currentDoorState)
		{
		case GearState.Lowering:
			m_openness += Time.deltaTime * (1f / m_loweringDuration) * num;
			if (m_openness >= 1f)
			{
				WingroveRoot.Instance.PostEventGO("HYDRAULICS_STOP", base.gameObject);
				m_openness = 1f;
				m_currentDoorState = GearState.Lowered;
			}
			if (m_shouldDoDboxEvent && num > 0f)
			{
				DboxInMissionController.DBoxCall(DboxSdkWrapper.PostLowerGear);
				m_shouldDoDboxEvent = false;
			}
			break;
		case GearState.Raising:
			m_openness -= Time.deltaTime * (1f / m_raisingDuration) * num;
			if (m_openness <= 0f)
			{
				WingroveRoot.Instance.PostEventGO("HYDRAULICS_STOP", base.gameObject);
				m_openness = 0f;
				m_currentDoorState = GearState.Raised;
			}
			if (m_shouldDoDboxEvent && num > 0f)
			{
				DboxInMissionController.DBoxCall(DboxSdkWrapper.PostRaiseGear);
				m_shouldDoDboxEvent = false;
			}
			break;
		}
		AeroSurface[] fuelEffiencySettings = m_fuelEffiencySettings;
		foreach (AeroSurface aeroSurface in fuelEffiencySettings)
		{
			aeroSurface.SetHidden(1f - m_openness);
		}
	}

	public float GetWinchProgress()
	{
		if (RequiresManualWinch())
		{
			if (m_currentDoorState == GearState.Raising)
			{
				return 1f - m_openness;
			}
			if (m_currentDoorState == GearState.Lowering)
			{
				return m_openness;
			}
			return 1f;
		}
		return 1f;
	}

	public bool RequiresManualWinch()
	{
		return (m_currentDoorState == GearState.Lowering || m_currentDoorState == GearState.Raising) && m_hydraulicsTank.IsBroken();
	}

	public GearState GetState()
	{
		return m_currentDoorState;
	}

	public void Raise()
	{
		if (m_currentDoorState == GearState.Lowering || m_currentDoorState == GearState.Lowered)
		{
			m_shouldDoDboxEvent = true;
			Switch();
		}
	}

	public void Lower()
	{
		if (m_currentDoorState == GearState.Raised || m_currentDoorState == GearState.Raising)
		{
			m_shouldDoDboxEvent = true;
			Switch();
		}
	}

	public void Switch()
	{
		if (m_currentDoorState == GearState.Disabled)
		{
			return;
		}
		if (m_currentDoorState == GearState.Lowering || m_currentDoorState == GearState.Lowered)
		{
			if (m_currentDoorState == GearState.Lowered)
			{
				WingroveRoot.Instance.PostEventGO("HYDRAULICS_START", base.gameObject);
			}
			m_currentDoorState = GearState.Raising;
		}
		else
		{
			if (m_currentDoorState == GearState.Raised)
			{
				WingroveRoot.Instance.PostEventGO("HYDRAULICS_START", base.gameObject);
			}
			m_currentDoorState = GearState.Lowering;
		}
	}

	public void ForceUp()
	{
		m_openness = 0f;
		m_currentDoorState = GearState.Raised;
	}

	public bool IsAffectedByHydraulics()
	{
		if (m_hydraulicsTank.IsBroken() && (m_currentDoorState == GearState.Lowering || m_currentDoorState == GearState.Raising))
		{
			return true;
		}
		return false;
	}

	public void Disable()
	{
		m_currentDoorState = GearState.Disabled;
	}

	public bool IsFullyRaised()
	{
		return m_currentDoorState == GearState.Raised || m_currentDoorState == GearState.Disabled;
	}

	public bool IsFullyLowered()
	{
		return m_currentDoorState == GearState.Lowered;
	}

	public bool IsLowering()
	{
		return m_currentDoorState == GearState.Lowering;
	}

	public bool IsRaising()
	{
		return m_currentDoorState == GearState.Raising;
	}
}
