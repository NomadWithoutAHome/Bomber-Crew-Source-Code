using System;
using UnityEngine;

public class CarryableItem : InteractiveItem
{
	public enum State
	{
		OnRack,
		Loose,
		Carried
	}

	public enum CarryableTypeQuick
	{
		AmmoBelt,
		Parachute,
		FirstAidKit,
		Extinguisher
	}

	private State m_currentState;

	private Interaction m_pickupInteraction;

	private Interaction m_pickupFromRackInteraction;

	private CrewmanAvatar m_carriedBy;

	[SerializeField]
	private string m_hudSpriteName;

	[SerializeField]
	private CarryableTypeQuick m_quickType;

	private GearRack m_currentRack;

	private Transform m_rootTransform;

	public event Action<CrewmanAvatar> OnPickedUp;

	public event Action OnDropped;

	public void SetParentTransform(Transform t)
	{
		m_rootTransform = t;
		base.transform.parent = t;
	}

	public CarryableTypeQuick GetCarryableType()
	{
		return m_quickType;
	}

	private void Awake()
	{
		ToState(State.Loose);
		m_pickupFromRackInteraction = new Interaction("Pick up", queryProgress: false, EnumToIconMapping.InteractionOrAlertType.Main);
		m_pickupFromRackInteraction.m_hasExtraHoverIconType = true;
		switch (m_quickType)
		{
		case CarryableTypeQuick.AmmoBelt:
			m_pickupInteraction = new Interaction("Pick up", queryProgress: false, EnumToIconMapping.InteractionOrAlertType.PickUpAmmoBox);
			m_pickupFromRackInteraction.m_iconTypeHovered = EnumToIconMapping.InteractionOrAlertType.PickUpAmmoBox;
			break;
		case CarryableTypeQuick.Extinguisher:
			m_pickupInteraction = new Interaction("Pick up", queryProgress: false, EnumToIconMapping.InteractionOrAlertType.PickUpExtinguisher);
			m_pickupFromRackInteraction.m_iconTypeHovered = EnumToIconMapping.InteractionOrAlertType.PickUpExtinguisher;
			break;
		case CarryableTypeQuick.FirstAidKit:
			m_pickupInteraction = new Interaction("Pick up", queryProgress: false, EnumToIconMapping.InteractionOrAlertType.PickUpMedKit);
			m_pickupFromRackInteraction.m_iconTypeHovered = EnumToIconMapping.InteractionOrAlertType.PickUpMedKit;
			break;
		case CarryableTypeQuick.Parachute:
			m_pickupInteraction = new Interaction("Pick up", queryProgress: false, EnumToIconMapping.InteractionOrAlertType.PickUpParachute);
			m_pickupFromRackInteraction.m_iconTypeHovered = EnumToIconMapping.InteractionOrAlertType.PickUpParachute;
			break;
		}
	}

	public override Interaction GetInteractionOptions(CrewmanAvatar crewman)
	{
		if (m_currentState != State.Carried && crewman != null)
		{
			if (m_currentState == State.OnRack)
			{
				return m_pickupFromRackInteraction;
			}
			return m_pickupInteraction;
		}
		return null;
	}

	public string GetHudSprite()
	{
		return m_hudSpriteName;
	}

	protected override void FinishInteraction(Interaction interaction, CrewmanAvatar crewman)
	{
		if (m_currentState == State.Loose)
		{
			PickedUpBy(crewman);
		}
		else if (m_currentState == State.OnRack)
		{
			m_currentRack.PickupItem(this);
			m_currentRack = null;
			PickedUpBy(crewman);
		}
	}

	public void PickedUpBy(CrewmanAvatar crewman)
	{
		m_carriedBy = crewman;
		ToState(State.Carried);
		crewman.PickUp(this);
		if (this.OnPickedUp != null)
		{
			this.OnPickedUp(crewman);
		}
	}

	public CrewmanAvatar GetCarriedBy()
	{
		return m_carriedBy;
	}

	public void Drop(Vector3 atPos)
	{
		if (m_currentState == State.Carried)
		{
			GetComponent<Rigidbody>().velocity = Vector3.zero;
			base.transform.position = atPos;
			m_carriedBy = null;
			ToState(State.Loose);
			if (this.OnDropped != null)
			{
				this.OnDropped();
			}
		}
	}

	public bool IsLoose()
	{
		return m_currentState == State.Loose;
	}

	protected override void AbandonInteraction(Interaction interaction, CrewmanAvatar crewman)
	{
	}

	protected override void BeginTimedInteraction(Interaction interaction, CrewmanAvatar crewman, InteractionContract contract)
	{
	}

	public void Store(GearRack rack)
	{
		m_carriedBy = null;
		m_currentRack = rack;
		ToState(State.OnRack);
	}

	public override bool IsStatic()
	{
		return m_currentState == State.OnRack;
	}

	private void ToState(State state)
	{
		switch (state)
		{
		case State.Loose:
			GetComponent<Rigidbody>().velocity = Vector3.zero;
			GetComponent<Rigidbody>().isKinematic = false;
			base.transform.parent = m_rootTransform;
			SetInteractive(interactive: true);
			break;
		case State.OnRack:
			GetComponent<Rigidbody>().velocity = Vector3.zero;
			GetComponent<Rigidbody>().isKinematic = true;
			base.transform.parent = m_rootTransform;
			SetInteractive(interactive: true);
			break;
		case State.Carried:
			GetComponent<Rigidbody>().velocity = Vector3.zero;
			GetComponent<Rigidbody>().isKinematic = true;
			base.transform.parent = m_carriedBy.GetHandLocator();
			base.transform.localRotation = Quaternion.identity;
			base.transform.localPosition = Vector3.zero;
			SetInteractive(interactive: false);
			break;
		}
		m_currentState = state;
	}
}
