using System;
using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;
using WingroveAudio;

public class CrewQuartersScreenController : MonoBehaviour
{
	[Serializable]
	public class GearTypeDisplay
	{
		[SerializeField]
		public CrewmanGearType m_gearType;

		[SerializeField]
		public bool m_isPresets;

		[NamedText]
		[SerializeField]
		public string m_namedText;
	}

	[SerializeField]
	private GameObject m_crewSelectionPrefab;

	[SerializeField]
	private LayoutGrid m_crewSelectionLayout;

	[SerializeField]
	private GameObject m_slotSelectionPrefab;

	[SerializeField]
	private LayoutGrid m_slotSelectionLayout;

	[SerializeField]
	private GameObject m_crewmanDetailsPrefab;

	[SerializeField]
	private Transform m_crewmanDetailsNode;

	[SerializeField]
	private GameObject m_itemDetailsPrefab;

	[SerializeField]
	private GameObject m_itemDetailsPrefabDLC;

	[SerializeField]
	private LayoutGrid m_itemDetailsLayout;

	[SerializeField]
	private tk2dUIScrollableArea m_scrollableArea;

	[SerializeField]
	private GameObject m_itemPreviewWindowPrefab;

	[SerializeField]
	private GameObject m_itemPresetPreviewWindowPrefab;

	[SerializeField]
	private Transform m_itemPreviewNode;

	[SerializeField]
	private UISelectFinder m_crewSelectFinder;

	[SerializeField]
	private UISelectFinder m_slotSelectFinder;

	[SerializeField]
	private UISelectFinder m_itemSelectFinder;

	[SerializeField]
	private UISelectFinder m_itemPreviewFinder;

	[SerializeField]
	private GameObject m_itemSelectActiveNode;

	[SerializeField]
	private Transform m_crewFocusPosition;

	[SerializeField]
	private AirbasePersistentCrew m_crewAvatars;

	[SerializeField]
	private AirbaseCameraNode m_crewCloseUpCamera;

	[SerializeField]
	private AirbasePersistentCrewTarget m_locationTarget;

	[SerializeField]
	private TextSetter m_gearTypeTextSetter;

	[SerializeField]
	private GameObject m_genericYesNoPopup;

	[SerializeField]
	private bool m_useRefundInsteadOfStore;

	[SerializeField]
	private bool m_useEndlessModeFilters;

	[SerializeField]
	private Camera m_depthClearCamera;

	[SerializeField]
	private GearTypeDisplay[] m_gearToDisplay;

	private List<GameObject> m_createdCrewmanNames = new List<GameObject>();

	private List<SelectableFilterButton> m_createdCrewmanNameButtons = new List<SelectableFilterButton>();

	private List<GameObject> m_createdSlotButtons = new List<GameObject>();

	private List<SelectableFilterButton> m_createdSlotButtonsButtons = new List<SelectableFilterButton>();

	private List<GameObject> m_createdItems = new List<GameObject>();

	private List<SelectableFilterButton> m_createdItemButtons = new List<SelectableFilterButton>();

	private GameObject m_currentCrewmanDetails;

	private GameObject m_currentItemPreview;

	private CrewmanEquipmentBase m_currentItem;

	private CrewmanEquipmentPreset m_currentPreset;

	private Crewman m_currentlySelectedCrewman;

	private GearTypeDisplay m_currentlySelectedGearType;

	private GameObject m_currentlyForwardAvatar;

	private bool m_hasCreatedItemList;

	private bool m_eventsAdded;

	private void Awake()
	{
		GetComponent<AirbaseAreaScreen>().OnAcceptButton += OnAcceptButton;
		GetComponent<AirbaseAreaScreen>().OnBackButton += OnBackButton;
		GetComponent<AirbaseAreaScreen>().OnAction2 += OnActionButtonPress;
	}

	private void OnEnable()
	{
		m_hasCreatedItemList = false;
		Clear();
		CreateTopRow();
		m_crewAvatars.DoTargetedWalk(m_locationTarget);
		m_depthClearCamera.clearFlags = CameraClearFlags.Nothing;
	}

