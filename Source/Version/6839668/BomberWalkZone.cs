using System;
using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class BomberWalkZone : MonoBehaviour
{
	[Serializable]
	public class BomberWalkZoneRouting
	{
		[SerializeField]
		public BomberWalkZone[] m_targetWalkZones;

		[SerializeField]
		public BomberWalkZoneConnector m_useConnector;
	}

	public class WalkTarget
	{
		public BomberWalkZone m_zone;

		public BomberWalkZoneConnector m_connector;

		public Vector3 m_zoneLocalPosition;
	}

	public class WalkPath
	{
		public List<WalkTarget> m_path = new List<WalkTarget>(10);

		public BomberWalkZone m_generatedFromWalkZone;
	}

	[SerializeField]
	private WalkableArea[] m_walkableAreas;

	[SerializeField]
	private BomberAllWalkZones m_allWalkZones;

	[SerializeField]
	private bool m_isExternal;

	[SerializeField]
	private Vector3 m_size;

	[SerializeField]
	private BomberWalkZoneRouting[] m_routing;

	[SerializeField]
	private bool m_dontIncludeInClickSearches;

	[SerializeField]
	private BomberDestroyableSection m_reliesOnSection;

	[SerializeField]
	private float m_stepHeight = 0.5f;

	[SerializeField]
	private Vector3 m_safeWalkZoneSize;

	public BomberWalkZoneRouting[] GetRouting()
	{
		return m_routing;
	}

	public Vector3 GetPlanarPoint(Vector3 pn)
	{
		Vector3 up = base.transform.up;
		Vector3 lhs = pn - base.transform.position;
		float num = Vector3.Dot(lhs, up);
		return pn - num * up;
	}

	public Vector3 GetPlanarPointLocal(Vector3 pn)
	{
		Vector3 up = base.transform.up;
		Vector3 lhs = pn - base.transform.position;
		float num = Vector3.Dot(lhs, up);
		Vector3 v = pn - num * up;
		return base.transform.worldToLocalMatrix.MultiplyPoint(v);
	}

	public Vector3 GetNearestPlanarPointLocalFast(Vector3 localPlanarIn)
	{
		Vector3 result = localPlanarIn;
		result.x = Mathf.Clamp(result.x, 0f - m_size.x, m_size.x);
		result.z = Mathf.Clamp(result.z, 0f - m_size.z, m_size.z);
		return result;
	}

	public Vector3 GetNearestPlanarPointLocalFastSafe(Vector3 localPlanarIn)
	{
		Vector3 result = localPlanarIn;
		result.x = Mathf.Clamp(result.x, 0f - m_size.x + m_safeWalkZoneSize.x, m_size.x - m_safeWalkZoneSize.x);
		result.z = Mathf.Clamp(result.z, 0f - m_size.z + m_safeWalkZoneSize.z, m_size.z - m_safeWalkZoneSize.z);
		return result;
	}

	public Vector3 GetNearestPlanarPointFast(Vector3 planarIn)
	{
		Vector3 v = base.transform.worldToLocalMatrix.MultiplyPoint(planarIn);
		v.x = Mathf.Clamp(v.x, 0f - m_size.x, m_size.x);
		v.z = Mathf.Clamp(v.z, 0f - m_size.z, m_size.z);
		return base.transform.localToWorldMatrix.MultiplyPoint(v);
	}

	public Vector3 GetNearestPlanarPoint(Vector3 pn)
	{
		Vector3 planarPoint = GetPlanarPoint(pn);
		Vector3 v = base.transform.worldToLocalMatrix.MultiplyPoint(planarPoint);
		v.x = Mathf.Clamp(v.x, 0f - m_size.x, m_size.x);
		v.z = Mathf.Clamp(v.z, 0f - m_size.z, m_size.z);
		return base.transform.localToWorldMatrix.MultiplyPoint(v);
	}

	public Vector3 GetNearestPlanarPointLocalFromWorld(Vector3 pn)
	{
		Vector3 nearestPlanarPoint = GetNearestPlanarPoint(pn);
		Vector3 result = base.transform.worldToLocalMatrix.MultiplyPoint(nearestPlanarPoint);
		result.x = Mathf.Clamp(result.x, 0f - m_size.x, m_size.x);
		result.z = Mathf.Clamp(result.z, 0f - m_size.z, m_size.z);
		return result;
	}

	public Vector3 GetPointFromLocal(Vector3 localPoint)
	{
		return base.transform.localToWorldMatrix.MultiplyPoint(localPoint);
	}

	private void Awake()
	{
		m_allWalkZones.RegisterWalkZone(this);
		if (m_reliesOnSection != null)
		{
			m_reliesOnSection.OnSectionDestroy += DestroyWalkableSection;
		}
	}

	public float GetStepHeight()
	{
		return m_stepHeight;
	}

	private void DestroyWalkableSection()
	{
		int currentCrewCount = Singleton<CrewContainer>.Instance.GetCurrentCrewCount();
		for (int i = 0; i < currentCrewCount; i++)
		{
			Crewman crewman = Singleton<CrewContainer>.Instance.GetCrewman(i);
			CrewmanAvatar avatarFor = Singleton<ContextControl>.Instance.GetAvatarFor(crewman);
			if (avatarFor.GetWalkZone() == this)
			{
				avatarFor.SetStation(null);
				avatarFor.SetNullWalkZone();
			}
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public WalkPath GetFullWalkPath(Vector3 currentPositionOfTarget, WalkPath pathIn)
	{
		BomberWalkZone bwz = m_allWalkZones.FindContainingWalkZone(currentPositionOfTarget, ignoreExternal: false);
		return GetFullWalkPath(bwz, currentPositionOfTarget, pathIn);
	}

	public BomberAllWalkZones GetAllWalkZonesInGroup()
	{
		return m_allWalkZones;
	}

	public bool ShouldSkipInClickSearch()
	{
		return m_dontIncludeInClickSearches;
	}

	public WalkPath GetFullWalkPath(BomberWalkZone bwz, Vector3 currentPositionOfTarget, WalkPath pathIn)
	{
		WalkPath walkPath = null;
		if (pathIn == null)
		{
			walkPath = new WalkPath();
			walkPath.m_generatedFromWalkZone = this;
		}
		else
		{
			walkPath = pathIn;
		}
		int count = walkPath.m_path.Count;
		if (count >= 1 && walkPath.m_generatedFromWalkZone == this && walkPath.m_path[count - 1].m_zone == bwz)
		{
			walkPath.m_path[count - 1].m_zoneLocalPosition = bwz.GetNearestPlanarPointLocalFromWorld(currentPositionOfTarget);
			return walkPath;
		}
		walkPath.m_path.Clear();
		walkPath.m_generatedFromWalkZone = this;
		BomberWalkZone bomberWalkZone = this;
		bool flag = true;
		while (bomberWalkZone != bwz)
		{
			bool flag2 = false;
			BomberWalkZoneRouting[] routing = bomberWalkZone.GetRouting();
			foreach (BomberWalkZoneRouting bomberWalkZoneRouting in routing)
			{
				bool flag3 = bomberWalkZoneRouting.m_targetWalkZones.Length == 0;
				BomberWalkZone[] targetWalkZones = bomberWalkZoneRouting.m_targetWalkZones;
				foreach (BomberWalkZone bomberWalkZone2 in targetWalkZones)
				{
					if (bomberWalkZone2 == bwz)
					{
						flag3 = true;
						break;
					}
				}
				if (flag3)
				{
					if (bomberWalkZone == null)
					{
						flag = false;
						flag2 = false;
						break;
					}
					WalkTarget walkTarget = new WalkTarget();
					walkTarget.m_connector = bomberWalkZoneRouting.m_useConnector;
					walkTarget.m_zone = bomberWalkZone;
					walkTarget.m_zoneLocalPosition = bomberWalkZone.GetNearestPlanarPointLocalFromWorld(bomberWalkZoneRouting.m_useConnector.transform.position);
					walkPath.m_path.Add(walkTarget);
					bomberWalkZone = walkTarget.m_connector.GetTargetConnector().GetThisWalkZone();
					flag2 = true;
					break;
				}
			}
			if (!flag2)
			{
				break;
			}
		}
		if (flag)
		{
			WalkTarget walkTarget2 = new WalkTarget();
			walkTarget2.m_zone = bwz;
			walkTarget2.m_zoneLocalPosition = bwz.GetNearestPlanarPointLocalFromWorld(currentPositionOfTarget);
			walkPath.m_path.Add(walkTarget2);
		}
		return walkPath;
	}

	public bool IsExternal()
	{
		return m_isExternal;
	}

	public void OnDrawGizmos()
	{
		Vector3 position = base.transform.position;
		Vector3 from = base.transform.position + base.transform.right * (m_size.x - m_safeWalkZoneSize.x) + base.transform.forward * (m_size.z - m_safeWalkZoneSize.z);
		Vector3 to = base.transform.position + base.transform.right * (0f - (m_size.x - m_safeWalkZoneSize.x)) + base.transform.forward * (m_size.z - m_safeWalkZoneSize.z);
		Vector3 vector = base.transform.position + base.transform.right * (m_size.x - m_safeWalkZoneSize.x) + base.transform.forward * (0f - (m_size.z - m_safeWalkZoneSize.z));
		Vector3 vector2 = base.transform.position + base.transform.right * (0f - (m_size.x - m_safeWalkZoneSize.x)) + base.transform.forward * (0f - (m_size.z - m_safeWalkZoneSize.z));
		Gizmos.color = Color.red;
		Gizmos.DrawLine(from, to);
		Gizmos.DrawLine(vector, vector2);
		Gizmos.DrawLine(vector2, to);
		Gizmos.DrawLine(from, vector);
		Gizmos.color = Color.white;
		Vector3 from2 = base.transform.position + base.transform.right * m_size.x + base.transform.forward * m_size.z;
		Vector3 to2 = base.transform.position + base.transform.right * (0f - m_size.x) + base.transform.forward * m_size.z;
		Vector3 vector3 = base.transform.position + base.transform.right * m_size.x + base.transform.forward * (0f - m_size.z);
		Vector3 vector4 = base.transform.position + base.transform.right * (0f - m_size.x) + base.transform.forward * (0f - m_size.z);
		Gizmos.color = Color.gray;
		Gizmos.DrawLine(from2, to2);
		Gizmos.DrawLine(vector3, vector4);
		Gizmos.DrawLine(vector4, to2);
		Gizmos.DrawLine(from2, vector3);
		Gizmos.color = Color.white;
	}

	public void OnDrawGizmosSelected()
	{
		Vector3 position = base.transform.position;
		Vector3 from = base.transform.position + base.transform.right * m_size.x + base.transform.forward * m_size.z;
		Vector3 to = base.transform.position + base.transform.right * (0f - m_size.x) + base.transform.forward * m_size.z;
		Vector3 vector = base.transform.position + base.transform.right * m_size.x + base.transform.forward * (0f - m_size.z);
		Vector3 vector2 = base.transform.position + base.transform.right * (0f - m_size.x) + base.transform.forward * (0f - m_size.z);
		Gizmos.DrawLine(from, to);
		Gizmos.DrawLine(vector, vector2);
		Gizmos.DrawLine(vector2, to);
		Gizmos.DrawLine(from, vector);
	}
}
