using UnityEngine;

namespace WingroveAudio;

public class ActiveCue
{
	public enum CueState
	{
		Initial,
		PlayingFadeIn,
		Playing,
		PlayingFadeOut,
		Stopped
	}

	private GameObject m_originatorSource;

	private BaseWingroveAudioSource m_audioClipSource;

	private GameObject m_targetGameObject;

	private bool m_hasTargetGameObject;

	private int m_targetGameObjectId;

	private AudioArea m_targetAudioArea;

	private double m_dspStartTime;

	private bool m_hasDSPStartTime;

	private Audio3DSetting m_audioSettings;

	private bool m_hasAudioSettings;

	private float m_fadeT;

	private float m_fadeSpeed;

	private CueState m_currentState;

	private bool m_isPaused;

	private int m_currentPosition;

	private float m_pitch;

	private WingroveRoot.AudioSourcePoolItem m_currentAudioSource;

	private bool m_currentAudioSourceExists;

	private float[] m_bufferDataL;

	private float[] m_bufferDataR;

	private float m_rms;

	private bool m_rmsRequested;

	private int m_framesAtZero;

	private Vector3 m_audioPositioning;

	private float m_audioPositioningDistanceSqr;

	private float m_theoreticalVolumeCached;

	private int m_importanceCached;

	private bool m_hasAudioArea;

	private bool m_hasCachedCurveData;

	private float m_cachedCurveParamVolume;

	private float m_cachedCurveParamPitch = 1f;

	private float m_playTimeEstimated;

	public void Initialise(GameObject originator, BaseWingroveAudioSource bas, GameObject target, AudioArea aa)
	{
		Initialise(originator, bas, target);
		m_targetAudioArea = aa;
		m_hasAudioArea = true;
	}

	public void Initialise(GameObject originator, BaseWingroveAudioSource bas, GameObject target)
	{
		m_hasAudioArea = false;
		m_hasCachedCurveData = false;
		m_rms = 0f;
		m_rmsRequested = false;
		m_framesAtZero = 0;
		m_currentState = CueState.Initial;
		m_hasDSPStartTime = false;
		m_fadeT = 0f;
		m_fadeSpeed = 0f;
		m_currentAudioSource = null;
		m_currentAudioSourceExists = false;
		m_currentPosition = 0;
		m_originatorSource = originator;
		m_targetGameObject = target;
		if (m_targetGameObject != null)
		{
			m_targetGameObjectId = target.GetInstanceID();
			m_hasTargetGameObject = true;
		}
		else
		{
			m_hasTargetGameObject = false;
			m_targetGameObjectId = 0;
		}
		m_audioClipSource = bas;
		m_pitch = m_audioClipSource.GetNewPitch();
		m_audioClipSource.AddUsage();
		m_importanceCached = m_audioClipSource.GetImportance();
		m_audioSettings = m_audioClipSource.Get3DSettings();
		if (m_audioSettings != null)
		{
			m_hasAudioSettings = true;
		}
		Update();
	}

	public GameObject GetOriginatorSource()
	{
		return m_originatorSource;
	}

	public GameObject GetTargetObject()
	{
		return m_targetGameObject;
	}

	public int GetTargetObjectId()
	{
		return m_targetGameObjectId;
	}

	public int GetImportance()
	{
		return m_importanceCached;
	}

