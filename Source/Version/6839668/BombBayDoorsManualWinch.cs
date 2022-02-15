using UnityEngine;

public class BombBayDoorsManualWinch : InteractiveItem
{
	[SerializeField]
	private BombBayDoors m_bayDoors;

	private bool m_isWinching;

	private Interaction m_winchInteraction = new Interaction("Winch", queryProgress: true, EnumToIconMapping.InteractionOrAlertType.Repair);

	public override Interaction GetInteractionOptions(CrewmanAvatar crewman)
	{
		if (m_bayDoors.RequiresManualWinch())
		{
			return m_winchInteraction;
		}
		return null;
	}

	protected override void AbandonInteraction(Interaction interaction, CrewmanAvatar crewman)
	{
		m_isWinching = false;
	}

	protected override void BeginTimedInteraction(Interaction interaction, CrewmanAvatar crewman, InteractionContract contract)
	{
		m_isWinching = true;
	}

	public override float QueryProgress(Interaction interaction, CrewmanAvatar crewman)
	{
		return 1f - m_bayDoors.GetWinchProgress();
	}

	protected override void FinishInteraction(Interaction interaction, CrewmanAvatar crewman)
	{
		m_isWinching = false;
	}

	public bool IsWinching()
	{
		return m_isWinching;
	}
}
