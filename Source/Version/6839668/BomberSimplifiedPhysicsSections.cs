using System;
using UnityEngine;

public class BomberSimplifiedPhysicsSections : MonoBehaviour
{
	[Serializable]
	public class Section
	{
		[SerializeField]
		public GameObject[] m_toRemove;

		[SerializeField]
		public BomberDestroyableSection m_relatedSection;
	}

	[SerializeField]
	private Section[] m_physicsSections;

	private void Start()
	{
		Section[] physicsSections = m_physicsSections;
		foreach (Section section in physicsSections)
		{
			Section ns = section;
			ns.m_relatedSection.OnSectionDestroy += delegate
			{
				GameObject[] toRemove = ns.m_toRemove;
				foreach (GameObject gameObject in toRemove)
				{
					if (gameObject != null)
					{
						UnityEngine.Object.Destroy(gameObject);
					}
				}
			};
		}
	}
}
