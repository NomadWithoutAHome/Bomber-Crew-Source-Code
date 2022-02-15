using UnityEngine;

namespace WingroveAudio;

[AddComponentMenu("WingroveAudio/Wingrove Group Information")]
public class WingroveGroupInformation : MonoBehaviour
{
	public enum HandleRepeatingAudio
	{
		IgnoreRepeatingAudio,
		GiveTimeUntilLoop,
		ReturnFloatMax,
		ReturnNegativeOne
	}

	private BaseWingroveAudioSource[] m_audioSources;

	[SerializeField]
	private HandleRepeatingAudio m_handleRepeatedAudio = HandleRepeatingAudio.ReturnFloatMax;

	private void Awake()
	{
		m_audioSources = GetComponentsInChildren<BaseWingroveAudioSource>();
	}

	public float GetTimeUntilFinished()
	{
		float num = 0f;
		if (m_audioSources != null)
		{
			BaseWingroveAudioSource[] audioSources = m_audioSources;
			foreach (BaseWingroveAudioSource baseWingroveAudioSource in audioSources)
			{
				num = Mathf.Max(baseWingroveAudioSource.GetTimeUntilFinished(m_handleRepeatedAudio), num);
			}
		}
		if (num == float.MaxValue && m_handleRepeatedAudio == HandleRepeatingAudio.ReturnNegativeOne)
		{
			num = -1f;
		}
		return num;
	}

	public bool IsAnyPlaying()
	{
		if (m_audioSources != null)
		{
			BaseWingroveAudioSource[] audioSources = m_audioSources;
			foreach (BaseWingroveAudioSource baseWingroveAudioSource in audioSources)
			{
				if (baseWingroveAudioSource.IsPlaying())
				{
					return true;
				}
			}
		}
		return false;
	}
}
