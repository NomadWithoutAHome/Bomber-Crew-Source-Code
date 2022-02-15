using BomberCrewCommon;
using UnityEngine;

public class BomberUpgradePurchaseableSelection : MonoBehaviour
{
	[SerializeField]
	private TextSetter m_name;

	[SerializeField]
	private tk2dTextMesh m_upgradeCost;

	[SerializeField]
	private GameObject m_equippedHierarchy;

	[SerializeField]
	private GameObject m_unequippedHierarchy;

	[SerializeField]
	private GameObject m_lockedHierarchy;

	[SerializeField]
	private TextSetter m_unlockRequiredIntelText;

	[SerializeField]
	private GameObject m_unlockedHierarchy;

	[SerializeField]
	private GameObject m_isNewHierarchy;

	[SerializeField]
	[NamedText]
	private string m_noneNamedText;

	[SerializeField]
	private SelectableFilterButton m_filterButton;

	[SerializeField]
	private tk2dBaseSprite[] m_dlcTypeIcons;

	private EquipmentUpgradeFittableBase m_thisFittable;

	private float m_refundRatio;

	private BomberRequirements.BomberEquipmentRequirement m_requirementSlot;

	private BomberUpgradeConfig m_config;

	public void SetUp(EquipmentUpgradeFittableBase thisFittable, BomberRequirements.BomberEquipmentRequirement requirementSlot, float refundRatio, BomberUpgradeConfig config)
	{
		m_thisFittable = thisFittable;
		m_config = config;
		m_requirementSlot = requirementSlot;
		m_refundRatio = refundRatio;
		if (thisFittable != null && m_dlcTypeIcons.Length > 0)
		{
			if (thisFittable.IsGiftIcon())
			{
				tk2dBaseSprite[] dlcTypeIcons = m_dlcTypeIcons;
				foreach (tk2dBaseSprite tk2dBaseSprite2 in dlcTypeIcons)
				{
					tk2dBaseSprite2.SetSprite("Icon_Gift");
				}
			}
			else
			{
				tk2dBaseSprite[] dlcTypeIcons2 = m_dlcTypeIcons;
				foreach (tk2dBaseSprite tk2dBaseSprite3 in dlcTypeIcons2)
				{
					tk2dBaseSprite3.SetSprite(thisFittable.GetDLCIconName());
				}
			}
		}
		if (m_filterButton != null)
		{
			m_filterButton.OnRefresh += Refresh;
		}
		Refresh();
	}

	private void OnDestroy()
	{
		if (Singleton<SaveDataContainer>.Instance != null && m_thisFittable != null && Singleton<SaveDataContainer>.Instance.Get().GetIntel() >= m_thisFittable.GetIntelUnlockRequirement())
		{
			Singleton<SaveDataContainer>.Instance.Get().SetViewed(m_thisFittable.name);
		}
	}

	public void Refresh()
	{
		EquipmentUpgradeFittableBase upgradeFor = m_config.GetUpgradeFor(m_requirementSlot);
		int num = ((!(m_thisFittable == null)) ? m_thisFittable.GetCost() : 0);
		int num2 = (int)((float)((!(upgradeFor == null)) ? upgradeFor.GetCost() : 0) * m_refundRatio);
		int num3 = num - num2;
		m_name.SetText((!(m_thisFittable == null)) ? m_thisFittable.GetNameTranslated() : Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_noneNamedText));
		if (!(m_thisFittable == null))
		{
			if (Singleton<SaveDataContainer>.Instance.Get().GetIntel() >= m_thisFittable.GetIntelUnlockRequirement())
			{
				m_unlockedHierarchy.SetActive(value: true);
				m_lockedHierarchy.SetActive(value: false);
			}
			else
			{
				m_unlockedHierarchy.SetActive(value: false);
				m_lockedHierarchy.SetActive(value: true);
				m_unlockRequiredIntelText.SetText(m_thisFittable.GetIntelUnlockRequirement().ToString());
			}
		}
		if (m_upgradeCost != null)
		{
			m_upgradeCost.text = num3.ToString();
		}
		if (m_thisFittable == upgradeFor)
		{
			if (m_equippedHierarchy != null)
			{
				m_equippedHierarchy.SetActive(value: true);
			}
			if (m_unequippedHierarchy != null)
			{
				m_unequippedHierarchy.SetActive(value: false);
			}
		}
		else
		{
			if (m_equippedHierarchy != null)
			{
				m_equippedHierarchy.SetActive(value: false);
			}
			if (m_unequippedHierarchy != null)
			{
				m_unequippedHierarchy.SetActive(value: true);
			}
		}
		if (m_thisFittable != null && !Singleton<SaveDataContainer>.Instance.Get().HasBeenViewed(m_thisFittable.name) && Singleton<SaveDataContainer>.Instance.Get().GetIntel() >= m_thisFittable.GetIntelUnlockRequirement() && m_thisFittable.GetIntelUnlockRequirement() > 0)
		{
			m_isNewHierarchy.SetActive(value: true);
		}
		else
		{
			m_isNewHierarchy.SetActive(value: false);
		}
	}
}
