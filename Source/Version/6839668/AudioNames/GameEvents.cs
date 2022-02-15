using WingroveAudio;

namespace AudioNames;

public static class GameEvents
{
	public static class Events
	{
		public const string AirbaseEnter = "AIRBASE_ENTER";

		public const string AirbaseIndoorsEnter = "AIRBASE_INDOORS_ENTER";

		public const string AirbaseIndoorsLeave = "AIRBASE_INDOORS_LEAVE";

		public const string AirbaseLeave = "AIRBASE_LEAVE";

		public const string ArtilleryFire = "ARTILLERY_FIRE";

		public const string BoatStart = "BOAT_START";

		public const string BoatStop = "BOAT_STOP";

		public const string BombDrop = "BOMB_DROP";

		public const string BombDropStop = "BOMB_DROP_STOP";

		public const string BombExplode = "BOMB_EXPLODE";

		public const string BomberGunfireJammed = "BOMBER_GUNFIRE_JAMMED";

		public const string BomberGunfireSingle = "BOMBER_GUNFIRE_SINGLE";

		public const string BomberGunfireStart = "BOMBER_GUNFIRE_START";

		public const string BomberGunfireStop = "BOMBER_GUNFIRE_STOP";

		public const string BomberPieceDisconnect = "BOMBER_PIECE_DISCONNECT";

		public const string BomberStart = "BOMBER_START";

		public const string BomberStop = "BOMBER_STOP";

		public const string Clapping = "CLAPPING";

		public const string CloudEnter = "CLOUD_ENTER";

		public const string CloudExit = "CLOUD_EXIT";

		public const string CloudPuff = "CLOUD_PUFF";

		public const string CrewmanSpeechStart = "CREWMAN_SPEECH_START";

		public const string ElectricalSpark = "ELECTRICAL_SPARK";

		public const string ElectricalSystemRepaired = "ELECTRICAL_SYSTEM_REPAIRED";

		public const string ElectricalSystemShutdown = "ELECTRICAL_SYSTEM_SHUTDOWN";

		public const string EngineIgnition = "ENGINE_IGNITION";

		public const string EngineSlowOff = "ENGINE_SLOW_OFF";

		public const string EngineStart = "ENGINE_START";

		public const string EngineStop = "ENGINE_STOP";

		public const string ExtinguishStart = "EXTINGUISH_START";

		public const string ExtinguishStop = "EXTINGUISH_STOP";

		public const string ExtinguisherEmpty = "EXTINGUISHER_EMPTY";

		public const string FighterEngineStart = "FIGHTER_ENGINE_START";

		public const string FighterEngineStop = "FIGHTER_ENGINE_STOP";

		public const string FighterGunfireSingleshot = "FIGHTER_GUNFIRE_SINGLESHOT";

		public const string FireExpand = "FIRE_EXPAND";

		public const string FireStart = "FIRE_START";

		public const string FireStop = "FIRE_STOP";

		public const string FlakAmbientStart = "FLAK_AMBIENT_START";

		public const string FlakAmbientStop = "FLAK_AMBIENT_STOP";

		public const string FlakExplode = "FLAK_EXPLODE";

		public const string Gunfire50cal = "GUNFIRE_50CAL";

		public const string Gunfire50calhi = "GUNFIRE_50CALHI";

		public const string GunfireFw = "GUNFIRE_FW";

		public const string GunfireFwhi = "GUNFIRE_FWHI";

		public const string GunfireM240 = "GUNFIRE_M240";

		public const string HangarEnter = "HANGAR_ENTER";

		public const string HangarLeave = "HANGAR_LEAVE";

		public const string HydraulicBrokenRandom = "HYDRAULIC_BROKEN_RANDOM";

		public const string HydraulicRepair = "HYDRAULIC_REPAIR";

		public const string HydraulicShutDown = "HYDRAULIC_SHUT_DOWN";

		public const string HydraulicsStart = "HYDRAULICS_START";

		public const string HydraulicsStop = "HYDRAULICS_STOP";

		public const string ImpactCharacter = "IMPACT_CHARACTER";

		public const string ImpactFuselage = "IMPACT_FUSELAGE";

		public const string ImpactFuselageHe = "IMPACT_FUSELAGE_HE";

		public const string ImpactGround = "IMPACT_GROUND";

		public const string ImpactHeavy = "IMPACT_HEAVY";

		public const string KeyMissionBeginEnd = "KEY_MISSION_BEGIN_END";

		public const string LandBigCrash = "LAND_BIG_CRASH";

		public const string LandSmallCrash = "LAND_SMALL_CRASH";

		public const string LandSuccess = "LAND_SUCCESS";

		public const string LightningStrike = "LIGHTNING_STRIKE";

