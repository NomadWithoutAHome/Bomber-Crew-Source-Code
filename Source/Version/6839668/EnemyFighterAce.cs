using UnityEngine;
using WingroveAudio;

[CreateAssetMenu(menuName = "Bomber Crew/Enemy Fighter Ace")]
public class EnemyFighterAce : ScriptableObject
{
	[SerializeField]
	private string m_firstName;

	[SerializeField]
	private string m_surname;

	[SerializeField]
	private bool m_identityKnown = true;

	[SerializeField]
	private Texture2D m_portraitTexture;

	[SerializeField]
	private Texture2D m_speechPortraitTexture;

	[SerializeField]
	[AudioEventName]
	private string m_jabberAudio;

	[SerializeField]
	private GameObject m_fighterPrefab;

	[SerializeField]
	[NamedText]
	private string m_initialAppearText;

	[SerializeField]
	[NamedText]
	private string m_subsequentAppearText;

	[SerializeField]
	private string m_namedTextRandomGroup;

	[SerializeField]
	private string m_runAwayText;

	[SerializeField]
	[NamedText]
	private string m_defeatText;

	[SerializeField]
	private int m_cashBonusForDefeat;

	[SerializeField]
	private int m_aceSpeechIndex;

	[SerializeField]
	private Jabberer.JabberSettings m_jabberSettings;

	[SerializeField]
	private bool m_isPluralAce;

	public string GetAppearanceMessage(bool previouslyEncountered)
	{
		if (previouslyEncountered)
		{
			return m_subsequentAppearText;
		}
		return m_initialAppearText;
	}

	public bool IsPluralAce()
	{
		return m_isPluralAce;
	}

	public string GetTauntGroup()
	{
		return m_namedTextRandomGroup;
	}

	public string GetRunAwayText()
	{
		return m_runAwayText;
	}

	public string GetDefeatText()
	{
		return m_defeatText;
	}

	public string GetFirstName()
	{
		return m_firstName;
	}

	public string GetSurname()
	{
		return m_surname;
	}

	public Texture2D GetPortraitTexture()
	{
		return m_portraitTexture;
	}

	public Texture2D GetSpeechPortraitTexture()
	{
		return m_speechPortraitTexture;
	}

	public string GetJabberAudio()
	{
		return m_jabberAudio;
	}

	public Jabberer.JabberSettings GetJabberSettings()
	{
		return m_jabberSettings;
	}

	public GameObject GetInMissionPrefab()
	{
		return m_fighterPrefab;
	}

	public int GetCashBonus()
	{
		return m_cashBonusForDefeat;
	}
}
