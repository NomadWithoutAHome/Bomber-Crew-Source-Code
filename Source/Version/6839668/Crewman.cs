using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using BomberCrewCommon;
using Newtonsoft.Json;
using UnityEngine;

[JsonObject(MemberSerialization.OptIn)]
public class Crewman
{
	public enum SpecialisationSkill
	{
		Piloting,
		Gunning,
		Navigator,
		RadioOp,
		Engineer,
		BombAiming,
		FirstAid,
		FireFighting
	}

	public enum ModelType
	{
		Male,
		Female
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class Skill
	{
		[JsonProperty]
		private SpecialisationSkill m_skillType;

		[JsonProperty]
		private int m_currentLevel;

		[JsonProperty]
		private int m_currentXP;

		public Skill(SpecialisationSkill skill, int level)
		{
			m_currentLevel = level;
			m_skillType = skill;
			m_currentXP = 0;
		}

		public SpecialisationSkill GetSkill()
		{
			return m_skillType;
		}

		public int GetLevel()
		{
			return m_currentLevel;
		}

		public float GetLevelNormalised()
		{
			return (float)m_currentLevel / (float)GetMaxLevel();
		}

		public int GetMaxLevel()
		{
			SpecialisationSkill skillType = m_skillType;
			if (skillType == SpecialisationSkill.FirstAid || skillType == SpecialisationSkill.FireFighting)
			{
				return 4;
			}
			return 12;
		}

		public int GetXP()
		{
			return m_currentXP;
		}

		public void ResetXP()
		{
			m_currentXP = 0;
			m_currentLevel = 1;
		}

		public float GetXPNormalised()
		{
			if (m_currentLevel >= GetMaxLevel())
			{
				return 1f;
			}
			return (float)m_currentXP / (float)Singleton<XPRequirements>.Instance.GetXPRequiredForLevel(m_currentLevel);
		}

		public void AddXP(int xpToAdd)
		{
			m_currentXP += xpToAdd;
			while (m_currentXP > Singleton<XPRequirements>.Instance.GetXPRequiredForLevel(m_currentLevel) && m_currentLevel < GetMaxLevel())
			{
				m_currentXP -= Singleton<XPRequirements>.Instance.GetXPRequiredForLevel(m_currentLevel);
				m_currentLevel++;
			}
		}
	}

	[JsonProperty]
	private string m_firstName;

	[JsonProperty]
	private string m_secondName;

	[JsonProperty]
	private CrewmanTraitJS m_trait;

	[JsonProperty]
	private string m_homeTown;

	[JsonProperty]
	private bool m_isNew;

	[JsonProperty]
	private Skill m_primarySkill;

	[JsonProperty]
	private Skill m_secondarySkill;

	[JsonProperty]
	private ModelType m_modelType;

	[JsonProperty]
	private Dictionary<CrewmanGearType, string> m_currentlyEquippedItems = new Dictionary<CrewmanGearType, string>();

	[JsonProperty]
	private bool m_isDead;

	[JsonProperty]
	private int m_civilianClothingIndex;

	[JsonProperty]
	private int m_eyeColor;

	[JsonProperty]
	private int m_hairColor;

	[JsonProperty]
	private int m_mouthType;

	[JsonProperty]
	private int m_skinTone;

	[JsonProperty]
	private int m_hairStyle;

	[JsonProperty]
	private int m_facialHairStyle;

	[JsonProperty]
	private int m_numberOfMissionsPlayed;

	[JsonProperty]
	private int m_numberOfMissionsSuccessful;

	[JsonProperty]
	private int m_age;

	[JsonProperty]
	private int m_voiceCode;

	private RenderTexture m_portraitRenderTexture;

	private static int s_createdIndex;

	private List<CrewmanEquipmentBase> m_equipmentCache = new List<CrewmanEquipmentBase>();

	private bool m_cacheValid;

	private string m_cachedJabberEvent;

	private bool m_runningConstructor;

