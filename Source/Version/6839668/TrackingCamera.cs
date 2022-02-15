using System;
using AudioNames;
using BomberCrewCommon;
using Rewired;
using UnityEngine;
using UnityStandardAssets.ImageEffects;
using WingroveAudio;

public class TrackingCamera : MonoBehaviour
{
	[SerializeField]
	private float m_headMovementInterpFactor = 3f;

	[SerializeField]
	private float m_eyeAcceleration = 3f;

	[SerializeField]
	private float m_dampingFactor = 0.2f;

	[SerializeField]
	private Transform m_rotateXTransform;

	[SerializeField]
	private Transform m_rotateYTransform;

	[SerializeField]
	private Transform m_extendArmTransform;

	[SerializeField]
	private Camera[] m_cameras;

	[SerializeField]
	private DepthOfField m_depthOfField;

	[SerializeField]
	private float m_distanceAudioParamMin;

	[SerializeField]
	private float m_distanceAudioParamMax;

	[SerializeField]
	private Transform m_mainCameraTransform;

	[SerializeField]
	private RumbleChildTransform m_rumbler;

	private Vector3 m_velocity;

	private Transform m_nodeToTrack;

	private float m_distance;

	private bool m_worldAligned;

	private float m_xRotation;

	private float m_yRotation;

	private float m_lockToTrackAmount;

	private float m_hitAtPoint;

	private float m_fov;

	private float m_heightOffsetTarget;

	private bool m_hitGround;

	private float m_blendToAnimFactor;

	private Camera m_animTrackCamera;

	private Animation m_animationToPlay;

	private bool m_trackXZ;

	private bool m_trackY;

	private float m_xMovementBuf;

	public static bool m_extraCameraLogging;

	private void Start()
	{
	}

	private void OnDestroy()
	{
	}

	private void OnGUI()
	{
		if (m_extraCameraLogging)
		{
			GUI.Label(new Rect(0f, 0f, 128f, 32f), Time.timeSinceLevelLoad.ToString());
		}
	}

	private void TrackingCameraDebug()
	{
		m_extraCameraLogging = GUILayout.Toggle(m_extraCameraLogging, "EXTRA CAMERA LOGGING");
	}

	public void Set(Transform nodeToTrack, float distance, bool worldAligned)
	{
		m_nodeToTrack = nodeToTrack;
		m_distance = distance;
		m_worldAligned = worldAligned;
	}

	public Transform GetMainCameraNode()
	{
		return m_mainCameraTransform;
	}

	public void SetLockToTrackingAmount(float amt)
	{
		m_lockToTrackAmount = amt;
	}

	public float GetDistance()
	{
		return m_extendArmTransform.localPosition.z;
	}

	public void GetAngleFromCurrent()
	{
	}

	private void UpdateTrackingPosition()
	{
		Vector3 zero = Vector3.zero;
		Vector3 position = Singleton<BomberSpawn>.Instance.GetBomberSystems().transform.position;
		if (m_trackXZ)
		{
			zero.x = position.x;
			zero.z = position.z;
		}
		if (m_trackY)
		{
			zero.y = position.y;
		}
		m_animationToPlay.transform.position = zero;
	}

	public void DoCameraTrackAnim(Camera c, Animation anim, bool trackXZ, bool trackY)
	{
		m_trackXZ = trackXZ;
		m_trackY = trackY;
		m_blendToAnimFactor = 1f;
		m_animTrackCamera = c;
		m_animationToPlay = anim;
		Vector3 position = m_animTrackCamera.transform.position;
		m_mainCameraTransform.position = position;
		Quaternion rotation = m_animTrackCamera.transform.rotation;
		m_mainCameraTransform.rotation = rotation;
	}

	public void UpdateAngle(float xRot, float yRot)
	{
		m_xRotation = xRot;
		m_yRotation = yRot;
	}

	public void SetFov(float fov)
	{
		m_fov = fov;
	}

	public float GetXRotation()
	{
		return m_rotateXTransform.localEulerAngles.y;
	}

	public float GetYRotation()
	{
		return 0f;
	}

	public void SetHeightOffset(float hoff)
	{
		m_heightOffsetTarget = hoff;
	}

	public bool HitGround()
	{
		return m_hitGround;
	}

	public float GetHitAtPoint()
	{
		if (m_hitAtPoint > 180f)
		{
			m_hitAtPoint -= 360f;
		}
		if (m_hitAtPoint < -180f)
		{
			m_hitAtPoint += 360f;
		}
		return m_hitAtPoint;
	}

	public bool IsDoingCameraAnim()
	{
		return m_animTrackCamera != null;
	}

	private void LogRecursively(Transform t, string ssf)
	{
		string text = ssf;
		ssf = string.Concat(text, "Transform: ", t.name, " pos: ", t.position, "\n");
		foreach (Transform item in t)
		{
			LogRecursively(item, ssf);
		}
		if (t.childCount != 0)
		{
		}
	}

