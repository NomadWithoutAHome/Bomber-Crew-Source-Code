using System;
using BomberCrewCommon;
using UnityEngine;

public class LoadoutPrefabReferences : Singleton<LoadoutPrefabReferences>
{
	[Serializable]
	public class LoadoutPrefabItem
	{
		[SerializeField]
		public string m_reference;

		[SerializeField]
		public GameObject m_prefab;

		[SerializeField]
		public int m_cost;
	}

	[SerializeField]
	private LoadoutPrefabItem[] m_loadoutPrefabItems;

	public GameObject GetPrefabForString(string reference)
	{
		LoadoutPrefabItem[] loadoutPrefabItems = m_loadoutPrefabItems;
		foreach (LoadoutPrefabItem loadoutPrefabItem in loadoutPrefabItems)
		{
			if (loadoutPrefabItem.m_reference == reference)
			{
				return loadoutPrefabItem.m_prefab;
			}
		}
		return null;
	}

	public int GetCostForString(string reference)
	{
		LoadoutPrefabItem[] loadoutPrefabItems = m_loadoutPrefabItems;
		foreach (LoadoutPrefabItem loadoutPrefabItem in loadoutPrefabItems)
		{
			if (loadoutPrefabItem.m_reference == reference)
			{
				return loadoutPrefabItem.m_cost;
			}
		}
		return 0;
	}
}
