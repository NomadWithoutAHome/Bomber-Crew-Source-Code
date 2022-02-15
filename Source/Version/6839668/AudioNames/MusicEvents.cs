using WingroveAudio;

namespace AudioNames;

public static class MusicEvents
{
	public static class Events
	{
		public const string KeyMissionBegin = "KEY_MISSION_BEGIN";

		public const string MusicAceFighter01 = "MUSIC_ACE_FIGHTER_01";

		public const string MusicAceFighter02 = "MUSIC_ACE_FIGHTER_02";

		public const string MusicAceFighter03 = "MUSIC_ACE_FIGHTER_03";

		public const string MusicAceFighter04 = "MUSIC_ACE_FIGHTER_04";

		public const string MusicAceFighter05 = "MUSIC_ACE_FIGHTER_05";

		public const string MusicAceFighter06 = "MUSIC_ACE_FIGHTER_06";

		public const string MusicAceFighter07 = "MUSIC_ACE_FIGHTER_07";

		public const string MusicAceFighterA = "MUSIC_ACE_FIGHTER_A";

		public const string MusicAceFighterB = "MUSIC_ACE_FIGHTER_B";

		public const string MusicDangerTense = "MUSIC_DANGER_TENSE";

		public const string MusicDangerTenseSparse = "MUSIC_DANGER_TENSE_SPARSE";

		public const string MusicFightersA = "MUSIC_FIGHTERS_A";

		public const string MusicFightersB = "MUSIC_FIGHTERS_B";

		public const string MusicFightersC = "MUSIC_FIGHTERS_C";

		public const string MusicMute = "MUSIC_MUTE";

		public const string MusicReturnOk = "MUSIC_RETURN_OK";

		public const string MusicStop = "MUSIC_STOP";

		public const string MusicStopFast = "MUSIC_STOP_FAST";

		public const string MusicUnmute = "MUSIC_UNMUTE";

		public const string ObjectiveCompleteBad = "OBJECTIVE_COMPLETE_BAD";

		public const string ObjectiveCompleteGood = "OBJECTIVE_COMPLETE_GOOD";

		public const string ObjectiveFailed = "OBJECTIVE_FAILED";

		public const string MusicAceFighter08 = "MUSIC_ACE_FIGHTER_08";

		public const string MusicSparseGood = "MUSIC_SPARSE_GOOD";

		public const string MusicStealth = "MUSIC_STEALTH";

		public const string MusicHazardsRegualr = "MUSIC_HAZARDS_REGUALR";

		public const string MusicHazardsTense = "MUSIC_HAZARDS_TENSE";

		public const string MusicAceDefeated = "MUSIC_ACE_DEFEATED";

		public const string RadioPlaylist = "RADIO_PLAYLIST";

		public const string RadioStatic = "RADIO_STATIC";

		public const string MusicDescent = "MUSIC_DESCENT";

		public const string Tutorial1Begin = "TUTORIAL1_BEGIN";

		public const string Tutorial2Begin = "TUTORIAL2_BEGIN";

		public const string MusicDangerTenseCrewlost = "MUSIC_DANGER_TENSE_CREWLOST";

		public const string MusicOverenglandGood = "MUSIC_OVERENGLAND_GOOD";

		public const string StartMissionCamera = "START_MISSION_CAMERA";

		public const string Endcredits = "ENDCREDITS";

		public const string MusicAceFighter09 = "MUSIC_ACE_FIGHTER_09";

		public const string MusicAceFighter10 = "MUSIC_ACE_FIGHTER_10";

		public const string MusicAceFighter11 = "MUSIC_ACE_FIGHTER_11";

		public const string MusicSnowstorm = "MUSIC_SNOWSTORM";

		public const string MusicArid = "MUSIC_ARID";

		public const string MusicAceFighter12 = "MUSIC_ACE_FIGHTER_12";

		public const string MusicSparseGoodArid = "MUSIC_SPARSE_GOOD_ARID";

		public const string MusicAceFighter13 = "MUSIC_ACE_FIGHTER_13";

		public const string MusicAceFighter14 = "MUSIC_ACE_FIGHTER_14";

		public const string MusicAceFighter15 = "MUSIC_ACE_FIGHTER_15";

		public const string StartMissionCameraUsaaf = "START_MISSION_CAMERA_USAAF";

		public const string MusicFightersBArid = "MUSIC_FIGHTERS_B_ARID";

		public const string EndcreditsUs = "ENDCREDITS_US";
	}

	public static class Parameters
	{
		public const string MusicDaynight = "MUSIC_DAYNIGHT";

		private static int CacheVal_MusicDaynight_Internal;

		public const string MusicXmas = "MUSIC_XMAS";

		private static int CacheVal_MusicXmas_Internal;

		public static int CacheVal_MusicDaynight()
		{
			if (CacheVal_MusicDaynight_Internal == 0)
			{
				CacheVal_MusicDaynight_Internal = WingroveRoot.Instance.GetParameterId("MUSIC_DAYNIGHT");
			}
			return CacheVal_MusicDaynight_Internal;
		}

		public static int CacheVal_MusicXmas()
		{
			if (CacheVal_MusicXmas_Internal == 0)
			{
				CacheVal_MusicXmas_Internal = WingroveRoot.Instance.GetParameterId("MUSIC_XMAS");
			}
			return CacheVal_MusicXmas_Internal;
		}
	}
}
