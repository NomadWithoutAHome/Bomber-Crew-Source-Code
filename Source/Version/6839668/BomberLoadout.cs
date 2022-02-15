using System;
using UnityEngine;

[CreateAssetMenu]
public class BomberLoadout : ScriptableObject
{
	[Serializable]
	public class Category
	{
		[SerializeField]
		private string m_name;

		[SerializeField]
		private string[] m_slots;

		private int m_lastSelectedSlotIndex;

		public int LastSelectedSlotIndex
		{
			get
			{
				return m_lastSelectedSlotIndex;
			}
			set
			{
				m_lastSelectedSlotIndex = value;
			}
		}

		public string GetName()
		{
			return m_name;
		}

		public string[] GetSlots()
		{
			return m_slots;
		}
	}

	[SerializeField]
	private Category[] m_categories;

	public int m_lastSelectedCategoryIndex;

	public Category[] GetCategories()
	{
		return m_categories;
	}
}
