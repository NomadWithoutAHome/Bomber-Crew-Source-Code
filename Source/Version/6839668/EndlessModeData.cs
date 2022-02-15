using System.Collections.Generic;
using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class EndlessModeData
{
	[JsonProperty]
	private Dictionary<string, int> m_highScorePerModeLocal = new Dictionary<string, int>();

	[JsonProperty]
	private Dictionary<string, int> m_waveCountPerModeLocal = new Dictionary<string, int>();

	public int GetHighScoreForMode(string modeName)
	{
		if (m_highScorePerModeLocal == null)
		{
			m_highScorePerModeLocal = new Dictionary<string, int>();
		}
		int value = 0;
		m_highScorePerModeLocal.TryGetValue(modeName, out value);
		return value;
	}

	public int GetWaveCountForMode(string modeName)
	{
		if (m_waveCountPerModeLocal == null)
		{
			m_waveCountPerModeLocal = new Dictionary<string, int>();
		}
		int value = 0;
		m_waveCountPerModeLocal.TryGetValue(modeName, out value);
		return value;
	}

	public void SetHighScoreForMode(string modeName, int score)
	{
		if (m_highScorePerModeLocal == null)
		{
			m_highScorePerModeLocal = new Dictionary<string, int>();
		}
		m_highScorePerModeLocal[modeName] = score;
	}

	public void SetWaveCountForMode(string modeName, int waveCount)
	{
		if (m_waveCountPerModeLocal == null)
		{
			m_waveCountPerModeLocal = new Dictionary<string, int>();
		}
		m_waveCountPerModeLocal[modeName] = waveCount;
	}
}
