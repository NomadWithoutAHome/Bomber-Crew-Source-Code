using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Bomber Crew/How To Play All Pages")]
public class PauseMenuHowToPlayAllPages : ScriptableObject
{
	[Serializable]
	public class CategoryHeading
	{
		[SerializeField]
		[NamedText]
		public string m_categoryName;

		[SerializeField]
		public HowToPlayInfoSection[] m_allPages;

		[SerializeField]
		public bool m_hasHeader;
	}

	[SerializeField]
	public CategoryHeading[] m_allCategories;
}
