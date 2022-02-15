using System;
using System.Collections.Generic;
using AudioNames;
using BomberCrewCommon;
using UnityEngine;
using WingroveAudio;

public class MusicSelectionRules : Singleton<MusicSelectionRules>
{
	public enum MusicTriggerEvents
	{
		AceFighterEncounterActive01,
		LandingAtBase,
		CriticalDamage,
		EnemyFighterActiveCount,
		MajorDamage,
		HazardNearBy,
		AceFighterEncounterActive02,
		AceFighterEncounterActive03,
		ReturnJourney,
		HighAltitude,
		MidAltitude,
		LowAltitude,
		EnemyFightersSmallGroup,
		EnemyFightersBigGroup,
		EnemyFightersHeavyType,
		EnemyFighterBelowType,
		EnemyFighterJetType,
		EmergencyLand,
		UNUSED_REPLACE,
		AboveEngland,
		ObjectiveFailed,
		ObjectiveComplete,
		KeyMissionBegin,
		AceFighterEncounterActive04,
		AceFighterEncounterActive05,
		AceFighterEncounterActive06,
		AceFighterEncounterActive07,
		AceFighterEncounterActive08,
		StealthHazardNearish,
		CrewmanDead,
		AceDefeated,
		DescendingBad,
		TrainingMission1,
		TrainingMission2,
		ObjectiveCompleteEver,
		ObjectiveFailedEver,
		TrainingMissionEver,
		AceFighterEncounterActive09,
		AceFighterEncounterActive10,
		AceFighterEncounterActive11,
		WinterEnvironment,
		AceFighterEncounterActive12,
		AceFighterEncounterActive13,
		AceFighterEncounterActive14,
		AceFighterEncounterActive15
	}

	public enum MusicPriority
	{
		LowAmbient,
		NormalAction,
		HighEvent
	}

	[Serializable]
	public class MusicTrigger
	{
		[SerializeField]
		public MusicTriggerEvents m_triggerName;

		[SerializeField]
		public bool m_requiredToStart;

		[SerializeField]
		public bool m_requiredOffToStart;

		[SerializeField]
		public bool m_requiredToContinue;

		[SerializeField]
		public bool m_requiredOffToContinue;
	}

	[Serializable]
	public class MusicSelection
	{
		[SerializeField]
		public string m_identifier;

		[SerializeField]
		public MusicTrigger[] m_respondToTriggers;

		[SerializeField]
		[AudioEventName]
		public string m_cueToFire;

		[SerializeField]
		public float m_silencePreviousRequired;

		[SerializeField]
		public float m_timeInMissionRequired;

		[SerializeField]
		public MusicPriority m_priority;

		[SerializeField]
		public int m_subPriority;

		[SerializeField]
		public float m_minPlayTime;

		[SerializeField]
		public bool m_noRepeats;

		[SerializeField]
		public float m_forceRetriggerAfterTime;

		[SerializeField]
		public bool m_interruptAny;
	}

	private class MusicSelectionLive
	{
		public MusicSelection m_selectionRule;

		public bool m_canStart;

		public bool m_canContinue;
	}

	private class ActiveTimedTrigger
	{
		public MusicTriggerEvents m_trigger;

		public float m_timer;
	}

	[SerializeField]
	private MusicSelection[] m_allMusicSelections;

	[SerializeField]
	private bool m_debug;

	private List<MusicSelectionLive> m_allMusicSelectionsLive = new List<MusicSelectionLive>();

	private MusicSelectionLive m_currentlyPlaying;

	private MusicSelectionLive m_toNextPlayingStable;

	private MusicSelectionLive m_lastPlayed;

	private float m_stabilityTimer;

	private float m_timeSinceMusic;

	private float m_beenPlayingFor;

	private float m_beenPlayingForLoopable;

	private bool m_internalRuleReturnJourneyCloseToHome;

	private bool m_internalRuleLowAlt;

	private bool m_internalRuleMidAlt;

