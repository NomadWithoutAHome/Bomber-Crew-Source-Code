using System;
using System.Collections.Generic;
using System.Linq;
using BomberCrewCommon;
using UnityEngine;
using WingroveAudio;

public class BomberUpgradeScreenController : MonoBehaviour
{
	[Serializable]
	public class UpgradeScreenCategory
	{
		[SerializeField]
		private BomberUpgradeType[] m_upgradeTypesInCategory;

		[SerializeField]
		[NamedText]
		private string m_namedText;

		public string GetNamedText()
		{
			return m_namedText;
		}

		public BomberUpgradeType[] GetFilters()
		{
			return m_upgradeTypesInCategory;
		}
	}

	[SerializeField]
	private UpgradeScreenCategory[] m_topCategories;

	[SerializeField]
	private float m_refundFactor = 0.8f;

	[SerializeField]
	private GameObject m_topSectionPrefab;

	[SerializeField]
	private LayoutGrid m_topSectionLayout;

	[SerializeField]
	private GameObject m_detailSectionPrefab;

	[SerializeField]
	private LayoutGrid m_detailSectionLayout;

	[SerializeField]
	private GameObject m_upgradeSectionPrefab;

	[SerializeField]
	private GameObject m_upgradeSectionPrefabDLC;

	[SerializeField]
	private LayoutGrid m_upgradeSectionLayout;

	[SerializeField]
	private tk2dUIScrollableArea m_upgradeSectionScrollableArea;

	[SerializeField]
	private GameObject m_previewSectionPrefab;

	[SerializeField]
	private GameObject m_previewSectionPrefabRack;

	[SerializeField]
	private GameObject m_previewSectionPrefabLivery;

	[SerializeField]
	private Transform m_previewSectionNode;

	[SerializeField]
	private UISelectFinder m_topFinder;

	[SerializeField]
	private UISelectFinder m_detailFinder;

	[SerializeField]
	private UISelectFinder m_purchaseableFinder;

	[SerializeField]
	private UISelectFinder m_previewFinder;

	[SerializeField]
	private TextSetter m_weightTotalText;

	[SerializeField]
	private TextSetter m_weightLimitText;

	[SerializeField]
	private TextSetter m_weightTotalChangeText;

	[SerializeField]
	private TextSetter m_weightLimitChangeText;

	[SerializeField]
	private TextSetter m_survivalLandText;

	[SerializeField]
	private TextSetter m_survivalSeaText;

	[SerializeField]
	private TextSetter m_survivalLandTextChange;

	[SerializeField]
	private TextSetter m_survivalSeaTextChange;

	[SerializeField]
	private GameObject m_itemSelectActiveNode;

	[SerializeField]
	private GameObject m_airbaseLancaster;

	[SerializeField]
	private AirbaseAreaScreen m_customArtScreenArea;

	[SerializeField]
	private EditCustomLiveryScreen m_customArtScreenController;

	[SerializeField]
	private AirbasePersistentCrew m_persistentCrew;

	[SerializeField]
	private TextSetter m_itemListTextTitle;

	[SerializeField]
	private TextSetter m_bomberName;

	[SerializeField]
	private tk2dUIItem m_renameButton;

	[SerializeField]
	[NamedText]
	private string m_renameBomberPopupTitle;

	[SerializeField]
	private Color m_weightLimitBadColor;

	[SerializeField]
	private Color m_weightLimitOKColor;

	[SerializeField]
	private GameObject m_textPopupPrefab;

	[SerializeField]
	[NamedText]
	private string m_weightLimitOverText;

	[SerializeField]
	private Camera m_depthClearCamera;

	[SerializeField]
	private bool m_useEndlessModeFilters;

	[SerializeField]
	private EquipmentUpgradeFittableBase.AircraftUpgradeType m_aircraftUpgradeTypeFilter;

	private UpgradeScreenCategory m_currentlySelectedCategory;

	private BomberRequirements.BomberEquipmentRequirement m_currentlySelectedRequirement;

	private EquipmentUpgradeFittableBase m_currentlySelectedEquippable;

	private List<SelectableFilterButton> m_topRowButtons = new List<SelectableFilterButton>();

	private List<GameObject> m_topRowObjects = new List<GameObject>();

	private List<SelectableFilterButton> m_detailRowButtons = new List<SelectableFilterButton>();

	private List<GameObject> m_detailRowObjects = new List<GameObject>();

	private List<SelectableFilterButton> m_upgradeRowButtons = new List<SelectableFilterButton>();

	private List<GameObject> m_upgradeRowObjects = new List<GameObject>();

	private GameObject m_currentPreviewSection;

	private BomberSystemUniqueId[] m_allUniqueIds;