	public Crewman(Skill primarySkill, Skill secondarySkill)
	{
		m_runningConstructor = true;
		m_modelType = (ModelType)(s_createdIndex % 2);
		s_createdIndex++;
		bool useUSNaming = Singleton<GameFlow>.Instance.GetGameMode().GetUseUSNaming();
		m_firstName = Singleton<NameAndBackstoryGenerator>.Instance.GenerateFirstName(m_modelType == ModelType.Female, useUSNaming);
		m_secondName = Singleton<NameAndBackstoryGenerator>.Instance.GenerateSurame(useUSNaming);
		m_firstName = m_firstName.Replace("\n", string.Empty);
		m_firstName = m_firstName.Replace("\r", string.Empty);
		m_secondName = m_secondName.Replace("\n", string.Empty);
		m_secondName = m_secondName.Replace("\r", string.Empty);
		m_homeTown = Singleton<NameAndBackstoryGenerator>.Instance.GenerateTown(useUSNaming);
		m_trait = Singleton<NameAndBackstoryGenerator>.Instance.GenerateTrait(useUSNaming);
		m_age = Singleton<NameAndBackstoryGenerator>.Instance.GetAge();
		m_civilianClothingIndex = UnityEngine.Random.Range(0, Singleton<CrewmanAppearanceCatalogue>.Instance.GetMaxCivviesIndex());
		m_eyeColor = UnityEngine.Random.Range(0, Singleton<CrewmanAppearanceCatalogue>.Instance.GetIndexCountEyes());
		m_mouthType = UnityEngine.Random.Range(0, Singleton<CrewmanAppearanceCatalogue>.Instance.GetIndexCountMouth());
		m_skinTone = UnityEngine.Random.Range(0, Singleton<CrewmanAppearanceCatalogue>.Instance.GetIndexCountSkin());
		m_hairStyle = UnityEngine.Random.Range(0, Singleton<CrewmanAppearanceCatalogue>.Instance.GetNumHairVariation(m_modelType));
		if (Singleton<CrewmanAppearanceCatalogue>.Instance.GetHairMesh(m_modelType, m_hairStyle) == null)
		{
			m_hairColor = UnityEngine.Random.Range(0, Singleton<CrewmanAppearanceCatalogue>.Instance.GetIndexCountHair());
		}
		else
		{
			m_hairColor = UnityEngine.Random.Range(1, Singleton<CrewmanAppearanceCatalogue>.Instance.GetIndexCountHair());
		}
		m_voiceCode = UnityEngine.Random.Range(0, Singleton<CrewmanAppearanceCatalogue>.Instance.GetMaxJabbers(m_modelType));
		if (m_modelType == ModelType.Male)
		{
			m_facialHairStyle = UnityEngine.Random.Range(0, Singleton<CrewmanAppearanceCatalogue>.Instance.GetNumFacialHairVariation());
		}
		m_isNew = true;
		m_primarySkill = primarySkill;
		m_secondarySkill = secondarySkill;
		foreach (Enum value in Enum.GetValues(typeof(CrewmanGearType)))
		{
			CrewmanGearType crewmanGearType = (CrewmanGearType)(object)value;
			SetEquippedFor(crewmanGearType, Singleton<CrewmanGearCatalogueLoader>.Instance.GetCatalogue().GetDefaultByType(crewmanGearType));
		}
		m_runningConstructor = false;
		RefreshPortrait();
	}

	public Crewman()
	{
	}

	public void SetTrait(CrewmanTrait trait)
	{
		m_trait = new CrewmanTraitJS(trait);
	}

	[OnDeserialized]
	private void OnDeserializedMethod(StreamingContext context)
	{
		m_firstName = m_firstName.Replace("\n", string.Empty);
		m_firstName = m_firstName.Replace("\r", string.Empty);
		m_secondName = m_secondName.Replace("\n", string.Empty);
		m_secondName = m_secondName.Replace("\r", string.Empty);
	}

	public string GetJabberEvent()
	{
		if (string.IsNullOrEmpty(m_cachedJabberEvent))
		{
			m_cachedJabberEvent = Singleton<CrewmanAppearanceCatalogue>.Instance.GetJabberEvent(m_modelType, m_voiceCode);
		}
		return m_cachedJabberEvent;
	}

	public ModelType GetModelType()
	{
		return m_modelType;
	}

	public void SetMissionFinished(bool success)
	{
		m_numberOfMissionsPlayed++;
		if (success)
		{
			m_numberOfMissionsSuccessful++;
		}
	}

	public int GetMissionsPlayed()
	{
		return m_numberOfMissionsPlayed;
	}

	public int GetMissionsSuccessful()
	{
		return m_numberOfMissionsSuccessful;
	}

	public int GetAge()
	{
		return m_age;
	}

	public CrewmanTraitJS GetTrait()
	{
		return m_trait;
	}

