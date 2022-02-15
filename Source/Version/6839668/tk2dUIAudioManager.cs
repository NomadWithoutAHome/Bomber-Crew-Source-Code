using UnityEngine;

[AddComponentMenu("2D Toolkit/UI/Core/tk2dUIAudioManager")]
public class tk2dUIAudioManager : MonoBehaviour
{
	private static tk2dUIAudioManager instance;

	private AudioSource audioSrc;

	public static tk2dUIAudioManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = Object.FindObjectOfType(typeof(tk2dUIAudioManager)) as tk2dUIAudioManager;
				if (instance == null)
				{
					instance = new GameObject("tk2dUIAudioManager").AddComponent<tk2dUIAudioManager>();
				}
			}
			return instance;
		}
	}

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else if (instance != this)
		{
			Object.Destroy(this);
			return;
		}
		Setup();
	}

	private void Setup()
	{
		if (audioSrc == null)
		{
			audioSrc = base.gameObject.GetComponent<AudioSource>();
		}
		if (audioSrc == null)
		{
			audioSrc = base.gameObject.AddComponent<AudioSource>();
			audioSrc.playOnAwake = false;
		}
	}

	public void Play(AudioClip clip)
	{
		audioSrc.PlayOneShot(clip, AudioListener.volume);
	}
}
