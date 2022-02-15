using System;
using UnityEngine;

public class Station : InteractiveItem
{
	public enum StationAction
	{
		Sit,
		Prone,
		Bed,
		StandAtStation,
		FoetalPosition
	}

	[SerializeField]
	private GameObject m_controlPanelUI;

	[SerializeField]
	private Camera[] m_stationCameras;

	[SerializeField]
	private string m_stationUISprite;

	[SerializeField]
	private Transform m_crewmanSeatTransform;

	[SerializeField]
	private StationAction m_stationAction;

	[SerializeField]
	protected EnumToIconMapping.InteractionOrAlertType m_interactionIcon;

	[SerializeField]
	private Transform m_stationTrackingTransform;

	[SerializeField]
	private InMissionNotificationProvider m_notificationProvider;

	protected bool m_locked;

	private CrewmanAvatar m_currentCrewman;

	private bool m_crewmanActivated;

	private bool m_isLockedInStation;

	private bool m_hasCrewman;

	protected Interaction m_sitInteraction;

	public event Action<bool> OnControlsStateChange;

	public void SetLocked(bool locked)
	{
		m_locked = locked;
	}

	public void SetOverridePanelUI(GameObject go)
	{
		m_controlPanelUI = go;
	}

	public Transform GetSeatedTransform()
	{
		if (m_crewmanSeatTransform == null)
		{
			return base.transform;
		}
		return m_crewmanSeatTransform;
	}

	public InMissionNotificationProvider GetNotifications()
	{
		return m_notificationProvider;
	}

	public StationAction GetStationAnimationAction()
	{
		return m_stationAction;
	}

	public Camera[] GetStationCameras()
	{
		return m_stationCameras;
	}

	public string GetUISpriteName()
	{
		return m_stationUISprite;
	}

	public GameObject GetUIPanelPrefab()
	{
		return m_controlPanelUI;
	}

	public override void Start()
	{
		base.Start();
		BomberDestroyableSection bomberDestroyableSection = BomberDestroyableSection.FindDestroyableSectionFor(base.transform);
		if (bomberDestroyableSection != null)
		{
			bomberDestroyableSection.OnSectionDestroy += SectionDestroy;
		}
	}

	private void SectionDestroy()
	{
		if (m_currentCrewman != null)
		{
			m_currentCrewman.SetStation(null);
		}
	}

	public override Interaction GetInteractionOptions(CrewmanAvatar crewman)
	{
		if (m_sitInteraction == null)
		{
			m_sitInteraction = new Interaction("Sit", queryProgress: false, m_interactionIcon);
		}
		if (m_currentCrewman == null && !m_locked)
		{
			return m_sitInteraction;
		}
		return null;
	}

	protected override void FinishInteraction(Interaction interaction, CrewmanAvatar crewman)
	{
		SetCrewman(crewman);
	}

	public void SetCrewman(CrewmanAvatar crewman)
	{
		m_crewmanActivated = false;
		crewman.SetStationSlow(this);
		m_currentCrewman = crewman;
		m_hasCrewman = true;
		m_isLockedInStation = false;
	}

	public void ActivateCrewman()
	{
		m_crewmanActivated = true;
		if (this.OnControlsStateChange != null)
		{
			this.OnControlsStateChange(obj: true);
		}
	}

	public void LeaveStation(CrewmanAvatar cm)
	{
		if (cm == m_currentCrewman)
		{
			m_isLockedInStation = false;
			m_currentCrewman = null;
			m_hasCrewman = false;
			m_crewmanActivated = false;
			if (this.OnControlsStateChange != null)
			{
				this.OnControlsStateChange(obj: false);
			}
		}
	}

	public void SetLockedInStation(bool locked)
	{
		m_isLockedInStation = locked;
	}

	public bool IsLockedInStation()
	{
		return m_isLockedInStation;
	}

	public void SetIncapacitated(CrewmanAvatar cm, bool incap)
	{
		if (cm == m_currentCrewman && this.OnControlsStateChange != null)
		{
			if (incap)
			{
				this.OnControlsStateChange(obj: false);
			}
			else
			{
				this.OnControlsStateChange(obj: true);
			}
		}
	}

	protected override void AbandonInteraction(Interaction interaction, CrewmanAvatar crewman)
	{
	}

	protected override void BeginTimedInteraction(Interaction interaction, CrewmanAvatar crewman, InteractionContract contract)
	{
	}

	public virtual bool IsActivityInProgress(out float progress, out string iconType)
	{
		progress = 0f;
		iconType = string.Empty;
		return false;
	}

	public bool HasCrewman()
	{
		return m_hasCrewman && m_crewmanActivated;
	}

	public CrewmanAvatar GetCurrentCrewmanRough()
	{
		return m_currentCrewman;
	}

	public CrewmanAvatar GetCurrentCrewman()
	{
		if (m_crewmanActivated)
		{
			return m_currentCrewman;
		}
		return null;
	}

	public GameObject GetControlPanelUIPrefab()
	{
		return m_controlPanelUI;
	}
}
