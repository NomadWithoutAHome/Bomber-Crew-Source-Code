using System.Collections;
using BomberCrewCommon;
using UnityEngine;

public class SplashScreenDisplayer : MonoBehaviour
{
	[SerializeField]
	private AnimationClip m_showAnimation;

	[SerializeField]
	private AnimationClip m_hideAnimation;

	[SerializeField]
	private AnimationClip m_hideAllAnimation;

	[SerializeField]
	private Animation m_animToPlay;

	[SerializeField]
	private Renderer m_logoRenderer;

	[SerializeField]
	private Texture2D[] m_texturesToShow;

	[SerializeField]
	private float m_minTime;

	[SerializeField]
	private float m_betweenTime;

	private IEnumerator Start()
	{
		Object.DontDestroyOnLoad(base.gameObject);
		Singleton<SystemLoader>.Instance.SetSplashSequence(splashSequence: true);
		Texture2D[] texturesToShow = m_texturesToShow;
		foreach (Texture2D tts in texturesToShow)
		{
			Singleton<SystemLoader>.Instance.SetPauseLoading(pause: true);
			while (!Singleton<SystemLoader>.Instance.IsPaused())
			{
				yield return null;
			}
			m_logoRenderer.sharedMaterial.mainTexture = tts;
			m_animToPlay.Play(m_showAnimation.name);
			while (m_animToPlay.isPlaying)
			{
				yield return null;
			}
			Singleton<SystemLoader>.Instance.SetPauseLoading(pause: false);
			yield return new WaitForSeconds(m_minTime);
			Singleton<SystemLoader>.Instance.SetPauseLoading(pause: true);
			while (!Singleton<SystemLoader>.Instance.IsPaused())
			{
				yield return null;
			}
			m_animToPlay.Play(m_hideAnimation.name);
			while (m_animToPlay.isPlaying)
			{
				yield return null;
			}
			Singleton<SystemLoader>.Instance.SetPauseLoading(pause: false);
			yield return new WaitForSeconds(m_betweenTime);
		}
		Singleton<SystemLoader>.Instance.SetPauseLoading(pause: true);
		while (!Singleton<SystemLoader>.Instance.IsPaused())
		{
			yield return null;
		}
		m_animToPlay.Play(m_hideAllAnimation.name);
		while (m_animToPlay.isPlaying)
		{
			yield return null;
		}
		Singleton<SystemLoader>.Instance.SetPauseLoading(pause: false);
		Singleton<SystemLoader>.Instance.SetSplashSequence(splashSequence: false);
		Object.Destroy(base.gameObject);
	}
}
