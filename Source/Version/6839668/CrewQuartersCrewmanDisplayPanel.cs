using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class CrewQuartersCrewmanDisplayPanel : MonoBehaviour
{
	[SerializeField]
	private TextSetter m_nameField;

	[SerializeField]
	private TextSetter m_rankText;

	[SerializeField]
	private tk2dTextMesh m_detailsText;

	[SerializeField]
	[NamedText]
	private string m_ageTextFieldPrefix;

	[SerializeField]
	[NamedText]
	private string m_traitsTextFieldPrefix;

	[SerializeField]
	[NamedText]
	private string m_hometownTextFieldPrefix;

	[SerializeField]
	private CrewmanDualSkillDisplay m_skillsDisplay;

	[SerializeField]
	private TextSetter m_movementAbilityField;

	[SerializeField]
	private TextSetter m_movementAbilityFieldChange;

	[SerializeField]
	private TextSetter m_armourField;

	[SerializeField]
	private TextSetter m_armourFieldChange;

	[SerializeField]
	private TextSetter m_temperatureResistField;

	[SerializeField]
	private TextSetter m_temperatureResistFieldChange;

	[SerializeField]
	private TextSetter m_oxygenField;

	[SerializeField]
	private TextSetter m_oxygenFieldChange;

	[SerializeField]
	private TextSetter m_survivalLandField;

	[SerializeField]
	private TextSetter m_survivalLandFieldChange;

	[SerializeField]
	private TextSetter m_survivalSeaField;

	[SerializeField]
	private TextSetter m_survivalSeaFieldChange;

	[SerializeField]
	private tk2dUIItem m_editButton;

	[SerializeField]
	private GameObject m_editHierarchy;

	[SerializeField]
	private GameObject m_nameTextHierarchy;

	[SerializeField]
	private Vector3 m_namePositionNoEdit = new Vector3(12f, -64f, 0f);

	private Crewman m_crewman;

	public void SetUp(Crewman crewman, SelectableFilterButton nameButton)
	{
		m_crewman = crewman;
		SetName();
		string empty = string.Empty;
		empty += $"{Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_ageTextFieldPrefix)} {crewman.GetAge()}";
		empty = empty + "\n" + $"{Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_traitsTextFieldPrefix)} {Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(crewman.GetTraitsNamedText())}";
		empty = empty + "\n" + $"{Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_hometownTextFieldPrefix)} {crewman.GetHomeTown()}";
		m_detailsText.text = empty;
		m_skillsDisplay.SetUp(crewman);
		if (nameButton == null)
		{
			m_editHierarchy.SetActive(value: false);
			m_nameTextHierarchy.transform.localPosition = m_namePositionNoEdit;
		}
		if (m_editButton != null)
		{
			m_editButton.OnClick += delegate
			{
				m_crewman.DoEdit(delegate
				{
					SetName();
					if (nameButton != null)
					{
						CrewTrainingCrewmanNameButton component = nameButton.GetComponent<CrewTrainingCrewmanNameButton>();
						if (component != null)
						{
							component.Refresh();
						}
						CrewQuartersCrewmanNameButton component2 = nameButton.GetComponent<CrewQuartersCrewmanNameButton>();
						if (component2 != null)
						{
							component2.Refresh();
						}
					}
					GameObject crewmanAvatar = Singleton<AirbaseNavigation>.Instance.GetPersistentCrew().GetCrewmanAvatar(m_crewman);
					crewmanAvatar.GetComponentInChildren<CrewmanGraphics>().SetFromCrewman(m_crewman);
				});
			};
		}
		Refresh((CrewmanEquipmentBase)null);
	}

	private void SetName()
	{
		string text = Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(Singleton<CrewmanSkillUpgradeInfo>.Instance.GetNamedText(m_crewman.GetPrimarySkill().GetSkill())).ToUpper();
		m_rankText.SetText(text + " - " + m_crewman.GetCrewmanRankTranslated());
		string text2 = $"{m_crewman.GetFirstName()}\n{m_crewman.GetSurname().ToUpper()}";
		m_nameField.SetText(text2);
	}

	public void Refresh(CrewmanEquipmentPreset withPresetSelected)
	{
		if (withPresetSelected != null && withPresetSelected.GetIntelUnlockRequirement() > Singleton<SaveDataContainer>.Instance.Get().GetIntel())
		{
			withPresetSelected = null;
		}
		List<CrewmanEquipmentBase> allEquipment = m_crewman.GetAllEquipment();
		StatHelper.StatInfo statInfo = StatHelper.StatInfo.CreatePercent(1f, biggerIsBetter: true, null);
		StatHelper.StatInfo statInfo2 = StatHelper.StatInfo.Create(0f, null);
		StatHelper.StatInfo statInfo3 = StatHelper.StatInfo.Create(0f, null);
		StatHelper.StatInfo statInfo4 = StatHelper.StatInfo.CreateTime(0f, biggerIsBetter: true, null);
		StatHelper.StatInfo statInfo5 = StatHelper.StatInfo.Create(0f, null);
		StatHelper.StatInfo statInfo6 = StatHelper.StatInfo.Create(0f, null);
		StatHelper.StatInfo statInfo7 = StatHelper.StatInfo.CreatePercent(1f, biggerIsBetter: true, null);
		StatHelper.StatInfo statInfo8 = StatHelper.StatInfo.Create(0f, null);
		StatHelper.StatInfo statInfo9 = StatHelper.StatInfo.Create(0f, null);
		StatHelper.StatInfo statInfo10 = StatHelper.StatInfo.CreateTime(0f, biggerIsBetter: true, null);
		StatHelper.StatInfo statInfo11 = StatHelper.StatInfo.Create(0f, null);
		StatHelper.StatInfo statInfo12 = StatHelper.StatInfo.Create(0f, null);
		foreach (CrewmanEquipmentBase item in allEquipment)
		{
			if (!(item != null))
			{
				continue;
			}
			statInfo.m_value *= item.GetMobilityMultiplier();
			statInfo2.m_value += item.GetTemperatureResistance();
			statInfo3.m_value += item.GetArmourAddition();
			statInfo4.m_value += item.GetOxygenTimerIncrease();
			statInfo5.m_value += item.GetSurvivalPointsLand();
			statInfo6.m_value += item.GetSurvivalPointsSea();
			CrewmanEquipmentBase crewmanEquipmentBase = item;
			if (withPresetSelected != null)
			{
				CrewmanEquipmentBase[] allInPreset = withPresetSelected.GetAllInPreset();
				CrewmanEquipmentBase[] array = allInPreset;
				foreach (CrewmanEquipmentBase crewmanEquipmentBase2 in array)
				{
					if (crewmanEquipmentBase2.GetGearType() == item.GetGearType())
					{
						crewmanEquipmentBase = crewmanEquipmentBase2;
					}
				}
			}
			statInfo7.m_value *= crewmanEquipmentBase.GetMobilityMultiplier();
			statInfo8.m_value += crewmanEquipmentBase.GetTemperatureResistance();
			statInfo9.m_value += crewmanEquipmentBase.GetArmourAddition();
			statInfo10.m_value += crewmanEquipmentBase.GetOxygenTimerIncrease();
			statInfo11.m_value += crewmanEquipmentBase.GetSurvivalPointsLand();
			statInfo12.m_value += crewmanEquipmentBase.GetSurvivalPointsSea();
		}
		statInfo.m_value *= m_crewman.GetTrait().GetMovementSpeedMultiplier();
		statInfo7.m_value *= m_crewman.GetTrait().GetMovementSpeedMultiplier();
		statInfo2.m_value += m_crewman.GetTrait().GetTemperatureResistBoost();
		statInfo8.m_value += m_crewman.GetTrait().GetTemperatureResistBoost();
		statInfo9.m_value += m_crewman.GetTrait().GetDefenseBoost();
		statInfo3.m_value += m_crewman.GetTrait().GetDefenseBoost();
		statInfo4.m_value += m_crewman.GetTrait().GetOxygenTimerBoost();
		statInfo10.m_value += m_crewman.GetTrait().GetOxygenTimerBoost();
		statInfo5.m_value += m_crewman.GetTrait().GetSurvivalLandBoost();
		statInfo11.m_value += m_crewman.GetTrait().GetSurvivalLandBoost();
		statInfo6.m_value += m_crewman.GetTrait().GetSurvivalSeaBoost();
		statInfo12.m_value += m_crewman.GetTrait().GetSurvivalSeaBoost();
		Singleton<StatHelper>.Instance.SetStatStringComparePreviewChange(statInfo, statInfo7, "{0}", "({0})", m_movementAbilityField, m_movementAbilityFieldChange);
		Singleton<StatHelper>.Instance.SetStatStringComparePreviewChange(statInfo2, statInfo8, "{0}", "({0})", m_temperatureResistField, m_temperatureResistFieldChange);
		Singleton<StatHelper>.Instance.SetStatStringComparePreviewChange(statInfo3, statInfo9, "{0}", "({0})", m_armourField, m_armourFieldChange);
		Singleton<StatHelper>.Instance.SetStatStringComparePreviewChange(statInfo4, statInfo10, "{0}", "({0})", m_oxygenField, m_oxygenFieldChange);
		Singleton<StatHelper>.Instance.SetStatStringComparePreviewChange(statInfo5, statInfo11, "{0}", "({0})", m_survivalLandField, m_survivalLandFieldChange);
		Singleton<StatHelper>.Instance.SetStatStringComparePreviewChange(statInfo6, statInfo12, "{0}", "({0})", m_survivalSeaField, m_survivalSeaFieldChange);
	}

	public void Refresh(CrewmanEquipmentBase withEquipmentSelected)
	{
		List<CrewmanEquipmentBase> allEquipment = m_crewman.GetAllEquipment();
		if (withEquipmentSelected != null && withEquipmentSelected.GetIntelUnlockRequirement() > Singleton<SaveDataContainer>.Instance.Get().GetIntel())
		{
			withEquipmentSelected = null;
		}
		StatHelper.StatInfo statInfo = StatHelper.StatInfo.CreatePercent(1f, biggerIsBetter: true, null);
		StatHelper.StatInfo statInfo2 = StatHelper.StatInfo.Create(0f, null);
		StatHelper.StatInfo statInfo3 = StatHelper.StatInfo.Create(0f, null);
		StatHelper.StatInfo statInfo4 = StatHelper.StatInfo.CreateTime(0f, biggerIsBetter: true, null);
		StatHelper.StatInfo statInfo5 = StatHelper.StatInfo.Create(0f, null);
		StatHelper.StatInfo statInfo6 = StatHelper.StatInfo.Create(0f, null);
		StatHelper.StatInfo statInfo7 = StatHelper.StatInfo.CreatePercent(1f, biggerIsBetter: true, null);
		StatHelper.StatInfo statInfo8 = StatHelper.StatInfo.Create(0f, null);
		StatHelper.StatInfo statInfo9 = StatHelper.StatInfo.Create(0f, null);
		StatHelper.StatInfo statInfo10 = StatHelper.StatInfo.CreateTime(0f, biggerIsBetter: true, null);
		StatHelper.StatInfo statInfo11 = StatHelper.StatInfo.Create(0f, null);
		StatHelper.StatInfo statInfo12 = StatHelper.StatInfo.Create(0f, null);
		foreach (CrewmanEquipmentBase item in allEquipment)
		{
			if (item != null)
			{
				statInfo.m_value *= item.GetMobilityMultiplier();
				statInfo2.m_value += item.GetTemperatureResistance();
				statInfo3.m_value += item.GetArmourAddition();
				statInfo4.m_value += item.GetOxygenTimerIncrease();
				statInfo5.m_value += item.GetSurvivalPointsLand();
				statInfo6.m_value += item.GetSurvivalPointsSea();
				CrewmanEquipmentBase crewmanEquipmentBase = item;
				if (withEquipmentSelected != null && withEquipmentSelected.GetGearType() == item.GetGearType())
				{
					crewmanEquipmentBase = withEquipmentSelected;
				}
				statInfo7.m_value *= crewmanEquipmentBase.GetMobilityMultiplier();
				statInfo8.m_value += crewmanEquipmentBase.GetTemperatureResistance();
				statInfo9.m_value += crewmanEquipmentBase.GetArmourAddition();
				statInfo10.m_value += crewmanEquipmentBase.GetOxygenTimerIncrease();
				statInfo11.m_value += crewmanEquipmentBase.GetSurvivalPointsLand();
				statInfo12.m_value += crewmanEquipmentBase.GetSurvivalPointsSea();
			}
		}
		statInfo.m_value *= m_crewman.GetTrait().GetMovementSpeedMultiplier();
		statInfo7.m_value *= m_crewman.GetTrait().GetMovementSpeedMultiplier();
		statInfo2.m_value += m_crewman.GetTrait().GetTemperatureResistBoost();
		statInfo8.m_value += m_crewman.GetTrait().GetTemperatureResistBoost();
		statInfo9.m_value += m_crewman.GetTrait().GetDefenseBoost();
		statInfo3.m_value += m_crewman.GetTrait().GetDefenseBoost();
		statInfo4.m_value += m_crewman.GetTrait().GetOxygenTimerBoost();
		statInfo10.m_value += m_crewman.GetTrait().GetOxygenTimerBoost();
		statInfo5.m_value += m_crewman.GetTrait().GetSurvivalLandBoost();
		statInfo11.m_value += m_crewman.GetTrait().GetSurvivalLandBoost();
		statInfo6.m_value += m_crewman.GetTrait().GetSurvivalSeaBoost();
		statInfo12.m_value += m_crewman.GetTrait().GetSurvivalSeaBoost();
		Singleton<StatHelper>.Instance.SetStatStringComparePreviewChange(statInfo, statInfo7, "{0}", "({0})", m_movementAbilityField, m_movementAbilityFieldChange);
		Singleton<StatHelper>.Instance.SetStatStringComparePreviewChange(statInfo2, statInfo8, "{0}", "({0})", m_temperatureResistField, m_temperatureResistFieldChange);
		Singleton<StatHelper>.Instance.SetStatStringComparePreviewChange(statInfo3, statInfo9, "{0}", "({0})", m_armourField, m_armourFieldChange);
		Singleton<StatHelper>.Instance.SetStatStringComparePreviewChange(statInfo4, statInfo10, "{0}", "({0})", m_oxygenField, m_oxygenFieldChange);
		Singleton<StatHelper>.Instance.SetStatStringComparePreviewChange(statInfo5, statInfo11, "{0}", "({0})", m_survivalLandField, m_survivalLandFieldChange);
		Singleton<StatHelper>.Instance.SetStatStringComparePreviewChange(statInfo6, statInfo12, "{0}", "({0})", m_survivalSeaField, m_survivalSeaFieldChange);
	}
}
