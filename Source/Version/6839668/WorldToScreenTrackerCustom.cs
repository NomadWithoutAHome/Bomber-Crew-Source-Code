using UnityEngine;

public class WorldToScreenTrackerCustom : MonoBehaviour
{
	[SerializeField]
	private tk2dCamera m_uiCamera;

	private Camera m_uiCameraCamera;

	[SerializeField]
	private bool m_zeroZPos;

	[SerializeField]
	private Transform m_transformToMove;

	[SerializeField]
	private Vector3 m_worldOffset = Vector3.zero;

	private tk2dCamera m_worldCamera;

	private Camera m_worldCameraCamera;

	private float m_zoomFactorUI;

	private float m_zoomFactorWorld;

	private float m_zPos;

	private Transform[] m_xPositionTransforms;

	private Transform[] m_yPositionTransforms;

	private void Start()
	{
		if (m_transformToMove == null)
		{
			m_transformToMove = base.transform;
		}
		m_worldCamera = Camera.main.GetComponent<tk2dCamera>();
		m_worldCameraCamera = m_worldCamera.GetComponent<Camera>();
		m_zoomFactorWorld = m_worldCamera.ZoomFactor;
		if (m_uiCamera != null)
		{
			m_zoomFactorUI = m_uiCamera.ZoomFactor;
			m_uiCameraCamera = m_uiCamera.GetComponent<Camera>();
		}
		m_zPos = ((!m_zeroZPos) ? m_transformToMove.position.z : 0f);
		LateUpdate();
	}

	public void SetTracking(Transform[] xTracking, Transform[] yTracking, tk2dCamera uiCamera)
	{
		m_xPositionTransforms = xTracking;
		m_yPositionTransforms = yTracking;
		m_worldCamera = Camera.main.GetComponent<tk2dCamera>();
		m_worldCameraCamera = m_worldCamera.GetComponent<Camera>();
		m_zoomFactorWorld = m_worldCamera.ZoomFactor;
		m_uiCamera = uiCamera;
		if (m_uiCamera != null)
		{
			m_zoomFactorUI = m_uiCamera.ZoomFactor;
			m_uiCameraCamera = m_uiCamera.GetComponent<Camera>();
		}
		if (m_xPositionTransforms != null && m_yPositionTransforms != null && m_xPositionTransforms.Length > 0 && m_yPositionTransforms.Length > 0)
		{
			LateUpdate();
		}
	}

	private void LateUpdate()
	{
		if (m_transformToMove == null)
		{
			m_transformToMove = base.transform;
		}
		if (m_xPositionTransforms != null && m_yPositionTransforms != null && m_xPositionTransforms.Length > 0 && m_yPositionTransforms.Length > 0)
		{
			m_zoomFactorWorld = m_worldCamera.ZoomFactor;
			m_zoomFactorUI = m_uiCamera.ZoomFactor;
			bool flag = false;
			bool isBehind = false;
			Vector3 zero = Vector3.zero;
			Vector3 zero2 = Vector3.zero;
			Vector3 forward = m_worldCamera.transform.forward;
			Vector3 position = m_worldCamera.transform.position;
			for (int i = 0; i < m_xPositionTransforms.Length; i++)
			{
				zero += GetPosition(m_xPositionTransforms[i], out isBehind, forward, position);
				flag = flag || isBehind;
			}
			for (int j = 0; j < m_yPositionTransforms.Length; j++)
			{
				zero2 += GetPosition(m_yPositionTransforms[j], out isBehind, forward, position);
				flag = flag || isBehind;
			}
			if (flag)
			{
				m_transformToMove.position = m_uiCamera.transform.position + (m_uiCameraCamera.nearClipPlane - 100f) * m_uiCamera.transform.forward;
			}
			else
			{
				m_transformToMove.position = new Vector3(zero.x / (float)m_xPositionTransforms.Length, zero2.y / (float)m_yPositionTransforms.Length, m_zPos);
			}
		}
	}

	public Vector3 GetPosition(Transform t, out bool isBehind, Vector3 worldFwd, Vector3 worldPos)
	{
		if (Vector3.Dot(worldFwd, t.position - worldPos) <= 0f)
		{
			isBehind = true;
		}
		else
		{
			isBehind = false;
		}
		Vector3 position = m_worldCameraCamera.WorldToScreenPoint(t.position + m_worldOffset);
		return m_uiCameraCamera.ScreenToWorldPoint(position);
	}

	public tk2dCamera GetCamera()
	{
		return m_uiCamera;
	}
}
