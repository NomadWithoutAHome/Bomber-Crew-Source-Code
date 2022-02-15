using System;
using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class GearRack : InteractiveItem
{
	public class HeldItem
	{
		public CarryableItem m_item;
	}

	[Serializable]
	public class StringToCarryableObjectLookup
	{
		[SerializeField]
		public string m_id;

		[SerializeField]
		public GameObject m_carryableObject;
	}

	[SerializeField]
	private Transform[] m_rackplacements;

	[SerializeField]
	private StringToCarryableObjectLookup[] m_placeableObjects;

	private List<HeldItem> m_heldItems = new List<HeldItem>();

	private Interaction m_placeInteraction;

	private int m_maxItems;

	private BomberSystemUniqueId m_baseSpawner;

	public int GetMaxItems()
	{
		return m_maxItems;
	}

	private new void Start()
	{
		m_baseSpawner = base.transform.parent.GetComponent<BomberSystemUniqueId>();
		EquipmentUpgradeFittableBase upgrade = m_baseSpawner.GetUpgrade();
		RackSystemUpgrade rackSystemUpgrade = (RackSystemUpgrade)upgrade;
		m_maxItems = 1;
		if (rackSystemUpgrade != null)
		{
			m_maxItems = rackSystemUpgrade.GetNumberOfSpaces();
		}
		while (m_heldItems.Count < m_maxItems)
		{
			m_heldItems.Add(null);
		}
		string uniqueId = m_baseSpawner.GetUniqueId();
		BomberUpgradeConfig currentConfig = Singleton<BomberContainer>.Instance.GetCurrentConfig();
		for (int i = 0; i < Mathf.Min(m_maxItems, m_rackplacements.Length); i++)
		{
			string text = currentConfig.GetUpgradeDetail(uniqueId, "rack" + i);
			if (string.IsNullOrEmpty(text))
			{
				text = m_placeableObjects[0].m_id;
			}
			StringToCarryableObjectLookup[] placeableObjects = m_placeableObjects;
			foreach (StringToCarryableObjectLookup stringToCarryableObjectLookup in placeableObjects)
			{
				if (stringToCarryableObjectLookup.m_id == text)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(stringToCarryableObjectLookup.m_carryableObject);
					gameObject.transform.parent = m_rackplacements[i];
					gameObject.GetComponent<CarryableItem>().SetParentTransform(base.transform);
					StoreItem(gameObject.GetComponent<CarryableItem>());
					break;
				}
			}
		}
		m_placeInteraction = new Interaction("Store", queryProgress: false, EnumToIconMapping.InteractionOrAlertType.PutBack);
	}

	public override Interaction GetInteractionOptions(CrewmanAvatar crewman)
	{
		if (crewman.IsCarryingItem() && FindNextFreeSlot() != -1)
		{
			return m_placeInteraction;
		}
		return null;
	}

	protected override void FinishInteraction(Interaction interaction, CrewmanAvatar crewman)
	{
		if (interaction == m_placeInteraction)
		{
			crewman.StoreItem(this);
		}
	}

	private int FindNextFreeSlot()
	{
		int result = -1;
		int num = 0;
		foreach (HeldItem heldItem in m_heldItems)
		{
			if (heldItem == null)
			{
				return num;
			}
			num++;
		}
		return result;
	}

	public bool StoreItem(CarryableItem item)
	{
		int num = FindNextFreeSlot();
		bool result = false;
		if (num != -1)
		{
			item.Store(this);
			m_heldItems[num] = new HeldItem();
			m_heldItems[num].m_item = item;
			item.transform.position = m_rackplacements[num].transform.position;
			item.transform.rotation = m_rackplacements[num].transform.rotation;
			m_rackplacements[num].gameObject.SetActive(value: true);
			result = true;
		}
		return result;
	}

	public List<HeldItem> GetHeldItems()
	{
		return m_heldItems;
	}

	public bool HasItem(CarryableItem item)
	{
		foreach (HeldItem heldItem in m_heldItems)
		{
			if (heldItem != null && heldItem.m_item == item)
			{
				return true;
			}
		}
		return false;
	}

	public void PickupItem(CarryableItem item)
	{
		int num = 0;
		int num2 = -1;
		foreach (HeldItem heldItem in m_heldItems)
		{
			if (heldItem != null && heldItem.m_item == item)
			{
				m_rackplacements[num].gameObject.SetActive(value: false);
				num2 = num;
				break;
			}
			num++;
		}
		if (num2 != -1)
		{
			m_heldItems[num2] = null;
		}
	}

	protected override void AbandonInteraction(Interaction interaction, CrewmanAvatar crewman)
	{
	}

	protected override void BeginTimedInteraction(Interaction interaction, CrewmanAvatar crewman, InteractionContract contract)
	{
	}
}
