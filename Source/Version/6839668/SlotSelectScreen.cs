using System;
using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class SlotSelectScreen : MonoBehaviour
{
	[SerializeField]
	private AirbaseCameraNode m_thisCameraNode;

	[SerializeField]
	private GameObject m_slotDisplayPrefab;

	[SerializeField]
	private GameObject m_selectToLoadHierarchy;

	[SerializeField]
	private GameObject m_selectToNewHierarchy;

	[SerializeField]
	private Transform m_slotHierarchy;

	[SerializeField]
	private LayoutGrid m_slotLayoutGrid;

	[SerializeField]
	private tk2dUIItem m_backButton;

	[SerializeField]
	private GameObject m_saveConfirmPopup;

	[SerializeField]
	private UISelectFinder m_finder;

	private List<GameObject> m_createdSlots = new List<GameObject>();

	private bool m_isLoading;

	private bool m_skipIntro;

	private void Start()
	{
		m_backButton.OnClick += Back;
	}

	private void OnEnable()
	{
		Singleton<AirbaseCameraController>.Instance.MoveCameraToLocation(m_thisCameraNode);
		Singleton<UISelector>.Instance.SetFinder(m_finder);
		Singleton<MainActionButtonMonitor>.Instance.AddListener(ButtonListener);
	}

	private void OnDisable()
	{
		Singleton<MainActionButtonMonitor>.Instance.RemoveListener(ButtonListener, invalidateCurrentPress: false);
	}

	private bool ButtonListener(MainActionButtonMonitor.ButtonPress bp)
	{
		if (bp == MainActionButtonMonitor.ButtonPress.Back)
		{
			Back();
		}
		return true;
	}

	private void Back()
	{
		Singleton<UIScreenManager>.Instance.ShowScreen("CampaignMenuScreen", showNavBarButtons: true);
	}

	public void SetUp(bool isLoading, bool skipIntro)
	{
		m_isLoading = isLoading;
		m_skipIntro = skipIntro;
		foreach (GameObject createdSlot in m_createdSlots)
		{
			UnityEngine.Object.DestroyImmediate(createdSlot);
		}
		m_createdSlots.Clear();
		List<SaveSlotSelection> list = new List<SaveSlotSelection>();
		for (int i = 0; i < Singleton<SaveDataContainer>.Instance.GetMaxSlots(); i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(m_slotDisplayPrefab);
			gameObject.transform.parent = m_slotHierarchy;
			gameObject.transform.localPosition = Vector3.zero;
			SaveSlotSelection component = gameObject.GetComponent<SaveSlotSelection>();
			component.SetUp(i, isLoading);
			component.OnSaveSelect += SaveSelected;
			list.Add(component);
			m_createdSlots.Add(gameObject);
		}
		m_selectToLoadHierarchy.SetActive(isLoading);
		m_selectToNewHierarchy.SetActive(!isLoading);
		SaveSlotSelection saveSlotSelection = null;
		if (isLoading)
		{
			DateTime dateTime = default(DateTime);
			foreach (SaveSlotSelection item in list)
			{
				if (saveSlotSelection == null)
				{
					dateTime = item.GetDateTime();
					saveSlotSelection = item;
				}
				else if (item.GetDateTime() > dateTime)
				{
					saveSlotSelection = item;
					dateTime = item.GetDateTime();
				}
			}
		}
		else
		{
			foreach (SaveSlotSelection item2 in list)
			{
				if (!item2.GetIsFilled())
				{
					saveSlotSelection = item2;
					break;
				}
			}
		}
		if (saveSlotSelection != null)
		{
			saveSlotSelection.GetComponent<UISelectorPointingHint>().SetDefault();
		}
		m_slotLayoutGrid.RepositionChildren();
	}

	private void SaveSelected(int index)
	{
		if (m_isLoading)
		{
			SaveData sd = Singleton<SaveDataContainer>.Instance.TempLoadSlot(index);
			if (sd.GetNumMissionsPlayed() == 0 && Singleton<GameFlow>.Instance.GetGameMode().HasTutorial())
			{
				m_skipIntro = false;
				DoNewGame(index);
			}
			else if (sd.HasCompletedMission("C08_KEY") || sd.HasCompletedMission("DLCMP02_C05_KEY"))
			{
				UIPopupData uIPopupData = new UIPopupData();
				uIPopupData.PopupStartCallback = (Action<UIPopUp>)Delegate.Combine(uIPopupData.PopupStartCallback, (Action<UIPopUp>)delegate(UIPopUp u)
				{
					OnContinuePopupDisplay(u, index, sd);
				});
				Singleton<UIPopupManager>.Instance.DisplayPopup(m_saveConfirmPopup, uIPopupData);
			}
			else
			{
				CheckDLCLoad(index, sd);
			}
		}
		else if (Singleton<SaveDataContainer>.Instance.Exists(index))
		{
			UIPopupData uIPopupData2 = new UIPopupData();
			uIPopupData2.PopupStartCallback = (Action<UIPopUp>)Delegate.Combine(uIPopupData2.PopupStartCallback, (Action<UIPopUp>)delegate(UIPopUp u)
			{
				OnPopupDisplay(u, index);
			});
			Singleton<UIPopupManager>.Instance.DisplayPopup(m_saveConfirmPopup, uIPopupData2);
		}
		else
		{
			DoNewGame(index);
		}
	}

	private void CheckDLCLoad(int index, SaveData sd)
	{
		if (sd.ShouldShowDLCWarning(Singleton<DLCManager>.Instance.GetCurrentInstalledDLCs()))
		{
			UIPopupData uIPopupData = new UIPopupData();
			uIPopupData.PopupStartCallback = (Action<UIPopUp>)Delegate.Combine(uIPopupData.PopupStartCallback, (Action<UIPopUp>)delegate(UIPopUp u)
			{
				OnDLCCheckPopUp(u, index, sd);
			});
			Singleton<UIPopupManager>.Instance.DisplayPopup(m_saveConfirmPopup, uIPopupData);
		}
		else
		{
			Singleton<GameFlow>.Instance.LoadGame(index);
		}
	}

	private void OnDLCCheckPopUp(UIPopUp uip, int index, SaveData sd)
	{
		uip.GetComponent<GenericYesNoPrompt>().SetUp(Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("ui_system_dlc_warning"), Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("ui_system_dlc_warning_continue"), Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("ui_system_dlc_warning_cancel"), danger: false);
		uip.GetComponent<GenericYesNoPrompt>().OnYes += delegate
		{
			Singleton<GameFlow>.Instance.LoadGame(index);
		};
	}

	private void OnContinuePopupDisplay(UIPopUp uip, int index, SaveData sd)
	{
		uip.GetComponent<GenericYesNoPrompt>().SetUp(Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("ui_save_menu_confirm_continue_prompt"), Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("ui_save_menu_continue"), Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("ui_save_menu_cancel"), danger: false);
		uip.GetComponent<GenericYesNoPrompt>().OnYes += delegate
		{
			CheckDLCLoad(index, sd);
		};
	}

	private void OnPopupDisplay(UIPopUp uip, int index)
	{
		uip.GetComponent<GenericYesNoPrompt>().SetUp(Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("ui_save_menu_confirm_overwrite_prompt"), Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("ui_save_menu_overwrite"), Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("ui_save_menu_cancel"), danger: true);
		uip.GetComponent<GenericYesNoPrompt>().OnYes += delegate
		{
			DoNewGame(index);
		};
	}

	private void DoNewGame(int index)
	{
		if (m_skipIntro || !Singleton<GameFlow>.Instance.GetGameMode().HasTutorial())
		{
			Singleton<GameFlow>.Instance.StartNewGameSkip(index);
		}
		else
		{
			Singleton<GameFlow>.Instance.StartNewGame(index);
		}
	}
}
