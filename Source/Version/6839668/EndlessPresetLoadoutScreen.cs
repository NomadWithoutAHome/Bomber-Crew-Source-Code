using System;
using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class EndlessPresetLoadoutScreen : MonoBehaviour
{
	[SerializeField]
	private GameObject m_presetPrefab;

	[SerializeField]
	private LayoutGrid m_presetsLayout;

	[SerializeField]
	private LayoutGrid m_customSlotsLayout;

	[SerializeField]
	private int m_maxSlots = 3;

	[SerializeField]
	private GameObject m_airbaseLancaster;

	[SerializeField]
	private GameObject m_loadSavePresetPrefab;

	[SerializeField]
	private GameObject m_confirmPrefab;

	private List<GameObject> m_createdObjects = new List<GameObject>();

	private BomberSystemUniqueId[] m_allUniqueIds;

	private List<EndlessModeLoadoutDisplay> m_endlessModeDisplaysCustom = new List<EndlessModeLoadoutDisplay>();

	private void Start()
	{
		m_allUniqueIds = m_airbaseLancaster.GetComponentsInChildren<BomberSystemUniqueId>(includeInactive: true);
	}

	private void OnEnable()
	{
		Clear();
		CreatePresets();
	}

	private void Clear()
	{
		foreach (GameObject createdObject in m_createdObjects)
		{
			UnityEngine.Object.DestroyImmediate(createdObject);
		}
	}

	private void CreatePresets()
	{
		EndlessModeVariant currentEndlessMode = Singleton<EndlessModeGameFlow>.Instance.GetCurrentEndlessMode();
		for (int i = 0; i < m_maxSlots; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(m_presetPrefab);
			gameObject.transform.parent = m_presetsLayout.transform;
			gameObject.transform.localPosition = Vector3.zero;
			EndlessModeVariant.Loadout loadout = currentEndlessMode.GetPreset(i);
			gameObject.GetComponent<EndlessModeLoadoutDisplay>().SetUp(loadout);
			gameObject.GetComponent<EndlessModeLoadoutDisplay>().EnableDeleteButton(enable: false);
			gameObject.GetComponent<EndlessModeLoadoutDisplay>().OnClick += delegate
			{
				ConfirmLoad(loadout);
			};
			m_createdObjects.Add(gameObject);
		}
		m_presetsLayout.RepositionChildren();
		m_endlessModeDisplaysCustom.Clear();
		for (int j = 0; j < m_maxSlots; j++)
		{
			GameObject gameObject2 = UnityEngine.Object.Instantiate(m_presetPrefab);
			gameObject2.transform.parent = m_customSlotsLayout.transform;
			gameObject2.transform.localPosition = Vector3.zero;
			EndlessModeVariant.LoadoutJS endlessModeCustomLoadout = Singleton<SystemDataContainer>.Instance.Get().GetEndlessModeCustomLoadout(currentEndlessMode.name, j, m_maxSlots);
			EndlessModeLoadoutDisplay component = gameObject2.GetComponent<EndlessModeLoadoutDisplay>();
			m_endlessModeDisplaysCustom.Add(component);
			component.SetUp(endlessModeCustomLoadout, currentEndlessMode.name, j);
			component.EnableDeleteButton(enable: true);
			int toSaveTo = j;
			component.OnClick += delegate
			{
				ClickCustomLoadout(toSaveTo);
			};
			gameObject2.GetComponent<EndlessModeLoadoutDisplay>().OnDelete += delegate
			{
				ConfirmDelete(toSaveTo);
			};
			m_createdObjects.Add(gameObject2);
		}
		m_customSlotsLayout.RepositionChildren();
	}

	private void ConfirmLoad(EndlessModeVariant.LoadoutJS loadout)
	{
		UIPopupData uIPopupData = new UIPopupData();
		uIPopupData.PopupStartCallback = (Action<UIPopUp>)Delegate.Combine(uIPopupData.PopupStartCallback, (Action<UIPopUp>)delegate(UIPopUp uip)
		{
			uip.GetComponent<GenericYesNoPrompt>().SetUp(string.Format(Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("endless_mode_presetchange_confirm"), loadout.m_title), Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("endless_mode_presetchange_confirm_load"), Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("endless_mode_presetchange_confirm_cancel"), danger: false);
			uip.GetComponent<GenericYesNoPrompt>().OnYes += delegate
			{
				DoEquip(loadout);
			};
		});
		Singleton<UIPopupManager>.Instance.DisplayPopup(m_confirmPrefab, uIPopupData);
	}

	private void ConfirmLoad(EndlessModeVariant.Loadout loadout)
	{
		UIPopupData uIPopupData = new UIPopupData();
		uIPopupData.PopupStartCallback = (Action<UIPopUp>)Delegate.Combine(uIPopupData.PopupStartCallback, (Action<UIPopUp>)delegate(UIPopUp uip)
		{
			uip.GetComponent<GenericYesNoPrompt>().SetUp(string.Format(Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("endless_mode_presetchange_confirm"), Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(loadout.m_title).ToUpper()), Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("endless_mode_presetchange_confirm_load"), Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("endless_mode_presetchange_confirm_cancel"), danger: false);
			uip.GetComponent<GenericYesNoPrompt>().OnYes += delegate
			{
				DoEquip(loadout);
			};
		});
		Singleton<UIPopupManager>.Instance.DisplayPopup(m_confirmPrefab, uIPopupData);
	}

	private void ConfirmDelete(int toIndex)
	{
		EndlessModeVariant variant = Singleton<EndlessModeGameFlow>.Instance.GetCurrentEndlessMode();
		EndlessModeVariant.LoadoutJS loadout = Singleton<SystemDataContainer>.Instance.Get().GetEndlessModeCustomLoadout(variant.name, toIndex, m_maxSlots);
		if (loadout == null)
		{
			return;
		}
		UIPopupData uIPopupData = new UIPopupData();
		uIPopupData.PopupStartCallback = (Action<UIPopUp>)Delegate.Combine(uIPopupData.PopupStartCallback, (Action<UIPopUp>)delegate(UIPopUp uip)
		{
			uip.GetComponent<GenericYesNoPrompt>().SetUp(string.Format(Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("endless_mode_presetchange_delete"), loadout.m_title), Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("endless_mode_presetchange_confirm_delete"), Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("endless_mode_presetchange_confirm_cancel"), danger: false);
			uip.GetComponent<GenericYesNoPrompt>().OnYes += delegate
			{
				Singleton<SystemDataContainer>.Instance.Get().SaveEndlessModeCustomLoadout(null, variant.name, toIndex, m_maxSlots);
				Singleton<SystemDataContainer>.Instance.Save();
				Refresh();
			};
		});
		Singleton<UIPopupManager>.Instance.DisplayPopup(m_confirmPrefab, uIPopupData);
	}

	private void DoInfoPopup(EndlessModeVariant.Loadout loadout)
	{
		UIPopupData uid = new UIPopupData();
		UIPopupData uIPopupData = uid;
		uIPopupData.PopupStartCallback = (Action<UIPopUp>)Delegate.Combine(uIPopupData.PopupStartCallback, (Action<UIPopUp>)delegate(UIPopUp uip)
		{
			uip.GetComponent<EndlessModePresetDisplayPopup>().SetUp(loadout);
			uip.GetComponent<EndlessModePresetDisplayPopup>().OnLoad += delegate
			{
				UIPopupData uIPopupData2 = uid;
				uIPopupData2.PopupDismissedCallback = (Action<UIPopUp>)Delegate.Combine(uIPopupData2.PopupDismissedCallback, (Action<UIPopUp>)delegate
				{
					ConfirmLoad(loadout);
				});
			};
		});
		Singleton<UIPopupManager>.Instance.DisplayPopup(m_loadSavePresetPrefab, uid);
	}

	private void DoInfoPopup(EndlessModeVariant.LoadoutJS loadout, string variant, int index)
	{
		UIPopupData uid = new UIPopupData();
		UIPopupData uIPopupData = uid;
		uIPopupData.PopupStartCallback = (Action<UIPopUp>)Delegate.Combine(uIPopupData.PopupStartCallback, (Action<UIPopUp>)delegate(UIPopUp uip)
		{
			uip.GetComponent<EndlessModePresetDisplayPopup>().SetUp(loadout, variant, index);
			uip.GetComponent<EndlessModePresetDisplayPopup>().OnLoad += delegate
			{
				UIPopupData uIPopupData2 = uid;
				uIPopupData2.PopupDismissedCallback = (Action<UIPopUp>)Delegate.Combine(uIPopupData2.PopupDismissedCallback, (Action<UIPopUp>)delegate
				{
					ConfirmLoad(loadout);
				});
			};
			uip.GetComponent<EndlessModePresetDisplayPopup>().OnSave += delegate(Texture2D tex, string saveName)
			{
				SaveLoadOut(index, tex, saveName);
			};
		});
		Singleton<UIPopupManager>.Instance.DisplayPopup(m_loadSavePresetPrefab, uid);
	}

	private void ClickCustomLoadout(int index)
	{
		EndlessModeVariant currentEndlessMode = Singleton<EndlessModeGameFlow>.Instance.GetCurrentEndlessMode();
		EndlessModeVariant.LoadoutJS endlessModeCustomLoadout = Singleton<SystemDataContainer>.Instance.Get().GetEndlessModeCustomLoadout(currentEndlessMode.name, index, m_maxSlots);
		DoInfoPopup(endlessModeCustomLoadout, currentEndlessMode.name, index);
	}

	private void Refresh()
	{
		EndlessModeVariant currentEndlessMode = Singleton<EndlessModeGameFlow>.Instance.GetCurrentEndlessMode();
		for (int i = 0; i < m_maxSlots; i++)
		{
			EndlessModeVariant.LoadoutJS endlessModeCustomLoadout = Singleton<SystemDataContainer>.Instance.Get().GetEndlessModeCustomLoadout(currentEndlessMode.name, i, m_maxSlots);
			m_endlessModeDisplaysCustom[i].SetUp(endlessModeCustomLoadout, currentEndlessMode.name, i);
		}
	}

	private void SaveLoadOut(int toIndex, Texture2D tex, string saveName)
	{
		EndlessModeVariant currentEndlessMode = Singleton<EndlessModeGameFlow>.Instance.GetCurrentEndlessMode();
		int currentCrewCount = Singleton<CrewContainer>.Instance.GetCurrentCrewCount();
		Crewman[] array = new Crewman[currentCrewCount];
		for (int i = 0; i < currentCrewCount; i++)
		{
			array[i] = Singleton<CrewContainer>.Instance.GetCrewman(i);
		}
		EndlessModeVariant.LoadoutJS data = new EndlessModeVariant.LoadoutJS(Singleton<BomberContainer>.Instance.GetCurrentConfig(), array, saveName, tex, currentEndlessMode.name, toIndex);
		Singleton<SystemDataContainer>.Instance.Get().SaveEndlessModeCustomLoadout(data, currentEndlessMode.name, toIndex, m_maxSlots);
		Singleton<SystemDataContainer>.Instance.Save();
		Refresh();
	}

	private void DoEquip(EndlessModeVariant.Loadout loadout)
	{
		EndlessModeVariant currentEndlessMode = Singleton<EndlessModeGameFlow>.Instance.GetCurrentEndlessMode();
		Singleton<EndlessModeGameFlow>.Instance.SetLoadout(loadout, currentEndlessMode.GetPreset(0));
		BomberSystemUniqueId[] allUniqueIds = m_allUniqueIds;
		foreach (BomberSystemUniqueId bomberSystemUniqueId in allUniqueIds)
		{
			bomberSystemUniqueId.RefreshPart();
		}
		for (int j = 0; j < Singleton<CrewContainer>.Instance.GetCurrentCrewCount(); j++)
		{
			Crewman crewman = Singleton<CrewContainer>.Instance.GetCrewman(j);
			Singleton<EndlessModeAirbase>.Instance.GetPersistentCrew().GetCrewmanAvatar(crewman).GetComponent<CrewQuartersCrewmanAvatar>()
				.RefreshEquipment();
		}
	}

	private void DoEquip(EndlessModeVariant.LoadoutJS loadout)
	{
		EndlessModeVariant currentEndlessMode = Singleton<EndlessModeGameFlow>.Instance.GetCurrentEndlessMode();
		Singleton<EndlessModeGameFlow>.Instance.SetLoadout(loadout, currentEndlessMode.GetPreset(0));
		BomberSystemUniqueId[] allUniqueIds = m_allUniqueIds;
		foreach (BomberSystemUniqueId bomberSystemUniqueId in allUniqueIds)
		{
			bomberSystemUniqueId.RefreshPart();
		}
		for (int j = 0; j < Singleton<CrewContainer>.Instance.GetCurrentCrewCount(); j++)
		{
			Crewman crewman = Singleton<CrewContainer>.Instance.GetCrewman(j);
			Singleton<EndlessModeAirbase>.Instance.GetPersistentCrew().GetCrewmanAvatar(crewman).GetComponent<CrewQuartersCrewmanAvatar>()
				.RefreshEquipment();
		}
	}
}
