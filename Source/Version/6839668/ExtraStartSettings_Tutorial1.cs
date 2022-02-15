using System.Collections;
using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class ExtraStartSettings_Tutorial1 : MonoBehaviour
{
	[SerializeField]
	private GameObject m_introOverlayPrefab;

	[SerializeField]
	private Vector3[] m_relativePositionsForDrogues;

	[SerializeField]
	private GameObject m_overridePilotPanel;

	[SerializeField]
	private GameObject m_miniPromptMove;

	[SerializeField]
	private GameObject m_miniPromptSelect;

	[SerializeField]
	private GameObject m_miniPromptZoomCamera;

	private FighterPlane m_upperPlane;

	private FighterPlane m_rearPlane;

	private FighterPlane m_frontPlane;

	private GameObject m_currentMiniPrompt;

	private GameObject m_currentMiniPromptPrefab;

	private void DoMiniPrompt(GameObject go)
	{
		if (m_currentMiniPromptPrefab != go)
		{
			if (m_currentMiniPrompt != null)
			{
				Object.Destroy(m_currentMiniPrompt);
			}
			if (Singleton<UISelector>.Instance.IsPrimary())
			{
				m_currentMiniPrompt = Object.Instantiate(go);
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
			Object.Destroy(m_currentMiniPrompt);
		}
	}

	private IEnumerator Start()
	{
		GameObject intro = Object.Instantiate(m_introOverlayPrefab);
		Singleton<DifficultyMagic>.Instance.SetDisabled();
		Singleton<TopBarMissionTipsConstant>.Instance.SetDisabledForMission(disabled: true);
		BomberSystems bs = Singleton<BomberSpawn>.Instance.GetBomberSystems();
		Singleton<MissionCoordinator>.Instance.SetOutwardLocked(locked: true);
		Vector3d newPos = base.gameObject.btransform().position;
		bs.GetComponent<BomberState>().ForceToFlying(setInvincible: true);
		bs.GetLandingGear().ForceUp();
		bs.GetStationFor(BomberSystems.StationType.Pilot).SetOverridePanelUI(m_overridePilotPanel);
		HydraulicsTank ht = bs.GetHydraulics();
		ht.SetInvincible();
		if (bs.GetBombLoad() != null)
		{
			bs.GetBombLoad().ForceGone();
		}
		bs.GetBomberState().SetBounds(new Vector3(-13000f, 0f, 8000f), new Vector3(-6000f, 0f, 15000f));
		List<CrewSpawner.CrewmanAvatarPairing> capList = Singleton<CrewSpawner>.Instance.GetAllCrew();
		int gunnersRemaining = 0;
		foreach (CrewSpawner.CrewmanAvatarPairing item in capList)
		{
			_ = item.m_spawnedAvatar;
			Crewman crewman = item.m_crewman;
			Crewman.SpecialisationSkill skill = crewman.GetPrimarySkill().GetSkill();
			if (skill == Crewman.SpecialisationSkill.Gunning)
			{
				gunnersRemaining++;
			}
		}
		foreach (CrewSpawner.CrewmanAvatarPairing item2 in capList)
		{
			CrewmanAvatar spawnedAvatar = item2.m_spawnedAvatar;
			Crewman crewman2 = item2.m_crewman;
			Crewman.SpecialisationSkill skill2 = crewman2.GetPrimarySkill().GetSkill();
			if (skill2 == Crewman.SpecialisationSkill.BombAiming || skill2 == Crewman.SpecialisationSkill.Engineer || (skill2 == Crewman.SpecialisationSkill.Gunning && gunnersRemaining > 1))
			{
				if (skill2 == Crewman.SpecialisationSkill.Gunning)
				{
					gunnersRemaining--;
				}
				DamageSource damageSource = new DamageSource();
				damageSource.m_damageShapeEffect = DamageSource.DamageShape.None;
				damageSource.m_damageType = DamageSource.DamageType.Impact;
				damageSource.m_position = spawnedAvatar.transform.position;
				spawnedAvatar.DamageGetPassthrough(500f, damageSource);
				spawnedAvatar.InstantKill();
			}
			spawnedAvatar.SetSelectionBlocked(blocked: true);
			spawnedAvatar.SetOrdersBlocked(blocked: true);
		}
		foreach (CrewSpawner.CrewmanAvatarPairing item3 in capList)
		{
			CrewmanAvatar spawnedAvatar2 = item3.m_spawnedAvatar;
			Crewman crewman3 = item3.m_crewman;
			Crewman.SpecialisationSkill skill3 = crewman3.GetPrimarySkill().GetSkill();
			if (skill3 == Crewman.SpecialisationSkill.Gunning)
			{
				spawnedAvatar2.SetStation(null);
			}
		}
		BomberFuselageSection[] bfs = bs.GetComponentsInChildren<BomberFuselageSection>();
		BomberFuselageSection[] array = bfs;
		foreach (BomberFuselageSection bomberFuselageSection in array)
		{
			bomberFuselageSection.SetInvincible(invincible: true);
		}
		Singleton<BomberSpawn>.Instance.GetBomberSystems().GetElectricalSystem().SetInvincible();
		Singleton<BomberSpawn>.Instance.GetBomberSystems().GetOxygenTank().SetInvincible();
		Singleton<BomberSpawn>.Instance.GetBomberSystems().GetTemperatureOxygen().SetInactive(inactive: true);
		Singleton<ObjectiveManager>.Instance.SetReturnJourneyDisabled();
		Singleton<ContextControl>.Instance.HidePrompts(select: true, move: true, tagging: true, zoom: true);
		Singleton<ContextControl>.Instance.SetSelectCrewBlocked(blocked: true);
		yield return null;
		bs.gameObject.btransform().position = newPos;
		bs.GetBomberState().GetPhysicsModel().transform.position = bs.transform.position;
		FighterAI_MatchSpeedDrogue[] allFAs = Object.FindObjectsOfType<FighterAI_MatchSpeedDrogue>();
		int dI = 0;
		FighterAI_MatchSpeedDrogue[] array2 = allFAs;
		foreach (FighterAI_MatchSpeedDrogue fighterAI_MatchSpeedDrogue in array2)
		{
			fighterAI_MatchSpeedDrogue.SetNormalisedMatchTargetPosition(m_relativePositionsForDrogues[dI]);
			dI++;
		}
		m_upperPlane = allFAs[0].GetComponent<FighterPlane>();
		m_frontPlane = allFAs[1].GetComponent<FighterPlane>();
		m_rearPlane = allFAs[2].GetComponent<FighterPlane>();
		yield return null;
		foreach (CrewSpawner.CrewmanAvatarPairing item4 in capList)
		{
			CrewmanAvatar spawnedAvatar3 = item4.m_spawnedAvatar;
			Crewman crewman4 = item4.m_crewman;
			Crewman.SpecialisationSkill skill4 = crewman4.GetPrimarySkill().GetSkill();
			if (skill4 == Crewman.SpecialisationSkill.Gunning)
			{
				Station stationFor = bs.GetStationFor(BomberSystems.StationType.GunsNose);
				stationFor.SetCrewman(spawnedAvatar3);
			}
		}
		MissionLog.LogObjective lo = new MissionLog.LogObjective();
		Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetMissionLog().LogObjectiveNew(lo);
		lo.m_isComplete = true;
		Singleton<ContextControl>.Instance.SetTargetingAllowed(allowed: false);
		Singleton<BomberSpawn>.Instance.GetBomberSystems().GetStationFor(BomberSystems.StationType.BombAimer).SetLocked(locked: true);
		Singleton<BomberSpawn>.Instance.GetBomberSystems().GetStationFor(BomberSystems.StationType.MedicalBay).SetLocked(locked: true);
		Singleton<BomberSpawn>.Instance.GetBomberSystems().GetStationFor(BomberSystems.StationType.FlightEngineer).SetLocked(locked: true);
		AmmoBeltBox abb = Object.FindObjectOfType<AmmoBeltBox>();
		abb.SetInteractive(interactive: false);
		Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState().LockWingWalking();
		((StationPilot)Singleton<BomberSpawn>.Instance.GetBomberSystems().GetStationFor(BomberSystems.StationType.Pilot)).SetInactiveLocked(inactiveLocked: true);
		((StationNavigator)Singleton<BomberSpawn>.Instance.GetBomberSystems().GetStationFor(BomberSystems.StationType.Navigation)).SetInactiveLocked(inactiveLocked: true);
		yield return new WaitForSeconds(10f);
		TopBarInfoQueue.TopBarRequest intro2 = TopBarInfoQueue.TopBarRequest.Speech("tutorial_speech_tutorial1_intro1", Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(Singleton<TopBarInfoQueue>.Instance.GetCommandNamedText()), Singleton<TopBarInfoQueue>.Instance.GetCommandPortrait(), Singleton<TopBarInfoQueue>.Instance.GetCommandJabberText(), isGoodGuy: true);
		Singleton<TopBarInfoQueue>.Instance.RegisterRequest(intro2);
		yield return new WaitForSeconds(12.5f);
		Singleton<TopBarInfoQueue>.Instance.RemoveRequest(intro2);
		AmmoFeed[] ammoFeeds = Object.FindObjectsOfType<AmmoFeed>();
		AmmoFeed[] array3 = ammoFeeds;
		foreach (AmmoFeed ammoFeed in array3)
		{
			ammoFeed.SetInfiniteTutorial(infinite: true);
		}
		Singleton<MusicSelectionRules>.Instance.TriggerTimed(MusicSelectionRules.MusicTriggerEvents.TrainingMission1, 30f);
		Singleton<MusicSelectionRules>.Instance.Trigger(MusicSelectionRules.MusicTriggerEvents.TrainingMissionEver);
		yield return new WaitForSeconds(2f);
		StartCoroutine(DoCameraControlTutorial());
	}

	private IEnumerator DoCameraControlTutorial()
	{
		Singleton<ContextControl>.Instance.HidePrompts(select: true, move: true, tagging: true, zoom: false);
		float flipTime = ((!Singleton<UISelector>.Instance.IsPrimary()) ? 7.5f : 12.5f);
		TopBarInfoQueue.TopBarRequest zoomRequest = TopBarInfoQueue.TopBarRequest.Standard("tutorial_topbar_camera_zoom_pc", "tutorial_topbar_camera_zoom_controller", Singleton<EnumToIconMapping>.Instance.GetIconName(EnumToIconMapping.InteractionOrAlertType.Visibility), 0f, flipTime, flipTime, 1);
		Singleton<TopBarInfoQueue>.Instance.RegisterRequest(zoomRequest);
		TopBarInfoQueue.TopBarRequest zoomRequest2 = TopBarInfoQueue.TopBarRequest.Standard("tutorial_topbar_camera_zoom_pc_2", "tutorial_topbar_camera_zoom_controller_2", Singleton<EnumToIconMapping>.Instance.GetIconName(EnumToIconMapping.InteractionOrAlertType.Visibility), 0f, flipTime, flipTime, 1);
		Singleton<TopBarInfoQueue>.Instance.RegisterRequest(zoomRequest2);
		float timer2 = 0f;
		while (true)
		{
			if (!Singleton<UISelector>.Instance.IsPrimary())
			{
				if ((Singleton<BomberCamera>.Instance.HasZoomedEver() && timer2 > 15f) || timer2 > 20f)
				{
					break;
				}
			}
			else
			{
				if (Singleton<BomberCamera>.Instance.HasZoomedEver() && timer2 > 20f)
				{
					break;
				}
				if (timer2 > 7.5f)
				{
					DoMiniPrompt(m_miniPromptZoomCamera);
				}
			}
			timer2 += Time.deltaTime;
			yield return null;
		}
		Singleton<TopBarInfoQueue>.Instance.RemoveRequest(zoomRequest);
		Singleton<TopBarInfoQueue>.Instance.RemoveRequest(zoomRequest2);
		ClearMiniPrompt();
		TopBarInfoQueue.TopBarRequest cameraMoveRequest = TopBarInfoQueue.TopBarRequest.Standard("tutorial_topbar_camera_move_pc", "tutorial_topbar_camera_move_controller", Singleton<EnumToIconMapping>.Instance.GetIconName(EnumToIconMapping.InteractionOrAlertType.Visibility), 0f, 0f, 0f, 1);
		Singleton<TopBarInfoQueue>.Instance.RegisterRequest(cameraMoveRequest);
		timer2 = 0f;
		while (true)
		{
			if (Singleton<UISelector>.Instance.IsPrimary())
			{
				if ((Singleton<BomberCamera>.Instance.HasMovedEver() && timer2 > 10f) || timer2 > 20f)
				{
					break;
				}
			}
			else if (Singleton<BomberCamera>.Instance.HasMovedEver() && timer2 > 12.5f)
			{
				break;
			}
			timer2 += Time.deltaTime;
			yield return null;
		}
		Singleton<TopBarInfoQueue>.Instance.RemoveRequest(cameraMoveRequest);
		yield return new WaitForSeconds(2f);
		StartCoroutine(DoTaggingTutorial());
	}

	private IEnumerator DoTaggingTutorial()
	{
		Singleton<ContextControl>.Instance.HidePrompts(select: true, move: true, tagging: false, zoom: false);
		TopBarInfoQueue.TopBarRequest tagCommandRequestN = TopBarInfoQueue.TopBarRequest.Standard("tutorial_topbar_tag_drogues", null, Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.Gunning), 0f, 0f, 5f, 1);
		Singleton<ContextControl>.Instance.SetTargetingAllowed(allowed: true);
		TopBarInfoQueue.TopBarRequest tagCommandRequest = TopBarInfoQueue.TopBarRequest.Standard("tutorial_topbar_tag_pc", "tutorial_topbar_tag_controller", Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.Gunning), 0f, 0f, 5f, 1);
		ObjectiveManager.Objective tagFightersObjective = new ObjectiveManager.Objective
		{
			m_countToComplete = 3,
			m_objectiveType = ObjectiveManager.ObjectiveType.Tutorial,
			m_objectiveTitle = "tutorial_tag_drogues"
		};
		Singleton<ObjectiveManager>.Instance.RegisterObjective(tagFightersObjective);
		int numCompleted = 0;
		while (true)
		{
			if (Singleton<ContextControl>.Instance.IsTargetingMode())
			{
				Singleton<TopBarInfoQueue>.Instance.RegisterRequest(tagCommandRequestN);
				Singleton<TopBarInfoQueue>.Instance.RemoveRequest(tagCommandRequest);
			}
			else
			{
				Singleton<TopBarInfoQueue>.Instance.RegisterRequest(tagCommandRequest);
				Singleton<TopBarInfoQueue>.Instance.RemoveRequest(tagCommandRequestN);
			}
			List<FighterPlane> allFighters = Singleton<FighterCoordinator>.Instance.GetAllFighters();
			int numDestroyed = 3 - allFighters.Count;
			foreach (FighterPlane item in allFighters)
			{
				if (!item.GetComponent<TaggableFighter>().IsTaggable())
				{
					numDestroyed++;
				}
			}
			for (; numCompleted < Singleton<FighterCoordinator>.Instance.GetNumTaggedCurrently() + numDestroyed; numCompleted++)
			{
				Singleton<ObjectiveManager>.Instance.CompleteObjective(tagFightersObjective);
			}
			if (numCompleted == 3)
			{
				break;
			}
			yield return null;
		}
		Singleton<TopBarInfoQueue>.Instance.RemoveRequest(tagCommandRequestN);
		Singleton<TopBarInfoQueue>.Instance.RemoveRequest(tagCommandRequest);
		yield return new WaitForSeconds(2f);
		Singleton<ContextControl>.Instance.SetTargetingAllowed(allowed: true);
		TopBarInfoQueue.TopBarRequest tagLeaveRequest = TopBarInfoQueue.TopBarRequest.Standard("tutorial_topbar_leave_tag_pc", "tutorial_topbar_leave_tag_controller", Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.Gunning), 0f, 0f, 5f, 1);
		while (Singleton<ContextControl>.Instance.IsTargetingMode())
		{
			Singleton<TopBarInfoQueue>.Instance.RegisterRequest(tagLeaveRequest);
			yield return null;
		}
		Singleton<TopBarInfoQueue>.Instance.RemoveRequest(tagLeaveRequest);
		StartCoroutine(DoGunnerSelect());
	}

	private IEnumerator DoGunnerSelect()
	{
		yield return new WaitForSeconds(2f);
		Singleton<ContextControl>.Instance.HidePrompts(select: false, move: true, tagging: false, zoom: false);
		Singleton<ContextControl>.Instance.SetSelectCrewBlocked(blocked: false);
		List<CrewSpawner.CrewmanAvatarPairing> capList = Singleton<CrewSpawner>.Instance.GetAllCrew();
		CrewSpawner.CrewmanAvatarPairing gunnerCap = null;
		foreach (CrewSpawner.CrewmanAvatarPairing item in capList)
		{
			item.m_spawnedAvatar.SetSelectionBlocked(blocked: false);
			if (item.m_crewman.GetPrimarySkill().GetSkill() == Crewman.SpecialisationSkill.Gunning)
			{
				gunnerCap = item;
				item.m_spawnedAvatar.SetOrdersBlocked(blocked: false);
			}
		}
		TopBarInfoQueue.TopBarRequest selectCrewRequest = TopBarInfoQueue.TopBarRequest.Standard("tutorial_topbar_select_crew_pc", "tutorial_topbar_select_crew_controller", Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.Gunning), 0f, 0f, 5f, 1);
		selectCrewRequest.AddPointerHint(gunnerCap.m_spawnedAvatar.GetCrewmanGraphics().GetPelvisTransform().gameObject, Vector3.zero);
		selectCrewRequest.AddPointerHint(Singleton<CrewPopulator>.Instance.GetPanelFor(Singleton<CrewSpawner>.Instance.GetIndexFromCrewman(gunnerCap.m_spawnedAvatar)), new Vector3(96f, 40f, 0f), downArrow: true, worldSpace: false);
		TopBarInfoQueue.TopBarRequest moveToStationsRG = TopBarInfoQueue.TopBarRequest.Standard("tutorial_topbar_issue_command_pc_rg", "tutorial_topbar_issue_command_controller_rg", Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.Gunning), 0f, 0f, 5f, 1);
		moveToStationsRG.AddPointerHint(Singleton<BomberSpawn>.Instance.GetBomberSystems().GetStationFor(BomberSystems.StationType.GunsTail).gameObject, Vector3.zero);
		TopBarInfoQueue.TopBarRequest moveToStationsFG = TopBarInfoQueue.TopBarRequest.Standard("tutorial_topbar_issue_command_pc_fg", "tutorial_topbar_issue_command_controller_fg", Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.Gunning), 0f, 0f, 5f, 1);
		moveToStationsFG.AddPointerHint(Singleton<BomberSpawn>.Instance.GetBomberSystems().GetStationFor(BomberSystems.StationType.GunsNose).gameObject, Vector3.zero);
		TopBarInfoQueue.TopBarRequest moveToStationsUG = TopBarInfoQueue.TopBarRequest.Standard("tutorial_topbar_issue_command_pc_ug", "tutorial_topbar_issue_command_controller_ug", Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.Gunning), 0f, 0f, 5f, 1);
		moveToStationsUG.AddPointerHint(Singleton<BomberSpawn>.Instance.GetBomberSystems().GetStationFor(BomberSystems.StationType.GunsUpperDeck).gameObject, Vector3.zero);
		ObjectiveManager.Objective destroyFightersObjective = new ObjectiveManager.Objective
		{
			m_countToComplete = 3,
			m_objectiveType = ObjectiveManager.ObjectiveType.Tutorial,
			m_objectiveTitle = "tutorial_destroy_drogues"
		};
		Singleton<ObjectiveManager>.Instance.RegisterObjective(destroyFightersObjective);
		CrewmanAvatar gunner = null;
		List<CrewSpawner.CrewmanAvatarPairing> allCaps = Singleton<CrewSpawner>.Instance.GetAllCrew();
		foreach (CrewSpawner.CrewmanAvatarPairing item2 in allCaps)
		{
			if (item2.m_crewman.GetPrimarySkill().GetSkill() == Crewman.SpecialisationSkill.Gunning)
			{
				gunner = item2.m_spawnedAvatar;
			}
		}
		int numCompleted = 0;
		List<TopBarInfoQueue.TopBarRequest> dirReqs = new List<TopBarInfoQueue.TopBarRequest> { moveToStationsFG, moveToStationsRG, moveToStationsUG, selectCrewRequest };
		Singleton<ContextControl>.Instance.HidePrompts(select: false, move: false, tagging: false, zoom: false);
		float timer = 0f;
		while (true)
		{
			List<FighterPlane> allFighters = Singleton<FighterCoordinator>.Instance.GetAllFighters();
			int numDestroyed = 3 - allFighters.Count;
			foreach (FighterPlane item3 in allFighters)
			{
				if (!item3.GetComponent<TaggableFighter>().IsTaggable())
				{
					numDestroyed++;
				}
			}
			for (; numCompleted < numDestroyed; numCompleted++)
			{
				Singleton<ObjectiveManager>.Instance.CompleteObjective(destroyFightersObjective);
			}
			if (numCompleted == 3)
			{
				break;
			}
			TopBarInfoQueue.TopBarRequest req = null;
			bool shouldShowGoToHint = false;
			if (gunner.GetStation() != null)
			{
				Station station = gunner.GetStation();
				bool flag = false;
				if (station == Singleton<BomberSpawn>.Instance.GetBomberSystems().GetStationFor(BomberSystems.StationType.GunsUpperDeck) && m_upperPlane != null && m_upperPlane.GetComponent<TaggableFighter>().IsTaggable())
				{
					flag = true;
				}
				if (station == Singleton<BomberSpawn>.Instance.GetBomberSystems().GetStationFor(BomberSystems.StationType.GunsNose) && m_frontPlane != null && m_frontPlane.GetComponent<TaggableFighter>().IsTaggable())
				{
					flag = true;
				}
				if (station == Singleton<BomberSpawn>.Instance.GetBomberSystems().GetStationFor(BomberSystems.StationType.GunsTail) && m_rearPlane != null && m_rearPlane.GetComponent<TaggableFighter>().IsTaggable())
				{
					flag = true;
				}
				if (!flag)
				{
					if (Singleton<ContextControl>.Instance.GetCurrentlySelected() != gunner)
					{
						req = selectCrewRequest;
					}
					else
					{
						shouldShowGoToHint = true;
					}
				}
			}
			else if (gunner.IsIdle())
			{
				if (Singleton<ContextControl>.Instance.GetCurrentlySelected() != gunner)
				{
					req = selectCrewRequest;
				}
				else
				{
					shouldShowGoToHint = true;
				}
			}
			if (shouldShowGoToHint)
			{
				if (m_upperPlane != null && m_upperPlane.GetComponent<TaggableFighter>().IsTaggable())
				{
					req = moveToStationsUG;
				}
				else if (m_frontPlane != null && m_frontPlane.GetComponent<TaggableFighter>().IsTaggable())
				{
					req = moveToStationsFG;
				}
				else if (m_rearPlane != null && m_rearPlane.GetComponent<TaggableFighter>().IsTaggable())
				{
					req = moveToStationsRG;
				}
				if (timer > 15f)
				{
					DoMiniPrompt(m_miniPromptMove);
				}
				else
				{
					ClearMiniPrompt();
				}
			}
			else if (timer > 7.5f && req == selectCrewRequest)
			{
				DoMiniPrompt(m_miniPromptSelect);
			}
			else
			{
				ClearMiniPrompt();
			}
			timer += Time.unscaledDeltaTime;
			foreach (TopBarInfoQueue.TopBarRequest item4 in dirReqs)
			{
				if (item4 == req)
				{
					Singleton<TopBarInfoQueue>.Instance.RegisterRequest(item4);
				}
				else
				{
					Singleton<TopBarInfoQueue>.Instance.RemoveRequest(item4);
				}
			}
			yield return null;
		}
		foreach (TopBarInfoQueue.TopBarRequest item5 in dirReqs)
		{
			Singleton<TopBarInfoQueue>.Instance.RemoveRequest(item5);
		}
		ClearMiniPrompt();
		yield return null;
		StartCoroutine(ReturnToBase());
	}

	private IEnumerator ReturnToBase()
	{
		Singleton<MissionCoordinator>.Instance.SetOutwardLocked(locked: false);
		((StationPilot)Singleton<BomberSpawn>.Instance.GetBomberSystems().GetStationFor(BomberSystems.StationType.Pilot)).SetInactiveLocked(inactiveLocked: false);
		((StationNavigator)Singleton<BomberSpawn>.Instance.GetBomberSystems().GetStationFor(BomberSystems.StationType.Navigation)).SetInactiveLocked(inactiveLocked: false);
		((StationNavigator)Singleton<BomberSpawn>.Instance.GetBomberSystems().GetStationFor(BomberSystems.StationType.Navigation)).SetNavigationProgressForce(1f);
		yield return new WaitForSeconds(2f);
		ObjectiveManager.Objective navObjective = new ObjectiveManager.Objective
		{
			m_countToComplete = 1,
			m_objectiveType = ObjectiveManager.ObjectiveType.Tutorial,
			m_objectiveTitle = "tutorial_tag_navigation_marker"
		};
		Singleton<ObjectiveManager>.Instance.RegisterObjective(navObjective);
		TopBarInfoQueue.TopBarRequest tagCommandRequestN = TopBarInfoQueue.TopBarRequest.Standard("tutorial_topbar_tag_navigation", null, Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.Navigator), 0f, 0f, 5f, 1);
		Singleton<ContextControl>.Instance.SetTargetingAllowed(allowed: true);
		TopBarInfoQueue.TopBarRequest tagCommandRequest = TopBarInfoQueue.TopBarRequest.Standard("tutorial_topbar_tag_pc", "tutorial_topbar_tag_controller", Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.Navigator), 30f, 10f, 10f, 1);
		Singleton<TopBarInfoQueue>.Instance.RegisterRequest(tagCommandRequest);
		Singleton<TopBarInfoQueue>.Instance.RegisterRequest(tagCommandRequestN);
		while (!Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberNavigation().GetCurrentType()
			.HasValue)
		{
			yield return null;
		}
		Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState().SetBounds(new Vector3(-15000f, 0f, -15000f), new Vector3(25000f, 0f, 30000f));
		Singleton<TopBarInfoQueue>.Instance.RemoveRequest(tagCommandRequest);
		Singleton<TopBarInfoQueue>.Instance.RemoveRequest(tagCommandRequestN);
		Singleton<ObjectiveManager>.Instance.CompleteObjective(navObjective);
		StartCoroutine(CheckForNoNavigation());
		StartCoroutine(WaitForLandingRange());
		yield return new WaitForSeconds(7f);
		TopBarInfoQueue.TopBarRequest mid1 = TopBarInfoQueue.TopBarRequest.Speech("tutorial_speech_tutorial1_mid1", Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(Singleton<TopBarInfoQueue>.Instance.GetCommandNamedText()), Singleton<TopBarInfoQueue>.Instance.GetCommandPortrait(), Singleton<TopBarInfoQueue>.Instance.GetCommandJabberText(), isGoodGuy: true);
		Singleton<TopBarInfoQueue>.Instance.RegisterRequest(mid1);
		yield return new WaitForSeconds(8.5f);
		Singleton<TopBarInfoQueue>.Instance.RemoveRequest(mid1);
		Singleton<MissionCoordinator>.Instance.FireTrigger("TUTORIAL_SPAWN_FIGHTERS");
		StartCoroutine(DoAmbushText());
	}

	private IEnumerator DoAmbushText()
	{
		TopBarInfoQueue.TopBarRequest tagCommandRequestN = TopBarInfoQueue.TopBarRequest.Alert("tutorial_topbar_enemyfighters", null, Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.Gunning), 0f, 0f, 0f, 1);
		TopBarInfoQueue.TopBarRequest tagCommandRequest = TopBarInfoQueue.TopBarRequest.Standard("tutorial_topbar_enemyfighters_move", null, Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.Gunning), 0f, 10f, 5f, 1);
		TopBarInfoQueue.TopBarRequest radarSpeech = TopBarInfoQueue.TopBarRequest.Speech("tutorial_speech_tutorial1_radar", Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(Singleton<TopBarInfoQueue>.Instance.GetCommandNamedText()), Singleton<TopBarInfoQueue>.Instance.GetCommandPortrait(), Singleton<TopBarInfoQueue>.Instance.GetCommandJabberText(), isGoodGuy: true);
		radarSpeech.AddPointerHint("RADAR_DISPLAY", Vector3.zero, downArrow: true, worldSpace: false);
		Singleton<TopBarInfoQueue>.Instance.RegisterRequest(radarSpeech);
		StartCoroutine(DoSpitfires());
		tagCommandRequestN.AddSeeMore("TAGGING");
		yield return new WaitForSeconds(10f);
		while (true)
		{
			if (Singleton<FighterCoordinator>.Instance.AreAnyUntaggedFightersEngaged())
			{
				Singleton<TopBarInfoQueue>.Instance.RegisterRequest(tagCommandRequestN);
				Singleton<TopBarInfoQueue>.Instance.RemoveRequest(tagCommandRequest);
			}
			else
			{
				Singleton<TopBarInfoQueue>.Instance.RegisterRequest(tagCommandRequest);
				Singleton<TopBarInfoQueue>.Instance.RemoveRequest(tagCommandRequestN);
			}
			List<FighterPlane> fpList = Singleton<FighterCoordinator>.Instance.GetAllFighters();
			int numTagged = 0;
			foreach (FighterPlane item in fpList)
			{
				if (item.IsTagged())
				{
					numTagged++;
				}
			}
			if (!Singleton<FighterCoordinator>.Instance.AreAnyFightersEngaged() || numTagged >= 4)
			{
				break;
			}
			yield return null;
		}
		Singleton<TopBarInfoQueue>.Instance.RemoveRequest(tagCommandRequestN);
		Singleton<TopBarInfoQueue>.Instance.RemoveRequest(tagCommandRequest);
	}

	private IEnumerator DoSpitfires()
	{
		yield return new WaitForSeconds(10f);
		StationRadioOperator sr = (StationRadioOperator)Singleton<BomberSpawn>.Instance.GetBomberSystems().GetStationFor(BomberSystems.StationType.RadioOperator);
		sr.DoSpitfiresNoTimer();
		yield return new WaitForSeconds(10f);
		TopBarInfoQueue.TopBarRequest spitfirespeech = TopBarInfoQueue.TopBarRequest.Speech("tutorial_speech_tutorial1_spitfires", Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(Singleton<TopBarInfoQueue>.Instance.GetCommandNamedText()), Singleton<TopBarInfoQueue>.Instance.GetCommandPortrait(), Singleton<TopBarInfoQueue>.Instance.GetCommandJabberText(), isGoodGuy: true);
		Singleton<TopBarInfoQueue>.Instance.RegisterRequest(spitfirespeech);
	}

	private IEnumerator CheckForNoNavigation()
	{
		TopBarInfoQueue.TopBarRequest tagCommandRequestN = TopBarInfoQueue.TopBarRequest.Alert("tutorial_topbar_tag_navigation_long", null, Singleton<EnumToIconMapping>.Instance.GetIconName(EnumToIconMapping.InteractionOrAlertType.Navigate), 25f, 15f, 10f, 2);
		tagCommandRequestN.AddSeeMore("TAGGING");
		while (true)
		{
			Vector3d pos = Singleton<BomberSpawn>.Instance.GetBomberSystems().gameObject.btransform().position;
			pos.y = 0.0;
			if (!Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberNavigation().GetCurrentType()
				.HasValue && pos.magnitude >= 4000.0)
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

	private IEnumerator WaitForLandingRange()
	{
		while (true)
		{
			Vector3d pos = Singleton<BomberSpawn>.Instance.GetBomberSystems().gameObject.btransform().position;
			pos.y = 0.0;
			if (pos.magnitude < 4000.0)
			{
				break;
			}
			yield return null;
		}
		CrewSpawner.CrewmanAvatarPairing pilotCap = null;
		List<CrewSpawner.CrewmanAvatarPairing> capList = Singleton<CrewSpawner>.Instance.GetAllCrew();
		foreach (CrewSpawner.CrewmanAvatarPairing item in capList)
		{
			if (item.m_crewman.GetPrimarySkill().GetSkill() == Crewman.SpecialisationSkill.Piloting)
			{
				item.m_spawnedAvatar.SetOrdersBlocked(blocked: false);
				item.m_spawnedAvatar.SetMoveOrdersBlocked(blocked: true);
				pilotCap = item;
			}
		}
		Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState().SetBounds(new Vector3(-4000f, 0f, -4000f), new Vector3(4000f, 0f, 4000f));
		TopBarInfoQueue.TopBarRequest tagLandingRequest = TopBarInfoQueue.TopBarRequest.Standard("tutorial_tag_landing", null, Singleton<EnumToIconMapping>.Instance.GetIconName(EnumToIconMapping.InteractionOrAlertType.Landing), 0f, 0f, 0f, 1);
		Singleton<TopBarInfoQueue>.Instance.RegisterRequest(tagLandingRequest);
		while (!Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberNavigation().GetCurrentType()
			.HasValue || Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberNavigation().GetCurrentType()
			.Value != BomberNavigation.NavigationPointType.FunctionLandingBombing)
		{
			yield return null;
		}
		Singleton<TopBarInfoQueue>.Instance.RemoveRequest(tagLandingRequest);
		((StationNavigator)Singleton<BomberSpawn>.Instance.GetBomberSystems().GetStationFor(BomberSystems.StationType.Navigation)).SetInactiveLocked(inactiveLocked: true);
		TopBarInfoQueue.TopBarRequest landingGearDownRequest = TopBarInfoQueue.TopBarRequest.Standard("tutorial_landing_gear_low_alt", null, Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.Piloting), 0f, 0f, 0f, 1);
		landingGearDownRequest.AddPointerHint("LOWERGEAR", Vector3.zero, downArrow: true, worldSpace: false);
		TopBarInfoQueue.TopBarRequest landingGearDownRequestSelectPilot = TopBarInfoQueue.TopBarRequest.Standard("tutorial_landing_gear_low_alt_select_pilot", null, Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.Piloting), 0f, 0f, 0f, 1);
		landingGearDownRequestSelectPilot.AddPointerHint(pilotCap.m_spawnedAvatar.GetCrewmanGraphics().GetPelvisTransform().gameObject, Vector3.zero);
		landingGearDownRequestSelectPilot.AddPointerHint(Singleton<CrewPopulator>.Instance.GetPanelFor(Singleton<CrewSpawner>.Instance.GetIndexFromCrewman(pilotCap.m_spawnedAvatar)), new Vector3(96f, 40f, 0f), downArrow: true, worldSpace: false);
		ObjectiveManager.Objective lowerGearObjective = new ObjectiveManager.Objective
		{
			m_countToComplete = 1,
			m_objectiveType = ObjectiveManager.ObjectiveType.ReturnHome,
			m_objectiveTitle = "tutorial_land_at_base"
		};
		Singleton<ObjectiveManager>.Instance.RegisterObjective(lowerGearObjective);
		TopBarInfoQueue.TopBarRequest landingWait = TopBarInfoQueue.TopBarRequest.Standard("tutorial_topbar_landing_wait", null, Singleton<EnumToIconMapping>.Instance.GetIconName(Crewman.SpecialisationSkill.Piloting), 0f, 0f, 0f, 1);
		while (true)
		{
			if ((!Singleton<BomberSpawn>.Instance.GetBomberSystems().GetLandingGear().IsFullyLowered() && !Singleton<BomberSpawn>.Instance.GetBomberSystems().GetLandingGear().IsLowering()) || Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState().GetAltitudeTarget() != 0)
			{
				if (Singleton<ContextControl>.Instance.GetCurrentlySelected() == null || Singleton<ContextControl>.Instance.GetCurrentlySelected().GetCrewman().GetPrimarySkill()
					.GetSkill() != 0)
				{
					Singleton<TopBarInfoQueue>.Instance.RegisterRequest(landingGearDownRequestSelectPilot);
					Singleton<TopBarInfoQueue>.Instance.RemoveRequest(landingGearDownRequest);
				}
				else
				{
					Singleton<TopBarInfoQueue>.Instance.RegisterRequest(landingGearDownRequest);
					Singleton<TopBarInfoQueue>.Instance.RemoveRequest(landingGearDownRequestSelectPilot);
				}
			}
			else
			{
				Singleton<TopBarInfoQueue>.Instance.RemoveRequest(landingGearDownRequestSelectPilot);
				Singleton<TopBarInfoQueue>.Instance.RemoveRequest(landingGearDownRequest);
			}
			if (Singleton<BomberSpawn>.Instance.GetBomberSystems().GetLandingGear().IsFullyLowered() && Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBomberState().GetAltitudeTarget() == 0)
			{
				Singleton<ObjectiveManager>.Instance.CompleteObjective(lowerGearObjective);
				Singleton<TopBarInfoQueue>.Instance.RegisterRequest(landingWait);
				Singleton<TopBarInfoQueue>.Instance.RemoveRequest(landingGearDownRequestSelectPilot);
				Singleton<TopBarInfoQueue>.Instance.RemoveRequest(landingGearDownRequest);
			}
			else
			{
				Singleton<TopBarInfoQueue>.Instance.RemoveRequest(landingWait);
			}
			yield return null;
		}
	}
}
