using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Bomber Crew/Livery Requirements")]
public class LiveryRequirements : ScriptableObject
{
	[Serializable]
	public class LiverySection
	{
		[SerializeField]
		public string m_uniqueId;

		[SerializeField]
		public string m_subIdVariant;

		[SerializeField]
		public bool m_isText;

		[SerializeField]
		public int m_maxTextLength;

		[SerializeField]
		public int m_lengthToSwitchToSmall = 8;

		[SerializeField]
		public Vector3[] m_centrePositions;

		[SerializeField]
		public Vector3[] m_sizes;

		[SerializeField]
		public int[] texCoords;
	}

	[SerializeField]
	private LiverySection[] m_liveryRequirements;

	public LiverySection GetRequirementsForId(string id)
	{
		LiverySection[] liveryRequirements = m_liveryRequirements;
		foreach (LiverySection liverySection in liveryRequirements)
		{
			if (liverySection.m_uniqueId == id)
			{
				return liverySection;
			}
		}
		return null;
	}

	public LiverySection[] GetAllRequirements()
	{
		return m_liveryRequirements;
	}
}
