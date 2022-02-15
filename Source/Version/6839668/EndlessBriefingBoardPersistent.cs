using BomberCrewCommon;
using Common;
using UnityEngine;

public class EndlessBriefingBoardPersistent : MonoBehaviour
{
	[SerializeField]
	private GameObject m_contentsBriefing;

	[SerializeField]
	private GameObject m_contentsDebrief;

	[SerializeField]
	private Renderer[] m_boardHeaderRenderers;

	[SerializeField]
	private TextSetter m_briefingTextMesh;

	[SerializeField]
	private TextSetter[] m_highscoreTextMeshes;

	[SerializeField]
	private GameObject m_resultsWavestHierachy;

	[SerializeField]
	private GameObject m_resultsScoreHierachy;

	[SerializeField]
	private TextSetter m_resultsWavesText;

	[SerializeField]
	private TextSetter m_resultsScoreText;

	[SerializeField]
	private GameObject m_newRecordScoreHierachy;

	[SerializeField]
	private GameObject m_newRecordWaveCountHierachy;

	[SerializeField]
	private GameObject m_newRecordHighScoreHierachy;

	public void ShowBriefingContents()
	{
		RefreshBriefingContents();
		if (!m_contentsBriefing.IsActivated())
		{
			m_contentsBriefing.CustomActivate(active: true);
		}
		if (m_contentsDebrief.IsActivated())
		{
			m_contentsDebrief.CustomActivate(active: false);
		}
	}

	public void ShowDebriefContents()
	{
		if (m_contentsBriefing.IsActivated())
		{
			m_contentsBriefing.CustomActivate(active: false);
		}
		if (!m_contentsDebrief.IsActivated())
		{
			m_contentsDebrief.CustomActivate(active: true);
		}
		m_resultsScoreHierachy.SetActive(value: false);
		m_resultsWavestHierachy.SetActive(value: false);
		m_newRecordScoreHierachy.SetActive(value: false);
		m_newRecordWaveCountHierachy.SetActive(value: false);
		m_newRecordHighScoreHierachy.SetActive(value: false);
	}

	public void RefreshBoardBackgrounds()
	{
		EndlessModeVariant currentEndlessMode = Singleton<EndlessModeGameFlow>.Instance.GetCurrentEndlessMode();
		Renderer[] boardHeaderRenderers = m_boardHeaderRenderers;
		foreach (Renderer renderer in boardHeaderRenderers)
		{
			renderer.sharedMaterial.SetTexture("_MainTex", currentEndlessMode.GetBriefingBoardHeaderImage());
		}
	}

	public void RefreshBriefingContents()
	{
		RefreshBoardBackgrounds();
		EndlessModeVariant currentEndlessMode = Singleton<EndlessModeGameFlow>.Instance.GetCurrentEndlessMode();
		m_briefingTextMesh.SetTextFromLanguageString(currentEndlessMode.GetNamedTextBriefing());
		EndlessModeData endlessModeData = Singleton<SystemDataContainer>.Instance.Get().GetEndlessModeData();
		int highScoreForMode = endlessModeData.GetHighScoreForMode(Singleton<EndlessModeGameFlow>.Instance.GetCurrentEndlessMode().name);
		UpdateHighscoreDisplay(highScoreForMode);
	}

	public void UpdateHighscoreDisplay(int highscore)
	{
		TextSetter[] highscoreTextMeshes = m_highscoreTextMeshes;
		foreach (TextSetter textSetter in highscoreTextMeshes)
		{
			textSetter.SetText($"{highscore:N0}");
		}
	}

	public void ShowNewRecordHighscore()
	{
		m_newRecordHighScoreHierachy.CustomActivate(active: true);
	}

	public void UpdateResultsDisplay(int waves, int score)
	{
		m_resultsScoreText.SetText($"{score:N0}");
		m_resultsWavesText.SetText(waves.ToString());
	}

	public void ShowResultWaves()
	{
		m_resultsWavestHierachy.CustomActivate(active: true);
	}

	public void ShowResultScore()
	{
		m_resultsScoreHierachy.CustomActivate(active: true);
	}

	public void ShowNewRecordWaves()
	{
		m_newRecordWaveCountHierachy.CustomActivate(active: true);
	}

	public void ShowNewRecordScore()
	{
		m_newRecordScoreHierachy.CustomActivate(active: true);
	}
}
