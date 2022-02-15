using System;
using System.Collections.Generic;
using System.Linq;
using BomberCrewCommon;
using UnityEngine;

public class CrewMiscSpeechTriggers : Singleton<CrewMiscSpeechTriggers>
{
	private class SpeechFuncReturn
	{
		public bool m_shouldFire;

		public bool m_usePosition;

		public Vector3 m_position;
	}

	public enum SpeechExternalTrigger
	{
		FighterDestroyed,
		BombTargetDestroyed,
		CrewmanIncapacitated,
		ReachedAltitudeLow,
		ReachedAltitudeMid,
		ReachedAltitudeHigh,
		EngineSetOnFire,
		SubmarineSubmergeTagged,
		AcePilotGreeting,
		CriticalDamageFlash,
		FuelTankLeak,
		EnginesRemainTwo,
		EnginesRemainOne,
		V1LaunchStarts,
		V1LaunchHappens,
		RadarSiteStart,
		RadarSiteFinish,
		FlakAheadLowAltitude,
		FlakAheadMidAltitude,
		FlakAheadBelow,
		FlakTargetedAppear,
		FlakTargetedClose,
		EnginesRemainNone,
		ManouevreStart,
		HeadingConfirmed,
		BombBayDoorsOpen,
		BombTargetTagged,
		AcePilotTagged,
		FighterDestroyedAce,
		V1Destroyed,
		V2Destroyed
	}

	private class SpeechExternalTriggerCtr
	{
		public SpeechExternalTrigger m_forTrigger;

		public float m_timeSinceTriggered;

		public SpeechPrioritiser.SpeechRequestTemplate m_speechToFire;

		public Crewman.SpecialisationSkill[] m_allowedSkillTypes;

		public BomberSystems.StationType[] m_allowedStations;

		public bool m_useTriggerer;

		public bool m_dontUseTriggerer;

		public SpeechExternalTriggerCtr(SpeechExternalTrigger trigger, Crewman.SpecialisationSkill[] skills, BomberSystems.StationType[] stations, SpeechPrioritiser.SpeechRequestTemplate srt, bool useTriggerer, bool dontUseTriggerer)
		{
			m_forTrigger = trigger;
			m_speechToFire = srt;
			m_allowedSkillTypes = skills;
			m_allowedStations = stations;
			m_useTriggerer = useTriggerer;
			m_dontUseTriggerer = dontUseTriggerer;
		}
	}

	private class SpeechTrigger
	{
		public Func<SpeechFuncReturn> m_shouldFireSpeech;

		public Crewman.SpecialisationSkill[] m_allowedSkillTypes;

		public bool m_isCurrentlyActive;

		public SpeechPrioritiser.SpeechRequestTemplate m_speechToFire;

		public BomberSystems.StationType[] m_allowedStations;

		public SpeechTrigger(Func<SpeechFuncReturn> function, Crewman.SpecialisationSkill[] skills, BomberSystems.StationType[] stations, SpeechPrioritiser.SpeechRequestTemplate srt)
		{
			m_speechToFire = srt;
			m_allowedStations = stations;
			m_shouldFireSpeech = function;
			m_allowedSkillTypes = skills;
		}
	}

	[SerializeField]
	private BomberSystems m_bomberSystems;

	private List<SpeechTrigger> m_allSpeechTriggers = new List<SpeechTrigger>();

	private List<SpeechExternalTriggerCtr> m_speechExternalCounters = new List<SpeechExternalTriggerCtr>();

	private Dictionary<SpeechExternalTrigger, List<SpeechExternalTriggerCtr>> m_fastLookUp = new Dictionary<SpeechExternalTrigger, List<SpeechExternalTriggerCtr>>();

