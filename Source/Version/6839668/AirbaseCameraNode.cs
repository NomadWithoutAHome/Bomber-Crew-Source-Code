using System;
using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class AirbaseCameraNode : MonoBehaviour
{
	[Serializable]
	public class BakedConnection
	{
		[SerializeField]
		public AirbaseCameraNode m_target;

		[SerializeField]
		public AirbaseCameraNode m_intermediate;
	}

	[SerializeField]
	private AirbaseCameraNode[] m_connectionCameras;

	[SerializeField]
	private int m_groupIndex;

	[SerializeField]
	private AirbasePersistentCrew m_persistentCrew;

	[SerializeField]
	private AirbasePersistentCrewTarget m_persistentCrewTarget;

	[SerializeField]
	private AirbaseCameraNode m_setTargetOnWayToNode;

	[SerializeField]
	private List<BakedConnection> m_bakedConnections = new List<BakedConnection>();

	private List<AirbaseCameraNode> m_twoWayConnections = new List<AirbaseCameraNode>();

	private Dictionary<AirbaseCameraNode, AirbaseCameraNode> m_bakedDictionary = new Dictionary<AirbaseCameraNode, AirbaseCameraNode>();

	private void Awake()
	{
		Singleton<AirbaseCameraController>.Instance.RegisterAirbaseCameraNode(this);
		if (m_connectionCameras != null)
		{
			m_twoWayConnections.AddRange(m_connectionCameras);
		}
		AirbaseCameraNode[] connectionCameras = m_connectionCameras;
		foreach (AirbaseCameraNode airbaseCameraNode in connectionCameras)
		{
			airbaseCameraNode.RegisterConnection(this);
		}
		foreach (BakedConnection bakedConnection in m_bakedConnections)
		{
			m_bakedDictionary[bakedConnection.m_target] = bakedConnection.m_intermediate;
		}
	}

	public void DoConnectionHooks(AirbaseCameraNode nextNode)
	{
		if (m_persistentCrew != null && m_persistentCrewTarget != null && m_setTargetOnWayToNode == nextNode)
		{
			m_persistentCrew.DoTargetedWalk(m_persistentCrewTarget);
		}
	}

	public void RegisterConnection(AirbaseCameraNode acn)
	{
		if (!m_twoWayConnections.Contains(acn))
		{
			m_twoWayConnections.Add(acn);
		}
	}

	public int GetGroupIndex()
	{
		return m_groupIndex;
	}

	public void InitPreBake()
	{
		m_twoWayConnections = new List<AirbaseCameraNode>();
		m_twoWayConnections.AddRange(m_connectionCameras);
	}

	public void PreBake()
	{
		AirbaseCameraNode[] connectionCameras = m_connectionCameras;
		foreach (AirbaseCameraNode airbaseCameraNode in connectionCameras)
		{
			airbaseCameraNode.RegisterConnection(this);
		}
		m_bakedConnections = new List<BakedConnection>();
	}

	public List<AirbaseCameraNode> GetFullPathBaked(AirbaseCameraNode target)
	{
		if (this == target)
		{
			List<AirbaseCameraNode> list = new List<AirbaseCameraNode>();
			list.Add(this);
			return list;
		}
		List<AirbaseCameraNode> fullPathBaked = m_bakedDictionary[target].GetFullPathBaked(target);
		fullPathBaked.Insert(0, this);
		return fullPathBaked;
	}

	public void BakeNavInfo(AirbaseCameraNode[] allNodes)
	{
		foreach (AirbaseCameraNode airbaseCameraNode in allNodes)
		{
			if (airbaseCameraNode != this)
			{
				BakedConnection bakedConnection = new BakedConnection();
				bakedConnection.m_target = airbaseCameraNode;
				List<AirbaseCameraNode> fullPath = GetFullPath(airbaseCameraNode, null, 0);
				if (fullPath != null && fullPath.Count > 1)
				{
					bakedConnection.m_intermediate = fullPath[1];
					m_bakedConnections.Add(bakedConnection);
					continue;
				}
				DebugLogWrapper.LogError(string.Concat("No pathing between ", this, " and ", airbaseCameraNode));
			}
		}
	}

	private float MeasureDistances(List<AirbaseCameraNode> path, AirbaseCameraNode newZero)
	{
		float num = 0f;
		if (path.Count > 0)
		{
			Vector3 position = newZero.transform.position;
			for (int i = 0; i < path.Count; i++)
			{
				num += (path[i].transform.position - position).magnitude;
				position = path[i].transform.position;
			}
		}
		return num;
	}

	public List<AirbaseCameraNode> GetFullPath(AirbaseCameraNode target, List<AirbaseCameraNode> alreadyTransitioned, int depth)
	{
		if (this == target)
		{
			List<AirbaseCameraNode> list = new List<AirbaseCameraNode>();
			list.Add(this);
			return list;
		}
		List<AirbaseCameraNode> list2 = null;
		foreach (AirbaseCameraNode twoWayConnection in m_twoWayConnections)
		{
			if (!(twoWayConnection != this))
			{
				continue;
			}
			List<AirbaseCameraNode> list3 = new List<AirbaseCameraNode>();
			if (alreadyTransitioned != null)
			{
				list3.AddRange(alreadyTransitioned);
			}
			if (!list3.Contains(twoWayConnection))
			{
				list3.AddRange(m_twoWayConnections);
				List<AirbaseCameraNode> fullPath = twoWayConnection.GetFullPath(target, list3, depth + 1);
				if (fullPath != null && (list2 == null || fullPath.Count < list2.Count))
				{
					list2 = fullPath;
				}
			}
		}
		list2?.Insert(0, this);
		return list2;
	}

	private void OnDrawGizmos()
	{
		if (m_connectionCameras != null)
		{
			AirbaseCameraNode[] connectionCameras = m_connectionCameras;
			foreach (AirbaseCameraNode airbaseCameraNode in connectionCameras)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawLine(base.transform.position, airbaseCameraNode.transform.position);
			}
		}
	}
}
