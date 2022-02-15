using System;
using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class TopBarMissionTipsConstant : Singleton<TopBarMissionTipsConstant>
{
	public class TopBarMissionTip
	{
		public Func<bool> m_shouldShowEvalFunc;

		public string m_identifier;

		public float m_secondsShownForThisMission;

		public float m_secondsShownForEver;

		public bool m_isCurrentlyQueued;

		public TopBarInfoQueue.TopBarRequest m_request;

		public TopBarMissionTip(Func<bool> evalFunc, string id, TopBarInfoQueue.TopBarRequest req)
		{
			m_request = req;
			m_shouldShowEvalFunc = evalFunc;
			m_identifier = id;
		}

		public void SetSecondsShownForEver(float amt)
		{
			m_secondsShownForEver = amt;
		}

		public void AddShownForTime(float amt)
		{
			m_secondsShownForThisMission += amt;
		}

		public float GetSecondsShownFor()
		{
			return m_secondsShownForThisMission;
		}

		public void LiteUpdate(bool currentlyShown)
		{
			if (currentlyShown)
			{
				m_secondsShownForThisMission += Time.deltaTime;
			}
		}

		public void Remove()
		{
			if (m_isCurrentlyQueued)
			{
				Singleton<TopBarInfoQueue>.Instance.RemoveRequest(m_request);
				m_isCurrentlyQueued = false;
			}
		}

		public void Update(bool currentlyShown)
		{
			if (currentlyShown)
			{
				m_secondsShownForThisMission += Time.deltaTime;
			}
			if (!m_isCurrentlyQueued)
			{
				if (m_secondsShownForThisMission < 30f && m_secondsShownForEver < 90f && m_shouldShowEvalFunc())
				{
					Singleton<TopBarInfoQueue>.Instance.RegisterRequest(m_request);
					m_isCurrentlyQueued = true;
				}
			}
			else if (!m_shouldShowEvalFunc())
			{
				Singleton<TopBarInfoQueue>.Instance.RemoveRequest(m_request);
				m_isCurrentlyQueued = false;
			}
		}
	}

	private bool m_shouldDoSearchAndRescue;

	private CrewmanAvatar m_engineer;

	private CrewmanAvatar m_pilot;

	private List<TopBarMissionTip> m_allMissionTips = new List<TopBarMissionTip>();

	private bool m_isDisabled;

	private int m_frameCtr;

	private void OnEnable()
	{
		foreach (TopBarMissionTip allMissionTip in m_allMissionTips)
		{
			allMissionTip.SetSecondsShownForEver(Singleton<SaveDataContainer>.Instance.Get().GetTimeHintShownFor(allMissionTip.m_identifier));
		}
	}

	private void OnDisable()
	{
		if (!(Singleton<SaveDataContainer>.Instance != null))
		{
			return;
		}
		foreach (TopBarMissionTip allMissionTip in m_allMissionTips)
		{
			Singleton<SaveDataContainer>.Instance.Get().AddTimeHintShownFor(allMissionTip.m_identifier, allMissionTip.m_secondsShownForThisMission);
		}
	}

	private void Awake()
	{
		m_allMissionTips.Add(new TopBarMissionTip(CheckCrewTemperature, "crew_temperature", new TopBarInfoQueue.TopBarRequest("tutorial_maingame_topbar_temperature", null, isAlert: true, Singleton<EnumToIconMapping>.Instance.GetIconName(EnumToIconMapping.InteractionOrAlertType.Temperature), 15f, 15f, 15f, 1).AddSeeMore("ALTITUDE")));
		m_allMissionTips.Add(new TopBarMissionTip(CheckCrewOxygen, "crew_oxygen", new TopBarInfoQueue.TopBarRequest("tutorial_maingame_topbar_oxygen", null, isAlert: true, Singleton<EnumToIconMapping>.Instance.GetIconName(EnumToIconMapping.InteractionOrAlertType.Oxygen), 15f, 15f, 15f, 1).AddSeeMore("ALTITUDE")));
		m_allMissionTips.Add(new TopBarMissionTip(CheckCrewBleedOut, "crew_bleedout", new TopBarInfoQueue.TopBarRequest("tutorial_maingame_topbar_bleedout", null, isAlert: true, "CarriedItem_FirstAidKit", 15f, 15f, 15f, 2)));
		m_allMissionTips.Add(new TopBarMissionTip(CheckCrewDead, "crew_dead_any_station", new TopBarInfoQueue.TopBarRequest("tutorial_maingame_topbar_dead_any_station", null, isAlert: false, "Icon_Warning", 15f, 15f, 15f, 2)));
		m_allMissionTips.Add(new TopBarMissionTip(CheckElectricsOut, "bomber_electrics_out", new TopBarInfoQueue.TopBarRequest("tutorial_maingame_topbar_electrics_out_radar", null, isAlert: true, Singleton<EnumToIconMapping>.Instance.GetIconName(EnumToIconMapping.InteractionOrAlertType.Electrics), 15f, 15f, 15f, 1).AddSeeMore("ELECTRICS")));
		m_allMissionTips.Add(new TopBarMissionTip(CheckHydraulicsOut, "bomber_hydraulics_out", new TopBarInfoQueue.TopBarRequest("tutorial_maingame_topbar_hydraulics_out", null, isAlert: true, Singleton<EnumToIconMapping>.Instance.GetIconName(EnumToIconMapping.InteractionOrAlertType.Hydraulics), 15f, 15f, 15f, 1).AddSeeMore("HYDRAULICS")));
		m_allMissionTips.Add(new TopBarMissionTip(CheckOxygenOut, "bomber_oxygen_out", new TopBarInfoQueue.TopBarRequest("tutorial_maingame_topbar_oxygen_out", null, isAlert: true, Singleton<EnumToIconMapping>.Instance.GetIconName(EnumToIconMapping.InteractionOrAlertType.Oxygen), 15f, 15f, 15f, 2).AddSeeMore("OXYGEN")));
		m_allMissionTips.Add(new TopBarMissionTip(CheckElectricsUnreliable, "bomber_electrics_unreliable", new TopBarInfoQueue.TopBarRequest("tutorial_maingame_topbar_electrics_unreliable", null, isAlert: false, Singleton<EnumToIconMapping>.Instance.GetIconName(EnumToIconMapping.InteractionOrAlertType.Electrics), 5f, 15f, 15f, 1).AddSeeMore("ELECTRICS")));
		m_allMissionTips.Add(new TopBarMissionTip(CheckHydraulicsUnreliable, "bomber_hydraulics_unreliable", new TopBarInfoQueue.TopBarRequest("tutorial_maingame_topbar_hydraulics_unreliable", null, isAlert: false, Singleton<EnumToIconMapping>.Instance.GetIconName(EnumToIconMapping.InteractionOrAlertType.Hydraulics), 5f, 15f, 15f, 1).AddSeeMore("HYDRAULICS")));
		m_allMissionTips.Add(new TopBarMissionTip(CheckOxygenUnreliable, "bomber_oxygen_unreliable", new TopBarInfoQueue.TopBarRequest("tutorial_maingame_topbar_oxygen_unreliable", null, isAlert: false, Singleton<EnumToIconMapping>.Instance.GetIconName(EnumToIconMapping.InteractionOrAlertType.Oxygen), 5f, 15f, 15f, 2).AddSeeMore("OXYGEN")));
		m_allMissionTips.Add(new TopBarMissionTip(CheckUntaggedFightersAttacking, "bomber_fighters_untagged", new TopBarInfoQueue.TopBarRequest("tutorial_maingame_topbar_fighters_untagged", null, isAlert: true, Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.Gunning), 45f, 15f, 15f, 1).AddSeeMore("TAGGING")));
		m_allMissionTips.Add(new TopBarMissionTip(CheckSlowdownHint, "slowdown_hint", new TopBarInfoQueue.TopBarRequest("tutorial_maingame1_slowdown_pc", "tutorial_maingame1_slowdown_controller", isAlert: false, "Icon_SlowTime", 10f, 15f, 240f, 1)));
		m_allMissionTips.Add(new TopBarMissionTip(CheckIdleCrew, "bomber_crew_idle", new TopBarInfoQueue.TopBarRequest("tutorial_maingame_topbar_crew_idle", null, isAlert: false, "Icon_Warning", 30f, 15f, 15f, 1)));
		TopBarInfoQueue.TopBarRequest topBarRequest;
		if (!Singleton<GameFlow>.Instance.GetGameMode().GetUseUSNaming())
		{
			topBarRequest = new TopBarInfoQueue.TopBarRequest("tutorial_maingame_topbar_low_ammo", null, isAlert: false, "Icon_Ammo", 5f, 15f, 15f, 1);
			topBarRequest.AddPointerHint(Singleton<BomberSpawn>.Instance.GetBomberSystems().GetAmmoBox().GetInteractionItem()
				.gameObject, Vector3.zero, downArrow: true, worldSpace: true);
				m_allMissionTips.Add(new TopBarMissionTip(CheckLowAmmo, "bomber_low_ammo", topBarRequest.AddSeeMore("GUNNER")));
				m_allMissionTips.Add(new TopBarMissionTip(CheckLowAmmoShortcutSelected, "bomber_low_ammo_shortcut", new TopBarInfoQueue.TopBarRequest("tutorial_maingame1_shortcut_ammo_reload_pc", "tutorial_maingame1_shortcut_ammo_reload_controller", isAlert: false, "Icon_Ammo", 0f, 15f, 60f, 2)));
			}
			topBarRequest = new TopBarInfoQueue.TopBarRequest("tutorial_maingame_topbar_low_health", null, isAlert: false, "Icon_FirstAid", 30f, 15f, 15f, 1);
			topBarRequest.AddPointerHint(Singleton<BomberSpawn>.Instance.GetBomberSystems().GetStationFor(BomberSystems.StationType.MedicalBay).GetInteractionItem()
				.gameObject, Vector3.zero, downArrow: true, worldSpace: true);
				m_allMissionTips.Add(new TopBarMissionTip(CheckLowHealthCrew, "bomber_crew_low_health", topBarRequest.AddSeeMore("MEDICALBED")));
				m_allMissionTips.Add(new TopBarMissionTip(CheckFirstAidShortcutSelected, "bomber_first_aid_shortcut", new TopBarInfoQueue.TopBarRequest("tutorial_maingame1_shortcut_medical_bay", "tutorial_maingame1_shortcut_medical_bay_controller", isAlert: false, "Icon_FirstAid", 0f, 15f, 60f, 2)));
				m_allMissionTips.Add(new TopBarMissionTip(CheckForFire, "bomber_fire_hint", new TopBarInfoQueue.TopBarRequest("tutorial_maingame_topbar_fire_hint", null, isAlert: true, Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.FireFighting), 15f, 15f, 15f, 1)));
				m_allMissionTips.Add(new TopBarMissionTip(CheckForFireEngine, "bomber_fire_engine_hint", new TopBarInfoQueue.TopBarRequest("tutorial_maingame_topbar_fire_engine_hint", null, isAlert: true, Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.FireFighting), 15f, 15f, 15f, 1)));
				m_allMissionTips.Add(new TopBarMissionTip(CheckForFightersLowAltitude, "bomber_fighters_escape_altitude", new TopBarInfoQueue.TopBarRequest("tutorial_maingame_topbar_escape_fighters_high", null, isAlert: false, "Icon_Warning", 30f, 15f, 15f, 1).AddSeeMore("ALTITUDE")));
				m_allMissionTips.Add(new TopBarMissionTip(CheckForOptionalPhoto, "bomber_spotted_photo_opportunity", new TopBarInfoQueue.TopBarRequest("tutorial_maingame_topbar_optional_recon", null, isAlert: false, "UITagIconIntelPhoto", 10f, 15f, 15f, 1).AddSeeMore("PHOTOGRAPHS")));
				m_allMissionTips.Add(new TopBarMissionTip(CheckManyFighters, "bomber_lots_of_fighters", new TopBarInfoQueue.TopBarRequest("tutorial_maingame_topbar_all_guns", null, isAlert: false, Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.Gunning), 10f, 15f, 15f, 1)));
				m_allMissionTips.Add(new TopBarMissionTip(CheckEngineerDeadOrIncap, "crew_engineer_dead", new TopBarInfoQueue.TopBarRequest("tutorial_maingame_topbar_engineer_dead", null, isAlert: false, Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.Engineer), 15f, 15f, 15f, 1)));
				m_allMissionTips.Add(new TopBarMissionTip(CheckPilotDeadOrIncap, "crew_pilot_dead", new TopBarInfoQueue.TopBarRequest("tutorial_maingame_topbar_pilot_dead", null, isAlert: true, Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.Piloting), 1f, 15f, 15f, 3)));
				m_allMissionTips.Add(new TopBarMissionTip(TaggableOnRadar, "bomber_radar_blips", new TopBarInfoQueue.TopBarRequest("tutorial_maingame_topbar_radar_blips", null, isAlert: false, Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.RadioOp), 0f, 15f, 15f, 1).AddSeeMore("TAGGING")));
				m_allMissionTips.Add(new TopBarMissionTip(CheckGrandSlam, "tutorial_maingame_topbar_grandslam", new TopBarInfoQueue.TopBarRequest("tutorial_maingame_topbar_grandslam", null, isAlert: false, Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.BombAiming), 0f, 15f, 15f, 3)));
				m_allMissionTips.Add(new TopBarMissionTip(CheckSwappedRoles, "swapped_roles", new TopBarInfoQueue.TopBarRequest("tutorial_maingame_topbar_swapped_roles", null, isAlert: false, "Icon_Warning", 15f, 15f, 15f, 3)));
				m_allMissionTips.Add(new TopBarMissionTip(CheckPromptFocus, "gunner_focus", new TopBarInfoQueue.TopBarRequest("tutorial_maingame_topbar_skills_gunner_focus", null, isAlert: false, "Icon_Gunnery", 0f, 15f, 15f, 1)));
				m_allMissionTips.Add(new TopBarMissionTip(CheckPromptCorkscrew, "piloting_corkscrew", new TopBarInfoQueue.TopBarRequest("tutorial_maingame_topbar_skills_pilot_corkscrew", null, isAlert: false, "Icon_Piloting", 0f, 15f, 15f, 1)));
				if (!Application.isEditor)
				{
					return;
				}
				List<string> list = new List<string>();
				foreach (TopBarMissionTip allMissionTip in m_allMissionTips)
				{
					if (list.Contains(allMissionTip.m_identifier))
					{
					}
					list.Add(allMissionTip.m_identifier);
				}
			}

			private void Start()
			{
				foreach (CrewSpawner.CrewmanAvatarPairing item in Singleton<CrewSpawner>.Instance.GetAllCrew())
				{
					if (item.m_crewman.GetPrimarySkill().GetSkill() == Crewman.SpecialisationSkill.Engineer)
					{
						m_engineer = item.m_spawnedAvatar;
					}
					if (item.m_crewman.GetPrimarySkill().GetSkill() == Crewman.SpecialisationSkill.Piloting)
					{
						m_pilot = item.m_spawnedAvatar;
					}
				}
			}

			private bool ShouldDoSearchAndRescue()
			{
				return m_shouldDoSearchAndRescue;
			}

			public void SetSearchAndRescueRelevant(bool relevant)
			{
				m_shouldDoSearchAndRescue = relevant;
			}

			private bool CheckPromptFocus()
			{
				if (!Singleton<SaveDataContainer>.Instance.Get().HasEverFocused() && Singleton<FighterCoordinator>.Instance.AreAnyFightersTagged())
				{
					foreach (CrewSpawner.CrewmanAvatarPairing item in Singleton<CrewSpawner>.Instance.GetAllCrew())
					{
						if (item.m_crewman.GetPrimarySkill().GetSkill() == Crewman.SpecialisationSkill.Gunning && item.m_crewman.GetPrimarySkill().GetLevel() >= 3)
						{
							return true;
						}
					}
				}
				return false;
			}

			private bool CheckPromptCorkscrew()
			{
				if (!Singleton<SaveDataContainer>.Instance.Get().HasEverCorkscrewed() && Singleton<FighterCoordinator>.Instance.AreAnyFightersTagged())
				{
					float lastLowest = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetCriticalFlash().GetLastLowest();
					if (lastLowest < 0.7f && lastLowest > 0.05f)
					{
						foreach (CrewSpawner.CrewmanAvatarPairing item in Singleton<CrewSpawner>.Instance.GetAllCrew())
						{
							if (item.m_crewman.GetPrimarySkill().GetSkill() == Crewman.SpecialisationSkill.Piloting && item.m_crewman.GetPrimarySkill().GetLevel() >= 4)
							{
								return true;
							}
						}
					}
				}
				return false;
			}

			private bool CheckSwappedRoles()
			{
				foreach (CrewSpawner.CrewmanAvatarPairing item in Singleton<CrewSpawner>.Instance.GetAllCrew())
				{
					if (item.m_crewman.IsDead() || item.m_spawnedAvatar.GetStation() == null)
					{
						return false;
					}
					if (item.m_spawnedAvatar.GetStation() != null && item.m_spawnedAvatar.GetStation() is StationMedicalBed)
					{
						return false;
					}
				}
				foreach (CrewSpawner.CrewmanAvatarPairing item2 in Singleton<CrewSpawner>.Instance.GetAllCrew())
				{
					if (item2.m_crewman.GetPrimarySkill().GetSkill() != Crewman.SpecialisationSkill.RadioOp && (item2.m_crewman.GetSecondarySkill() == null || item2.m_crewman.GetSecondarySkill().GetSkill() != Crewman.SpecialisationSkill.RadioOp) && item2.m_spawnedAvatar.GetStation() is StationRadioOperator)
					{
						return true;
					}
					if (item2.m_crewman.GetPrimarySkill().GetSkill() != Crewman.SpecialisationSkill.Navigator && (item2.m_crewman.GetSecondarySkill() == null || item2.m_crewman.GetSecondarySkill().GetSkill() != Crewman.SpecialisationSkill.Navigator) && item2.m_spawnedAvatar.GetStation() is StationNavigator)
					{
						return true;
					}
					if (item2.m_crewman.GetPrimarySkill().GetSkill() != Crewman.SpecialisationSkill.BombAiming && (item2.m_crewman.GetSecondarySkill() == null || item2.m_crewman.GetSecondarySkill().GetSkill() != Crewman.SpecialisationSkill.BombAiming) && item2.m_spawnedAvatar.GetStation() is StationBombAimer)
					{
						return true;
					}
				}
				return false;
			}

			private bool CheckGrandSlam()
			{
				if (Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBombLoad().IsGrandSlam() && !Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBombLoad().HasDropped() && Singleton<MissionCoordinator>.Instance.IsOutwardJourney() && Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState().GetAltitudeTarget() < 2 && Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberNavigation().GetCurrentType() == BomberNavigation.NavigationPointType.FunctionLandingBombing)
				{
					return true;
				}
				return false;
			}

			private bool CheckEngineerDeadOrIncap()
			{
				if (m_engineer != null && (m_engineer.GetCrewman().IsDead() || m_engineer.GetHealthState().IsCountingDown()))
				{
					return true;
				}
				return false;
			}

			private bool CheckSlowdownHint()
			{
				if (Singleton<SaveDataContainer>.Instance.Get().GetNumMissionsPlayed() > 3 && Singleton<FighterCoordinator>.Instance.GetNumTaggableCurrently() > 3)
				{
					return true;
				}
				return false;
			}

			private bool TaggableOnRadar()
			{
				if (Singleton<SaveDataContainer>.Instance.Get().GetNumMissionsPlayed() > 3 && Singleton<FighterCoordinator>.Instance.GetNumTaggableCurrently() > 3)
				{
					return true;
				}
				return false;
			}

			private bool CheckPilotDeadOrIncap()
			{
				if (Singleton<BomberSpawn>.Instance.GetBomberSystems().GetStationFor(BomberSystems.StationType.Pilot).GetCurrentCrewman() == null)
				{
					return true;
				}
				return false;
			}

			private bool CheckManyFighters()
			{
				if (Singleton<SaveDataContainer>.Instance.Get().GetNumMissionsPlayed() > 4 && Singleton<FighterCoordinator>.Instance.GetNumTaggedCurrently() + Singleton<FighterCoordinator>.Instance.GetNumTaggableCurrently() > 5)
				{
					Station stationFor = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetStationFor(BomberSystems.StationType.GunsNose);
					Station stationFor2 = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetStationFor(BomberSystems.StationType.GunsLowerDeck);
					if (stationFor != null && stationFor.GetCurrentCrewman() == null)
					{
						return true;
					}
					if (stationFor2 != null && stationFor2.GetCurrentCrewman() == null)
					{
						return true;
					}
				}
				return false;
			}

			private bool CheckLowHealthCrew()
			{
				Station stationFor = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetStationFor(BomberSystems.StationType.MedicalBay);
				if (stationFor.GetCurrentCrewman() == null)
				{
					foreach (CrewSpawner.CrewmanAvatarPairing item in Singleton<CrewSpawner>.Instance.GetAllCrew())
					{
						if (!item.m_crewman.IsDead() && !item.m_spawnedAvatar.GetHealthState().IsCountingDown() && item.m_spawnedAvatar.GetHealthState().GetPhysicalHealthN() < 0.6f)
						{
							return true;
						}
					}
				}
				return false;
			}

			private bool CheckLowAmmo()
			{
				AmmoFeed[] ammoFeeds = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetAmmoFeeds();
				AmmoFeed[] array = ammoFeeds;
				foreach (AmmoFeed ammoFeed in array)
				{
					if (ammoFeed != null && ammoFeed.GetBelts() == 0 && ammoFeed.GetAmmo() < 200 && ammoFeed.HasBeenSet() && !ammoFeed.IsInfinite())
					{
						return true;
					}
				}
				return false;
			}

			private bool CheckLowAmmoShortcutSelected()
			{
				if (Singleton<ContextControl>.Instance.GetCurrentlySelected() != null)
				{
					Station station = Singleton<ContextControl>.Instance.GetCurrentlySelected().GetStation();
					if (station != null && station is StationGunner)
					{
						StationGunner stationGunner = (StationGunner)station;
						if (stationGunner.GetAmmoFeed().GetAmmo() < 200 && stationGunner.GetAmmoFeed().GetBelts() == 0 && !stationGunner.GetAmmoFeed().IsInfinite())
						{
							return true;
						}
					}
				}
				return false;
			}

			private bool CheckFirstAidShortcutSelected()
			{
				if (Singleton<ContextControl>.Instance.GetCurrentlySelected() != null && Singleton<ContextControl>.Instance.GetCurrentlySelected().GetHealthState().GetPhysicalHealthN() < 0.75f)
				{
					Station stationFor = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetStationFor(BomberSystems.StationType.MedicalBay);
					if (stationFor.GetCurrentCrewman() == null)
					{
						return true;
					}
				}
				return false;
			}

			private bool CheckForFire()
			{
				if (Singleton<BomberSpawn>.Instance.GetBomberSystems().GetFireOverview().GetAnyOnFire(engine: false))
				{
					return true;
				}
				return false;
			}

			private bool CheckForFireEngine()
			{
				if (Singleton<BomberSpawn>.Instance.GetBomberSystems().GetFireOverview().GetAnyOnFire(engine: true))
				{
					return true;
				}
				return false;
			}

			private bool CheckIdleCrew()
			{
				foreach (CrewSpawner.CrewmanAvatarPairing item in Singleton<CrewSpawner>.Instance.GetAllCrew())
				{
					if (item.m_spawnedAvatar.IsIdle())
					{
						return true;
					}
				}
				return false;
			}

			private bool CheckUntaggedFightersAttacking()
			{
				return Singleton<FighterCoordinator>.Instance.AreAnyUntaggedFightersEngaged();
			}

			private bool CheckForFightersLowAltitude()
			{
				if (Singleton<FighterCoordinator>.Instance.AreAnyFightersEngaged() && Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState().GetAltitudeNormalised() < 0.6f && !Singleton<MissionCoordinator>.Instance.IsOutwardJourney() && Singleton<SaveDataContainer>.Instance.Get().GetNumMissionsPlayed() > 3)
				{
					return true;
				}
				return false;
			}

			private bool CheckForOptionalPhoto()
			{
				return Singleton<MissionMapController>.Instance.HasSpottedOptionalReconRecently();
			}

			private bool CheckElectricsOut()
			{
				return Singleton<BomberSpawn>.Instance.GetBomberSystems().GetElectricalSystem().IsBroken();
			}

			private bool CheckElectricsUnreliable()
			{
				return Singleton<BomberSpawn>.Instance.GetBomberSystems().GetElectricalSystem().IsUnreliable();
			}

			private bool CheckHydraulicsOut()
			{
				return Singleton<BomberSpawn>.Instance.GetBomberSystems().GetHydraulics().IsBroken();
			}

			private bool CheckHydraulicsUnreliable()
			{
				return Singleton<BomberSpawn>.Instance.GetBomberSystems().GetHydraulics().IsUnreliable();
			}

			private bool CheckOxygenOut()
			{
				return Singleton<BomberSpawn>.Instance.GetBomberSystems().GetOxygenTank().IsBroken();
			}

			private bool CheckOxygenUnreliable()
			{
				return Singleton<BomberSpawn>.Instance.GetBomberSystems().GetOxygenTank().IsUnreliable();
			}

			private bool CheckCrewDead()
			{
				foreach (CrewSpawner.CrewmanAvatarPairing item in Singleton<CrewSpawner>.Instance.GetAllCrew())
				{
					if (item.m_spawnedAvatar.GetHealthState().IsDead())
					{
						return true;
					}
				}
				return false;
			}

			private bool CheckCrewBleedOut()
			{
				foreach (CrewSpawner.CrewmanAvatarPairing item in Singleton<CrewSpawner>.Instance.GetAllCrew())
				{
					if (item.m_spawnedAvatar.GetHealthState().IsCountingDown())
					{
						return true;
					}
				}
				return false;
			}

			private bool CheckCrewOxygen()
			{
				if (!Singleton<BomberSpawn>.Instance.GetBomberSystems().GetOxygenTank().IsBroken())
				{
					foreach (CrewSpawner.CrewmanAvatarPairing item in Singleton<CrewSpawner>.Instance.GetAllCrew())
					{
						if (!item.m_crewman.IsDead() && !item.m_spawnedAvatar.IsBailedOut() && item.m_spawnedAvatar.GetHealthState().AffectedByOxygen() && item.m_spawnedAvatar.GetHealthState().GetOxygenTemperatureN() > 0.2f)
						{
							return true;
						}
					}
				}
				return false;
			}

			private bool CheckCrewTemperature()
			{
				foreach (CrewSpawner.CrewmanAvatarPairing item in Singleton<CrewSpawner>.Instance.GetAllCrew())
				{
					if (!item.m_crewman.IsDead() && !item.m_spawnedAvatar.IsBailedOut() && item.m_spawnedAvatar.GetHealthState().AffectedByTemperature() && item.m_spawnedAvatar.GetHealthState().GetOxygenTemperatureN() > 0.2f)
					{
						return true;
					}
				}
				return false;
			}

			public void SetDisabledForMission(bool disabled)
			{
				m_isDisabled = disabled;
			}

			private void Update()
			{
				if (m_isDisabled)
				{
					return;
				}
				if (!Singleton<SystemDataContainer>.Instance.Get().BlockHints())
				{
					m_frameCtr++;
					int num = m_frameCtr;
					int num2 = 4;
					TopBarInfoQueue.TopBarRequest currentlyShownRequest = Singleton<TopBarInfoQueue>.Instance.GetCurrentlyShownRequest();
					{
						foreach (TopBarMissionTip allMissionTip in m_allMissionTips)
						{
							if (num % num2 == 0)
							{
								allMissionTip.Update(allMissionTip.m_request == currentlyShownRequest);
							}
							else
							{
								allMissionTip.LiteUpdate(allMissionTip.m_request == currentlyShownRequest);
							}
							num++;
						}
						return;
					}
				}
				foreach (TopBarMissionTip allMissionTip2 in m_allMissionTips)
				{
					allMissionTip2.Remove();
				}
			}
		}
