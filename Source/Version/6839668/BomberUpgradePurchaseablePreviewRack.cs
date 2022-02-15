using System.Collections.Generic;
using UnityEngine;

public class BomberUpgradePurchaseablePreviewRack : BomberUpgradePurchaseablePreview
{
	[SerializeField]
	private GameObject m_slotPreviewPrefab;

	[SerializeField]
	private LayoutGrid m_slotPreviewLayout;

	[SerializeField]
	private GameObject m_isEditingPrompt;

	[SerializeField]
	private GameObject m_isNotEditingPrompt;

	private List<GameObject> m_createdSlots = new List<GameObject>();

	private RackSystemUpgrade m_previouslyCreatedFor;

	public void SetIsEditing(bool isEditing)
	{
		m_isEditingPrompt.SetActive(isEditing);
		m_isNotEditingPrompt.SetActive(!isEditing);
	}

	public override void Refresh()
	{
		base.Refresh();
		EquipmentUpgradeFittableBase upgradeFor = m_config.GetUpgradeFor(m_requirementSlot);
		if (upgradeFor == m_equipment)
		{
			if (m_equipment != null)
			{
				RackSystemUpgrade rackSystemUpgrade = (RackSystemUpgrade)m_equipment;
				if (rackSystemUpgrade != m_previouslyCreatedFor)
				{
					ClearCreatedSlots();
					for (int i = 0; i < rackSystemUpgrade.GetNumberOfSpaces(); i++)
					{
						GameObject gameObject = Object.Instantiate(m_slotPreviewPrefab);
						gameObject.transform.parent = m_slotPreviewLayout.transform;
						gameObject.transform.localPosition = Vector3.zero;
						gameObject.GetComponent<BomberUpgradeRackSlotContents>().SetUp(m_config, m_requirementSlot, i);
						m_createdSlots.Add(gameObject);
					}
				}
				else
				{
					for (int j = 0; j < rackSystemUpgrade.GetNumberOfSpaces(); j++)
					{
						GameObject gameObject2 = m_createdSlots[j];
						gameObject2.GetComponent<BomberUpgradeRackSlotContents>().SetUp(m_config, m_requirementSlot, j);
					}
				}
				m_slotPreviewLayout.RepositionChildren();
				m_previouslyCreatedFor = rackSystemUpgrade;
			}
			else
			{
				m_previouslyCreatedFor = null;
				ClearCreatedSlots();
			}
		}
		else
		{
			m_previouslyCreatedFor = null;
			ClearCreatedSlots();
		}
	}

	private void ClearCreatedSlots()
	{
		foreach (GameObject createdSlot in m_createdSlots)
		{
			Object.DestroyImmediate(createdSlot);
		}
		m_createdSlots.Clear();
	}
}
