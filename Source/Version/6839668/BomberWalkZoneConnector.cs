using UnityEngine;

public class BomberWalkZoneConnector : MonoBehaviour
{
	[SerializeField]
	private BomberWalkZone m_myWalkZone;

	[SerializeField]
	private BomberWalkZoneConnector m_targetConnector;

	[SerializeField]
	private float m_timeToUseConnection;

	[SerializeField]
	private string m_animationTriggerForConnection;

	[SerializeField]
	private bool m_connectionRequiresWarp;

	[SerializeField]
	private HatchDoor m_hatchToActivate;

	public BomberWalkZoneConnector GetTargetConnector()
	{
		return m_targetConnector;
	}

	public BomberWalkZone GetThisWalkZone()
	{
		return m_myWalkZone;
	}

	public bool RequiresWarp()
	{
		return m_connectionRequiresWarp;
	}

	public string GetAnimationTriggerForConnection()
	{
		return m_animationTriggerForConnection;
	}

	public HatchDoor GetHatchToActivate()
	{
		return m_hatchToActivate;
	}

	public void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(base.transform.position, 0.2f);
		if (m_targetConnector != null)
		{
			Gizmos.DrawLine(base.transform.position, m_targetConnector.transform.position);
		}
		if (m_myWalkZone != null)
		{
			Gizmos.DrawLine(base.transform.position, m_myWalkZone.GetNearestPlanarPoint(base.transform.position));
		}
	}
}
