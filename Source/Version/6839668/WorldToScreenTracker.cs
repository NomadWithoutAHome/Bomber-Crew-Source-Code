using UnityEngine;

public class WorldToScreenTracker : MonoBehaviour
{
	[SerializeField]
	private Transform m_trackingTransform;

	[SerializeField]
	private tk2dCamera m_uiCamera;

	[SerializeField]
	private bool m_zeroZPos;

	[SerializeField]
	private Transform m_transformToMove;

	private tk2dCamera m_worldCamera;

	private float m_zoomFactorUI;

	private float m_zoomFactorWorld;

	private float m_zPos;

	private void Start()
	{
		if (m_transformToMove == null)
		{
			m_transformToMove = base.transform;
		}
		m_worldCamera = Camera.main.GetComponent<tk2dCamera>();
		m_zoomFactorWorld = m_worldCamera.ZoomFactor;
		if (m_uiCamera != null)
		{
			m_zoomFactorUI = m_uiCamera.ZoomFactor;
		}
		m_zPos = ((!m_zeroZPos) ? m_transformToMove.position.z : 0f);
		LateUpdate();
	}

	public void UpdateTracking(Transform tracking)
	{
		m_worldCamera = Camera.main.GetComponent<tk2dCamera>();
		m_zoomFactorWorld = m_worldCamera.ZoomFactor;
		m_trackingTransform = tracking;
		if (m_trackingTransform != null)
		{
			LateUpdate();
		}
	}

	public void SetTracking(Transform tracking, tk2dCamera uiCamera)
	{
		m_worldCamera = Camera.main.GetComponent<tk2dCamera>();
		m_zoomFactorWorld = m_worldCamera.ZoomFactor;
		m_trackingTransform = tracking;
		m_uiCamera = uiCamera;
		if (m_uiCamera != null)
		{
			m_zoomFactorUI = m_uiCamera.ZoomFactor;
		}
		if (m_trackingTransform != null)
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
		if (m_trackingTransform != null)
		{
			m_zoomFactorWorld = m_worldCamera.ZoomFactor;
			m_zoomFactorUI = m_uiCamera.ZoomFactor;
			bool isBehind = false;
			Vector3 position = GetPosition(out isBehind);
			if (isBehind)
			{
				m_transformToMove.position = m_uiCamera.transform.position + (m_uiCamera.GetComponent<Camera>().nearClipPlane - 100f) * m_uiCamera.transform.forward;
			}
			else
			{
				m_transformToMove.position = new Vector3(position.x, position.y, m_zPos);
			}
		}
	}

	public Transform GetTrackingTransform()
	{
		return m_trackingTransform;
	}

	public Vector3 GetPosition(out bool isBehind)
	{
		if (m_trackingTransform != null)
		{
			if (Vector3.Dot(m_worldCamera.transform.forward, (m_trackingTransform.position - m_worldCamera.transform.position).normalized) <= 0f)
			{
				isBehind = true;
			}
			else
			{
				isBehind = false;
			}
			Vector3 position = m_worldCamera.GetComponent<Camera>().WorldToScreenPoint(m_trackingTransform.position);
			return m_uiCamera.GetComponent<Camera>().ScreenToWorldPoint(position);
		}
		isBehind = true;
		return Vector3.zero;
	}

	public tk2dCamera GetCamera()
	{
		return m_uiCamera;
	}
}