		public const string LiteExplode = "LITE_EXPLODE";

		public const string Me163FighterEngineStart = "ME163_FIGHTER_ENGINE_START";

		public const string Me163FighterEngineStop = "ME163_FIGHTER_ENGINE_STOP";

		public const string Me262FighterEngineStart = "ME262_FIGHTER_ENGINE_START";

		public const string Me262FighterEngineStop = "ME262_FIGHTER_ENGINE_STOP";

		public const string MemorialEnter = "MEMORIAL_ENTER";

		public const string MemorialLeave = "MEMORIAL_LEAVE";

		public const string MissionEnd = "MISSION_END";

		public const string MorseCodeIncoming = "MORSE_CODE_INCOMING";

		public const string OxygenBrokenRandom = "OXYGEN_BROKEN_RANDOM";

		public const string PauseGame = "PAUSE_GAME";

		public const string PhotoCameraClick = "PHOTO_CAMERA_CLICK";

		public const string PhotoGoodConfirm = "PHOTO_GOOD_CONFIRM";

		public const string PigeonCoo = "PIGEON_COO";

		public const string RainStart = "RAIN_START";

		public const string RainStop = "RAIN_STOP";

		public const string RecruitmentEnter = "RECRUITMENT_ENTER";

		public const string RecruitmentLeave = "RECRUITMENT_LEAVE";

		public const string RepairCancel = "REPAIR_CANCEL";

		public const string RepairFinish = "REPAIR_FINISH";

		public const string RepairStart = "REPAIR_START";

		public const string RocketFire = "ROCKET_FIRE";

		public const string RocketLoop = "ROCKET_LOOP";

		public const string RocketLoopStop = "ROCKET_LOOP_STOP";

		public const string SpitfireFighterEngineStart = "SPITFIRE_FIGHTER_ENGINE_START";

		public const string SpitfireFighterEngineStop = "SPITFIRE_FIGHTER_ENGINE_STOP";

		public const string SplashExplode = "SPLASH_EXPLODE";

		public const string StationGunnerReloadFinish = "STATION_GUNNER_RELOAD_FINISH";

		public const string StationGunnerReplaceBelts = "STATION_GUNNER_REPLACE_BELTS";

		public const string StopClapping = "STOP_CLAPPING";

		public const string TutorialEnd = "TUTORIAL_END";

		public const string TutorialStart = "TUTORIAL_START";

		public const string UnpauseGame = "UNPAUSE_GAME";

		public const string V2LaunchStart = "V2_LAUNCH_START";

		public const string V2LaunchStop = "V2_LAUNCH_STOP";

		public const string WirelessChannel0 = "WIRELESS_CHANNEL_0";

		public const string WirelessChannel1 = "WIRELESS_CHANNEL_1";

		public const string WirelessOff = "WIRELESS_OFF";

		public const string WirelessOn = "WIRELESS_ON";

		public const string WirelessRetune = "WIRELESS_RETUNE";

		public const string OxygenShutdown = "OXYGEN_SHUTDOWN";

		public const string OxygenRepaired = "OXYGEN_REPAIRED";

		public const string Gunfire50calAa = "GUNFIRE_50CAL_AA";

		public const string GunfireM240cannon = "GUNFIRE_M240CANNON";

		public const string GunfireCannonhi = "GUNFIRE_CANNONHI";

		public const string SingleClap = "SINGLE_CLAP";

		public const string StukaStart = "STUKA_START";

		public const string StukaStop = "STUKA_STOP";
	}

	public static class Parameters
	{
		public const string BomberHoles = "BOMBER_HOLES";

		private static int CacheVal_BomberHoles_Internal;

		public const string BomberSpeed = "BOMBER_SPEED";

		private static int CacheVal_BomberSpeed_Internal;

		public const string CameraZoomLevel = "CAMERA_ZOOM_LEVEL";

		private static int CacheVal_CameraZoomLevel_Internal;

		public const string EngineIndividualSpeed = "ENGINE_INDIVIDUAL_SPEED";

		private static int CacheVal_EngineIndividualSpeed_Internal;

		public const string EngineSpeed = "ENGINE_SPEED";

		private static int CacheVal_EngineSpeed_Internal;

		public const string ExtinguisherCapacity = "EXTINGUISHER_CAPACITY";

		private static int CacheVal_ExtinguisherCapacity_Internal;

		public const string FireSize = "FIRE_SIZE";

		private static int CacheVal_FireSize_Internal;

		public const string GameSpeedMultiplier1to3 = "GAME_SPEED_MULTIPLIER1TO3";

		private static int CacheVal_GameSpeedMultiplier1to3_Internal;

		public const string ObjectDistance = "OBJECT_DISTANCE";

		private static int CacheVal_ObjectDistance_Internal;

