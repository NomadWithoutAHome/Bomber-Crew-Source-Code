using System;
using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class CrewTrainingScreenController : MonoBehaviour
{
	[SerializeField]
	private GameObject m_crewSelectionPrefab;

	[SerializeField]
	private LayoutGrid m_crewSelectionLayout;

	[SerializeField]
	private GameObject m_crewmanDetailsPrefab;

	[SerializeField]
	private Transform m_crewmanDetailsNode;

	[SerializeField]
	private GameObject m_canTrainHierarchy;

	[SerializeField]
	private GameObject m_cantTrainHierarchy;

	[SerializeField]
	private GameObject m_trainingSelectionItemPrefab;

	[SerializeField]
	private LayoutGrid m_trainingSelectionLayoutGrid;

	[SerializeField]
	private GameObject m_confirmTrainingPopUp;

	[SerializeField]
	[NamedText]
	private string m_confirmPopUpTitle;

	[SerializeField]
	[NamedText]
	private string m_confirmPopUpMessage;

	[SerializeField]
	[NamedText]
	private string m_confirmPopUpYesText;

	[SerializeField]
	[NamedText]
	private string m_confirmPopUpNoText;

	[SerializeField]
	[NamedText]
	private string m_confirmRemovePopUpTitle;

	[SerializeField]
	[NamedText]
	private string m_confirmRemovePopUpMessage;

	[SerializeField]
	[NamedText]
	private string m_confirmPopUpYesRemoveText;

	[SerializeField]
	private UISelectFinder m_crewSelectFinder;

	[SerializeField]
	private UISelectFinder m_trainingTypeSelectionFinder;

	[SerializeField]
	private Transform m_crewFocusPosition;

	[SerializeField]
	private AirbasePersistentCrew m_crewAvatars;

	[SerializeField]
	private GameObject m_skillSelectEnabledHierarchy;

	[SerializeField]
	private UISelectorMovementType m_autoClickMovementType;

	[SerializeField]
	private UISelectorMovementType m_noAutoClickMovementType;

	[SerializeField]
	private AirbaseCameraNode m_crewCloseUpCamera;

	[SerializeField]
	private TextSetter m_upgradeLevelInfo;

	[SerializeField]
	private GameObject m_unlockedAbilityPrefab;

	[SerializeField]
	private LayoutGrid m_primarySkillLayoutGrid;

	[SerializeField]
	private LayoutGrid m_secondarySkillLayoutGrid;

	[SerializeField]
	private AirbasePersistentCrewTarget m_locationTarget;

	[SerializeField]
	private TextSetter m_unlockAtLevelXSetter;

	[SerializeField]
	[NamedText]
	private string m_unlockAtLevelXString;

	[SerializeField]
	private GameObject m_viewEnabledHierarchy;

	[SerializeField]
	private TextSetter m_primarySkillTitle;

	[SerializeField]
	private TextSetter m_secondarySkillTitle;

	[SerializeField]
	[NamedText]
	private string m_primarySkillTitlePrefix;

	[SerializeField]
	[NamedText]
	private string m_secondarySkillTitlePrefix;

	[SerializeField]
	[NamedText]
	private string m_skillInfoPrompt;

	[SerializeField]
	private TextSetter m_skillInfo;

	[SerializeField]
	private GameObject m_retrainHierarchy;

	[SerializeField]
	private tk2dUIItem m_retrainButton;

	private List<GameObject> m_createdCrewmanNames = new List<GameObject>();

	private List<SelectableFilterButton> m_createdCrewmanNameButtons = new List<SelectableFilterButton>();

	private List<GameObject> m_createdItems = new List<GameObject>();

	private List<tk2dUIItem> m_createdItemButtons = new List<tk2dUIItem>();

	private List<SelectableFilterButton> m_createdSkillButtons = new List<SelectableFilterButton>();

	private List<GameObject> m_createdSkillItems = new List<GameObject>();

	private GameObject m_currentCrewmanDetails;

	private Crewman m_currentlySelectedCrewman;

	private GameObject m_currentlyForwardAvatar;

	private Crewman.SpecialisationSkill m_selectedSkill;

	private bool m_eventsAdded;

	private void Start()
	{
		m_retrainButton.OnClick += Retrain;
		AirbaseAreaScreen component = GetComponent<AirbaseAreaScreen>();
		component.OnAcceptButton += OnAcceptPressed;
		component.OnBackButton += OnBackPressed;
	}

	private void TrainSkill(Crewman.SpecialisationSkill skillToTrain)
	{
		if (m_currentlySelectedCrewman != null)
		{
			m_currentlySelectedCrewman.TrainSecondarySkill(skillToTrain);
			m_currentlyForwardAvatar.GetComponent<AirbaseCrewmanAvatarBehaviour>().GetCrewmanGraphics().FacialAnimController.Smile(0.8f);
		}
		Refresh();
	}

	private void HookUpPersistentCrewClicks(bool addEvent)
	{
		if (m_eventsAdded == addEvent)
		{
			return;
		}
		int currentCrewCount = Singleton<CrewContainer>.Instance.GetCurrentCrewCount();
		if (Singleton<CrewContainer>.Instance != null)
		{
			for (int i = 0; i < currentCrewCount; i++)
			{
				GameObject crewmanAvatar = m_crewAvatars.GetCrewmanAvatar(i);
				if (crewmanAvatar != null)
				{
					if (addEvent)
					{
						crewmanAvatar.GetComponent<AirbaseCrewmanAvatarBehaviour>().OnClick += ClickCrew;
					}
					else
					{
						crewmanAvatar.GetComponent<AirbaseCrewmanAvatarBehaviour>().OnClick -= ClickCrew;
					}
				}
			}
		}
		m_eventsAdded = addEvent;
	}

	private void ClickCrew(GameObject go)
	{
		int currentCrewCount = Singleton<CrewContainer>.Instance.GetCurrentCrewCount();
		for (int i = 0; i < currentCrewCount; i++)
		{
			GameObject crewmanAvatar = m_crewAvatars.GetCrewmanAvatar(i);
			if (!(crewmanAvatar == go))
			{
				continue;
			}
			int num = 0;
			foreach (SelectableFilterButton createdCrewmanNameButton in m_createdCrewmanNameButtons)
			{
				createdCrewmanNameButton.SetSelected(num == i);
				num++;
			}
			Crewman crewman = Singleton<CrewContainer>.Instance.GetCrewman(i);
			ClearUpgrades();
			CreateSlotRowAndPreviewForCrewman(crewman, crewmanAvatar);
			Singleton<AirbaseCameraController>.Instance.MoveCameraToLocation(m_crewCloseUpCamera);
			Singleton<TopBarInfoQueue>.Instance.RemoveRequest(Singleton<TopBarInfoQueue>.Instance.GetCurrentlyShownRequest());
		}
	}

	private void OnEnable()
	{
		Clear();
		CreateTopRow();
		m_crewAvatars.DoTargetedWalk(m_locationTarget);
	}

	private void OnDisable()
	{
		Clear();
		HookUpPersistentCrewClicks(addEvent: false);
	}

	private void Retrain()
	{
		UIPopupData uIPopupData = new UIPopupData();
		uIPopupData.PopupStartCallback = (Action<UIPopUp>)Delegate.Combine(uIPopupData.PopupStartCallback, (Action<UIPopUp>)delegate(UIPopUp uip)
		{
			Crewman crewman = m_currentlySelectedCrewman;
			string namedTextImmediate = Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_confirmRemovePopUpMessage);
			string arg = crewman.GetFirstName() + " " + crewman.GetSurname();
			string namedTextImmediate2 = Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(Singleton<CrewmanSkillUpgradeInfo>.Instance.GetNamedText(crewman.GetSecondarySkill().GetSkill()));
			uip.GetComponent<UIPopUpConfirm>().SetUp(Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_confirmRemovePopUpTitle), string.Format(namedTextImmediate, arg, namedTextImmediate2), Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_confirmPopUpYesRemoveText), Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_confirmPopUpNoText));
			uip.GetComponent<UIPopUpConfirm>().OnConfirm += delegate
			{
				crewman.RemoveSecondarySkill();
				Refresh();
				Singleton<UISelector>.Instance.SetFinder(m_trainingTypeSelectionFinder);
			};
		});
		Singleton<UIPopupManager>.Instance.DisplayPopup(m_confirmTrainingPopUp, uIPopupData);
	}

	private void DebugCallback()
	{
		if (m_currentlySelectedCrewman != null && GUILayout.Button("XP+"))
		{
			m_currentlySelectedCrewman.GetPrimarySkill().AddXP(5000);
			if (m_currentlySelectedCrewman.GetSecondarySkill() != null)
			{
				m_currentlySelectedCrewman.GetSecondarySkill().AddXP(5000);
			}
			Refresh();
		}
	}

	private void CreateTopRow()
	{
		int currentCrewCount = Singleton<CrewContainer>.Instance.GetCurrentCrewCount();
		for (int i = 0; i < currentCrewCount; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(m_crewSelectionPrefab);
			gameObject.transform.parent = m_crewSelectionLayout.transform;
			gameObject.transform.localPosition = Vector3.zero;
			m_createdCrewmanNames.Add(gameObject);
			Crewman toSelect = Singleton<CrewContainer>.Instance.GetCrewman(i);
			gameObject.GetComponent<CrewTrainingCrewmanNameButton>().SetUp(toSelect);
			SelectableFilterButton thisFilter = gameObject.GetComponent<SelectableFilterButton>();
			m_createdCrewmanNameButtons.Add(thisFilter);
			GameObject crewmanAvatar = m_crewAvatars.GetCrewmanAvatar(i);
			thisFilter.SetRelatedObject(toSelect);
			thisFilter.OnClick += delegate
			{
				foreach (SelectableFilterButton createdCrewmanNameButton in m_createdCrewmanNameButtons)
				{
					createdCrewmanNameButton.SetSelected(createdCrewmanNameButton == thisFilter);
				}
				CreateSlotRowAndPreviewForCrewman(toSelect, crewmanAvatar);
				Singleton<TopBarInfoQueue>.Instance.RemoveRequest(Singleton<TopBarInfoQueue>.Instance.GetCurrentlyShownRequest());
				Singleton<AirbaseCameraController>.Instance.MoveCameraToLocation(m_crewCloseUpCamera);
			};
			HookUpPersistentCrewClicks(addEvent: true);
		}
		m_crewSelectionLayout.RepositionChildren();
	}

	private void CreateSlotRowAndPreviewForCrewman(Crewman crewman, GameObject avatar)
	{
		ClearUpgrades();
		m_currentlySelectedCrewman = crewman;
		m_currentlyForwardAvatar = avatar;
		m_skillInfo.SetTextFromLanguageString(m_skillInfoPrompt);
		m_crewAvatars.DoTargetedWalk(m_locationTarget);
		if (crewman != null)
		{
			HookUpPersistentCrewClicks(addEvent: false);
			m_viewEnabledHierarchy.SetActive(value: true);
			Transform crewFocusPosition = m_crewFocusPosition;
			AirbaseCrewmanAvatarBehaviour thisAvatar = m_currentlyForwardAvatar.GetComponent<AirbaseCrewmanAvatarBehaviour>();
			thisAvatar.WalkTo(crewFocusPosition.position, crewFocusPosition, 1f, 25f, delegate
			{
				thisAvatar.Talk();
			}, 1f);
			m_currentCrewmanDetails = UnityEngine.Object.Instantiate(m_crewmanDetailsPrefab);
			m_currentCrewmanDetails.transform.parent = m_crewmanDetailsNode;
			m_currentCrewmanDetails.transform.localPosition = Vector3.zero;
			SelectableFilterButton nameButton = null;
			foreach (SelectableFilterButton createdCrewmanNameButton in m_createdCrewmanNameButtons)
			{
				if (createdCrewmanNameButton.GetRelatedObject() == crewman)
				{
					nameButton = createdCrewmanNameButton;
				}
			}
			m_currentCrewmanDetails.GetComponent<CrewQuartersCrewmanDisplayPanel>().SetUp(crewman, nameButton);
			List<Crewman.SpecialisationSkill> list = new List<Crewman.SpecialisationSkill>();
			if (crewman.GetSecondarySkill() == null && crewman.GetPrimarySkill().GetLevel() >= Singleton<CrewmanSkillUpgradeInfo>.Instance.GetSecondarySkillUnlocksAt(crewman.GetPrimarySkill().GetSkill()))
			{
				Crewman.SpecialisationSkill[] array = Singleton<CrewmanSkillUpgradeInfo>.Instance.SecondarySkillsAvailable(crewman.GetPrimarySkill().GetSkill());
				if (array != null)
				{
					list.AddRange(array);
				}
			}
			if (list.Count == 0)
			{
				m_cantTrainHierarchy.SetActive(crewman.GetSecondarySkill() == null);
				m_canTrainHierarchy.SetActive(value: false);
			}
			else
			{
				m_cantTrainHierarchy.SetActive(value: false);
				m_canTrainHierarchy.SetActive(value: true);
				foreach (Crewman.SpecialisationSkill sk in list)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(m_trainingSelectionItemPrefab);
					gameObject.GetComponent<CrewTrainingSkillTypeButton>().SetUp(sk);
					gameObject.transform.parent = m_trainingSelectionLayoutGrid.transform;
					tk2dUIItem component = gameObject.GetComponent<tk2dUIItem>();
					m_createdItemButtons.Add(component);
					component.OnClick += delegate
					{
						m_skillSelectEnabledHierarchy.SetActive(value: true);
						m_selectedSkill = sk;
						UIPopupData uIPopupData = new UIPopupData();
						uIPopupData.PopupStartCallback = (Action<UIPopUp>)Delegate.Combine(uIPopupData.PopupStartCallback, (Action<UIPopUp>)delegate(UIPopUp pop)
						{
							UIPopUpConfirm component4 = pop.GetComponent<UIPopUpConfirm>();
							string namedTextImmediate3 = Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_confirmPopUpMessage);
							string arg = crewman.GetFirstName() + " " + crewman.GetSurname();
							string namedTextImmediate4 = Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(Singleton<CrewmanSkillUpgradeInfo>.Instance.GetNamedText(m_selectedSkill));
							component4.SetUp(Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_confirmPopUpTitle), string.Format(namedTextImmediate3, arg, namedTextImmediate4), Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_confirmPopUpYesText), Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_confirmPopUpNoText));
							component4.OnConfirm += delegate
							{
								TrainSkill(m_selectedSkill);
								Singleton<UISelector>.Instance.SetFinder(m_trainingTypeSelectionFinder);
							};
							component4.OnCancel += delegate
							{
								Singleton<UISelector>.Instance.SetFinder(m_trainingTypeSelectionFinder);
							};
						});
						Singleton<UIPopupManager>.Instance.DisplayPopup(m_confirmTrainingPopUp, uIPopupData);
					};
					m_createdItems.Add(gameObject);
				}
				m_trainingSelectionLayoutGrid.RepositionChildren();
			}
			Crewman.Skill primarySkill = crewman.GetPrimarySkill();
			Dictionary<CrewmanSkillAbilityBool, int> allSkills = Singleton<CrewmanSkillUpgradeInfo>.Instance.GetAllSkills(primarySkill);
			string namedTextImmediate = Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_primarySkillTitlePrefix);
			string text = Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(Singleton<CrewmanSkillUpgradeInfo>.Instance.GetNamedText(primarySkill.GetSkill())).ToUpper();
			m_primarySkillTitle.SetText(namedTextImmediate + " " + text);
			foreach (KeyValuePair<CrewmanSkillAbilityBool, int> kvp2 in allSkills)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(m_unlockedAbilityPrefab);
				gameObject2.transform.parent = m_primarySkillLayoutGrid.transform;
				gameObject2.GetComponent<AbilityLockUnlockDisplay>().SetFromSkill(kvp2.Key, primarySkill.GetSkill(), kvp2.Value <= primarySkill.GetLevel());
				SelectableFilterButton sfb2 = gameObject2.GetComponent<SelectableFilterButton>();
				sfb2.OnClick += delegate
				{
					foreach (SelectableFilterButton createdSkillButton in m_createdSkillButtons)
					{
						createdSkillButton.SetSelected(createdSkillButton == sfb2);
					}
					m_skillInfo.SetText(kvp2.Key.GetDescriptionText());
				};
				m_createdSkillButtons.Add(sfb2);
				m_createdSkillItems.Add(gameObject2);
			}
			m_primarySkillLayoutGrid.RepositionChildren();
			primarySkill = crewman.GetSecondarySkill();
			if (primarySkill != null)
			{
				m_retrainHierarchy.SetActive(value: true);
				string namedTextImmediate2 = Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_secondarySkillTitlePrefix);
				string text2 = Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(Singleton<CrewmanSkillUpgradeInfo>.Instance.GetNamedText(primarySkill.GetSkill())).ToUpper();
				m_secondarySkillTitle.SetText(namedTextImmediate2 + " " + text2);
				allSkills = Singleton<CrewmanSkillUpgradeInfo>.Instance.GetAllSkills(primarySkill);
				foreach (KeyValuePair<CrewmanSkillAbilityBool, int> kvp in allSkills)
				{
					GameObject gameObject3 = UnityEngine.Object.Instantiate(m_unlockedAbilityPrefab);
					gameObject3.transform.parent = m_secondarySkillLayoutGrid.transform;
					gameObject3.GetComponent<AbilityLockUnlockDisplay>().SetFromSkill(kvp.Key, primarySkill.GetSkill(), kvp.Value <= primarySkill.GetLevel());
					SelectableFilterButton sfb = gameObject3.GetComponent<SelectableFilterButton>();
					sfb.OnClick += delegate
					{
						foreach (SelectableFilterButton createdSkillButton2 in m_createdSkillButtons)
						{
							createdSkillButton2.SetSelected(createdSkillButton2 == sfb);
						}
						m_skillInfo.SetText(kvp.Key.GetDescriptionText());
					};
					m_createdSkillButtons.Add(sfb);
					m_createdSkillItems.Add(gameObject3);
				}
				m_secondarySkillLayoutGrid.RepositionChildren();
			}
			else
			{
				m_retrainHierarchy.SetActive(value: false);
				m_secondarySkillTitle.SetText(Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_secondarySkillTitlePrefix));
				int secondarySkillUnlocksAt = Singleton<CrewmanSkillUpgradeInfo>.Instance.GetSecondarySkillUnlocksAt(crewman.GetPrimarySkill().GetSkill());
				m_unlockAtLevelXSetter.SetText(string.Format(Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_unlockAtLevelXString), secondarySkillUnlocksAt));
			}
			int num = 0;
			int num2 = 0;
			foreach (SelectableFilterButton createdSkillButton3 in m_createdSkillButtons)
			{
				UISelectorPointingHint component2 = createdSkillButton3.GetUIItem().GetComponent<UISelectorPointingHint>();
				int num3 = num - 1 + num2 * 4;
				int num4 = num + 1 + num2 * 4;
				int num5 = num + (num2 - 1) * 4;
				int num6 = num + (num2 + 1) * 4;
				if (num3 >= 0 && num3 < m_createdSkillButtons.Count)
				{
					component2.SetLeftLink(m_createdSkillButtons[num3].GetUIItem());
				}
				if (num4 >= 0 && num4 < m_createdSkillButtons.Count)
				{
					component2.SetRightLink(m_createdSkillButtons[num4].GetUIItem());
				}
				if (num5 >= 0 && num5 < m_createdSkillButtons.Count && Singleton<UISelector>.Instance.IsValid(m_createdSkillButtons[num5].GetUIItem()))
				{
					component2.SetUpLink(m_createdSkillButtons[num5].GetUIItem());
				}
				if (num6 >= 0 && num6 < m_createdSkillButtons.Count && Singleton<UISelector>.Instance.IsValid(m_createdSkillButtons[num6].GetUIItem()))
				{
					component2.SetDownLink(m_createdSkillButtons[num6].GetUIItem());
				}
				num++;
				if (num == 4)
				{
					num = 0;
					num2++;
				}
			}
			num = 0;
			num2 = 0;
			{
				foreach (tk2dUIItem createdItemButton in m_createdItemButtons)
				{
					UISelectorPointingHint component3 = createdItemButton.GetComponent<UISelectorPointingHint>();
					int num7 = num - 1 + num2 * 3;
					int num8 = num + 1 + num2 * 3;
					int num9 = num + (num2 - 1) * 3;
					int num10 = num + (num2 + 1) * 3;
					if (num7 >= 0 && num7 < m_createdItemButtons.Count)
					{
						component3.SetLeftLink(m_createdItemButtons[num7]);
					}
					if (num8 >= 0 && num8 < m_createdItemButtons.Count)
					{
						component3.SetRightLink(m_createdItemButtons[num8]);
					}
					if (num9 >= 0 && num9 < m_createdItemButtons.Count && Singleton<UISelector>.Instance.IsValid(m_createdItemButtons[num9]))
					{
						component3.SetUpLink(m_createdItemButtons[num9]);
					}
					if (num10 >= 0 && num10 < m_createdItemButtons.Count && Singleton<UISelector>.Instance.IsValid(m_createdItemButtons[num10]))
					{
						component3.SetDownLink(m_createdItemButtons[num10]);
					}
					num++;
					if (num == 4)
					{
						num = 0;
						num2++;
					}
				}
				return;
			}
		}
		m_retrainHierarchy.SetActive(value: false);
		HookUpPersistentCrewClicks(addEvent: true);
		m_viewEnabledHierarchy.SetActive(value: false);
	}

	private void Clear()
	{
		foreach (GameObject createdCrewmanName in m_createdCrewmanNames)
		{
			UnityEngine.Object.DestroyImmediate(createdCrewmanName);
		}
		m_viewEnabledHierarchy.SetActive(value: false);
		m_createdCrewmanNames.Clear();
		m_createdCrewmanNameButtons.Clear();
		m_currentlyForwardAvatar = null;
		m_retrainHierarchy.SetActive(value: false);
		ClearUpgrades();
	}

	private void ClearUpgrades()
	{
		HookUpPersistentCrewClicks(addEvent: true);
		if (m_currentCrewmanDetails != null)
		{
			UnityEngine.Object.DestroyImmediate(m_currentCrewmanDetails);
		}
		foreach (GameObject createdItem in m_createdItems)
		{
			UnityEngine.Object.DestroyImmediate(createdItem);
		}
		m_skillSelectEnabledHierarchy.SetActive(value: false);
		m_createdItems.Clear();
		m_createdItemButtons.Clear();
		m_currentlyForwardAvatar = null;
		foreach (GameObject createdSkillItem in m_createdSkillItems)
		{
			UnityEngine.Object.DestroyImmediate(createdSkillItem);
		}
		m_createdSkillButtons.Clear();
		m_createdSkillItems.Clear();
	}

	private void Refresh()
	{
		foreach (SelectableFilterButton createdCrewmanNameButton in m_createdCrewmanNameButtons)
		{
			createdCrewmanNameButton.Refresh();
		}
		if (m_currentlySelectedCrewman != null)
		{
			CreateSlotRowAndPreviewForCrewman(m_currentlySelectedCrewman, m_currentlyForwardAvatar);
		}
		if (m_currentCrewmanDetails != null)
		{
			m_currentCrewmanDetails.GetComponent<CrewQuartersCrewmanDisplayPanel>().Refresh((CrewmanEquipmentBase)null);
		}
		Singleton<AirbaseNavigation>.Instance.Refresh();
	}

	private void OnAcceptPressed(bool down)
	{
		if (!down)
		{
			if (Singleton<UISelector>.Instance.GetCurrentFinder() == m_crewSelectFinder)
			{
				Singleton<UISelector>.Instance.SetFinder(m_trainingTypeSelectionFinder);
			}
			else if (Singleton<UISelector>.Instance.GetCurrentFinder() != m_trainingTypeSelectionFinder)
			{
				Singleton<UISelector>.Instance.SetFinder(m_crewSelectFinder);
			}
		}
	}

	private void OnBackPressed()
	{
		if (Singleton<UISelector>.Instance.GetCurrentFinder() == m_trainingTypeSelectionFinder)
		{
			Singleton<UISelector>.Instance.SetFinder(m_crewSelectFinder);
		}
	}
}