	private void FixedUpdate()
	{
		if (m_nodeToTrack != null)
		{
			Vector3 position = m_nodeToTrack.position;
			m_depthOfField.focalTransform = m_nodeToTrack;
			float num = ((!(Time.timeScale > 0f)) ? 0f : (Time.deltaTime / Time.timeScale));
			Vector3 position2 = base.transform.position;
			position2.x = position.x;
			position2.z = position.z;
			Vector3 vector = position - position2;
			m_velocity += vector * Mathf.Clamp01(m_eyeAcceleration * num);
			m_velocity -= m_velocity * Mathf.Clamp01(num * m_dampingFactor);
			position2 += m_velocity * num;
			float z = m_extendArmTransform.localPosition.z;
			z = Mathf.Lerp(z, m_distance, Mathf.Clamp01(m_headMovementInterpFactor * num));
			float y = m_extendArmTransform.localPosition.y;
			y = Mathf.Lerp(y, m_heightOffsetTarget, Mathf.Clamp01(m_headMovementInterpFactor * num));
			WingroveRoot.Instance.SetParameterGlobal(GameEvents.Parameters.CacheVal_CameraZoomLevel(), Mathf.Clamp01((z - m_distanceAudioParamMin) / (m_distanceAudioParamMax - m_distanceAudioParamMin)));
			float y2 = m_rotateXTransform.localEulerAngles.y;
			float x = m_rotateYTransform.localEulerAngles.x;
			float num2 = ((!m_worldAligned) ? m_xRotation : (m_nodeToTrack.eulerAngles.y + m_xRotation));
			if (m_extraCameraLogging)
			{
				LogRecursively(base.transform, string.Empty);
			}
			if (Mathf.Abs(num2 - y2) > 180f)
			{
				num2 = ((!(num2 > y2)) ? (num2 + 360f) : (num2 - 360f));
				y2 = Mathf.Lerp(y2, num2, Mathf.Clamp01(m_headMovementInterpFactor * num));
			}
			else
			{
				y2 = Mathf.Lerp(y2, num2, Mathf.Clamp01(m_headMovementInterpFactor * num));
			}
			float num3 = ((!m_worldAligned) ? m_yRotation : (m_nodeToTrack.eulerAngles.x + m_yRotation));
			float num4 = x;
			if (Mathf.Abs(num3 - x) > 180f)
			{
				num3 = ((!(num3 > x)) ? (num3 + 360f) : (num3 - 360f));
				x = Mathf.Lerp(x, num3, Mathf.Clamp01(m_headMovementInterpFactor * num));
			}
			else
			{
				x = Mathf.Lerp(x, num3, Mathf.Clamp01(m_headMovementInterpFactor * num));
			}
			m_rotateXTransform.localEulerAngles = new Vector3(0f, y2, 0f);
			m_rotateYTransform.localEulerAngles = new Vector3(x, 0f, 0f);
			base.transform.position = Vector3.Lerp(position2, position, m_lockToTrackAmount);
			float z2 = z;
			if (x < 20f)
			{
				if (Physics.Raycast(base.transform.position, m_extendArmTransform.forward, out var hitInfo, z + 20f, 1 << LayerMask.NameToLayer("Environment"), QueryTriggerInteraction.Ignore))
				{
					z2 = hitInfo.distance - 20f;
				}
				else
				{
					m_hitGround = false;
				}
			}
			float num5 = ((!(x < 180f)) ? 1f : Mathf.Cos(Mathf.Clamp(x, 0f, 90f) * ((float)Math.PI / 180f)));
			m_extendArmTransform.localPosition = new Vector3(0f, y * num5, z2);
			Camera[] cameras = m_cameras;
			foreach (Camera camera in cameras)
			{
				tk2dCamera component = camera.GetComponent<tk2dCamera>();
				float fieldOfView = component.CameraSettings.fieldOfView;
				fieldOfView = Mathf.Lerp(fieldOfView, m_fov, Mathf.Clamp01(m_headMovementInterpFactor * num));
				component.CameraSettings.fieldOfView = fieldOfView;
			}
		}
		if (m_animTrackCamera != null)
		{
			UpdateTrackingPosition();
			m_rumbler.SetDisabled(disabled: true);
			Vector3 position3 = m_animTrackCamera.transform.position;
			Vector3 position4 = Vector3.Lerp(m_mainCameraTransform.parent.position, position3, m_blendToAnimFactor);
			m_mainCameraTransform.position = position4;
			Quaternion rotation = m_animTrackCamera.transform.rotation;
			Camera[] cameras2 = m_cameras;
			foreach (Camera camera2 in cameras2)
			{
				tk2dCamera component2 = camera2.GetComponent<tk2dCamera>();
				component2.CameraSettings.fieldOfView = m_animTrackCamera.fieldOfView;
			}
			m_mainCameraTransform.rotation = Quaternion.Lerp(m_mainCameraTransform.parent.rotation, rotation, Mathf.Pow(m_blendToAnimFactor, 2f));
			if (!m_animationToPlay.isPlaying || ReInput.players.GetPlayer(0).GetButtonDown(17) || ReInput.players.GetPlayer(0).GetButtonDown(45) || ReInput.players.GetPlayer(0).GetButtonDown(48))
			{
				Singleton<BigTransformCoordinator>.Instance.SetCameraWarp();
				m_mainCameraTransform.localRotation = Quaternion.identity;
				m_mainCameraTransform.localPosition = Vector3.zero;
				m_animTrackCamera = null;
				m_animationToPlay = null;
				Camera[] cameras3 = m_cameras;
				foreach (Camera camera3 in cameras3)
				{
					tk2dCamera component3 = camera3.GetComponent<tk2dCamera>();
					component3.CameraSettings.fieldOfView = m_fov;
				}
			}
			if (m_blendToAnimFactor < 0f)
			{
				m_animTrackCamera = null;
				m_animationToPlay = null;
			}
		}
		else
		{
			m_mainCameraTransform.localRotation = Quaternion.identity;
			m_mainCameraTransform.localPosition = Vector3.zero;
			m_rumbler.SetDisabled(disabled: false);
		}
	}

	public float GetDofApertureAnim()
	{
		if (m_animTrackCamera != null)
		{
			return m_animTrackCamera.GetComponent<DepthOfField>().aperture;
		}
		return 0f;
	}
}
