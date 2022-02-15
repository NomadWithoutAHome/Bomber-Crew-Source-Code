using System;
using System.Collections;
using BomberCrewCommon;
using UnityEngine;

public class ResourcesLoadList : MonoBehaviour, LoadableSystem
{
	[SerializeField]
	private string[] m_resourcesLoadNames;

	[SerializeField]
	private string[] m_asyncSceneNames;

	[SerializeField]
	private bool m_doSceneBasedLoad;

	[SerializeField]
	private string m_loaderNodeName = "__LoaderNode";

	private bool m_isLoadComplete;

	private void Awake()
	{
		Singleton<SystemLoader>.Instance.RegisterLoadableSystem(this);
	}

	public void ContinueLoad()
	{
	}

	public LoadableSystem[] GetDependencies()
	{
		return null;
	}

	public string GetName()
	{
		return "ResourcesLoadList";
	}

	public bool IsLoadComplete()
	{
		return m_isLoadComplete;
	}

	public void StartLoad()
	{
		StartCoroutine(LoadAll());
	}

	private IEnumerator LoadAll()
	{
		if (m_doSceneBasedLoad)
		{
			string[] asyncSceneNames = m_asyncSceneNames;
			foreach (string s in asyncSceneNames)
			{
				DateTime startTime2 = DateTime.UtcNow;
				yield return StartCoroutine(Singleton<GameFlow>.Instance.LoadLevelBase(s, additive: true));
				yield return null;
				GameObject ln = GameObject.Find(m_loaderNodeName);
				UnityEngine.Object.DontDestroyOnLoad(ln);
				if (ln != null)
				{
					ln.transform.parent = base.transform;
					ln.name = "__PLACED";
				}
				yield return null;
				TimeSpan ts2 = DateTime.UtcNow - startTime2;
			}
		}
		else
		{
			string[] resourcesLoadNames = m_resourcesLoadNames;
			foreach (string sr in resourcesLoadNames)
			{
				DateTime startTime = DateTime.UtcNow;
				ResourceRequest rr = Resources.LoadAsync<GameObject>(sr);
				while (!rr.isDone)
				{
					yield return null;
				}
				yield return null;
				GameObject go = UnityEngine.Object.Instantiate((GameObject)rr.asset);
				go.transform.parent = base.transform;
				TimeSpan ts = DateTime.UtcNow - startTime;
			}
		}
		m_isLoadComplete = true;
	}
}
