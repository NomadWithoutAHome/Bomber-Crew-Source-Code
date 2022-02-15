using BomberCrewCommon;
using UnityEngine;

[CreateAssetMenu(menuName = "Bomber Crew/Crewmen Gear/Standard")]
public class CrewmanEquipmentBase : ScriptableObject
{
	[SerializeField]
	[NamedText]
	private string m_namedTextReference;

	[SerializeField]
	private int m_markNumber;

	[SerializeField]
	private int m_armourAddition;

	[SerializeField]
	private CrewmanGearType m_gearType;

	[SerializeField]
	private int m_oxygenTimerIncrease;

	[SerializeField]
	private int m_temperatureResistance;

	[SerializeField]
	private float m_mobilityMultiplier;

	[SerializeField]
	private int m_survivalPointsLand;

	[SerializeField]
	private int m_survivalPointsSea;

	[SerializeField]
	private int m_cost;

	[SerializeField]
	private int m_intelUnlock;

	[SerializeField]
	[NamedText]
	private string m_descriptionTextReference;

	[SerializeField]
	private Crewman.SpecialisationSkill[] m_lockedToSkills;

	[SerializeField]
	private bool m_hideHair;

	[SerializeField]
	private bool m_hideFacialHair;

	[SerializeField]
	private GameObject m_model;

	[SerializeField]
	private bool m_isANone;

	[SerializeField]
	private int m_textureOffsetIndex;

	[SerializeField]
	private bool m_isDLC;

	[SerializeField]
	private string m_DLCIconName;

	[SerializeField]
	private string m_DLCIconNameConsole;

	[SerializeField]
	private bool m_isGiftIcon;

	[SerializeField]
	private int m_sortBump;

	public bool IsAvailableForCrewman(Crewman cr)
	{
		if (IsAvailableForSkill(cr.GetPrimarySkill().GetSkill()))
		{
			return true;
		}
		if (cr.GetSecondarySkill() != null && IsAvailableForSkill(cr.GetSecondarySkill().GetSkill()))
		{
			return true;
		}
		return false;
	}

	public bool IsDLC()
	{
		return m_isDLC;
	}

	public string GetDLCIconName()
	{
		return m_DLCIconName;
	}

	public int GetSortBump()
	{
		return m_sortBump;
	}

	public bool IsGiftIcon()
	{
		return m_isGiftIcon;
	}

	public bool IsAvailableForSkill(Crewman.SpecialisationSkill sk)
	{
		if (m_lockedToSkills == null || m_lockedToSkills.Length == 0)
		{
			return true;
		}
		Crewman.SpecialisationSkill[] lockedToSkills = m_lockedToSkills;
		foreach (Crewman.SpecialisationSkill specialisationSkill in lockedToSkills)
		{
			if (sk == specialisationSkill)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsAvailableForAllSkills()
	{
		return m_lockedToSkills == null || m_lockedToSkills.Length == 0;
	}

	public CrewmanGearType GetGearType()
	{
		return m_gearType;
	}

	public int GetCost()
	{
		return m_cost;
	}

	public int GetIntelShowRequirement()
	{
		float num = Mathf.Max((float)m_intelUnlock * 0.5f, 500f);
		return Mathf.Max(m_intelUnlock - (int)num, 0);
	}

	public int GetIntelUnlockRequirement()
	{
		return m_intelUnlock;
	}

	public int GetArmourAddition()
	{
		return m_armourAddition;
	}

	public int GetOxygenTimerIncrease()
	{
		return m_oxygenTimerIncrease;
	}

	public int GetTemperatureResistance()
	{
		return m_temperatureResistance;
	}

	public float GetMobilityMultiplier()
	{
		return m_mobilityMultiplier;
	}

	public int GetSurvivalPointsLand()
	{
		return m_survivalPointsLand;
	}

	public int GetSurvivalPointsSea()
	{
		return m_survivalPointsSea;
	}

	public bool ShouldHideHair()
	{
		return m_hideHair;
	}

	public bool ShouldHideFacialHair()
	{
		return m_hideFacialHair;
	}

	public int GetMarkNumber()
	{
		return m_markNumber;
	}

	public StatHelper.StatInfo GetArmourStat()
	{
		return StatHelper.StatInfo.Create(m_armourAddition, null);
	}

	public StatHelper.StatInfo GetSurvivalPointsLandStat()
	{
		return StatHelper.StatInfo.Create(m_survivalPointsLand, null);
	}

	public StatHelper.StatInfo GetSurvivalPointsSeaStat()
	{
		return StatHelper.StatInfo.Create(m_survivalPointsSea, null);
	}

	public StatHelper.StatInfo GetOxygenStat()
	{
		return StatHelper.StatInfo.CreateTime(m_oxygenTimerIncrease, biggerIsBetter: true, null);
	}

	public StatHelper.StatInfo GetTemperatureStat()
	{
		return StatHelper.StatInfo.Create(m_temperatureResistance, null);
	}

	public StatHelper.StatInfo GetSpeedStat()
	{
		return StatHelper.StatInfo.CreatePercent(m_mobilityMultiplier, biggerIsBetter: true, null);
	}

	public string GetNamedTextTranslated()
	{
		return string.Format(Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_namedTextReference), m_markNumber);
	}

	public string GetDescription()
	{
		return m_descriptionTextReference;
	}

	public Mesh GetMesh()
	{
		if (m_model != null)
		{
			return m_model.GetComponent<SkinnedMeshRenderer>().sharedMesh;
		}
		return null;
	}

	public Material[] GetMaterials()
	{
		if (m_model != null)
		{
			return m_model.GetComponent<SkinnedMeshRenderer>().sharedMaterials;
		}
		return null;
	}

	public int GetTextureOffsetIndex()
	{
		return m_textureOffsetIndex;
	}
}
