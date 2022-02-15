using System.Collections.Generic;
using UnityEngine;

public class SetSurroundSoundOptions : MonoBehaviour
{
	[SerializeField]
	private List<AudioSpeakerMode> m_preferredSpeakerModesInOrder;

	[SerializeField]
	private List<AudioSpeakerMode> m_outputModesInOrder;

	private void Start()
	{
		AudioSettings.OnAudioConfigurationChanged += OnAudioConfigChanged;
		OnAudioConfigChanged(deviceWasChanged: true);
	}

	private void OnAudioConfigChanged(bool deviceWasChanged)
	{
		if (deviceWasChanged)
		{
			AudioConfiguration configuration = AudioSettings.GetConfiguration();
			AudioSpeakerMode driverCapabilities = AudioSettings.driverCapabilities;
			AudioSpeakerMode audioSpeakerMode = AudioSpeakerMode.Stereo;
			if (m_preferredSpeakerModesInOrder.Contains(driverCapabilities))
			{
				audioSpeakerMode = m_outputModesInOrder[m_preferredSpeakerModesInOrder.IndexOf(driverCapabilities)];
			}
			if (audioSpeakerMode != configuration.speakerMode)
			{
				configuration.speakerMode = audioSpeakerMode;
				AudioSettings.Reset(configuration);
			}
		}
	}
}
