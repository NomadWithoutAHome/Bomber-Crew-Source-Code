using System;
using BomberCrewCommon;
using UnityEngine;

public abstract class EquipmentUpgradeFittableBase : ScriptableObject
{
	[Serializable]
	public class MeshUpgrades
	{
		[SerializeField]
		public string m_forRequirement;

		[SerializeField]
		public Mesh[] m_upgradeMeshes;
	}

	public enum AircraftUpgradeType
	{
		AllAircraft,
		LancasterOnly,
		B17Only
	}

	[SerializeField]
	[NamedText]
	private string m_languageString;

	[SerializeField]
	private int m_markNumber;

	[SerializeField]
	[NamedText]
	private string m_descriptionString;

	[SerializeField]
	private Texture2D m_displayIcon;

	[SerializeField]
	private int m_moneyCost;

	[SerializeField]
	private int m_intelUnlockRequirement;

	[SerializeField]
	private int m_weight;

	[SerializeField]
	private int m_armour;

	[SerializeField]
	private int m_survivalPointsLand;

	[SerializeField]
	private int m_survivalPointsSea;

	[SerializeField]
	[Range(0f, 1f)]
	private float m_reliability;

	[SerializeField]
	private GameObject m_prefabPlaceable;

	[SerializeField]
	private MeshUpgrades[] m_meshUpgrades;

	[SerializeField]
	private int m_sortBump;

	[SerializeField]
	private bool m_isDLC;

	[SerializeField]
	private string m_DLCIconName;

	[SerializeField]
	private string m_DLCIconNameConsole;

	[SerializeField]
	private bool m_isGiftIcon;

	[SerializeField]
	private bool m_usePlatformOverride;

	[SerializeField]
	[NamedText]
	private string m_xboxNamedText;

	[SerializeField]
	[NamedText]
	private string m_ps4NamedText;

	[SerializeField]
	[NamedText]
	private string m_switchNamedText;

	[SerializeField]
	private string m_substitutionText;

	[SerializeField]
	private bool m_useSubstitutionText;

	[SerializeField]
	private AircraftUpgradeType m_aircraftUpgradeType;

	[SerializeField]
	private int m_assetDataVersion;

	public int GetAssetDataVersion()
	{
		return m_assetDataVersion;
	}

	public abstract BomberUpgradeType GetUpgradeType();

	public virtual bool IsDisplayableFor(BomberRequirements.BomberEquipmentRequirement requirement)
	{
		return true;
	}

	public AircraftUpgradeType GetAircraftUpgradeType()
	{
		if (m_assetDataVersion == 0)
		{
			return AircraftUpgradeType.LancasterOnly;
		}
		return m_aircraftUpgradeType;
	}

	public virtual bool HasArmour()
	{
		return false;
	}

	public int GetSortBump()
	{
		return m_sortBump;
	}

	public bool IsDLC()
	{
		return m_isDLC;
	}

	public string GetDLCIconName()
	{
		return m_DLCIconName;
	}

	public bool IsGiftIcon()
	{
		return m_isGiftIcon;
	}

	public Mesh[] GetMeshes(string forRequirement)
	{
		Mesh[] array = null;
		MeshUpgrades[] meshUpgrades = m_meshUpgrades;
		foreach (MeshUpgrades meshUpgrades2 in meshUpgrades)
		{
			if (meshUpgrades2.m_forRequirement == forRequirement || (string.IsNullOrEmpty(meshUpgrades2.m_forRequirement) && array == null))
			{
				array = meshUpgrades2.m_upgradeMeshes;
			}
		}
		return array;
	}

	public virtual bool HasWeight()
	{
		return false;
	}

	public virtual bool HasReliability()
	{
		return false;
	}

	public virtual bool HasSurvival()
	{
		return false;
	}

	public virtual StatHelper.StatInfo GetPrimaryStatInfo()
	{
		return null;
	}

	public virtual StatHelper.StatInfo GetSecondaryStatInfo()
	{
		return null;
	}

	public int GetIntelShowRequirement()
	{
		float num = Mathf.Max((float)m_intelUnlockRequirement * 0.5f, 500f);
		return Mathf.Max(m_intelUnlockRequirement - (int)num, 0);
	}

	public int GetIntelUnlockRequirement()
	{
		return m_intelUnlockRequirement;
	}

	public int GetWeight()
	{
		return m_weight;
	}

	public StatHelper.StatInfo GetWeightStat()
	{
		return StatHelper.StatInfo.CreateSmallerBetter(m_weight, null);
	}

	public int GetArmour()
	{
		return m_armour;
	}

	public StatHelper.StatInfo GetArmourStat()
	{
		return StatHelper.StatInfo.Create(m_armour, null);
	}

	public float GetReliability()
	{
		return m_reliability;
	}

	public int GetSurvivalPointsLand()
	{
		return m_survivalPointsLand;
	}

	public int GetSurvivalPointsSea()
	{
		return m_survivalPointsSea;
	}

	public StatHelper.StatInfo GetReliaibilityStat()
	{
		return StatHelper.StatInfo.CreatePercent(m_reliability, biggerIsBetter: true, null);
	}

	public int GetCost()
	{
		return m_moneyCost;
	}

	private string GetInternalText()
	{
		if (m_usePlatformOverride)
		{
			return string.Format(Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_languageString), m_markNumber);
		}
		return string.Format(Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_languageString), m_markNumber);
	}

	public string GetNameTranslated()
	{
		string internalText = GetInternalText();
		if (m_useSubstitutionText)
		{
			return $"{internalText} ({Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_substitutionText)})";
		}
		return internalText;
	}

	public string GetDescriptionStringTranslated()
	{
		return Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_descriptionString);
	}

	public int GetMarkNumber()
	{
		return m_markNumber;
	}

	public Texture2D GetDisplayIcon()
	{
		return m_displayIcon;
	}

	public GameObject GetPrefabToPlace()
	{
		return m_prefabPlaceable;
	}
}
