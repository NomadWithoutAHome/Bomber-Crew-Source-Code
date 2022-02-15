using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace AssetBundles;

public class AssetBundleManager : MonoBehaviour
{
	public enum LogMode
	{
		All,
		JustErrors
	}

	public enum LogType
	{
		Info,
		Warning,
		Error
	}

	private static LogMode m_LogMode = LogMode.All;

	private static string m_BaseDownloadingURL = string.Empty;

	private static string[] m_ActiveVariants = new string[0];

	private static AssetBundleManifest m_AssetBundleManifest = null;

	private static Dictionary<string, LoadedAssetBundle> m_LoadedAssetBundles = new Dictionary<string, LoadedAssetBundle>();

	private static Dictionary<string, WWW> m_DownloadingWWWs = new Dictionary<string, WWW>();

	private static Dictionary<string, string> m_DownloadingErrors = new Dictionary<string, string>();

	private static List<AssetBundleLoadOperation> m_InProgressOperations = new List<AssetBundleLoadOperation>();

	private static Dictionary<string, string[]> m_Dependencies = new Dictionary<string, string[]>();

	public static LogMode logMode
	{
		get
		{
			return m_LogMode;
		}
		set
		{
			m_LogMode = value;
		}
	}

	public static string BaseDownloadingURL
	{
		get
		{
			return m_BaseDownloadingURL;
		}
		set
		{
			m_BaseDownloadingURL = value;
		}
	}

	public static string[] ActiveVariants
	{
		get
		{
			return m_ActiveVariants;
		}
		set
		{
			m_ActiveVariants = value;
		}
	}

	public static AssetBundleManifest AssetBundleManifestObject
	{
		set
		{
			m_AssetBundleManifest = value;
		}
	}

	private static void Log(LogType logType, string text)
	{
		if (logType == LogType.Error)
		{
			Debug.LogError("[AssetBundleManager] " + text);
		}
		else if (m_LogMode == LogMode.All)
		{
			Debug.Log("[AssetBundleManager] " + text);
		}
	}

	private static string GetStreamingAssetsPath()
	{
		if (Application.isEditor)
		{
			return "file://" + Environment.CurrentDirectory.Replace("\\", "/");
		}
		if (Application.isWebPlayer)
		{
			return Path.GetDirectoryName(Application.absoluteURL).Replace("\\", "/") + "/StreamingAssets";
		}
		if (Application.isMobilePlatform || Application.isConsolePlatform)
		{
			return Application.streamingAssetsPath;
		}
		return "file://" + Application.streamingAssetsPath;
	}

	public static void SetSourceAssetBundleDirectory(string relativePath)
	{
		BaseDownloadingURL = GetStreamingAssetsPath() + relativePath;
	}

	public static void SetSourceAssetBundleURL(string absolutePath)
	{
		BaseDownloadingURL = absolutePath + Utility.GetPlatformName() + "/";
	}

	public static void SetDevelopmentAssetBundleServer()
	{
		TextAsset textAsset = Resources.Load("AssetBundleServerURL") as TextAsset;
		string text = ((!(textAsset != null)) ? null : textAsset.text.Trim());
		if (text == null || text.Length == 0)
		{
			Debug.LogError("Development Server URL could not be found.");
		}
		else
		{
			SetSourceAssetBundleURL(text);
		}
	}

	public static LoadedAssetBundle GetLoadedAssetBundle(string assetBundleName, out string error)
	{
		if (m_DownloadingErrors.TryGetValue(assetBundleName, out error))
		{
			return null;
		}
		LoadedAssetBundle value = null;
		m_LoadedAssetBundles.TryGetValue(assetBundleName, out value);
		if (value == null)
		{
			return null;
		}
		string[] value2 = null;
		if (!m_Dependencies.TryGetValue(assetBundleName, out value2))
		{
			return value;
		}
		string[] array = value2;
		foreach (string key in array)
		{
			if (m_DownloadingErrors.TryGetValue(assetBundleName, out error))
			{
				return value;
			}
			m_LoadedAssetBundles.TryGetValue(key, out var value3);
			if (value3 == null)
			{
				return null;
			}
		}
		return value;
	}

