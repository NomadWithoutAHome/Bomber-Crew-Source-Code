using AudioNames;
using BomberCrewCommon;
using Common;
using Rewired;
using UnityEngine;
using WingroveAudio;

public class MissionSpeedControls : Singleton<MissionSpeedControls>
{
	[SerializeField]
	private GameObject m_enabledHierarchy;

	[SerializeField]
	private Animation m_animation;

	[SerializeField]
	private AnimationClip m_animShowFFW;

	[SerializeField]
	private AnimationClip m_animHideFFW;

	[SerializeField]
	private tk2dUIItem m_speedToggleButton;

	[SerializeField]
	private float m_distanceFromHomeAllowed;

	[SerializeField]
	private float m_minAltitude;

	[SerializeField]
	private float m_maxSlowdownTime = 10f;

	[SerializeField]
	private float m_slowdownTimeRecovery = 0.2f;

	[SerializeField]
	private float m_minSlowdownTimeToStart = 1f;

	[SerializeField]
	private ButtonStateDisplay m_buttonStateDisplay;

	[AudioEventName]
	public string m_buttonDownAudioEvent;

	[AudioEventName]
	public string m_buttonUpAudioEvent;

	private bool m_currentlyAllowed = true;

	private BomberState m_bomberState;

	private BomberSystems m_bomberSystems;

	private float m_targetSpeed;

	private float m_currentSpeed;

	private float m_cooldown;

	private bool m_blocked;

	private bool m_isDoingSlowdown;

	private float m_slowdownAmountRemaining;

	private bool m_shouldBeSlowedByControls;

	private bool m_wasSlowedByControlsThisFrame;

	private void Start()
	{
		m_bomberSystems = Singleton<BomberSpawn>.Instance.GetBomberSystems();
		m_bomberState = m_bomberSystems.GetBomberState();
		m_speedToggleButton.OnClick += ToggleFFwd;
		m_currentSpeed = 1f;
		SetSpeed(1f);
		m_slowdownAmountRemaining = m_maxSlowdownTime;
		if (GameFlow.Is30FPS())
		{
			Time.fixedDeltaTime = 1f / 30f;
		}
		else
		{
			Time.fixedDeltaTime = 1f / 60f;
		}
	}

	public void SetIsSlowedByControls()
	{
		m_shouldBeSlowedByControls = true;
	}

	public bool CanStartSlowdown()
	{
		return m_slowdownAmountRemaining > m_minSlowdownTimeToStart && !Singleton<SystemDataContainer>.Instance.Get().BlockSlowDown();
	}

	public bool CanContinueSlowDown()
	{
		return m_slowdownAmountRemaining > 0f && !Singleton<SystemDataContainer>.Instance.Get().BlockSlowDown();
	}

	public float GetSlowdownAmountNormalised()
	{
		return m_slowdownAmountRemaining / m_maxSlowdownTime;
	}

	public bool IsSlowdownActive()
	{
		return m_isDoingSlowdown || m_wasSlowedByControlsThisFrame;
	}

	public void SetBlocked(bool blocked)
	{
		m_blocked = blocked;
	}

	private void OnDestroy()
	{
		Time.timeScale = 1f;
	}

	public void SetToCurrent()
	{
		Time.timeScale = m_currentSpeed;
	}

	private void ToggleFFwd()
	{
		if (m_currentlyAllowed)
		{
			if (m_targetSpeed == 3f)
			{
				SetSpeed(1f);
			}
			else
			{
				SetSpeed(3f);
			}
		}
	}

	public void SetSpeed(float speed)
	{
		m_targetSpeed = speed;
		if (speed == 3f)
		{
			m_buttonStateDisplay.SetLockDown(lockDown: true);
		}
		else
		{
			m_buttonStateDisplay.SetLockDown(lockDown: false);
		}
	}

	public bool IsAllowed()
	{
		bool result = false;
		Vector3d position = m_bomberState.GetBigTransform().position;
		position.y = 0.0;
		if (!m_blocked && m_bomberState.IsAboveEngland() && !m_bomberState.IsLanding())
		{
			if (m_bomberState.HasNotCompletedTakeOff())
			{
				result = true;
			}
			else if (m_bomberState.GetAltitude() > m_minAltitude && !Singleton<FighterCoordinator>.Instance.AreAnyFightersEngaged())
			{
				result = true;
			}
			Station stationFor = m_bomberSystems.GetStationFor(BomberSystems.StationType.Pilot);
			if (stationFor.GetCurrentCrewman() == null)
			{
				result = false;
			}
			int num = 0;
			for (int i = 0; i < m_bomberSystems.GetEngineCount(); i++)
			{
				Engine engine = m_bomberSystems.GetEngine(i);
				if (engine != null && !engine.IsBroken() && !engine.IsDestroyed() && !engine.IsOnFire())
				{
					num++;
				}
			}
			if (num == 0)
			{
				result = false;
			}
			if (m_bomberSystems.GetFuelTank(0).GetFuelNormalised() == 0f && m_bomberSystems.GetFuelTank(1).GetFuelNormalised() == 0f)
			{
				result = false;
			}
			if (m_bomberSystems.GetCriticalFlash().IsDestroyed())
			{
				result = false;
			}
		}
		return result;
	}