	public int GetCivilianClothing()
	{
		return m_civilianClothingIndex;
	}

	public int GetEyeColor()
	{
		return m_eyeColor;
	}

	public int GetMouthType()
	{
		return m_mouthType;
	}

	public int GetSkinTone()
	{
		return m_skinTone;
	}

	public int GetHairColor()
	{
		return m_hairColor;
	}

	public int GetHairVariation()
	{
		return m_hairStyle;
	}

	public void SetHairVariation(int index)
	{
		m_hairStyle = index;
	}

	public void SetHairColor(int index)
	{
		m_hairColor = index;
	}

	public void SetSkinTone(int index)
	{
		m_skinTone = index;
	}

	public void SetMouthType(int index)
	{
		m_mouthType = index;
	}

	public void SetEyes(int index)
	{
		m_eyeColor = index;
	}

	public void ChangeBodyType()
	{
		if (m_modelType == ModelType.Female)
		{
			m_modelType = ModelType.Male;
		}
		else
		{
			m_modelType = ModelType.Female;
		}
		m_cachedJabberEvent = Singleton<CrewmanAppearanceCatalogue>.Instance.GetJabberEvent(m_modelType, m_voiceCode);
	}

	public int GetFacialHairVariation()
	{
		return m_facialHairStyle;
	}

	public void SetFacialHairVariation(int val)
	{
		m_facialHairStyle = val;
	}

	public bool IsDead()
	{
		return m_isDead;
	}

	public void MagicallyResurrect()
	{
		m_isDead = false;
	}

	public void SetDead()
	{
		m_isDead = true;
	}

	public Skill GetPrimarySkill()
	{
		return m_primarySkill;
	}

	public Skill GetSecondarySkill()
	{
		return m_secondarySkill;
	}

	public void TrainSecondarySkill(SpecialisationSkill skill)
	{
		m_secondarySkill = new Skill(skill, 1);
	}

	public void RemoveSecondarySkill()
	{
		m_secondarySkill = null;
	}

	public int GetCrewmanRank()
	{
		return Singleton<CrewmanSkillUpgradeInfo>.Instance.GetRank(this);
	}

	public string GetCrewmanRankTranslated()
	{
		return Singleton<CrewmanSkillUpgradeInfo>.Instance.GetRankTranslated(this);
	}

	public CrewmanEquipmentBase GetEquippedFor(CrewmanGearType typeBase)
	{
		if (m_currentlyEquippedItems == null)
		{
			m_currentlyEquippedItems = new Dictionary<CrewmanGearType, string>();
		}
		CrewmanEquipmentBase crewmanEquipmentBase = null;
		string value = null;
		m_currentlyEquippedItems.TryGetValue(typeBase, out value);
		if (!string.IsNullOrEmpty(value))
		{
			crewmanEquipmentBase = Singleton<CrewmanGearCatalogueLoader>.Instance.GetCatalogue().GetByName(value);
		}
		if (crewmanEquipmentBase == null)
		{
			DebugLogWrapper.LogError("NO EQUIPMENT FOUND FOR: " + value + " " + typeBase);
			return Singleton<CrewmanGearCatalogueLoader>.Instance.GetCatalogue().GetDefaultByType(typeBase);
		}
		return crewmanEquipmentBase;
	}

	public void DoEdit(Action OnEditFinished)
	{
		UIPopupData uIPopupData = new UIPopupData();
		uIPopupData.PopupStartCallback = (Action<UIPopUp>)Delegate.Combine(uIPopupData.PopupStartCallback, (Action<UIPopUp>)delegate(UIPopUp uip)
		{
			uip.GetComponent<PopupEditCharacter>().SetUp(this);
		});
		uIPopupData.PopupDismissedCallback = (Action<UIPopUp>)Delegate.Combine(uIPopupData.PopupDismissedCallback, (Action<UIPopUp>)delegate
		{
			if (OnEditFinished != null)
			{
				OnEditFinished();
			}
		});
		Singleton<UIPopupManager>.Instance.DisplayPopup(Singleton<CrewmanAppearanceCatalogue>.Instance.GetEditPrefab(), uIPopupData);
	}

	public void ClearEquipmentCache()
	{
		m_cacheValid = false;
	}

