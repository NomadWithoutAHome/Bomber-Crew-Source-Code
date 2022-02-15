using BomberCrewCommon;
using UnityEngine;

public class EndlessModeUI : Singleton<EndlessModeUI>
{
	[SerializeField]
	private TextSetter m_scoreSetter;

	[SerializeField]
	private TextSetter m_waveSetter;

	[SerializeField]
	private GameObject m_toEnable;

	[SerializeField]
	private GameObject m_toDisable;

	[SerializeField]
	private Animation m_scoreChangeAnim;

	[SerializeField]
	private Animation m_waveChangeAnim;

	private int m_lastScore;

	private int m_lastWave;

	public void SetEndlessModeActive()
	{
		m_toEnable.SetActive(value: true);
		m_toDisable.SetActive(value: false);
		m_scoreSetter.SetText("0");
	}

	public void SetWave(int index)
	{
		m_waveSetter.SetText(string.Format(Singleton<LanguageProvider>.Instance.GetNamedTextImmediate("endlessmode_wave_title"), index + 1));
		if (m_lastWave != index)
		{
			m_waveChangeAnim.Play();
			m_lastWave = index;
		}
	}

	public void SetScore(int score)
	{
		if (score != m_lastScore)
		{
			m_lastScore = score;
			m_scoreSetter.SetText($"{score:N0}");
			m_scoreChangeAnim.Play();
		}
	}
}
