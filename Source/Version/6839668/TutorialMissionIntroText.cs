using System.Collections;
using BomberCrewCommon;
using UnityEngine;
using WingroveAudio;

public class TutorialMissionIntroText : MonoBehaviour
{
	[SerializeField]
	[NamedText]
	private string[] m_namedTextStrings;

	[SerializeField]
	private float m_timeBetweenStrings = 5f;

	[SerializeField]
	private tk2dBaseSprite m_fader;

	[SerializeField]
	private float m_fadeSpeed = 0.1f;

	[SerializeField]
	private TextSetter m_textSetter;

	[SerializeField]
	private int m_startFadeAt;

	[SerializeField]
	private float m_audioEventStopTrigger;

	private int m_fadeProgressCached;

	private IEnumerator FadeOutFader()
	{
		Singleton<InputLayerInterface>.Instance.EnableAllLayers();
		Singleton<UISelector>.Instance.Resume();
		Singleton<BomberCamera>.Instance.Resume();
		Singleton<ContextControl>.Instance.UnlockInput();
		float t = 1f;
		while (t > 0f)
		{
			t -= Time.deltaTime * m_fadeSpeed;
			WingroveRoot.Instance.SetParameterGlobal(m_fadeProgressCached, t);
			m_fader.color = new Color(0f, 0f, 0f, t);
			yield return null;
		}
		WingroveRoot.Instance.SetParameterGlobal(m_fadeProgressCached, 0f);
		Object.Destroy(base.gameObject);
	}

	private void Awake()
	{
		m_fadeProgressCached = WingroveRoot.Instance.GetParameterId("TUTORIAL_FADE_PROGRESS");
		WingroveRoot.Instance.SetParameterGlobal(m_fadeProgressCached, 1f);
		WingroveRoot.Instance.PostEvent("TUTORIAL_START");
	}

	private IEnumerator WaitAndFireTutorialStopEvent()
	{
		yield return new WaitForSeconds(m_audioEventStopTrigger);
		WingroveRoot.Instance.PostEvent("TUTORIAL_END");
	}

	private IEnumerator Start()
	{
		Singleton<InputLayerInterface>.Instance.DisableAllLayers();
		Singleton<UISelector>.Instance.Pause();
		Singleton<BomberCamera>.Instance.Pause();
		Singleton<ContextControl>.Instance.LockInput();
		StartCoroutine(WaitAndFireTutorialStopEvent());
		int index = 0;
		string[] namedTextStrings = m_namedTextStrings;
		foreach (string s in namedTextStrings)
		{
			m_textSetter.SetTextFromLanguageString(s);
			if (index == m_startFadeAt)
			{
				StartCoroutine(FadeOutFader());
			}
			index++;
			yield return new WaitForSeconds(m_timeBetweenStrings);
		}
		m_textSetter.SetText(string.Empty);
	}
}
