using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class NavigationPoint : MonoBehaviour
{
	[SerializeField]
	private TaggableItem m_tagItem;

	[SerializeField]
	private BomberNavigation.NavigationPointType m_pointType;

	[SerializeField]
	private bool m_dontHideWithPriority;

	private BomberNavigation.NavigationPoint m_myPoint;

	private void Start()
	{
		m_tagItem.OnTagComplete += OnTagComplete;
		m_myPoint = new BomberNavigation.NavigationPoint();
		m_myPoint.m_associatedObject = base.gameObject;
		m_myPoint.m_position = base.gameObject.btransform().position;
		m_myPoint.m_navPointType = m_pointType;
	}

	private void OnTagComplete()
	{
		Singleton<CrewMiscSpeechTriggers>.Instance.DoExternalTrigger(CrewMiscSpeechTriggers.SpeechExternalTrigger.HeadingConfirmed, null);
		Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberNavigation().SetNavigationPoint(m_myPoint);
	}

	private void Update()
	{
		List<TaggableItem> functionalTagsVisible = Singleton<TagManager>.Instance.GetFunctionalTagsVisible();
		bool flag = false;
		if (!m_dontHideWithPriority)
		{
			Vector3 position = Singleton<BomberSpawn>.Instance.GetBomberSystems().transform.position;
			Vector3 vector = base.transform.position - position;
			foreach (TaggableItem item in functionalTagsVisible)
			{
				if (Vector3.Dot(rhs: (item.transform.position - position).normalized, lhs: vector.normalized) > 0.3f)
				{
					flag = true;
				}
			}
		}
		if (flag)
		{
			m_tagItem.SetTaggable(taggable: false);
		}
		else
		{
			m_tagItem.SetTaggable(taggable: true);
		}
		if (Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberNavigation().GetAssociatedObject() != base.gameObject)
		{
			m_tagItem.SetTagIncomplete();
		}
		m_myPoint.m_position = base.gameObject.btransform().position;
	}
}
