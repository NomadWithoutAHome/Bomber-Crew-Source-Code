using System;
using BomberCrewCommon;
using Rewired;
using UnityEngine;
using UnityStandardAssets.CinematicEffects;
using UnityStandardAssets.ImageEffects;

public class BomberCamera : Singleton<BomberCamera>
{
	[Serializable]
	public class ZoomLevel
	{
		[SerializeField]
		public float m_heightOffset;

		[SerializeField]
		public float m_fov;

		[SerializeField]
		public float m_distance;

		[SerializeField]
		public float m_dofAperture;

		[SerializeField]
		public float m_audioDistance;

		[SerializeField]
		public float m_rumbleFactor;

		[SerializeField]
		public bool m_allowCameraScroll;

		[SerializeField]
		public bool m_allowCameraSideScroll;

		[SerializeField]
		public bool m_showSides;

		[SerializeField]
		public float m_lockTrackingAmount;

		[SerializeField]
		public bool m_showCrewmanOverlays;

		[SerializeField]
		public bool m_showStationOverlays;
	}

	[SerializeField]
	private TrackingCamera m_trackingCamera;

	[SerializeField]
	private Camera m_referenceCamera;

	[SerializeField]
	private float m_cameraOrbitMinMovement;

	[SerializeField]
	private float m_cameraOrbitMaxMovement;

	[SerializeField]
	private float m_yCameraMaxAngle;

	[SerializeField]
	private float m_yCameraMinAngle;

	[SerializeField]
	private float m_cameraScrollSpeedMin = 0.3f;

	[SerializeField]
	private float m_cameraScrollSpeedMax = 0.7f;

	[SerializeField]
	private float m_zoomLevelAdjustSpeed = 2f;

	[SerializeField]
	private UnityStandardAssets.ImageEffects.DepthOfField m_depthOfField;

	[SerializeField]
	private int m_targetingModeZoomIndex = 2;

	[SerializeField]
	private float m_targetingHeightOffset = 16f;

	[SerializeField]
	private float m_controllerSpeedMultiplier = 4f;

	[SerializeField]
	private float m_mouseSpeedScrollMultiplier = 100f;

	[SerializeField]
	private bool m_requireMouseButtonForRotateInTargeting;

	[SerializeField]
	private int m_maxManualZoom = 3;

	[SerializeField]
	private float m_targetingModeSensitivityMultiplier = 0.5f;

	[SerializeField]
	private float m_slowTimeEffectLerpSpeed = 2f;

	[SerializeField]
	private LensAberrations m_lensAberrations;

	[SerializeField]
	private float m_slowTimeFovBoost = 2f;

	[SerializeField]
	private float m_lensAbberationsVignetteIntensityBoost = 2f;

	[SerializeField]
	private float m_controllerSideScrollMultiplier = 1f;

	[SerializeField]
	private float m_sensitivityMultiplierSwitch = 0.5f;

	[SerializeField]
	private float m_sensitivityMultiplierXbox = 0.8f;

	[SerializeField]
	private float m_sensitivityMultiplierPS4 = 0.8f;

	[SerializeField]
	private ZoomLevel[] m_zoomLevels;

	[SerializeField]
	private Animation m_startAnim;

	[SerializeField]
	private Camera m_startAnimCamera;

	[SerializeField]
	private AnimationClip m_startAnimClip;

	[SerializeField]
	private AnimationClip m_landingAnimClip;

	[SerializeField]
	private float m_crashDisplayAngle = 40f;

	private int m_currentZoomLevel = 1;

	private float m_currentZoomLevelAdjusted = 1f;

	private bool m_lockAtMaxZoom;

	private int m_currentMaxZoomLock;

	private bool m_isScrolling;

	private float m_timeSinceScroll;

	private float m_xRotation;

	private float m_yRotation;

	private float m_lastXRotationExternal;

	private float m_lastYRotationExternal;

	private float m_lastBomberXRotation;

