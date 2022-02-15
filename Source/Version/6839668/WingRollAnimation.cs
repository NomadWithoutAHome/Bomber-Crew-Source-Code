using System;
using UnityEngine;

public class WingRollAnimation : MonoBehaviour
{
	[SerializeField]
	private Animation m_animation;

	[SerializeField]
	private float m_frequencyBase;

	[SerializeField]
	private AnimationClip m_animClip;

	private float m_currentT;

	private float m_currentAmplitude;

	private float m_currentAmplitudeTarget;

	private float m_frequencyMultiplier;

	public void SetAmplitudeFrequency(float amplitude, float frequencyMultiplier)
	{
		m_currentAmplitudeTarget = amplitude;
		m_frequencyMultiplier = frequencyMultiplier;
	}

	private void Update()
	{
		float num = m_currentAmplitudeTarget - m_currentAmplitude;
		m_currentAmplitude += num * Time.deltaTime * 5f;
		m_currentT += m_frequencyBase * m_frequencyMultiplier * Time.deltaTime;
		float num2 = Mathf.Sin(m_currentT * (float)Math.PI * 2f);
		float normalizedTime = Mathf.Clamp01(0.5f + num2 * m_currentAmplitude * 0.5f);
		string animation = m_animClip.name;
		m_animation.Play(animation);
		m_animation[animation].enabled = true;
		m_animation[animation].speed = 0f;
		m_animation[animation].normalizedTime = normalizedTime;
	}
}