	private void Start()
	{
		SpeechTrigger item = new SpeechTrigger(FireSpotted, null, null, StaticSpeechSets.FireSpotted);
		m_allSpeechTriggers.Add(item);
		SpeechTrigger item2 = new SpeechTrigger(FireSpottedLarge, null, null, StaticSpeechSets.FireSpottedLarge);
		m_allSpeechTriggers.Add(item2);
		SpeechTrigger speechTrigger = new SpeechTrigger(NoPilot, null, null, StaticSpeechSets.NoPilot);
		AddExternalTrigger(new SpeechExternalTriggerCtr(SpeechExternalTrigger.FighterDestroyed, null, null, StaticSpeechSets.FighterDestroyed, useTriggerer: true, dontUseTriggerer: false));
		AddExternalTrigger(new SpeechExternalTriggerCtr(SpeechExternalTrigger.FighterDestroyedAce, null, null, StaticSpeechSets.FighterDestroyedAce, useTriggerer: true, dontUseTriggerer: false));
		AddExternalTrigger(new SpeechExternalTriggerCtr(SpeechExternalTrigger.V1Destroyed, null, null, StaticSpeechSets.V1Destroyed, useTriggerer: true, dontUseTriggerer: false));
		AddExternalTrigger(new SpeechExternalTriggerCtr(SpeechExternalTrigger.V2Destroyed, null, null, StaticSpeechSets.V2Destroyed, useTriggerer: true, dontUseTriggerer: false));
		AddExternalTrigger(new SpeechExternalTriggerCtr(SpeechExternalTrigger.FighterDestroyed, null, null, StaticSpeechSets.FighterDestroyedCompliment, useTriggerer: false, dontUseTriggerer: true));
		AddExternalTrigger(new SpeechExternalTriggerCtr(SpeechExternalTrigger.BombTargetDestroyed, null, new BomberSystems.StationType[1] { BomberSystems.StationType.BombAimer }, StaticSpeechSets.BombTargetDestroyed, useTriggerer: false, dontUseTriggerer: false));
		AddExternalTrigger(new SpeechExternalTriggerCtr(SpeechExternalTrigger.ReachedAltitudeLow, null, new BomberSystems.StationType[1], StaticSpeechSets.ReachedAltitudeLow, useTriggerer: false, dontUseTriggerer: false));
		AddExternalTrigger(new SpeechExternalTriggerCtr(SpeechExternalTrigger.ReachedAltitudeMid, null, new BomberSystems.StationType[1], StaticSpeechSets.ReachedAltitudeMid, useTriggerer: false, dontUseTriggerer: false));
		AddExternalTrigger(new SpeechExternalTriggerCtr(SpeechExternalTrigger.ReachedAltitudeHigh, null, new BomberSystems.StationType[1], StaticSpeechSets.ReachedAltitudeHigh, useTriggerer: false, dontUseTriggerer: false));
		AddExternalTrigger(new SpeechExternalTriggerCtr(SpeechExternalTrigger.CrewmanIncapacitated, null, null, StaticSpeechSets.CrewmanIncapacitatedSelf, useTriggerer: true, dontUseTriggerer: false));
		AddExternalTrigger(new SpeechExternalTriggerCtr(SpeechExternalTrigger.CrewmanIncapacitated, null, null, StaticSpeechSets.CrewmanIncapacitatedOther, useTriggerer: false, dontUseTriggerer: true));
		AddExternalTrigger(new SpeechExternalTriggerCtr(SpeechExternalTrigger.CriticalDamageFlash, null, null, StaticSpeechSets.CriticalDamageReached, useTriggerer: false, dontUseTriggerer: false));
		AddExternalTrigger(new SpeechExternalTriggerCtr(SpeechExternalTrigger.EnginesRemainOne, null, null, StaticSpeechSets.EnginesRemainOne, useTriggerer: false, dontUseTriggerer: false));
		AddExternalTrigger(new SpeechExternalTriggerCtr(SpeechExternalTrigger.EnginesRemainTwo, null, null, StaticSpeechSets.EnginesRemainTwo, useTriggerer: false, dontUseTriggerer: false));
		AddExternalTrigger(new SpeechExternalTriggerCtr(SpeechExternalTrigger.EnginesRemainNone, null, null, StaticSpeechSets.EnginesRemainNone, useTriggerer: false, dontUseTriggerer: false));
		AddExternalTrigger(new SpeechExternalTriggerCtr(SpeechExternalTrigger.FlakAheadBelow, null, null, StaticSpeechSets.FlakAheadBelow, useTriggerer: false, dontUseTriggerer: false));
		AddExternalTrigger(new SpeechExternalTriggerCtr(SpeechExternalTrigger.FlakAheadLowAltitude, null, null, StaticSpeechSets.FlakAheadLowAltitude, useTriggerer: false, dontUseTriggerer: false));
		AddExternalTrigger(new SpeechExternalTriggerCtr(SpeechExternalTrigger.FlakAheadMidAltitude, null, null, StaticSpeechSets.FlakAheadLowAltitude, useTriggerer: false, dontUseTriggerer: false));
		AddExternalTrigger(new SpeechExternalTriggerCtr(SpeechExternalTrigger.V1LaunchStarts, null, null, StaticSpeechSets.V1LaunchStarts, useTriggerer: false, dontUseTriggerer: false));
		AddExternalTrigger(new SpeechExternalTriggerCtr(SpeechExternalTrigger.V1LaunchHappens, null, null, StaticSpeechSets.V1LaunchHappens, useTriggerer: false, dontUseTriggerer: false));
		AddExternalTrigger(new SpeechExternalTriggerCtr(SpeechExternalTrigger.RadarSiteFinish, null, new BomberSystems.StationType[3]
		{
			BomberSystems.StationType.Navigation,
			BomberSystems.StationType.RadioOperator,
			BomberSystems.StationType.Pilot
		}, StaticSpeechSets.RadarSiteFinish, useTriggerer: false, dontUseTriggerer: false));
		AddExternalTrigger(new SpeechExternalTriggerCtr(SpeechExternalTrigger.RadarSiteStart, null, new BomberSystems.StationType[3]
		{
			BomberSystems.StationType.Navigation,
			BomberSystems.StationType.RadioOperator,
			BomberSystems.StationType.Pilot
		}, StaticSpeechSets.RadarSiteStart, useTriggerer: false, dontUseTriggerer: false));
		AddExternalTrigger(new SpeechExternalTriggerCtr(SpeechExternalTrigger.FuelTankLeak, null, new BomberSystems.StationType[2]
		{
			BomberSystems.StationType.Pilot,
			BomberSystems.StationType.FlightEngineer
		}, StaticSpeechSets.FuelTankLeak, useTriggerer: false, dontUseTriggerer: false));
		AddExternalTrigger(new SpeechExternalTriggerCtr(SpeechExternalTrigger.FuelTankLeak, new Crewman.SpecialisationSkill[2]
		{
			Crewman.SpecialisationSkill.Engineer,
			Crewman.SpecialisationSkill.Piloting
		}, null, StaticSpeechSets.FuelTankLeak, useTriggerer: false, dontUseTriggerer: false));
		AddExternalTrigger(new SpeechExternalTriggerCtr(SpeechExternalTrigger.SubmarineSubmergeTagged, null, new BomberSystems.StationType[1] { BomberSystems.StationType.BombAimer }, StaticSpeechSets.SubmarineSubmerging, useTriggerer: false, dontUseTriggerer: false));
		AddExternalTrigger(new SpeechExternalTriggerCtr(SpeechExternalTrigger.BombBayDoorsOpen, null, new BomberSystems.StationType[1] { BomberSystems.StationType.BombAimer }, StaticSpeechSets.BombBayDoorsOpen, useTriggerer: false, dontUseTriggerer: false));
		AddExternalTrigger(new SpeechExternalTriggerCtr(SpeechExternalTrigger.HeadingConfirmed, null, new BomberSystems.StationType[1], StaticSpeechSets.HeadingConfirmed, useTriggerer: false, dontUseTriggerer: false));
		AddExternalTrigger(new SpeechExternalTriggerCtr(SpeechExternalTrigger.BombTargetTagged, null, new BomberSystems.StationType[1], StaticSpeechSets.BombRunStarted, useTriggerer: false, dontUseTriggerer: false));
		AddExternalTrigger(new SpeechExternalTriggerCtr(SpeechExternalTrigger.ManouevreStart, null, new BomberSystems.StationType[1], StaticSpeechSets.ManouevreStart, useTriggerer: false, dontUseTriggerer: false));
		AddExternalTrigger(new SpeechExternalTriggerCtr(SpeechExternalTrigger.AcePilotTagged, null, new BomberSystems.StationType[4]
		{
			BomberSystems.StationType.GunsLowerDeck,
			BomberSystems.StationType.GunsNose,
			BomberSystems.StationType.GunsTail,
			BomberSystems.StationType.GunsUpperDeck
		}, StaticSpeechSets.AcePilotTagged, useTriggerer: false, dontUseTriggerer: false));
	}

