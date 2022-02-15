using BomberCrewCommon;
using UnityEngine;

public class BomberCriticalFlashAll : MonoBehaviour
{
	[SerializeField]
	private DamageFlash m_criticalDamageFlash;

	[SerializeField]
	private SmoothDamageable[] m_allDamageablesToMonitor;

	[SerializeField]
	private float m_healthLowSlowFlash;

	[SerializeField]
	private float m_healthLowFastFlash;

	[SerializeField]
	private float m_majorDamageMusicTriggerAmount;

	[SerializeField]
	private float m_criticalDamageMusicTriggerAmount;

	[SerializeField]
	private BomberDestroyableSection[] m_bomberDestroyedIfDisconnected;

	[SerializeField]
	private BomberDestroyableSection[] m_bomberDestroyedIfMultipleDisconnected;

	private bool m_isDestroyed;

	private bool m_isLanded;

	private bool m_startedMajorDamageMusic;

	private bool m_startedCriticalDamageMusic;

	private float m_timeSinceSpeechEvent;

	private bool m_isFlashing;

	private float m_lastLowest;

	private void OnSectionDestroy()
	{
		m_isDestroyed = true;
	}

	private void Start()
	{
		SmoothDamageable[] allDamageablesToMonitor = m_allDamageablesToMonitor;
		foreach (SmoothDamageable smoothDamageable in allDamageablesToMonitor)
		{
			BomberDestroyableSection bomberDestroyableSection = BomberDestroyableSection.FindDestroyableSectionFor(smoothDamageable.transform);
			bomberDestroyableSection.OnSectionDestroy += OnSectionDestroy;
		}
		m_lastLowest = 1f;
	}

	public bool IsFlashing()
	{
		return m_isFlashing;
	}

	public float GetLastLowest()
	{
		return m_lastLowest;
	}

	public void DoUpgradeFlash()
	{
		m_criticalDamageFlash.DoUpgradeFlash();
	}

	public bool IsDestroyed()
	{
		BomberDestroyableSection[] bomberDestroyedIfDisconnected = m_bomberDestroyedIfDisconnected;
		foreach (BomberDestroyableSection bomberDestroyableSection in bomberDestroyedIfDisconnected)
		{
			if (bomberDestroyableSection.IsDestroyed())
			{
				return true;
			}
		}
		int num = 0;
		BomberDestroyableSection[] bomberDestroyedIfMultipleDisconnected = m_bomberDestroyedIfMultipleDisconnected;
		foreach (BomberDestroyableSection bomberDestroyableSection2 in bomberDestroyedIfMultipleDisconnected)
		{
			if (bomberDestroyableSection2.IsDestroyed())
			{
				num++;
			}
		}
		if (num >= 2)
		{
			return true;
		}
		return false;
	}

	private void Update()
	{
		if (m_isDestroyed || m_isLanded)
		{
			m_criticalDamageFlash.ReturnToNormal();
			return;
		}
		float num = 1f;
		SmoothDamageable[] allDamageablesToMonitor = m_allDamageablesToMonitor;
		foreach (SmoothDamageable smoothDamageable in allDamageablesToMonitor)
		{
			float healthNormalised = smoothDamageable.GetHealthNormalised();
			if (healthNormalised < num)
			{
				num = healthNormalised;
			}
		}
		if (num < m_healthLowSlowFlash)
		{
			m_criticalDamageFlash.DoLowHealth(num < m_healthLowFastFlash);
			m_isFlashing = true;
			if (m_timeSinceSpeechEvent > 30f)
			{
				m_timeSinceSpeechEvent = Random.Range(-10, 0);
				Singleton<CrewMiscSpeechTriggers>.Instance.DoExternalTrigger(CrewMiscSpeechTriggers.SpeechExternalTrigger.CriticalDamageFlash, null);
			}
		}
		else if (m_isFlashing)
		{
			m_criticalDamageFlash.ReturnToNormal();
			m_isFlashing = false;
		}
		m_timeSinceSpeechEvent += Time.deltaTime;
		if (num < m_majorDamageMusicTriggerAmount && !m_startedMajorDamageMusic)
		{
			Singleton<MusicSelectionRules>.Instance.Trigger(MusicSelectionRules.MusicTriggerEvents.MajorDamage);
			m_startedMajorDamageMusic = true;
		}
		if (num < m_criticalDamageMusicTriggerAmount && !m_startedCriticalDamageMusic)
		{
			Singleton<MusicSelectionRules>.Instance.Trigger(MusicSelectionRules.MusicTriggerEvents.CriticalDamage);
			m_startedCriticalDamageMusic = true;
		}
		if (Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState().IsLanded())
		{
			m_isLanded = true;
		}
		m_lastLowest = num;
	}
}
