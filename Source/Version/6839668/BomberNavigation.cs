using BomberCrewCommon;
using UnityEngine;

public class BomberNavigation : MonoBehaviour
{
	public enum NavigationPointType
	{
		Navigation,
		FunctionLandingBombing,
		Detour
	}

	public class NavigationPoint
	{
		public Vector3d m_position;

		public bool m_switchToLandingState;

		public bool m_goToNextPreLanding;

		public int m_nextPreLanding;

		public bool m_hasDirection;

		public bool m_isUserPlaced;

		public bool m_holdSteady;

		public Vector3 m_direction;

		public NavigationPointType m_navPointType;

		public GameObject m_associatedObject;

		public bool m_forceNormalLowAltitude;

		public bool m_forceUltraLowAltitude;

		public NavigationPoint m_next;
	}

	[SerializeField]
	private BomberState m_bomberState;

	private NavigationPoint m_navigationPoint;

	public NavigationPoint GetNextNavigationPoint()
	{
		return m_navigationPoint;
	}

	public NavigationPointType? GetCurrentType()
	{
		return (m_navigationPoint != null) ? new NavigationPointType?(m_navigationPoint.m_navPointType) : null;
	}

	public void MarkNavigationPointReached()
	{
		if (m_navigationPoint != null)
		{
			m_navigationPoint = m_navigationPoint.m_next;
		}
		else
		{
			m_navigationPoint = null;
		}
	}

	public void SetNavigationPoint(NavigationPoint targetPoint)
	{
		m_navigationPoint = targetPoint;
	}

	public void SetNavigationPoint(Vector3d position, NavigationPointType pointType, GameObject associatedObject)
	{
		NavigationPoint navigationPoint = new NavigationPoint();
		navigationPoint.m_position = position;
		navigationPoint.m_hasDirection = false;
		navigationPoint.m_switchToLandingState = false;
		navigationPoint.m_holdSteady = false;
		navigationPoint.m_navPointType = pointType;
		navigationPoint.m_associatedObject = associatedObject;
		m_navigationPoint = navigationPoint;
	}

	public void SetPreLanding(int landingIndex)
	{
		NavigationPoint navigationPoint = new NavigationPoint();
		MissionPlaceableObject objectByType = Singleton<MissionCoordinator>.Instance.GetObjectByType("MissionStart");
		MissionLandingArea component = objectByType.GetComponent<MissionLandingArea>();
		navigationPoint.m_position = component.GetLandingStartPosition(landingIndex);
		navigationPoint.m_forceNormalLowAltitude = true;
		if (landingIndex > 2)
		{
			navigationPoint.m_forceUltraLowAltitude = true;
		}
		if (landingIndex == component.GetNumLandingStartPositions() - 1)
		{
			navigationPoint.m_switchToLandingState = true;
			navigationPoint.m_direction = component.GetAlignment();
		}
		else
		{
			navigationPoint.m_goToNextPreLanding = true;
			navigationPoint.m_nextPreLanding = landingIndex + 1;
			navigationPoint.m_direction = (Vector3)(component.GetLandingStartPosition(landingIndex + 1) - component.GetLandingStartPosition(landingIndex)).normalized;
		}
		navigationPoint.m_hasDirection = true;
		navigationPoint.m_navPointType = NavigationPointType.FunctionLandingBombing;
		navigationPoint.m_associatedObject = component.gameObject;
		m_navigationPoint = navigationPoint;
	}

	public void SetFinalLanding()
	{
		NavigationPoint navigationPoint = new NavigationPoint();
		MissionPlaceableObject objectByType = Singleton<MissionCoordinator>.Instance.GetObjectByType("MissionStart");
		MissionLandingArea component = objectByType.GetComponent<MissionLandingArea>();
		navigationPoint.m_position = objectByType.gameObject.btransform().position;
		navigationPoint.m_hasDirection = false;
		navigationPoint.m_direction = component.GetAlignment();
		navigationPoint.m_holdSteady = false;
		navigationPoint.m_navPointType = NavigationPointType.FunctionLandingBombing;
		navigationPoint.m_associatedObject = component.gameObject;
		m_navigationPoint = navigationPoint;
	}

	public GameObject GetAssociatedObject()
	{
		if (m_navigationPoint == null)
		{
			return null;
		}
		return m_navigationPoint.m_associatedObject;
	}
}
