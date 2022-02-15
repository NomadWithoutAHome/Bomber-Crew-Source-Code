using System.Collections;
using AssetBundles;
using UnityEngine;

public class LoadAssets : MonoBehaviour
{
	public const string AssetBundlesOutputPath = "/AssetBundles/";

	public string assetBundleName;

	public string assetName;

	private IEnumerator Start()
	{
		yield return StartCoroutine(Initialize());
		yield return StartCoroutine(InstantiateGameObjectAsync(assetBundleName, assetName));
	}

	protected IEnumerator Initialize()
	{
		Object.DontDestroyOnLoad(base.gameObject);
		AssetBundleManager.SetSourceAssetBundleURL(Application.dataPath + "/");
		AssetBundleLoadManifestOperation request = AssetBundleManager.Initialize();
		if (request != null)
		{
			yield return StartCoroutine(request);
		}
	}

	protected IEnumerator InstantiateGameObjectAsync(string assetBundleName, string assetName)
	{
		float startTime = Time.realtimeSinceStartup;
		AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync(assetBundleName, assetName, typeof(GameObject));
		if (request != null)
		{
			yield return StartCoroutine(request);
			GameObject prefab = request.GetAsset<GameObject>();
			if (prefab != null)
			{
				Object.Instantiate(prefab);
			}
			float elapsedTime = Time.realtimeSinceStartup - startTime;
			Debug.Log(assetName + ((!(prefab == null)) ? " was" : " was not") + " loaded successfully in " + elapsedTime + " seconds");
		}
	}
}
