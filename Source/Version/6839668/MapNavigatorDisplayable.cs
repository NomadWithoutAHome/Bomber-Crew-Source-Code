using BomberCrewCommon;
using UnityEngine;

public class MapNavigatorDisplayable : MonoBehaviour
{
	public enum NavigatorDescriptionType
	{
		Hazard,
		Target,
		PhotoSite,
		Rescue,
		PhotoSiteOptional
	}

	[SerializeField]
	private GameObject m_mapDisplayPrefab;

	[SerializeField]
	private bool m_requiresVisibility;

	[SerializeField]
	private bool m_showOnOutwardJourneyOnly;

	[SerializeField]
	private float m_radius;

	[SerializeField]
	private NavigatorDescriptionType m_navigationTypeDescription;

	private bool m_shouldDisplay = true;

	private void Start()
	{
		if (m_shouldDisplay)
		{
			Singleton<MissionMapController>.Instance.RegisterMapObject(this);
		}
	}

	public void SetRadius(float radius)
	{
		m_radius = radius;
	}

	public void SetDisplayablePrefab(GameObject go, NavigatorDescriptionType descType)
	{
		m_mapDisplayPrefab = go;
		m_navigationTypeDescription = descType;
	}

	public void SetShouldDisplay(bool shouldDisplay)
	{
		if (m_shouldDisplay != shouldDisplay)
		{
			m_shouldDisplay = shouldDisplay;
			if (!m_shouldDisplay)
			{
				Singleton<MissionMapController>.Instance.DeRegisterMapObject(this);
			}
			else
			{
				Singleton<MissionMapController>.Instance.RegisterMapObject(this);
			}
		}
	}

	private void OnDisable()
	{
		if (Singleton<MissionMapController>.Instance != null)
		{
			Singleton<MissionMapController>.Instance.DeRegisterMapObject(this);
		}
	}

	public NavigatorDescriptionType GetDescriptionType()
	{
		return m_navigationTypeDescription;
	}

	public GameObject GetDisplayPrefab()
	{
		return m_mapDisplayPrefab;
	}

	public bool RequiresVisibility()
	{
		return m_requiresVisibility;
	}

	public bool OutwardJourneyOnly()
	{
		return m_showOnOutwardJourneyOnly;
	}

	public float GetRadius()
	{
		return m_radius;
	}
}
