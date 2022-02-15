using GameAnalyticsSDK.Wrapper;

namespace GameAnalyticsSDK.Events;

public static class GA_Business
{
	public static void NewEvent(string currency, int amount, string itemType, string itemId, string cartType)
	{
		GA_Wrapper.AddBusinessEvent(currency, amount, itemType, itemId, cartType);
	}
}
