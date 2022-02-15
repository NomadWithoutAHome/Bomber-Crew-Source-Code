using BomberCrewCommon;
using UnityEngine;

public class ExitPoint : InteractiveItem
{
	[SerializeField]
	private Transform m_externalPosition;

	[SerializeField]
	private HatchDoor m_exitHatch;

	[SerializeField]
	private BomberDestroyableSection m_onlyActiveIfSectionDestroyed;

	[SerializeField]
	private BomberDestroyableSection m_onlyActiveIfSectionExists;

	[SerializeField]
	private bool m_onlyActiveIfCrewmanInternal;

	[SerializeField]
	private bool m_onlyActiveIfCrewmanExternal;

	private Interaction m_exitInteraction;

	private void Awake()
	{
		m_exitInteraction = new Interaction("Bail Out", 0.1f, EnumToIconMapping.InteractionOrAlertType.BailOut);
	}

	public override void Start()
	{
		base.Start();
		Singleton<BomberSpawn>.Instance.GetBomberSystems().RegisterInteractiveSearchable(typeof(ExitPoint), GetComponent<InteractiveItem>());
	}

	public Vector3 GetLocalPageExternalPosition()
	{
		return m_externalPosition.position;
	}

	public override Interaction GetInteractionOptions(CrewmanAvatar crewman)
	{
		if (crewman.IsCarryingItem() && crewman.GetCarriedItem().GetComponent<Parachute>() != null)
		{
			if (IsValidForCrewman(crewman))
			{
				return m_exitInteraction;
			}
			return null;
		}
		return null;
	}

	public bool IsValidForCrewman(CrewmanAvatar ca)
	{
		if (m_onlyActiveIfSectionDestroyed != null && !m_onlyActiveIfSectionDestroyed.IsDestroyed())
		{
			return false;
		}
		if (m_onlyActiveIfSectionExists != null && m_onlyActiveIfSectionExists.IsDestroyed())
		{
			return false;
		}
		if (m_onlyActiveIfCrewmanExternal && (ca.GetWalkZone() == null || !ca.GetWalkZone().IsExternal()))
		{
			return false;
		}
		if (m_onlyActiveIfCrewmanInternal && (ca.GetWalkZone() == null || ca.GetWalkZone().IsExternal()))
		{
			return false;
		}
		return true;
	}

	protected override void FinishInteraction(Interaction interaction, CrewmanAvatar crewman)
	{
		if (interaction == m_exitInteraction)
		{
			crewman.BailOutFromExitPoint(this);
		}
	}

	protected override void AbandonInteraction(Interaction interaction, CrewmanAvatar crewman)
	{
	}

	protected override void BeginTimedInteraction(Interaction interaction, CrewmanAvatar crewman, InteractionContract contract)
	{
		if (m_exitHatch != null)
		{
			m_exitHatch.RequestOpen(0.5f);
		}
	}
}