	private bool m_internalRuleHighAlt;

	private bool m_internalRuleReturnJourney;

	private bool m_internalRuleAboveEngland;

	private int m_internalRuleNumEngaged;

	private List<ActiveTimedTrigger> m_activeTriggers = new List<ActiveTimedTrigger>(16);

	private Dictionary<int, int> m_currentTriggerPoints = new Dictionary<int, int>(32);

	private bool m_shouldRefreshValid;

	private WingroveGroupInformation m_musicGroupInfo;

	private float m_timeInMission;

	private List<ActiveTimedTrigger> m_cachedToRemove = new List<ActiveTimedTrigger>(10);

	public void Trigger(MusicTriggerEvents triggerName)
	{
		int value = 0;
		m_currentTriggerPoints.TryGetValue((int)triggerName, out value);
		value++;
		m_currentTriggerPoints[(int)triggerName] = value;
		m_shouldRefreshValid = true;
	}

	public void TriggerTimed(MusicTriggerEvents triggerName, float time)
	{
		Trigger(triggerName);
		ActiveTimedTrigger activeTimedTrigger = new ActiveTimedTrigger();
		activeTimedTrigger.m_trigger = triggerName;
		activeTimedTrigger.m_timer = time;
		m_activeTriggers.Add(activeTimedTrigger);
	}

	public void Untrigger(MusicTriggerEvents triggerName)
	{
		int value = 0;
		m_currentTriggerPoints.TryGetValue((int)triggerName, out value);
		value--;
		m_currentTriggerPoints[(int)triggerName] = value;
		m_shouldRefreshValid = true;
	}

	private void Awake()
	{
		MusicSelection[] allMusicSelections = m_allMusicSelections;
		foreach (MusicSelection selectionRule in allMusicSelections)
		{
			MusicSelectionLive musicSelectionLive = new MusicSelectionLive();
			musicSelectionLive.m_selectionRule = selectionRule;
			m_allMusicSelectionsLive.Add(musicSelectionLive);
		}
		m_musicGroupInfo = WingroveRoot.Instance.transform.Find("Master").Find("MusicMaster").Find("Music")
			.GetComponent<WingroveGroupInformation>();
		if (m_musicGroupInfo == null)
		{
			DebugLogWrapper.LogError("Couldn't find audio group to monitor...");
		}
	}

	private void OnDisable()
	{
		WingroveRoot.Instance.PostEvent("MUSIC_STOP");
	}