	public static AssetBundleLoadManifestOperation Initialize()
	{
		return Initialize(Utility.GetPlatformName());
	}

	public static AssetBundleLoadManifestOperation Initialize(string manifestAssetBundleName)
	{
		GameObject target = new GameObject("AssetBundleManager", typeof(AssetBundleManager));
		UnityEngine.Object.DontDestroyOnLoad(target);
		LoadAssetBundle(manifestAssetBundleName, isLoadingAssetBundleManifest: true);
		AssetBundleLoadManifestOperation assetBundleLoadManifestOperation = new AssetBundleLoadManifestOperation(manifestAssetBundleName, "AssetBundleManifest", typeof(AssetBundleManifest));
		m_InProgressOperations.Add(assetBundleLoadManifestOperation);
		return assetBundleLoadManifestOperation;
	}

	protected static void LoadAssetBundle(string assetBundleName, bool isLoadingAssetBundleManifest = false)
	{
		Log(LogType.Info, "Loading Asset Bundle " + ((!isLoadingAssetBundleManifest) ? ": " : "Manifest: ") + assetBundleName);
		if (!isLoadingAssetBundleManifest && m_AssetBundleManifest == null)
		{
			Debug.LogError("Please initialize AssetBundleManifest by calling AssetBundleManager.Initialize()");
		}
		else if (!LoadAssetBundleInternal(assetBundleName, isLoadingAssetBundleManifest) && !isLoadingAssetBundleManifest)
		{
			LoadDependencies(assetBundleName);
		}
	}

	protected static string RemapVariantName(string assetBundleName)
	{
		string[] allAssetBundlesWithVariant = m_AssetBundleManifest.GetAllAssetBundlesWithVariant();
		string[] array = assetBundleName.Split('.');
		int num = int.MaxValue;
		int num2 = -1;
		for (int i = 0; i < allAssetBundlesWithVariant.Length; i++)
		{
			string[] array2 = allAssetBundlesWithVariant[i].Split('.');
			if (!(array2[0] != array[0]))
			{
				int num3 = Array.IndexOf(m_ActiveVariants, array2[1]);
				if (num3 == -1)
				{
					num3 = 2147483646;
				}
				if (num3 < num)
				{
					num = num3;
					num2 = i;
				}
			}
		}
		if (num == 2147483646)
		{
			Debug.LogWarning("Ambigious asset bundle variant chosen because there was no matching active variant: " + allAssetBundlesWithVariant[num2]);
		}
		if (num2 != -1)
		{
			return allAssetBundlesWithVariant[num2];
		}
		return assetBundleName;
	}

	protected static bool LoadAssetBundleInternal(string assetBundleName, bool isLoadingAssetBundleManifest)
	{
		LoadedAssetBundle value = null;
		m_LoadedAssetBundles.TryGetValue(assetBundleName, out value);
		if (value != null)
		{
			value.m_ReferencedCount++;
			return true;
		}
		if (m_DownloadingWWWs.ContainsKey(assetBundleName))
		{
			return true;
		}
		WWW wWW = null;
		string url = m_BaseDownloadingURL + assetBundleName;
		wWW = ((!isLoadingAssetBundleManifest) ? WWW.LoadFromCacheOrDownload(url, m_AssetBundleManifest.GetAssetBundleHash(assetBundleName), 0u) : new WWW(url));
		m_DownloadingWWWs.Add(assetBundleName, wWW);
		return false;
	}

	protected static void LoadDependencies(string assetBundleName)
	{
		if (m_AssetBundleManifest == null)
		{
			Debug.LogError("Please initialize AssetBundleManifest by calling AssetBundleManager.Initialize()");
			return;
		}
		string[] allDependencies = m_AssetBundleManifest.GetAllDependencies(assetBundleName);
		if (allDependencies.Length != 0)
		{
			for (int i = 0; i < allDependencies.Length; i++)
			{
				allDependencies[i] = RemapVariantName(allDependencies[i]);
			}
			m_Dependencies.Add(assetBundleName, allDependencies);
			for (int j = 0; j < allDependencies.Length; j++)
			{
				LoadAssetBundleInternal(allDependencies[j], isLoadingAssetBundleManifest: false);
			}
		}
	}

