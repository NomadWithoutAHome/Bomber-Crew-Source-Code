using System.Collections;
using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class AutoPool : MonoBehaviour, LoadableSystem
{
	[SerializeField]
	private GameObject m_prefabToPool;

	[SerializeField]
	private int m_numberToPool;

	[SerializeField]
	private bool m_instant;

	[SerializeField]
	private bool m_asLoadableSystem;

	private bool m_isLoaded;

	private int m_prefabInstanceId;

	private static List<AutoPool> s_allAutoPools = new List<AutoPool>();

	public static void RepoolAllToSize()
	{
		foreach (AutoPool s_allAutoPool in s_allAutoPools)
		{
			s_allAutoPool.ResetLoaded();
		}
		Singleton<SystemLoader>.Instance.ResetHaveAllLoaded();
	}

	public void ResetLoaded()
	{
		int poolCount = Singleton<PoolManager>.Instance.GetPoolCount(m_prefabToPool);
		if (poolCount < m_numberToPool)
		{
			m_isLoaded = false;
		}
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
		return "Autopool(" + m_prefabToPool.name + ")";
	}

	public bool IsLoadComplete()
	{
		return m_isLoaded;
	}

	public void StartLoad()
	{
		DoPool();
	}

	private void OnDestroy()
	{
		s_allAutoPools.Remove(this);
		if (Singleton<PoolManager>.Instance != null)
		{
			Singleton<PoolManager>.Instance.ClearPool(m_prefabInstanceId);
		}
	}

	private void Awake()
	{
		m_prefabInstanceId = m_prefabToPool.GetInstanceID();
		if (m_asLoadableSystem)
		{
			s_allAutoPools.Add(this);
			Singleton<SystemLoader>.Instance.RegisterLoadableSystem(this);
		}
		else
		{
			DoPool();
		}
	}

	private void DoPool()
	{
		if (m_instant)
		{
			Singleton<PoolManager>.Instance.PoolPrefab(m_prefabToPool, m_numberToPool);
		}
		else
		{
			StartCoroutine(PoolPrefabsOverTime());
		}
	}

	private IEnumerator PoolPrefabsOverTime()
	{
		int startIndex = Singleton<PoolManager>.Instance.GetPoolCount(m_prefabToPool);
		for (int i = startIndex + 1; i <= m_numberToPool; i++)
		{
			Singleton<PoolManager>.Instance.PoolPrefab(m_prefabToPool, i);
			if (i % 20 == 0)
			{
				yield return null;
			}
		}
		Singleton<PoolManager>.Instance.RefreshPool(m_prefabToPool);
		m_isLoaded = true;
	}
}
