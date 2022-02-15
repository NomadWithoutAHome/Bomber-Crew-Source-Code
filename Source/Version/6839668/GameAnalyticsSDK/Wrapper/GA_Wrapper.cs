using System.Collections;
using GameAnalyticsSDK.Net;
using GameAnalyticsSDK.State;
using GameAnalyticsSDK.Utilities;
using UnityEngine;

namespace GameAnalyticsSDK.Wrapper;

public class GA_Wrapper
{
	private static void configureAvailableCustomDimensions01(string list)
	{
		ArrayList arrayList = (ArrayList)GA_MiniJSON.JsonDecode(list);
		GameAnalyticsSDK.Net.GameAnalytics.ConfigureAvailableCustomDimensions01((string[])arrayList.ToArray(typeof(string)));
	}

	private static void configureAvailableCustomDimensions02(string list)
	{
		ArrayList arrayList = (ArrayList)GA_MiniJSON.JsonDecode(list);
		GameAnalyticsSDK.Net.GameAnalytics.ConfigureAvailableCustomDimensions02((string[])arrayList.ToArray(typeof(string)));
	}

	private static void configureAvailableCustomDimensions03(string list)
	{
		ArrayList arrayList = (ArrayList)GA_MiniJSON.JsonDecode(list);
		GameAnalyticsSDK.Net.GameAnalytics.ConfigureAvailableCustomDimensions03((string[])arrayList.ToArray(typeof(string)));
	}

	private static void configureAvailableResourceCurrencies(string list)
	{
		ArrayList arrayList = (ArrayList)GA_MiniJSON.JsonDecode(list);
		GameAnalyticsSDK.Net.GameAnalytics.ConfigureAvailableResourceCurrencies((string[])arrayList.ToArray(typeof(string)));
	}

	private static void configureAvailableResourceItemTypes(string list)
	{
		ArrayList arrayList = (ArrayList)GA_MiniJSON.JsonDecode(list);
		GameAnalyticsSDK.Net.GameAnalytics.ConfigureAvailableResourceItemTypes((string[])arrayList.ToArray(typeof(string)));
	}

	private static void configureSdkGameEngineVersion(string unitySdkVersion)
	{
		GameAnalyticsSDK.Net.GameAnalytics.ConfigureSdkGameEngineVersion(unitySdkVersion);
	}

	private static void configureGameEngineVersion(string unityEngineVersion)
	{
		GameAnalyticsSDK.Net.GameAnalytics.ConfigureGameEngineVersion(unityEngineVersion);
	}

	private static void configureBuild(string build)
	{
		GameAnalyticsSDK.Net.GameAnalytics.ConfigureBuild(build);
	}

	private static void configureUserId(string userId)
	{
		GameAnalyticsSDK.Net.GameAnalytics.ConfigureUserId(userId);
	}

	private static void initialize(string gamekey, string gamesecret)
	{
		GameAnalyticsSDK.Net.GameAnalytics.Initialize(gamekey, gamesecret);
	}

	private static void setCustomDimension01(string customDimension)
	{
		GameAnalyticsSDK.Net.GameAnalytics.SetCustomDimension01(customDimension);
	}

	private static void setCustomDimension02(string customDimension)
	{
		GameAnalyticsSDK.Net.GameAnalytics.SetCustomDimension02(customDimension);
	}

	private static void setCustomDimension03(string customDimension)
	{
		GameAnalyticsSDK.Net.GameAnalytics.SetCustomDimension03(customDimension);
	}

	private static void addBusinessEvent(string currency, int amount, string itemType, string itemId, string cartType)
	{
		GameAnalyticsSDK.Net.GameAnalytics.AddBusinessEvent(currency, amount, itemType, itemId, cartType);
	}

	private static void addResourceEvent(int flowType, string currency, float amount, string itemType, string itemId)
	{
		GameAnalyticsSDK.Net.GameAnalytics.AddResourceEvent((EGAResourceFlowType)flowType, currency, amount, itemType, itemId);
	}

	private static void addProgressionEvent(int progressionStatus, string progression01, string progression02, string progression03)
	{
		GameAnalyticsSDK.Net.GameAnalytics.AddProgressionEvent((EGAProgressionStatus)progressionStatus, progression01, progression02, progression03);
	}

	private static void addProgressionEventWithScore(int progressionStatus, string progression01, string progression02, string progression03, int score)
	{
		GameAnalyticsSDK.Net.GameAnalytics.AddProgressionEvent((EGAProgressionStatus)progressionStatus, progression01, progression02, progression03, score);
	}

	private static void addDesignEvent(string eventId)
	{
		GameAnalyticsSDK.Net.GameAnalytics.AddDesignEvent(eventId);
	}

	private static void addDesignEventWithValue(string eventId, float value)
	{
		GameAnalyticsSDK.Net.GameAnalytics.AddDesignEvent(eventId, value);
	}

	private static void addErrorEvent(int severity, string message)
	{
		GameAnalyticsSDK.Net.GameAnalytics.AddErrorEvent((EGAErrorSeverity)severity, message);
	}

	private static void setEnabledInfoLog(bool enabled)
	{
		GameAnalyticsSDK.Net.GameAnalytics.SetEnabledInfoLog(enabled);
	}

