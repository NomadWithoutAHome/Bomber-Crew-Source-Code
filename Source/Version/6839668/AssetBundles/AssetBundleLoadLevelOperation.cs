using UnityEngine;

namespace AssetBundles;

public class AssetBundleLoadLevelOperation : AssetBundleLoadOperation
{
	protected string m_AssetBundleName;

	protected string m_LevelName;

	protected bool m_IsAdditive;

	protected string m_DownloadingError;

	protected AsyncOperation m_Request;

	public AssetBundleLoadLevelOperation(string assetbundleName, string levelName, bool isAdditive)
	{
		m_AssetBundleName = assetbundleName;
		m_LevelName = levelName;
		m_IsAdditive = isAdditive;
	}

	public override bool Update()
	{
		if (m_Request != null)
		{
			return false;
		}
		LoadedAssetBundle loadedAssetBundle = AssetBundleManager.GetLoadedAssetBundle(m_AssetBundleName, out m_DownloadingError);
		if (loadedAssetBundle != null)
		{
			if (m_IsAdditive)
			{
				m_Request = Application.LoadLevelAdditiveAsync(m_LevelName);
			}
			else
			{
				m_Request = Application.LoadLevelAsync(m_LevelName);
			}
			return false;
		}
		return true;
	}

	public override bool IsDone()
	{
		if (m_Request == null && m_DownloadingError != null)
		{
			Debug.LogError(m_DownloadingError);
			return true;
		}
		return m_Request != null && m_Request.isDone;
	}
}