	public void Update()
	{
		if (m_currentState == CueState.Stopped)
		{
			return;
		}
		m_theoreticalVolumeCached = GetTheoreticalVolume();
		bool flag = false;
		if (!m_currentAudioSourceExists)
		{
			if (m_theoreticalVolumeCached > 0f && GetState() != CueState.PlayingFadeOut)
			{
				m_currentAudioSource = WingroveRoot.Instance.TryClaimPoolSource(this);
				if (m_currentAudioSource != null)
				{
					m_currentAudioSourceExists = true;
					m_currentAudioSource.m_audioSource.dopplerLevel = 0f;
				}
			}
			if (m_currentAudioSourceExists)
			{
				m_currentAudioSource.m_audioSource.clip = m_audioClipSource.GetAudioClip();
				m_currentAudioSource.m_audioSource.loop = m_audioClipSource.GetLooping();
				if (!m_isPaused)
				{
					flag = true;
				}
			}
			else if (!m_isPaused)
			{
				m_currentPosition += (int)(WingroveRoot.GetDeltaTime() * (float)m_audioClipSource.GetAudioClip().frequency);
				if (m_currentPosition > m_audioClipSource.GetAudioClip().samples)
				{
					if (m_audioClipSource.GetLooping())
					{
						m_currentPosition -= m_audioClipSource.GetAudioClip().samples;
					}
					else
					{
						StopInternal();
					}
				}
			}
		}
		else if (!m_isPaused)
		{
			m_currentPosition = m_currentAudioSource.m_audioSource.timeSamples;
			m_playTimeEstimated += WingroveRoot.GetDeltaTime() * GetMixPitch();
		}
		if (!m_isPaused)
		{
			switch (m_currentState)
			{
			case CueState.Playing:
				m_fadeT = 1f;
				break;
			case CueState.PlayingFadeIn:
				m_fadeT += m_fadeSpeed * WingroveRoot.GetDeltaTime();
				if (m_fadeT >= 1f)
				{
					m_fadeT = 1f;
					m_currentState = CueState.Playing;
				}
				break;
			case CueState.PlayingFadeOut:
				m_fadeT -= m_fadeSpeed * WingroveRoot.GetDeltaTime();
				if (m_fadeT <= 0f)
				{
					m_fadeT = 0f;
					StopInternal();
					return;
				}
				break;
			}
			if (!m_audioClipSource.GetLooping())
			{
				if (m_currentPosition > m_audioClipSource.GetAudioClip().samples - 1000)
				{
					StopInternal();
					return;
				}
				if (m_currentPosition == 0)
				{
					m_framesAtZero++;
					if (m_framesAtZero == 100)
					{
						StopInternal();
						return;
					}
				}
			}
		}
		if (m_hasTargetGameObject && m_targetGameObject == null)
		{
			m_hasTargetGameObject = false;
			m_hasAudioArea = false;
		}
		SetMix();
		if (!flag || !m_currentAudioSourceExists)
		{
			return;
		}
		m_currentAudioSource.m_audioSource.timeSamples = m_currentPosition;
		m_currentAudioSource.m_audioSource.enabled = true;
		if (!m_hasAudioSettings)
		{
			if (m_currentAudioSource.m_audioSource.spatialBlend != 0f)
			{
				m_currentAudioSource.m_audioSource.spatialBlend = 0f;
			}
			if (m_currentAudioSource.m_audioSource.spatialize)
			{
				m_currentAudioSource.m_audioSource.spatialize = false;
			}
		}
		else
		{
			AudioRolloffMode rolloffMode = m_audioSettings.GetRolloffMode();
			m_currentAudioSource.m_audioSource.rolloffMode = rolloffMode;
			m_currentAudioSource.m_audioSource.minDistance = m_audioSettings.GetMinDistance();
			m_currentAudioSource.m_audioSource.maxDistance = m_audioSettings.GetMaxDistance();
			if (!m_currentAudioSource.m_audioSource.spatialize)
			{
				m_currentAudioSource.m_audioSource.spatialize = true;
			}
		}
		if (m_hasDSPStartTime && m_dspStartTime > AudioSettings.dspTime)
		{
			m_currentAudioSource.m_audioSource.timeSamples = (m_currentPosition = 0);
			m_currentAudioSource.m_audioSource.PlayScheduled(m_dspStartTime);
		}
		else
		{
			m_currentAudioSource.m_audioSource.Play();
			m_currentAudioSource.m_audioSource.timeSamples = m_currentPosition;
		}
	}

	private float GetTheoreticalVolume()
	{
		if (m_isPaused)
		{
			return 0f;
		}
		float num = 1f;
		if (m_hasAudioSettings)
		{
			float distance = Mathf.Sqrt(m_audioPositioningDistanceSqr);
			num = m_audioSettings.EvaluateStandard(distance);
		}
		return m_fadeT * m_audioClipSource.GetMixBusLevel() * num;
	}

	public float GetTheoreticalVolumeCached()
	{
		return m_theoreticalVolumeCached;
	}

	public float GetMixPitch()
	{
		return m_pitch * m_audioClipSource.GetPitchModifier(m_targetGameObjectId);
	}

	public void UpdatePosition()
	{
		if (m_hasAudioSettings && m_hasTargetGameObject)
		{
			if (!m_hasAudioArea)
			{
				m_audioPositioning = WingroveRoot.Instance.GetRelativeListeningPosition(m_targetGameObject.transform.position);
			}
			else
			{
				m_audioPositioning = WingroveRoot.Instance.GetRelativeListeningPosition(m_targetAudioArea, m_targetGameObject.transform.position);
			}
			m_audioPositioningDistanceSqr = (WingroveRoot.Instance.GetSingleListener().GetPosition() - m_audioPositioning).sqrMagnitude;
		}
	}