	public List<CrewmanEquipmentBase> GetAllEquipment()
	{
		if (!m_cacheValid)
		{
			m_equipmentCache.Clear();
			foreach (Enum value in Enum.GetValues(typeof(CrewmanGearType)))
			{
				CrewmanGearType typeBase = (CrewmanGearType)(object)value;
				m_equipmentCache.Add(GetEquippedFor(typeBase));
			}
			m_cacheValid = true;
		}
		return m_equipmentCache;
	}

	public void SetEquippedFor(CrewmanGearType typeBase, CrewmanEquipmentBase equipment)
	{
		if (m_currentlyEquippedItems == null)
		{
			m_currentlyEquippedItems = new Dictionary<CrewmanGearType, string>();
		}
		if (Singleton<SaveDataContainer>.Instance.Get() != null)
		{
			Singleton<SaveDataContainer>.Instance.Get().SetViewed(equipment.name);
		}
		m_currentlyEquippedItems[typeBase] = equipment.name;
		m_cacheValid = false;
		RefreshPortrait();
	}

	public void RefreshPortrait()
	{
		if (!m_runningConstructor)
		{
			m_firstName = m_firstName.Replace("\n", string.Empty);
			m_firstName = m_firstName.Replace("\r", string.Empty);
			m_secondName = m_secondName.Replace("\n", string.Empty);
			m_secondName = m_secondName.Replace("\r", string.Empty);
			m_portraitRenderTexture = Singleton<CrewmanPhotoBooth>.Instance.RenderForCrewman(this, m_portraitRenderTexture);
		}
	}

	public void SetSeen()
	{
		m_isNew = false;
	}

	public bool IsNewRecruit()
	{
		return m_isNew;
	}

	public RenderTexture GetPortraitPictureTexture()
	{
		if (m_portraitRenderTexture == null)
		{
			RefreshPortrait();
		}
		return m_portraitRenderTexture;
	}

	public void SetFirstName(string newName)
	{
		m_firstName = newName;
	}

	public void SetSurname(string newName)
	{
		m_secondName = newName;
	}

	public string GetFirstName()
	{
		return m_firstName;
	}

	public string GetSurname()
	{
		return m_secondName;
	}

	public string GetInitialAndSurname()
	{
		return m_firstName[0] + "." + m_secondName;
	}

	public string GetFullName()
	{
		return m_firstName + " " + m_secondName;
	}

	public string GetTraitsNamedText()
	{
		if (m_trait == null)
		{
			return "*NULL*";
		}
		return m_trait.GetNamedText();
	}

	public string GetHomeTown()
	{
		return m_homeTown;
	}

	public int GetArmourTotal()
	{
		int num = 0;
		foreach (CrewmanEquipmentBase item in GetAllEquipment())
		{
			num += item.GetArmourAddition();
		}
		if (m_trait != null)
		{
			num += m_trait.GetDefenseBoost();
		}
		return num;
	}

	public float GetMovementFactor()
	{
		float num = 1f;
		foreach (CrewmanEquipmentBase item in GetAllEquipment())
		{
			num *= item.GetMobilityMultiplier();
		}
		if (m_trait != null)
		{
			num *= m_trait.GetMovementSpeedMultiplier();
		}
		return num;
	}

	public int GetSurvivalLand()
	{
		int num = 0;
		foreach (CrewmanEquipmentBase item in GetAllEquipment())
		{
			num += item.GetSurvivalPointsLand();
		}
		if (m_trait != null)
		{
			num += m_trait.GetSurvivalLandBoost();
		}
		return num;
	}

	public int GetSurvivalSea()
	{
		int num = 0;
		foreach (CrewmanEquipmentBase item in GetAllEquipment())
		{
			num += item.GetSurvivalPointsSea();
		}
		if (m_trait != null)
		{
			num += m_trait.GetSurvivalSeaBoost();
		}
		return num;
	}

	public int GetTemperatureResistance()
	{
		int num = 0;
		foreach (CrewmanEquipmentBase item in GetAllEquipment())
		{
			num += item.GetTemperatureResistance();
		}
		if (m_trait != null)
		{
			num += m_trait.GetTemperatureResistBoost();
		}
		return num;
	}

	public int GetOxygenTime()
	{
		int num = 0;
		foreach (CrewmanEquipmentBase item in GetAllEquipment())
		{
			num += item.GetOxygenTimerIncrease();
		}
		if (m_trait != null)
		{
			num += m_trait.GetOxygenTimerBoost();
		}
		return num;
	}
}
