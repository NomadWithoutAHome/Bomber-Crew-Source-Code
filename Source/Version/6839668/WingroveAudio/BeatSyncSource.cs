using System.Collections.Generic;
using UnityEngine;

namespace WingroveAudio;

[RequireComponent(typeof(BaseWingroveAudioSource))]
public class BeatSyncSource : MonoBehaviour
{
	[SerializeField]
	private float m_bpm = 120f;

	private BaseWingroveAudioSource m_audioSource;

	private static List<BeatSyncSource> m_beatSyncs = new List<BeatSyncSource>();

	private void Start()
	{
		m_audioSource = GetComponent<BaseWingroveAudioSource>();
		m_beatSyncs.Add(this);
	}

	public static BeatSyncSource GetCurrent()
	{
		foreach (BeatSyncSource beatSync in m_beatSyncs)
		{
			if (beatSync.IsActive())
			{
				return beatSync;
			}
		}
		return null;
	}

	private void OnDestroy()
	{
		m_beatSyncs.Remove(this);
	}

	public bool IsActive()
	{
		return m_audioSource.IsPlaying();
	}

	public double GetNextBeatTime()
	{
		float currentTime = m_audioSource.GetCurrentTime();
		double dspTime = AudioSettings.dspTime;
		int num = Mathf.CeilToInt(currentTime / (60f / m_bpm));
		float num2 = (float)num * (60f / m_bpm);
		float num3 = num2 - currentTime;
		return dspTime + (double)num3;
	}
}
