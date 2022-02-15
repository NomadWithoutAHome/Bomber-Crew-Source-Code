using System;
using BomberCrewCommon;
using UnityEngine;

public class BomberUpgradePurchaseablePreview : MonoBehaviour
{
	[SerializeField]
	private TextSetter m_titleText;

	[SerializeField]
	private TextSetter m_descriptionText;

	[SerializeField]
	private SelectableFilterButton m_purchaseItemButton;

	[SerializeField]
	private GameObject m_equippedHierarchy;

	[SerializeField]
	private GameObject m_unequippedHierarchy;

	[SerializeField]
	private TextSetter m_armourTextField;

	[SerializeField]
	private TextSetter m_weightTextField;

	[SerializeField]
	private TextSetter m_survivalLandTextField;

	[SerializeField]
	private TextSetter m_survivalSeaTextField;

	[SerializeField]
	private TextSetter m_reliabilityTextField;

	[SerializeField]
	private TextSetter m_armourCompareTextField;

	[SerializeField]
	private TextSetter m_weightCompareTextField;

	[SerializeField]
	private TextSetter m_reliabilityCompareTextField;

	[SerializeField]
	private TextSetter m_survivalLandCompareTextField;

	[SerializeField]
	private TextSetter m_survivalSeaCompareTextField;

	[SerializeField]
	private GameObject m_armourHierarchy;

	[SerializeField]
	private GameObject m_weightHierarchy;

	[SerializeField]
	private GameObject m_reliabilityHierarchy;

	[SerializeField]
	private GameObject m_survivalHierarchy;

	[SerializeField]
	private GameObject m_firstStatDisplayHierarchy;

	[SerializeField]
	private GameObject m_secondStatDisplayHierarchy;

	[SerializeField]
	private TextSetter m_firstStatText;

	[SerializeField]
	private TextSetter m_firstStatChangeText;

	[SerializeField]
	private TextSetter m_firstStatTitle;

	[SerializeField]
	private TextSetter m_secondStatText;

	[SerializeField]
	private TextSetter m_secondStatChangeText;

	[SerializeField]
	private TextSetter m_secondStatTitle;

	[SerializeField]
	[NamedText]
	private string m_noneNamedText;

	[SerializeField]
	[NamedText]
	private string m_installNamedTextFormat;

	[SerializeField]
	[NamedText]
	private string m_installRefundNamedTextFormat;

	[SerializeField]
	private GameObject m_isAnItemHierarchy;

	[SerializeField]
	private LayoutGrid m_layoutGrid;

	[SerializeField]
	private GameObject m_lockedHierarchy;

	[SerializeField]
	private GameObject m_unlockedHierarchy;

	protected EquipmentUpgradeFittableBase m_equipment;

	protected BomberUpgradeConfig m_config;

	protected BomberRequirements.BomberEquipmentRequirement m_requirementSlot;

	protected float m_refundFactor;

	private bool m_unlocked;

	public event Action OnClick;

	private void Awake()
	{
		m_purchaseItemButton.GetUIItem().OnClick += OnClickPurchase;
	}

	public void SetUp(EquipmentUpgradeFittableBase equipment, BomberUpgradeConfig config, BomberRequirements.BomberEquipmentRequirement requirementSlot, float refundFactor, bool unlocked)
	{
		this.OnClick = null;
		m_unlocked = unlocked;
		m_refundFactor = refundFactor;
		m_equipment = equipment;
		m_config = config;
		m_requirementSlot = requirementSlot;
		if (equipment != null)
		{
			Singleton<SaveDataContainer>.Instance.Get().SetViewed(equipment.name);
		}
		Refresh();
	}