	private float m_sideScroll = 0.45f;

	private Vector3 m_lastMousePos;

	private float m_timeSinceZoomIn = 1f;

	private float m_timeSinceZoomOut = 1f;

	private bool m_inTargetingMode;

	private bool m_hasZoomedInEver;

	private bool m_hasZoomedOutEver;

	private float m_movedAmount;

	private int m_pauseCount;

	private bool m_hasCrashed;

	private float m_slowTimeEffectAmount;

	private float m_defaultLensAbVIntensity;

	private int m_preToggledZoomLevel;

	private float m_zoomDirection;

	private bool m_hasZoomedRecently;

	private Quaternion m_lastPS4Orientation;

	private bool m_hasLastFramePS4Orientation;

	public void Pause()
	{
		m_pauseCount++;
	}

	public void Resume()
	{
		m_pauseCount--;
	}

	public float DeadZone(float input, float deadzone)
	{
		if (input == 0f)
		{
			return 0f;
		}
		if (deadzone == 1f)
		{
			return input;
		}
		return Mathf.Sign(input) * Mathf.Clamp01((Mathf.Abs(input) - deadzone) / (1f - deadzone));
	}

	public Vector2 DoGyroAim(Quaternion newQuat, Vector2 cameraMovementAxis, bool flipX)
	{
		if (m_hasLastFramePS4Orientation)
		{
			Vector3 eulerAngles = (newQuat * Quaternion.Inverse(m_lastPS4Orientation)).eulerAngles;
			float num = eulerAngles.y;
			if (num > 180f)
			{
				num = 0f - (360f - num);
			}
			else if (num < -180f)
			{
				num = 0f - (-360f + num);
			}
			float num2 = eulerAngles.x;
			if (num2 > 180f)
			{
				num2 = 0f - (360f - num2);
			}
			else if (num < -180f)
			{
				num2 = 0f - (-360f + num2);
			}
			cameraMovementAxis.x += num * 0.35f * (float)((!flipX) ? 1 : (-1));
			cameraMovementAxis.y -= num2 * 0.35f * (float)((!Singleton<SystemDataContainer>.Instance.Get().GetInvertYMotionAiming()) ? 1 : (-1));
		}
		m_lastPS4Orientation = newQuat;
		return cameraMovementAxis;
	}

	public Vector2 DeadZone(Vector2 input, float deadzone)
	{
		float magnitude = input.magnitude;
		if (magnitude == 0f)
		{
			return input;
		}
		return input.normalized * Mathf.Clamp01((magnitude - deadzone) / (1f - deadzone));
	}

	public Camera GetCamera()
	{
		return m_referenceCamera;
	}

	public Transform GetMoveableNode()
	{
		return m_trackingCamera.GetMainCameraNode();
	}

	public float GetDistance()
	{
		return m_trackingCamera.GetDistance();
	}

	public void SetZoomDirection(float amt)
	{
		m_zoomDirection = amt;
	}

	public void DoCameraAnimation(Animation an, Camera toTrack, AnimationClip clip, bool trackXZ, bool trackY)
	{
		an.transform.position = Vector3.zero;
		an.Play(clip.name);
		an.Sample();
		m_trackingCamera.DoCameraTrackAnim(toTrack, an, trackXZ, trackY);
	}

	public void SetHasCrashed()
	{
		m_hasCrashed = true;
	}

	private void Awake()
	{
		if (Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetCurrentlySelectedMissionDetails().m_missionReferenceName != "FIRST_MISSION")
		{
			DoCameraAnimation(m_startAnim, m_startAnimCamera, m_startAnimClip, trackXZ: true, trackY: true);
		}
		m_defaultLensAbVIntensity = m_lensAberrations.vignette.intensity;
	}

	public void DoLandingAnimation()
	{
		DoCameraAnimation(m_startAnim, m_startAnimCamera, m_landingAnimClip, trackXZ: true, trackY: true);
	}

