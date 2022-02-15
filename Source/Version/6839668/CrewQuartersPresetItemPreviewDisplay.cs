using System;
using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class CrewQuartersPresetItemPreviewDisplay : MonoBehaviour
{
	[SerializeField]
	private TextSetter m_title;

	[SerializeField]
	private TextSetter m_description;

	[SerializeField]
	private TextSetter m_armourStat;

	[SerializeField]
	private TextSetter m_armourStatChange;

	[SerializeField]
	private TextSetter m_speedStat;

	[SerializeField]
	private TextSetter m_speedStatChange;

	[SerializeField]
	private TextSetter m_oxygenStat;

	[SerializeField]
	private TextSetter m_oxygenStatChange;

	[SerializeField]
	private TextSetter m_temperatureStat;

	[SerializeField]
	private TextSetter m_temperatureStatChange;

	[SerializeField]
	private TextSetter m_survivalLandStat;

	[SerializeField]
	private TextSetter m_survivalLandChange;

	[SerializeField]
	private TextSetter m_survivalSeaStat;

	[SerializeField]
	private TextSetter m_survivalSeaChange;

	[SerializeField]
	private GameObject m_equippedHierarchy;

	[SerializeField]
	private GameObject m_unequippedHierarchy;

	[SerializeField]
	[NamedText]
	private string m_languageStringEquipCost;

	[SerializeField]
	[NamedText]
	private string m_languageStringEquipInInventory;

	[SerializeField]
	[NamedText]
	private string m_languageStringEquipCostRefund;

	[SerializeField]
	[NamedText]
	private string m_languageStringEquipAllRefund;

	[SerializeField]
	private SelectableFilterButton m_equipButton;

	private CrewmanEquipmentPreset m_equipmentPreset;

	private Crewman m_forCrewman;

	[SerializeField]
	[NamedText]
	private string m_languageStringEquipAll = "ui_crewquarters_equipall_cost";

	[SerializeField]
	private GameObject m_equipToAllHierarchy;

	[SerializeField]
	private GameObject m_equippedToAllHierarchy;

	[SerializeField]
	private SelectableFilterButton m_equipAllButton;

	[SerializeField]
	private GameObject[] m_controlIcons;

	[SerializeField]
	private GameObject m_unlockedHierarchy;

	[SerializeField]
	private GameObject m_lockedHierarchy;

	private bool m_useRefund;

	public event Action OnClick;

	public event Action OnClickAll;

	private void Awake()
	{
		m_equipButton.GetUIItem().OnClick += OnEquipClick;
		m_equipAllButton.GetUIItem().OnClick += OnEquipClickAll;
	}

	private void OnEquipClick()
	{
		if (this.OnClick != null)
		{
			this.OnClick();
		}
	}

	private void OnEquipClickAll()
	{
		if (this.OnClickAll != null)
		{
			this.OnClickAll();
		}
	}

	public void FakeClickPurchaseAll()
	{
		if (Singleton<UISelector>.Instance.IsValid(m_equipAllButton.GetUIItem()))
		{
			m_equipAllButton.GetUIItem().SimulateUpClick();
		}
	}

	public void FakeClickPurchase()
	{
		if (Singleton<UISelector>.Instance.IsValid(m_equipButton.GetUIItem()))
		{
			m_equipButton.GetUIItem().SimulateUpClick();
		}
	}

	public void FakeDownPurchaseAll()
	{
		if (Singleton<UISelector>.Instance.IsValid(m_equipAllButton.GetUIItem()))
		{
			m_equipAllButton.GetUIItem().SimulateDown();
		}
	}

	public void FakeDownPurchase()
	{
		if (Singleton<UISelector>.Instance.IsValid(m_equipButton.GetUIItem()))
		{
			m_equipButton.GetUIItem().SimulateDown();
		}
	}

	public void SetControlIconsEnabled(bool enabled)
	{
		GameObject[] controlIcons = m_controlIcons;
		foreach (GameObject gameObject in controlIcons)
		{
			gameObject.SetActive(enabled);
		}
	}

	public void SetUp(CrewmanEquipmentPreset preset, Crewman crewman, bool useRefund)
	{
		m_useRefund = useRefund;
		m_equipmentPreset = preset;
		m_forCrewman = crewman;
		if (preset != null)
		{
			Singleton<SaveDataContainer>.Instance.Get().SetViewed(preset.name);
		}
		Refresh();
	}

	public void Refresh()
	{
		List<CrewmanEquipmentBase> allEquipment = m_forCrewman.GetAllEquipment();
		CrewmanEquipmentBase[] allInPreset = m_equipmentPreset.GetAllInPreset();
		bool flag = true;
		int num = 0;
		int num2 = 0;
		float num3 = 1f;
		int num4 = 0;
		int num5 = 0;
		int num6 = 0;
		int num7 = 0;
		int num8 = 0;
		float num9 = 1f;
		int num10 = 0;
		int num11 = 0;
		int num12 = 0;
		List<CrewmanEquipmentBase> list = new List<CrewmanEquipmentBase>();
		list.AddRange(m_forCrewman.GetAllEquipment());
		CrewmanEquipmentBase[] array = allInPreset;
		foreach (CrewmanEquipmentBase crewmanEquipmentBase in array)
		{
			if (m_forCrewman.GetEquippedFor(crewmanEquipmentBase.GetGearType()) != Singleton<CrewmanGearCatalogueLoader>.Instance.GetCatalogue().GetByName(crewmanEquipmentBase.name))
			{
				flag = false;
			}
			list.Remove(m_forCrewman.GetEquippedFor(crewmanEquipmentBase.GetGearType()));
			num += crewmanEquipmentBase.GetArmourAddition();
			num2 += crewmanEquipmentBase.GetOxygenTimerIncrease();
			num3 *= crewmanEquipmentBase.GetMobilityMultiplier();
			num4 += crewmanEquipmentBase.GetTemperatureResistance();
			num5 += crewmanEquipmentBase.GetSurvivalPointsLand();
			num6 += crewmanEquipmentBase.GetSurvivalPointsSea();
		}
		foreach (CrewmanEquipmentBase item in list)
		{
			num += item.GetArmourAddition();
			num2 += item.GetOxygenTimerIncrease();
			num3 *= item.GetMobilityMultiplier();
			num4 += item.GetTemperatureResistance();
			num5 += item.GetSurvivalPointsLand();
			num6 += item.GetSurvivalPointsSea();
		}
		foreach (CrewmanEquipmentBase item2 in m_forCrewman.GetAllEquipment())
		{
			num7 += item2.GetArmourAddition();
			num8 += item2.GetOxygenTimerIncrease();
			num9 *= item2.GetMobilityMultiplier();
			num10 += item2.GetTemperatureResistance();
			num11 += item2.GetSurvivalPointsLand();
			num12 += item2.GetSurvivalPointsSea();
		}
		StatHelper.StatInfo left = StatHelper.StatInfo.Create(num, null);
		StatHelper.StatInfo left2 = StatHelper.StatInfo.CreateTime(num2, biggerIsBetter: true, null);
		StatHelper.StatInfo left3 = StatHelper.StatInfo.CreatePercent(num3, biggerIsBetter: true, null);
		StatHelper.StatInfo left4 = StatHelper.StatInfo.Create(num4, null);
		StatHelper.StatInfo left5 = StatHelper.StatInfo.Create(num5, null);
		StatHelper.StatInfo left6 = StatHelper.StatInfo.Create(num6, null);
		StatHelper.StatInfo right = StatHelper.StatInfo.Create(num7, null);
		StatHelper.StatInfo right2 = StatHelper.StatInfo.CreateTime(num8, biggerIsBetter: true, null);
		StatHelper.StatInfo right3 = StatHelper.StatInfo.CreatePercent(num9, biggerIsBetter: true, null);
		StatHelper.StatInfo right4 = StatHelper.StatInfo.Create(num10, null);
		StatHelper.StatInfo right5 = StatHelper.StatInfo.Create(num11, null);
		StatHelper.StatInfo right6 = StatHelper.StatInfo.Create(num12, null);
		if (m_equippedHierarchy != null)
		{
			m_equippedHierarchy.SetActive(flag);
		}
		if (m_unequippedHierarchy != null)
		{
			m_unequippedHierarchy.SetActive(!flag);
		}
		Singleton<StatHelper>.Instance.SetStatStringCompare(left, right, "{0}", "({0})", m_armourStat, m_armourStatChange);
		Singleton<StatHelper>.Instance.SetStatStringCompare(left3, right3, "{0}", "({0})", m_speedStat, m_speedStatChange);
		Singleton<StatHelper>.Instance.SetStatStringCompare(left2, right2, "{0}", "({0})", m_oxygenStat, m_oxygenStatChange);
		Singleton<StatHelper>.Instance.SetStatStringCompare(left4, right4, "{0}", "({0})", m_temperatureStat, m_temperatureStatChange);
		Singleton<StatHelper>.Instance.SetStatStringCompare(left5, right5, "{0}", "({0})", m_survivalLandStat, m_survivalLandChange);
		Singleton<StatHelper>.Instance.SetStatStringCompare(left6, right6, "{0}", "({0})", m_survivalSeaStat, m_survivalSeaChange);
		m_title.SetTextFromLanguageString(m_equipmentPreset.GetPresetName());
		m_description.SetTextFromLanguageString(m_equipmentPreset.GetDescriptionName());
		int cost = m_equipmentPreset.GetCost(m_forCrewman, m_useRefund, null);
		if (cost >= 0)
		{
			m_equipButton.SetText(Singleton<GameFlow>.Instance.GetGameMode().ReplaceCurrencySymbol(string.Format(Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_languageStringEquipCost), cost)));
		}
		else
		{
			m_equipButton.SetText(Singleton<GameFlow>.Instance.GetGameMode().ReplaceCurrencySymbol(string.Format(Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_languageStringEquipCostRefund), -cost)));
		}
		bool disabled = cost > Singleton<SaveDataContainer>.Instance.Get().GetBalance();
		m_equipButton.SetDisabled(disabled);
		if (m_equipmentPreset.IsAvailableForAllSkills())
		{
			int num13 = 0;
			int currentCrewCount = Singleton<CrewContainer>.Instance.GetCurrentCrewCount();
			int num14 = 0;
			Dictionary<string, int> stockUsed = new Dictionary<string, int>();
			for (int j = 0; j < currentCrewCount; j++)
			{
				Crewman crewman = Singleton<CrewContainer>.Instance.GetCrewman(j);
				num13 += m_equipmentPreset.GetCost(crewman, m_useRefund, stockUsed);
				bool flag2 = true;
				CrewmanEquipmentBase[] array2 = allInPreset;
				foreach (CrewmanEquipmentBase crewmanEquipmentBase2 in array2)
				{
					if (crewman.GetEquippedFor(crewmanEquipmentBase2.GetGearType()) != Singleton<CrewmanGearCatalogueLoader>.Instance.GetCatalogue().GetByName(crewmanEquipmentBase2.name))
					{
						flag2 = false;
					}
				}
				if (flag2)
				{
					num14++;
				}
			}
			if (num14 == currentCrewCount)
			{
				m_equippedToAllHierarchy.SetActive(value: true);
				m_equipToAllHierarchy.SetActive(value: false);
			}
			else
			{
				m_equippedToAllHierarchy.SetActive(value: false);
				m_equipToAllHierarchy.SetActive(value: true);
				if (num13 >= 0)
				{
					m_equipAllButton.SetText(string.Format(Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_languageStringEquipAll), num13));
				}
				else
				{
					m_equipAllButton.SetText(string.Format(Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_languageStringEquipAllRefund), -num13));
				}
				m_equipAllButton.SetDisabled(num13 > Singleton<SaveDataContainer>.Instance.Get().GetBalance());
			}
		}
		else
		{
			m_equippedToAllHierarchy.SetActive(value: false);
			m_equipToAllHierarchy.SetActive(value: false);
		}
		if (Singleton<SaveDataContainer>.Instance.Get().GetIntel() >= m_equipmentPreset.GetIntelUnlockRequirement())
		{
			m_unlockedHierarchy.SetActive(value: true);
			m_lockedHierarchy.SetActive(value: false);
		}
		else
		{
			m_unlockedHierarchy.SetActive(value: false);
			m_lockedHierarchy.SetActive(value: true);
		}
	}
}
