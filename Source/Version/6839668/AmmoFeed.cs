using UnityEngine;

public class AmmoFeed : MonoBehaviour
{
	[SerializeField]
	private int m_ammoPerBelt = 1000;

	[SerializeField]
	private int m_maxBelts = 3;

	private int m_currentAmmo;

	private int m_numBelts;

	private bool m_isInfinite;

	private bool m_isInfiniteTutorial;

	private bool m_hasBeenSet;

	private bool m_hasAmmoFeed;

	public bool HasBeenSet()
	{
		return m_hasBeenSet;
	}

	public void SetAmmoPerBelt(int ammo, bool linkedAmmoFeed)
	{
		m_hasBeenSet = true;
		m_ammoPerBelt = ammo;
		m_currentAmmo = m_ammoPerBelt;
		m_hasAmmoFeed = linkedAmmoFeed;
	}

	public void SetInfinite(bool infinite)
	{
		m_isInfinite = infinite;
	}

	public void SetInfiniteTutorial(bool infinite)
	{
		m_isInfiniteTutorial = infinite;
	}

	public bool IsInfinite()
	{
		return m_isInfinite || m_isInfiniteTutorial;
	}

	public bool HasAmmoFeed()
	{
		return m_hasAmmoFeed;
	}

	public float GetAmmoNormalised()
	{
		float num = m_ammoPerBelt * m_maxBelts;
		float num2 = m_ammoPerBelt * m_numBelts + m_currentAmmo;
		return num2 / num;
	}

	public int GetAmmo()
	{
		if (m_isInfinite)
		{
			return 1;
		}
		return m_currentAmmo;
	}

	public int GetBelts()
	{
		if (m_isInfinite || m_hasAmmoFeed)
		{
			return 1;
		}
		return m_numBelts;
	}

	public void ReplaceBelts()
	{
		m_currentAmmo = m_ammoPerBelt;
		m_numBelts = m_maxBelts - 1;
	}

	public void ChangeBelt()
	{
		if (m_isInfinite || m_isInfiniteTutorial || m_hasAmmoFeed)
		{
			m_currentAmmo = m_ammoPerBelt;
		}
		else if (m_numBelts > 0)
		{
			m_numBelts--;
			m_currentAmmo = m_ammoPerBelt;
		}
	}

	public void UseAmmo()
	{
		if (!m_isInfinite && !m_isInfiniteTutorial)
		{
			m_currentAmmo--;
			if (m_currentAmmo < 0)
			{
				m_currentAmmo = 0;
			}
		}
	}

	private void Awake()
	{
		m_currentAmmo = m_ammoPerBelt;
		m_numBelts = m_maxBelts - 1;
	}
}
