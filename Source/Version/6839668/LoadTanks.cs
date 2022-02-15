using System.Collections;
using AssetBundles;
using UnityEngine;

public class LoadTanks : MonoBehaviour
{
	public string sceneAssetBundle;

	public string sceneName;

	public string textAssetBundle;

	public string textAssetName;

	private string[] activeVariants;

	private bool bundlesLoaded;

	private bool sd;

	private bool hd;

	private bool normal;

	private bool desert;

	private bool english;

	private bool danish;

	private string tankAlbedoStyle;

	private string tankAlbedoResolution;

	private string language;

	private void Awake()
	{
		activeVariants = new string[2];
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
			GUILayout.BeginHorizontal();
			GUILayout.Toggle(sd, string.Empty);
			if (GUILayout.Button("Load SD"))
			{
				sd = true;
				hd = false;
				tankAlbedoResolution = "sd";
			}
			GUILayout.Toggle(hd, string.Empty);
			if (GUILayout.Button("Load HD"))
			{
				sd = false;
				hd = true;
				tankAlbedoResolution = "hd";
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Toggle(normal, string.Empty);
			if (GUILayout.Button("Normal"))
			{
				normal = true;
				desert = false;
				tankAlbedoStyle = "normal";
			}
			GUILayout.Toggle(desert, string.Empty);
			if (GUILayout.Button("Desert"))
			{
				normal = false;
				desert = true;
				tankAlbedoStyle = "desert";
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Toggle(english, string.Empty);
			if (GUILayout.Button("English"))
			{
				english = true;
				danish = false;
				language = "english";
			}
			GUILayout.Toggle(danish, string.Empty);
			if (GUILayout.Button("Danish"))
			{
				english = false;
				danish = true;
				language = "danish";
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(15f);
			if (GUILayout.Button("Load Scene"))
			{
				bundlesLoaded = true;
				activeVariants[0] = tankAlbedoStyle + "-" + tankAlbedoResolution;
				activeVariants[1] = language;
				Debug.Log(activeVariants[0]);
				Debug.Log(activeVariants[1]);
				StartCoroutine(BeginExample());
			}
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
		}
	}

	private IEnumerator BeginExample()
	{
		yield return StartCoroutine(Initialize());
		AssetBundleManager.ActiveVariants = activeVariants;
		Debug.Log(AssetBundleManager.ActiveVariants[0]);
		Debug.Log(AssetBundleManager.ActiveVariants[1]);
		yield return StartCoroutine(InitializeLevelAsync(sceneName, isAdditive: true));
		yield return StartCoroutine(InstantiateGameObjectAsync(textAssetBundle, textAssetName));
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

	protected IEnumerator InstantiateGameObjectAsync(string assetBundleName, string assetName)
	{
		float startTime = Time.realtimeSinceStartup;
		AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync(assetBundleName, assetName, typeof(GameObject));
		if (request == null)
		{
			Debug.LogError("Failed AssetBundleLoadAssetOperation on " + assetName + " from the AssetBundle " + assetBundleName + ".");
			yield break;
		}
		yield return StartCoroutine(request);
		GameObject prefab = request.GetAsset<GameObject>();
		if (prefab != null)
		{
			Object.Instantiate(prefab);
		}
		else
		{
			Debug.LogError("Failed to GetAsset from request");
		}
		float elapsedTime = Time.realtimeSinceStartup - startTime;
		Debug.Log(assetName + ((!(prefab == null)) ? " was" : " was not") + " loaded successfully in " + elapsedTime + " seconds");
	}
}