	public void SetMix()
	{
		UpdatePosition();
		if (m_currentAudioSourceExists)
		{
			if (!m_hasAudioSettings)
			{
				if (m_currentAudioSource.m_audioSource.spatialBlend != 0f)
				{
					m_currentAudioSource.m_audioSource.spatialBlend = 0f;
				}
				if (m_currentAudioSource.m_audioSource.spatialize)
				{
					m_currentAudioSource.m_audioSource.spatialize = false;
				}
				m_currentAudioSource.m_audioSource.transform.position = Vector3.zero;
			}
			else
			{
				float spatialBlend = m_audioSettings.GetSpatialBlend(m_audioPositioningDistanceSqr);
				if (m_currentAudioSource.m_audioSource.spatialBlend != spatialBlend)
				{
					m_currentAudioSource.m_audioSource.spatialBlend = spatialBlend;
				}
				if (!m_currentAudioSource.m_audioSource.spatialize)
				{
					m_currentAudioSource.m_audioSource.spatialize = true;
				}
				if (m_hasTargetGameObject)
				{
					m_currentAudioSource.m_audioSource.transform.position = m_audioPositioning;
				}
			}
			if (m_theoreticalVolumeCached > 0.0001f)
			{
				m_cachedCurveParamVolume = m_audioClipSource.GetVolumeModifier(m_targetGameObjectId);
				m_cachedCurveParamPitch = GetMixPitch();
				m_hasCachedCurveData = true;
			}
			float num = m_fadeT * m_audioClipSource.GetMixBusLevel() * m_cachedCurveParamVolume;
			if (num != m_currentAudioSource.m_audioSource.volume)
			{
				m_currentAudioSource.m_audioSource.volume = num;
			}
			if (m_currentAudioSource.m_audioSource.pitch != m_cachedCurveParamPitch)
			{
				m_currentAudioSource.m_audioSource.pitch = m_cachedCurveParamPitch;
			}
			float num2 = ((!m_hasAudioSettings) ? 1f : m_audioSettings.GetDopplerLevel());
			float dopplerLevel = m_currentAudioSource.m_audioSource.dopplerLevel;
			if (dopplerLevel < num2)
			{
				m_currentAudioSource.m_audioSource.dopplerLevel = Mathf.Clamp(dopplerLevel + 0.1f, 0f, num2);
			}
			else
			{
				m_currentAudioSource.m_audioSource.dopplerLevel = num2;
			}
			if (m_theoreticalVolumeCached > 0.0001f)
			{
				m_audioClipSource.UpdateFilters(m_currentAudioSource.m_pooledAudioSource, m_targetGameObjectId);
			}
			if (!WingroveRoot.Instance.ShouldCalculateMS(0))
			{
				return;
			}
			if (m_rmsRequested)
			{
				if (m_bufferDataL == null)
				{
					m_bufferDataL = new float[512];
				}
				if (m_bufferDataR == null)
				{
					m_bufferDataR = new float[512];
				}
				float num3 = 0f;
				if (m_audioClipSource.GetAudioClip().channels == 2)
				{
					m_currentAudioSource.m_audioSource.GetOutputData(m_bufferDataL, 0);
					m_currentAudioSource.m_audioSource.GetOutputData(m_bufferDataR, 1);
					for (int i = 0; i < 512; i++)
					{
						num3 += (Mathf.Abs(m_bufferDataR[i]) + Mathf.Abs(m_bufferDataL[i])) * (Mathf.Abs(m_bufferDataR[i]) + Mathf.Abs(m_bufferDataL[i]));
					}
				}
				else
				{
					m_currentAudioSource.m_audioSource.GetOutputData(m_bufferDataL, 0);
					for (int j = 0; j < 512; j++)
					{
						num3 += m_bufferDataL[j] * m_bufferDataL[j];
					}
				}
				num3 = Mathf.Sqrt(Mathf.Clamp01(num3 / 512f));
				m_rms = Mathf.Max(num3 * m_fadeT * m_audioClipSource.GetMixBusLevel(), m_rms * 0.9f);
			}
			m_rmsRequested = false;
		}
		else
		{
			m_rms = 0f;
		}
	}