	public void ToggleZoomInternalIfNotInternal(Vector3 worldPos)
	{
		if (!m_inTargetingMode)
		{
			if (m_currentZoomLevel > 1)
			{
				m_preToggledZoomLevel = m_currentZoomLevel;
				m_currentZoomLevel = 1;
			}
			DoCentralisingActions(worldPos);
		}
	}

	public void DoCentralisingActions(Vector3 worldPos)
	{
		if (m_currentZoomLevel == 1)
		{
			ReCentre();
		}
		else
		{
			float num = (m_sideScroll = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetCameraTrackingNode().GetPos(worldPos));
		}
	}

	public float GetXRotation()
	{
		return m_xRotation;
	}

	public float GetYRotation()
	{
		return m_yRotation;
	}

	public bool ToggleZoomWhileInteracting(float zoomDir, bool zoomButton, Vector3 worldPos)
	{
		if ((zoomDir < -0.5f || zoomButton) && m_currentZoomLevel == 1)
		{
			m_currentZoomLevel = 0;
			DoCentralisingActions(worldPos);
			return true;
		}
		if ((zoomDir > 0.5f || zoomButton) && m_currentZoomLevel == 0)
		{
			m_currentZoomLevel = 1;
			ReCentre();
			return true;
		}
		return false;
	}

	public void NudgeScroll(float amt)
	{
		m_sideScroll += amt;
		m_sideScroll = Mathf.Clamp01(m_sideScroll);
	}

	public void ToggleZoomInternal()
	{
		if (m_inTargetingMode)
		{
			return;
		}
		if (m_currentZoomLevel != 1)
		{
			if (m_currentZoomLevel > 1)
			{
				m_preToggledZoomLevel = m_currentZoomLevel;
			}
			m_currentZoomLevel = 1;
			m_hasZoomedInEver = true;
			ReCentre();
		}
		else
		{
			m_hasZoomedOutEver = true;
			if (m_preToggledZoomLevel != 0)
			{
				m_currentZoomLevel = m_preToggledZoomLevel;
			}
			else
			{
				m_currentZoomLevel = 3;
			}
		}
	}

