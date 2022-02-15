using System;
using System.Collections.Generic;
using UnityEngine;

public class FlashManager : MonoBehaviour
{
	public class ActiveFlash
	{
		public float m_duration;

		public float m_flashFrequency;

		public float m_fadeEffect;

		public float m_phaseAmt;

		public int m_levels;

		public float m_timeRemaining;

		public float m_t;

		public Color m_maxColor;
	}

	[SerializeField]
	private Renderer[] m_toFlash;

	[SerializeField]
	private bool m_useUnscaledTime;

	[SerializeField]
	private bool m_useInheritFlash;

	[SerializeField]
	private FlashManager m_inheritFlash;

	private List<ActiveFlash> m_flashes = new List<ActiveFlash>(4);

	private Color m_currentColor;

	private float m_currentR;

	private float m_currentG;

	private float m_currentB;

	private bool m_outlineOn;

	private bool m_hasEverRun;

	private List<ActiveFlash> m_cacheToRemove = new List<ActiveFlash>(4);

	private MaterialPropertyBlock m_propertyBlock;

	private void Awake()
	{
		m_propertyBlock = new MaterialPropertyBlock();
	}

	private void Update()
	{
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		if (m_useInheritFlash)
		{
			Color currentColor = m_inheritFlash.GetCurrentColor();
			num = currentColor.r;
			num2 = currentColor.g;
			num3 = currentColor.b;
		}
		bool flag = false;
		float num4 = ((!m_useUnscaledTime) ? Time.deltaTime : Time.unscaledDeltaTime);
		List<ActiveFlash>.Enumerator enumerator = m_flashes.GetEnumerator();
		while (enumerator.MoveNext())
		{
			ActiveFlash current = enumerator.Current;
			if (current.m_duration != 0f)
			{
				current.m_timeRemaining -= num4;
				if (current.m_timeRemaining < 0f)
				{
					flag = true;
					m_cacheToRemove.Add(current);
					continue;
				}
			}
			current.m_t += num4 * (float)Math.PI * 2f;
			float num5 = Mathf.Cos(current.m_t * current.m_flashFrequency);
			float num6 = num5 / 2f + 0.5f;
			num6 = Mathf.Round(num6 * (float)current.m_levels) / (float)current.m_levels;
			float num7 = 1f;
			if (current.m_duration != 0f)
			{
				num7 = 1f - current.m_timeRemaining / current.m_duration;
			}
			float num8 = num7 * current.m_fadeEffect;
			float num9 = (1f - num8) * (num6 * current.m_phaseAmt + (1f - current.m_phaseAmt));
			num += num9 * current.m_maxColor.r;
			num2 += num9 * current.m_maxColor.g;
			num3 += num9 * current.m_maxColor.b;
		}
		if (flag)
		{
			List<ActiveFlash>.Enumerator enumerator2 = m_cacheToRemove.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				ActiveFlash current2 = enumerator2.Current;
				m_flashes.Remove(current2);
			}
			m_cacheToRemove.Clear();
		}
		Color color = new Color(num, num2, num3);
		if (num != m_currentR || num2 != m_currentG || num3 != m_currentB || !m_hasEverRun)
		{
			m_hasEverRun = true;
			m_currentColor = color;
			m_currentR = num;
			m_currentG = num2;
			m_currentB = num3;
			SetColor(color);
		}
	}

	public Color GetCurrentColor()
	{
		return m_currentColor;
	}

	protected virtual void SetColor(Color c)
	{
		m_propertyBlock.SetColor("_FlashAddition", c);
		Renderer[] toFlash = m_toFlash;
		foreach (Renderer renderer in toFlash)
		{
			renderer.SetPropertyBlock(m_propertyBlock);
		}
	}

	public ActiveFlash AddOrUpdateFlash(float duration, float frequency, float fadeAmt, int levels, float phaseAmt, Color col, ActiveFlash flashIn)
	{
		ActiveFlash activeFlash = flashIn;
		if (activeFlash == null)
		{
			activeFlash = new ActiveFlash();
		}
		activeFlash.m_duration = duration;
		activeFlash.m_flashFrequency = frequency;
		activeFlash.m_fadeEffect = fadeAmt;
		activeFlash.m_levels = levels;
		activeFlash.m_maxColor = col;
		activeFlash.m_timeRemaining = duration;
		activeFlash.m_phaseAmt = phaseAmt;
		if (frequency == 0f)
		{
			activeFlash.m_t = 0f;
		}
		if (!m_flashes.Contains(activeFlash))
		{
			activeFlash.m_t = 0f;
			m_flashes.Add(activeFlash);
		}
		return activeFlash;
	}

	public void RemoveFlash(ActiveFlash af)
	{
		m_flashes.Remove(af);
	}

	public void FadeExistingFlash(ActiveFlash af, float duration, float fadeAmt)
	{
		if (af.m_duration == 0f)
		{
			af.m_duration = duration;
			af.m_timeRemaining = duration;
			af.m_fadeEffect = fadeAmt;
		}
	}
}