		public const string RepairSkill = "REPAIR_SKILL";

		private static int CacheVal_RepairSkill_Internal;

		public const string TutorialFadeProgress = "TUTORIAL_FADE_PROGRESS";

		private static int CacheVal_TutorialFadeProgress_Internal;

		public const string GameSpeedMultiplier1tohalf = "GAME_SPEED_MULTIPLIER1TOHALF";

		private static int CacheVal_GameSpeedMultiplier1tohalf_Internal;

		public static int CacheVal_BomberHoles()
		{
			if (CacheVal_BomberHoles_Internal == 0)
			{
				CacheVal_BomberHoles_Internal = WingroveRoot.Instance.GetParameterId("BOMBER_HOLES");
			}
			return CacheVal_BomberHoles_Internal;
		}

		public static int CacheVal_BomberSpeed()
		{
			if (CacheVal_BomberSpeed_Internal == 0)
			{
				CacheVal_BomberSpeed_Internal = WingroveRoot.Instance.GetParameterId("BOMBER_SPEED");
			}
			return CacheVal_BomberSpeed_Internal;
		}

		public static int CacheVal_CameraZoomLevel()
		{
			if (CacheVal_CameraZoomLevel_Internal == 0)
			{
				CacheVal_CameraZoomLevel_Internal = WingroveRoot.Instance.GetParameterId("CAMERA_ZOOM_LEVEL");
			}
			return CacheVal_CameraZoomLevel_Internal;
		}

		public static int CacheVal_EngineIndividualSpeed()
		{
			if (CacheVal_EngineIndividualSpeed_Internal == 0)
			{
				CacheVal_EngineIndividualSpeed_Internal = WingroveRoot.Instance.GetParameterId("ENGINE_INDIVIDUAL_SPEED");
			}
			return CacheVal_EngineIndividualSpeed_Internal;
		}

		public static int CacheVal_EngineSpeed()
		{
			if (CacheVal_EngineSpeed_Internal == 0)
			{
				CacheVal_EngineSpeed_Internal = WingroveRoot.Instance.GetParameterId("ENGINE_SPEED");
			}
			return CacheVal_EngineSpeed_Internal;
		}

		public static int CacheVal_ExtinguisherCapacity()
		{
			if (CacheVal_ExtinguisherCapacity_Internal == 0)
			{
				CacheVal_ExtinguisherCapacity_Internal = WingroveRoot.Instance.GetParameterId("EXTINGUISHER_CAPACITY");
			}
			return CacheVal_ExtinguisherCapacity_Internal;
		}

		public static int CacheVal_FireSize()
		{
			if (CacheVal_FireSize_Internal == 0)
			{
				CacheVal_FireSize_Internal = WingroveRoot.Instance.GetParameterId("FIRE_SIZE");
			}
			return CacheVal_FireSize_Internal;
		}

		public static int CacheVal_GameSpeedMultiplier1to3()
		{
			if (CacheVal_GameSpeedMultiplier1to3_Internal == 0)
			{
				CacheVal_GameSpeedMultiplier1to3_Internal = WingroveRoot.Instance.GetParameterId("GAME_SPEED_MULTIPLIER1TO3");
			}
			return CacheVal_GameSpeedMultiplier1to3_Internal;
		}

		public static int CacheVal_ObjectDistance()
		{
			if (CacheVal_ObjectDistance_Internal == 0)
			{
				CacheVal_ObjectDistance_Internal = WingroveRoot.Instance.GetParameterId("OBJECT_DISTANCE");
			}
			return CacheVal_ObjectDistance_Internal;
		}

		public static int CacheVal_RepairSkill()
		{
			if (CacheVal_RepairSkill_Internal == 0)
			{
				CacheVal_RepairSkill_Internal = WingroveRoot.Instance.GetParameterId("REPAIR_SKILL");
			}
			return CacheVal_RepairSkill_Internal;
		}

		public static int CacheVal_TutorialFadeProgress()
		{
			if (CacheVal_TutorialFadeProgress_Internal == 0)
			{
				CacheVal_TutorialFadeProgress_Internal = WingroveRoot.Instance.GetParameterId("TUTORIAL_FADE_PROGRESS");
			}
			return CacheVal_TutorialFadeProgress_Internal;
		}

		public static int CacheVal_GameSpeedMultiplier1tohalf()
		{
			if (CacheVal_GameSpeedMultiplier1tohalf_Internal == 0)
			{
				CacheVal_GameSpeedMultiplier1tohalf_Internal = WingroveRoot.Instance.GetParameterId("GAME_SPEED_MULTIPLIER1TOHALF");
			}
			return CacheVal_GameSpeedMultiplier1tohalf_Internal;
		}
	}
}