	private void AddExternalTrigger(SpeechExternalTriggerCtr ctr)
	{
		m_speechExternalCounters.Add(ctr);
		List<SpeechExternalTriggerCtr> value = null;
		m_fastLookUp.TryGetValue(ctr.m_forTrigger, out value);
		if (value == null)
		{
			value = new List<SpeechExternalTriggerCtr>();
			m_fastLookUp[ctr.m_forTrigger] = value;
		}
		value.Add(ctr);
	}

	public void DoExternalTrigger(SpeechExternalTrigger set, CrewmanAvatar fromCrewmanOptional)
	{
		List<SpeechExternalTriggerCtr> value = null;
		m_fastLookUp.TryGetValue(set, out value);
		if (value == null || value.Count <= 0)
		{
			return;
		}
		int count = value.Count;
		int num = UnityEngine.Random.Range(0, count);
		int num2 = 0;
		SpeechExternalTriggerCtr speechExternalTriggerCtr = null;
		while (true)
		{
			if (!m_fastLookUp[set][num2].m_useTriggerer || fromCrewmanOptional != null)
			{
				if (num == 0)
				{
					break;
				}
				num--;
			}
			num2 = (num2 + 1) % count;
		}
		speechExternalTriggerCtr = m_fastLookUp[set][num2];
		if (speechExternalTriggerCtr == null || !(speechExternalTriggerCtr.m_timeSinceTriggered > 5f))
		{
			return;
		}
		foreach (SpeechExternalTriggerCtr item in value)
		{
			item.m_timeSinceTriggered = 0f;
		}
		CrewmanAvatar crewmanAvatar = null;
		crewmanAvatar = ((!speechExternalTriggerCtr.m_useTriggerer) ? GetCrewmanFor(speechExternalTriggerCtr.m_allowedSkillTypes, speechExternalTriggerCtr.m_allowedStations, (!speechExternalTriggerCtr.m_dontUseTriggerer) ? null : fromCrewmanOptional) : fromCrewmanOptional);
		if (crewmanAvatar != null)
		{
			Singleton<SpeechPrioritiser>.Instance.RequestNewSpeech(speechExternalTriggerCtr.m_speechToFire, crewmanAvatar, fromCrewmanOptional);
		}
	}