	private void Update()
	{
		float num = 0.1f;
		float num2 = 1f;
		float sensitivityCamera = Singleton<SystemDataContainer>.Instance.Get().GetSensitivityCamera();
		float num3 = Singleton<SystemDataContainer>.Instance.Get().GetSensitivityTargeting() * m_targetingModeSensitivityMultiplier;
		int insideBomberHorizontalMultiplier = Singleton<SystemDataContainer>.Instance.Get().GetInsideBomberHorizontalMultiplier();
		float num4 = ((!(Time.timeScale > 0f)) ? 0f : (Time.deltaTime / Time.timeScale));
		bool flag = m_pauseCount > 0;
		Vector2 vector = DeadZone(ReInput.players.GetPlayer(0).GetAxis2D(6, 7), 0.2f);
		bool hasLastFramePS4Orientation = false;
		if (m_inTargetingMode)
		{
			vector.y += ReInput.players.GetPlayer(0).GetAxis(35);
		}
		m_hasLastFramePS4Orientation = hasLastFramePS4Orientation;
		vector.y *= ((!Singleton<SystemDataContainer>.Instance.Get().GetInvert()) ? 1 : (-1));
		float num5 = ReInput.players.GetPlayer(0).GetAxis(8) + m_zoomDirection;
		if (ReInput.players.GetPlayer(0).GetButtonDown(45))
		{
			ToggleZoomInternal();
		}
		if (!m_inTargetingMode && !flag)
		{
			if (num5 > 0.1f && m_timeSinceZoomIn > 0.3f)
			{
				m_hasZoomedRecently = true;
				m_currentZoomLevel--;
				m_hasZoomedInEver = true;
				if (m_currentZoomLevel < 0)
				{
					m_currentZoomLevel = 0;
				}
				m_timeSinceZoomIn = 0f;
				m_timeSinceZoomOut = 0.5f;
			}
			if (num5 < -0.1f && m_timeSinceZoomOut > 0.3f)
			{
				m_hasZoomedRecently = true;
				m_currentZoomLevel++;
				m_hasZoomedOutEver = true;
				if (m_currentZoomLevel > m_maxManualZoom)
				{
					m_currentZoomLevel = m_maxManualZoom;
				}
				if (m_currentZoomLevel == m_zoomLevels.Length)
				{
					m_currentZoomLevel = m_zoomLevels.Length - 1;
				}
				m_timeSinceZoomOut = 0f;
				m_timeSinceZoomIn = 0.5f;
			}
			m_timeSinceZoomIn += num4;
			m_timeSinceZoomOut += num4;
		}
		int zoomLevel = GetZoomLevel();
		m_trackingCamera.Set(Singleton<BomberSpawn>.Instance.GetBomberSystems().GetCameraTrackingNode().transform, m_zoomLevels[zoomLevel].m_distance, !m_zoomLevels[zoomLevel].m_allowCameraScroll);
		m_trackingCamera.SetFov(m_zoomLevels[zoomLevel].m_fov + m_slowTimeFovBoost * m_slowTimeEffectAmount);
		m_trackingCamera.SetHeightOffset((!m_inTargetingMode) ? m_zoomLevels[zoomLevel].m_heightOffset : m_targetingHeightOffset);
		m_trackingCamera.SetLockToTrackingAmount(m_zoomLevels[zoomLevel].m_lockTrackingAmount);
		if (!m_zoomLevels[zoomLevel].m_allowCameraScroll && !flag)
		{
			m_trackingCamera.UpdateAngle(0f, 0f);
			m_xRotation = m_trackingCamera.GetXRotation();
			m_yRotation = m_trackingCamera.GetYRotation();
			Singleton<BomberSpawn>.Instance.GetBomberSystems().GetCameraTrackingNode().SetT(m_sideScroll);
			float num6 = Mathf.Lerp(m_cameraScrollSpeedMin, m_cameraScrollSpeedMax, sensitivityCamera);
			m_sideScroll += num4 * num6 * vector.x * m_controllerSpeedMultiplier * (float)insideBomberHorizontalMultiplier * m_controllerSideScrollMultiplier * num2;
			m_movedAmount += Mathf.Abs(num4 * num6 * vector.x * m_controllerSpeedMultiplier);
			if (ReInput.players.GetPlayer(0).GetButton(33) || Input.GetMouseButton(1))
			{
				Vector2 vector2 = Input.mousePosition - m_lastMousePos;
				m_lastMousePos = Input.mousePosition;
				Vector2 vector3 = new Vector2(vector2.x / (float)m_referenceCamera.pixelWidth, vector2.y / (float)m_referenceCamera.pixelHeight);
				m_sideScroll += num6 * vector3.x * m_mouseSpeedScrollMultiplier * (float)insideBomberHorizontalMultiplier;
				m_movedAmount += Mathf.Abs(num6 * vector3.x * m_mouseSpeedScrollMultiplier);
			}
			m_sideScroll = Mathf.Clamp01(m_sideScroll);
		}
		else if (!flag)
		{
			m_xRotation = m_lastXRotationExternal;
			m_yRotation = m_lastYRotationExternal;
			Singleton<BomberSpawn>.Instance.GetBomberSystems().GetCameraTrackingNode().SetT(0.5f);
			m_trackingCamera.UpdateAngle(m_xRotation, m_yRotation);
			if (!m_hasCrashed)
			{
				bool flag2 = ReInput.players.GetPlayer(0).GetButton(33) || Input.GetMouseButton(1) || (!m_requireMouseButtonForRotateInTargeting && m_inTargetingMode);
				float num7 = Mathf.Lerp(m_cameraOrbitMinMovement, m_cameraOrbitMaxMovement, (!m_inTargetingMode) ? sensitivityCamera : num3);
				if (flag2)
				{
					Vector2 vector4 = new Vector2(Input.GetAxis("Mouse X") * 40f, Input.GetAxis("Mouse Y") * 40f);
					float num8 = ReInput.controllers.Mouse.GetAxis(0) * 4f;
					float num9 = ReInput.controllers.Mouse.GetAxis(1) * 4f;
					if (Mathf.Abs(num8) > Mathf.Abs(vector4.x))
					{
						vector4.x = num8;
					}
					if (Mathf.Abs(num9) > Mathf.Abs(vector4.y))
					{
						vector4.y = num9;
					}
					Vector2 vector5 = new Vector2(vector4.x / (float)Screen.width, vector4.y / (float)Screen.height);
					vector5.y *= ((!Singleton<SystemDataContainer>.Instance.Get().GetInvert()) ? 1 : (-1));
					m_xRotation += vector5.x * num7 * m_mouseSpeedScrollMultiplier;
					m_yRotation -= vector5.y * num7 * m_mouseSpeedScrollMultiplier;
					m_movedAmount += Mathf.Abs(vector5.x * num7 * m_mouseSpeedScrollMultiplier);
					m_movedAmount += Mathf.Abs(vector5.y * num7 * m_mouseSpeedScrollMultiplier);
				}
				m_xRotation += num7 * num4 * vector.x * m_controllerSpeedMultiplier * num2;
				m_yRotation -= num7 * num4 * vector.y * m_controllerSpeedMultiplier * num2;
				m_movedAmount += Mathf.Abs(num7 * num4 * vector.x * m_controllerSpeedMultiplier);
				m_movedAmount += Mathf.Abs(num7 * num4 * vector.y * m_controllerSpeedMultiplier);
			}
			else
			{
				float num10 = m_crashDisplayAngle - m_yRotation;
				m_yRotation += num10 * Mathf.Clamp01(num4 * 5f);
			}
			m_yRotation = Mathf.Clamp(m_yRotation, m_yCameraMinAngle, m_yCameraMaxAngle);
			if (Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState().GetAltitudeAboveGround() < 20f)
			{
				m_yRotation = Mathf.Clamp(m_yRotation, 0f, m_yCameraMaxAngle);
			}
			if (m_trackingCamera.HitGround())
			{
				m_yRotation = Mathf.Max(m_yRotation, m_trackingCamera.GetHitAtPoint());
			}
		}
		if (m_xRotation > 180f)
		{
			m_xRotation -= 360f;
		}
		else if (m_xRotation < -180f)
		{
			m_xRotation += 360f;
		}
		if (m_zoomLevels[zoomLevel].m_allowCameraScroll)
		{
			m_lastXRotationExternal = m_xRotation;
			m_lastYRotationExternal = m_yRotation;
		}
		else
		{
			float y = Singleton<BomberSpawn>.Instance.GetBomberSystems().transform.rotation.eulerAngles.y;
			float num11 = y - m_lastBomberXRotation;
			m_lastXRotationExternal += num11;
		}
		m_currentZoomLevelAdjusted += ((float)zoomLevel - m_currentZoomLevelAdjusted) * num4 * m_zoomLevelAdjustSpeed;
		m_currentZoomLevelAdjusted = Mathf.Clamp(m_currentZoomLevelAdjusted, 0f, m_zoomLevels.Length - 1);
		m_lastBomberXRotation = Singleton<BomberSpawn>.Instance.GetBomberSystems().transform.rotation.eulerAngles.y;
		if (m_depthOfField != null)
		{
			m_depthOfField.aperture = GetDofAperture();
		}
		if (Singleton<MissionSpeedControls>.Instance.IsSlowdownActive())
		{
			m_slowTimeEffectAmount += Time.unscaledDeltaTime * m_slowTimeEffectLerpSpeed;
		}
		else
		{
			m_slowTimeEffectAmount -= Time.unscaledDeltaTime * m_slowTimeEffectLerpSpeed;
		}
		m_slowTimeEffectAmount = Mathf.Clamp01(m_slowTimeEffectAmount);
		m_lensAberrations.vignette.intensity = m_defaultLensAbVIntensity + m_slowTimeEffectAmount * m_lensAbberationsVignetteIntensityBoost;
		m_lastMousePos = Input.mousePosition;
	}

