using BomberCrewCommon;
using UnityEngine;

public class Rumbler : MonoBehaviour
{
	[SerializeField]
	private float m_maxDistance;

	[SerializeField]
	private float m_rumbleAmount;

	[SerializeField]
	private float m_rumbleFrequency;

	[SerializeField]
	private float m_lifetime;

	private RumbleMixer.ActiveRumble m_rumble;

	private void OnEnable()
	{
		m_rumble = new RumbleMixer.ActiveRumble();
		m_rumble.m_amplitude = m_rumbleAmount;
		m_rumble.m_frequency = m_rumbleFrequency;
		m_rumble.m_maxDistance = m_maxDistance;
		m_rumble.m_t = 0f;
		m_rumble.m_transform = base.transform;
		m_rumble.m_fadeTime = m_lifetime;
		if (Singleton<RumbleMixer>.Instance != null)
		{
			Singleton<RumbleMixer>.Instance.RegisterRumbler(m_rumble);
		}
	}

	private void OnDisable()
	{
		if (Singleton<RumbleMixer>.Instance != null)
		{
			Singleton<RumbleMixer>.Instance.DeregisterRumbler(m_rumble);
		}
	}

	public void SetExternalScale(float scale)
	{
		if (m_rumble != null)
		{
			m_rumble.m_amplitude = m_rumbleAmount * scale;
		}
	}
}
