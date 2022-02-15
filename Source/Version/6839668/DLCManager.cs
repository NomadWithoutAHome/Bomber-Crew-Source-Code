using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using BomberCrewCommon;
using UnityEngine;

public class DLCManager : Singleton<DLCManager>
{
	[Serializable]
	public class UpdatedFileOverride
	{
		[SerializeField]
		public string m_fromDLCPack;

		[SerializeField]
		public UnityEngine.Object m_file;
	}

	[SerializeField]
	private string[] m_searchForDLCPacks;

	[SerializeField]
	private string[] m_ps4Entitlements;

	[SerializeField]
	private string m_windowsPath = "PCBundles";

	[SerializeField]
	private string m_linuxPath = "LinuxBundles";

	[SerializeField]
	private string m_macPath = "MacBundles";

	[SerializeField]
	private GameObject m_loadingDLCPopup;

	[SerializeField]
	private GameObject m_genericYesNoPopup;

	[SerializeField]
	private bool[] m_fakeAssetBundlesExist;

	[SerializeField]
	private int[] m_switchBundleIndexExpectations;

	[SerializeField]
	private UpdatedFileOverride[] m_fileUpdatePatchHacks;

	private List<string> m_currentInstalledDLCs = new List<string>();

	private List<AssetBundle> m_allLoadedBundles = new List<AssetBundle>();

	private Dictionary<string, Dictionary<string, UnityEngine.Object>> m_fileUpdatePatchDictionary = new Dictionary<string, Dictionary<string, UnityEngine.Object>>();

	private bool m_updateDictionaryCreated;

	private bool m_requiresReload;

	private bool m_reloadDeferred;

	private bool m_isLoading;

	private bool[] m_fakeAssetBundlesExistChecked;

	private bool m_isShowingDLCLoadingPopup;

	private Dictionary<string, Dictionary<string, UnityEngine.Object>> m_allObjectsNamed = new Dictionary<string, Dictionary<string, UnityEngine.Object>>();

	private Dictionary<string, string> m_versionInfo = new Dictionary<string, string>();

	private bool m_dlcPackIsOutOfDate;

	private uint m_numMarketPlaceItemsFound;

	public event Action<string> OnDLCUpdate;

	private bool DoesFakeAssetBundleExist(int index)
	{
		if (m_fakeAssetBundlesExist.Length > index)
		{
			return m_fakeAssetBundlesExist[index];
		}
		return true;
	}

	private bool HasFakeAssetBundleBeenSeen(int index)
	{
		return m_fakeAssetBundlesExistChecked[index];
	}

	private void ShowDLCLoadingPopup(bool show)
	{
		if (Singleton<UIPopupManager>.Instance != null && m_isShowingDLCLoadingPopup != show)
		{
			if (show)
			{
				Singleton<UIPopupManager>.Instance.DisplayPopup(m_loadingDLCPopup);
			}
			else
			{
				Singleton<UIPopupManager>.Instance.DismissCurrentPopup();
			}
			m_isShowingDLCLoadingPopup = show;
		}
	}

	private IEnumerator LoadAssetBundleAll(AssetBundle ab, string abName)
	{
		ShowDLCLoadingPopup(show: true);
		AssetBundleRequest abr = ab.LoadAllAssetsAsync();
		while (!abr.isDone)
		{
			yield return null;
		}
		m_allObjectsNamed[abName] = new Dictionary<string, UnityEngine.Object>();
		m_versionInfo[abName] = "L";
		UnityEngine.Object[] allAssets = abr.allAssets;
		foreach (UnityEngine.Object @object in allAssets)
		{
			m_allObjectsNamed[abName][@object.name] = @object;
			if (@object.name == "DLCPackageVersion")
			{
				TextAsset textAsset = (TextAsset)@object;
				m_versionInfo[abName] = textAsset.text;
			}
		}
	}

	public string GetVersionInfo(string abName)
	{
		string value = null;
		m_versionInfo.TryGetValue(abName, out value);
		if (!string.IsNullOrEmpty(value))
		{
			return value;
		}
		return string.Empty;
	}

	public string GetVersionInfoLong()
	{
		string text = string.Empty;
		foreach (KeyValuePair<string, string> item in m_versionInfo)
		{
			int num = 0;
			int num2 = -1;
			string[] searchForDLCPacks = m_searchForDLCPacks;
			foreach (string text2 in searchForDLCPacks)
			{
				if (text2 == item.Key)
				{
					num2 = num;
					break;
				}
				num++;
			}
			string text3 = text;
			text = text3 + "[" + num2 + ":" + item.Value + "]";
		}
		return text;
	}

	public List<string> GetCurrentInstalledDLCs()
	{
		return m_currentInstalledDLCs;
	}

	private IEnumerator LoadAB(string bundleName, int zeroBasedIndex)
	{
		bool loaded = false;
		string assetBundlePath = Application.dataPath + "/" + m_windowsPath + "/" + bundleName;
		if (assetBundlePath != null && !loaded && File.Exists(assetBundlePath))
		{
			ShowDLCLoadingPopup(show: true);
			AssetBundle ab = AssetBundle.LoadFromFile(assetBundlePath);
			if (ab != null)
			{
				yield return StartCoroutine(LoadAssetBundleAll(ab, bundleName));
				m_allLoadedBundles.Add(ab);
				loaded = true;
			}
		}
		if (loaded)
		{
			m_currentInstalledDLCs.Add(bundleName);
		}
	}