	private bool m_hasCreatedPurchaseableRow;

	private LiveryEquippable m_liveryCategoryReturnFrom;

	private bool m_returnFromLivery;

	private bool m_inputLocked;

	private void Awake()
	{
		m_allUniqueIds = m_airbaseLancaster.GetComponentsInChildren<BomberSystemUniqueId>(includeInactive: true);
		GetComponent<AirbaseAreaScreen>().OnAcceptButton += OnAcceptButton;
		GetComponent<AirbaseAreaScreen>().OnBackButton += OnBackButton;
		m_renameButton.OnClick += RenameBomber;
	}

	private void RenameBomber()
	{
		Singleton<RenameStringUsingPlatform>.Instance.Rename(Singleton<BomberContainer>.Instance.GetCurrentConfig().GetName(), Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(m_renameBomberPopupTitle), 32, delegate(string newName)
		{
			if (!string.IsNullOrEmpty(newName))
			{
				Singleton<BomberContainer>.Instance.GetCurrentConfig().SetName(newName);
				m_bomberName.SetText("\"" + Singleton<BomberContainer>.Instance.GetCurrentConfig().GetName() + "\"");
			}
		}, null);
	}

	private void BomberNameChange(tk2dUITextInput ti)
	{
		Singleton<BomberContainer>.Instance.GetCurrentConfig().SetName(ti.Text);
	}

	private void OnEnable()
	{
		m_hasCreatedPurchaseableRow = false;
		Singleton<PoolManager>.Instance.ClearPool(m_previewSectionPrefabRack.GetInstanceID());
		Singleton<PoolManager>.Instance.ClearPool(m_previewSectionPrefabLivery.GetInstanceID());
		Singleton<PoolManager>.Instance.ClearPool(m_previewSectionPrefab.GetInstanceID());
		RefreshWeight();
		SpawnTopRow();
		m_persistentCrew.DoFreeWalk();
	}

	private void OnDisable()
	{
		m_depthClearCamera.clearFlags = CameraClearFlags.Nothing;
		Singleton<BomberContainer>.Instance.GetLivery().Refresh();
		if (m_inputLocked)
		{
			m_inputLocked = false;
			RewiredKeyboardDisable.SetKeyboardDisable(disable: false);
		}
		Clear();
	}

	public void EditCustom(string idS, int customIndex)
	{
		AttemptPurchase();
		Singleton<AirbaseNavigation>.Instance.SetSelectingArea(m_customArtScreenArea);
		m_customArtScreenController.SetSelectedIndex(idS, customIndex, (LiveryEquippable)m_currentlySelectedEquippable);
	}

	public void SetReturnFromLivery(LiveryEquippable liveryFrom)
	{
		m_liveryCategoryReturnFrom = liveryFrom;
		m_returnFromLivery = true;
	}

