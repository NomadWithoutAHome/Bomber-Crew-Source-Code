using System.Collections.Generic;
using BomberCrewCommon;
using Rewired;
using UnityEngine;

public class ControllerRumble : Singleton<ControllerRumble>
{
	public enum Pitch
	{
		LowPitch,
		Combined,
		HighPitch
	}

	public class RumbleMixer
	{
		private float m_maxAmplitude;

		private float m_fadeDelay;

		private Pitch m_pitch;

		private float m_currentTimer;

		public RumbleMixer(float maxAmp, float fadeDelay, Pitch pitch)
		{
			m_maxAmplitude = maxAmp;
			m_fadeDelay = fadeDelay;
			m_currentTimer = fadeDelay;
			m_pitch = pitch;
		}

		public void GetMix(out float hi, out float low)
		{
			float num = Mathf.Clamp01(1f - m_currentTimer / m_fadeDelay);
			float num2 = num * m_maxAmplitude;
			switch (m_pitch)
			{
			case Pitch.LowPitch:
				hi = num2 * 0.25f;
				low = num2;
				break;
			case Pitch.HighPitch:
				hi = num2;
				low = num2 * 0.25f;
				break;
			case Pitch.Combined:
				hi = num2 * 0.75f;
				low = num2 * 0.75f;
				break;
			default:
				hi = 0f;
				low = 0f;
				break;
			}
		}

		public void Update(float speedMult)
		{
			m_currentTimer += Time.unscaledDeltaTime * speedMult;
		}

		public void Kick()
		{
			m_currentTimer = 0f;
		}
	}

	[SerializeField]
	private float m_platformMultiplierSwitch = 1f;

	[SerializeField]
	private float m_platformMultiplierXbox = 1f;

	[SerializeField]
	private float m_platformMultiplierPS4 = 1f;

	[SerializeField]
	private float m_switchLowFrequencyLow = 200f;

	[SerializeField]
	private float m_switchLowFrequencyHigh = 200f;

	[SerializeField]
	private float m_switchHighFrequencyLow = 1750f;

	[SerializeField]
	private float m_switchHighFrequencyHigh = 1750f;

	[SerializeField]
	private float m_ambientRumbleMax = 0.2f;

	[SerializeField]
	private float m_ambientRumbleGate = 1f;

	[SerializeField]
	private float m_ambientRumbleMultiplier = 0.05f;

	[SerializeField]
	private float m_ambientRumbleDropoff = 2f;

	[SerializeField]
	private float m_speedMultiplierSwitch = 0.5f;

	private List<RumbleMixer> m_allRumbleMixers = new List<RumbleMixer>();

	private RumbleMixer m_getHitMixer = new RumbleMixer(0.5f, 0.15f, Pitch.HighPitch);

	private RumbleMixer m_shockMixer = new RumbleMixer(0.75f, 0.25f, Pitch.Combined);

	private RumbleMixer m_stampMixer = new RumbleMixer(0.6f, 0.5f, Pitch.Combined);

	private RumbleMixer m_longShock = new RumbleMixer(0.5f, 1f, Pitch.Combined);

	private float m_ambientRumble;

	private void Start()
	{
		m_allRumbleMixers.Add(m_getHitMixer);
		m_allRumbleMixers.Add(m_shockMixer);
		m_allRumbleMixers.Add(m_stampMixer);
		m_allRumbleMixers.Add(m_longShock);
	}

	public RumbleMixer GetRumbleMixerHit()
	{
		return m_getHitMixer;
	}

	public RumbleMixer GetRumbleMixerShock()
	{
		return m_shockMixer;
	}

	public RumbleMixer GetRumbleMixerShockLong()
	{
		return m_longShock;
	}

	public RumbleMixer GetRumbleMixerStamp()
	{
		return m_stampMixer;
	}

	public void SetAmbientRumble(float amb)
	{
		m_ambientRumble += Mathf.Max(amb - m_ambientRumbleGate, 0f) * m_ambientRumbleMultiplier;
		if (m_ambientRumble > m_ambientRumbleMax)
		{
			m_ambientRumble = m_ambientRumbleMax;
		}
	}

	private void Update()
	{
		float num = 0f;
		float num2 = 0f;
		if (Time.timeScale != 0f && Singleton<SystemDataContainer>.Instance.Get().AllowVibration())
		{
			foreach (RumbleMixer allRumbleMixer in m_allRumbleMixers)
			{
				allRumbleMixer.Update(1f);
				float hi = 0f;
				float low = 0f;
				allRumbleMixer.GetMix(out hi, out low);
				num += hi;
				num2 += low;
			}
			num2 += Mathf.Clamp01(m_ambientRumble);
			m_ambientRumble -= m_ambientRumble * m_ambientRumbleDropoff * Time.deltaTime;
		}
		if (!(Singleton<UISelector>.Instance != null) || !Singleton<UISelector>.Instance.IsPrimary())
		{
			return;
		}
		Controller lastActiveController = ReInput.players.GetPlayer(0).controllers.GetLastActiveController(ControllerType.Joystick);
		if (lastActiveController == null || !(lastActiveController is Joystick))
		{
			return;
		}
		Joystick joystick = (Joystick)lastActiveController;
		if (joystick == null)
		{
			return;
		}
		int vibrationMotorCount = joystick.vibrationMotorCount;
		if (vibrationMotorCount > 0)
		{
			if (num2 == 0f && num == 0f)
			{
				joystick.StopVibration();
			}
			else
			{
				joystick.SetVibration(num2 * 0.5f, num * 0.5f);
			}
		}
	}
}