	private void UpdateInternalRules()
	{
		BomberSystems bomberSystems = Singleton<BomberSpawn>.Instance.GetBomberSystems();
		if (!Singleton<MissionCoordinator>.Instance.IsOutwardJourney())
		{
			if (!m_internalRuleReturnJourney)
			{
				Trigger(MusicTriggerEvents.ReturnJourney);
				m_internalRuleReturnJourney = true;
			}
			if (bomberSystems.gameObject.btransform().position.magnitude < 3000.0 && !m_internalRuleReturnJourneyCloseToHome && (bomberSystems.GetBomberState().IsLanding() || bomberSystems.GetBomberState().IsPreLandingClose()))
			{
				m_internalRuleReturnJourneyCloseToHome = true;
				Trigger(MusicTriggerEvents.LandingAtBase);
			}
		}
		else if (m_internalRuleReturnJourney)
		{
			Untrigger(MusicTriggerEvents.ReturnJourney);
			m_internalRuleReturnJourney = false;
		}
		if (m_internalRuleAboveEngland != bomberSystems.GetBomberState().IsAboveEngland())
		{
			if (m_internalRuleAboveEngland)
			{
				Untrigger(MusicTriggerEvents.AboveEngland);
			}
			else
			{
				Trigger(MusicTriggerEvents.AboveEngland);
			}
			m_internalRuleAboveEngland = !m_internalRuleAboveEngland;
		}
		foreach (ActiveTimedTrigger activeTrigger in m_activeTriggers)
		{
			activeTrigger.m_timer -= Time.deltaTime;
			if (activeTrigger.m_timer < 0f)
			{
				Untrigger(activeTrigger.m_trigger);
				m_cachedToRemove.Add(activeTrigger);
			}
		}
		foreach (ActiveTimedTrigger item in m_cachedToRemove)
		{
			m_activeTriggers.Remove(item);
		}
		m_cachedToRemove.Clear();
		float altitudeNormalised = bomberSystems.GetBomberState().GetAltitudeNormalised();
		if (altitudeNormalised < 0.3f)
		{
			if (!m_internalRuleLowAlt)
			{
				Trigger(MusicTriggerEvents.LowAltitude);
				m_internalRuleLowAlt = true;
			}
			if (m_internalRuleMidAlt)
			{
				Untrigger(MusicTriggerEvents.MidAltitude);
				m_internalRuleMidAlt = false;
			}
			if (m_internalRuleHighAlt)
			{
				Untrigger(MusicTriggerEvents.HighAltitude);
				m_internalRuleHighAlt = false;
			}
		}
		else if (altitudeNormalised < 0.6f)
		{
			if (!m_internalRuleMidAlt)
			{
				Trigger(MusicTriggerEvents.MidAltitude);
				m_internalRuleMidAlt = true;
			}
			if (m_internalRuleLowAlt)
			{
				Untrigger(MusicTriggerEvents.LowAltitude);
				m_internalRuleLowAlt = false;
			}
			if (m_internalRuleHighAlt)
			{
				Untrigger(MusicTriggerEvents.HighAltitude);
				m_internalRuleHighAlt = false;
			}
		}
		else
		{
			if (!m_internalRuleHighAlt)
			{
				Trigger(MusicTriggerEvents.HighAltitude);
				m_internalRuleHighAlt = true;
			}
			if (m_internalRuleMidAlt)
			{
				Untrigger(MusicTriggerEvents.MidAltitude);
				m_internalRuleMidAlt = false;
			}
			if (m_internalRuleLowAlt)
			{
				Untrigger(MusicTriggerEvents.LowAltitude);
				m_internalRuleLowAlt = false;
			}
		}
	}

	private void OnGUI()
	{
		if (!m_debug)
		{
			return;
		}
		GUILayout.BeginArea(new Rect(0f, 0f, 500f, 1000f));
		foreach (MusicSelectionLive item in m_allMusicSelectionsLive)
		{
			if (item == m_currentlyPlaying)
			{
				GUILayout.Label("--> " + item.m_selectionRule.m_identifier + ": " + item.m_canContinue);
			}
			else
			{
				GUILayout.Label(item.m_selectionRule.m_identifier + ": " + item.m_canStart);
			}
		}
		GUILayout.EndArea();
	}

	private void RefreshValidity()
	{
		foreach (MusicSelectionLive item in m_allMusicSelectionsLive)
		{
			bool canStart = true;
			bool canContinue = true;
			MusicTrigger[] respondToTriggers = item.m_selectionRule.m_respondToTriggers;
			foreach (MusicTrigger musicTrigger in respondToTriggers)
			{
				int value = 0;
				m_currentTriggerPoints.TryGetValue((int)musicTrigger.m_triggerName, out value);
				if (musicTrigger.m_requiredToStart && value <= 0)
				{
					canStart = false;
				}
				if (musicTrigger.m_requiredOffToStart && value > 0)
				{
					canStart = false;
				}
				if (musicTrigger.m_requiredToContinue && value <= 0)
				{
					canContinue = false;
				}
				if (musicTrigger.m_requiredOffToContinue && value > 0)
				{
					canContinue = false;
				}
			}
			if (m_timeSinceMusic < item.m_selectionRule.m_silencePreviousRequired)
			{
				canStart = false;
			}
			if (m_timeInMission < item.m_selectionRule.m_timeInMissionRequired)
			{
				canStart = false;
			}
			item.m_canStart = canStart;
			item.m_canContinue = canContinue;
		}
	}

