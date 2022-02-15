using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class CrewPopulator : Singleton<CrewPopulator>
{
	[SerializeField]
	private GameObject m_crewmanPanelPrefab;

	[SerializeField]
	private Transform m_crewmanPanelNode;

	[SerializeField]
	private LayoutGrid m_layoutGrid;

	[SerializeField]
	private Transform m_topPreselectPosition;

	private List<GameObject> m_allPanels = new List<GameObject>();

	private void Start()
	{
		Refresh();
	}

	public GameObject GetPanelFor(int index)
	{
		return m_allPanels[m_allPanels.Count - 1 - index];
	}

	private void Refresh()
	{
		if (Singleton<CrewContainer>.Instance != null)
		{
			int currentCrewCount = Singleton<CrewContainer>.Instance.GetCurrentCrewCount();
			for (int i = 0; i < currentCrewCount; i++)
			{
				GameObject gameObject = Object.Instantiate(m_crewmanPanelPrefab);
				gameObject.transform.parent = m_crewmanPanelNode;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.GetComponent<CrewmanPanelDisplay>().SetUpForCrewman(currentCrewCount - 1 - i);
				m_allPanels.Add(gameObject);
			}
			m_layoutGrid.RepositionChildren();
			m_topPreselectPosition.transform.position = m_allPanels[m_allPanels.Count - 1].transform.position;
		}
	}
}
