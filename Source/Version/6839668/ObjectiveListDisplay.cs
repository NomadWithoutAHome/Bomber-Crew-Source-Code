using System.Collections;
using System.Collections.Generic;
using BomberCrewCommon;
using Common;
using UnityEngine;

public class ObjectiveListDisplay : MonoBehaviour
{
	public class ObjectiveDisplayLink
	{
		public GameObject m_createdObject;

		public ObjectiveDisplay m_objectiveDisplay;

		public ObjectiveManager.Objective m_objective;
	}

	[SerializeField]
	private GameObject m_showObjectivePrefab;

	[SerializeField]
	private LayoutGrid m_layoutGrid;

	private List<ObjectiveDisplayLink> m_shownObjectives = new List<ObjectiveDisplayLink>();

	public ObjectiveDisplayLink GetODLFor(ObjectiveManager.Objective o)
	{
		foreach (ObjectiveDisplayLink shownObjective in m_shownObjectives)
		{
			if (shownObjective.m_objective == o)
			{
				return shownObjective;
			}
		}
		return null;
	}

	public ObjectiveDisplayLink CreateODLFor(ObjectiveManager.Objective o)
	{
		ObjectiveDisplayLink objectiveDisplayLink = new ObjectiveDisplayLink();
		objectiveDisplayLink.m_objective = o;
		GameObject original = m_showObjectivePrefab;
		if (o.m_prefabOverride != null)
		{
			original = o.m_prefabOverride;
		}
		GameObject gameObject = Object.Instantiate(original);
		gameObject.SetActive(value: false);
		gameObject.transform.parent = m_layoutGrid.transform;
		objectiveDisplayLink.m_createdObject = gameObject;
		objectiveDisplayLink.m_objectiveDisplay = gameObject.GetComponent<ObjectiveDisplay>();
		objectiveDisplayLink.m_objectiveDisplay.SetUp(o);
		m_shownObjectives.Add(objectiveDisplayLink);
		gameObject.CustomActivate(active: true);
		m_layoutGrid.RepositionChildren();
		return objectiveDisplayLink;
	}

	private void Update()
	{
		List<ObjectiveManager.Objective> currentObjectives = Singleton<ObjectiveManager>.Instance.GetCurrentObjectives();
		List<ObjectiveDisplayLink> list = new List<ObjectiveDisplayLink>();
		list.AddRange(m_shownObjectives);
		foreach (ObjectiveManager.Objective item in currentObjectives)
		{
			ObjectiveDisplayLink objectiveDisplayLink = GetODLFor(item);
			if (objectiveDisplayLink == null)
			{
				objectiveDisplayLink = CreateODLFor(item);
			}
			list.Remove(objectiveDisplayLink);
		}
		bool flag = false;
		foreach (ObjectiveDisplayLink item2 in list)
		{
			flag = true;
			item2.m_createdObject.transform.parent = m_layoutGrid.transform.parent;
			item2.m_createdObject.CustomActivate(active: false);
			StartCoroutine(WaitAndDestroy(item2.m_createdObject));
			m_shownObjectives.Remove(item2);
		}
		if (flag)
		{
			m_layoutGrid.RepositionChildren();
		}
	}

	private IEnumerator WaitAndDestroy(GameObject go)
	{
		yield return new WaitForSeconds(3f);
		Object.Destroy(go);
	}
}
