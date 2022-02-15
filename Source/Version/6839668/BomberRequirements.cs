using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Bomber Crew/Bomber Requirements")]
public class BomberRequirements : ScriptableObject
{
	[Serializable]
	public class BomberEquipmentRequirement
	{
		[SerializeField]
		private BomberUpgradeType m_upgradeType;

		[SerializeField]
		private bool m_allowNull;

		[SerializeField]
		private EquipmentUpgradeFittableBase m_default;

		[SerializeField]
		private string m_uniqueUpgradePartId;

		[SerializeField]
		[NamedText]
		private string m_namedText;

		[SerializeField]
		private bool m_clearDepth;

		[SerializeField]
		private int m_filteringFlags;

		public BomberUpgradeType GetUpgradeConfig()
		{
			return m_upgradeType;
		}

		public bool CanBeNull()
		{
			return m_allowNull;
		}

		public bool ShouldClearDepthToPreview()
		{
			return m_clearDepth;
		}

		public string GetUniquePartId()
		{
			return m_uniqueUpgradePartId;
		}

		public EquipmentUpgradeFittableBase GetDefault()
		{
			return m_default;
		}

		public string GetNamedText()
		{
			return m_namedText;
		}

		public int GetFilteringFlags()
		{
			return m_filteringFlags;
		}
	}

	[SerializeField]
	private BomberEquipmentRequirement[] m_equipmentRequirements;

	[SerializeField]
	private string[] m_defaultItemDetails;

	public BomberEquipmentRequirement[] GetRequirements()
	{
		return m_equipmentRequirements;
	}

	public string[] GetDefaultItemDetails()
	{
		return m_defaultItemDetails;
	}
}
