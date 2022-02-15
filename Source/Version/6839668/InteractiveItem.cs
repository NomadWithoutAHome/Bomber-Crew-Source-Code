using System;
using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public abstract class InteractiveItem : MonoBehaviour
{
	[Serializable]
	public class Interaction
	{
		public enum InteractionAnimType
		{
			InteractingNormal,
			Healing,
			RepairEngine,
			RepairFuelTank
		}

		[SerializeField]
		public string m_description;

		[SerializeField]
		public float m_duration;

		[SerializeField]
		public bool m_queryProgress;

		[SerializeField]
		public EnumToIconMapping.InteractionOrAlertType m_iconType;

		[SerializeField]
		public EnumToIconMapping.InteractionOrAlertType m_iconTypeHovered;

		[SerializeField]
		public bool m_hasExtraHoverIconType;

		[SerializeField]
		private bool m_finishRemotely;

		[SerializeField]
		private float m_animationMultiplier;

		[SerializeField]
		public int m_teardropMaterialIndex;

		public bool m_looseDistance;

		[SerializeField]
		public InteractionAnimType m_animType;

		public Interaction(string description, float duration, EnumToIconMapping.InteractionOrAlertType iconType)
		{
			m_description = description;
			m_duration = duration;
			m_queryProgress = false;
			m_iconType = iconType;
			if (m_queryProgress)
			{
				m_duration = 1f;
			}
		}

		public Interaction(string description, bool queryProgress, EnumToIconMapping.InteractionOrAlertType iconType)
		{
			m_iconType = iconType;
			m_description = description;
			m_duration = 0f;
			m_queryProgress = queryProgress;
			if (m_queryProgress)
			{
				m_duration = 1f;
			}
		}

		public void SetMaterialIndex(int i)
		{
			m_teardropMaterialIndex = i;
		}

		public void SetFinishRemotely()
		{
			m_finishRemotely = true;
		}

		public bool CanFinishRemotely()
		{
			return m_finishRemotely;
		}

		public void SetAnimationMultiplier(float am)
		{
			m_animationMultiplier = am;
		}

		public float GetAnimationMultiplier()
		{
			return m_animationMultiplier;
		}

		public void SetInteractionAnimType(InteractionAnimType animType)
		{
			m_animType = animType;
		}

		public InteractionAnimType GetInteractionType()
		{
			return m_animType;
		}
	}

	public class InteractionContract
	{
		public enum ContractState
		{
			IntentRegistered,
			Started,
			Finished
		}

		public CrewmanAvatar m_crewman;

		public Interaction m_interaction;

		public float m_durationCountdown;

		public InteractiveItem m_item;

		public float m_maxDuration;

		public Station m_startingFromStation;

		public Action<InteractionContract> OnContractUpdate;

		public Action<InteractionContract> OnContractNotUpdating;

		public Action<InteractionContract> OnContractFinish;

		public ContractState m_contractState;

		public InteractionContract(CrewmanAvatar cmn, Interaction interaction, InteractiveItem item)
		{
			m_interaction = interaction;
			m_maxDuration = (m_durationCountdown = interaction.m_duration);
			m_crewman = cmn;
			m_item = item;
			m_startingFromStation = cmn.GetStation();
		}

		public ContractState GetContractState()
		{
			return m_contractState;
		}

		public float GetValue()
		{
			if (m_maxDuration == 0f)
			{
				return 1f;
			}
			return Mathf.Clamp01(1f - m_durationCountdown / m_maxDuration);
		}

		public void DontPumpContract()
		{
			m_item.InteractionPaused(this);
		}

		public bool PumpContract(float speedMultiplier)
		{
			if (m_interaction.m_queryProgress)
			{
				m_durationCountdown = m_item.QueryProgress(m_interaction, m_crewman);
			}
			else
			{
				m_durationCountdown -= Time.deltaTime * speedMultiplier;
			}
			if (OnContractUpdate != null)
			{
				OnContractUpdate(this);
			}
			if (m_durationCountdown <= 0f)
			{
				m_item.FinishInteraction(m_interaction, m_crewman);
				m_item.m_currentContracts.Remove(this);
				if (OnContractFinish != null)
				{
					OnContractFinish(this);
				}
				return false;
			}
			return true;
		}

		public void AbandonContract()
		{
			m_contractState = ContractState.Finished;
			m_item.AbandonInteraction(m_interaction, m_crewman);
			m_item.m_currentContracts.Remove(this);
			if (OnContractFinish != null)
			{
				OnContractFinish(this);
			}
		}
	}

	[SerializeField]
	private Transform[] m_interactionPositions;

	[SerializeField]
	private Transform m_walkingToArrowLocator;

	[SerializeField]
	private OutlineMesh[] m_meshesToOutline;

	[SerializeField]
	private tk2dUIItem m_interactionItem;

	[SerializeField]
	private BomberWalkZone m_containerZone;

	[SerializeField]
	private bool m_allowMultipleContracts;

	[SerializeField]
	private int m_rewiredInputId;

	[SerializeField]
	private string m_hotkeyPCDisplay;

	[SerializeField]
	private string m_hotkeyControllerDisplay;

	[SerializeField]
	private bool m_hotkeyIsCritical;

	private bool m_isInteractive = true;

	private bool m_isHoveredOver;

	private bool m_disconnectedFromBomber;

	private bool m_hasWalkingToArrowLocator;

	private List<OutlineMesh> m_meshesToOutlineFull;

	private List<InteractionContract> m_currentContracts = new List<InteractionContract>();

	private void OnDestroy()
	{
		for (int i = 0; i < m_currentContracts.Count; i++)
		{
			m_currentContracts[i].m_crewman.CancelFromContract(m_currentContracts[i]);
		}
	}

	private void Awake()
	{
		if (m_walkingToArrowLocator != null)
		{
			m_hasWalkingToArrowLocator = true;
		}
	}

	public virtual void Start()
	{
		BomberDestroyableSection bomberDestroyableSection = BomberDestroyableSection.FindDestroyableSectionFor(base.transform);
		if (bomberDestroyableSection != null)
		{
			bomberDestroyableSection.OnSectionDestroy += DestroyInteractive;
		}
	}

	private void DestroyInteractive()
	{
		for (int i = 0; i < m_currentContracts.Count; i++)
		{
			m_currentContracts[i].m_crewman.CancelFromContract(m_currentContracts[i]);
		}
		m_disconnectedFromBomber = true;
	}

	public virtual bool IsStatic()
	{
		return true;
	}

	public Transform GetMovementArrowTransform()
	{
		if (!m_hasWalkingToArrowLocator)
		{
			return base.transform;
		}
		return m_walkingToArrowLocator;
	}

	public int GetInputId()
	{
		return m_rewiredInputId;
	}

	public tk2dUIItem GetInteractionItem()
	{
		return m_interactionItem;
	}

	public BomberWalkZone GetContainerWalkZone()
	{
		return m_containerZone;
	}

	public string GetHotkeyText()
	{
		return m_hotkeyPCDisplay;
	}

	public string GetHotkeyControllerText()
	{
		return m_hotkeyControllerDisplay;
	}

	public bool GetHotkeyIsCritical()
	{
		return m_hotkeyIsCritical;
	}

	public abstract Interaction GetInteractionOptions(CrewmanAvatar crewman);

	public Interaction GetInteractionOptionsPublic(CrewmanAvatar crewman, bool skipNullCheck = false)
	{
		if (!skipNullCheck && crewman == null)
		{
			return null;
		}
		if (crewman.AreMoveOrdersBlocked() || !m_isInteractive || m_disconnectedFromBomber)
		{
			return null;
		}
		if (m_currentContracts.Count == 0 || m_allowMultipleContracts)
		{
			return GetInteractionOptions(crewman);
		}
		if (m_currentContracts[0].m_crewman == crewman)
		{
			return GetInteractionOptions(crewman);
		}
		return null;
	}

	protected abstract void FinishInteraction(Interaction interaction, CrewmanAvatar crewman);

	protected abstract void AbandonInteraction(Interaction interaction, CrewmanAvatar crewman);

	protected abstract void BeginTimedInteraction(Interaction interaction, CrewmanAvatar crewman, InteractionContract contract);

	public virtual float QueryProgress(Interaction interaction, CrewmanAvatar crewman)
	{
		return 1f;
	}

	public void HoverOver(bool hover)
	{
		if (m_meshesToOutlineFull == null)
		{
			m_meshesToOutlineFull = new List<OutlineMesh>();
			m_meshesToOutlineFull.AddRange(m_meshesToOutline);
		}
		foreach (OutlineMesh item in m_meshesToOutlineFull)
		{
			if (item != null && item.GetClonedRenderer() != null)
			{
				item.GetClonedRenderer().enabled = hover;
			}
		}
		m_isHoveredOver = hover;
		if (hover)
		{
			Singleton<ContextControl>.Instance.HoverInteractive(this);
		}
		else
		{
			Singleton<ContextControl>.Instance.UnhoverInteractive(this);
		}
	}

	public bool IsHovered()
	{
		return m_isHoveredOver;
	}

	protected void RegisterAdditionalOutlines(OutlineMesh[] meshes)
	{
		if (m_meshesToOutlineFull == null)
		{
			m_meshesToOutlineFull = new List<OutlineMesh>();
			m_meshesToOutlineFull.AddRange(m_meshesToOutline);
		}
		m_meshesToOutlineFull.AddRange(meshes);
	}

	public InteractionContract SetIntentInteraction(Interaction interaction, CrewmanAvatar crewman)
	{
		InteractionContract interactionContract = new InteractionContract(crewman, interaction, this);
		m_currentContracts.Add(interactionContract);
		return interactionContract;
	}

	public InteractionContract GetContractFor(CrewmanAvatar ca)
	{
		foreach (InteractionContract currentContract in m_currentContracts)
		{
			if (currentContract.m_crewman == ca)
			{
				return currentContract;
			}
		}
		return null;
	}

	public InteractionContract StartInteraction(InteractionContract ic)
	{
		if (ic.m_interaction.m_duration == 0f)
		{
			ic.m_contractState = InteractionContract.ContractState.Finished;
			FinishInteraction(ic.m_interaction, ic.m_crewman);
			m_currentContracts.Remove(ic);
			if (ic.OnContractFinish != null)
			{
				ic.OnContractFinish(ic);
			}
			return null;
		}
		ic.m_contractState = InteractionContract.ContractState.Started;
		BeginTimedInteraction(ic.m_interaction, ic.m_crewman, ic);
		return ic;
	}

	public bool IsAnyInteractionInProgress()
	{
		return m_currentContracts.Count > 0;
	}

	public bool IsAnyInteractionInProgress(CrewmanAvatar cm)
	{
		if (m_allowMultipleContracts)
		{
			foreach (InteractionContract currentContract in m_currentContracts)
			{
				if (currentContract.m_crewman == cm)
				{
					return true;
				}
			}
			return false;
		}
		return m_currentContracts.Count > 0;
	}

	public bool IsInteractionInProgress(Interaction interaction)
	{
		foreach (InteractionContract currentContract in m_currentContracts)
		{
			if (currentContract.m_interaction == interaction)
			{
				return true;
			}
		}
		return false;
	}

	public void SetInteractive(bool interactive)
	{
		m_isInteractive = interactive;
	}

	public virtual void InteractionPaused(InteractionContract ic)
	{
	}

	public virtual Vector3 GetInteractionPositionFor(Transform originalPosition, CrewmanAvatar crewman)
	{
		Vector3 position = base.transform.position;
		float num = float.MaxValue;
		Transform[] interactionPositions = m_interactionPositions;
		foreach (Transform transform in interactionPositions)
		{
			float magnitude = (transform.position - originalPosition.position).magnitude;
			if (magnitude < num)
			{
				num = magnitude;
				position = transform.position;
			}
		}
		return position;
	}
}
