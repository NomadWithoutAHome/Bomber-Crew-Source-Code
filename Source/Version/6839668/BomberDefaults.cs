using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Bomber Crew/Bomber Default Settings")]
public class BomberDefaults : ScriptableObject
{
	[Serializable]
	public class RequirementDefault
	{
		[SerializeField]
		public string m_requirementName;

		[SerializeField]
		public EquipmentUpgradeFittableBase m_equipment;
	}

	[Serializable]
	public class RequirementDetail
	{
		[SerializeField]
		public string m_slotName;

		[SerializeField]
		public string m_detailName;

		[SerializeField]
		public string m_detailValue;
	}

	[SerializeField]
	public RequirementDefault[] m_requirement;

	[SerializeField]
	public int m_intelMinReq;

	[SerializeField]
	public RequirementDetail[] m_details;

	[SerializeField]
	public EquipmentUpgradeFittableBase.AircraftUpgradeType m_upgradeType;
}