	public bool HasZoomedEver()
	{
		return m_hasZoomedInEver && m_hasZoomedOutEver;
	}

	public bool HasMovedEver()
	{
		return m_movedAmount > 2f;
	}

	public void ResetZoomedRecently()
	{
		m_hasZoomedRecently = false;
	}

	public bool HasZoomedRecently()
	{
		return m_hasZoomedRecently;
	}

	public int GetZoomLevel()
	{
		int result = ((!m_inTargetingMode) ? m_currentZoomLevel : m_targetingModeZoomIndex);
		if (m_hasCrashed)
		{
			result = 3;
		}
		else if (m_lockAtMaxZoom && !m_inTargetingMode)
		{
			result = Mathf.Min(m_currentZoomLevel, m_currentMaxZoomLock);
		}
		return result;
	}

	public int GetZoomLevelForUIWidget()
	{
		int result = m_currentZoomLevel;
		if (m_hasCrashed)
		{
			result = 3;
		}
		else if (m_lockAtMaxZoom && !m_inTargetingMode)
		{
			result = Mathf.Min(m_currentZoomLevel, m_currentMaxZoomLock);
		}
		return result;
	}

	public void SetLockedAtZoomMax(int max)
	{
		m_currentMaxZoomLock = max;
		m_lockAtMaxZoom = true;
	}