	private static void setEnabledVerboseLog(bool enabled)
	{
		GameAnalyticsSDK.Net.GameAnalytics.SetEnabledVerboseLog(enabled);
	}

	private static void setManualSessionHandling(bool enabled)
	{
		GameAnalyticsSDK.Net.GameAnalytics.SetEnabledManualSessionHandling(enabled);
	}

	private static void gameAnalyticsStartSession()
	{
		GameAnalyticsSDK.Net.GameAnalytics.StartSession();
	}

	private static void gameAnalyticsEndSession()
	{
		GameAnalyticsSDK.Net.GameAnalytics.EndSession();
	}

	private static void setFacebookId(string facebookId)
	{
		GameAnalyticsSDK.Net.GameAnalytics.SetFacebookId(facebookId);
	}

	private static void setGender(string gender)
	{
		switch (gender)
		{
		case "male":
			GameAnalyticsSDK.Net.GameAnalytics.SetGender(EGAGender.Male);
			break;
		case "female":
			GameAnalyticsSDK.Net.GameAnalytics.SetGender(EGAGender.Female);
			break;
		}
	}

	private static void setBirthYear(int birthYear)
	{
		GameAnalyticsSDK.Net.GameAnalytics.SetBirthYear(birthYear);
	}

	public static void SetAvailableCustomDimensions01(string list)
	{
		configureAvailableCustomDimensions01(list);
	}

	public static void SetAvailableCustomDimensions02(string list)
	{
		configureAvailableCustomDimensions02(list);
	}

	public static void SetAvailableCustomDimensions03(string list)
	{
		configureAvailableCustomDimensions03(list);
	}

	public static void SetAvailableResourceCurrencies(string list)
	{
		configureAvailableResourceCurrencies(list);
	}

	public static void SetAvailableResourceItemTypes(string list)
	{
		configureAvailableResourceItemTypes(list);
	}

	public static void SetUnitySdkVersion(string unitySdkVersion)
	{
		configureSdkGameEngineVersion(unitySdkVersion);
	}

	public static void SetUnityEngineVersion(string unityEngineVersion)
	{
		configureGameEngineVersion(unityEngineVersion);
	}

	public static void SetBuild(string build)
	{
		configureBuild(build);
	}

	public static void SetCustomUserId(string userId)
	{
		configureUserId(userId);
	}

	public static void SetEnabledManualSessionHandling(bool enabled)
	{
		setManualSessionHandling(enabled);
	}

	public static void StartSession()
	{
		if (GAState.IsManualSessionHandlingEnabled())
		{
			gameAnalyticsStartSession();
		}
		else
		{
			Debug.Log("Manual session handling is not enabled. \nPlease check the \"Use manual session handling\" option in the \"Advanced\" section of the Settings object.");
		}
	}

	public static void EndSession()
	{
		if (GAState.IsManualSessionHandlingEnabled())
		{
			gameAnalyticsEndSession();
		}
		else
		{
			Debug.Log("Manual session handling is not enabled. \nPlease check the \"Use manual session handling\" option in the \"Advanced\" section of the Settings object.");
		}
	}

	public static void Initialize(string gamekey, string gamesecret)
	{
		initialize(gamekey, gamesecret);
	}

	public static void SetCustomDimension01(string customDimension)
	{
		setCustomDimension01(customDimension);
	}

	public static void SetCustomDimension02(string customDimension)
	{
		setCustomDimension02(customDimension);
	}

	public static void SetCustomDimension03(string customDimension)
	{
		setCustomDimension03(customDimension);
	}

	public static void AddBusinessEvent(string currency, int amount, string itemType, string itemId, string cartType)
	{
		addBusinessEvent(currency, amount, itemType, itemId, cartType);
	}

	public static void AddResourceEvent(GAResourceFlowType flowType, string currency, float amount, string itemType, string itemId)
	{
		addResourceEvent((int)flowType, currency, amount, itemType, itemId);
	}

	public static void AddProgressionEvent(GAProgressionStatus progressionStatus, string progression01, string progression02, string progression03)
	{
		addProgressionEvent((int)progressionStatus, progression01, progression02, progression03);
	}

	public static void AddProgressionEventWithScore(GAProgressionStatus progressionStatus, string progression01, string progression02, string progression03, int score)
	{
		addProgressionEventWithScore((int)progressionStatus, progression01, progression02, progression03, score);
	}

	public static void AddDesignEvent(string eventID, float eventValue)
	{
		addDesignEventWithValue(eventID, eventValue);
	}

	public static void AddDesignEvent(string eventID)
	{
		addDesignEvent(eventID);
	}

	public static void AddErrorEvent(GAErrorSeverity severity, string message)
	{
		addErrorEvent((int)severity, message);
	}

	public static void SetInfoLog(bool enabled)
	{
		setEnabledInfoLog(enabled);
	}

	public static void SetVerboseLog(bool enabled)
	{
		setEnabledVerboseLog(enabled);
	}

	public static void SetFacebookId(string facebookId)
	{
		setFacebookId(facebookId);
	}

	public static void SetGender(string gender)
	{
		setGender(gender);
	}

	public static void SetBirthYear(int birthYear)
	{
		setBirthYear(birthYear);
	}
}
