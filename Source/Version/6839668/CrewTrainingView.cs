using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class CrewTrainingView : MonoBehaviour
{
	[SerializeField]
	private TextSetter m_skillInfo;

	[SerializeField]
	private GameObject m_unlockedAbilityPrefab;

	[SerializeField]
	private LayoutGrid m_primarySkillLayoutGrid;

	[SerializeField]
	private LayoutGrid m_secondarySkillLayoutGrid;

	[SerializeField]
	[NamedText]
	private string m_primarySkillTitlePrefix;

	[SerializeField]
	private TextSetter m_primarySkillTitle;

	[SerializeField]
	[NamedText]
	private string m_secondarySkillTitlePrefix;

	[SerializeField]
	private TextSetter m_secondarySkillTitle;

	private List<SelectableFilterButton> m_createdSkillButtons = new List<SelectableFilterButton>();

	private List<GameObject> m_createdSkillItems = new List<GameObject>();

	private void SetLayer(Transform t, int l)
	{
		t.gameObject.layer = l;
		foreach (Transform item in t)
		{
			SetLayer(item, l);
		}
	}

	public void Clear()
	{
		foreach (GameObject createdSkillItem in m_createdSkillItems)
		{
			Object.DestroyImmediate(createdSkillItem);
		}
		m_createdSkillItems.Clear();
		m_createdSkillButtons.Clear();
		m_skillInfo.SetTextFromLanguageString("ui_crewtraining_skillinfo_prompt");
	}

	public void SetUpForCrewman(Crewman crewman)
	{
		Clear();
		Crewman.Skill primarySkill = crewman.GetPrimarySkill();
		Dictionary<CrewmanSkillAbilityBool, int> allSkills = Singleton<CrewmanSkillUpgradeInfo>.Instance.GetAllSkills(primarySkill);
		string namedTextImmediate = Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_primarySkillTitlePrefix);
		string text = Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(Singleton<CrewmanSkillUpgradeInfo>.Instance.GetNamedText(primarySkill.GetSkill())).ToUpper();
		m_primarySkillTitle.SetText(namedTextImmediate + " " + text);
		foreach (KeyValuePair<CrewmanSkillAbilityBool, int> kvp2 in allSkills)
		{
			GameObject gameObject = Object.Instantiate(m_unlockedAbilityPrefab);
			gameObject.transform.parent = m_primarySkillLayoutGrid.transform;
			gameObject.GetComponent<AbilityLockUnlockDisplay>().SetFromSkill(kvp2.Key, primarySkill.GetSkill(), kvp2.Value <= primarySkill.GetLevel());
			SelectableFilterButton sfb2 = gameObject.GetComponent<SelectableFilterButton>();
			sfb2.OnClick += delegate
			{
				foreach (SelectableFilterButton createdSkillButton in m_createdSkillButtons)
				{
					createdSkillButton.SetSelected(createdSkillButton == sfb2);
				}
				m_skillInfo.SetText(kvp2.Key.GetDescriptionText());
			};
			m_createdSkillButtons.Add(sfb2);
			m_createdSkillItems.Add(gameObject);
		}
		m_primarySkillLayoutGrid.RepositionChildren();
		primarySkill = crewman.GetSecondarySkill();
		if (primarySkill != null)
		{
			string namedTextImmediate2 = Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_secondarySkillTitlePrefix);
			string text2 = Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(Singleton<CrewmanSkillUpgradeInfo>.Instance.GetNamedText(primarySkill.GetSkill())).ToUpper();
			m_secondarySkillTitle.SetText(namedTextImmediate2 + " " + text2);
			allSkills = Singleton<CrewmanSkillUpgradeInfo>.Instance.GetAllSkills(primarySkill);
			foreach (KeyValuePair<CrewmanSkillAbilityBool, int> kvp in allSkills)
			{
				GameObject gameObject2 = Object.Instantiate(m_unlockedAbilityPrefab);
				gameObject2.transform.parent = m_secondarySkillLayoutGrid.transform;
				gameObject2.GetComponent<AbilityLockUnlockDisplay>().SetFromSkill(kvp.Key, primarySkill.GetSkill(), kvp.Value <= primarySkill.GetLevel());
				SelectableFilterButton sfb = gameObject2.GetComponent<SelectableFilterButton>();
				sfb.OnClick += delegate
				{
					foreach (SelectableFilterButton createdSkillButton2 in m_createdSkillButtons)
					{
						createdSkillButton2.SetSelected(createdSkillButton2 == sfb);
					}
					m_skillInfo.SetText(kvp.Key.GetDescriptionText());
				};
				m_createdSkillButtons.Add(sfb);
				m_createdSkillItems.Add(gameObject2);
			}
			m_secondarySkillLayoutGrid.RepositionChildren();
		}
		else
		{
			m_secondarySkillTitle.SetText(Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_secondarySkillTitlePrefix));
		}
		int l = LayerMask.NameToLayer("_Popup");
		foreach (GameObject createdSkillItem in m_createdSkillItems)
		{
			SetLayer(createdSkillItem.transform, l);
		}
	}
}