	public void ReCentre()
	{
		m_sideScroll = 0.45f;
	}

	public void SetNotLockedAtMax()
	{
		m_lockAtMaxZoom = false;
	}

	public bool ShouldShowSides()
	{
		return m_zoomLevels[GetZoomLevel()].m_showSides || m_trackingCamera.IsDoingCameraAnim();
	}

	public bool ShouldShowCrewmenOverlays()
	{
		return m_zoomLevels[GetZoomLevel()].m_showCrewmanOverlays && !m_trackingCamera.IsDoingCameraAnim();
	}

	public bool ShouldShowStationOverlays()
	{
		return m_zoomLevels[GetZoomLevel()].m_showStationOverlays && !m_trackingCamera.IsDoingCameraAnim();
	}

	public float GetRumbleFactor()
	{
		int num = Mathf.FloorToInt(m_currentZoomLevelAdjusted);
		int num2 = Mathf.Clamp(num + 1, 0, m_zoomLevels.Length - 1);
		float t = m_currentZoomLevelAdjusted - (float)num;
		return Mathf.Lerp(m_zoomLevels[num].m_rumbleFactor, m_zoomLevels[num2].m_rumbleFactor, t);
	}

	private float GetDofAperture()
	{
		if (m_trackingCamera.IsDoingCameraAnim())
		{
			return m_trackingCamera.GetDofApertureAnim();
		}
		int num = Mathf.FloorToInt(m_currentZoomLevelAdjusted);
		int num2 = Mathf.Clamp(num + 1, 0, m_zoomLevels.Length - 1);
		float t = m_currentZoomLevelAdjusted - (float)num;
		return Mathf.Lerp(m_zoomLevels[num].m_dofAperture, m_zoomLevels[num2].m_dofAperture, t);
	}

	public void SetTargetingMode(bool targetingMode)
	{
		m_inTargetingMode = targetingMode;
		if (!Singleton<UISelector>.Instance.IsPrimary())
		{
			if (targetingMode && !m_requireMouseButtonForRotateInTargeting)
			{
				Cursor.visible = false;
				Cursor.lockState = CursorLockMode.Locked;
			}
			else
			{
				Cursor.visible = true;
				Cursor.lockState = CursorLockMode.None;
			}
		}
	}
}
