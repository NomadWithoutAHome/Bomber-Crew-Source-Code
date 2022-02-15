using dbox;
using UnityEngine;
using WingroveAudio;

public class BombBayDoors : DamagePassthroughModifier
{
	public enum DoorState
	{
		Closed,
		Opening,
		Open,
		Closing,
		Removed
	}

	[SerializeField]
	private float m_openingDuration = 5f;

	[SerializeField]
	private float m_closingDuration = 5f;

	[SerializeField]
	private Animation m_animation;

	[SerializeField]
	private HydraulicsTank m_hydraulicsTank;

	[SerializeField]
	private BombBayDoorsManualWinch m_manualWinch;

	[SerializeField]
	private AeroSurface[] m_fuelEffiencySettings;

	[SerializeField]
	private GameObject m_toDestroyOnRemove;

	[SerializeField]
	private BomberSystemUniqueId m_upgradeSystem;

	private DoorState m_currentDoorState;

	private float m_openness;

	private float m_power = 1f;

	private bool m_requiresDboxEvent;

	private void Start()
	{
		EquipmentUpgradeFittableBase upgrade = m_upgradeSystem.GetUpgrade();
		HydraulicSystemUpgrade hydraulicSystemUpgrade = (HydraulicSystemUpgrade)upgrade;
		m_power = hydraulicSystemUpgrade.GetSpeedMultiplier();
	}

	private void Update()
	{
		if (m_animation != null)
		{
			string animation = m_animation.clip.name;
			m_animation.Play(animation);
			m_animation[animation].enabled = true;
			m_animation[animation].speed = 0f;
			m_animation[animation].normalizedTime = m_openness;
			float num = ((!m_hydraulicsTank.IsBroken()) ? 1f : ((!(m_manualWinch != null) || !m_manualWinch.IsWinching()) ? 0f : 0.3f));
			num *= Mathf.Max(m_power, 1f);
			switch (m_currentDoorState)
			{
			case DoorState.Opening:
				m_openness += Time.deltaTime * (1f / m_openingDuration) * num;
				if (m_openness >= 1f)
				{
					m_openness = 1f;
					WingroveRoot.Instance.PostEventGO("HYDRAULICS_STOP", base.gameObject);
					m_currentDoorState = DoorState.Open;
				}
				if (m_requiresDboxEvent && num > 0f)
				{
					DboxInMissionController.DBoxCall(DboxSdkWrapper.PostOpenBayDoors);
					m_requiresDboxEvent = false;
				}
				break;
			case DoorState.Closing:
				m_openness -= Time.deltaTime * (1f / m_openingDuration) * num;
				if (m_openness <= 0f)
				{
					m_openness = 0f;
					WingroveRoot.Instance.PostEventGO("HYDRAULICS_STOP", base.gameObject);
					m_currentDoorState = DoorState.Closed;
				}
				if (m_requiresDboxEvent && num > 0f)
				{
					DboxInMissionController.DBoxCall(DboxSdkWrapper.PostCloseBayDoors);
					m_requiresDboxEvent = false;
				}
				break;
			}
		}
		AeroSurface[] fuelEffiencySettings = m_fuelEffiencySettings;
		foreach (AeroSurface aeroSurface in fuelEffiencySettings)
		{
			if (aeroSurface != null)
			{
				if (m_currentDoorState == DoorState.Removed)
				{
					aeroSurface.SetHidden(1f);
				}
				else
				{
					aeroSurface.SetHidden(1f - m_openness);
				}
			}
		}
	}

	public float GetWinchProgress()
	{
		if (RequiresManualWinch())
		{
			if (m_currentDoorState == DoorState.Closing)
			{
				return 1f - m_openness;
			}
			if (m_currentDoorState == DoorState.Opening)
			{
				return m_openness;
			}
			return 1f;
		}
		return 1f;
	}

	public bool RequiresManualWinch()
	{
		return (m_currentDoorState == DoorState.Opening || m_currentDoorState == DoorState.Closing) && m_hydraulicsTank.IsBroken();
	}

	public bool IsAffectedByHydraulics()
	{
		if (m_hydraulicsTank.IsBroken() && (m_currentDoorState == DoorState.Opening || m_currentDoorState == DoorState.Closing))
		{
			return true;
		}
		return false;
	}

	public void Close()
	{
		if (m_currentDoorState == DoorState.Opening || m_currentDoorState == DoorState.Open)
		{
			m_requiresDboxEvent = true;
			Switch();
		}
	}

	public void Open()
	{
		if (m_currentDoorState == DoorState.Closing || m_currentDoorState == DoorState.Closed)
		{
			m_requiresDboxEvent = true;
			Switch();
		}
	}

	public void RemoveDoors()
	{
		if (m_currentDoorState == DoorState.Closing || m_currentDoorState == DoorState.Opening)
		{
			WingroveRoot.Instance.PostEventGO("HYDRAULICS_STOP", base.gameObject);
		}
		m_currentDoorState = DoorState.Removed;
		m_openness = 1f;
		if (m_toDestroyOnRemove != null)
		{
			m_toDestroyOnRemove.SetActive(value: false);
		}
	}

	public void DontRemoveDoors()
	{
		if (m_currentDoorState == DoorState.Removed)
		{
			m_currentDoorState = DoorState.Closed;
			m_openness = 0f;
			if (m_toDestroyOnRemove != null)
			{
				m_toDestroyOnRemove.SetActive(value: true);
			}
		}
	}

	public void Switch()
	{
		if (m_currentDoorState == DoorState.Removed)
		{
			return;
		}
		if (m_currentDoorState == DoorState.Opening || m_currentDoorState == DoorState.Open)
		{
			if (m_currentDoorState == DoorState.Open)
			{
				WingroveRoot.Instance.PostEventGO("HYDRAULICS_START", base.gameObject);
			}
			m_currentDoorState = DoorState.Closing;
		}
		else
		{
			if (m_currentDoorState == DoorState.Closed)
			{
				WingroveRoot.Instance.PostEventGO("HYDRAULICS_START", base.gameObject);
			}
			m_currentDoorState = DoorState.Opening;
		}
	}

	public override float GetDamagePassthroughModifier()
	{
		return m_openness;
	}

	public bool IsFullyOpen()
	{
		return m_currentDoorState == DoorState.Open || m_currentDoorState == DoorState.Removed;
	}

	public bool IsOpening()
	{
		return m_currentDoorState == DoorState.Opening;
	}

	public bool IsFullyClosed()
	{
		return m_currentDoorState == DoorState.Closed;
	}

	public DoorState GetState()
	{
		return m_currentDoorState;
	}
}