	public virtual void Refresh()
	{
		m_unlockedHierarchy.SetActive(m_unlocked);
		m_lockedHierarchy.SetActive(!m_unlocked);
		EquipmentUpgradeFittableBase upgradeFor = m_config.GetUpgradeFor(m_requirementSlot);
		if (upgradeFor == m_equipment)
		{
			m_equippedHierarchy.SetActive(value: true);
			m_unequippedHierarchy.SetActive(value: false);
		}
		else
		{
			m_equippedHierarchy.SetActive(value: false);
			m_unequippedHierarchy.SetActive(value: true);
		}
		m_isAnItemHierarchy.SetActive(m_equipment != null);
		int num = ((!(m_equipment == null)) ? m_equipment.GetCost() : 0);
		int num2 = ((!(upgradeFor == null)) ? upgradeFor.GetCost() : 0);
		int num3 = num - Mathf.RoundToInt((float)num2 * m_refundFactor);
		if (num3 >= 0)
		{
			m_purchaseItemButton.SetText(Singleton<GameFlow>.Instance.GetGameMode().ReplaceCurrencySymbol(string.Format(Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_installNamedTextFormat), num3)));
			bool disabled = num3 > Singleton<SaveDataContainer>.Instance.Get().GetBalance();
			m_purchaseItemButton.SetDisabled(disabled);
		}
		else
		{
			m_purchaseItemButton.SetText(Singleton<GameFlow>.Instance.GetGameMode().ReplaceCurrencySymbol(string.Format(Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_installRefundNamedTextFormat), -num3)));
		}
		if (m_equipment == null)
		{
			m_titleText.SetTextFromLanguageString(m_noneNamedText);
			m_descriptionText.SetText(string.Empty);
			if (m_layoutGrid != null)
			{
				m_armourHierarchy.SetActive(value: false);
				m_reliabilityHierarchy.SetActive(value: false);
				m_weightHierarchy.SetActive(value: false);
				m_survivalHierarchy.SetActive(value: false);
				m_layoutGrid.RepositionChildren();
			}
			return;
		}
		m_titleText.SetText(m_equipment.GetNameTranslated());
		m_descriptionText.SetText(m_equipment.GetDescriptionStringTranslated());
		if (m_weightTextField != null)
		{
			StatHelper.StatInfo weightStat = m_equipment.GetWeightStat();
			StatHelper.StatInfo right = ((!(upgradeFor == null)) ? upgradeFor.GetWeightStat() : null);
			Singleton<StatHelper>.Instance.SetStatStringCompare(weightStat, right, "{0}", "({0})", m_weightTextField, m_weightCompareTextField);
		}
		if (m_armourTextField != null)
		{
			StatHelper.StatInfo armourStat = m_equipment.GetArmourStat();
			StatHelper.StatInfo right2 = ((!(upgradeFor == null)) ? upgradeFor.GetArmourStat() : null);
			Singleton<StatHelper>.Instance.SetStatStringCompare(armourStat, right2, "{0}", "({0})", m_armourTextField, m_armourCompareTextField);
		}
		if (m_reliabilityTextField != null)
		{
			StatHelper.StatInfo reliaibilityStat = m_equipment.GetReliaibilityStat();
			StatHelper.StatInfo right3 = ((!(upgradeFor == null)) ? upgradeFor.GetReliaibilityStat() : null);
			Singleton<StatHelper>.Instance.SetStatStringCompare(reliaibilityStat, right3, "{0}", "({0})", m_reliabilityTextField, m_reliabilityCompareTextField);
		}
		if (m_survivalLandTextField != null)
		{
			StatHelper.StatInfo statInfo = StatHelper.StatInfo.Create(0f, null);
			StatHelper.StatInfo statInfo2 = StatHelper.StatInfo.Create(0f, null);
			statInfo.m_value += m_equipment.GetSurvivalPointsLand();
			statInfo2.m_value += ((!(upgradeFor == null)) ? upgradeFor.GetSurvivalPointsLand() : 0);
			Singleton<StatHelper>.Instance.SetStatStringCompare(statInfo, statInfo2, "{0}", "({0})", m_survivalLandTextField, m_survivalLandCompareTextField);
		}
		if (m_survivalSeaTextField != null)
		{
			StatHelper.StatInfo statInfo3 = StatHelper.StatInfo.Create(0f, null);
			StatHelper.StatInfo statInfo4 = StatHelper.StatInfo.Create(0f, null);
			statInfo3.m_value += m_equipment.GetSurvivalPointsSea();
			statInfo4.m_value += ((!(upgradeFor == null)) ? upgradeFor.GetSurvivalPointsSea() : 0);
			Singleton<StatHelper>.Instance.SetStatStringCompare(statInfo3, statInfo4, "{0}", "({0})", m_survivalSeaTextField, m_survivalSeaCompareTextField);
		}
		if (m_firstStatDisplayHierarchy != null)
		{
			StatHelper.StatInfo primaryStatInfo = m_equipment.GetPrimaryStatInfo();
			if (primaryStatInfo != null)
			{
				m_firstStatDisplayHierarchy.SetActive(value: true);
				m_firstStatTitle.SetTextFromLanguageString(primaryStatInfo.m_titleText);
				StatHelper.StatInfo right4 = ((!(upgradeFor == null)) ? upgradeFor.GetPrimaryStatInfo() : null);
				Singleton<StatHelper>.Instance.SetStatStringCompare(primaryStatInfo, right4, "{0}", "({0})", m_firstStatText, m_firstStatChangeText);
			}
			else
			{
				m_firstStatDisplayHierarchy.SetActive(value: false);
			}
		}
		if (m_secondStatDisplayHierarchy != null)
		{
			StatHelper.StatInfo secondaryStatInfo = m_equipment.GetSecondaryStatInfo();
			if (secondaryStatInfo != null)
			{
				m_secondStatDisplayHierarchy.SetActive(value: true);
				m_secondStatTitle.SetTextFromLanguageString(secondaryStatInfo.m_titleText);
				StatHelper.StatInfo right5 = ((!(upgradeFor == null)) ? upgradeFor.GetSecondaryStatInfo() : null);
				Singleton<StatHelper>.Instance.SetStatStringCompare(secondaryStatInfo, right5, "{0}", "({0})", m_secondStatText, m_secondStatChangeText);
			}
			else
			{
				m_secondStatDisplayHierarchy.SetActive(value: false);
			}
		}
		if (m_layoutGrid != null)
		{
			if (m_armourHierarchy != null)
			{
				m_armourHierarchy.SetActive(m_equipment.HasArmour());
			}
			if (m_reliabilityHierarchy != null)
			{
				m_reliabilityHierarchy.SetActive(m_equipment.HasReliability());
			}
			if (m_weightHierarchy != null)
			{
				m_weightHierarchy.SetActive(m_equipment.HasWeight());
			}
			if (m_survivalHierarchy != null)
			{
				m_survivalHierarchy.SetActive(m_equipment.HasSurvival());
			}
			m_layoutGrid.RepositionChildren();
		}
	}

	private void OnClickPurchase()
	{
		if (this.OnClick != null)
		{
			this.OnClick();
		}
	}
}
