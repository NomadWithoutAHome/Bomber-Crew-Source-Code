using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class AmmoBeltBox : InteractiveItem
{
	[SerializeField]
	private GameObject m_ammoBeltHoldableItemPrefab;

	[SerializeField]
	private Transform m_createdObjectsParent;

	private Interaction m_primaryInteraction = new Interaction("PickUpAmmo", queryProgress: false, EnumToIconMapping.InteractionOrAlertType.Main);

	private List<CarryableItem> m_currentAmmoBelts = new List<CarryableItem>(8);

	public override Interaction GetInteractionOptions(CrewmanAvatar crewman)
	{
		bool flag = false;
		if (crewman != null && crewman.GetCarriedItem() != null && crewman.GetCarriedItem().GetComponent<AmmoBelt>() != null)
		{
			flag = true;
		}
		if (!flag)
		{
			return m_primaryInteraction;
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
		while (m_currentAmmoBelts.Contains(null))
		{
			m_currentAmmoBelts.Remove(null);
		}
		int num = m_currentAmmoBelts.Count;
		if (num > 4)
		{
			List<CarryableItem> list = new List<CarryableItem>(4);
			foreach (CarryableItem currentAmmoBelt in m_currentAmmoBelts)
			{
				if (currentAmmoBelt.IsLoose())
				{
					list.Add(currentAmmoBelt);
				}
			}
			int num2 = list.Count;
			while (num2 > 0 && num > 4)
			{
				Object.Destroy(list[0].gameObject);
				list.RemoveAt(0);
				num2--;
				num--;
			}
		}
		GameObject gameObject = Object.Instantiate(m_ammoBeltHoldableItemPrefab);
		gameObject.transform.parent = m_createdObjectsParent;
		CarryableItem component = gameObject.GetComponent<CarryableItem>();
		component.SetParentTransform(m_createdObjectsParent);
		component.PickedUpBy(crewman);
		m_currentAmmoBelts.Add(component);
		InteractionContract contractFor = GetContractFor(crewman);
		bool flag = false;
		if (contractFor != null)
		{
			Station startingFromStation = contractFor.m_startingFromStation;
			if (startingFromStation is StationGunner)
			{
				Interaction interactionOptionsPublic = startingFromStation.GetInteractionOptionsPublic(crewman);
				if (interactionOptionsPublic != null)
				{
					crewman.QueueAction(andCancelExisting: false, startingFromStation, interactionOptionsPublic);
					flag = true;
				}
			}
		}
		if (flag)
		{
			Singleton<SpeechPrioritiser>.Instance.RequestNewSpeech(StaticSpeechSets.GetAmmoReturn, crewman);
		}
		else
		{
			Singleton<SpeechPrioritiser>.Instance.RequestNewSpeech(StaticSpeechSets.GetAmmoStaying, crewman);
		}
	}

	private new void Start()
	{
	}

	private void Update()
	{
	}
}
