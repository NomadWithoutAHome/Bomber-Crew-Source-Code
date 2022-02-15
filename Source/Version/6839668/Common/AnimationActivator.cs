using System;
using System.Collections;
using UnityEngine;
using WingroveAudio;

namespace Common;

[RequireComponent(typeof(Animation))]
public class AnimationActivator : CustomActivator
{
	[Serializable]
	private class MinMax
	{
		public bool Enabled;

		public float Min = 0.9f;

		public float Max = 1.1f;
	}

	[SerializeField]
	private bool m_activateOnEnable;

	[SerializeField]
	private float m_defaultDelay;

	[SerializeField]
	private float m_crossfadeTime;

	[SerializeField]
	private MinMax m_randomAnimSpeed;

	[SerializeField]
	private Animation m_animation;

	[SerializeField]
	private AnimationClip m_animClipShow;

	[SerializeField]
	private AnimationClip m_animClipIdle;

	[SerializeField]
	private AnimationClip m_animClipHide;

	private AnimationClip m_currentAnimationClip;

	[SerializeField]
	private string m_activateAudioEvent;

	[SerializeField]
	private string m_hideAudioEvent;

	private Collider[] m_colliders;

	private float m_delay;

	private bool m_boolValue;

	private bool m_enableColliders;

	[AudioEventName]
	private string m_audioEventValue = string.Empty;

	private void OnEnable()
	{
		if (m_animation == null)
		{
			m_animation = base.gameObject.GetComponent<Animation>();
		}
		GetColliders();
		if (m_activateOnEnable)
		{
			Show(null, null);
		}
	}

	public override void Show(float? delay, ActivatorCallback callback)
	{
		if (m_animClipShow != null)
		{
			if (base.gameObject.activeInHierarchy)
			{
				StopAllCoroutines();
				if (delay.HasValue)
				{
					m_delay = delay.Value;
				}
				else
				{
					m_delay = m_defaultDelay;
				}
				m_currentAnimationClip = m_animClipShow;
				m_boolValue = true;
				m_enableColliders = true;
				m_audioEventValue = m_activateAudioEvent;
				if (base.gameObject.activeInHierarchy)
				{
					StartCoroutine(SetBoolWithDelay(callback));
				}
			}
		}
		else
		{
			callback?.Invoke();
			base.gameObject.SetActive(value: true);
		}
	}

	public override void Hide(ActivatorCallback callback)
	{
		if (m_animClipHide != null)
		{
			if (!m_animation.IsPlaying(m_animClipHide.name) && base.gameObject.activeInHierarchy)
			{
				StopAllCoroutines();
				m_currentAnimationClip = m_animClipHide;
				m_delay = 0f;
				m_boolValue = false;
				m_enableColliders = false;
				m_audioEventValue = m_hideAudioEvent;
				StartCoroutine(SetBoolWithDelay(callback));
			}
		}
		else
		{
			callback?.Invoke();
			base.gameObject.SetActive(value: false);
		}
	}

	public override bool IsActive()
	{
		return m_active;
	}

	private IEnumerator SetBoolWithDelay(ActivatorCallback callback = null)
	{
		if (m_delay > 0f)
		{
			if (m_currentAnimationClip == m_animClipShow)
			{
				m_animation.clip = m_animClipShow;
				m_animation.Rewind();
				m_animation.Play();
				m_animation[m_animClipShow.name].speed = 0f;
			}
			yield return new WaitForSeconds(m_delay);
		}
		if (!m_randomAnimSpeed.Enabled)
		{
			m_animation[m_currentAnimationClip.name].speed = 1f;
		}
		else
		{
			m_animation[m_currentAnimationClip.name].speed = UnityEngine.Random.Range(m_randomAnimSpeed.Min, m_randomAnimSpeed.Max);
		}
		if (!string.IsNullOrEmpty(m_audioEventValue))
		{
			WingroveRoot.Instance.PostEvent(m_audioEventValue);
		}
		m_animation.Stop();
		m_animation.clip = m_currentAnimationClip;
		m_animation.Rewind();
		m_animation.Play();
		m_animation.Sample();
		if (!m_boolValue)
		{
			StartCoroutine(DeactivateWhenHidden(callback));
		}
		else
		{
			StartCoroutine(CallbackWhenShown(callback));
		}
		if ((bool)(m_currentAnimationClip = m_animClipShow) && m_animClipIdle != null)
		{
			if (m_crossfadeTime > 0f)
			{
				m_animation.CrossFadeQueued(m_animClipIdle.name, m_crossfadeTime);
			}
			else
			{
				m_animation.PlayQueued(m_animClipIdle.name);
			}
		}
		m_active = m_boolValue;
		EnableColliders(m_enableColliders);
	}

	private IEnumerator CallbackWhenShown(ActivatorCallback callback = null)
	{
		while (m_animation.IsPlaying(m_animClipShow.name))
		{
			yield return null;
		}
		callback?.Invoke();
	}

	private IEnumerator DeactivateWhenHidden(ActivatorCallback callback = null)
	{
		while (m_animation.IsPlaying(m_animClipHide.name))
		{
			yield return null;
		}
		callback?.Invoke();
		base.gameObject.SetActive(value: false);
	}

	private void GetColliders()
	{
		m_colliders = base.gameObject.GetComponentsInChildren<Collider>(includeInactive: true);
	}

	private void EnableColliders(bool enable)
	{
		if (m_colliders == null)
		{
			return;
		}
		Collider[] colliders = m_colliders;
		foreach (Collider collider in colliders)
		{
			if (collider != null)
			{
				collider.enabled = enable;
			}
		}
	}
}
