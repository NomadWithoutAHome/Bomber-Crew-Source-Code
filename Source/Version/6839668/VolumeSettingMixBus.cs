using BomberCrewCommon;
using UnityEngine;
using WingroveAudio;

public class VolumeSettingMixBus : MonoBehaviour
{
	[SerializeField]
	private bool m_isMusicSetting;

	[SerializeField]
	private WingroveMixBus m_mixBus;

	private void Update()
	{
		if (m_isMusicSetting)
		{
			m_mixBus.Volume = Singleton<SystemDataContainer>.Instance.Get().GetMusicVolume();
		}
		else
		{
			m_mixBus.Volume = Singleton<SystemDataContainer>.Instance.Get().GetSFXVolume();
		}
	}
}