	public T LoadAssetFromBundle<T>(string bundleName, string assetName) where T : UnityEngine.Object
	{
		if (m_currentInstalledDLCs.Contains(bundleName))
		{
			Dictionary<string, UnityEngine.Object> value = null;
			m_allObjectsNamed.TryGetValue(bundleName, out value);
			if (value != null)
			{
				UnityEngine.Object value2 = null;
				value.TryGetValue(assetName, out value2);
				return value2 as T;
			}
		}
		return (T)null;
	}

	public void DoContentLoad()
	{
		if ((Singleton<UIPopupManager>.Instance == null || !Singleton<UIPopupManager>.Instance.IsCurrentlyShowingPopup) && !Singleton<GameFlow>.Instance.IsLoading() && Singleton<SystemLoader>.Instance.IsLoadComplete())
		{
			StartCoroutine(ContentLoadCoroutine());
		}
	}

	public bool IsLoading()
	{
		return m_isLoading;
	}

	public bool IsDLCOutOfDate()
	{
		return m_dlcPackIsOutOfDate;
	}

	public void DoDLCReloadCheck()
	{
		if (!m_requiresReload || m_reloadDeferred || Singleton<UIPopupManager>.Instance.IsCurrentlyShowingPopup)
		{
			return;
		}
		UIPopupData uIPopupData = new UIPopupData();
		uIPopupData.PopupStartCallback = (Action<UIPopUp>)Delegate.Combine(uIPopupData.PopupStartCallback, (Action<UIPopUp>)delegate(UIPopUp uip)
		{
			GenericYesNoPrompt component = uip.GetComponent<GenericYesNoPrompt>();
			component.SetUp(Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("consolesystem_new_dlc"), Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("consolesystem_new_dlc_yes"), Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("consolesystem_new_dlc_no"), danger: false);
			component.OnYes += delegate
			{
				Singleton<GameFlow>.Instance.SetPaused(paused: false);
				m_reloadDeferred = true;
				Singleton<SaveDataContainer>.Instance.Save();
				Singleton<AirbaseNavigation>.Instance.SaveCrewPhoto(instant: true);
				Singleton<SystemDataContainer>.Instance.Save();
				Singleton<GameFlow>.Instance.ReturnToMainMenu();
			};
			component.OnNo += delegate
			{
				Time.timeScale = 1f;
				m_reloadDeferred = true;
			};
		});
		Singleton<GameFlow>.Instance.SetPaused(paused: true);
		Singleton<UIPopupManager>.Instance.DisplayPopup(m_genericYesNoPopup, uIPopupData);
	}

	private IEnumerator ContentLoadCoroutine()
	{
		if (m_isLoading || !m_requiresReload)
		{
			yield break;
		}
		if (Singleton<UISelector>.Instance != null)
		{
			Singleton<UISelector>.Instance.Pause();
		}
		if (Singleton<InputLayerInterface>.Instance != null)
		{
			Singleton<InputLayerInterface>.Instance.DisableAllLayers();
		}
		m_isLoading = true;
		if (!m_updateDictionaryCreated)
		{
			UpdatedFileOverride[] fileUpdatePatchHacks = m_fileUpdatePatchHacks;
			foreach (UpdatedFileOverride updatedFileOverride in fileUpdatePatchHacks)
			{
				Dictionary<string, UnityEngine.Object> value = null;
				m_fileUpdatePatchDictionary.TryGetValue(updatedFileOverride.m_fromDLCPack, out value);
				if (value == null)
				{
					value = new Dictionary<string, UnityEngine.Object>();
					m_fileUpdatePatchDictionary[updatedFileOverride.m_fromDLCPack] = value;
				}
				value[updatedFileOverride.m_file.name] = updatedFileOverride.m_file;
			}
			m_updateDictionaryCreated = true;
		}
		while (m_requiresReload)
		{
			m_requiresReload = false;
			int index = 0;
			string[] searchForDLCPacks = m_searchForDLCPacks;
			foreach (string s in searchForDLCPacks)
			{
				if (!m_currentInstalledDLCs.Contains(s))
				{
					yield return StartCoroutine(LoadAB(s, index));
					yield return null;
					if (m_currentInstalledDLCs.Contains(s) && this.OnDLCUpdate != null)
					{
						this.OnDLCUpdate(s);
					}
					yield return null;
				}
				index++;
			}
		}
		ShowDLCLoadingPopup(show: false);
		if (Singleton<UISelector>.Instance != null)
		{
			Singleton<UISelector>.Instance.Resume();
		}
		if (Singleton<InputLayerInterface>.Instance != null)
		{
			Singleton<InputLayerInterface>.Instance.EnableAllLayers();
		}
		m_isLoading = false;
	}

	private void Awake()
	{
		m_fakeAssetBundlesExistChecked = new bool[m_searchForDLCPacks.Length];
		m_requiresReload = true;
	}
}
