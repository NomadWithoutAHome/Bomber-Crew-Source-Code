using WingroveAudio;

namespace AudioNames;

public static class GUIEvents
{
	public static class Events
	{
		public const string ButtonDown = "BUTTON_DOWN";

		public const string ButtonUp = "BUTTON_UP";

		public const string SelectCrewman = "SELECT_CREWMAN";

		public const string DeselectCrewman = "DESELECT_CREWMAN";

		public const string GiveOrder = "GIVE_ORDER";

		public const string BadClick = "BAD_CLICK";

		public const string TargetingModeOn = "TARGETING_MODE_ON";

		public const string TargetingModeOff = "TARGETING_MODE_OFF";

		public const string TargetLocked = "TARGET_LOCKED";

		public const string TargetingStart = "TARGETING_START";

		public const string TargetingStop = "TARGETING_STOP";

		public const string LevelupStop = "LEVELUP_STOP";

		public const string LevelupStart = "LEVELUP_START";

		public const string LevelupUp = "LEVELUP_UP";

		public const string LevelupEnd = "LEVELUP_END";

		public const string CrewmanEquipClothing = "CREWMAN_EQUIP_CLOTHING";

		public const string CrewmanEquipBoots = "CREWMAN_EQUIP_BOOTS";

		public const string Money = "MONEY";

		public const string BuildUpgrade = "BUILD_UPGRADE";

		public const string TutorialDing = "TUTORIAL_DING";

		public const string ObjectiveComplete = "OBJECTIVE_COMPLETE";

		public const string KeyMissionBeginStart = "KEY_MISSION_BEGIN_START";

		public const string DebriefBomberDestroyed = "DEBRIEF_BOMBER_DESTROYED";

		public const string DebriefBomberOk = "DEBRIEF_BOMBER_OK";

		public const string DebriefObjectiveComplete = "DEBRIEF_OBJECTIVE_COMPLETE";

		public const string DebriefObjectiveFail = "DEBRIEF_OBJECTIVE_FAIL";

		public const string DebriefDrumrollStart = "DEBRIEF_DRUMROLL_START";

		public const string DebriefSurvivalGood = "DEBRIEF_SURVIVAL_GOOD";

		public const string DebriefSurvivalBad = "DEBRIEF_SURVIVAL_BAD";

		public const string DebriefDrumrollStop = "DEBRIEF_DRUMROLL_STOP";

		public const string TutorialDingHigh = "TUTORIAL_DING_HIGH";

		public const string DebriefAceDefeat = "DEBRIEF_ACE_DEFEAT";

		public const string DebriefAceEscape = "DEBRIEF_ACE_ESCAPE";

		public const string CollectEndlessPowerup = "COLLECT_ENDLESS_POWERUP";

		public const string EndlessOverlayAppear = "ENDLESS_OVERLAY_APPEAR";

		public const string EndlessOverlayAppear2 = "ENDLESS_OVERLAY_APPEAR2";

		public const string EndlessOverlayFail = "ENDLESS_OVERLAY_FAIL";

		public const string EndlessOverlayWave = "ENDLESS_OVERLAY_WAVE";

		public const string EndlessOverlayScore = "ENDLESS_OVERLAY_SCORE";

		public const string EndlessOverlayDisappear = "ENDLESS_OVERLAY_DISAPPEAR";
	}

	public static class Parameters
	{
		public const string LevelUpProgress = "LEVEL_UP_PROGRESS";

		private static int CacheVal_LevelUpProgress_Internal;

		public const string TargetingProgress = "TARGETING_PROGRESS";

		private static int CacheVal_TargetingProgress_Internal;

		public static int CacheVal_LevelUpProgress()
		{
			if (CacheVal_LevelUpProgress_Internal == 0)
			{
				CacheVal_LevelUpProgress_Internal = WingroveRoot.Instance.GetParameterId("LEVEL_UP_PROGRESS");
			}
			return CacheVal_LevelUpProgress_Internal;
		}

		public static int CacheVal_TargetingProgress()
		{
			if (CacheVal_TargetingProgress_Internal == 0)
			{
				CacheVal_TargetingProgress_Internal = WingroveRoot.Instance.GetParameterId("TARGETING_PROGRESS");
			}
			return CacheVal_TargetingProgress_Internal;
		}
	}
}
