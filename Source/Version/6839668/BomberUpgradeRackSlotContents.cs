using System;
using UnityEngine;

public class BomberUpgradeRackSlotContents : MonoBehaviour
{
	[Serializable]
	public class BomberUpgradeRackSelectionButton
	{
		[SerializeField]
		public GameObject m_installedHierarchy;

		[SerializeField]
		public string m_rackContentsName;

		[SerializeField]
		public tk2dUIItem m_clickButton;
	}

	[SerializeField]
	private BomberUpgradeRackSelectionButton[] m_selectionButtons;

	private BomberRequirements.BomberEquipmentRequirement m_requirement;

	private int m_slotIndex;

	private BomberUpgradeConfig m_config;

	private void Awake()
	{
		BomberUpgradeRackSelectionButton[] selectionButtons = m_selectionButtons;
		foreach (BomberUpgradeRackSelectionButton bursb in selectionButtons)
		{
			bursb.m_clickButton.OnClick += delegate
			{
				Install(bursb.m_rackContentsName);
			};
		}
	}

	private void Install(string toInstall)
	{
		m_config.SetUpgradeDetail(m_requirement.GetUniquePartId(), "rack" + m_slotIndex, toInstall);
		Refresh();
		BomberUpgradeScreenController bomberUpgradeScreenController = UnityEngine.Object.FindObjectOfType<BomberUpgradeScreenController>();
		if (bomberUpgradeScreenController != null)
		{
			bomberUpgradeScreenController.Refresh();
		}
	}

	public void SetUp(BomberUpgradeConfig config, BomberRequirements.BomberEquipmentRequirement requirement, int slotIndex)
	{
		m_config = config;
		m_requirement = requirement;
		m_slotIndex = slotIndex;
		Refresh();
	}

	private void Refresh()
	{
		string text = m_config.GetUpgradeDetail(m_requirement.GetUniquePartId(), "rack" + m_slotIndex);
		if (string.IsNullOrEmpty(text))
		{
			text = m_selectionButtons[0].m_rackContentsName;
			m_config.SetUpgradeDetail(m_requirement.GetUniquePartId(), "rack" + m_slotIndex, text);
		}
		BomberUpgradeRackSelectionButton[] selectionButtons = m_selectionButtons;
		foreach (BomberUpgradeRackSelectionButton bomberUpgradeRackSelectionButton in selectionButtons)
		{
			bomberUpgradeRackSelectionButton.m_installedHierarchy.SetActive(bomberUpgradeRackSelectionButton.m_rackContentsName == text);
		}
	}
}
