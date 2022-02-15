using System;
using System.Collections;
using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;
using WingroveAudio;

public class ExtraStartSettings_Tutorial2 : MonoBehaviour
{
	[SerializeField]
	private GameObject m_overridePilotPanel;

	[SerializeField]
	private tk2dBaseSprite m_sprite;

	private bool m_hasCompletedBombTarget;

	private IEnumerator Start()
	{
		BomberSystems bs = Singleton<BomberSpawn>.Instance.GetBomberSystems();
		Singleton<DifficultyMagic>.Instance.SetDisabled();
		Singleton<BomberSpawn>.Instance.GetBomberSystems().GetElectricalSystem().SetInvincible();
		Singleton<BomberSpawn>.Instance.GetBomberSystems().GetOxygenTank().SetInvincible();
		Singleton<BomberSpawn>.Instance.GetBomberSystems().GetTemperatureOxygen().SetInactive(inactive: true);
		Singleton<BomberSpawn>.Instance.GetBomberSystems().GetHydraulics().SetInvincible();
		Singleton<MissionSpeedControls>.Instance.SetBlocked(blocked: true);
		Singleton<TopBarMissionTipsConstant>.Instance.SetDisabledForMission(disabled: true);
		yield return null;
		Singleton<BomberSpawn>.Instance.GetBomberSystems().GetStationFor(BomberSystems.StationType.MedicalBay).SetLocked(locked: true);
		Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState().LockWingWalking();
		bs.GetStationFor(BomberSystems.StationType.Pilot).SetOverridePanelUI(m_overridePilotPanel);
		List<CrewSpawner.CrewmanAvatarPairing> capList = Singleton<CrewSpawner>.Instance.GetAllCrew();
		foreach (CrewSpawner.CrewmanAvatarPairing item in capList)
		{
			item.m_spawnedAvatar.SetOrdersBlocked(blocked: true);
			item.m_spawnedAvatar.SetInvincible(invincible: true);
		}
		MissionCoordinator instance = Singleton<MissionCoordinator>.Instance;
		instance.OnTrigger = (Action<string>)Delegate.Combine(instance.OnTrigger, (Action<string>)delegate(string st)
		{
			if (st == "T2BOMBRUN_SUCCESS")
			{
				m_hasCompletedBombTarget = true;
			}
		});
		m_sprite.gameObject.SetActive(value: false);
		StartCoroutine(WaitForTakeOff());
	}

