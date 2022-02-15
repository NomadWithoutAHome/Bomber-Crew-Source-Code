using BomberCrewCommon;
using UnityEngine;

public class AceFighterInMission : MonoBehaviour
{
	[SerializeField]
	private FighterAI m_fighterAI;

	[SerializeField]
	private FighterPlane m_fighterPlane;

	[SerializeField]
	private float m_minTaunt = 15f;

	[SerializeField]
	private float m_maxTaunt = 30f;

	[SerializeField]
	private MusicSelectionRules.MusicTriggerEvents m_musicTrigger;

	[SerializeField]
	private GameObject m_objectiveDisplay;

	private EnemyFighterAce m_aceData;

	private TopBarInfoQueue.TopBarRequest m_initial;

	private TopBarInfoQueue.TopBarRequest m_randomSpeech;

	private TopBarInfoQueue.TopBarRequest m_destroyed;

	private TopBarInfoQueue.TopBarRequest m_retreat;

	private bool m_isEngaged;

	private bool m_isDestroyed;

	private float m_tauntTime;

	private bool m_encounteredPreviously;

	private int m_numEncounters;

	private bool m_triggeredMusic;

	private AceFighterSpawner m_spawner;

	private ObjectiveManager.Objective m_defeatObjective;

	private void TriggerMusic(bool on)
	{
		if (Singleton<MusicSelectionRules>.Instance != null && m_triggeredMusic != on)
		{
			m_triggeredMusic = on;
			if (on)
			{
				Singleton<MusicSelectionRules>.Instance.Trigger(m_musicTrigger);
			}
			else
			{
				Singleton<MusicSelectionRules>.Instance.Untrigger(m_musicTrigger);
			}
		}
	}

	private void Start()
	{
		GetComponent<TaggableItem>().OnTagComplete += OnTagged;
		Singleton<DifficultyMagic>.Instance.SetAcePresent();
	}

	private void OnTagged()
	{
		Singleton<CrewMiscSpeechTriggers>.Instance.DoExternalTrigger(CrewMiscSpeechTriggers.SpeechExternalTrigger.AcePilotTagged, null);
	}

	public void SetUp(EnemyFighterAce efa, AceFighterSpawner spawner)
	{
		m_spawner = spawner;
		m_aceData = efa;
		m_encounteredPreviously = Singleton<SaveDataContainer>.Instance.Get().HasPreviouslyEncountered(efa.name);
		m_numEncounters = Singleton<SaveDataContainer>.Instance.Get().GetNumTimesEncountered(efa.name);
		m_initial = TopBarInfoQueue.TopBarRequest.Speech(efa.GetAppearanceMessage(m_encounteredPreviously), $"{m_aceData.GetFirstName()} {m_aceData.GetSurname()}:", efa.GetSpeechPortraitTexture(), efa.GetJabberAudio(), isGoodGuy: false);
		m_initial.SetJabberSettings(efa.GetJabberSettings());
		m_initial.m_priority = 1000;
		m_destroyed = TopBarInfoQueue.TopBarRequest.Speech(efa.GetDefeatText(), $"{m_aceData.GetFirstName()} {m_aceData.GetSurname()}:", efa.GetSpeechPortraitTexture(), efa.GetJabberAudio(), isGoodGuy: false);
		m_destroyed.SetJabberSettings(efa.GetJabberSettings());
		m_destroyed.m_priority = 1000;
		m_retreat = TopBarInfoQueue.TopBarRequest.Speech(efa.GetRunAwayText(), $"{m_aceData.GetFirstName()} {m_aceData.GetSurname()}:", efa.GetSpeechPortraitTexture(), efa.GetJabberAudio(), isGoodGuy: false);
		m_retreat.SetJabberSettings(efa.GetJabberSettings());
		m_retreat.m_priority = 1000;
		m_tauntTime = Random.Range(m_minTaunt, m_maxTaunt);
		m_fighterPlane.OnDestroyed += OnDestroyed;
	}

	private void OnDisable()
	{
		TriggerMusic(on: false);
	}

	private void OnDestroy()
	{
		if (m_defeatObjective != null && !m_isDestroyed && !m_defeatObjective.m_complete)
		{
			Singleton<ObjectiveManager>.Instance.FailObjective(m_defeatObjective);
		}
		TriggerMusic(on: false);
	}

	public bool HasBeenEncounteredPreviously()
	{
		return m_encounteredPreviously;
	}