	private SpeechFuncReturn NoPilot()
	{
		SpeechFuncReturn speechFuncReturn = new SpeechFuncReturn();
		if (m_bomberSystems.GetStationFor(BomberSystems.StationType.Pilot).GetCurrentCrewman() == null)
		{
			speechFuncReturn.m_usePosition = false;
			speechFuncReturn.m_shouldFire = true;
		}
		return speechFuncReturn;
	}

	private SpeechFuncReturn FireSpotted()
	{
		SpeechFuncReturn speechFuncReturn = new SpeechFuncReturn();
		FireOverview.Extinguishable biggestFire = m_bomberSystems.GetFireOverview().GetBiggestFire(engine: false);
		if (biggestFire != null)
		{
			speechFuncReturn.m_position = biggestFire.GetTransform().position;
			speechFuncReturn.m_usePosition = true;
			speechFuncReturn.m_shouldFire = true;
		}
		return speechFuncReturn;
	}

	private SpeechFuncReturn FireSpottedLarge()
	{
		SpeechFuncReturn speechFuncReturn = new SpeechFuncReturn();
		FireOverview.Extinguishable biggestFire = m_bomberSystems.GetFireOverview().GetBiggestFire(engine: false);
		if (biggestFire != null && biggestFire.GetFireIntensityNormalised() > 0.75f)
		{
			speechFuncReturn.m_position = biggestFire.GetTransform().position;
			speechFuncReturn.m_usePosition = true;
			speechFuncReturn.m_shouldFire = true;
		}
		return speechFuncReturn;
	}

