using System;
using BomberCrewCommon;
using dbox;
using UnityEngine;

public class DboxInMissionController : Singleton<DboxInMissionController>
{
	[SerializeField]
	private BomberCamera m_camera;

	private DboxStructs.CameraFrameUpdate m_cameraFrameUpdate = default(DboxStructs.CameraFrameUpdate);

	private LandingGear.GearState m_currentGearState = LandingGear.GearState.Lowered;

	private float m_lastXRotation;

	private float m_lastYRotation;

	private bool m_hasLast;

	private bool m_wasLanded;

	private bool m_isAtStart;

	private static bool s_initOK;

	private bool m_isRunning;

	private void OnEnable()
	{
		s_initOK = true;
		try
		{
			DboxSdkWrapper.ResetStateDbox();
			DboxSdkWrapper.StartDbox();
			m_isRunning = true;
		}
		catch
		{
			Debug.LogError("[DBOX] Didn't initialise!");
			s_initOK = false;
		}
	}

	private void OnDisable()
	{
		try
		{
			if (m_isRunning)
			{
				DboxSdkWrapper.StopDbox();
			}
			m_isRunning = false;
		}
		catch
		{
		}
	}

	public static bool IsInitOK()
	{
		return s_initOK;
	}

	public static void DBoxCall(Func<float, float, int> call, Vector3 position, float intensity)
	{
		if (!IsInitOK() || Time.timeScale == 0f)
		{
			return;
		}
		try
		{
			Transform transform = Singleton<BomberSpawn>.Instance.GetBomberSystems().transform;
			Vector3 forward = transform.worldToLocalMatrix.MultiplyPoint(position);
			float num = Quaternion.LookRotation(forward, Vector3.up).eulerAngles.y - 90f;
			if (num > 180f)
			{
				num -= 360f;
			}
			if (num < -180f)
			{
				num += 360f;
			}
			num *= (float)Math.PI / 180f;
			call(num, intensity);
		}
		catch
		{
		}
	}

	public static void DBoxCall(Func<float, float, int> call, Vector3 position)
	{
		if (!IsInitOK() || Time.timeScale == 0f)
		{
			return;
		}
		try
		{
			Transform transform = Singleton<BomberSpawn>.Instance.GetBomberSystems().transform;
			Vector3 forward = transform.worldToLocalMatrix.MultiplyPoint(position);
			float num = Quaternion.LookRotation(forward).eulerAngles.y - 90f;
			if (num > 180f)
			{
				num -= 360f;
			}
			if (num < -180f)
			{
				num += 360f;
			}
			num *= (float)Math.PI / 180f;
			call(num, forward.magnitude);
		}
		catch
		{
		}
	}

	public static void DBoxCall(Func<int> call)
	{
		if (IsInitOK() && Time.timeScale != 0f)
		{
			try
			{
				call();
			}
			catch
			{
			}
		}
	}

	public void DoDamageEffect(Vector3 damagePosition, float amount, Transform bomberPosition)
	{
		DBoxCall(DboxSdkWrapper.PostTakeDamage, damagePosition, amount);
	}

	private void Update()
	{
		if (Time.deltaTime != 0f)
		{
			float xRotation = m_camera.GetXRotation();
			float yRotation = m_camera.GetYRotation();
			if (m_hasLast)
			{
				float num = xRotation - m_lastXRotation;
				if (num > 360f)
				{
					num -= 360f;
				}
				if (num < -360f)
				{
					num += 360f;
				}
				float x = (yRotation - m_lastYRotation) * ((float)Math.PI / 180f) / Time.deltaTime;
				m_cameraFrameUpdate.angularVelocity.X = x;
				m_cameraFrameUpdate.angularVelocity.Y = num * ((float)Math.PI / 180f) / Time.deltaTime;
				m_cameraFrameUpdate.angularVelocity.Z = 0f;
			}
			m_cameraFrameUpdate.distance = m_camera.GetDistance();
			m_lastXRotation = xRotation;
			m_lastYRotation = yRotation;
			m_hasLast = true;
		}
		bool flag = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState().IsLanded();
		if (m_wasLanded != flag)
		{
			if (flag)
			{
				DboxSdkWrapper.PostLand();
			}
			m_wasLanded = flag;
		}
		m_wasLanded = flag;
		if (Time.timeScale != 0f)
		{
			if (!m_isRunning && s_initOK)
			{
				try
				{
					DboxSdkWrapper.StartDbox();
					m_isRunning = true;
				}
				catch
				{
				}
			}
			DBoxCall(() => DboxSdkWrapper.PostCameraFrameUpdate(m_cameraFrameUpdate));
		}
		else if (m_isRunning && s_initOK)
		{
			try
			{
				DboxSdkWrapper.StopDbox();
				m_isRunning = false;
			}
			catch
			{
			}
		}
	}
}
