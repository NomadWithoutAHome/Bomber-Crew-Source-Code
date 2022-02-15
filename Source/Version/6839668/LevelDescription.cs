using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelDescription : ScriptableObject
{
	[Serializable]
	public class LevelItemSerialized
	{
		[SerializeField]
		public Vector3 m_position;

		[SerializeField]
		public Quaternion m_rotation;

		[SerializeField]
		public Vector3 m_size;

		[SerializeField]
		public string m_spawnTrigger;

		[SerializeField]
		public string m_missionKeyRequired;

		[SerializeField]
		public string m_missionKeySuppress;

		[SerializeField]
		public string m_type;

		[SerializeField]
		public string m_typeBase;

		[SerializeField]
		public List<LevelParameter> m_levelParameters = new List<LevelParameter>();

		[SerializeField]
		public string m_ref;
	}

	[Serializable]
	public class LevelParameter
	{
		[SerializeField]
		public string m_key;

		[SerializeField]
		public string m_value;
	}

	public List<LevelItemSerialized> m_levelItems = new List<LevelItemSerialized>();
}