	private bool CheckEligibility(Crewman.SpecialisationSkill[] allowedSkills, BomberSystems.StationType[] stationTypes, CrewSpawner.CrewmanAvatarPairing cap)
	{
		if (allowedSkills == null || allowedSkills.Contains(cap.m_crewman.GetPrimarySkill().GetSkill()) || (cap.m_crewman.GetSecondarySkill() != null && allowedSkills.Contains(cap.m_crewman.GetSecondarySkill().GetSkill())))
		{
			if (stationTypes == null)
			{
				return true;
			}
			if (cap.m_spawnedAvatar.HasStation())
			{
				Station station = cap.m_spawnedAvatar.GetStation();
				foreach (BomberSystems.StationType station2 in stationTypes)
				{
					if (Singleton<BomberSpawn>.Instance.GetBomberSystems().GetStationFor(station2) == station)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	private CrewmanAvatar GetCrewmanFor(Crewman.SpecialisationSkill[] allowedSkills, BomberSystems.StationType[] stationTypes, CrewmanAvatar avoidCrewman)
	{
		List<CrewmanAvatar> list = new List<CrewmanAvatar>();
		foreach (CrewSpawner.CrewmanAvatarPairing item in Singleton<CrewSpawner>.Instance.GetAllCrew())
		{
			if (item.m_spawnedAvatar != avoidCrewman && !item.m_crewman.IsDead() && !item.m_spawnedAvatar.GetHealthState().IsCountingDown() && CheckEligibility(allowedSkills, stationTypes, item))
			{
				list.Add(item.m_spawnedAvatar);
			}
		}
		if (list.Count > 0)
		{
			return list[UnityEngine.Random.Range(0, list.Count)];
		}
		return null;
	}

	private CrewmanAvatar GetCrewmanFor(SpeechTrigger genTrigger, SpeechFuncReturn sr, CrewmanAvatar avoidCrewman)
	{
		if (sr.m_usePosition)
		{
			float num = float.MaxValue;
			CrewmanAvatar result = null;
			{
				foreach (CrewSpawner.CrewmanAvatarPairing item in Singleton<CrewSpawner>.Instance.GetAllCrew())
				{
					if (item.m_spawnedAvatar != avoidCrewman && !item.m_crewman.IsDead() && !item.m_spawnedAvatar.GetHealthState().IsCountingDown() && CheckEligibility(genTrigger.m_allowedSkillTypes, genTrigger.m_allowedStations, item))
					{
						Vector3 vector = sr.m_position - item.m_spawnedAvatar.transform.position;
						if (vector.magnitude < num)
						{
							num = vector.magnitude;
							result = item.m_spawnedAvatar;
						}
					}
				}
				return result;
			}
		}
		List<CrewmanAvatar> list = new List<CrewmanAvatar>();
		foreach (CrewSpawner.CrewmanAvatarPairing item2 in Singleton<CrewSpawner>.Instance.GetAllCrew())
		{
			if (item2.m_spawnedAvatar != avoidCrewman && !item2.m_crewman.IsDead() && !item2.m_spawnedAvatar.GetHealthState().IsCountingDown() && CheckEligibility(genTrigger.m_allowedSkillTypes, genTrigger.m_allowedStations, item2))
			{
				list.Add(item2.m_spawnedAvatar);
			}
		}
		if (list.Count > 0)
		{
			return list[UnityEngine.Random.Range(0, list.Count)];
		}
		return null;
	}

	private void Update()
	{
		foreach (SpeechTrigger allSpeechTrigger in m_allSpeechTriggers)
		{
			SpeechFuncReturn speechFuncReturn = allSpeechTrigger.m_shouldFireSpeech();
			if (speechFuncReturn.m_shouldFire == allSpeechTrigger.m_isCurrentlyActive)
			{
				continue;
			}
			if (speechFuncReturn.m_shouldFire)
			{
				CrewmanAvatar crewmanFor = GetCrewmanFor(allSpeechTrigger, speechFuncReturn, null);
				if (crewmanFor != null)
				{
					Singleton<SpeechPrioritiser>.Instance.RequestNewSpeech(allSpeechTrigger.m_speechToFire, crewmanFor);
					allSpeechTrigger.m_isCurrentlyActive = speechFuncReturn.m_shouldFire;
				}
			}
			else
			{
				allSpeechTrigger.m_isCurrentlyActive = speechFuncReturn.m_shouldFire;
			}
		}
		foreach (SpeechExternalTriggerCtr speechExternalCounter in m_speechExternalCounters)
		{
			speechExternalCounter.m_timeSinceTriggered += Time.deltaTime;
		}
	}
}