	private void OnDisable()
	{
		Clear();
		HookUpPersistentCrewClicks(addEvent: false);
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
			ClearPreview();
			CreateSlotRowAndPreviewForCrewman(crewman, crewmanAvatar);
			Singleton<AirbaseCameraController>.Instance.MoveCameraToLocation(m_crewCloseUpCamera);
			Singleton<TopBarInfoQueue>.Instance.RemoveRequest(Singleton<TopBarInfoQueue>.Instance.GetCurrentlyShownRequest());
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
			gameObject.GetComponent<CrewQuartersCrewmanNameButton>().SetUp(toSelect);
			SelectableFilterButton thisFilter = gameObject.GetComponent<SelectableFilterButton>();
			thisFilter.SetRelatedObject(toSelect);
			m_createdCrewmanNameButtons.Add(thisFilter);
			GameObject crewmanAvatar = m_crewAvatars.GetCrewmanAvatar(i);
			thisFilter.OnClick += delegate
			{
				foreach (SelectableFilterButton createdCrewmanNameButton in m_createdCrewmanNameButtons)
				{
					createdCrewmanNameButton.SetSelected(createdCrewmanNameButton == thisFilter);
				}
				ClearPreview();
				CreateSlotRowAndPreviewForCrewman(toSelect, crewmanAvatar);
				Singleton<AirbaseCameraController>.Instance.MoveCameraToLocation(m_crewCloseUpCamera);
				Singleton<TopBarInfoQueue>.Instance.RemoveRequest(Singleton<TopBarInfoQueue>.Instance.GetCurrentlyShownRequest());
			};
			HookUpPersistentCrewClicks(addEvent: true);
		}
		m_crewSelectionLayout.RepositionChildren();
	}

	private void CreateSlotRowAndPreviewForCrewman(Crewman crewman, GameObject avatar)
	{
		GearTypeDisplay gearTypeDisplay = ((m_currentlySelectedGearType != null) ? m_currentlySelectedGearType : m_gearToDisplay[0]);
		int num = 0;
		int num2 = 0;
		foreach (SelectableFilterButton createdSlotButtonsButton in m_createdSlotButtonsButtons)
		{
			if (createdSlotButtonsButton.IsSelected())
			{
				num = num2;
			}
			num2++;
		}
		ClearSlots();
		m_currentlySelectedCrewman = crewman;
		m_currentlyForwardAvatar = avatar;
		m_crewAvatars.DoTargetedWalk(m_locationTarget);
		if (crewman != null)
		{
			Transform crewFocusPosition = m_crewFocusPosition;
			AirbaseCrewmanAvatarBehaviour thisAvatar = m_currentlyForwardAvatar.GetComponent<AirbaseCrewmanAvatarBehaviour>();
			thisAvatar.WalkTo(crewFocusPosition.position, crewFocusPosition, 1f, 25f, delegate
			{
				thisAvatar.Talk();
			}, 1f);
			GearTypeDisplay[] gearToDisplay = m_gearToDisplay;
			foreach (GearTypeDisplay gtd in gearToDisplay)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(m_slotSelectionPrefab);
				gameObject.transform.parent = m_slotSelectionLayout.transform;
				gameObject.transform.localPosition = Vector3.zero;
				m_createdSlotButtons.Add(gameObject);
				SelectableFilterButton thisFilter = gameObject.GetComponent<SelectableFilterButton>();
				gameObject.GetComponent<CrewQuartersSlotCategoryButton>().SetUp(crewman, gtd);
				m_createdSlotButtonsButtons.Add(thisFilter);
				thisFilter.SetRelatedObject(gtd);
				thisFilter.OnClick += delegate
				{
					foreach (SelectableFilterButton createdSlotButtonsButton2 in m_createdSlotButtonsButtons)
					{
						createdSlotButtonsButton2.SetSelected(createdSlotButtonsButton2 == thisFilter);
					}
					CreateItemListForType(gtd);
				};
			}
			m_slotSelectionLayout.RepositionChildren();
			SelectableFilterButton nameButton = null;
			foreach (SelectableFilterButton createdCrewmanNameButton in m_createdCrewmanNameButtons)
			{
				if (createdCrewmanNameButton.GetRelatedObject() == crewman)
				{
					nameButton = createdCrewmanNameButton;
				}
			}
			m_currentCrewmanDetails = UnityEngine.Object.Instantiate(m_crewmanDetailsPrefab);
			m_currentCrewmanDetails.transform.parent = m_crewmanDetailsNode;
			m_currentCrewmanDetails.transform.localPosition = Vector3.zero;
			m_currentCrewmanDetails.GetComponent<CrewQuartersCrewmanDisplayPanel>().SetUp(crewman, nameButton);
			if (gearTypeDisplay != null && num != -1 && m_hasCreatedItemList)
			{
				m_createdSlotButtonsButtons[num].SetSelected(selected: true);
				CreateItemListForType(gearTypeDisplay);
			}
		}
		else
		{
			HookUpPersistentCrewClicks(addEvent: true);
		}
	}

	private int ComparePresetsFunc(CrewmanEquipmentPreset cepA, CrewmanEquipmentPreset cepB)
	{
		int num = Mathf.Clamp(cepA.GetIntelUnlockRequirement() - cepB.GetIntelUnlockRequirement(), -1, 1);
		if (num == 0)
		{
			return Mathf.Clamp(cepA.GetCost(null, useRefund: true, null) - cepB.GetCost(null, useRefund: true, null), -1, 1);
		}
		return num;
	}

	private int CompareEquipmentFunc(CrewmanEquipmentBase cebA, CrewmanEquipmentBase cebB)
	{
		int num = Mathf.Clamp(cebA.GetIntelUnlockRequirement() - cebB.GetIntelUnlockRequirement(), -1, 1);
		if (num == 0)
		{
			if (cebA.GetCost() - cebB.GetCost() == 0)
			{
				if (cebA.GetSortBump() == cebB.GetSortBump())
				{
					int num2 = Mathf.Clamp(cebA.GetMarkNumber() - cebB.GetMarkNumber(), -1, 1);
					if (num2 == 0)
					{
						return string.Compare(cebA.GetNamedTextTranslated(), cebB.GetNamedTextTranslated());
					}
					return num2;
				}
				return Mathf.Clamp(cebA.GetSortBump() - cebB.GetSortBump(), -1, 1);
			}
			return Mathf.Clamp(cebA.GetCost() - cebB.GetCost(), -1, 1);
		}
		return num;
	}

	private void CreateItemListForType(GearTypeDisplay gtd)
	{
		m_hasCreatedItemList = true;
		ClearItems();
		m_currentlySelectedGearType = gtd;
		int intel = Singleton<SaveDataContainer>.Instance.Get().GetIntel();
		if (gtd != null)
		{
			HookUpPersistentCrewClicks(addEvent: false);
			m_itemSelectActiveNode.SetActive(value: true);
			m_gearTypeTextSetter.SetTextFromLanguageString(gtd.m_namedText);
			SelectableFilterButton selectableFilterButton = null;
			if (gtd.m_isPresets)
			{
				List<CrewmanEquipmentPreset> allPresets = Singleton<CrewmanGearCatalogueLoader>.Instance.GetCatalogue().GetAllPresets();
				List<CrewmanEquipmentPreset> list = new List<CrewmanEquipmentPreset>();
				list.AddRange(allPresets);
				list.Sort(ComparePresetsFunc);
				foreach (CrewmanEquipmentPreset preset in list)
				{
					bool flag = true;
					if (m_useEndlessModeFilters && preset.IsDLC() && !Singleton<EndlessModeGameFlow>.Instance.GetCurrentEndlessMode().AllowDLCItems())
					{
						flag = false;
					}
					if (!preset.IsAvailableForCrewman(m_currentlySelectedCrewman) || !flag || intel < preset.GetIntelShowRequirement())
					{
						continue;
					}
					GameObject original = ((!preset.IsDLC() && !preset.IsGiftIcon()) ? m_itemDetailsPrefab : m_itemDetailsPrefabDLC);
					GameObject gameObject = UnityEngine.Object.Instantiate(original);
					gameObject.transform.parent = m_itemDetailsLayout.transform;
					gameObject.transform.localPosition = Vector3.zero;
					gameObject.GetComponent<CrewQuartersItemSelectButton>().SetUp(m_currentlySelectedCrewman, preset);
					SelectableFilterButton sfb2 = gameObject.GetComponent<SelectableFilterButton>();
					sfb2.SetRelatedObject(preset);
					if (gameObject.GetComponent<CrewQuartersItemSelectButton>().IsEquippedTicked())
					{
						selectableFilterButton = sfb2;
					}
					sfb2.OnClick += delegate
					{
						foreach (SelectableFilterButton createdItemButton in m_createdItemButtons)
						{
							createdItemButton.SetSelected(createdItemButton == sfb2);
						}
						ShowPreviewFor(preset);
					};
					m_createdItems.Add(gameObject);
					m_createdItemButtons.Add(sfb2);
				}
			}
			else
			{
				List<CrewmanEquipmentBase> byType = Singleton<CrewmanGearCatalogueLoader>.Instance.GetCatalogue().GetByType(gtd.m_gearType);
				List<CrewmanEquipmentBase> list2 = new List<CrewmanEquipmentBase>();
				list2.AddRange(byType);
				list2.Sort(CompareEquipmentFunc);
				foreach (CrewmanEquipmentBase equip in list2)
				{
					bool flag2 = true;
					if (m_useEndlessModeFilters && equip.IsDLC() && !Singleton<EndlessModeGameFlow>.Instance.GetCurrentEndlessMode().AllowDLCItems())
					{
						flag2 = false;
					}
					if (!equip.IsAvailableForCrewman(m_currentlySelectedCrewman) || !flag2 || intel < equip.GetIntelShowRequirement())
					{
						continue;
					}
					GameObject original2 = ((!equip.IsDLC() && !equip.IsGiftIcon()) ? m_itemDetailsPrefab : m_itemDetailsPrefabDLC);
					GameObject gameObject2 = UnityEngine.Object.Instantiate(original2);
					gameObject2.transform.parent = m_itemDetailsLayout.transform;
					gameObject2.transform.localPosition = Vector3.zero;
					gameObject2.GetComponent<CrewQuartersItemSelectButton>().SetUp(m_currentlySelectedCrewman, equip);
					SelectableFilterButton sfb = gameObject2.GetComponent<SelectableFilterButton>();
					if (gameObject2.GetComponent<CrewQuartersItemSelectButton>().IsEquippedTicked())
					{
						selectableFilterButton = sfb;
					}
					sfb.SetRelatedObject(equip);
					sfb.OnClick += delegate
					{
						foreach (SelectableFilterButton createdItemButton2 in m_createdItemButtons)
						{
							createdItemButton2.SetSelected(createdItemButton2 == sfb);
						}
						ShowPreviewFor(equip);
					};
					m_createdItems.Add(gameObject2);
					m_createdItemButtons.Add(sfb);
				}
			}
			if (selectableFilterButton == null)
			{
				selectableFilterButton = m_createdItemButtons[0];
			}
			foreach (SelectableFilterButton createdItemButton3 in m_createdItemButtons)
			{
				createdItemButton3.SetSelected(createdItemButton3 == selectableFilterButton);
			}
			object relatedObject = selectableFilterButton.GetRelatedObject();
			if (relatedObject != null && relatedObject is CrewmanEquipmentBase)
			{
				ShowPreviewFor((CrewmanEquipmentBase)relatedObject);
			}
			if (relatedObject != null && relatedObject is CrewmanEquipmentPreset)
			{
				ShowPreviewFor((CrewmanEquipmentPreset)relatedObject);
			}
			m_itemDetailsLayout.RepositionChildren();
			m_scrollableArea.ContentLength = m_scrollableArea.MeasureContentLength();
		}
		else
		{
			HookUpPersistentCrewClicks(addEvent: true);
			m_itemSelectActiveNode.SetActive(value: false);
		}
	}

	public void ShowPreviewFor(CrewmanEquipmentBase equipment)
	{
		ClearPreview();
		m_currentItem = equipment;
		m_currentPreset = null;
		bool flag = false;
		if (equipment != null && Singleton<SaveDataContainer>.Instance.Get().GetIntel() >= equipment.GetIntelUnlockRequirement())
		{
			flag = true;
			m_currentCrewmanDetails.GetComponent<CrewQuartersCrewmanDisplayPanel>().Refresh(equipment);
		}
		else
		{
			m_currentCrewmanDetails.GetComponent<CrewQuartersCrewmanDisplayPanel>().Refresh((CrewmanEquipmentBase)null);
		}
		if (equipment != null)
		{
			if (flag)
			{
				m_currentlyForwardAvatar.GetComponent<CrewQuartersCrewmanAvatar>().ShowEquipment(equipment);
			}
			else
			{
				m_currentlyForwardAvatar.GetComponent<CrewQuartersCrewmanAvatar>().RefreshEquipment();
			}
			m_currentItemPreview = UnityEngine.Object.Instantiate(m_itemPreviewWindowPrefab);
			m_currentItemPreview.transform.parent = m_itemPreviewNode;
			m_currentItemPreview.transform.localPosition = Vector3.zero;
			m_currentItemPreview.GetComponent<CrewQuartersItemPreviewDisplay>().SetUp(equipment, m_currentlySelectedCrewman, m_useRefundInsteadOfStore);
			if (Singleton<UISelector>.Instance.GetCurrentFinder() == m_itemSelectFinder)
			{
				m_currentItemPreview.GetComponent<CrewQuartersItemPreviewDisplay>().SetControlIconsEnabled(enabled: true);
			}
			m_currentItemPreview.GetComponent<CrewQuartersItemPreviewDisplay>().OnClick += delegate
			{
				PurchaseEquipment(equipment);
			};
			m_currentItemPreview.GetComponent<CrewQuartersItemPreviewDisplay>().OnClickAll += delegate
			{
				UIPopupData uIPopupData = new UIPopupData();
				uIPopupData.PopupStartCallback = (Action<UIPopUp>)Delegate.Combine(uIPopupData.PopupStartCallback, (Action<UIPopUp>)delegate(UIPopUp uip)
				{
					uip.GetComponent<GenericYesNoPrompt>().SetUp(string.Format(Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("ui_crewquarters_popup_equipall_title"), equipment.GetNamedTextTranslated().ToUpper()), Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("ui_crewquarters_popup_equipall_yes"), Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("ui_crewquarters_popup_equipall_no"), danger: false);
					uip.GetComponent<GenericYesNoPrompt>().OnYes += delegate
					{
						PurchaseEquipmentAll(equipment);
					};
				});
				Singleton<UIPopupManager>.Instance.DisplayPopup(m_genericYesNoPopup, uIPopupData);
			};
			Refresh();
		}
		else
		{
			m_currentlyForwardAvatar.GetComponent<CrewQuartersCrewmanAvatar>().RefreshEquipment();
		}
	}

	public void ShowPreviewFor(CrewmanEquipmentPreset preset)
	{
		ClearPreview();
		m_currentPreset = preset;
		m_currentItem = null;
		bool flag = false;
		if (preset != null && Singleton<SaveDataContainer>.Instance.Get().GetIntel() >= preset.GetIntelUnlockRequirement())
		{
			flag = true;
			m_currentCrewmanDetails.GetComponent<CrewQuartersCrewmanDisplayPanel>().Refresh(preset);
		}
		else
		{
			m_currentCrewmanDetails.GetComponent<CrewQuartersCrewmanDisplayPanel>().Refresh((CrewmanEquipmentPreset)null);
		}
		if (!(preset != null))
		{
			return;
		}
		m_currentlyForwardAvatar.GetComponent<CrewQuartersCrewmanAvatar>().RefreshEquipment();
		if (flag)
		{
			CrewmanEquipmentBase[] allInPreset = preset.GetAllInPreset();
			CrewmanEquipmentBase[] array = allInPreset;
			foreach (CrewmanEquipmentBase ceb in array)
			{
				m_currentlyForwardAvatar.GetComponent<CrewQuartersCrewmanAvatar>().ShowEquipment(ceb);
			}
		}
		m_currentItemPreview = UnityEngine.Object.Instantiate(m_itemPresetPreviewWindowPrefab);
		m_currentItemPreview.transform.parent = m_itemPreviewNode;
		m_currentItemPreview.transform.localPosition = Vector3.zero;
		m_currentItemPreview.GetComponent<CrewQuartersPresetItemPreviewDisplay>().SetUp(preset, m_currentlySelectedCrewman, m_useRefundInsteadOfStore);
		if (Singleton<UISelector>.Instance.GetCurrentFinder() == m_itemSelectFinder)
		{
			m_currentItemPreview.GetComponent<CrewQuartersPresetItemPreviewDisplay>().SetControlIconsEnabled(enabled: true);
		}
		m_currentItemPreview.GetComponent<CrewQuartersPresetItemPreviewDisplay>().OnClick += delegate
		{
			PurchaseEquipment(preset);
		};
		m_currentItemPreview.GetComponent<CrewQuartersPresetItemPreviewDisplay>().OnClickAll += delegate
		{
			UIPopupData uIPopupData = new UIPopupData();
			uIPopupData.PopupStartCallback = (Action<UIPopUp>)Delegate.Combine(uIPopupData.PopupStartCallback, (Action<UIPopUp>)delegate(UIPopUp uip)
			{
				uip.GetComponent<GenericYesNoPrompt>().SetUp(string.Format(Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("ui_crewquarters_popup_equipall_title"), Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(preset.GetPresetName()).ToUpper()), Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("ui_crewquarters_popup_equipall_yes"), Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("ui_crewquarters_popup_equipall_no"), danger: false);
				uip.GetComponent<GenericYesNoPrompt>().OnYes += delegate
				{
					PurchaseEquipmentAll(preset);
				};
			});
			Singleton<UIPopupManager>.Instance.DisplayPopup(m_genericYesNoPopup, uIPopupData);
		};
		Refresh();
	}

	private void Clear()
	{
		foreach (GameObject createdCrewmanName in m_createdCrewmanNames)
		{
			UnityEngine.Object.DestroyImmediate(createdCrewmanName);
		}
		m_createdCrewmanNames.Clear();
		m_createdCrewmanNameButtons.Clear();
		m_currentlyForwardAvatar = null;
		ClearSlots();
	}

	private void PurchaseEquipmentAll(CrewmanEquipmentBase equipment)
	{
		int currentCrewCount = Singleton<CrewContainer>.Instance.GetCurrentCrewCount();
		bool flag = false;
		for (int i = 0; i < currentCrewCount; i++)
		{
			Crewman crewman = Singleton<CrewContainer>.Instance.GetCrewman(i);
			GameObject crewmanAvatar = m_crewAvatars.GetCrewmanAvatar(crewman);
			bool flag2 = false;
			CrewmanEquipmentBase equippedFor = crewman.GetEquippedFor(equipment.GetGearType());
			if (equipment != equippedFor)
			{
				if (Singleton<SaveDataContainer>.Instance.Get().GetStockForCrewGear(equipment) > 0 && !m_useRefundInsteadOfStore)
				{
					Singleton<SaveDataContainer>.Instance.Get().ModifyStockForCrewGear(equipment, -1);
					flag2 = true;
				}
				else if (Singleton<SaveDataContainer>.Instance.Get().GetBalance() >= equipment.GetCost())
				{
					if (m_useRefundInsteadOfStore)
					{
						Singleton<SaveDataContainer>.Instance.Get().AddBalance(equippedFor.GetCost());
						Singleton<SaveDataContainer>.Instance.Get().SpendFunds(-equippedFor.GetCost());
					}
					Singleton<SaveDataContainer>.Instance.Get().AddBalance(-equipment.GetCost());
					Singleton<SaveDataContainer>.Instance.Get().SpendFunds(equipment.GetCost());
					flag2 = true;
				}
				if (flag2)
				{
					flag = true;
					if (equippedFor != null && !m_useRefundInsteadOfStore)
					{
						Singleton<SaveDataContainer>.Instance.Get().ModifyStockForCrewGear(equippedFor, 1);
					}
					crewman.SetEquippedFor(equipment.GetGearType(), equipment);
					Singleton<AirbaseNavigation>.Instance.SetCrewPhotoRequiresRefresh();
					crewmanAvatar.GetComponent<CrewQuartersCrewmanAvatar>().RefreshEquipment();
					crewmanAvatar.GetComponent<AirbaseCrewmanAvatarBehaviour>().GetCrewmanGraphics().FacialAnimController.Smile(0.8f);
				}
			}
			if (flag)
			{
				if (equipment.GetGearType() == CrewmanGearType.Boots)
				{
					WingroveRoot.Instance.PostEvent("CREWMAN_EQUIP_BOOTS");
				}
				else
				{
					WingroveRoot.Instance.PostEvent("CREWMAN_EQUIP_CLOTHING");
				}
			}
			Refresh();
		}
	}

	private void PurchaseEquipment(CrewmanEquipmentBase equipment)
	{
		bool flag = false;
		CrewmanEquipmentBase equippedFor = m_currentlySelectedCrewman.GetEquippedFor(equipment.GetGearType());
		if (equipment != equippedFor)
		{
			if (m_useRefundInsteadOfStore)
			{
				int num = 0;
				num += equipment.GetCost();
				num -= equippedFor.GetCost();
				if (Singleton<SaveDataContainer>.Instance.Get().GetBalance() >= num)
				{
					Singleton<SaveDataContainer>.Instance.Get().AddBalance(-num);
					Singleton<SaveDataContainer>.Instance.Get().SpendFunds(equipment.GetCost());
					flag = true;
				}
			}
			else if (Singleton<SaveDataContainer>.Instance.Get().GetStockForCrewGear(equipment) > 0)
			{
				Singleton<SaveDataContainer>.Instance.Get().ModifyStockForCrewGear(equipment, -1);
				flag = true;
			}
			else if (Singleton<SaveDataContainer>.Instance.Get().GetBalance() >= equipment.GetCost())
			{
				Singleton<SaveDataContainer>.Instance.Get().AddBalance(-equipment.GetCost());
				Singleton<SaveDataContainer>.Instance.Get().SpendFunds(equipment.GetCost());
				flag = true;
			}
			if (flag)
			{
				if (equippedFor != null && !m_useRefundInsteadOfStore)
				{
					Singleton<SaveDataContainer>.Instance.Get().ModifyStockForCrewGear(equippedFor, 1);
				}
				m_currentlySelectedCrewman.SetEquippedFor(equipment.GetGearType(), equipment);
				Singleton<AirbaseNavigation>.Instance.SetCrewPhotoRequiresRefresh();
				m_currentlyForwardAvatar.GetComponent<CrewQuartersCrewmanAvatar>().RefreshEquipment();
				if (equipment.GetGearType() == CrewmanGearType.Boots)
				{
					WingroveRoot.Instance.PostEvent("CREWMAN_EQUIP_BOOTS");
				}
				else
				{
					WingroveRoot.Instance.PostEvent("CREWMAN_EQUIP_CLOTHING");
				}
				m_currentlyForwardAvatar.GetComponent<AirbaseCrewmanAvatarBehaviour>().GetCrewmanGraphics().FacialAnimController.Smile(0.8f);
			}
		}
		Refresh();
	}

	private void PurchaseEquipmentAll(CrewmanEquipmentPreset preset)
	{
		int currentCrewCount = Singleton<CrewContainer>.Instance.GetCurrentCrewCount();
		bool flag = false;
		for (int i = 0; i < currentCrewCount; i++)
		{
			Crewman crewman = Singleton<CrewContainer>.Instance.GetCrewman(i);
			GameObject crewmanAvatar = m_crewAvatars.GetCrewmanAvatar(crewman);
			Dictionary<string, int> stockUsed = new Dictionary<string, int>();
			int cost = preset.GetCost(crewman, m_useRefundInsteadOfStore, stockUsed);
			if (Singleton<SaveDataContainer>.Instance.Get().GetBalance() >= cost)
			{
				bool flag2 = false;
				Singleton<SaveDataContainer>.Instance.Get().AddBalance(-cost);
				Singleton<SaveDataContainer>.Instance.Get().SpendFunds(cost);
				CrewmanEquipmentBase[] allInPreset = preset.GetAllInPreset();
				foreach (CrewmanEquipmentBase crewmanEquipmentBase in allInPreset)
				{
					CrewmanEquipmentBase equippedFor = crewman.GetEquippedFor(crewmanEquipmentBase.GetGearType());
					if (!(equippedFor != Singleton<CrewmanGearCatalogueLoader>.Instance.GetCatalogue().GetByName(crewmanEquipmentBase.name)))
					{
						continue;
					}
					if (!m_useRefundInsteadOfStore)
					{
						if (equippedFor != null)
						{
							Singleton<SaveDataContainer>.Instance.Get().ModifyStockForCrewGear(equippedFor, 1);
						}
						if (Singleton<SaveDataContainer>.Instance.Get().GetStockForCrewGear(crewmanEquipmentBase) > 0)
						{
							Singleton<SaveDataContainer>.Instance.Get().ModifyStockForCrewGear(crewmanEquipmentBase, -1);
						}
					}
					crewman.SetEquippedFor(crewmanEquipmentBase.GetGearType(), crewmanEquipmentBase);
					flag2 = true;
				}
				if (flag2)
				{
					Singleton<AirbaseNavigation>.Instance.SetCrewPhotoRequiresRefresh();
					flag = true;
					crewmanAvatar.GetComponent<CrewQuartersCrewmanAvatar>().RefreshEquipment();
					crewmanAvatar.GetComponent<AirbaseCrewmanAvatarBehaviour>().GetCrewmanGraphics().FacialAnimController.Smile(0.8f);
				}
			}
			Refresh();
			if (flag)
			{
				WingroveRoot.Instance.PostEvent("CREWMAN_EQUIP_CLOTHING");
			}
		}
	}

	private void PurchaseEquipment(CrewmanEquipmentPreset preset)
	{
		int cost = preset.GetCost(m_currentlySelectedCrewman, m_useRefundInsteadOfStore, null);
		if (Singleton<SaveDataContainer>.Instance.Get().GetBalance() >= cost)
		{
			bool flag = false;
			Singleton<SaveDataContainer>.Instance.Get().AddBalance(-cost);
			Singleton<SaveDataContainer>.Instance.Get().SpendFunds(cost);
			CrewmanEquipmentBase[] allInPreset = preset.GetAllInPreset();
			foreach (CrewmanEquipmentBase crewmanEquipmentBase in allInPreset)
			{
				CrewmanEquipmentBase equippedFor = m_currentlySelectedCrewman.GetEquippedFor(crewmanEquipmentBase.GetGearType());
				if (!(equippedFor != Singleton<CrewmanGearCatalogueLoader>.Instance.GetCatalogue().GetByName(crewmanEquipmentBase.name)))
				{
					continue;
				}
				if (!m_useRefundInsteadOfStore)
				{
					if (equippedFor != null)
					{
						Singleton<SaveDataContainer>.Instance.Get().ModifyStockForCrewGear(equippedFor, 1);
					}
					if (Singleton<SaveDataContainer>.Instance.Get().GetStockForCrewGear(crewmanEquipmentBase) > 0)
					{
						Singleton<SaveDataContainer>.Instance.Get().ModifyStockForCrewGear(crewmanEquipmentBase, -1);
					}
				}
				m_currentlySelectedCrewman.SetEquippedFor(crewmanEquipmentBase.GetGearType(), crewmanEquipmentBase);
				flag = true;
			}
			if (flag)
			{
				Singleton<AirbaseNavigation>.Instance.SetCrewPhotoRequiresRefresh();
				m_currentlyForwardAvatar.GetComponent<CrewQuartersCrewmanAvatar>().RefreshEquipment();
				WingroveRoot.Instance.PostEvent("CREWMAN_EQUIP_CLOTHING");
				m_currentlyForwardAvatar.GetComponent<AirbaseCrewmanAvatarBehaviour>().GetCrewmanGraphics().FacialAnimController.Smile(0.8f);
			}
		}
		Refresh();
	}

	private void Refresh()
	{
		foreach (SelectableFilterButton createdSlotButtonsButton in m_createdSlotButtonsButtons)
		{
			createdSlotButtonsButton.Refresh();
		}
		foreach (SelectableFilterButton createdItemButton in m_createdItemButtons)
		{
			createdItemButton.Refresh();
		}
		foreach (SelectableFilterButton createdCrewmanNameButton in m_createdCrewmanNameButtons)
		{
			createdCrewmanNameButton.Refresh();
		}
		if (m_currentCrewmanDetails != null)
		{
			if (m_currentItem == null && m_currentPreset == null)
			{
				m_currentCrewmanDetails.GetComponent<CrewQuartersCrewmanDisplayPanel>().Refresh(m_currentItem);
			}
			else if (m_currentItem != null)
			{
				m_currentCrewmanDetails.GetComponent<CrewQuartersCrewmanDisplayPanel>().Refresh(m_currentItem);
			}
			else
			{
				m_currentCrewmanDetails.GetComponent<CrewQuartersCrewmanDisplayPanel>().Refresh(m_currentPreset);
			}
		}
		if (m_currentItemPreview != null)
		{
			if (m_currentItemPreview.GetComponent<CrewQuartersItemPreviewDisplay>() != null)
			{
				m_currentItemPreview.GetComponent<CrewQuartersItemPreviewDisplay>().Refresh();
			}
			if (m_currentItemPreview.GetComponent<CrewQuartersPresetItemPreviewDisplay>() != null)
			{
				m_currentItemPreview.GetComponent<CrewQuartersPresetItemPreviewDisplay>().Refresh();
			}
		}
		Singleton<AirbaseNavigation>.Instance.Refresh();
	}

	private void ClearSlots()
	{
		if (m_currentCrewmanDetails != null)
		{
			UnityEngine.Object.DestroyImmediate(m_currentCrewmanDetails);
		}
		foreach (GameObject createdSlotButton in m_createdSlotButtons)
		{
			UnityEngine.Object.DestroyImmediate(createdSlotButton);
		}
		m_createdSlotButtons.Clear();
		m_createdSlotButtonsButtons.Clear();
		m_currentlyForwardAvatar = null;
		ClearItems();
	}

	private void ClearItems()
	{
		HookUpPersistentCrewClicks(addEvent: true);
		foreach (GameObject createdItem in m_createdItems)
		{
			UnityEngine.Object.DestroyImmediate(createdItem);
		}
		m_itemSelectActiveNode.SetActive(value: false);
		m_createdItems.Clear();
		m_createdItemButtons.Clear();
		ClearPreview();
	}

	private void ClearPreview()
	{
		if (m_currentItemPreview != null)
		{
			UnityEngine.Object.DestroyImmediate(m_currentItemPreview);
		}
		if (m_currentlyForwardAvatar != null)
		{
			m_currentlyForwardAvatar.GetComponent<CrewQuartersCrewmanAvatar>().RefreshEquipment();
		}
	}

	private void OnActionButtonPress(bool down)
	{
		if (!(Singleton<UISelector>.Instance.GetCurrentFinder() == m_itemSelectFinder) || !(m_currentItemPreview != null))
		{
			return;
		}
		CrewQuartersItemPreviewDisplay component = m_currentItemPreview.GetComponent<CrewQuartersItemPreviewDisplay>();
		if (component != null)
		{
			if (down)
			{
				component.FakeDownPurchaseAll();
			}
			else
			{
				component.FakeClickPurchaseAll();
			}
			return;
		}
		CrewQuartersPresetItemPreviewDisplay component2 = m_currentItemPreview.GetComponent<CrewQuartersPresetItemPreviewDisplay>();
		if (component2 != null)
		{
			if (down)
			{
				component2.FakeDownPurchaseAll();
			}
			else
			{
				component2.FakeClickPurchaseAll();
			}
		}
	}

	private void OnAcceptButton(bool down)
	{
		if (Singleton<UISelector>.Instance.GetCurrentFinder() == m_crewSelectFinder)
		{
			if (!down)
			{
				Singleton<UISelector>.Instance.SetFinder(m_slotSelectFinder);
			}
		}
		else if (Singleton<UISelector>.Instance.GetCurrentFinder() == m_slotSelectFinder)
		{
			if (!down)
			{
				Singleton<UISelector>.Instance.SetFinder(m_itemSelectFinder);
			}
		}
		else if (Singleton<UISelector>.Instance.GetCurrentFinder() == m_itemSelectFinder)
		{
			if (!(m_currentItemPreview != null))
			{
				return;
			}
			CrewQuartersItemPreviewDisplay component = m_currentItemPreview.GetComponent<CrewQuartersItemPreviewDisplay>();
			if (component != null)
			{
				if (down)
				{
					component.FakeDownPurchase();
				}
				else
				{
					component.FakeClickPurchase();
				}
				return;
			}
			CrewQuartersPresetItemPreviewDisplay component2 = m_currentItemPreview.GetComponent<CrewQuartersPresetItemPreviewDisplay>();
			if (component2 != null)
			{
				if (down)
				{
					component2.FakeDownPurchase();
				}
				else
				{
					component2.FakeClickPurchase();
				}
			}
		}
		else if (Singleton<UISelector>.Instance.GetCurrentFinder() != m_itemPreviewFinder && !down)
		{
			Singleton<UISelector>.Instance.SetFinder(m_crewSelectFinder);
		}
	}

	private void OnBackButton()
	{
		if (Singleton<UISelector>.Instance.GetCurrentFinder() == m_slotSelectFinder)
		{
			Singleton<UISelector>.Instance.SetFinder(m_crewSelectFinder);
		}
		else if (Singleton<UISelector>.Instance.GetCurrentFinder() == m_itemSelectFinder)
		{
			if (m_currentItemPreview != null)
			{
				if (m_currentPreset != null)
				{
					m_currentItemPreview.GetComponent<CrewQuartersPresetItemPreviewDisplay>().SetControlIconsEnabled(enabled: false);
				}
				else if (m_currentItem != null)
				{
					m_currentItemPreview.GetComponent<CrewQuartersItemPreviewDisplay>().SetControlIconsEnabled(enabled: false);
				}
			}
			Singleton<UISelector>.Instance.SetFinder(m_slotSelectFinder);
		}
		else if (Singleton<UISelector>.Instance.GetCurrentFinder() == m_itemPreviewFinder)
		{
			Singleton<UISelector>.Instance.SetFinder(m_itemSelectFinder);
		}
	}
}
