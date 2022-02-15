using UnityEngine;

namespace WingroveAudio;

public class Audio3DSetting : ScriptableObject
{
	[SerializeField]
	private bool m_linearRolloff = true;

	[SerializeField]
	private float m_maxDistance = 100f;

	[SerializeField]
	private float m_minDistance = 1f;

	[SerializeField]
	private float m_dopplerMultiplier = 1f;

	[SerializeField]
	private bool m_useDynamicSpatialBlend;

	[SerializeField]
	[Range(0f, 1f)]
	private float m_blendValueNear = 1f;

	[SerializeField]
	[Range(0f, 1f)]
	private float m_blendValueFar = 1f;

	[SerializeField]
	private float m_blendNearDistance;

	[SerializeField]
	private float m_blendFarDistance = 100f;

	public float GetMaxDistance()
	{
		return m_maxDistance;
	}

	public float GetMinDistance()
	{
		return m_minDistance;
	}

	public AudioRolloffMode GetRolloffMode()
	{
		return m_linearRolloff ? AudioRolloffMode.Linear : AudioRolloffMode.Logarithmic;
	}

	public float EvaluateStandard(float distance)
	{
		return 1f - Mathf.Clamp01((distance - m_minDistance) / (m_maxDistance - m_minDistance));
	}

	public float GetSpatialBlend(float distanceSquared)
	{
		if (m_useDynamicSpatialBlend)
		{
			float num = Mathf.Sqrt(distanceSquared);
			float value = (num - m_blendNearDistance) / (m_blendFarDistance - m_blendNearDistance);
			return Mathf.Lerp(m_blendValueNear, m_blendValueFar, Mathf.Clamp01(value));
		}
		return m_blendValueNear;
	}

	public float GetDopplerLevel()
	{
		return m_dopplerMultiplier;
	}
}
