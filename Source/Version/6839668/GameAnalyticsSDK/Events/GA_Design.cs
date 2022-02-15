using GameAnalyticsSDK.Wrapper;

namespace GameAnalyticsSDK.Events;

public static class GA_Design
{
	public static void NewEvent(string eventName, float eventValue)
	{
		CreateNewEvent(eventName, eventValue);
	}

	public static void NewEvent(string eventName)
	{
		CreateNewEvent(eventName, null);
	}

	private static void CreateNewEvent(string eventName, float? eventValue)
	{
		if (eventValue.HasValue)
		{
			GA_Wrapper.AddDesignEvent(eventName, eventValue.Value);
		}
		else
		{
			GA_Wrapper.AddDesignEvent(eventName);
		}
	}
}
