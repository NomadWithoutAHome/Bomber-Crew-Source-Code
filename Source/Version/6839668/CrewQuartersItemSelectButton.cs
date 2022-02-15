using BomberCrewCommon;
using UnityEngine;

public class CrewQuartersItemSelectButton : MonoBehaviour
{
	[SerializeField]
	private GameObject m_lockedHierachy;

	[SerializeField]
	private TextSetter m_unlockRequiredIntelText;

	[SerializeField]
	private GameObject m_unlockedHierachy;

	[SerializeField]
	private TextSetter m_itemName;

	[SerializeField]
	private GameObject m_equippedHierarchy;

	[SerializeField]
	private GameObject m_unequippedHierarchy;

	[SerializeField]
	private SelectableFilterButton m_filterButton;

	[SerializeField]
	private GameObject m_inStockHierarchy;

	[SerializeField]
	private TextSetter m_inStockText;

	[SerializeField]
	private GameObject m_newHierarchy;

	[SerializeField]
	private tk2dSprite[] m_iconSprites;

	private Crewman m_crewman;

	private CrewmanEquipmentBase m_equipment;

	private CrewmanEquipmentPreset m_preset;

	private void Awake()
	{
		m_filterButton.OnRefresh += Refresh;
	}

	public void SetUp(Crewman forCrewman, CrewmanEquipmentBase equipment)
	{
		m_crewman = forCrewman;
		m_equipment = equipment;
		if (m_iconSprites.Length > 0)
		{
			if (equipment.IsGiftIcon())
			{
				tk2dSprite[] iconSprites = m_iconSprites;
				foreach (tk2dSprite tk2dSprite2 in iconSprites)
				{
					tk2dSprite2.SetSprite("Icon_Gift");
				}
			}
			else if (equipment.IsDLC())
			{
				tk2dSprite[] iconSprites2 = m_iconSprites;
				foreach (tk2dSprite tk2dSprite3 in iconSprites2)
				{
					tk2dSprite3.SetSprite(equipment.GetDLCIconName());
				}
			}
		}
		Refresh();
	}

	public void SetUp(Crewman forCrewman, CrewmanEquipmentPreset preset)
	{
		m_crewman = forCrewman;
		m_preset = preset;
		if (m_iconSprites.Length > 0)
		{
			if (preset.IsGiftIcon())
			{
				tk2dSprite[] iconSprites = m_iconSprites;
				foreach (tk2dSprite tk2dSprite2 in iconSprites)
				{
					tk2dSprite2.SetSprite("Icon_Gift");
				}
			}
			else if (preset.IsDLC())
			{
				tk2dSprite[] iconSprites2 = m_iconSprites;
				foreach (tk2dSprite tk2dSprite3 in iconSprites2)
				{
					tk2dSprite3.SetSprite(preset.GetDLCIconName());
				}
			}
		}
		Refresh();
	}

	private void OnDestroy()
	{
		if (Singleton<SaveDataContainer>.Instance != null && m_equipment != null && Singleton<SaveDataContainer>.Instance.Get().GetIntel() >= m_equipment.GetIntelUnlockRequirement())
		{
			Singleton<SaveDataContainer>.Instance.Get().SetViewed(m_equipment.name);
		}
	}

	public bool IsEquippedTicked()
	{
		return m_equippedHierarchy.activeInHierarchy;
	}

	private void Refresh()
	{
		bool flag = false;
		if (m_equipment != null)
		{
			m_itemName.SetText(m_equipment.GetNamedTextTranslated());
			if (Singleton<SaveDataContainer>.Instance.Get().GetIntel() >= m_equipment.GetIntelUnlockRequirement())
			{
				m_unlockedHierachy.SetActive(value: true);
				m_lockedHierachy.SetActive(value: false);
				CrewmanEquipmentBase equippedFor = m_crewman.GetEquippedFor(m_equipment.GetGearType());
				flag = equippedFor == m_equipment;
				int stockForCrewGear = Singleton<SaveDataContainer>.Instance.Get().GetStockForCrewGear(m_equipment);
				if (stockForCrewGear > 0)
				{
					m_inStockHierarchy.SetActive(value: true);
					m_inStockText.SetText(stockForCrewGear.ToString());
				}
				else
				{
					m_inStockHierarchy.SetActive(value: false);
				}
				m_newHierarchy.SetActive(!Singleton<SaveDataContainer>.Instance.Get().HasBeenViewed(m_equipment.name) && m_equipment.GetCost() != 0);
			}
			else
			{
				m_unlockedHierachy.SetActive(value: false);
				m_lockedHierachy.SetActive(value: true);
				m_unlockRequiredIntelText.SetText(m_equipment.GetIntelUnlockRequirement().ToString());
			}
		}
		else
		{
			m_itemName.SetTextFromLanguageString(m_preset.GetPresetName());
			if (Singleton<SaveDataContainer>.Instance.Get().GetIntel() >= m_preset.GetIntelUnlockRequirement())
			{
				m_unlockedHierachy.SetActive(value: true);
				m_lockedHierachy.SetActive(value: false);
				CrewmanEquipmentBase[] allInPreset = m_preset.GetAllInPreset();
				bool flag2 = true;
				bool flag3 = false;
				CrewmanEquipmentBase[] array = allInPreset;
				foreach (CrewmanEquipmentBase crewmanEquipmentBase in array)
				{
					if (m_crewman.GetEquippedFor(crewmanEquipmentBase.GetGearType()) != Singleton<CrewmanGearCatalogueLoader>.Instance.GetCatalogue().GetByName(crewmanEquipmentBase.name))
					{
						flag2 = false;
					}
					else
					{
						flag3 = true;
					}
				}
				flag = flag2 && flag3;
				m_inStockHierarchy.SetActive(value: false);
				m_newHierarchy.SetActive(!Singleton<SaveDataContainer>.Instance.Get().HasBeenViewed(m_preset.name));
			}
			else
			{
				m_unlockedHierachy.SetActive(value: false);
				m_lockedHierachy.SetActive(value: true);
				m_unlockRequiredIntelText.SetText(m_preset.GetIntelUnlockRequirement().ToString());
			}
		}
		if (m_equippedHierarchy != null)
		{
			m_equippedHierarchy.SetActive(flag);
		}
		if (m_unequippedHierarchy != null)
		{
			m_unequippedHierarchy.SetActive(!flag);
		}
	}
}