	private void SpawnTopRow()
	{
		m_bomberName.SetText("\"" + Singleton<BomberContainer>.Instance.GetCurrentConfig().GetName() + "\"");
		UpgradeScreenCategory[] topCategories = m_topCategories;
		foreach (UpgradeScreenCategory usc in topCategories)
		{
			BomberRequirements.BomberEquipmentRequirement[] requirements = Singleton<GameFlow>.Instance.GetGameMode().GetBomberRequirements().GetRequirements();
			BomberUpgradeConfig currentConfig = Singleton<BomberContainer>.Instance.GetCurrentConfig();
			int intel = Singleton<SaveDataContainer>.Instance.Get().GetIntel();
			int num = 0;
			BomberRequirements.BomberEquipmentRequirement[] array = requirements;
			foreach (BomberRequirements.BomberEquipmentRequirement bomberEquipmentRequirement in array)
			{
				if (!usc.GetFilters().Contains(bomberEquipmentRequirement.GetUpgradeConfig()))
				{
					continue;
				}
				List<EquipmentUpgradeFittableBase> allOfType = Singleton<BomberUpgradeCatalogueLoader>.Instance.GetCatalogue().GetAllOfType(bomberEquipmentRequirement.GetUpgradeConfig());
				int num2 = (bomberEquipmentRequirement.CanBeNull() ? 1 : 0);
				foreach (EquipmentUpgradeFittableBase item in allOfType)
				{
					if (item != null && item.IsDisplayableFor(bomberEquipmentRequirement) && intel >= item.GetIntelShowRequirement())
					{
						num2++;
					}
				}
				if (num2 > 1)
				{
					num++;
				}
			}
			if (num <= 0)
			{
				continue;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(m_topSectionPrefab);
			gameObject.transform.parent = m_topSectionLayout.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.GetComponent<BomberUpgradeCategorySelection>().SetUp(usc.GetNamedText(), usc.GetFilters(), m_aircraftUpgradeTypeFilter);
			SelectableFilterButton newButton = gameObject.GetComponent<SelectableFilterButton>();
			newButton.SetRelatedObject(usc);
			newButton.OnClick += delegate
			{
				foreach (SelectableFilterButton topRowButton in m_topRowButtons)
				{
					topRowButton.SetSelected(topRowButton == newButton);
				}
				TopRowClick(usc);
				Singleton<TopBarInfoQueue>.Instance.RemoveRequest(Singleton<TopBarInfoQueue>.Instance.GetCurrentlyShownRequest());
			};
			m_topRowButtons.Add(newButton);
			m_topRowObjects.Add(gameObject);
		}
		if (m_returnFromLivery)
		{
			foreach (SelectableFilterButton topRowButton2 in m_topRowButtons)
			{
				UpgradeScreenCategory upgradeScreenCategory = (UpgradeScreenCategory)topRowButton2.GetRelatedObject();
				if (upgradeScreenCategory.GetFilters().Contains(BomberUpgradeType.Livery))
				{
					topRowButton2.SetSelected(selected: true);
					TopRowClick(upgradeScreenCategory);
				}
				else
				{
					topRowButton2.SetSelected(selected: false);
				}
			}
		}
		m_topSectionLayout.RepositionChildren();
	}

	private void Clear()
	{
		ClearDetailSelection();
		ClearDetailRow();
		foreach (GameObject topRowObject in m_topRowObjects)
		{
			UnityEngine.Object.DestroyImmediate(topRowObject);
		}
		m_topRowObjects.Clear();
		m_topRowButtons.Clear();
	}

	private void ClearDetailRow()
	{
		foreach (GameObject detailRowObject in m_detailRowObjects)
		{
			UnityEngine.Object.DestroyImmediate(detailRowObject);
		}
		m_detailRowObjects.Clear();
		m_detailRowButtons.Clear();
	}

	private void ClearDetailSelection()
	{
		ClearPreview();
		foreach (GameObject upgradeRowObject in m_upgradeRowObjects)
		{
			UnityEngine.Object.DestroyImmediate(upgradeRowObject);
		}
		m_itemSelectActiveNode.SetActive(value: false);
		m_upgradeRowObjects.Clear();
		m_upgradeRowButtons.Clear();
	}

	private void ClearPreview()
	{
		if (m_currentPreviewSection != null && Singleton<PoolManager>.Instance != null)
		{
			Singleton<PoolManager>.Instance.ReturnToPool(m_currentPreviewSection);
		}
	}

	private void TopRowClick(UpgradeScreenCategory onCategory)
	{
		m_currentlySelectedCategory = onCategory;
		CreateDetailRow();
	}

	private void CreateDetailRow()
	{
		ClearDetailRow();
		if (m_currentlySelectedCategory == null)
		{
			return;
		}
		BomberRequirements.BomberEquipmentRequirement[] requirements = Singleton<GameFlow>.Instance.GetGameMode().GetBomberRequirements().GetRequirements();
		BomberUpgradeConfig currentConfig = Singleton<BomberContainer>.Instance.GetCurrentConfig();
		int intel = Singleton<SaveDataContainer>.Instance.Get().GetIntel();
		BomberRequirements.BomberEquipmentRequirement[] array = requirements;
		foreach (BomberRequirements.BomberEquipmentRequirement br in array)
		{
			if (!m_currentlySelectedCategory.GetFilters().Contains(br.GetUpgradeConfig()))
			{
				continue;
			}
			List<EquipmentUpgradeFittableBase> allOfType = Singleton<BomberUpgradeCatalogueLoader>.Instance.GetCatalogue().GetAllOfType(br.GetUpgradeConfig());
			int num = (br.CanBeNull() ? 1 : 0);
			foreach (EquipmentUpgradeFittableBase item in allOfType)
			{
				if (item != null && item.IsDisplayableFor(br) && intel >= item.GetIntelShowRequirement())
				{
					num++;
				}
			}
			if (num <= 1)
			{
				continue;
			}
			GameObject detailSectionPrefab = m_detailSectionPrefab;
			GameObject gameObject = UnityEngine.Object.Instantiate(detailSectionPrefab);
			gameObject.transform.parent = m_detailSectionLayout.transform;
			gameObject.transform.localPosition = Vector3.zero;
			m_detailRowObjects.Add(gameObject);
			SelectableFilterButton newButton = gameObject.GetComponent<SelectableFilterButton>();
			gameObject.GetComponent<BomberUpgradeRequirementSelection>().SetUp(br, currentConfig, m_aircraftUpgradeTypeFilter);
			m_detailRowButtons.Add(newButton);
			newButton.SetRelatedObject(br);
			newButton.OnClick += delegate
			{
				foreach (SelectableFilterButton detailRowButton in m_detailRowButtons)
				{
					detailRowButton.SetSelected(detailRowButton == newButton);
				}
				DetailRowClick(br);
			};
		}
		if (m_hasCreatedPurchaseableRow)
		{
			m_detailRowButtons[0].SetSelected(selected: true);
			DetailRowClick((BomberRequirements.BomberEquipmentRequirement)m_detailRowButtons[0].GetRelatedObject());
		}
		m_detailSectionLayout.RepositionChildren();
		if (!m_returnFromLivery)
		{
			return;
		}
		foreach (SelectableFilterButton detailRowButton2 in m_detailRowButtons)
		{
			BomberRequirements.BomberEquipmentRequirement bomberEquipmentRequirement = (BomberRequirements.BomberEquipmentRequirement)detailRowButton2.GetRelatedObject();
			if (m_liveryCategoryReturnFrom.IsDisplayableFor(bomberEquipmentRequirement))
			{
				detailRowButton2.SetSelected(selected: true);
				DetailRowClick(bomberEquipmentRequirement);
			}
			else
			{
				detailRowButton2.SetSelected(selected: false);
			}
		}
	}

	private void DetailRowClick(BomberRequirements.BomberEquipmentRequirement req)
	{
		m_hasCreatedPurchaseableRow = true;
		m_currentlySelectedRequirement = req;
		m_returnFromLivery = false;
		CreatePurchaseableRow();
		Singleton<AirbaseCameraController>.Instance.MoveCameraToLocation(Singleton<AirbaseCameraController>.Instance.GetCameraNodeFor(req.GetUniquePartId()));
	}

	private int SortEquipment(EquipmentUpgradeFittableBase eA, EquipmentUpgradeFittableBase eB)
	{
		if (eA.GetIntelUnlockRequirement() != eB.GetIntelUnlockRequirement())
		{
			return Mathf.Clamp(eA.GetIntelUnlockRequirement() - eB.GetIntelUnlockRequirement(), -1, 1);
		}
		if (eA.GetMarkNumber() == eB.GetMarkNumber())
		{
			if (eA.GetSortBump() == eB.GetSortBump())
			{
				return string.Compare(eA.GetNameTranslated(), eB.GetNameTranslated());
			}
			return Mathf.Clamp(eA.GetSortBump() - eB.GetSortBump(), -1, 1);
		}
		return Mathf.Clamp(eA.GetMarkNumber() - eB.GetMarkNumber(), -1, 1);
	}

	private void CreatePurchaseableRow()
	{
		ClearDetailSelection();
		if (m_currentlySelectedRequirement != null)
		{
			if (m_currentlySelectedRequirement.ShouldClearDepthToPreview())
			{
				m_depthClearCamera.clearFlags = CameraClearFlags.Depth;
			}
			else
			{
				m_depthClearCamera.clearFlags = CameraClearFlags.Nothing;
			}
			m_itemSelectActiveNode.SetActive(value: true);
			m_itemListTextTitle.SetTextFromLanguageString(m_currentlySelectedRequirement.GetNamedText());
			List<EquipmentUpgradeFittableBase> allOfType = Singleton<BomberUpgradeCatalogueLoader>.Instance.GetCatalogue().GetAllOfType(m_currentlySelectedRequirement.GetUpgradeConfig());
			allOfType.Sort(SortEquipment);
			if (m_currentlySelectedRequirement.CanBeNull())
			{
				allOfType.Insert(0, null);
			}
			BomberUpgradeConfig currentConfig = Singleton<BomberContainer>.Instance.GetCurrentConfig();
			int intel = Singleton<SaveDataContainer>.Instance.Get().GetIntel();
			EquipmentUpgradeFittableBase upgradeFor = currentConfig.GetUpgradeFor(m_currentlySelectedRequirement);
			int index = 0;
			int num = 0;
			foreach (EquipmentUpgradeFittableBase item in allOfType)
			{
				bool flag = true;
				if (m_useEndlessModeFilters && item != null && item.IsDLC() && !Singleton<EndlessModeGameFlow>.Instance.GetCurrentEndlessMode().AllowDLCItems())
				{
					flag = false;
				}
				if (item != null)
				{
					EquipmentUpgradeFittableBase.AircraftUpgradeType aircraftUpgradeType = item.GetAircraftUpgradeType();
					if (aircraftUpgradeType != 0 && aircraftUpgradeType != m_aircraftUpgradeTypeFilter)
					{
						flag = false;
					}
				}
				if ((!(item == null) && (!item.IsDisplayableFor(m_currentlySelectedRequirement) || !flag)) || (!(item == null) && intel < item.GetIntelShowRequirement()))
				{
					continue;
				}
				GameObject gameObject = UnityEngine.Object.Instantiate((!(item != null) || (!item.IsDLC() && !item.IsGiftIcon())) ? m_upgradeSectionPrefab : m_upgradeSectionPrefabDLC);
				gameObject.transform.parent = m_upgradeSectionLayout.transform;
				gameObject.transform.localPosition = Vector3.zero;
				SelectableFilterButton button = gameObject.GetComponent<SelectableFilterButton>();
				gameObject.GetComponent<BomberUpgradePurchaseableSelection>().SetUp(item, m_currentlySelectedRequirement, m_refundFactor, currentConfig);
				button.SetRelatedObject(item);
				button.OnClick += delegate
				{
					foreach (SelectableFilterButton upgradeRowButton in m_upgradeRowButtons)
					{
						upgradeRowButton.SetSelected(upgradeRowButton == button);
					}
					SelectPurchaseable(button, item);
				};
				m_upgradeRowObjects.Add(gameObject);
				m_upgradeRowButtons.Add(button);
				if (upgradeFor == item)
				{
					index = num;
				}
				num++;
			}
			m_upgradeRowButtons[index].SetSelected(selected: true);
			SelectPurchaseable(m_upgradeRowButtons[index], (EquipmentUpgradeFittableBase)m_upgradeRowButtons[index].GetRelatedObject());
			m_upgradeSectionLayout.RepositionChildren();
			m_upgradeSectionScrollableArea.ContentLength = m_upgradeSectionScrollableArea.MeasureContentLength();
		}
		else
		{
			m_depthClearCamera.clearFlags = CameraClearFlags.Nothing;
			m_itemSelectActiveNode.SetActive(value: false);
		}
	}

	private void SelectPurchaseable(SelectableFilterButton fromButton, EquipmentUpgradeFittableBase item)
	{
		EquipmentUpgradeFittableBase currentlySelectedEquippable = m_currentlySelectedEquippable;
		m_currentlySelectedEquippable = item;
		if (m_currentPreviewSection != null)
		{
			Singleton<PoolManager>.Instance.ReturnToPool(m_currentPreviewSection);
		}
		bool flag = true;
		if (item != null && item.GetIntelUnlockRequirement() > Singleton<SaveDataContainer>.Instance.Get().GetIntel())
		{
			flag = false;
		}
		if (fromButton != null)
		{
			if (flag && item is RackSystemUpgrade)
			{
				m_currentPreviewSection = Singleton<PoolManager>.Instance.GetFromPoolSlowNoReparent(m_previewSectionPrefabRack);
			}
			else if (flag && item is LiveryEquippable)
			{
				m_currentPreviewSection = Singleton<PoolManager>.Instance.GetFromPoolSlowNoReparent(m_previewSectionPrefabLivery);
			}
			else
			{
				m_currentPreviewSection = Singleton<PoolManager>.Instance.GetFromPoolSlowNoReparent(m_previewSectionPrefab);
			}
			m_currentPreviewSection.transform.parent = m_previewSectionNode;
			m_currentPreviewSection.transform.localPosition = Vector3.zero;
			m_currentPreviewSection.GetComponent<BomberUpgradePurchaseablePreview>().SetUp(m_currentlySelectedEquippable, Singleton<BomberContainer>.Instance.GetCurrentConfig(), m_currentlySelectedRequirement, m_refundFactor, flag);
			m_currentPreviewSection.GetComponent<BomberUpgradePurchaseablePreview>().OnClick += delegate
			{
				AttemptPurchase();
			};
			Refresh();
			if (flag)
			{
				BomberSystemUniqueId[] allUniqueIds = m_allUniqueIds;
				foreach (BomberSystemUniqueId bomberSystemUniqueId in allUniqueIds)
				{
					if (bomberSystemUniqueId.GetUniqueId() == m_currentlySelectedRequirement.GetUniquePartId())
					{
						bomberSystemUniqueId.RefreshForPreview(m_currentlySelectedEquippable);
					}
				}
			}
			if (m_currentlySelectedCategory.GetFilters().Contains(BomberUpgradeType.Livery))
			{
				if (!flag)
				{
					Singleton<BomberContainer>.Instance.GetLivery().Refresh();
				}
				else if (m_currentlySelectedEquippable == null || m_currentlySelectedEquippable is LiveryEquippable)
				{
					Singleton<BomberContainer>.Instance.GetLivery().RefreshOverride(m_currentlySelectedRequirement.GetUniquePartId(), (LiveryEquippable)m_currentlySelectedEquippable);
				}
				else if (currentlySelectedEquippable == null || currentlySelectedEquippable is LiveryEquippable)
				{
					Singleton<BomberContainer>.Instance.GetLivery().Refresh();
				}
			}
			else if (currentlySelectedEquippable != null && currentlySelectedEquippable is LiveryEquippable)
			{
				Singleton<BomberContainer>.Instance.GetLivery().Refresh();
			}
		}
		RefreshWeight();
	}

	public void AttemptPurchase()
	{
		int num = ((!(m_currentlySelectedEquippable == null)) ? m_currentlySelectedEquippable.GetCost() : 0);
		EquipmentUpgradeFittableBase upgradeFor = Singleton<BomberContainer>.Instance.GetCurrentConfig().GetUpgradeFor(m_currentlySelectedRequirement);
		if (!(m_currentlySelectedEquippable != upgradeFor))
		{
			return;
		}
		int num2 = ((!(upgradeFor == null)) ? upgradeFor.GetCost() : 0);
		int num3 = Mathf.RoundToInt((float)num2 * m_refundFactor);
		int num4 = num - num3;
		BomberUpgradeConfig currentConfig = Singleton<BomberContainer>.Instance.GetCurrentConfig();
		BomberRequirements.BomberEquipmentRequirement[] requirements = Singleton<GameFlow>.Instance.GetGameMode().GetBomberRequirements().GetRequirements();
		int num5 = 0;
		int num6 = 0;
		BomberRequirements.BomberEquipmentRequirement[] array = requirements;
		foreach (BomberRequirements.BomberEquipmentRequirement bomberEquipmentRequirement in array)
		{
			if (bomberEquipmentRequirement == m_currentlySelectedRequirement)
			{
				if (m_currentlySelectedEquippable != null)
				{
					if (m_currentlySelectedEquippable is EngineUpgrade)
					{
						EngineUpgrade engineUpgrade = (EngineUpgrade)m_currentlySelectedEquippable;
						num5 += engineUpgrade.GetAdditionalWeightLimit();
					}
					else
					{
						num6 += m_currentlySelectedEquippable.GetWeight();
					}
				}
				continue;
			}
			string upgradeFor2 = currentConfig.GetUpgradeFor(bomberEquipmentRequirement.GetUniquePartId());
			if (string.IsNullOrEmpty(upgradeFor2))
			{
				continue;
			}
			EquipmentUpgradeFittableBase byName = Singleton<BomberUpgradeCatalogueLoader>.Instance.GetCatalogue().GetByName(upgradeFor2);
			if (byName != null)
			{
				if (byName is EngineUpgrade)
				{
					EngineUpgrade engineUpgrade2 = (EngineUpgrade)byName;
					num5 += engineUpgrade2.GetAdditionalWeightLimit();
				}
				int weight = byName.GetWeight();
				if (weight != -1)
				{
					num6 += weight;
				}
			}
		}
		int num7 = ((!(m_currentlySelectedEquippable == null)) ? m_currentlySelectedEquippable.GetIntelUnlockRequirement() : 0);
		if (Singleton<SaveDataContainer>.Instance.Get().GetBalance() < num4 || Singleton<SaveDataContainer>.Instance.Get().GetIntel() < num7)
		{
			return;
		}
		if (num6 <= num5)
		{
			Singleton<SaveDataContainer>.Instance.Get().AddBalance(num3);
			Singleton<SaveDataContainer>.Instance.Get().AddBalance(-num);
			Singleton<SaveDataContainer>.Instance.Get().SpendFunds(-num3);
			Singleton<SaveDataContainer>.Instance.Get().SpendFunds(num);
			Singleton<BomberContainer>.Instance.GetCurrentConfig().SetUpgrade(m_currentlySelectedRequirement.GetUniquePartId(), m_currentlySelectedEquippable);
			WingroveRoot.Instance.PostEvent("BUILD_UPGRADE");
			Singleton<AirbaseNavigation>.Instance.SetCrewPhotoRequiresRefresh();
			if (m_currentlySelectedEquippable == null || m_currentlySelectedEquippable.GetUpgradeType() == BomberUpgradeType.Livery)
			{
				Singleton<BomberContainer>.Instance.GetLivery().Refresh();
			}
			Refresh();
		}
		else
		{
			UIPopupData uIPopupData = new UIPopupData();
			uIPopupData.PopupStartCallback = (Action<UIPopUp>)Delegate.Combine(uIPopupData.PopupStartCallback, (Action<UIPopUp>)delegate(UIPopUp pop)
			{
				pop.GetComponent<GenericInfoTextPopup>().SetText(m_weightLimitOverText);
			});
			Singleton<UIPopupManager>.Instance.DisplayPopup(m_textPopupPrefab, uIPopupData);
		}
	}

	public void Refresh()
	{
		BomberUpgradeConfig currentConfig = Singleton<BomberContainer>.Instance.GetCurrentConfig();
		foreach (SelectableFilterButton detailRowButton in m_detailRowButtons)
		{
			detailRowButton.Refresh();
		}
		foreach (SelectableFilterButton upgradeRowButton in m_upgradeRowButtons)
		{
			upgradeRowButton.Refresh();
		}
		foreach (SelectableFilterButton topRowButton in m_topRowButtons)
		{
			topRowButton.Refresh();
		}
		if (m_currentPreviewSection != null)
		{
			m_currentPreviewSection.GetComponent<BomberUpgradePurchaseablePreview>().Refresh();
		}
		RefreshWeight();
		BomberSystemUniqueId[] allUniqueIds = m_allUniqueIds;
		foreach (BomberSystemUniqueId bomberSystemUniqueId in allUniqueIds)
		{
			bomberSystemUniqueId.RefreshPart();
		}
		Singleton<AirbaseNavigation>.Instance.Refresh();
	}

	private void RefreshWeight()
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		BomberUpgradeConfig currentConfig = Singleton<BomberContainer>.Instance.GetCurrentConfig();
		BomberRequirements.BomberEquipmentRequirement[] requirements = Singleton<GameFlow>.Instance.GetGameMode().GetBomberRequirements().GetRequirements();
		BomberRequirements.BomberEquipmentRequirement[] array = requirements;
		foreach (BomberRequirements.BomberEquipmentRequirement bomberEquipmentRequirement in array)
		{
			string upgradeFor = currentConfig.GetUpgradeFor(bomberEquipmentRequirement.GetUniquePartId());
			if (string.IsNullOrEmpty(upgradeFor))
			{
				continue;
			}
			EquipmentUpgradeFittableBase byName = Singleton<BomberUpgradeCatalogueLoader>.Instance.GetCatalogue().GetByName(upgradeFor);
			if (byName != null)
			{
				if (byName is EngineUpgrade)
				{
					EngineUpgrade engineUpgrade = (EngineUpgrade)byName;
					num2 += engineUpgrade.GetAdditionalWeightLimit();
				}
				int weight = byName.GetWeight();
				if (weight != -1)
				{
					num += weight;
				}
				num4 += byName.GetSurvivalPointsSea();
				num3 += byName.GetSurvivalPointsLand();
			}
		}
		int num5 = num;
		int num6 = num2;
		int num7 = num4;
		int num8 = num3;
		if (m_currentlySelectedRequirement != null && m_currentPreviewSection != null)
		{
			EquipmentUpgradeFittableBase upgradeFor2 = currentConfig.GetUpgradeFor(m_currentlySelectedRequirement);
			int num9 = ((!(upgradeFor2 == null)) ? upgradeFor2.GetWeight() : 0);
			int num10 = ((!(m_currentlySelectedEquippable == null)) ? m_currentlySelectedEquippable.GetWeight() : 0);
			int num11 = ((!(upgradeFor2 == null)) ? upgradeFor2.GetSurvivalPointsSea() : 0);
			int num12 = ((!(upgradeFor2 == null)) ? upgradeFor2.GetSurvivalPointsLand() : 0);
			int num13 = ((!(m_currentlySelectedEquippable == null)) ? m_currentlySelectedEquippable.GetSurvivalPointsSea() : 0);
			int num14 = ((!(m_currentlySelectedEquippable == null)) ? m_currentlySelectedEquippable.GetSurvivalPointsLand() : 0);
			num5 += num10;
			num5 -= num9;
			num7 += num13;
			num7 -= num11;
			num8 += num14;
			num8 -= num12;
			if (upgradeFor2 != null && upgradeFor2 is EngineUpgrade)
			{
				EngineUpgrade engineUpgrade2 = (EngineUpgrade)upgradeFor2;
				num6 -= engineUpgrade2.GetAdditionalWeightLimit();
			}
			if (m_currentlySelectedEquippable != null && m_currentlySelectedEquippable is EngineUpgrade)
			{
				EngineUpgrade engineUpgrade3 = (EngineUpgrade)m_currentlySelectedEquippable;
				num6 += engineUpgrade3.GetAdditionalWeightLimit();
			}
		}
		StatHelper.StatInfo left = StatHelper.StatInfo.CreateSmallerBetter(num, null);
		StatHelper.StatInfo statInfo = StatHelper.StatInfo.CreateSmallerBetter(num5, null);
		StatHelper.StatInfo left2 = StatHelper.StatInfo.Create(num2, null);
		StatHelper.StatInfo statInfo2 = StatHelper.StatInfo.Create(num6, null);
		StatHelper.StatInfo left3 = StatHelper.StatInfo.Create(num4, null);
		StatHelper.StatInfo right = StatHelper.StatInfo.Create(num7, null);
		StatHelper.StatInfo left4 = StatHelper.StatInfo.Create(num3, null);
		StatHelper.StatInfo right2 = StatHelper.StatInfo.Create(num8, null);
		Singleton<StatHelper>.Instance.SetStatStringComparePreviewChange(left, statInfo, "{0:N0}", "{0:N0}", m_weightTotalText, m_weightTotalChangeText);
		Singleton<StatHelper>.Instance.SetStatStringComparePreviewChange(left2, statInfo2, "{0:N0}", "{0:N0}", m_weightLimitText, m_weightLimitChangeText);
		if (statInfo.m_value > statInfo2.m_value)
		{
			m_weightLimitText.SetColor(m_weightLimitBadColor);
			m_weightTotalText.SetColor(m_weightLimitBadColor);
		}
		else
		{
			m_weightLimitText.SetColor(m_weightLimitOKColor);
			m_weightTotalText.SetColor(m_weightLimitOKColor);
		}
		Singleton<StatHelper>.Instance.SetStatStringComparePreviewChange(left3, right, "{0}", "{0}", m_survivalSeaText, m_survivalSeaTextChange);
		Singleton<StatHelper>.Instance.SetStatStringComparePreviewChange(left4, right2, "{0}", "{0}", m_survivalLandText, m_survivalLandTextChange);
	}

