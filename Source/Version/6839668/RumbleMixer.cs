using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class RumbleMixer : Singleton<RumbleMixer>
{
	public class ActiveRumble
	{
		public Transform m_transform;

		public float m_maxDistance;

		public float m_frequency;

		public float m_amplitude;

		public bool m_smooth;

		public float m_t;

		public float m_fadeTime;

		public Vector2 UpdateAndCalculate(Vector3 fromPos)
		{
			m_t += Time.deltaTime;
			if (m_fadeTime != 0f && m_t > m_fadeTime)
			{
				return Vector2.zero;
			}
			if (m_t > 100f && m_fadeTime == 0f)
			{
				m_t -= 100f;
			}
			Vector2 result = default(Vector2);
			if (m_transform == null)
			{
				return result;
			}
			Vector3 vector = fromPos - m_transform.position;
			if (vector.magnitude < m_maxDistance && !float.IsNaN(vector.magnitude))
			{
				if (m_fadeTime != 0f)
				{
					float num = 1f - Mathf.Clamp01(m_t / m_fadeTime);
					float num2 = Mathf.Clamp01(1f - vector.magnitude / Mathf.Max(m_maxDistance, 1f)) * num;
					result.x = Mathf.Clamp(Mathf.Sin(m_t * m_frequency) + Mathf.Cos(m_t * m_frequency * 1.3f), -1f, 1f) * m_amplitude * num2;
					result.y = Mathf.Clamp(Mathf.Cos(m_t * m_frequency * 0.8f) - Mathf.Cos(m_t * m_frequency * 1.1f), -1f, 1f) * m_amplitude * num2;
				}
				else
				{
					float num3 = Mathf.Clamp01(1f - vector.magnitude / Mathf.Max(m_maxDistance, 1f));
					result.x = Mathf.Clamp(Mathf.Sin(m_t * m_frequency) + Mathf.Cos(m_t * m_frequency * 1.3f), -1f, 1f) * m_amplitude * num3;
					result.y = Mathf.Clamp(Mathf.Cos(m_t * m_frequency * 0.8f) - Mathf.Cos(m_t * m_frequency * 1.1f), -1f, 1f) * m_amplitude * num3;
				}
			}
			return result;
		}
	}

	private List<ActiveRumble> m_currentRumbles = new List<ActiveRumble>();

	private Vector2 m_currentRumble = Vector2.zero;

	private void FixedUpdate()
	{
		Vector2 vector = Vector2.zero;
		BomberState bomberState = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState();
		foreach (ActiveRumble currentRumble in m_currentRumbles)
		{
			vector += currentRumble.UpdateAndCalculate(bomberState.transform.position);
		}
		if (vector.magnitude > 100f)
		{
			vector = vector.normalized * 100f;
		}
		else if (float.IsNaN(vector.magnitude))
		{
			vector = Vector2.zero;
		}
		m_currentRumble = (vector + m_currentRumble) / 2f;
		Singleton<ControllerRumble>.Instance.SetAmbientRumble(m_currentRumble.magnitude);
	}

	public Vector2 GetCurrentRumble()
	{
		return m_currentRumble;
	}

	public void RegisterRumbler(ActiveRumble ar)
	{
		m_currentRumbles.Add(ar);
	}

	public void DeregisterRumbler(ActiveRumble ar)
	{
		m_currentRumbles.Remove(ar);
	}
}
