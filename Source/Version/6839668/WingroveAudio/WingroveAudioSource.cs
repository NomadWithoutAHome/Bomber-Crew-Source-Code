using UnityEngine;

namespace WingroveAudio;

[AddComponentMenu("WingroveAudio/Wingrove Audio Source")]
public class WingroveAudioSource : BaseWingroveAudioSource
{
	[SerializeField]
	private AudioClip m_audioClip;

	public override AudioClip GetAudioClip()
	{
		return m_audioClip;
	}

	public override void RemoveUsage()
	{
	}

	public override void AddUsage()
	{
	}
}