	public static void UnloadAssetBundle(string assetBundleName)
	{
		UnloadAssetBundleInternal(assetBundleName);
		UnloadDependencies(assetBundleName);
	}

	protected static void UnloadDependencies(string assetBundleName)
	{
		string[] value = null;
		if (m_Dependencies.TryGetValue(assetBundleName, out value))
		{
			string[] array = value;
			foreach (string assetBundleName2 in array)
			{
				UnloadAssetBundleInternal(assetBundleName2);
			}
			m_Dependencies.Remove(assetBundleName);
		}
	}

	protected static void UnloadAssetBundleInternal(string assetBundleName)
	{
		string error;
		LoadedAssetBundle loadedAssetBundle = GetLoadedAssetBundle(assetBundleName, out error);
		if (loadedAssetBundle != null && --loadedAssetBundle.m_ReferencedCount == 0)
		{
			loadedAssetBundle.m_AssetBundle.Unload(unloadAllLoadedObjects: false);
			m_LoadedAssetBundles.Remove(assetBundleName);
			Log(LogType.Info, assetBundleName + " has been unloaded successfully");
		}
	}

	private void Update()
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, WWW> downloadingWWW in m_DownloadingWWWs)
		{
			WWW value = downloadingWWW.Value;
			if (value.error != null)
			{
				m_DownloadingErrors.Add(downloadingWWW.Key, $"Failed downloading bundle {downloadingWWW.Key} from {value.url}: {value.error}");
				list.Add(downloadingWWW.Key);
			}
			else if (value.isDone)
			{
				AssetBundle assetBundle = value.assetBundle;
				if (assetBundle == null)
				{
					m_DownloadingErrors.Add(downloadingWWW.Key, $"{downloadingWWW.Key} is not a valid asset bundle.");
					list.Add(downloadingWWW.Key);
				}
				else
				{
					m_LoadedAssetBundles.Add(downloadingWWW.Key, new LoadedAssetBundle(value.assetBundle));
					list.Add(downloadingWWW.Key);
				}
			}
		}
		foreach (string item in list)
		{
			WWW wWW = m_DownloadingWWWs[item];
			m_DownloadingWWWs.Remove(item);
			wWW.Dispose();
		}
		int num = 0;
		while (num < m_InProgressOperations.Count)
		{
			if (!m_InProgressOperations[num].Update())
			{
				m_InProgressOperations.RemoveAt(num);
			}
			else
			{
				num++;
			}
		}
	}

	public static AssetBundleLoadAssetOperation LoadAssetAsync(string assetBundleName, string assetName, Type type)
	{
		Log(LogType.Info, "Loading " + assetName + " from " + assetBundleName + " bundle");
		AssetBundleLoadAssetOperation assetBundleLoadAssetOperation = null;
		assetBundleName = RemapVariantName(assetBundleName);
		LoadAssetBundle(assetBundleName);
		assetBundleLoadAssetOperation = new AssetBundleLoadAssetOperationFull(assetBundleName, assetName, type);
		m_InProgressOperations.Add(assetBundleLoadAssetOperation);
		return assetBundleLoadAssetOperation;
	}

	public static AssetBundleLoadOperation LoadLevelAsync(string assetBundleName, string levelName, bool isAdditive)
	{
		Log(LogType.Info, "Loading " + levelName + " from " + assetBundleName + " bundle");
		AssetBundleLoadOperation assetBundleLoadOperation = null;
		assetBundleName = RemapVariantName(assetBundleName);
		LoadAssetBundle(assetBundleName);
		assetBundleLoadOperation = new AssetBundleLoadLevelOperation(assetBundleName, levelName, isAdditive);
		m_InProgressOperations.Add(assetBundleLoadOperation);
		return assetBundleLoadOperation;
	}
}
