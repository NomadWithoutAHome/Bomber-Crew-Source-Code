using System.Collections;
using AssetBundles;
using UnityEngine;

public class LoadScenes : MonoBehaviour
{
	public string sceneAssetBundle;

	public string sceneName;

	private IEnumerator Start()
	{
		yield return StartCoroutine(Initialize());
		yield return StartCoroutine(InitializeLevelAsync(sceneName, isAdditive: true));
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

	protected IEnumerator InitializeLevelAsync(string levelName, bool isAdditive)
	{
		float startTime = Time.realtimeSinceStartup;
		AssetBundleLoadOperation request = AssetBundleManager.LoadLevelAsync(sceneAssetBundle, levelName, isAdditive);
		if (request != null)
		{
			yield return StartCoroutine(request);
			float elapsedTime = Time.realtimeSinceStartup - startTime;
			Debug.Log("Finished loading scene " + levelName + " in " + elapsedTime + " seconds");
		}
	}
}
