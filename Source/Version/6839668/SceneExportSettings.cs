using System;
using UnityEngine;

public class SceneExportSettings : ScriptableObject
{
	[Serializable]
	public class HeightSetting
	{
		[SerializeField]
		public string m_name;

		[SerializeField]
		public float m_height;

		[SerializeField]
		public float m_heightRadius;
	}

	[SerializeField]
	public float m_worldScaleUp;

	[SerializeField]
	public string m_scriptableObjectPostfix;

	[SerializeField]
	public HeightSetting[] m_heights;
}
