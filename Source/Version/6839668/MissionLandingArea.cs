using System;
using BomberCrewCommon;
using UnityEngine;

public class MissionLandingArea : MonoBehaviour
{
	[Serializable]
	public class LandingPath
	{
		[SerializeField]
		public Transform[] m_landingAlignmentTransform;

		[SerializeField]
		public Vector3 m_landingAlignment;
	}

	[SerializeField]
	private TaggableItem m_taggableItem;

	[SerializeField]
	private LandingPath[] m_allLandingPaths;

	private BomberNavigation m_bomberNavigation;

	private int m_currentLandingPath;

	public Vector3 GetAlignment()
	{
		return base.transform.TransformDirection(m_allLandingPaths[m_currentLandingPath].m_landingAlignment);
	}

	private void Start()
	{
		m_taggableItem.OnTagComplete += OnTagComplete;
		m_bomberNavigation = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberNavigation();
	}

	private void OnTagComplete()
	{
		float num = float.MaxValue;
		int currentLandingPath = 0;
		for (int i = 0; i < m_allLandingPaths.Length; i++)
		{
			float magnitude = (m_allLandingPaths[i].m_landingAlignmentTransform[0].position - m_bomberNavigation.transform.position).magnitude;
			if (magnitude < num)
			{
				currentLandingPath = i;
				num = magnitude;
			}
		}
		m_currentLandingPath = currentLandingPath;
		Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState().SetAltitudeTarget(0);
		m_bomberNavigation.SetPreLanding(0);
	}

	public Vector3d GetLandingStartPosition(int index)
	{
		return new Vector3d(base.transform.TransformDirection(m_allLandingPaths[m_currentLandingPath].m_landingAlignmentTransform[index].localPosition)) + base.gameObject.btransform().position;
	}

	public int GetNumLandingStartPositions()
	{
		return m_allLandingPaths[m_currentLandingPath].m_landingAlignmentTransform.Length;
	}

	private void Update()
	{
		if (!Singleton<MissionCoordinator>.Instance.IsOutwardJourney())
		{
			m_taggableItem.SetTaggable(taggable: true);
			if (m_bomberNavigation.GetCurrentType() != BomberNavigation.NavigationPointType.FunctionLandingBombing)
			{
				m_taggableItem.SetTagIncomplete();
			}
		}
		else
		{
			m_taggableItem.SetTaggable(taggable: false);
		}
	}
}