	public float GetRMS()
	{
		m_rmsRequested = true;
		return m_rms;
	}

	public void Play(float fade)
	{
		m_currentPosition = 0;
		if (m_currentAudioSourceExists)
		{
			m_currentAudioSource.m_audioSource.timeSamples = 0;
		}
		if (fade == 0f)
		{
			m_currentState = CueState.Playing;
			m_fadeT = 1f;
		}
		else
		{
			m_currentState = CueState.PlayingFadeIn;
			m_fadeSpeed = 1f / fade;
		}
	}

	public void Play(float fade, double dspStartTime)
	{
		m_currentPosition = 0;
		m_hasDSPStartTime = true;
		m_dspStartTime = dspStartTime;
		if (m_currentAudioSourceExists)
		{
			m_currentAudioSource.m_audioSource.timeSamples = 0;
		}
		if (fade == 0f)
		{
			m_currentState = CueState.Playing;
			return;
		}
		m_currentState = CueState.PlayingFadeIn;
		m_fadeSpeed = 1f / fade;
	}

	public float GetTime()
	{
		return (float)m_currentPosition / ((float)m_audioClipSource.GetAudioClip().frequency * GetMixPitch());
	}

	public void SetTime(float time)
	{
		m_currentPosition = (int)(time * ((float)m_audioClipSource.GetAudioClip().frequency * GetMixPitch())) % m_audioClipSource.GetAudioClip().samples;
	}

	public float GetTimeUntilFinished(WingroveGroupInformation.HandleRepeatingAudio handleRepeat)
	{
		float num = (float)(m_audioClipSource.GetAudioClip().samples - m_currentPosition) / ((float)m_audioClipSource.GetAudioClip().frequency * GetMixPitch());
		if (m_audioClipSource.GetLooping())
		{
			switch (handleRepeat)
			{
			case WingroveGroupInformation.HandleRepeatingAudio.IgnoreRepeatingAudio:
				num = 0f;
				break;
			case WingroveGroupInformation.HandleRepeatingAudio.ReturnFloatMax:
			case WingroveGroupInformation.HandleRepeatingAudio.ReturnNegativeOne:
				num = float.MaxValue;
				break;
			}
		}
		if (m_currentState == CueState.PlayingFadeOut && m_fadeSpeed != 0f)
		{
			num = Mathf.Min(m_fadeT / m_fadeSpeed, num);
		}
		return num;
	}

	public float GetFadeT()
	{
		return m_fadeT;
	}

	public WingroveRoot.AudioSourcePoolItem GetCurrentAudioSource()
	{
		return m_currentAudioSource;
	}

	public void Stop(float fade)
	{
		if (fade == 0f)
		{
			StopInternal();
		}
		else if (m_currentState != CueState.PlayingFadeOut)
		{
			m_currentState = CueState.PlayingFadeOut;
			m_fadeSpeed = 1f / fade;
		}
		else
		{
			m_currentState = CueState.PlayingFadeOut;
			m_fadeSpeed = Mathf.Max(1f / fade, m_fadeSpeed);
		}
	}

	private void StopInternal()
	{
		Unlink();
	}

	public void Unlink()
	{
		m_currentState = CueState.Stopped;
		if (m_currentAudioSourceExists)
		{
			WingroveRoot.Instance.UnlinkSource(m_currentAudioSource, fromVirtualise: false);
			m_currentAudioSource = null;
			m_currentAudioSourceExists = false;
		}
		m_audioClipSource.RemoveUsage();
		m_audioClipSource.RePool(this);
	}

	public void Virtualise()
	{
		if (m_currentAudioSourceExists)
		{
			WingroveRoot.Instance.UnlinkSource(m_currentAudioSource, fromVirtualise: true);
			m_currentAudioSource = null;
			m_currentAudioSourceExists = false;
		}
	}

	public void Pause()
	{
		if (!m_isPaused && m_currentAudioSourceExists)
		{
			m_currentAudioSource.m_audioSource.Pause();
		}
		m_isPaused = true;
	}

	public void Unpause()
	{
		if (m_isPaused && m_currentAudioSourceExists && m_currentAudioSource != null && m_currentAudioSource.m_audioSource != null)
		{
			m_currentAudioSource.m_audioSource.Play();
		}
		m_isPaused = false;
	}

	public CueState GetState()
	{
		return m_currentState;
	}
}