	private void Update()
	{
		if (Singleton<GameFlow>.Instance.IsPaused())
		{
			return;
		}
		if (IsAllowed())
		{
			m_cooldown = 3f;
		}
		else
		{
			m_cooldown -= Time.deltaTime;
			if (m_cooldown < 0f)
			{
				m_cooldown = 0f;
			}
		}
		bool flag = m_cooldown > 0f;
		if (flag != m_currentlyAllowed)
		{
			m_currentlyAllowed = flag;
			m_enabledHierarchy.CustomActivate(m_currentlyAllowed);
			if (flag)
			{
				m_animation.Play(m_animShowFFW.name);
			}
			else
			{
				m_animation.Play(m_animHideFFW.name);
			}
			SetSpeed(Mathf.Min(1f, m_targetSpeed));
		}
		if (ReInput.players.GetPlayer(0).GetButtonDown(22) && m_currentlyAllowed)
		{
			int num = (int)m_targetSpeed + 2;
			if (num >= 4)
			{
				num = 1;
				WingroveRoot.Instance.PostEvent(m_buttonUpAudioEvent);
			}
			else
			{
				WingroveRoot.Instance.PostEvent(m_buttonDownAudioEvent);
			}
			SetSpeed(num);
		}
		if (m_isDoingSlowdown)
		{
			if (Time.timeScale != 0f)
			{
				m_slowdownAmountRemaining -= Time.deltaTime / Time.timeScale;
			}
			if (m_slowdownAmountRemaining < 0f)
			{
				m_slowdownAmountRemaining = 0f;
			}
			if (ReInput.players.GetPlayer(0).GetButton(36) && CanContinueSlowDown())
			{
				Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetMissionLog().SetUsedSlowTime();
				SetSpeed(1f);
				m_targetSpeed = 0.5f;
			}
			else
			{
				m_isDoingSlowdown = false;
				SetSpeed(1f);
			}
		}
		else
		{
			m_slowdownAmountRemaining += Time.deltaTime * m_slowdownTimeRecovery;
			if (m_slowdownAmountRemaining > m_maxSlowdownTime)
			{
				m_slowdownAmountRemaining = m_maxSlowdownTime;
			}
			if (ReInput.players.GetPlayer(0).GetButtonDown(36) && CanStartSlowdown())
			{
				m_isDoingSlowdown = true;
				SetSpeed(1f);
				m_targetSpeed = 0.5f;
			}
		}
		float num2 = m_targetSpeed;
		if (m_shouldBeSlowedByControls)
		{
			num2 = 0.25f;
		}
		float num3 = 1f;
		if (num2 <= 1f)
		{
			num3 = 2f;
		}
		bool flag2 = false;
		if (num2 > m_currentSpeed)
		{
			m_currentSpeed += Time.unscaledDeltaTime * num3;
			if (num2 < m_currentSpeed)
			{
				m_currentSpeed = num2;
			}
			flag2 = true;
		}
		else if (num2 < m_currentSpeed)
		{
			m_currentSpeed -= Time.unscaledDeltaTime * num3;
			if (num2 > m_currentSpeed)
			{
				m_currentSpeed = num2;
			}
			flag2 = true;
		}
		if (flag2)
		{
			Time.timeScale = m_currentSpeed;
			if (GameFlow.Is30FPS())
			{
				if (m_currentSpeed > 2f)
				{
					Time.fixedDeltaTime = 1f / 15f;
				}
				else if (m_currentSpeed < 0.4f)
				{
					Time.fixedDeltaTime = 1f / 120f;
				}
				else if (m_currentSpeed < 0.75f)
				{
					Time.fixedDeltaTime = 1f / 60f;
				}
				else
				{
					Time.fixedDeltaTime = 1f / 30f;
				}
			}
			else if (m_currentSpeed > 2f)
			{
				Time.fixedDeltaTime = 1f / 30f;
			}
			else if (m_currentSpeed < 0.4f)
			{
				Time.fixedDeltaTime = 0.004166667f;
			}
			else if (m_currentSpeed < 0.75f)
			{
				Time.fixedDeltaTime = 1f / 120f;
			}
			else
			{
				Time.fixedDeltaTime = 1f / 60f;
			}
		}
		WingroveRoot.Instance.SetParameterGlobal(GameEvents.Parameters.CacheVal_GameSpeedMultiplier1to3(), Mathf.Clamp01((m_currentSpeed - 1f) / 2f));
		float setValue = Mathf.Clamp01(Mathf.InverseLerp(1f, 0.25f, m_currentSpeed));
		WingroveRoot.Instance.SetParameterGlobal(GameEvents.Parameters.CacheVal_GameSpeedMultiplier1tohalf(), setValue);
		m_wasSlowedByControlsThisFrame = m_shouldBeSlowedByControls;
		m_shouldBeSlowedByControls = false;
	}
}
