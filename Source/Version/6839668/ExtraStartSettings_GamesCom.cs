using System;
using System.Collections;
using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class ExtraStartSettings_GamesCom : MonoBehaviour
{
	[SerializeField]
	private GameObject m_miniPromptMove;

	[SerializeField]
	private GameObject m_miniPromptSelect;

	[SerializeField]
	private GameObject m_miniPromptZoomCamera;

	private bool m_hasEverTagged;

	private GameObject m_currentMiniPrompt;

	private GameObject m_currentMiniPromptPrefab;

	private void DoMiniPrompt(GameObject go)
	{
		if (m_currentMiniPromptPrefab != go)
		{
			if (m_currentMiniPrompt != null)
			{
				UnityEngine.Object.Destroy(m_currentMiniPrompt);
			}
			if (Singleton<UISelector>.Instance.IsPrimary())
			{
				m_currentMiniPrompt = UnityEngine.Object.Instantiate(go);
				m_currentMiniPrompt.transform.position = Vector3.zero;
				m_currentMiniPromptPrefab = go;
			}
		}
	}

	private void ClearMiniPrompt()
	{
		m_currentMiniPromptPrefab = null;
		if (m_currentMiniPrompt != null)
		{
			UnityEngine.Object.Destroy(m_currentMiniPrompt);
		}
	}

	private IEnumerator Start()
	{
		Singleton<DifficultyMagic>.Instance.SetDisabled();
		BomberSystems bs = Singleton<BomberSpawn>.Instance.GetBomberSystems();
		Singleton<BomberSpawn>.Instance.GetBomberSystems().GetElectricalSystem().SetInvincible();
		Singleton<BomberSpawn>.Instance.GetBomberSystems().GetOxygenTank().SetInvincible();
		Singleton<BomberSpawn>.Instance.GetBomberSystems().GetTemperatureOxygen().SetInactive(inactive: true);
		Singleton<BomberSpawn>.Instance.GetBomberSystems().GetHydraulics().SetInvincible();
		Singleton<MissionSpeedControls>.Instance.SetBlocked(blocked: true);
		Singleton<BomberSpawn>.Instance.GetBomberSystems().GetFuelTank(0).SetDontUse(dontuse: true);
		Singleton<BomberSpawn>.Instance.GetBomberSystems().GetFuelTank(1).SetDontUse(dontuse: true);
		Singleton<TopBarMissionTipsConstant>.Instance.SetDisabledForMission(disabled: true);
		yield return null;
		List<CrewSpawner.CrewmanAvatarPairing> capList = Singleton<CrewSpawner>.Instance.GetAllCrew();
		foreach (CrewSpawner.CrewmanAvatarPairing item in capList)
		{
			item.m_spawnedAvatar.SetOrdersBlocked(blocked: true);
			if (item.m_crewman.GetPrimarySkill().GetSkill() == Crewman.SpecialisationSkill.Piloting)
			{
				item.m_spawnedAvatar.SetMoveOrdersBlocked(blocked: true);
			}
		}
		((StationPilot)Singleton<BomberSpawn>.Instance.GetBomberSystems().GetStationFor(BomberSystems.StationType.Pilot)).SetInactiveLocked(inactiveLocked: true);
		((StationNavigator)Singleton<BomberSpawn>.Instance.GetBomberSystems().GetStationFor(BomberSystems.StationType.Navigation)).SetInactiveLocked(inactiveLocked: true);
		AmmoFeed[] ammoFeeds = UnityEngine.Object.FindObjectsOfType<AmmoFeed>();
		AmmoFeed[] array = ammoFeeds;
		foreach (AmmoFeed ammoFeed in array)
		{
			ammoFeed.SetInfiniteTutorial(infinite: true);
		}
		AmmoBeltBox abb = UnityEngine.Object.FindObjectOfType<AmmoBeltBox>();
		abb.SetInteractive(interactive: false);
		Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState().SetBounds(new Vector3(-500f, 0f, -1500f), new Vector3(5500f, 0f, 1500f));
		StartCoroutine(DoChecks());
		StartCoroutine(WaitForTakeOff());
	}

	private IEnumerator DoChecks()
	{
		while (true)
		{
			if (Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberNavigation().GetNextNavigationPoint() != null)
			{
				if (!m_hasEverTagged)
				{
					Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState().SetBoundsDefaults();
				}
				m_hasEverTagged = true;
			}
			yield return null;
		}
	}

	private IEnumerator DoCameraControlTutorial()
	{
		TopBarInfoQueue.TopBarRequest zoomRequest = TopBarInfoQueue.TopBarRequest.Standard("tutorial_topbar_camera_zoom_pc", "tutorial_topbar_camera_zoom_controller", Singleton<EnumToIconMapping>.Instance.GetIconName(EnumToIconMapping.InteractionOrAlertType.Visibility), 0f, 7.5f, 7.5f, 1);
		TopBarInfoQueue.TopBarRequest zoomRequest2 = TopBarInfoQueue.TopBarRequest.Standard("tutorial_topbar_camera_zoom_pc_2", "tutorial_topbar_camera_zoom_controller_2", Singleton<EnumToIconMapping>.Instance.GetIconName(EnumToIconMapping.InteractionOrAlertType.Visibility), 0f, 7.5f, 7.5f, 1);
		Singleton<TopBarInfoQueue>.Instance.RegisterRequest(zoomRequest);
		Singleton<TopBarInfoQueue>.Instance.RegisterRequest(zoomRequest2);
		float timer2 = 0f;
		while (!Singleton<BomberCamera>.Instance.HasZoomedEver() || !(timer2 > 5f))
		{
			if (timer2 > 7.5f)
			{
				DoMiniPrompt(m_miniPromptZoomCamera);
			}
			if (timer2 > 15f)
			{
				break;
			}
			timer2 += Time.deltaTime;
			yield return null;
		}
		ClearMiniPrompt();
		Singleton<TopBarInfoQueue>.Instance.RemoveRequest(zoomRequest);
		Singleton<TopBarInfoQueue>.Instance.RemoveRequest(zoomRequest2);
		TopBarInfoQueue.TopBarRequest cameraMoveRequest = TopBarInfoQueue.TopBarRequest.Standard("tutorial_topbar_camera_move_pc", "tutorial_topbar_camera_move_controller", Singleton<EnumToIconMapping>.Instance.GetIconName(EnumToIconMapping.InteractionOrAlertType.Visibility), 0f, 0f, 0f, 1);
		Singleton<TopBarInfoQueue>.Instance.RegisterRequest(cameraMoveRequest);
		((StationPilot)Singleton<BomberSpawn>.Instance.GetBomberSystems().GetStationFor(BomberSystems.StationType.Pilot)).SetInactiveLocked(inactiveLocked: false);
		((StationNavigator)Singleton<BomberSpawn>.Instance.GetBomberSystems().GetStationFor(BomberSystems.StationType.Navigation)).SetInactiveLocked(inactiveLocked: false);
		timer2 = 0f;
		while ((!Singleton<BomberCamera>.Instance.HasMovedEver() || !(timer2 > 10f)) && !(timer2 > 15f))
		{
			timer2 += Time.deltaTime;
			yield return null;
		}
		Singleton<TopBarInfoQueue>.Instance.RemoveRequest(cameraMoveRequest);
		StartCoroutine(DoNavigationTag());
	}

	private IEnumerator WaitForTakeOff()
	{
		yield return new WaitForSeconds(6f);
		List<CrewSpawner.CrewmanAvatarPairing> capList = Singleton<CrewSpawner>.Instance.GetAllCrew();
		foreach (CrewSpawner.CrewmanAvatarPairing item in capList)
		{
			item.m_spawnedAvatar.SetOrdersBlocked(blocked: false);
			item.m_spawnedAvatar.SetMoveOrdersBlocked(blocked: true);
		}
		TopBarInfoQueue.TopBarRequest takeOffRequest = TopBarInfoQueue.TopBarRequest.Standard("tutorial_topbar_bombrun_takeoff", null, Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.Piloting), 0f, 0f, 0f, 1);
		TopBarInfoQueue.TopBarRequest takeOffRequestSelect = TopBarInfoQueue.TopBarRequest.Standard("tutorial_topbar_bombrun_takeoff_select_long", "tutorial_topbar_bombrun_takeoff_select_console", Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.Piloting), 0f, 0f, 0f, 1);
		CrewSpawner.CrewmanAvatarPairing pilot = null;
		foreach (CrewSpawner.CrewmanAvatarPairing item2 in capList)
		{
			if (item2.m_crewman.GetPrimarySkill().GetSkill() == Crewman.SpecialisationSkill.Piloting)
			{
				pilot = item2;
			}
		}
		takeOffRequestSelect.AddPointerHint(pilot.m_spawnedAvatar.GetCrewmanGraphics().GetPelvisTransform().gameObject, Vector3.zero);
		takeOffRequestSelect.AddPointerHint(Singleton<CrewPopulator>.Instance.GetPanelFor(Singleton<CrewSpawner>.Instance.GetIndexFromCrewman(pilot.m_spawnedAvatar)), new Vector3(96f, 40f, 0f), downArrow: true, worldSpace: false);
		takeOffRequest.AddPointerHint("TAKEOFFBUTTON", Vector3.zero, downArrow: true, worldSpace: false);
		float selectTimer = 0f;
		while (Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState().HasNotCompletedTakeOff())
		{
			selectTimer += Time.deltaTime;
			bool showMiniPrompt = false;
			if (Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState().HasNotStartedTakeOff())
			{
				if (Singleton<ContextControl>.Instance.GetCurrentlySelected() != pilot.m_spawnedAvatar)
				{
					Singleton<TopBarInfoQueue>.Instance.RemoveRequest(takeOffRequest);
					Singleton<TopBarInfoQueue>.Instance.RegisterRequest(takeOffRequestSelect);
					if (selectTimer > 7.5f)
					{
						showMiniPrompt = true;
					}
				}
				else
				{
					Singleton<TopBarInfoQueue>.Instance.RegisterRequest(takeOffRequest);
					Singleton<TopBarInfoQueue>.Instance.RemoveRequest(takeOffRequestSelect);
				}
			}
			else
			{
				Singleton<TopBarInfoQueue>.Instance.RemoveRequest(takeOffRequest);
				Singleton<TopBarInfoQueue>.Instance.RemoveRequest(takeOffRequestSelect);
			}
			if (showMiniPrompt)
			{
				DoMiniPrompt(m_miniPromptSelect);
			}
			else
			{
				ClearMiniPrompt();
			}
			yield return null;
		}
		ClearMiniPrompt();
		Singleton<TopBarInfoQueue>.Instance.RemoveRequest(takeOffRequest);
		Singleton<TopBarInfoQueue>.Instance.RemoveRequest(takeOffRequestSelect);
		yield return new WaitForSeconds(2f);
		TopBarInfoQueue.TopBarRequest landingGearUpRequest = TopBarInfoQueue.TopBarRequest.Standard("tutorial_topbar_landing_gear_up", null, Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.Piloting), 0f, 0f, 0f, 1);
		landingGearUpRequest.AddPointerHint("RAISEGEAR", Vector3.zero, downArrow: true, worldSpace: false);
		while (!Singleton<BomberSpawn>.Instance.GetBomberSystems().GetLandingGear().IsFullyRaised())
		{
			if (!Singleton<BomberSpawn>.Instance.GetBomberSystems().GetLandingGear().IsRaising())
			{
				if (Singleton<ContextControl>.Instance.GetCurrentlySelected() != pilot.m_spawnedAvatar)
				{
					Singleton<TopBarInfoQueue>.Instance.RegisterRequest(takeOffRequestSelect);
					Singleton<TopBarInfoQueue>.Instance.RemoveRequest(landingGearUpRequest);
				}
				else
				{
					Singleton<TopBarInfoQueue>.Instance.RegisterRequest(landingGearUpRequest);
					Singleton<TopBarInfoQueue>.Instance.RemoveRequest(takeOffRequestSelect);
				}
			}
			else
			{
				Singleton<TopBarInfoQueue>.Instance.RemoveRequest(takeOffRequestSelect);
				Singleton<TopBarInfoQueue>.Instance.RemoveRequest(landingGearUpRequest);
			}
			yield return null;
		}
		Singleton<TopBarInfoQueue>.Instance.RemoveRequest(landingGearUpRequest);
		yield return new WaitForSeconds(3f);
		foreach (CrewSpawner.CrewmanAvatarPairing item3 in capList)
		{
			item3.m_spawnedAvatar.SetOrdersBlocked(blocked: false);
			item3.m_spawnedAvatar.SetMoveOrdersBlocked(blocked: false);
			if (item3.m_crewman.GetPrimarySkill().GetSkill() == Crewman.SpecialisationSkill.Piloting)
			{
				item3.m_spawnedAvatar.SetMoveOrdersBlocked(blocked: true);
			}
		}
		StartCoroutine(DoCameraControlTutorial());
	}

	private IEnumerator DoNavigationTag()
	{
		if (!m_hasEverTagged)
		{
			TopBarInfoQueue.TopBarRequest tagCommandRequestN = TopBarInfoQueue.TopBarRequest.Standard("tutorial_topbar_tag_navigation", null, Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.Navigator), 10f, 10f, 10f, 1);
			Singleton<ContextControl>.Instance.SetTargetingAllowed(allowed: true);
			TopBarInfoQueue.TopBarRequest tagCommandRequest = TopBarInfoQueue.TopBarRequest.Standard("tutorial_topbar_tag_pc", "tutorial_topbar_tag_controller", Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.Navigator), 0f, 10f, 10f, 1);
			tagCommandRequestN.AddSeeMore("TAGGING");
			tagCommandRequest.AddSeeMore("TAGGING");
			Singleton<TopBarInfoQueue>.Instance.RegisterRequest(tagCommandRequest);
			Singleton<TopBarInfoQueue>.Instance.RegisterRequest(tagCommandRequestN);
			while (!Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberNavigation().GetCurrentType()
				.HasValue)
			{
				yield return null;
			}
			Singleton<TopBarInfoQueue>.Instance.RemoveRequest(tagCommandRequest);
			Singleton<TopBarInfoQueue>.Instance.RemoveRequest(tagCommandRequestN);
		}
		Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState().SetBoundsDefaults();
		StartCoroutine(WaitForFightersAppear());
		StartCoroutine(CheckForNoNavigation());
		yield return new WaitForSeconds(5f);
		TopBarInfoQueue.TopBarRequest intro1 = TopBarInfoQueue.TopBarRequest.Speech("mission_speech_gamescom_1", Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(Singleton<TopBarInfoQueue>.Instance.GetCommandNamedText()), Singleton<TopBarInfoQueue>.Instance.GetCommandPortrait(), Singleton<TopBarInfoQueue>.Instance.GetCommandJabberText(), isGoodGuy: true);
		Singleton<TopBarInfoQueue>.Instance.RegisterRequest(intro1);
	}

	private IEnumerator WaitForFightersAppear()
	{
		while (!Singleton<FighterCoordinator>.Instance.AreAnyFightersEngaged())
		{
			yield return null;
		}
		Vector3d pos = Singleton<BomberSpawn>.Instance.GetBomberSystems().gameObject.btransform().position;
		Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState().SetBounds(new Vector3(7160f, 0f, -10650f), new Vector3(13000f, 0f, -2000f));
		List<CrewSpawner.CrewmanAvatarPairing> all = Singleton<CrewSpawner>.Instance.GetAllCrew();
		foreach (CrewSpawner.CrewmanAvatarPairing item in all)
		{
			item.m_spawnedAvatar.SetDamageMultiplier(0.1f);
			if (item.m_crewman.GetPrimarySkill().GetSkill() == Crewman.SpecialisationSkill.BombAiming)
			{
				item.m_spawnedAvatar.SetDamageMultiplier(0f);
			}
		}
		List<FighterPlane> fp = Singleton<FighterCoordinator>.Instance.GetAllFighters();
		foreach (FighterPlane item2 in fp)
		{
			if (item2.GetAI().IsEngaged())
			{
				item2.ResetHealth(75f);
			}
		}
		BomberFuselageSection[] bfs = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetComponentsInChildren<BomberFuselageSection>();
		BomberFuselageSection[] array = bfs;
		foreach (BomberFuselageSection bomberFuselageSection in array)
		{
			bomberFuselageSection.SetInvincible(invincible: true);
		}
		for (int j = 0; j < Singleton<BomberSpawn>.Instance.GetBomberSystems().GetEngineCount(); j++)
		{
			Engine engine = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetEngine(j);
			if (engine != null && !engine.IsBroken() && !engine.IsDestroyed() && !engine.IsOnFire())
			{
				engine.SetInvincible(invincible: true);
			}
		}
		TopBarInfoQueue.TopBarRequest tagCommandRequestN = TopBarInfoQueue.TopBarRequest.Alert("tutorial_topbar_enemyfighters_gamescom", null, Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.Gunning), 0f, 0f, 0f, 1);
		TopBarInfoQueue.TopBarRequest tagCommandRequest = TopBarInfoQueue.TopBarRequest.Standard("tutorial_topbar_tag_pc", "tutorial_topbar_tag_controller", Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.Navigator), 15f, 10f, 10f, 1);
		tagCommandRequestN.AddSeeMore("TAGGING");
		tagCommandRequest.AddSeeMore("TAGGING");
		Singleton<TopBarInfoQueue>.Instance.RegisterRequest(tagCommandRequestN);
		Singleton<TopBarInfoQueue>.Instance.RegisterRequest(tagCommandRequest);
		float timeWithNoFighters = 0f;
		float timeAllTagged = 0f;
		while (timeWithNoFighters < 2.5f)
		{
			if (!Singleton<FighterCoordinator>.Instance.AreAnyFightersEngaged())
			{
				timeWithNoFighters += Time.deltaTime;
			}
			if (!Singleton<FighterCoordinator>.Instance.AreAnyUntaggedFightersEngaged())
			{
				timeAllTagged += Time.deltaTime;
			}
			if (timeAllTagged > 2f)
			{
				Singleton<TopBarInfoQueue>.Instance.RemoveRequest(tagCommandRequestN);
				Singleton<TopBarInfoQueue>.Instance.RemoveRequest(tagCommandRequest);
				Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState().SetBoundsDefaults();
			}
			yield return null;
		}
		Singleton<MissionCoordinator>.Instance.FireTrigger("FIGHTERS_DEFEATED");
		BomberFuselageSection[] array2 = bfs;
		foreach (BomberFuselageSection bomberFuselageSection2 in array2)
		{
			bomberFuselageSection2.SetInvincible(invincible: false);
		}
		foreach (CrewSpawner.CrewmanAvatarPairing item3 in all)
		{
			if (item3.m_crewman.GetPrimarySkill().GetSkill() == Crewman.SpecialisationSkill.Piloting)
			{
				item3.m_spawnedAvatar.SetDamageMultiplier(0.4f);
			}
			else
			{
				item3.m_spawnedAvatar.SetDamageMultiplier(1f);
			}
		}
		Singleton<TopBarInfoQueue>.Instance.RemoveRequest(tagCommandRequestN);
		Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState().SetBoundsDefaults();
		Singleton<TopBarMissionTipsConstant>.Instance.SetDisabledForMission(disabled: false);
		StartCoroutine(HintPhotos());
		StartCoroutine(WaitForBombTargetApproach());
		foreach (CrewSpawner.CrewmanAvatarPairing item4 in all)
		{
			item4.m_spawnedAvatar.SetOrdersBlocked(blocked: false);
			item4.m_spawnedAvatar.SetMoveOrdersBlocked(blocked: false);
		}
	}

	private IEnumerator HintPhotos()
	{
		bool anyPhotosTaken = false;
		MissionCoordinator instance = Singleton<MissionCoordinator>.Instance;
		instance.OnTrigger = (Action<string>)Delegate.Combine(instance.OnTrigger, (Action<string>)delegate(string st)
		{
			if (st.StartsWith("INTEL_PHOTO"))
			{
				anyPhotosTaken = true;
			}
			if (st == "OTHEROBJECTIVE_FINISHED")
			{
				anyPhotosTaken = true;
			}
		});
		while (Singleton<FighterCoordinator>.Instance.AreAnyUntaggedFightersEngaged())
		{
			yield return null;
		}
		List<CrewSpawner.CrewmanAvatarPairing> capList = Singleton<CrewSpawner>.Instance.GetAllCrew();
		CrewSpawner.CrewmanAvatarPairing bombAimer = null;
		foreach (CrewSpawner.CrewmanAvatarPairing item in capList)
		{
			if (item.m_crewman.GetPrimarySkill().GetSkill() == Crewman.SpecialisationSkill.BombAiming)
			{
				bombAimer = item;
			}
		}
		TopBarInfoQueue.TopBarRequest requestSelect = TopBarInfoQueue.TopBarRequest.Standard("tutorial_topbar_bombrun_move_bombaimer_select", null, Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.BombAiming), 0f, 0f, 0f, 1);
		requestSelect.AddPointerHint(bombAimer.m_spawnedAvatar.GetCrewmanGraphics().GetPelvisTransform().gameObject, Vector3.zero);
		requestSelect.AddPointerHint(Singleton<CrewPopulator>.Instance.GetPanelFor(Singleton<CrewSpawner>.Instance.GetIndexFromCrewman(bombAimer.m_spawnedAvatar)), new Vector3(96f, 40f, 0f), downArrow: true, worldSpace: false);
		TopBarInfoQueue.TopBarRequest requestSelectAnon = TopBarInfoQueue.TopBarRequest.Standard("tutorial_topbar_bombrun_move_bombaimer_select_short", null, Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.BombAiming), 0f, 0f, 0f, 1);
		requestSelect.AddPointerHint(bombAimer.m_spawnedAvatar.GetCrewmanGraphics().GetPelvisTransform().gameObject, Vector3.zero);
		requestSelect.AddPointerHint(Singleton<CrewPopulator>.Instance.GetPanelFor(Singleton<CrewSpawner>.Instance.GetIndexFromCrewman(bombAimer.m_spawnedAvatar)), new Vector3(96f, 40f, 0f), downArrow: true, worldSpace: false);
		TopBarInfoQueue.TopBarRequest requestMove = TopBarInfoQueue.TopBarRequest.Standard("tutorial_topbar_bombrun_move_bombaimer", "tutorial_topbar_bombrun_move_bombaimer_console", Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.BombAiming), 0f, 0f, 0f, 1);
		requestMove.AddPointerHint(Singleton<BomberSpawn>.Instance.GetBomberSystems().GetStationFor(BomberSystems.StationType.BombAimer).gameObject, Vector3.zero);
		requestSelectAnon.AddPointerHint(Singleton<BomberSpawn>.Instance.GetBomberSystems().GetStationFor(BomberSystems.StationType.BombAimer).gameObject, Vector3.zero);
		TopBarInfoQueue.TopBarRequest requestTag = TopBarInfoQueue.TopBarRequest.Standard("tutorial_topbar_gamescom_tag_photo", null, Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.BombAiming), 0f, 0f, 0f, 1);
		TopBarInfoQueue.TopBarRequest requestPressPhoto = TopBarInfoQueue.TopBarRequest.Standard("tutorial_topbar_gamescom_take_photo", null, Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.BombAiming), 0f, 0f, 0f, 1);
		TopBarInfoQueue.TopBarRequest requestWatchPhoto = TopBarInfoQueue.TopBarRequest.Standard("tutorial_topbar_gamescom_take_photo", null, Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.BombAiming), 0f, 0f, 0f, 1);
		requestPressPhoto.AddPointerHint("PHOTOBUTTON", Vector3.zero, downArrow: true, worldSpace: false);
		requestWatchPhoto.AddPointerHint("BOMBVIEW", Vector3.zero, downArrow: true, worldSpace: false);
		List<TopBarInfoQueue.TopBarRequest> tbrList = new List<TopBarInfoQueue.TopBarRequest> { requestSelect, requestSelectAnon, requestMove, requestTag, requestWatchPhoto, requestPressPhoto };
		requestPressPhoto.AddSeeMore("PHOTOGRAPHS");
		requestWatchPhoto.AddSeeMore("PHOTOGRAPHS");
		float timer = 0f;
		while (!anyPhotosTaken)
		{
			timer += Time.deltaTime;
			GameObject miniPromptToShow = null;
			CrewmanAvatar selectedCrewman = Singleton<ContextControl>.Instance.GetCurrentlySelected();
			TopBarInfoQueue.TopBarRequest toShow2 = null;
			if (SomeoneInBombAimingStation())
			{
				toShow2 = ((selectedCrewman == null) ? requestSelectAnon : ((Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberNavigation().GetCurrentType() != BomberNavigation.NavigationPointType.Detour) ? requestTag : ((!Singleton<BomberSpawn>.Instance.GetBomberSystems().GetPhotoCamera().WouldBeGoodPhoto()) ? requestWatchPhoto : requestPressPhoto)));
			}
			else if (selectedCrewman == null || selectedCrewman.GetCrewman().GetPrimarySkill().GetSkill() != Crewman.SpecialisationSkill.BombAiming)
			{
				toShow2 = requestSelect;
				if (timer > 7.5f)
				{
					miniPromptToShow = m_miniPromptSelect;
				}
			}
			else
			{
				toShow2 = requestMove;
				if (timer > 12.5f)
				{
					miniPromptToShow = m_miniPromptMove;
				}
			}
			foreach (TopBarInfoQueue.TopBarRequest item2 in tbrList)
			{
				if (item2 == toShow2)
				{
					Singleton<TopBarInfoQueue>.Instance.RegisterRequest(item2);
				}
				else
				{
					Singleton<TopBarInfoQueue>.Instance.RemoveRequest(item2);
				}
			}
			if (miniPromptToShow != null)
			{
				DoMiniPrompt(miniPromptToShow);
			}
			else
			{
				ClearMiniPrompt();
			}
			yield return null;
		}
		ClearMiniPrompt();
		foreach (TopBarInfoQueue.TopBarRequest item3 in tbrList)
		{
			Singleton<TopBarInfoQueue>.Instance.RemoveRequest(item3);
		}
		Singleton<BomberSpawn>.Instance.GetBomberSystems().GetFuelTank(0).SetDontUse(dontuse: false);
		Singleton<BomberSpawn>.Instance.GetBomberSystems().GetFuelTank(1).SetDontUse(dontuse: false);
		Singleton<BomberSpawn>.Instance.GetBomberSystems().GetElectricalSystem().SetNoLongerInvincible();
		Singleton<BomberSpawn>.Instance.GetBomberSystems().GetTemperatureOxygen().SetInactive(inactive: false);
		Singleton<BomberSpawn>.Instance.GetBomberSystems().GetHydraulics().SetNoLongerInvincible();
		for (int i = 0; i < Singleton<BomberSpawn>.Instance.GetBomberSystems().GetEngineCount(); i++)
		{
			Engine engine = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetEngine(i);
			if (engine != null && !engine.IsBroken() && !engine.IsDestroyed() && !engine.IsOnFire())
			{
				engine.SetInvincible(invincible: false);
			}
		}
		Singleton<MissionSpeedControls>.Instance.SetBlocked(blocked: false);
		StartCoroutine(DoExcitementTweaksCrew());
		StartCoroutine(DoExcitementTweaksBomber());
		StartCoroutine(DoExcitementTweaksEngines());
		Singleton<MissionCoordinator>.Instance.FireTrigger("ACE_SPEECH_ALLOW");
		yield return new WaitForSeconds(10f);
		StartCoroutine(WaitForInFlak());
	}

	private bool SomeoneInBombAimingStation()
	{
		return Singleton<BomberSpawn>.Instance.GetBomberSystems().GetStationFor(BomberSystems.StationType.BombAimer).GetCurrentCrewman() != null;
	}

	private IEnumerator WaitForInFlak()
	{
		FlakHazardArea[] fha = UnityEngine.Object.FindObjectsOfType<FlakHazardArea>();
		Transform bbt = Singleton<BomberSpawn>.Instance.GetBomberSystems().transform;
		bool shouldShow = false;
		float timeShownFor = 0f;
		float timeSinceFirstShow = 0f;
		bool everShown = false;
		TopBarInfoQueue.TopBarRequest tagCommandRequestN = TopBarInfoQueue.TopBarRequest.Standard("tutorial_topbar_flak_gohigher", null, Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.Piloting), 15f, 0f, 0f, 1);
		tagCommandRequestN.AddSeeMore("ALTITUDE");
		do
		{
			FlakHazardArea[] array = fha;
			foreach (FlakHazardArea flakHazardArea in array)
			{
				Vector3 vector = flakHazardArea.transform.position - bbt.position;
				vector.y = 0f;
				if (vector.x < 3200f && vector.x > -3200f && vector.y < 3200f && vector.y > -3200f && Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState().GetAltitudeTarget() == 0)
				{
					shouldShow = true;
				}
			}
			yield return null;
			if (shouldShow)
			{
				everShown = true;
				timeShownFor += Time.deltaTime;
				Singleton<TopBarInfoQueue>.Instance.RegisterRequest(tagCommandRequestN);
			}
			if (everShown)
			{
				timeSinceFirstShow += Time.deltaTime;
			}
		}
		while (!(timeShownFor > 45f) && !(timeSinceFirstShow > 240f));
		Singleton<TopBarInfoQueue>.Instance.RemoveRequest(tagCommandRequestN);
	}

	private IEnumerator WaitForBombTargetApproach()
	{
		bool isInRange = false;
		bool targetsDone = false;
		MissionCoordinator instance = Singleton<MissionCoordinator>.Instance;
		instance.OnTrigger = (Action<string>)Delegate.Combine(instance.OnTrigger, (Action<string>)delegate(string st)
		{
			if (st == "STARTLAUNCH")
			{
				isInRange = true;
			}
			if (st.StartsWith("BOMBTARGETS"))
			{
				targetsDone = true;
			}
		});
		while (!isInRange)
		{
			yield return null;
		}
		TopBarInfoQueue.TopBarRequest nearTarget = TopBarInfoQueue.TopBarRequest.Standard("tutorial_topbar_gamescom_bomb_near_target", null, Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.BombAiming), 0f, 10f, 15f, 1);
		TopBarInfoQueue.TopBarRequest requestTag = TopBarInfoQueue.TopBarRequest.Standard("tutorial_topbar_gamescom_bomb_target", null, Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.BombAiming), 0f, 10f, 15f, 1);
		TopBarInfoQueue.TopBarRequest requestDoorsArm = TopBarInfoQueue.TopBarRequest.Standard("tutorial_topbar_gamescom_doors_open", null, Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.BombAiming), 0f, 0f, 0f, 1);
		requestDoorsArm.AddPointerHint("BOMBBAYOPEN", Vector3.zero, downArrow: true, worldSpace: false);
		TopBarInfoQueue.TopBarRequest requestArmBombs = TopBarInfoQueue.TopBarRequest.Standard("tutorial_topbar_gamescom_arm_bombs", null, Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.BombAiming), 0f, 0f, 0f, 1);
		requestArmBombs.AddPointerHint("ARMBOMBS", Vector3.zero, downArrow: true, worldSpace: false);
		TopBarInfoQueue.TopBarRequest requestDoorsDrop = TopBarInfoQueue.TopBarRequest.Standard("tutorial_topbar_gamescom_bomb_target_2", null, Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.BombAiming), 0f, 0f, 0f, 1);
		requestDoorsDrop.AddPointerHint("BOMBRELEASE", Vector3.zero, downArrow: true, worldSpace: false);
		requestDoorsDrop.AddPointerHint("BOMBVIEW", Vector3.zero, downArrow: true, worldSpace: false);
		TopBarInfoQueue.TopBarRequest requestSelectAnon = TopBarInfoQueue.TopBarRequest.Standard("tutorial_topbar_bombrun_move_bombaimer_select_short", null, Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.BombAiming), 0f, 0f, 0f, 1);
		List<CrewSpawner.CrewmanAvatarPairing> capList = Singleton<CrewSpawner>.Instance.GetAllCrew();
		CrewSpawner.CrewmanAvatarPairing bombAimer2 = null;
		foreach (CrewSpawner.CrewmanAvatarPairing item in capList)
		{
			if (item.m_crewman.GetPrimarySkill().GetSkill() == Crewman.SpecialisationSkill.BombAiming)
			{
				bombAimer2 = item;
			}
		}
		requestSelectAnon.AddPointerHint(Singleton<BomberSpawn>.Instance.GetBomberSystems().GetStationFor(BomberSystems.StationType.BombAimer).gameObject, Vector3.zero);
		yield return new WaitForSeconds(10f);
		List<TopBarInfoQueue.TopBarRequest> tbrList = new List<TopBarInfoQueue.TopBarRequest> { requestTag, requestDoorsArm, requestArmBombs, requestDoorsDrop, requestSelectAnon, nearTarget };
		nearTarget.AddSeeMore("BOMBRUN");
		requestTag.AddSeeMore("BOMBRUN");
		requestDoorsDrop.AddSeeMore("BOMBRUN");
		requestArmBombs.AddSeeMore("BOMBRUN");
		requestDoorsArm.AddSeeMore("BOMBRUN");
		requestSelectAnon.AddSeeMore("BOMBRUN");
		while (!targetsDone)
		{
			TopBarInfoQueue.TopBarRequest toShow2 = null;
			toShow2 = ((!IsBombAimerInStation()) ? nearTarget : ((!(Singleton<ContextControl>.Instance.GetCurrentlySelected() == Singleton<BomberSpawn>.Instance.GetBomberSystems().GetStationFor(BomberSystems.StationType.BombAimer).GetCurrentCrewman())) ? requestSelectAnon : (Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBombBayDoors().IsFullyClosed() ? requestDoorsArm : ((Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBombLoad().GetNumBombsRacksArmed() < 3) ? requestArmBombs : ((Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberNavigation().GetCurrentType() != BomberNavigation.NavigationPointType.FunctionLandingBombing) ? requestTag : requestDoorsDrop)))));
			foreach (TopBarInfoQueue.TopBarRequest item2 in tbrList)
			{
				if (item2 == toShow2)
				{
					Singleton<TopBarInfoQueue>.Instance.RegisterRequest(item2);
				}
				else
				{
					Singleton<TopBarInfoQueue>.Instance.RemoveRequest(item2);
				}
			}
			yield return null;
		}
		foreach (TopBarInfoQueue.TopBarRequest item3 in tbrList)
		{
			Singleton<TopBarInfoQueue>.Instance.RemoveRequest(item3);
		}
		TopBarInfoQueue.TopBarRequest spitfiresHint = TopBarInfoQueue.TopBarRequest.Standard("tutorial_topbar_gamescom_radioop_spitfires", null, Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.BombAiming), 0f, 15f, 15f, 1);
		StationRadioOperator sro = (StationRadioOperator)Singleton<BomberSpawn>.Instance.GetBomberSystems().GetStationFor(BomberSystems.StationType.RadioOperator);
		float counter = 0f;
		while (Singleton<FighterCoordinator>.Instance.GetNumTaggableCurrently() + Singleton<FighterCoordinator>.Instance.GetNumTaggedCurrently() >= 5 || counter < 60f)
		{
			bool shouldShowHint = false;
			if (Singleton<FighterCoordinator>.Instance.GetNumTaggableCurrently() + Singleton<FighterCoordinator>.Instance.GetNumTaggedCurrently() >= 1 && sro.GetCurrentCrewman() != null && sro.GetCurrentCrewman().GetCrewman().GetPrimarySkill()
				.GetSkill() == Crewman.SpecialisationSkill.RadioOp && sro.GetSpitfireTimer().CanStart())
			{
				shouldShowHint = true;
			}
			if (shouldShowHint)
			{
				Singleton<TopBarInfoQueue>.Instance.RegisterRequest(spitfiresHint);
			}
			else
			{
				Singleton<TopBarInfoQueue>.Instance.RemoveRequest(spitfiresHint);
			}
			counter += Time.deltaTime;
			yield return null;
		}
		Singleton<TopBarInfoQueue>.Instance.RemoveRequest(spitfiresHint);
	}

	private bool IsBombAimerInStation()
	{
		int currentCrewCount = Singleton<CrewContainer>.Instance.GetCurrentCrewCount();
		bool result = false;
		for (int i = 0; i < currentCrewCount; i++)
		{
			Crewman crewman = Singleton<CrewContainer>.Instance.GetCrewman(i);
			if (crewman.GetPrimarySkill().GetSkill() == Crewman.SpecialisationSkill.BombAiming)
			{
				CrewmanAvatar avatarFor = Singleton<ContextControl>.Instance.GetAvatarFor(crewman);
				if (avatarFor.GetStation() != null && avatarFor.GetStation() is StationBombAimer)
				{
					result = true;
				}
			}
		}
		return result;
	}

	private IEnumerator CheckForNoNavigation()
	{
		TopBarInfoQueue.TopBarRequest tagCommandRequestN = TopBarInfoQueue.TopBarRequest.Alert("tutorial_topbar_tag_navigation_long", null, Singleton<EnumToIconMapping>.Instance.GetIconName(EnumToIconMapping.InteractionOrAlertType.Navigate), 15f, 15f, 10f, 2);
		tagCommandRequestN.AddSeeMore("TAGGING");
		while (true)
		{
			Vector3d pos = Singleton<BomberSpawn>.Instance.GetBomberSystems().gameObject.btransform().position;
			pos.y = 0.0;
			if (!Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberNavigation().GetCurrentType()
				.HasValue)
			{
				Singleton<TopBarInfoQueue>.Instance.RegisterRequest(tagCommandRequestN);
			}
			else
			{
				Singleton<TopBarInfoQueue>.Instance.RemoveRequest(tagCommandRequestN);
			}
			yield return null;
		}
	}

	private IEnumerator DoExcitementTweaksBomber()
	{
		while (!Singleton<BomberSpawn>.Instance.GetBomberSystems().GetCriticalFlash().IsFlashing())
		{
			yield return null;
		}
		BomberFuselageSection[] bfs = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetComponentsInChildren<BomberFuselageSection>();
		BomberFuselageSection[] array = bfs;
		foreach (BomberFuselageSection bomberFuselageSection in array)
		{
			bomberFuselageSection.SetInvincible(invincible: true);
		}
		yield return new WaitForSeconds(30f);
		BomberFuselageSection[] array2 = bfs;
		foreach (BomberFuselageSection bomberFuselageSection2 in array2)
		{
			bomberFuselageSection2.SetInvincible(invincible: false);
		}
	}

	private IEnumerator DoExcitementTweaksEngines()
	{
		while (true)
		{
			int numEngines = 0;
			for (int i = 0; i < Singleton<BomberSpawn>.Instance.GetBomberSystems().GetEngineCount(); i++)
			{
				Engine engine = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetEngine(i);
				if (engine != null && !engine.IsBroken() && !engine.IsDestroyed() && !engine.IsOnFire())
				{
					numEngines++;
				}
			}
			if (numEngines == 1)
			{
				break;
			}
			yield return null;
		}
		for (int j = 0; j < Singleton<BomberSpawn>.Instance.GetBomberSystems().GetEngineCount(); j++)
		{
			Engine engine2 = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetEngine(j);
			if (engine2 != null && !engine2.IsBroken() && !engine2.IsDestroyed() && !engine2.IsOnFire())
			{
				engine2.SetInvincible(invincible: true);
			}
		}
		yield return new WaitForSeconds(30f);
		for (int k = 0; k < Singleton<BomberSpawn>.Instance.GetBomberSystems().GetEngineCount(); k++)
		{
			Engine engine3 = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetEngine(k);
			if (engine3 != null && !engine3.IsBroken() && !engine3.IsDestroyed() && !engine3.IsOnFire())
			{
				engine3.SetInvincible(invincible: false);
			}
		}
	}

	private IEnumerator DoExcitementTweaksCrew()
	{
		while (Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState().GetAltitudeAboveGround() < 50f)
		{
			yield return null;
		}
		int prevNumOut = 0;
		float invincibilityTimer = 0f;
		List<CrewSpawner.CrewmanAvatarPairing> all = Singleton<CrewSpawner>.Instance.GetAllCrew();
		foreach (CrewSpawner.CrewmanAvatarPairing item in all)
		{
			if (item.m_crewman.GetPrimarySkill().GetSkill() == Crewman.SpecialisationSkill.Piloting)
			{
				item.m_spawnedAvatar.SetDamageMultiplier(0.4f);
			}
			else
			{
				item.m_spawnedAvatar.SetDamageMultiplier(1f);
			}
		}
		while (Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState().GetAltitudeAboveGround() > 30f)
		{
			int numDead = 0;
			int numBleeding = 0;
			foreach (CrewSpawner.CrewmanAvatarPairing item2 in all)
			{
				if (item2.m_crewman.IsDead())
				{
					numDead++;
				}
				else if (item2.m_spawnedAvatar.GetHealthState().IsCountingDown())
				{
					numBleeding++;
				}
			}
			if (numDead + numBleeding >= 2 && numDead + numBleeding > prevNumOut)
			{
				prevNumOut = numDead + numBleeding;
				invincibilityTimer = 30f;
				foreach (CrewSpawner.CrewmanAvatarPairing item3 in all)
				{
					item3.m_spawnedAvatar.SetDamageMultiplier(0.1f);
				}
			}
			if (invincibilityTimer > 0f)
			{
				invincibilityTimer -= Time.deltaTime;
				if (invincibilityTimer <= 0f)
				{
					foreach (CrewSpawner.CrewmanAvatarPairing item4 in all)
					{
						if (item4.m_crewman.GetPrimarySkill().GetSkill() == Crewman.SpecialisationSkill.Piloting)
						{
							item4.m_spawnedAvatar.SetDamageMultiplier(0.4f);
						}
						else
						{
							item4.m_spawnedAvatar.SetDamageMultiplier(1f);
						}
					}
				}
			}
			yield return null;
		}
		foreach (CrewSpawner.CrewmanAvatarPairing item5 in all)
		{
			item5.m_spawnedAvatar.SetInvincible(invincible: false);
		}
	}
}