	public float GetHealthMultiplier()
	{
		if (m_numEncounters == 0)
		{
			return 1f;
		}
		if (m_numEncounters == 1)
		{
			return 0.66f;
		}
		if (m_numEncounters == 2)
		{
			return 0.4f;
		}
		return 0.25f;
	}

	public int GetNumEncounters()
	{
		if (m_numEncounters == 0 && m_encounteredPreviously)
		{
			return 1;
		}
		return m_numEncounters;
	}

	private void Update()
	{
		if (m_fighterPlane.IsDestroyed())
		{
			return;
		}
		if (!m_isEngaged)
		{
			if (m_fighterAI.IsEngaged())
			{
				m_isEngaged = true;
				Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetMissionLog().SetAceEncountered(m_aceData);
				Singleton<TopBarInfoQueue>.Instance.RegisterRequest(m_initial);
				Singleton<SaveDataContainer>.Instance.Get().SetAceEncountered(m_aceData.name);
				if (m_spawner != null)
				{
					m_spawner.OnAceEncounter();
				}
				TriggerMusic(on: true);
				if (m_defeatObjective == null)
				{
					ObjectiveManager.Objective objective = new ObjectiveManager.Objective();
					objective.m_aceFighter = this;
					objective.m_prefabOverride = m_objectiveDisplay;
					objective.m_countToComplete = 1;
					objective.m_objectiveType = ObjectiveManager.ObjectiveType.Ace;
					objective.m_objectiveGroup = m_aceData.name;
					m_defeatObjective = Singleton<ObjectiveManager>.Instance.RegisterObjective(objective);
				}
			}
		}
		else if (m_fighterAI.IsEngaged())
		{
			m_tauntTime -= Time.deltaTime;
			if (m_tauntTime < 0f)
			{
				m_tauntTime = Random.Range(m_minTaunt, m_maxTaunt);
				if (m_randomSpeech != null)
				{
					Singleton<TopBarInfoQueue>.Instance.RemoveRequest(m_randomSpeech);
					m_randomSpeech = null;
				}
				m_randomSpeech = TopBarInfoQueue.TopBarRequest.Speech(m_aceData.GetTauntGroup(), $"{m_aceData.GetFirstName()} {m_aceData.GetSurname()}:", m_aceData.GetSpeechPortraitTexture(), m_aceData.GetJabberAudio(), isGoodGuy: false);
				m_randomSpeech.SetJabberSettings(m_aceData.GetJabberSettings());
				m_randomSpeech.m_priority = 5;
				Singleton<TopBarInfoQueue>.Instance.RegisterRequest(m_randomSpeech);
			}
		}
		else
		{
			TriggerMusic(on: false);
			Singleton<TopBarInfoQueue>.Instance.RegisterRequest(m_retreat);
			Singleton<TopBarInfoQueue>.Instance.RemoveRequest(m_initial);
			if (m_randomSpeech != null)
			{
				Singleton<TopBarInfoQueue>.Instance.RemoveRequest(m_randomSpeech);
				m_randomSpeech = null;
			}
			m_isEngaged = false;
		}
	}

	private void OnDestroyed()
	{
		if (!m_isDestroyed && m_fighterPlane.IsDestroyed())
		{
			string text = m_aceData.name;
			Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetMissionLog().SetAceDefeated(!Singleton<SaveDataContainer>.Instance.Get().IsAceDefeated(text));
			Singleton<SaveDataContainer>.Instance.Get().SetAceDefeated(text);
			m_isEngaged = true;
			m_isDestroyed = true;
			Singleton<TopBarInfoQueue>.Instance.RemoveRequest(m_initial);
			Singleton<TopBarInfoQueue>.Instance.RemoveRequest(m_retreat);
			if (m_randomSpeech != null)
			{
				Singleton<TopBarInfoQueue>.Instance.RemoveRequest(m_randomSpeech);
			}
			Singleton<TopBarInfoQueue>.Instance.RegisterRequest(m_destroyed);
			TriggerMusic(on: false);
			Singleton<MusicSelectionRules>.Instance.TriggerTimed(MusicSelectionRules.MusicTriggerEvents.AceDefeated, 5f);
			if (m_defeatObjective != null)
			{
				Singleton<ObjectiveManager>.Instance.CompleteObjective(m_defeatObjective);
			}
		}
	}

	public EnemyFighterAce GetAceInformation()
	{
		return m_aceData;
	}
}
