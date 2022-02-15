using BomberCrewCommon;
using Common;
using UnityEngine;

public class MapUI : Singleton<MapUI>
{
	[SerializeField]
	private GameObject m_mapDisplay;

	[SerializeField]
	private Transform m_bomberMarker;

	[SerializeField]
	private Transform m_targetMarker;

	[SerializeField]
	private Transform m_baseMarker;

	[SerializeField]
	private LineRenderer m_lineRenderer;

	[SerializeField]
	private Transform m_origin;

	[SerializeField]
	private LayerMask m_uiLayer;

	[SerializeField]
	private tk2dUIItem m_mapClickItem;

	[SerializeField]
	private tk2dUIItem m_clearCustomButton;

	[SerializeField]
	private float m_worldSizeRepresentation;

	[SerializeField]
	private Transform m_mapScale;

	[SerializeField]
	private Matrix4x4 m_convertCoordinateMatrix;

	private BomberNavigation m_bomberNavigation;

	private BomberNavigation.NavigationPoint m_currentNavigationPoint;

	private Vector3d m_dragStartPos;

	private Camera m_uiCamera;

	private void Start()
	{
		m_bomberNavigation = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberNavigation();
		Camera[] array = Object.FindObjectsOfType<Camera>();
		Camera[] array2 = array;
		foreach (Camera camera in array2)
		{
			if ((camera.cullingMask & (int)m_uiLayer) != 0)
			{
				m_uiCamera = camera;
			}
		}
		if (m_mapDisplay != null)
		{
			m_mapDisplay.SetActive(value: false);
		}
		Vector3 position = m_lineRenderer.transform.position;
		position.x = 0f;
		position.y = 0f;
		m_lineRenderer.transform.position = position;
	}

	public void ShowMap(bool show)
	{
		m_mapDisplay.CustomActivate(show);
	}

	public bool isMapShowing()
	{
		return m_mapDisplay.IsActivated();
	}

	public void Update()
	{
		m_baseMarker.transform.position = GetFromPositionXZ(Vector3d.zero);
		MissionPlaceableObject objectByType = Singleton<MissionCoordinator>.Instance.GetObjectByType("MissionTarget");
		if (objectByType != null)
		{
			m_targetMarker.transform.position = GetFromPositionXZ(objectByType.gameObject.btransform().position);
		}
		Vector3d position = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBigTransform().position;
		m_bomberMarker.transform.position = GetFromPositionXZ(position);
		Vector3 eulerAngles = Singleton<BomberSpawn>.Instance.GetBomberSystems().transform.localRotation.eulerAngles;
		m_bomberMarker.transform.localRotation = Quaternion.Euler(0f, 0f, 0f - eulerAngles.y - 90f);
		if (m_currentNavigationPoint != null)
		{
			Vector3 position2 = m_mapClickItem.Touch.position;
			Vector3 pos = m_uiCamera.ScreenToWorldPoint(position2);
			pos.z = 0f;
			Vector3d xZFromPosition = GetXZFromPosition(pos);
			m_currentNavigationPoint.m_position = xZFromPosition;
		}
	}

	public float GetWorldToMapScalar()
	{
		return m_mapScale.localScale.x / m_worldSizeRepresentation;
	}

	public Vector3 GetFromPositionXZ(Vector3d pos)
	{
		Vector3 v = (Vector3)(pos * GetWorldToMapScalar());
		Vector3 vector = m_convertCoordinateMatrix.MultiplyPoint(v);
		vector.z = 0f;
		return vector + m_origin.transform.position;
	}

	public Vector3d GetXZFromPosition(Vector3 pos)
	{
		Vector3d result = new Vector3d(m_convertCoordinateMatrix.inverse.MultiplyPoint(pos - m_origin.transform.position));
		result *= (double)(1f / GetWorldToMapScalar());
		result.y = 0.0;
		return result;
	}
}