	private void OnAcceptButton(bool down)
	{
		if (down)
		{
			return;
		}
		if (Singleton<UISelector>.Instance.GetCurrentFinder() == m_topFinder)
		{
			Singleton<UISelector>.Instance.SetFinder(m_detailFinder);
		}
		else if (Singleton<UISelector>.Instance.GetCurrentFinder() == m_detailFinder)
		{
			Singleton<UISelector>.Instance.SetFinder(m_purchaseableFinder);
		}
		else if (Singleton<UISelector>.Instance.GetCurrentFinder() == m_purchaseableFinder)
		{
			if (m_currentlySelectedEquippable != null && m_currentlySelectedEquippable is RackSystemUpgrade)
			{
				EquipmentUpgradeFittableBase upgradeFor = Singleton<BomberContainer>.Instance.GetCurrentConfig().GetUpgradeFor(m_currentlySelectedRequirement);
				if (upgradeFor == m_currentlySelectedEquippable)
				{
					Singleton<UISelector>.Instance.SetFinder(m_previewFinder);
					BomberUpgradePurchaseablePreviewRack component = m_currentPreviewSection.GetComponent<BomberUpgradePurchaseablePreviewRack>();
					if (component != null)
					{
						component.SetIsEditing(isEditing: true);
					}
				}
			}
			AttemptPurchase();
		}
		else if (Singleton<UISelector>.Instance.GetCurrentFinder() != m_previewFinder)
		{
			Singleton<UISelector>.Instance.SetFinder(m_topFinder);
		}
	}

