using System.Collections;
using BomberCrewCommon;
using UnityEngine;
using WingroveAudio;

public class EndlessDebriefingScreen : MonoBehaviour
{
	[SerializeField]
	private EndlessBriefingBoardPersistent m_briefingBoard;

	[SerializeField]
	private tk2dUIItem m_exitButton;

	[SerializeField]
	private GameObject m_exitHierarchy;

	[SerializeField]
	private AirbaseCameraNode m_camera;

	[SerializeField]
	[AudioEventName]
	private string m_showResultAudioEvent;

	[SerializeField]
	[AudioEventName]
	private string m_newRecordAudioEvent;

	[SerializeField]
	[AudioEventName]
	private string m_newHighscoreAudioEvent;

	[SerializeField]
	private Animator m_commanderAnimator;

	private EndlessModeGameFlow.EndlessModeResult m_result;

	private EndlessModeData m_endlessModeData;

	private int m_highestScore;

	private int m_highestWave;

	private void OnEnable()
	{
		m_exitButton.OnClick += OnExitDebrief;
		m_exitHierarchy.SetActive(value: false);
		AirbaseMainMenuButton.SetPauseBlocked(blocked: true);
		Singleton<AirbaseCameraController>.Instance.MoveCameraToLocationInstant(m_camera);
		m_result = Singleton<EndlessModeGameFlow>.Instance.GetEndlessModeResult();
		Singleton<HiScoreInterface>.Instance.SubmitScore(Singleton<EndlessModeGameFlow>.Instance.GetCurrentEndlessMode().GetLeaderboardIds(), m_result.m_totalScore);
		m_briefingBoard.UpdateResultsDisplay(m_result.m_wavesActuallyCompleted, m_result.m_totalScore);
		m_endlessModeData = Singleton<SystemDataContainer>.Instance.Get().GetEndlessModeData();
		m_highestScore = m_endlessModeData.GetHighScoreForMode(Singleton<EndlessModeGameFlow>.Instance.GetCurrentEndlessMode().name);
		m_highestWave = m_endlessModeData.GetWaveCountForMode(Singleton<EndlessModeGameFlow>.Instance.GetCurrentEndlessMode().name);
		m_briefingBoard.UpdateHighscoreDisplay(m_highestScore);
		m_briefingBoard.RefreshBoardBackgrounds();
		m_briefingBoard.ShowDebriefContents();
		StartCoroutine(DoDebrief());
	}

	private void OnExitDebrief()
	{
		Singleton<CrewContainer>.Instance.EndlessResurrectCrewmen();
		Singleton<AirbaseNavigation>.Instance.GoToDefaultScreen(jump: false);
		Singleton<GameFlow>.Instance.GetCurrentMissionInfo().SetCleared();
		AirbaseMainMenuButton.SetPauseBlocked(blocked: false);
		Singleton<EndlessModeGameFlow>.Instance.ClearEndlessModeScore();
		m_briefingBoard.ShowBriefingContents();
		if (!Singleton<HiScoreInterface>.Instance.IsOnline())
		{
			Singleton<HiScoreInterface>.Instance.RequestLogin(requestedByPlayer: false);
		}
		m_commanderAnimator.ResetTrigger("Clap");
	}

	private IEnumerator DoDebrief()
	{
		yield return new WaitForSeconds(2f);
		m_briefingBoard.ShowResultWaves();
		WingroveRoot.Instance.PostEvent(m_showResultAudioEvent);
		yield return new WaitForSeconds(1f);
		m_briefingBoard.ShowResultScore();
		WingroveRoot.Instance.PostEvent(m_showResultAudioEvent);
		yield return new WaitForSeconds(2f);
		if (m_result.m_wavesActuallyCompleted > m_highestScore)
		{
			m_briefingBoard.ShowNewRecordWaves();
			m_endlessModeData.SetWaveCountForMode(Singleton<EndlessModeGameFlow>.Instance.GetCurrentEndlessMode().name, m_result.m_wavesActuallyCompleted);
			WingroveRoot.Instance.PostEvent(m_newRecordAudioEvent);
			m_commanderAnimator.SetTrigger("Clap");
			yield return new WaitForSeconds(1f);
		}
		if (m_result.m_totalScore > m_highestScore)
		{
			m_briefingBoard.ShowNewRecordScore();
			m_endlessModeData.SetHighScoreForMode(Singleton<EndlessModeGameFlow>.Instance.GetCurrentEndlessMode().name, m_result.m_totalScore);
			WingroveRoot.Instance.PostEvent(m_newRecordAudioEvent);
			m_commanderAnimator.SetTrigger("Clap");
			yield return new WaitForSeconds(1f);
			m_briefingBoard.UpdateHighscoreDisplay(m_result.m_totalScore);
			WingroveRoot.Instance.PostEvent(m_newHighscoreAudioEvent);
		}
		yield return new WaitForSeconds(1f);
		m_exitHierarchy.SetActive(value: true);
	}
}
