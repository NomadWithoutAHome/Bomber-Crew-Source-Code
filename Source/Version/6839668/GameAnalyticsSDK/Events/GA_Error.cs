using GameAnalyticsSDK.Wrapper;

namespace GameAnalyticsSDK.Events;

public static class GA_Error
{
	public static void NewEvent(GAErrorSeverity severity, string message)
	{
		CreateNewEvent(severity, message);
	}

	private static void CreateNewEvent(GAErrorSeverity severity, string message)
	{
		GA_Wrapper.AddErrorEvent(severity, message);
	}
}
