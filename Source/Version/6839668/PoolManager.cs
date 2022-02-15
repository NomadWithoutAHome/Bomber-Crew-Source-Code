using System;
using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
	private class Pool
	{
		public int m_version;

		public GameObject m_prefabBase;

		public List<PooledObject> m_poolItems = new List<PooledObject>(16);

		public List<PooledObject> m_poolItemsAll = new List<PooledObject>(16);

		public GameObject GetNext()
		{
			if (m_poolItems.Count > 0)
			{
				GameObject gameObject = m_poolItems[0].gameObject;
				m_poolItems.RemoveAt(0);
				return gameObject;
			}
			return null;
		}
	}

	private Dictionary<int, Pool> m_poolDic = new Dictionary<int, Pool>(32);

	private bool m_poolsWereCleared;

	private GameObject m_scenePoolObjects;

	public event Action PoolsWereCleared;

	private void OnLevelWasLoaded(int level)
	{
		m_scenePoolObjects = new GameObject("LocalScenePool");
	}

	public void PoolPrefabAdditional(GameObject prefab, int count)
	{
		int instanceID = prefab.GetInstanceID();
		Pool pool = GetPool(instanceID);
		if (pool == null)
		{
			pool = new Pool();
			pool.m_prefabBase = prefab;
			m_poolDic[instanceID] = pool;
		}
		for (int i = 0; i < count; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(prefab);
			gameObject.SetActive(value: false);
			PooledObject pooledObject = gameObject.AddComponent<PooledObject>();
			pooledObject.transform.parent = base.transform;
			pooledObject.m_basePrefabInstanceId = instanceID;
			pool.m_poolItems.Add(pooledObject);
			pool.m_poolItemsAll.Add(pooledObject);
		}
	}

	public int GetPoolCount(GameObject prefab)
	{
		int instanceID = prefab.GetInstanceID();
		return GetPool(instanceID)?.m_poolItems.Count ?? 0;
	}

	public void PoolPrefab(GameObject prefab, int count)
	{
		int instanceID = prefab.GetInstanceID();
		Pool pool = GetPool(instanceID);
		if (pool == null)
		{
			pool = new Pool();
			pool.m_prefabBase = prefab;
			m_poolDic[instanceID] = pool;
		}
		while (pool.m_poolItems.Count < count)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(prefab);
			gameObject.SetActive(value: false);
			PooledObject pooledObject = gameObject.AddComponent<PooledObject>();
			pooledObject.transform.parent = base.transform;
			pooledObject.m_basePrefabInstanceId = instanceID;
			pool.m_poolItems.Add(pooledObject);
			pool.m_poolItemsAll.Add(pooledObject);
		}
	}

	private Pool GetPool(int basePrefabInstanceId)
	{
		Pool value = null;
		m_poolDic.TryGetValue(basePrefabInstanceId, out value);
		return value;
	}

	private Pool GetPool(PooledObject po)
	{
		if (po != null)
		{
			Pool pool = GetPool(po.m_basePrefabInstanceId);
			if (pool.m_version == po.m_objectVersion)
			{
				return pool;
			}
			return null;
		}
		return null;
	}

	public void ClearPool(int prefabInstanceId)
	{
		Pool pool = GetPool(prefabInstanceId);
		if (pool != null)
		{
			ClearPool(pool);
		}
	}

	public void RefreshPool(GameObject go)
	{
		Pool pool = GetPool(go.GetInstanceID());
		if (pool != null)
		{
			while (pool.m_poolItemsAll.Contains(null))
			{
				pool.m_poolItemsAll.Remove(null);
			}
		}
	}

	private void ClearPool(Pool p)
	{
		p.m_version++;
		foreach (PooledObject item in p.m_poolItemsAll)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
		p.m_poolItems.Clear();
		p.m_poolItemsAll.Clear();
	}

	public void ClearAllPools()
	{
		foreach (Pool value in m_poolDic.Values)
		{
			ClearPool(value);
		}
		m_poolDic.Clear();
		m_poolsWereCleared = true;
	}

	private void Update()
	{
		if (m_poolsWereCleared)
		{
			m_poolsWereCleared = false;
			if (this.PoolsWereCleared != null)
			{
				this.PoolsWereCleared();
			}
		}
	}

	public void ReturnToPoolDontDisable(GameObject pooledObject)
	{
		PooledObject component = pooledObject.GetComponent<PooledObject>();
		if (component != null)
		{
			Pool pool = GetPool(component);
			if (pool != null)
			{
				component.transform.parent = base.transform;
				pool.m_poolItems.Add(component);
			}
			else
			{
				UnityEngine.Object.Destroy(pooledObject);
			}
		}
	}

	public void ReturnToPool(GameObject pooledObject)
	{
		PooledObject component = pooledObject.GetComponent<PooledObject>();
		if (component != null)
		{
			Pool pool = GetPool(component);
			if (pool != null)
			{
				pooledObject.SetActive(value: false);
				component.transform.parent = base.transform;
				pool.m_poolItems.Add(component);
			}
			else
			{
				UnityEngine.Object.Destroy(pooledObject);
			}
		}
	}

	public GameObject GetFromPoolSlow(GameObject prefab)
	{
		return GetFromPool(prefab, prefab.GetInstanceID());
	}

	public GameObject GetFromPoolSlowNoReparent(GameObject prefab)
	{
		return GetFromPoolNoReparent(prefab, prefab.GetInstanceID());
	}

	public void RevoveFromPool(int poolId, PooledObject toRemove)
	{
		Pool pool = GetPool(poolId);
		if (pool != null)
		{
			pool.m_poolItems.Remove(toRemove);
			pool.m_poolItemsAll.Remove(toRemove);
		}
	}

	public GameObject GetFromPool(GameObject prefab, int prefabInstanceId)
	{
		Pool pool = GetPool(prefabInstanceId);
		GameObject gameObject = null;
		if (pool != null)
		{
			gameObject = pool.GetNext();
		}
		else
		{
			pool = new Pool();
			pool.m_prefabBase = prefab;
			m_poolDic[prefabInstanceId] = pool;
		}
		if (gameObject == null)
		{
			gameObject = UnityEngine.Object.Instantiate(prefab);
			PooledObject pooledObject = gameObject.AddComponent<PooledObject>();
			pooledObject.m_basePrefabInstanceId = prefabInstanceId;
			pool.m_poolItemsAll.Add(pooledObject);
		}
		gameObject.SetActive(value: true);
		gameObject.transform.parent = m_scenePoolObjects.transform;
		return gameObject;
	}

	public GameObject GetFromPoolNoReparent(GameObject prefab, int prefabInstanceId)
	{
		Pool pool = GetPool(prefabInstanceId);
		GameObject gameObject = null;
		if (pool != null)
		{
			gameObject = pool.GetNext();
		}
		else
		{
			pool = new Pool();
			pool.m_prefabBase = prefab;
			m_poolDic[prefabInstanceId] = pool;
		}
		if (gameObject == null)
		{
			gameObject = UnityEngine.Object.Instantiate(prefab);
			PooledObject pooledObject = gameObject.AddComponent<PooledObject>();
			pooledObject.m_basePrefabInstanceId = prefabInstanceId;
			pool.m_poolItemsAll.Add(pooledObject);
		}
		gameObject.SetActive(value: true);
		return gameObject;
	}
}