	private void OnBackButton()
	{
		if (Singleton<UISelector>.Instance.GetCurrentFinder() == m_detailFinder)
		{
			Singleton<UISelector>.Instance.SetFinder(m_topFinder);
		}
		else if (Singleton<UISelector>.Instance.GetCurrentFinder() == m_purchaseableFinder)
		{
			Singleton<UISelector>.Instance.SetFinder(m_detailFinder);
		}
		else
		{
			if (!(Singleton<UISelector>.Instance.GetCurrentFinder() == m_previewFinder))
			{
				return;
			}
			Singleton<UISelector>.Instance.SetFinder(m_purchaseableFinder);
			if (m_currentlySelectedEquippable != null && m_currentlySelectedEquippable is RackSystemUpgrade && m_currentPreviewSection != null)
			{
				BomberUpgradePurchaseablePreviewRack component = m_currentPreviewSection.GetComponent<BomberUpgradePurchaseablePreviewRack>();
				if (component != null)
				{
					component.SetIsEditing(isEditing: false);
				}
			}
		}
	}

	private void Update()
	{
		if (Singleton<UISelector>.Instance.GetCurrentFinder() == m_previewFinder)
		{
			EquipmentUpgradeFittableBase upgradeFor = Singleton<BomberContainer>.Instance.GetCurrentConfig().GetUpgradeFor(m_currentlySelectedRequirement);
			if (m_currentlySelectedEquippable == null || !(m_currentlySelectedEquippable is RackSystemUpgrade) || upgradeFor != m_currentlySelectedEquippable)
			{
				Singleton<UISelector>.Instance.SetFinder(m_purchaseableFinder);
			}
		}
	}
}
