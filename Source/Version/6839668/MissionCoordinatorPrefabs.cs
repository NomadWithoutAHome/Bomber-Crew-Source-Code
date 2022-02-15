using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Bomber Crew/Mission Prefabs")]
public class MissionCoordinatorPrefabs : ScriptableObject
{
	[Serializable]
	public class PrefabConnection
	{
		[SerializeField]
		public string m_missionObjectString;

		[SerializeField]
		public GameObject m_prefab;
	}

	[SerializeField]
	private PrefabConnection[] m_objectTypes;

	public PrefabConnection[] GetAllObjectTypes()
	{
		return m_objectTypes;
	}
}
