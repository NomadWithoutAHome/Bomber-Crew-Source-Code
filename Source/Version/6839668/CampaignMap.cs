using System.Collections.Generic;
using UnityEngine;

public class CampaignMap : MonoBehaviour
{
	[SerializeField]
	private Transform m_airbaseRootNode;

	[SerializeField]
	private float m_scale = 0.75f;

	[SerializeField]
	private Transform m_markerRotationReference;

	[SerializeField]
	private GameObject[] m_missionMarkerPrefabForRisk;

	[SerializeField]
	private GameObject[] m_missionMarkerPrefabForRiskNumbered;

	[SerializeField]
	private GameObject[] m_targetMarkerPrefabForRisk;

	[SerializeField]
	private GameObject m_routeLinePrefab;

	[SerializeField]
	private GameObject m_hazardMarkerPrefab;

	private List<GameObject> m_createdObjects = new List<GameObject>(32);

	private void Awake()
	{
		m_scale *= 0.0001f;
	}

	public void Clear()
	{
		foreach (GameObject createdObject in m_createdObjects)
		{
			Object.DestroyImmediate(createdObject);
		}
		m_createdObjects.Clear();
	}

	public Vector3 ConvertPos(Vector2 inPos)
	{
		return new Vector3(inPos.y * m_scale, 0f - inPos.x * m_scale, 0f);
	}

	public GameObject SetMissionTargetMarker(Vector2 missionPosition, bool interactive, List<Vector2> route, int risk, bool isCritical, int numbering, bool isComplete)
	{
		GameObject[] array = ((numbering != 0) ? m_missionMarkerPrefabForRiskNumbered : m_missionMarkerPrefabForRisk);
		risk = ((!isCritical) ? Mathf.Min(risk, 3) : 4);
		GameObject gameObject = Object.Instantiate((!interactive) ? m_targetMarkerPrefabForRisk[risk] : array[risk]);
		gameObject.transform.parent = m_airbaseRootNode;
		gameObject.transform.rotation = m_markerRotationReference.rotation;
		m_createdObjects.Add(gameObject);
		Vector3 localPosition = ConvertPos(missionPosition);
		gameObject.transform.localPosition = localPosition;
		GameObject gameObject2 = Object.Instantiate(m_routeLinePrefab);
		gameObject2.transform.parent = m_airbaseRootNode;
		gameObject2.transform.localPosition = Vector3.zero;
		gameObject2.transform.localEulerAngles = Vector3.zero;
		LineRenderer component = gameObject2.GetComponent<LineRenderer>();
		m_createdObjects.Add(gameObject2);
		if (route != null)
		{
			Vector3[] array2 = new Vector3[route.Count + 1];
			ref Vector3 reference = ref array2[0];
			reference = Vector3.zero;
			int num = 0;
			foreach (Vector2 item in route)
			{
				ref Vector3 reference2 = ref array2[num];
				reference2 = ConvertPos(item);
				num++;
			}
			component.positionCount = array2.Length;
			component.SetPositions(array2);
		}
		else
		{
			Vector3[] array3 = new Vector3[2]
			{
				Vector3.zero,
				ConvertPos(missionPosition)
			};
			component.positionCount = array3.Length;
			component.SetPositions(array3);
		}
		return gameObject;
	}

	public void SetHazardMarker(Vector2 hazardPosition)
	{
		GameObject gameObject = Object.Instantiate(m_hazardMarkerPrefab);
		gameObject.transform.parent = m_airbaseRootNode;
		gameObject.transform.rotation = m_markerRotationReference.rotation;
		Vector3 localPosition = new Vector3(hazardPosition.y * m_scale, 0f - hazardPosition.x * m_scale, 0f);
		gameObject.transform.localPosition = localPosition;
		m_createdObjects.Add(gameObject);
	}
}
