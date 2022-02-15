using System.Collections;
using AssetBundles;
using UnityEngine;

public class LoadVariants : MonoBehaviour
{
	private const string variantSceneAssetBundle = "variants/variant-scene";

	private const string variantSceneName = "VariantScene";

	private string[] activeVariants;

	private bool bundlesLoaded;

	private void Awake()
	{
		activeVariants = new string[1];
		bundlesLoaded = false;
	}

	private void OnGUI()
	{
		if (!bundlesLoaded)
		{
			GUILayout.Space(20f);
			GUILayout.BeginHorizontal();
			GUILayout.Space(20f);
			GUILayout.BeginVertical();
			if (GUILayout.Button("Load SD"))
			{
				activeVariants[0] = "sd";
				bundlesLoaded = true;
				StartCoroutine(BeginExample());
				BeginExample();
			}
			GUILayout.Space(5f);
			if (GUILayout.Button("Load HD"))
			{
				activeVariants[0] = "hd";
				bundlesLoaded = true;
				StartCoroutine(BeginExample());
				Debug.Log("Loading HD");
			}
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
		}
	}

	private IEnumerator BeginExample()
	{
		yield return StartCoroutine(Initialize());
		AssetBundleManager.ActiveVariants = activeVariants;
		yield return StartCoroutine(InitializeLevelAsync("VariantScene", isAdditive: true));
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
		AssetBundleLoadOperation request = AssetBundleManager.LoadLevelAsync("variants/variant-scene", levelName, isAdditive);
		if (request != null)
		{
			yield return StartCoroutine(request);
			float elapsedTime = Time.realtimeSinceStartup - startTime;
			Debug.Log("Finished loading scene " + levelName + " in " + elapsedTime + " seconds");
		}
	}
}