	private void Update()
	{
		WingroveRoot.Instance.SetParameterGlobal(MusicEvents.Parameters.CacheVal_MusicDaynight(), Singleton<VisibilityHelpers>.Instance.GetNightFactor());
		UpdateInternalRules();
		if (m_shouldRefreshValid)
		{
			RefreshValidity();
		}
		MusicSelectionLive musicSelectionLive = null;
		MusicPriority musicPriority = MusicPriority.LowAmbient;
		int num = 0;
		foreach (MusicSelectionLive item in m_allMusicSelectionsLive)
		{
			if (item.m_canStart)
			{
				bool flag = false;
				if (musicSelectionLive == null)
				{
					flag = true;
				}
				else if (item.m_selectionRule.m_priority > musicPriority)
				{
					flag = true;
				}
				else if (item.m_selectionRule.m_priority == musicPriority && item.m_selectionRule.m_subPriority > num)
				{
					flag = true;
				}
				if (item.m_selectionRule.m_noRepeats && item == m_lastPlayed)
				{
					flag = false;
				}
				if (flag)
				{
					musicSelectionLive = item;
					musicPriority = item.m_selectionRule.m_priority;
					num = item.m_selectionRule.m_subPriority;
				}
			}
		}
		if (m_toNextPlayingStable != musicSelectionLive)
		{
			m_toNextPlayingStable = musicSelectionLive;
			m_stabilityTimer = 0f;
		}
		else
		{
			m_stabilityTimer += Time.deltaTime;
		}
		if (!m_musicGroupInfo.IsAnyPlaying())
		{
			m_currentlyPlaying = null;
		}
		if (m_currentlyPlaying == null)
		{
			m_timeSinceMusic += Time.deltaTime;
		}
		else
		{
			m_timeSinceMusic = 0f;
			if (!m_currentlyPlaying.m_canContinue)
			{
				m_currentlyPlaying = null;
				WingroveRoot.Instance.PostEvent("MUSIC_STOP");
			}
			else if (m_currentlyPlaying.m_selectionRule.m_forceRetriggerAfterTime != 0f && m_beenPlayingForLoopable > m_currentlyPlaying.m_selectionRule.m_forceRetriggerAfterTime)
			{
				WingroveRoot.Instance.PostEvent("MUSIC_STOP_FAST");
				WingroveRoot.Instance.PostEvent(m_currentlyPlaying.m_selectionRule.m_cueToFire);
				m_beenPlayingForLoopable = 0f;
			}
		}
		if (m_currentlyPlaying != musicSelectionLive && musicSelectionLive != null)
		{
			bool flag2 = m_currentlyPlaying == null || m_beenPlayingFor > m_currentlyPlaying.m_selectionRule.m_minPlayTime;
			if (m_currentlyPlaying != null)
			{
				if (musicSelectionLive.m_selectionRule.m_interruptAny && !m_currentlyPlaying.m_selectionRule.m_interruptAny)
				{
					flag2 = true;
				}
				flag2 &= musicSelectionLive.m_selectionRule.m_priority > m_currentlyPlaying.m_selectionRule.m_priority;
			}
			if ((m_stabilityTimer > 3f || musicSelectionLive.m_selectionRule.m_interruptAny) && flag2)
			{
				m_lastPlayed = musicSelectionLive;
				m_currentlyPlaying = musicSelectionLive;
				WingroveRoot.Instance.PostEvent("MUSIC_STOP_FAST");
				WingroveRoot.Instance.PostEvent(musicSelectionLive.m_selectionRule.m_cueToFire);
				m_beenPlayingFor = 0f;
				m_beenPlayingForLoopable = 0f;
			}
		}
		m_beenPlayingFor += Time.deltaTime;
		m_beenPlayingForLoopable += Time.deltaTime;
		m_timeInMission += Time.deltaTime;
	}
}
