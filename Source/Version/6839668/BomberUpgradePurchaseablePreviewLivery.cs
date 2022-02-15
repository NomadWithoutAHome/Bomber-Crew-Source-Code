using BomberCrewCommon;
using UnityEngine;

public class BomberUpgradePurchaseablePreviewLivery : BomberUpgradePurchaseablePreview
{
	[SerializeField]
	private GameObject m_textEntryHierarchy;

	[SerializeField]
	private GameObject m_customEditHierarchy;

	[SerializeField]
	private tk2dUIItem m_customEditButton;

	[SerializeField]
	private tk2dUIItem m_renameButton;

	[SerializeField]
	private TextSetter m_textDisplay;

	[SerializeField]
	[NamedText]
	private string m_changeLiveryTextNamedText;

	private bool m_inputLocked;

	private void Start()
	{
		m_customEditButton.OnClick += EditCustom;
		m_renameButton.OnClick += ChangeText;
	}

	private void EditCustom()
	{
		BomberUpgradeScreenController bomberUpgradeScreenController = Object.FindObjectOfType<BomberUpgradeScreenController>();
		bomberUpgradeScreenController.EditCustom(((LiveryEquippable)m_equipment).GetCustomRef(), ((LiveryEquippable)m_equipment).GetCustomIndex());
	}

	private void OnDisable()
	{
		if (m_inputLocked)
		{
			m_inputLocked = false;
			RewiredKeyboardDisable.SetKeyboardDisable(disable: false);
		}
	}

	private void ChangeText()
	{
		LiveryRequirements.LiverySection requirementsForId = Singleton<BomberContainer>.Instance.GetLivery().GetRequirements().GetRequirementsForId(m_requirementSlot.GetUniquePartId());
		string upgradeDetail = m_config.GetUpgradeDetail(m_requirementSlot.GetUniquePartId(), "text");
		Singleton<RenameStringUsingPlatform>.Instance.Rename(upgradeDetail, Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_changeLiveryTextNamedText), requirementsForId.m_maxTextLength, OnTextChanged, null);
	}

	public override void Refresh()
	{
		base.Refresh();
		EquipmentUpgradeFittableBase upgradeFor = m_config.GetUpgradeFor(m_requirementSlot);
		if (m_equipment != null)
		{
			LiveryRequirements.LiverySection requirementsForId = Singleton<BomberContainer>.Instance.GetLivery().GetRequirements().GetRequirementsForId(m_requirementSlot.GetUniquePartId());
			if (requirementsForId.m_isText)
			{
				m_textEntryHierarchy.SetActive(value: true);
				string text = m_config.GetUpgradeDetail(m_requirementSlot.GetUniquePartId(), "text");
				if (text == null)
				{
					text = "BOMBER CREW";
					m_config.SetUpgradeDetail(m_requirementSlot.GetUniquePartId(), "text", "BOMBER CREW");
					OnTextChanged(text);
				}
				m_textDisplay.SetText(text);
			}
			else
			{
				m_textEntryHierarchy.SetActive(value: false);
			}
		}
		else
		{
			m_textEntryHierarchy.SetActive(value: false);
		}
		if (m_equipment != null && ((LiveryEquippable)m_equipment).IsCustom())
		{
			m_customEditHierarchy.SetActive(value: true);
		}
		else
		{
			m_customEditHierarchy.SetActive(value: false);
		}
	}

	private void Update()
	{
	}

	private void OnTextChanged(string newText)
	{
		if (!string.IsNullOrEmpty(newText))
		{
			m_config.SetUpgradeDetail(m_requirementSlot.GetUniquePartId(), "text", newText);
			EquipmentUpgradeFittableBase upgradeFor = m_config.GetUpgradeFor(m_requirementSlot);
			if (upgradeFor == m_equipment)
			{
				Singleton<BomberContainer>.Instance.GetLivery().Refresh();
			}
			else
			{
				Singleton<BomberContainer>.Instance.GetLivery().RefreshOverride(m_requirementSlot.GetUniquePartId(), (LiveryEquippable)m_equipment);
			}
			m_textDisplay.SetText(newText);
		}
	}
}
