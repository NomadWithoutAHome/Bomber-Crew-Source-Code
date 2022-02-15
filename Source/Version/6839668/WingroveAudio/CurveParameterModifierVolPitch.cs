using UnityEngine;

namespace WingroveAudio;

[AddComponentMenu("WingroveAudio/Audio Source Modifiers/Pitch and Volume Curve")]
public class CurveParameterModifierVolPitch : ParameterModifierBase
{
	[AudioParameterName]
	public string m_parameter;

	public AnimationCurve m_volumeCurve = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));

	public AnimationCurve m_pitchCurve = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));

	[SerializeField]
	private bool m_doVolumeCurve = true;

	[SerializeField]
	private bool m_doPitchCurve = true;

	[SerializeField]
	private bool m_optimiseForGlobal;

	private int m_cachedParameter;

	private WingroveRoot.CachedParameterValue m_cachedParameterValue;

	private bool m_hasCachedGlobalVol;

	private long m_lastCachedNVol;

	private bool m_hasCachedGlobalPitch;

	private long m_lastCachedNPitch;

	private float m_lastCachedNullParamValVol;

	private float m_lastCachedNullParamValPitch;

	private float m_lastCachedVolResult = 1f;

	private float m_lastCachedPitchResult = 1f;

	private float m_cachedZeroVol;

	private float m_cachedZeroPitch;

	private void Awake()
	{
		m_cachedZeroVol = m_volumeCurve.Evaluate(0f);
		m_cachedZeroPitch = m_pitchCurve.Evaluate(0f);
	}

	public override float GetVolumeMultiplier(int linkedObjectId)
	{
		if (!m_doVolumeCurve)
		{
			return 1f;
		}
		if (m_cachedParameter == 0)
		{
			m_cachedParameter = WingroveRoot.Instance.GetParameterId(m_parameter);
		}
		if (m_cachedParameterValue == null)
		{
			m_cachedParameterValue = WingroveRoot.Instance.GetParameter(m_cachedParameter);
		}
		if (m_cachedParameterValue != null)
		{
			if (m_cachedParameterValue.m_isGlobalValue)
			{
				if (m_lastCachedNullParamValVol != m_cachedParameterValue.m_valueNull || !m_hasCachedGlobalVol)
				{
					m_lastCachedVolResult = m_volumeCurve.Evaluate(m_cachedParameterValue.m_valueNull);
					m_hasCachedGlobalVol = true;
					m_lastCachedNullParamValVol = m_cachedParameterValue.m_valueNull;
				}
				return m_lastCachedVolResult;
			}
			if (m_optimiseForGlobal)
			{
				Debug.LogError("[WINGROVEAUDIO] Parameter marked as optimise for global, but using non-global variable");
			}
			float value = 0f;
			m_cachedParameterValue.m_valueObject.TryGetValue(linkedObjectId, out value);
			return m_volumeCurve.Evaluate(value);
		}
		return m_cachedZeroVol;
	}

	public override bool IsGlobalOptimised()
	{
		return m_optimiseForGlobal;
	}

	public override float GetPitchMultiplier(int linkedObjectId)
	{
		if (!m_doPitchCurve)
		{
			return 1f;
		}
		if (m_cachedParameter == 0)
		{
			m_cachedParameter = WingroveRoot.Instance.GetParameterId(m_parameter);
		}
		if (m_cachedParameterValue == null)
		{
			m_cachedParameterValue = WingroveRoot.Instance.GetParameter(m_cachedParameter);
		}
		if (m_cachedParameterValue != null)
		{
			if (m_cachedParameterValue.m_isGlobalValue)
			{
				if (m_lastCachedNullParamValPitch != m_cachedParameterValue.m_valueNull || !m_hasCachedGlobalPitch)
				{
					m_lastCachedPitchResult = m_pitchCurve.Evaluate(m_cachedParameterValue.m_valueNull);
					m_hasCachedGlobalPitch = true;
					m_lastCachedNullParamValPitch = m_cachedParameterValue.m_valueNull;
				}
				return m_lastCachedPitchResult;
			}
			if (m_optimiseForGlobal)
			{
				Debug.LogError("[WINGROVEAUDIO] Parameter marked as optimise for global, but using non-global variable");
			}
			float value = 0f;
			m_cachedParameterValue.m_valueObject.TryGetValue(linkedObjectId, out value);
			return m_pitchCurve.Evaluate(value);
		}
		return m_cachedZeroPitch;
	}
}
