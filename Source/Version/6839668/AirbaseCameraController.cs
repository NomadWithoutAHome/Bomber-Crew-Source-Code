using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class AirbaseCameraController : Singleton<AirbaseCameraController>
{
	[SerializeField]
	private Transform m_cameraRoot;

	[SerializeField]
	private Camera m_camera;

	[SerializeField]
	private AirbaseCameraNode m_startNode;

	[SerializeField]
	private float m_maxSpeed;

	[SerializeField]
	private float m_acceleration;

	[SerializeField]
	private float m_targetSpeedDistanceMultiplier;

	[SerializeField]
	private Blur m_blurShader;

	private List<AirbaseCameraNode> m_allNodes = new List<AirbaseCameraNode>();

	private Dictionary<string, AirbaseCameraNode> m_bomberUpgradeCameras = new Dictionary<string, AirbaseCameraNode>();

	private AirbaseCameraNode m_currentCameraNode;

	private AirbaseCameraNode m_currentTargetCameraNode;

	private List<AirbaseCameraNode> m_currentPath = new List<AirbaseCameraNode>();

	private Vector3 m_currentVelocity;

	private int m_pauseCounter;

	private float m_currentBlur;

	private float m_blurTarget;

	[SerializeField]
	private float m_blurSpeedMultiplier = 2f;

	private bool m_doDoFLerp;

	private Quaternion m_startRotation;

	private float m_startFov;

	private float m_startRotationAmt;

	public void RegisterAirbaseCameraNode(AirbaseCameraNode acn)
	{
		m_allNodes.Add(acn);
		BomberSystemUniqueIdNameHolder component = acn.GetComponent<BomberSystemUniqueIdNameHolder>();
		if (component != null)
		{
			m_bomberUpgradeCameras[component.GetUniqueId()] = acn;
		}
	}

	public void SetPaused(bool paused)
	{
		if (paused)
		{
			m_pauseCounter++;
			return;
		}
		m_pauseCounter--;
		if (m_pauseCounter < 0)
		{
			m_pauseCounter = 0;
		}
	}

	private void Awake()
	{
		m_doDoFLerp = QualitySettings.names[QualitySettings.GetQualityLevel()] != "Low";
		MoveCameraToLocationInstant(m_startNode);
		if (!m_doDoFLerp)
		{
			DepthOfField component = m_camera.GetComponent<DepthOfField>();
			component.enabled = false;
		}
	}

	public AirbaseCameraNode GetCameraNodeFor(string uniqueId)
	{
		return m_bomberUpgradeCameras[uniqueId];
	}

	public void MoveCameraToLocation(AirbaseCameraNode targetNode)
	{
		if (targetNode != m_currentTargetCameraNode)
		{
			m_currentTargetCameraNode = targetNode;
			m_startRotation = m_cameraRoot.transform.rotation;
			m_startRotationAmt = 0f;
			m_startFov = m_camera.fieldOfView;
			AirbaseCameraNode currentCameraNode = m_currentCameraNode;
			if (m_currentPath != null && m_currentPath.Count > 0)
			{
				m_currentCameraNode = m_currentPath[0];
			}
			m_currentPath = m_currentCameraNode.GetFullPathBaked(targetNode);
			if (m_currentPath.Count > 1 && m_currentPath[0] == m_currentCameraNode)
			{
				m_currentPath.RemoveAt(0);
			}
			if (currentCameraNode != m_currentCameraNode && m_currentPath.Count > 0)
			{
				m_currentCameraNode.DoConnectionHooks(m_currentPath[0]);
			}
		}
	}

	public bool IsMoving()
	{
		return m_currentCameraNode != m_currentTargetCameraNode;
	}

	public void MoveCameraToLocationInstant(AirbaseCameraNode targetNode)
	{
		m_currentCameraNode = targetNode;
		m_currentTargetCameraNode = targetNode;
		Camera component = targetNode.GetComponent<Camera>();
		m_cameraRoot.SetPositionAndRotation(component.transform.position, component.transform.rotation);
		m_camera.fieldOfView = component.fieldOfView;
		m_camera.cullingMask = component.cullingMask;
		m_startRotation = component.transform.rotation;
		m_startFov = component.fieldOfView;
		m_startRotationAmt = 1f;
		m_currentVelocity = Vector3.zero;
		m_currentPath = null;
	}

	public void EnableBlur(bool enable, bool instant)
	{
		if (enable)
		{
			if (instant)
			{
				m_blurTarget = 1f;
				m_currentBlur = 1f;
			}
			else
			{
				m_blurTarget = 1f;
			}
		}
		else if (instant)
		{
			m_blurTarget = 0f;
			m_currentBlur = 0f;
		}
		else
		{
			m_blurTarget = 0f;
		}
	}

	private void LerpDepthOfField(Camera a, Camera b, float amt)
	{
		if (m_doDoFLerp)
		{
			DepthOfField component = a.GetComponent<DepthOfField>();
			DepthOfField component2 = b.GetComponent<DepthOfField>();
			DepthOfField component3 = m_camera.GetComponent<DepthOfField>();
			component3.focalLength = Mathf.Lerp(component.focalLength, component2.focalLength, amt);
			component3.focalSize = Mathf.Lerp(component.focalSize, component2.focalSize, amt);
			component3.aperture = Mathf.Lerp(component.aperture, component2.aperture, amt);
			component3.maxBlurSize = Mathf.Lerp(component.maxBlurSize, component2.maxBlurSize, amt);
			component3.foregroundOverlap = Mathf.Lerp(component.foregroundOverlap, component2.foregroundOverlap, amt);
		}
	}

	private void Update()
	{
		if (m_pauseCounter > 0)
		{
			return;
		}
		if (m_blurShader != null)
		{
			if (m_blurTarget > m_currentBlur)
			{
				m_currentBlur += Time.unscaledDeltaTime * m_blurSpeedMultiplier;
				if (m_currentBlur > 1f)
				{
					m_currentBlur = 1f;
				}
			}
			if (m_blurTarget < m_currentBlur)
			{
				m_currentBlur -= Time.unscaledDeltaTime * m_blurSpeedMultiplier;
				if (m_currentBlur < 0f)
				{
					m_currentBlur = 0f;
				}
			}
			m_blurShader.blurAmount = m_currentBlur;
		}
		if (m_currentPath != null && m_currentPath.Count > 0)
		{
			AirbaseCameraNode airbaseCameraNode = m_currentPath[0];
			if (!(airbaseCameraNode != null))
			{
				return;
			}
			m_camera.cullingMask = airbaseCameraNode.GetComponent<Camera>().cullingMask;
			AirbaseCameraNode airbaseCameraNode2 = null;
			AirbaseCameraNode airbaseCameraNode3 = null;
			int groupIndex = m_currentCameraNode.GetGroupIndex();
			foreach (AirbaseCameraNode item in m_currentPath)
			{
				if (item.GetGroupIndex() != groupIndex)
				{
					airbaseCameraNode2 = ((!(airbaseCameraNode3 == null)) ? airbaseCameraNode3 : item);
					break;
				}
				airbaseCameraNode3 = item;
			}
			if (airbaseCameraNode2 == null)
			{
				airbaseCameraNode2 = m_currentTargetCameraNode;
			}
			Vector3 vector = airbaseCameraNode2.transform.position - m_cameraRoot.transform.position;
			Vector3 lhs = airbaseCameraNode.transform.position - m_cameraRoot.transform.position;
			float num = vector.magnitude * m_targetSpeedDistanceMultiplier;
			Vector3 vector2 = Vector3.ClampMagnitude(num * lhs.normalized, m_maxSpeed) - m_currentVelocity;
			m_currentVelocity += vector2 * Time.deltaTime * m_acceleration;
			m_cameraRoot.transform.position += m_currentVelocity * Time.deltaTime;
			float magnitude = (airbaseCameraNode.transform.position - m_currentCameraNode.transform.position).magnitude;
			Vector3 vector3 = airbaseCameraNode.transform.position - m_cameraRoot.transform.position;
			if (magnitude < 1f)
			{
				m_cameraRoot.transform.rotation = Quaternion.Lerp(m_cameraRoot.transform.rotation, airbaseCameraNode.transform.rotation, Mathf.Clamp01(5f * Time.deltaTime));
				m_startRotation = m_cameraRoot.transform.rotation;
				m_camera.fieldOfView = Mathf.Lerp(m_camera.fieldOfView, airbaseCameraNode.GetComponent<Camera>().fieldOfView, Mathf.Clamp01(5f * Time.deltaTime));
				m_startFov = m_camera.fieldOfView;
				m_startRotationAmt = 1f;
				m_camera.fieldOfView = Mathf.Lerp(m_camera.fieldOfView, airbaseCameraNode.GetComponent<Camera>().fieldOfView, Mathf.Clamp01(5f * Time.deltaTime));
				LerpDepthOfField(m_camera, airbaseCameraNode.GetComponent<Camera>(), Mathf.Clamp01(5f * Time.deltaTime));
			}
			else
			{
				float value = 1f - vector3.magnitude / magnitude;
				float num2 = Mathf.Clamp01(value);
				m_startRotationAmt += Time.deltaTime * 2f;
				m_startRotationAmt = Mathf.Clamp01(m_startRotationAmt);
				m_cameraRoot.transform.rotation = Quaternion.Lerp(Quaternion.Lerp(m_startRotation, m_currentCameraNode.transform.rotation, m_startRotationAmt), airbaseCameraNode.transform.rotation, num2);
				m_camera.fieldOfView = Mathf.Lerp(Mathf.Lerp(m_startFov, m_currentCameraNode.GetComponent<Camera>().fieldOfView, m_startRotationAmt), airbaseCameraNode.GetComponent<Camera>().fieldOfView, num2);
				LerpDepthOfField(m_currentCameraNode.GetComponent<Camera>(), airbaseCameraNode.GetComponent<Camera>(), num2);
			}
			Vector3 rhs = airbaseCameraNode.transform.position - m_cameraRoot.transform.position;
			if (lhs.magnitude < 0.1f || Vector3.Dot(lhs, rhs) < 0f)
			{
				m_startRotation = m_cameraRoot.transform.rotation;
				m_startFov = m_camera.fieldOfView;
				m_startRotationAmt = 1f;
				m_currentCameraNode = airbaseCameraNode;
				m_currentPath.RemoveAt(0);
				if (m_currentPath.Count == 0)
				{
					m_currentPath = null;
				}
				else
				{
					m_currentCameraNode.DoConnectionHooks(m_currentPath[0]);
				}
			}
		}
		else
		{
			m_currentPath = null;
			m_currentCameraNode = m_currentTargetCameraNode;
			Vector3 vector4 = m_currentCameraNode.transform.position - m_cameraRoot.transform.position;
			m_cameraRoot.transform.position += vector4 * Time.deltaTime * 3f;
			m_cameraRoot.transform.rotation = Quaternion.Lerp(m_cameraRoot.transform.rotation, m_currentCameraNode.transform.rotation, Mathf.Clamp01(5f * Time.deltaTime));
			m_startRotation = m_cameraRoot.transform.rotation;
			m_camera.fieldOfView = Mathf.Lerp(m_camera.fieldOfView, m_currentCameraNode.GetComponent<Camera>().fieldOfView, Mathf.Clamp01(5f * Time.deltaTime));
			m_startFov = m_camera.fieldOfView;
			m_camera.cullingMask = m_currentCameraNode.GetComponent<Camera>().cullingMask;
			LerpDepthOfField(m_camera, m_currentCameraNode.GetComponent<Camera>(), Mathf.Clamp01(5f * Time.deltaTime));
		}
	}
}
