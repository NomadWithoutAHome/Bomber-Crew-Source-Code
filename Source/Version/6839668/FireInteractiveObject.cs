using UnityEngine;

public class FireInteractiveObject : InteractiveItem
{
	[SerializeField]
	private FireOverview m_overview;

	private FireArea m_fireArea;

	private Interaction m_putOutFire = new Interaction("Extinguish", 0f, EnumToIconMapping.InteractionOrAlertType.Extinguish);

	private void Awake()
	{
		m_fireArea = GetComponent<FireArea>();
		m_putOutFire.SetMaterialIndex(3);
	}

	public override Interaction GetInteractionOptions(CrewmanAvatar crewman)
	{
		if (crewman.IsCarryingItem() && crewman.GetCarriedItem().GetComponent<FireExtinguisher>() != null && m_fireArea.IsOnFire() && m_overview.IsNearestBurningInDirection(m_fireArea, crewman.transform.position))
		{
			m_putOutFire.SetFinishRemotely();
			return m_putOutFire;
		}
		return null;
	}

	protected override void AbandonInteraction(Interaction interaction, CrewmanAvatar crewman)
	{
	}

	protected override void BeginTimedInteraction(Interaction interaction, CrewmanAvatar crewman, InteractionContract contract)
	{
	}

	protected override void FinishInteraction(Interaction interaction, CrewmanAvatar crewman)
	{
		crewman.QueueFireMode(andCancelOthers: true, m_fireArea);
	}
}
