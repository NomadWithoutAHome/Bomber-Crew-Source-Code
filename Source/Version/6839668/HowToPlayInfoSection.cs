using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Bomber Crew/How To Play Section")]
public class HowToPlayInfoSection : ScriptableObject
{
	public enum ItemType
	{
		Image,
		Title,
		Paragraph,
		BulletPoint,
		IconPoint,
		LinkButton
	}

	[Serializable]
	public class HowToPlaySectionItem
	{
		[SerializeField]
		public ItemType m_itemType;

		[SerializeField]
		[NamedText]
		public string m_text;

		[SerializeField]
		public string m_textController;

		[SerializeField]
		public Texture2D m_image;

		[SerializeField]
		public string m_iconReference;

		[SerializeField]
		public string m_linkReference;
	}

	[SerializeField]
	[NamedText]
	public string m_titleText;

	[SerializeField]
	public string m_jumpToReference;

	[SerializeField]
	public HowToPlaySectionItem[] m_allItems;
}