	private IEnumerator WaitForTakeOff()
	{
		TopBarInfoQueue.TopBarRequest speech1 = TopBarInfoQueue.TopBarRequest.Speech("tutorial_speech_tutorial2_intro1", Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(Singleton<TopBarInfoQueue>.Instance.GetCommandNamedText()), Singleton<TopBarInfoQueue>.Instance.GetCommandPortrait(), Singleton<TopBarInfoQueue>.Instance.GetCommandJabberText(), isGoodGuy: true);
		Singleton<TopBarInfoQueue>.Instance.RegisterRequest(speech1);
		yield return new WaitForSeconds(5f);
		List<CrewSpawner.CrewmanAvatarPairing> capList = Singleton<CrewSpawner>.Instance.GetAllCrew();
		foreach (CrewSpawner.CrewmanAvatarPairing item in capList)
		{
			item.m_spawnedAvatar.SetOrdersBlocked(blocked: false);
			item.m_spawnedAvatar.SetMoveOrdersBlocked(blocked: true);
		}
		yield return new WaitForSeconds(5f);
		Singleton<TopBarInfoQueue>.Instance.RemoveRequest(speech1);
		TopBarInfoQueue.TopBarRequest takeOffRequest = TopBarInfoQueue.TopBarRequest.Standard("tutorial_topbar_bombrun_takeoff", null, Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.Piloting), 0f, 0f, 0f, 1);
		TopBarInfoQueue.TopBarRequest takeOffRequestSelect = TopBarInfoQueue.TopBarRequest.Standard("tutorial_topbar_bombrun_takeoff_select", null, Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.Piloting), 0f, 0f, 0f, 1);
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
		while (Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState().HasNotCompletedTakeOff())
		{
			if (Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState().HasNotStartedTakeOff())
			{
				if (Singleton<ContextControl>.Instance.GetCurrentlySelected() != pilot.m_spawnedAvatar)
				{
					Singleton<TopBarInfoQueue>.Instance.RemoveRequest(takeOffRequest);
					Singleton<TopBarInfoQueue>.Instance.RegisterRequest(takeOffRequestSelect);
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
			yield return null;
		}
		Singleton<TopBarInfoQueue>.Instance.RemoveRequest(takeOffRequest);
		Singleton<TopBarInfoQueue>.Instance.RemoveRequest(takeOffRequestSelect);
		yield return new WaitForSeconds(2f);
		TopBarInfoQueue.TopBarRequest landingGearUpRequest = TopBarInfoQueue.TopBarRequest.Standard("tutorial_topbar_landing_gear_up", null, Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.Piloting), 0f, 0f, 0f, 1);
		landingGearUpRequest.AddPointerHint("RAISEGEAR", Vector3.zero, downArrow: true, worldSpace: false);
		TopBarInfoQueue.TopBarRequest landingGearUpExpl = TopBarInfoQueue.TopBarRequest.Standard("tutorial_topbar_landing_gear_expl", null, Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.Piloting), 0f, 0f, 0f, 1);
		while (!Singleton<BomberSpawn>.Instance.GetBomberSystems().GetLandingGear().IsFullyRaised())
		{
			if (!Singleton<BomberSpawn>.Instance.GetBomberSystems().GetLandingGear().IsRaising())
			{
				if (Singleton<ContextControl>.Instance.GetCurrentlySelected() != pilot.m_spawnedAvatar)
				{
					Singleton<TopBarInfoQueue>.Instance.RegisterRequest(takeOffRequestSelect);
					Singleton<TopBarInfoQueue>.Instance.RemoveRequest(landingGearUpRequest);
					Singleton<TopBarInfoQueue>.Instance.RemoveRequest(landingGearUpExpl);
				}
				else
				{
					Singleton<TopBarInfoQueue>.Instance.RegisterRequest(landingGearUpRequest);
					Singleton<TopBarInfoQueue>.Instance.RemoveRequest(landingGearUpExpl);
					Singleton<TopBarInfoQueue>.Instance.RemoveRequest(takeOffRequestSelect);
				}
			}
			else
			{
				Singleton<TopBarInfoQueue>.Instance.RemoveRequest(landingGearUpRequest);
				Singleton<TopBarInfoQueue>.Instance.RemoveRequest(takeOffRequestSelect);
				Singleton<TopBarInfoQueue>.Instance.RegisterRequest(landingGearUpExpl);
			}
			yield return null;
		}
		Singleton<TopBarInfoQueue>.Instance.RemoveRequest(landingGearUpRequest);
		StartCoroutine(CheckForNoNavigation());
		CrewSpawner.CrewmanAvatarPairing bombAimer2 = null;
		foreach (CrewSpawner.CrewmanAvatarPairing item3 in capList)
		{
			if (item3.m_crewman.GetPrimarySkill().GetSkill() == Crewman.SpecialisationSkill.BombAiming)
			{
				bombAimer2 = item3;
				item3.m_spawnedAvatar.SetOrdersBlocked(blocked: false);
				item3.m_spawnedAvatar.SetMoveOrdersBlocked(blocked: false);
			}
			else if (item3.m_crewman.GetPrimarySkill().GetSkill() == Crewman.SpecialisationSkill.Piloting)
			{
				item3.m_spawnedAvatar.SetOrdersBlocked(blocked: true);
			}
		}
		Singleton<MusicSelectionRules>.Instance.TriggerTimed(MusicSelectionRules.MusicTriggerEvents.TrainingMission2, 30f);
		Singleton<MusicSelectionRules>.Instance.Trigger(MusicSelectionRules.MusicTriggerEvents.TrainingMissionEver);
		yield return new WaitForSeconds(3f);
		Singleton<TopBarInfoQueue>.Instance.RemoveRequest(landingGearUpExpl);
		StartCoroutine(DoBombRunSequenceTutorial());
	}

	private IEnumerator DoBombRunSequenceTutorial()
	{
		List<CrewSpawner.CrewmanAvatarPairing> capList = Singleton<CrewSpawner>.Instance.GetAllCrew();
		CrewSpawner.CrewmanAvatarPairing bombAimer = null;
		foreach (CrewSpawner.CrewmanAvatarPairing item in capList)
		{
			if (item.m_crewman.GetPrimarySkill().GetSkill() == Crewman.SpecialisationSkill.BombAiming)
			{
				bombAimer = item;
			}
		}
		TopBarInfoQueue.TopBarRequest moveBARequestSelect = TopBarInfoQueue.TopBarRequest.Standard("tutorial_topbar_bombrun_move_bombaimer_select", null, Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.BombAiming), 0f, 10f, 0f, 1);
		moveBARequestSelect.AddPointerHint(bombAimer.m_spawnedAvatar.GetCrewmanGraphics().GetPelvisTransform().gameObject, Vector3.zero);
		moveBARequestSelect.AddPointerHint(Singleton<CrewPopulator>.Instance.GetPanelFor(Singleton<CrewSpawner>.Instance.GetIndexFromCrewman(bombAimer.m_spawnedAvatar)), new Vector3(96f, 40f, 0f), downArrow: true, worldSpace: false);
		TopBarInfoQueue.TopBarRequest moveBARequestSelectShort = TopBarInfoQueue.TopBarRequest.Standard("tutorial_topbar_bombrun_move_bombaimer_select_short", null, Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.BombAiming), 0f, 10f, 0f, 1);
		moveBARequestSelectShort.AddPointerHint(bombAimer.m_spawnedAvatar.GetCrewmanGraphics().GetPelvisTransform().gameObject, Vector3.zero);
		moveBARequestSelectShort.AddPointerHint(Singleton<CrewPopulator>.Instance.GetPanelFor(Singleton<CrewSpawner>.Instance.GetIndexFromCrewman(bombAimer.m_spawnedAvatar)), new Vector3(96f, 40f, 0f), downArrow: true, worldSpace: false);
		TopBarInfoQueue.TopBarRequest moveBARequest = TopBarInfoQueue.TopBarRequest.Standard("tutorial_topbar_bombrun_move_bombaimer", null, Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.BombAiming), 0f, 10f, 0f, 1);
		moveBARequest.AddPointerHint(Singleton<BomberSpawn>.Instance.GetBomberSystems().GetStationFor(BomberSystems.StationType.BombAimer).gameObject, Vector3.zero);
		while (!IsBombAimerInStation())
		{
			if (bombAimer.m_spawnedAvatar.IsIdle() || bombAimer.m_spawnedAvatar.GetStation() != null)
			{
				if (Singleton<ContextControl>.Instance.GetCurrentlySelected() != bombAimer.m_spawnedAvatar)
				{
					Singleton<TopBarInfoQueue>.Instance.RegisterRequest(moveBARequestSelect);
					Singleton<TopBarInfoQueue>.Instance.RemoveRequest(moveBARequest);
				}
				else
				{
					Singleton<TopBarInfoQueue>.Instance.RegisterRequest(moveBARequest);
					Singleton<TopBarInfoQueue>.Instance.RemoveRequest(moveBARequestSelect);
				}
			}
			else
			{
				Singleton<TopBarInfoQueue>.Instance.RemoveRequest(moveBARequest);
				Singleton<TopBarInfoQueue>.Instance.RemoveRequest(moveBARequestSelect);
			}
			yield return null;
		}
		Singleton<TopBarInfoQueue>.Instance.RemoveRequest(moveBARequest);
		Singleton<TopBarInfoQueue>.Instance.RemoveRequest(moveBARequestSelect);
		TopBarInfoQueue.TopBarRequest moveBARequestR = TopBarInfoQueue.TopBarRequest.Alert("tutorial_topbar_bombrun_move_bombaimer_reminder", null, Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.BombAiming), 0f, 10f, 5f, 1);
		moveBARequestR.AddPointerHint(Singleton<BomberSpawn>.Instance.GetBomberSystems().GetStationFor(BomberSystems.StationType.BombAimer).gameObject, Vector3.zero);
		TopBarInfoQueue.TopBarRequest tagBombTarget = TopBarInfoQueue.TopBarRequest.Standard("tutorial_topbar_bombrun_tag_target", null, Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.BombAiming), 0f, 0f, 0f, 2);
		TopBarInfoQueue.TopBarRequest openBombBayDoors = TopBarInfoQueue.TopBarRequest.Standard("tutorial_topbar_bombrun_open_bay_doors", null, Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.BombAiming), 0f, 0f, 0f, 2);
		openBombBayDoors.AddPointerHint("BOMBBAYOPEN", Vector3.zero, downArrow: true, worldSpace: false);
		TopBarInfoQueue.TopBarRequest armBombs = TopBarInfoQueue.TopBarRequest.Standard("tutorial_topbar_bombrun_arm_bombs", null, Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.BombAiming), 0f, 0f, 0f, 2);
		armBombs.AddPointerHint("ARMBOMBS", Vector3.zero, downArrow: true, worldSpace: false);
		TopBarInfoQueue.TopBarRequest dropTiming = TopBarInfoQueue.TopBarRequest.Standard("tutorial_topbar_bombrun_drop_timing", null, Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.BombAiming), 0f, 0f, 0f, 1);
		dropTiming.AddPointerHint("BOMBVIEW", Vector3.zero, downArrow: true, worldSpace: false);
		TopBarInfoQueue.TopBarRequest dropTimingNow = TopBarInfoQueue.TopBarRequest.Standard("tutorial_topbar_bombrun_drop_timing", null, Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.BombAiming), 0f, 0f, 0f, 1);
		dropTimingNow.AddPointerHint("BOMBRELEASE", Vector3.zero, downArrow: true, worldSpace: false);
		while (!Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBombLoad().HasDroppedAny())
		{
			bool isBASelected = false;
			if (Singleton<ContextControl>.Instance.GetCurrentlySelected() != bombAimer.m_spawnedAvatar)
			{
				Singleton<TopBarInfoQueue>.Instance.RegisterRequest(moveBARequestSelectShort);
			}
			else
			{
				isBASelected = true;
				Singleton<TopBarInfoQueue>.Instance.RemoveRequest(moveBARequestSelectShort);
			}
			bool allReady = true;
			if (!IsBombAimerInStation() && isBASelected)
			{
				allReady = false;
				Singleton<TopBarInfoQueue>.Instance.RegisterRequest(moveBARequestR);
			}
			else
			{
				Singleton<TopBarInfoQueue>.Instance.RemoveRequest(moveBARequestR);
			}
			if (Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberNavigation().GetCurrentType() != BomberNavigation.NavigationPointType.FunctionLandingBombing && isBASelected && allReady)
			{
				allReady = false;
				Singleton<TopBarInfoQueue>.Instance.RegisterRequest(tagBombTarget);
			}
			else
			{
				Singleton<TopBarInfoQueue>.Instance.RemoveRequest(tagBombTarget);
			}
			if (!Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBombBayDoors().IsFullyOpen() && !Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBombBayDoors().IsOpening() && isBASelected && allReady)
			{
				allReady = false;
				Singleton<TopBarInfoQueue>.Instance.RegisterRequest(openBombBayDoors);
			}
			else
			{
				Singleton<TopBarInfoQueue>.Instance.RemoveRequest(openBombBayDoors);
			}
			if (Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBombLoad().GetNumBombsRacksArmed() == 0 && isBASelected && allReady)
			{
				allReady = false;
				Singleton<TopBarInfoQueue>.Instance.RegisterRequest(armBombs);
			}
			else
			{
				Singleton<TopBarInfoQueue>.Instance.RemoveRequest(armBombs);
			}
			if (allReady && isBASelected)
			{
				if (Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBombSight().WouldHitTarget())
				{
					tagBombTarget.m_preShowTime = 5f;
					Singleton<TopBarInfoQueue>.Instance.RemoveRequest(dropTiming);
					Singleton<TopBarInfoQueue>.Instance.RegisterRequest(dropTimingNow);
				}
				else
				{
					Singleton<TopBarInfoQueue>.Instance.RegisterRequest(dropTiming);
					Singleton<TopBarInfoQueue>.Instance.RemoveRequest(dropTimingNow);
				}
			}
			else
			{
				Singleton<TopBarInfoQueue>.Instance.RemoveRequest(dropTiming);
				Singleton<TopBarInfoQueue>.Instance.RemoveRequest(dropTimingNow);
			}
			yield return null;
		}
		Singleton<TopBarInfoQueue>.Instance.RemoveRequest(moveBARequest);
		Singleton<TopBarInfoQueue>.Instance.RemoveRequest(tagBombTarget);
		Singleton<TopBarInfoQueue>.Instance.RemoveRequest(openBombBayDoors);
		Singleton<TopBarInfoQueue>.Instance.RemoveRequest(armBombs);
		Singleton<TopBarInfoQueue>.Instance.RemoveRequest(moveBARequestR);
		Singleton<TopBarInfoQueue>.Instance.RemoveRequest(moveBARequestSelectShort);
		Singleton<TopBarInfoQueue>.Instance.RemoveRequest(dropTiming);
		Singleton<TopBarInfoQueue>.Instance.RemoveRequest(dropTimingNow);
		yield return new WaitForSeconds(8f);
		if (m_hasCompletedBombTarget)
		{
			StartCoroutine(DoPhotoSection());
		}
		else
		{
			StartCoroutine(ResetForBombAim());
		}
	}

	private IEnumerator ResetForBombAim()
	{
		TopBarInfoQueue.TopBarRequest speech1 = TopBarInfoQueue.TopBarRequest.Speech("tutorial_speech_tutorial2_try_again", Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(Singleton<TopBarInfoQueue>.Instance.GetCommandNamedText()), Singleton<TopBarInfoQueue>.Instance.GetCommandPortrait(), Singleton<TopBarInfoQueue>.Instance.GetCommandJabberText(), isGoodGuy: true);
		Singleton<TopBarInfoQueue>.Instance.RegisterRequest(speech1);
		yield return new WaitForSeconds(7f);
		m_sprite.gameObject.SetActive(value: true);
		int progCached = WingroveRoot.Instance.GetParameterId("TUTORIAL_FADE_PROGRESS");
		WingroveRoot.Instance.SetParameterGlobal(progCached, 0f);
		m_sprite.color = new Color(0f, 0f, 0f, 0f);
		float alpha3 = 0f;
		while (alpha3 < 1f)
		{
			alpha3 += Time.deltaTime * 0.25f;
			WingroveRoot.Instance.SetParameterGlobal(progCached, Mathf.Clamp01(alpha3));
			m_sprite.color = new Color(0f, 0f, 0f, Mathf.Clamp01(alpha3));
			yield return null;
		}
		WingroveRoot.Instance.SetParameterGlobal(progCached, 1f);
		m_sprite.color = new Color(0f, 0f, 0f, 1f);
		alpha3 = 1f;
		Singleton<TopBarInfoQueue>.Instance.RemoveRequest(speech1);
		Singleton<BomberSpawn>.Instance.GetBomberSystems().gameObject.btransform().position = new Vector3d(0f, Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState().GetAltitude(), 120f);
		Singleton<BomberSpawn>.Instance.GetBomberSystems().transform.rotation = Quaternion.Euler(0f, -120f, 0f);
		Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState().GetPhysicsModel()
			.transform.position = Singleton<BomberSpawn>.Instance.GetBomberSystems().transform.position;
		Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState().GetPhysicsModel()
			.transform.rotation = Singleton<BomberSpawn>.Instance.GetBomberSystems().transform.rotation;
		WingroveRoot.Instance.SetParameterGlobal(progCached, 1f);
		Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBombBay().Reset();
		alpha3 = 1f;
		while (alpha3 > 0f)
		{
			alpha3 -= Time.deltaTime * 0.5f;
			WingroveRoot.Instance.SetParameterGlobal(progCached, Mathf.Clamp01(alpha3));
			m_sprite.color = new Color(0f, 0f, 0f, Mathf.Clamp01(alpha3));
			yield return null;
		}
		WingroveRoot.Instance.SetParameterGlobal(progCached, 0f);
		m_sprite.color = new Color(0f, 0f, 0f, 0f);
		m_sprite.gameObject.SetActive(value: false);
		TopBarInfoQueue.TopBarRequest speech2 = TopBarInfoQueue.TopBarRequest.Speech("tutorial_speech_tutorial2_try_again2", Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(Singleton<TopBarInfoQueue>.Instance.GetCommandNamedText()), Singleton<TopBarInfoQueue>.Instance.GetCommandPortrait(), Singleton<TopBarInfoQueue>.Instance.GetCommandJabberText(), isGoodGuy: true);
		Singleton<TopBarInfoQueue>.Instance.RegisterRequest(speech2);
		StartCoroutine(DoBombRunSequenceTutorial());
	}

	private IEnumerator DoPhotoSection()
	{
		yield return new WaitForSeconds(5f);
		TopBarInfoQueue.TopBarRequest speech1 = TopBarInfoQueue.TopBarRequest.Speech("tutorial_speech_tutorial2_sea_intro1", Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(Singleton<TopBarInfoQueue>.Instance.GetCommandNamedText()), Singleton<TopBarInfoQueue>.Instance.GetCommandPortrait(), Singleton<TopBarInfoQueue>.Instance.GetCommandJabberText(), isGoodGuy: true);
		TopBarInfoQueue.TopBarRequest speech2 = TopBarInfoQueue.TopBarRequest.Speech("tutorial_speech_tutorial2_sea_intro2", Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(Singleton<TopBarInfoQueue>.Instance.GetCommandNamedText()), Singleton<TopBarInfoQueue>.Instance.GetCommandPortrait(), Singleton<TopBarInfoQueue>.Instance.GetCommandJabberText(), isGoodGuy: true);
		Singleton<TopBarInfoQueue>.Instance.RegisterRequest(speech1);
		yield return new WaitForSeconds(10f);
		Singleton<TopBarInfoQueue>.Instance.RemoveRequest(speech1);
		Singleton<TopBarInfoQueue>.Instance.RegisterRequest(speech2);
		yield return new WaitForSeconds(10f);
		Singleton<TopBarInfoQueue>.Instance.RemoveRequest(speech2);
		TopBarInfoQueue.TopBarRequest goToArea = TopBarInfoQueue.TopBarRequest.Standard("tutorial_topbar_photo_area", null, Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.Navigator), 5f, 10f, 30f, 2);
		TopBarInfoQueue.TopBarRequest tag = TopBarInfoQueue.TopBarRequest.Standard("tutorial_topbar_photo_tag", null, Singleton<EnumToIconMapping>.Instance.GetIconName(EnumToIconMapping.InteractionOrAlertType.Photo), 0f, 0f, 0f, 1);
		TopBarInfoQueue.TopBarRequest take = TopBarInfoQueue.TopBarRequest.Standard("tutorial_topbar_photo_take", null, Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.BombAiming), 0f, 0f, 0f, 1);
		take.AddPointerHint("BOMBVIEW", Vector3.zero, downArrow: true, worldSpace: false);
		TopBarInfoQueue.TopBarRequest takeNow = TopBarInfoQueue.TopBarRequest.Standard("tutorial_topbar_photo_take", null, Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.BombAiming), 0f, 0f, 0f, 1);
		takeNow.AddPointerHint("PHOTOBUTTON", Vector3.zero, downArrow: true, worldSpace: false);
		while (Singleton<ObjectiveManager>.Instance.ObjectivesOfTypeRemain(ObjectiveManager.ObjectiveType.Other))
		{
			if (!Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberNavigation().GetCurrentType()
				.HasValue)
			{
				Singleton<TopBarInfoQueue>.Instance.RegisterRequest(goToArea);
			}
			else
			{
				Singleton<TopBarInfoQueue>.Instance.RemoveRequest(goToArea);
			}
			if (Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberNavigation().GetCurrentType() != BomberNavigation.NavigationPointType.Detour)
			{
				Singleton<TopBarInfoQueue>.Instance.RegisterRequest(tag);
				Singleton<TopBarInfoQueue>.Instance.RemoveRequest(take);
				Singleton<TopBarInfoQueue>.Instance.RemoveRequest(takeNow);
			}
			else
			{
				Singleton<TopBarInfoQueue>.Instance.RemoveRequest(tag);
				if (Singleton<BomberSpawn>.Instance.GetBomberSystems().GetPhotoCamera().WouldBeGoodPhoto())
				{
					Singleton<TopBarInfoQueue>.Instance.RemoveRequest(take);
					Singleton<TopBarInfoQueue>.Instance.RegisterRequest(takeNow);
				}
				else
				{
					Singleton<TopBarInfoQueue>.Instance.RegisterRequest(take);
					Singleton<TopBarInfoQueue>.Instance.RemoveRequest(takeNow);
				}
			}
			yield return null;
		}
		Singleton<TopBarInfoQueue>.Instance.RemoveRequest(goToArea);
		Singleton<TopBarInfoQueue>.Instance.RemoveRequest(take);
		Singleton<TopBarInfoQueue>.Instance.RemoveRequest(tag);
		Singleton<TopBarInfoQueue>.Instance.RemoveRequest(takeNow);
		StartCoroutine(ReturnToBase());
	}

	private IEnumerator ReturnToBase()
	{
		TopBarInfoQueue.TopBarRequest speech1 = TopBarInfoQueue.TopBarRequest.Speech("tutorial_speech_tutorial2_sea_end1", Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(Singleton<TopBarInfoQueue>.Instance.GetCommandNamedText()), Singleton<TopBarInfoQueue>.Instance.GetCommandPortrait(), Singleton<TopBarInfoQueue>.Instance.GetCommandJabberText(), isGoodGuy: true);
		Singleton<TopBarInfoQueue>.Instance.RegisterRequest(speech1);
		yield return new WaitForSeconds(10f);
		Singleton<TopBarInfoQueue>.Instance.RemoveRequest(speech1);
		while (true)
		{
			Vector3d pos = Singleton<BomberSpawn>.Instance.GetBomberSystems().gameObject.btransform().position;
			pos.y = 0.0;
			if (pos.magnitude < 6000.0)
			{
				break;
			}
			yield return null;
		}
		CrewSpawner.CrewmanAvatarPairing engineer = null;
		List<CrewSpawner.CrewmanAvatarPairing> capList = Singleton<CrewSpawner>.Instance.GetAllCrew();
		foreach (CrewSpawner.CrewmanAvatarPairing item in capList)
		{
			if (item.m_crewman.GetPrimarySkill().GetSkill() == Crewman.SpecialisationSkill.Engineer)
			{
				engineer = item;
				item.m_spawnedAvatar.SetOrdersBlocked(blocked: false);
				item.m_spawnedAvatar.SetMoveOrdersBlocked(blocked: false);
			}
			else if (item.m_crewman.GetPrimarySkill().GetSkill() == Crewman.SpecialisationSkill.Piloting)
			{
				item.m_spawnedAvatar.SetOrdersBlocked(blocked: false);
			}
		}
		Singleton<BomberSpawn>.Instance.GetBomberSystems().GetHydraulics().SetNoLongerInvincible();
		DamageSource ds = new DamageSource
		{
			m_damageType = DamageSource.DamageType.Impact,
			m_damageShapeEffect = DamageSource.DamageShape.None
		};
		Singleton<BomberSpawn>.Instance.GetBomberSystems().GetHydraulics().DamageGetPassthrough(500f, ds);
		yield return new WaitForSeconds(1f);
		TopBarInfoQueue.TopBarRequest goToArea = TopBarInfoQueue.TopBarRequest.Standard("tutorial_hydraulics_out", null, Singleton<EnumToIconMapping>.Instance.GetIconName(EnumToIconMapping.InteractionOrAlertType.Hydraulics), 0f, 10f, 0f, 3);
		Singleton<TopBarInfoQueue>.Instance.RegisterRequest(goToArea);
		TopBarInfoQueue.TopBarRequest selectEngineer = TopBarInfoQueue.TopBarRequest.Standard("tutorial_topbar_select_engineer", null, Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.Engineer), 0f, 0f, 0f, 2);
		selectEngineer.AddPointerHint(engineer.m_spawnedAvatar.GetCrewmanGraphics().GetPelvisTransform().gameObject, Vector3.zero);
		selectEngineer.AddPointerHint(Singleton<CrewPopulator>.Instance.GetPanelFor(Singleton<CrewSpawner>.Instance.GetIndexFromCrewman(engineer.m_spawnedAvatar)), new Vector3(96f, 40f, 0f), downArrow: true, worldSpace: false);
		TopBarInfoQueue.TopBarRequest sendEngineer = TopBarInfoQueue.TopBarRequest.Standard("tutorial_topbar_send_engineer", null, Singleton<EnumToIconMapping>.Instance.GetIconName(EnumToIconMapping.InteractionOrAlertType.Hydraulics), 0f, 0f, 0f, 2);
		TopBarInfoQueue.TopBarRequest anyCanRepair = TopBarInfoQueue.TopBarRequest.Standard("tutorial_topbar_tutorial2_any_hydraulics", null, Singleton<EnumToIconMapping>.Instance.GetIconName(EnumToIconMapping.InteractionOrAlertType.Repair), 15f, 15f, 0f, 1);
		Singleton<TopBarInfoQueue>.Instance.RegisterRequest(anyCanRepair);
		sendEngineer.AddPointerHint(Singleton<BomberSpawn>.Instance.GetBomberSystems().GetHydraulics().gameObject, Vector3.zero);
		while (Singleton<BomberSpawn>.Instance.GetBomberSystems().GetHydraulics().IsBroken())
		{
			if (engineer.m_spawnedAvatar.GetStation() != null || engineer.m_spawnedAvatar.IsIdle())
			{
				if (Singleton<ContextControl>.Instance.GetCurrentlySelected() == engineer.m_spawnedAvatar)
				{
					Singleton<TopBarInfoQueue>.Instance.RegisterRequest(sendEngineer);
					Singleton<TopBarInfoQueue>.Instance.RemoveRequest(selectEngineer);
				}
				else
				{
					Singleton<TopBarInfoQueue>.Instance.RemoveRequest(sendEngineer);
					Singleton<TopBarInfoQueue>.Instance.RegisterRequest(selectEngineer);
				}
			}
			else
			{
				Singleton<TopBarInfoQueue>.Instance.RemoveRequest(sendEngineer);
				Singleton<TopBarInfoQueue>.Instance.RemoveRequest(selectEngineer);
			}
			yield return null;
		}
		Singleton<BomberSpawn>.Instance.GetBomberSystems().GetHydraulics().SetInvincible();
		Singleton<TopBarInfoQueue>.Instance.RemoveRequest(anyCanRepair);
		Singleton<TopBarInfoQueue>.Instance.RemoveRequest(sendEngineer);
		Singleton<TopBarInfoQueue>.Instance.RemoveRequest(selectEngineer);
		Singleton<TopBarInfoQueue>.Instance.RemoveRequest(goToArea);
		TopBarInfoQueue.TopBarRequest fixedH = TopBarInfoQueue.TopBarRequest.Standard("tutorial_topbar_hydraulics_fixed", null, Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.Piloting), 0f, 15f, 25f, 1);
		Singleton<TopBarInfoQueue>.Instance.RegisterRequest(fixedH);
		Singleton<MissionSpeedControls>.Instance.SetBlocked(blocked: false);
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
}
