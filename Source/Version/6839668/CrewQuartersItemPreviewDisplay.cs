using System;
using BomberCrewCommon;
using UnityEngine;

public class CrewQuartersItemPreviewDisplay : MonoBehaviour
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
	private GameObject m_oxygenStatHierarchy;

	[SerializeField]
	private GameObject m_temperatureStatHierarchy;

	[SerializeField]
	[NamedText]
	private string m_languageStringEquipCost;

	[SerializeField]
	[NamedText]
	private string m_languageStringEquipCostRefund;

	[SerializeField]
	[NamedText]
	private string m_languageStringEquipInInventory;

	[SerializeField]
	[NamedText]
	private string m_languageStringEquipAll = "ui_crewquarters_equipall_cost";

	[SerializeField]
	[NamedText]
	private string m_languageStringEquipAllRefund;

	[SerializeField]
	private SelectableFilterButton m_equipButton;

	[SerializeField]
	private GameObject m_equipToAllHierarchy;

	[SerializeField]
	private GameObject m_equippedToAllHierarchy;

	[SerializeField]
	private SelectableFilterButton m_equipAllButton;

	[SerializeField]
	private GameObject[] m_controlIcons;

	[SerializeField]
	private GameObject m_lockedHierarchy;

	[SerializeField]
	private GameObject m_unlockedHierarchy;

	private CrewmanEquipmentBase m_equipment;

	private Crewman m_forCrewman;

	private bool m_useRefund;

	public event Action OnClick;

	public event Action OnClickAll;

	private void Awake()
	{
		m_equipButton.GetUIItem().OnClick += OnEquipClick;
		m_equipAllButton.GetUIItem().OnClick += OnEquipClickAll;
	}

	public void SetControlIconsEnabled(bool enabled)
	{
		GameObject[] controlIcons = m_controlIcons;
		foreach (GameObject gameObject in controlIcons)
		{
			gameObject.SetActive(enabled);
		}
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

	public void SetUp(CrewmanEquipmentBase equipment, Crewman crewman, bool useRefund)
	{
		m_equipment = equipment;
		m_forCrewman = crewman;
		m_useRefund = useRefund;
		if (equipment != null)
		{
			Singleton<SaveDataContainer>.Instance.Get().SetViewed(equipment.name);
		}
		Refresh();
	}

	public void Refresh()
	{
		CrewmanEquipmentBase equippedFor = m_forCrewman.GetEquippedFor(m_equipment.GetGearType());
		if (m_equippedHierarchy != null)
		{
			m_equippedHierarchy.SetActive(equippedFor == m_equipment);
		}
		if (m_unequippedHierarchy != null)
		{
			m_unequippedHierarchy.SetActive(equippedFor != m_equipment);
		}
		StatHelper.StatInfo armourStat = m_equipment.GetArmourStat();
		StatHelper.StatInfo armourStat2 = equippedFor.GetArmourStat();
		Singleton<StatHelper>.Instance.SetStatStringCompare(armourStat, armourStat2, "{0}", "({0})", m_armourStat, m_armourStatChange);
		StatHelper.StatInfo speedStat = m_equipment.GetSpeedStat();
		StatHelper.StatInfo speedStat2 = equippedFor.GetSpeedStat();
		Singleton<StatHelper>.Instance.SetStatStringCompare(speedStat, speedStat2, "{0}", "({0})", m_speedStat, m_speedStatChange);
		StatHelper.StatInfo oxygenStat = m_equipment.GetOxygenStat();
		StatHelper.StatInfo oxygenStat2 = equippedFor.GetOxygenStat();
		Singleton<StatHelper>.Instance.SetStatStringCompare(oxygenStat, oxygenStat2, "{0}", "({0})", m_oxygenStat, m_oxygenStatChange);
		StatHelper.StatInfo temperatureStat = m_equipment.GetTemperatureStat();
		StatHelper.StatInfo temperatureStat2 = equippedFor.GetTemperatureStat();
		Singleton<StatHelper>.Instance.SetStatStringCompare(temperatureStat, temperatureStat2, "{0}", "({0})", m_temperatureStat, m_temperatureStatChange);
		StatHelper.StatInfo survivalPointsLandStat = m_equipment.GetSurvivalPointsLandStat();
		StatHelper.StatInfo survivalPointsLandStat2 = equippedFor.GetSurvivalPointsLandStat();
		Singleton<StatHelper>.Instance.SetStatStringCompare(survivalPointsLandStat, survivalPointsLandStat2, "{0}", "({0})", m_survivalLandStat, m_survivalLandChange);
		StatHelper.StatInfo survivalPointsSeaStat = m_equipment.GetSurvivalPointsSeaStat();
		StatHelper.StatInfo survivalPointsSeaStat2 = equippedFor.GetSurvivalPointsSeaStat();
		Singleton<StatHelper>.Instance.SetStatStringCompare(survivalPointsSeaStat, survivalPointsSeaStat2, "{0}", "({0})", m_survivalSeaStat, m_survivalSeaChange);
		m_oxygenStatHierarchy.SetActive(m_equipment.GetOxygenTimerIncrease() != 0);
		m_temperatureStatHierarchy.SetActive(m_equipment.GetOxygenTimerIncrease() == 0);
		m_title.SetText(m_equipment.GetNamedTextTranslated());
		m_description.SetTextFromLanguageString(m_equipment.GetDescription());
		int num = m_equipment.GetCost();
		int stockForCrewGear = Singleton<SaveDataContainer>.Instance.Get().GetStockForCrewGear(m_equipment);
		if (stockForCrewGear == 0 || m_useRefund)
		{
			if (m_useRefund)
			{
				num -= equippedFor.GetCost();
			}
			if (num >= 0)
			{
				m_equipButton.SetText(Singleton<GameFlow>.Instance.GetGameMode().ReplaceCurrencySymbol(string.Format(Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_languageStringEquipCost), num)));
			}
			else
			{
				m_equipButton.SetText(Singleton<GameFlow>.Instance.GetGameMode().ReplaceCurrencySymbol(string.Format(Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_languageStringEquipCostRefund), -num)));
			}
			bool disabled = num > Singleton<SaveDataContainer>.Instance.Get().GetBalance();
			m_equipButton.SetDisabled(disabled);
		}
		else
		{
			m_equipButton.SetText(string.Format(Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_languageStringEquipInInventory), stockForCrewGear));
		}
		if (m_equipment.IsAvailableForAllSkills())
		{
			int cost = m_equipment.GetCost();
			int num2 = 0;
			int currentCrewCount = Singleton<CrewContainer>.Instance.GetCurrentCrewCount();
			for (int i = 0; i < currentCrewCount; i++)
			{
				Crewman crewman = Singleton<CrewContainer>.Instance.GetCrewman(i);
				CrewmanEquipmentBase equippedFor2 = crewman.GetEquippedFor(m_equipment.GetGearType());
				if (equippedFor2 == m_equipment)
				{
					num2++;
				}
			}
			if (num2 == currentCrewCount)
			{
				m_equippedToAllHierarchy.SetActive(value: true);
				m_equipToAllHierarchy.SetActive(value: false);
			}
			else
			{
				int num3 = 0;
				if (m_useRefund)
				{
					for (int j = 0; j < currentCrewCount; j++)
					{
						Crewman crewman2 = Singleton<CrewContainer>.Instance.GetCrewman(j);
						CrewmanEquipmentBase equippedFor3 = crewman2.GetEquippedFor(m_equipment.GetGearType());
						num3 -= equippedFor3.GetCost();
						num3 += cost;
					}
				}
				else
				{
					int num4 = currentCrewCount - num2 - stockForCrewGear;
					if (num4 < 0)
					{
						num4 = 0;
					}
					m_equippedToAllHierarchy.SetActive(value: false);
					m_equipToAllHierarchy.SetActive(value: true);
					num3 = num4 * cost;
				}
				if (num3 >= 0)
				{
					m_equipAllButton.SetText(Singleton<GameFlow>.Instance.GetGameMode().ReplaceCurrencySymbol(string.Format(Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_languageStringEquipAll), num3)));
				}
				else
				{
					m_equipAllButton.SetText(Singleton<GameFlow>.Instance.GetGameMode().ReplaceCurrencySymbol(string.Format(Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_languageStringEquipAllRefund), -num3)));
				}
				m_equipAllButton.SetDisabled(num3 > Singleton<SaveDataContainer>.Instance.Get().GetBalance());
			}
		}
		else
		{
			m_equippedToAllHierarchy.SetActive(value: false);
			m_equipToAllHierarchy.SetActive(value: false);
		}
		if (Singleton<SaveDataContainer>.Instance.Get().GetIntel() >= m_equipment.GetIntelUnlockRequirement())
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
