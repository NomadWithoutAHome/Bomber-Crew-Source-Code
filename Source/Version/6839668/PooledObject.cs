using BomberCrewCommon;
using UnityEngine;

public class PooledObject : MonoBehaviour
{
	public int m_objectVersion;

	public int m_basePrefabInstanceId;

	private void OnDestroy()
	{
		if (Singleton<PoolManager>.Instance != null)
		{
			Singleton<PoolManager>.Instance.RevoveFromPool(m_basePrefabInstanceId, this);
		}
	}
}
