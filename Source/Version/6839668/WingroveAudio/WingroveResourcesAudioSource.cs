using UnityEngine;
using UnityEngine.SceneManagement;

namespace WingroveAudio;

[AddComponentMenu("WingroveAudio/Wingrove Resources Audio Source")]
public class WingroveResourcesAudioSource : BaseWingroveAudioSource
{
	[SerializeField]
	private string m_audioClipResourceName = string.Empty;

	[SerializeField]
	private float m_holdResource;

	[SerializeField]
	private bool m_onlyUnloadOnLevelChange;

	[SerializeField]
	private bool m_useStreamLoader;

	private AudioClip m_audioClip;

	private int m_users;

	private float m_timer;

	private void Update()
	{
		if (m_users == 0)
		{
			m_timer -= WingroveRoot.GetDeltaTime();
			if (m_timer < 0f && !m_onlyUnloadOnLevelChange)
			{
				Unload();
			}
		}
		UpdateInternal();
	}

	public override AudioClip GetAudioClip()
	{
		return m_audioClip;
	}

	private void OnDestroy()
	{
		Unload();
	}

	private void Load()
	{
		if (m_audioClip == null)
		{
			if (m_useStreamLoader)
			{
				GameObject gameObject = (GameObject)Resources.Load(m_audioClipResourceName + "_SL");
				StreamLoader component = gameObject.GetComponent<StreamLoader>();
				m_audioClip = component.m_referencedAudioClip;
			}
			else
			{
				m_audioClip = (AudioClip)Resources.Load(m_audioClipResourceName);
			}
			SceneManager.sceneLoaded += UnloadOnLevelChange;
		}
	}

	private void Unload()
	{
		if (m_audioClip != null)
		{
			Resources.UnloadAsset(m_audioClip);
			m_audioClip = null;
			SceneManager.sceneLoaded -= UnloadOnLevelChange;
		}
	}

	private void UnloadOnLevelChange(Scene scene, LoadSceneMode lsm)
	{
		if (m_onlyUnloadOnLevelChange)
		{
			Unload();
		}
	}

	public override void RemoveUsage()
	{
		m_users--;
		if (m_users > 0)
		{
			return;
		}
		m_users = 0;
		if (m_holdResource == 0f)
		{
			if (!m_onlyUnloadOnLevelChange)
			{
				Unload();
			}
		}
		else
		{
			m_timer = m_holdResource;
		}
	}

	public override void AddUsage()
	{
		if (m_audioClip == null)
		{
			Load();
		}
		m_users++;
	}
}
