using UnityEngine;

public class EngineExtinguisher : MonoBehaviour
{
	[SerializeField]
	private EffectScaler m_extinguisherEffectSystem;

	[SerializeField]
	private Engine m_engine;

	[SerializeField]
	private BomberSystemUniqueId m_upgradeSystem;

	private ExtinguisherSystemUpgrade m_extinguisherUpgrade;

	private bool m_isEnabled;

	private float m_currentRate;

	private float m_timeEnabledFor;

	private void Start()
	{
		m_extinguisherUpgrade = (ExtinguisherSystemUpgrade)m_upgradeSystem.GetUpgrade();
	}

	public void SetEnabled(bool enabled)
	{
		if (CanBeActivated() && m_isEnabled != enabled)
		{
			m_isEnabled = enabled;
		}
	}

	public bool IsEnabled()
	{
		return m_isEnabled;
	}

	public bool CanBeActivated()
	{
		return m_extinguisherUpgrade != null && !m_isEnabled;
	}

	public bool IsInstalled()
	{
		return m_extinguisherUpgrade != null;
	}

	private void Update()
	{
		if (!(m_extinguisherUpgrade != null))
		{
			return;
		}
		if (m_isEnabled)
		{
			m_currentRate += Time.deltaTime;
			m_engine.PutOutFire(0.2f * Time.deltaTime);
			if (!m_engine.IsOnFire() && m_timeEnabledFor > 5f)
			{
				m_isEnabled = false;
			}
			m_timeEnabledFor += Time.deltaTime;
		}
		else
		{
			m_currentRate -= Time.deltaTime;
		}
		m_currentRate = Mathf.Clamp01(m_currentRate);
		m_extinguisherEffectSystem.SetScale(m_currentRate);
	}
}
