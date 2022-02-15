using System.Collections;
using BomberCrewCommon;
using Common;
using UnityEngine;
using WingroveAudio;

public class EndlessModeWaveDisplay : MonoBehaviour
{
	[SerializeField]
	private TextSetter m_titleSetter;

	[SerializeField]
	private TextSetter m_waveNumberSetter;

	[SerializeField]
	private float m_delayAudio;

	[SerializeField]
	[AudioEventName]
	private string m_audioHookStart;

	[SerializeField]
	[AudioEventName]
	private string m_audioHookEnd;

	[SerializeField]
	private float m_displayTime;

	[SerializeField]
	private TextSetter m_mainScore;

	[SerializeField]
	private TextSetter m_timeBonusScore;

	[SerializeField]
	private GameObject m_waveNumber;

	[SerializeField]
	private GameObject m_backgroundDefault;

	[SerializeField]
	private GameObject m_waveObjective;

	[SerializeField]
	private GameObject m_waveComplete;

	[SerializeField]
	private GameObject m_waveFailed;

	[SerializeField]
	private GameObject m_backgroundComplete;

	[SerializeField]
	private GameObject m_backgroundFailed;

	[SerializeField]
	private GameObject m_mainScoreHierarchy;

	[SerializeField]
	private GameObject m_timeScoreHierarchy;

	private int m_mainScoreValue;

	private int m_timeScoreValue;

	public void SetUp(string waveText, string objectiveText)
	{
		m_titleSetter.SetText(objectiveText);
		m_waveNumberSetter.SetText(waveText);
		StartCoroutine(DoRegular());
	}

	public void SetUpComplete(string waveText, string objectiveText, int mainScore, int timeBonusScore)
	{
		m_titleSetter.SetText(objectiveText);
		m_waveNumberSetter.SetText(waveText);
		m_mainScore.SetText($"{mainScore:N0}");
		m_timeBonusScore.SetText($"{timeBonusScore:N0}");
		m_mainScoreValue = mainScore;
		m_timeScoreValue = timeBonusScore;
		StartCoroutine(DoWaveComplete());
	}

	public void SetUpFail(string waveText, string objectiveText)
	{
		m_titleSetter.SetText(objectiveText);
		m_waveNumberSetter.SetText(waveText);
		StartCoroutine(DoWaveFailed());
	}

	private IEnumerator DoRegular()
	{
		WingroveRoot.Instance.PostEvent("ENDLESS_OVERLAY_APPEAR");
		m_backgroundDefault.CustomActivate(active: true);
		yield return new WaitForSeconds(0.5f);
		WingroveRoot.Instance.PostEvent("ENDLESS_OVERLAY_APPEAR2");
		m_waveNumber.CustomActivate(active: true);
		yield return new WaitForSeconds(1f);
		WingroveRoot.Instance.PostEvent("ENDLESS_OVERLAY_APPEAR");
		m_waveObjective.CustomActivate(active: true);
		yield return new WaitForSeconds(6f);
		WingroveRoot.Instance.PostEvent("ENDLESS_OVERLAY_DISAPPEAR");
		m_waveObjective.CustomActivate(active: false);
		m_waveNumber.CustomActivate(active: false);
		m_backgroundDefault.CustomActivate(active: false);
		yield return new WaitForSeconds(1f);
		Object.Destroy(base.gameObject);
	}

	private IEnumerator DoWaveComplete()
	{
		WingroveRoot.Instance.PostEvent("ENDLESS_OVERLAY_APPEAR");
		m_backgroundDefault.CustomActivate(active: true);
		m_waveNumber.CustomActivate(active: true);
		m_waveObjective.CustomActivate(active: true);
		yield return new WaitForSeconds(2f);
		WingroveRoot.Instance.PostEvent("ENDLESS_OVERLAY_WAVE");
		m_waveObjective.SetActive(value: false);
		m_backgroundDefault.SetActive(value: false);
		m_waveComplete.CustomActivate(active: true);
		m_backgroundComplete.CustomActivate(active: true);
		yield return new WaitForSeconds(2f);
		m_waveComplete.CustomActivate(active: false);
		yield return new WaitForSeconds(0.5f);
		WingroveRoot.Instance.PostEvent("ENDLESS_OVERLAY_SCORE");
		m_mainScoreHierarchy.CustomActivate(active: true);
		WingroveRoot.Instance.PostEvent(m_audioHookEnd);
		Singleton<EndlessModeGameFlow>.Instance.AddScore(m_mainScoreValue);
		yield return new WaitForSeconds(1f);
		WingroveRoot.Instance.PostEvent("ENDLESS_OVERLAY_SCORE");
		m_timeScoreHierarchy.CustomActivate(active: true);
		WingroveRoot.Instance.PostEvent(m_audioHookEnd);
		Singleton<EndlessModeGameFlow>.Instance.AddScore(m_timeScoreValue);
		yield return new WaitForSeconds(6f);
		WingroveRoot.Instance.PostEvent("ENDLESS_OVERLAY_DISAPPEAR");
		m_waveNumber.CustomActivate(active: false);
		m_backgroundComplete.CustomActivate(active: false);
		m_mainScoreHierarchy.CustomActivate(active: false);
		m_timeScoreHierarchy.CustomActivate(active: false);
		yield return new WaitForSeconds(1f);
		Object.Destroy(base.gameObject);
	}

	private IEnumerator DoWaveFailed()
	{
		WingroveRoot.Instance.PostEvent("ENDLESS_OVERLAY_APPEAR");
		m_backgroundDefault.CustomActivate(active: true);
		m_waveNumber.CustomActivate(active: true);
		m_waveObjective.CustomActivate(active: true);
		yield return new WaitForSeconds(2f);
		WingroveRoot.Instance.PostEvent("ENDLESS_OVERLAY_FAIL");
		m_waveObjective.SetActive(value: false);
		m_backgroundDefault.SetActive(value: false);
		m_waveFailed.CustomActivate(active: true);
		m_backgroundFailed.CustomActivate(active: true);
		yield return new WaitForSeconds(3f);
		WingroveRoot.Instance.PostEvent("ENDLESS_OVERLAY_DISAPPEAR");
		m_waveNumber.CustomActivate(active: false);
		m_waveFailed.CustomActivate(active: false);
		m_backgroundFailed.CustomActivate(active: false);
		yield return new WaitForSeconds(1f);
		Object.Destroy(base.gameObject);
	}
}
