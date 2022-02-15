using System.Text.RegularExpressions;
using GameAnalyticsSDK.State;

namespace GameAnalyticsSDK.Validators;

internal static class GAValidator
{
	public static bool StringMatch(string s, string pattern)
	{
		if (s == null || pattern == null)
		{
			return false;
		}
		return Regex.IsMatch(s, pattern);
	}

	public static bool ValidateBusinessEvent(string currency, int amount, string cartType, string itemType, string itemId)
	{
		if (!ValidateCurrency(currency))
		{
			return false;
		}
		if (!ValidateShortString(cartType, canBeEmpty: true))
		{
			return false;
		}
		if (!ValidateEventPartLength(itemType, allowNull: false))
		{
			return false;
		}
		if (!ValidateEventPartCharacters(itemType))
		{
			return false;
		}
		if (!ValidateEventPartLength(itemId, allowNull: false))
		{
			return false;
		}
		if (!ValidateEventPartCharacters(itemId))
		{
			return false;
		}
		return true;
	}

	public static bool ValidateResourceEvent(GAResourceFlowType flowType, string currency, float amount, string itemType, string itemId)
	{
		if (string.IsNullOrEmpty(currency))
		{
			return false;
		}
		if (flowType == GAResourceFlowType.Undefined)
		{
		}
		if (!GAState.HasAvailableResourceCurrency(currency))
		{
			return false;
		}
		if (!(amount > 0f))
		{
			return false;
		}
		if (string.IsNullOrEmpty(itemType))
		{
			return false;
		}
		if (!ValidateEventPartLength(itemType, allowNull: false))
		{
			return false;
		}
		if (!ValidateEventPartCharacters(itemType))
		{
			return false;
		}
		if (!GAState.HasAvailableResourceItemType(itemType))
		{
			return false;
		}
		if (!ValidateEventPartLength(itemId, allowNull: false))
		{
			return false;
		}
		if (!ValidateEventPartCharacters(itemId))
		{
			return false;
		}
		return true;
	}

	public static bool ValidateProgressionEvent(GAProgressionStatus progressionStatus, string progression01, string progression02, string progression03)
	{
		if (progressionStatus == GAProgressionStatus.Undefined)
		{
			return false;
		}
		if (!string.IsNullOrEmpty(progression03) && string.IsNullOrEmpty(progression02) && !string.IsNullOrEmpty(progression01))
		{
			return false;
		}
		if (!string.IsNullOrEmpty(progression02) && string.IsNullOrEmpty(progression01))
		{
			return false;
		}
		if (string.IsNullOrEmpty(progression01))
		{
			return false;
		}
		if (!ValidateEventPartLength(progression01, allowNull: false))
		{
			return false;
		}
		if (!ValidateEventPartCharacters(progression01))
		{
			return false;
		}
		if (!string.IsNullOrEmpty(progression02))
		{
			if (!ValidateEventPartLength(progression02, allowNull: true))
			{
				return false;
			}
			if (!ValidateEventPartCharacters(progression02))
			{
				return false;
			}
		}
		if (!string.IsNullOrEmpty(progression03))
		{
			if (!ValidateEventPartLength(progression03, allowNull: true))
			{
				return false;
			}
			if (!ValidateEventPartCharacters(progression03))
			{
				return false;
			}
		}
		return true;
	}

	public static bool ValidateDesignEvent(string eventId)
	{
		if (!ValidateEventIdLength(eventId))
		{
			return false;
		}
		if (!ValidateEventIdCharacters(eventId))
		{
			return false;
		}
		return true;
	}

	public static bool ValidateErrorEvent(GAErrorSeverity severity, string message)
	{
		if (severity == GAErrorSeverity.Undefined)
		{
			return false;
		}
		if (!ValidateLongString(message, canBeEmpty: true))
		{
			return false;
		}
		return true;
	}

	public static bool ValidateSdkErrorEvent(string gameKey, string gameSecret, GAErrorSeverity type)
	{
		if (!ValidateKeys(gameKey, gameSecret))
		{
			return false;
		}
		if (type == GAErrorSeverity.Undefined)
		{
			return false;
		}
		return true;
	}

	public static bool ValidateKeys(string gameKey, string gameSecret)
	{
		if (StringMatch(gameKey, "^[A-z0-9]{32}$") && StringMatch(gameSecret, "^[A-z0-9]{40}$"))
		{
			return true;
		}
		return false;
	}

	public static bool ValidateCurrency(string currency)
	{
		if (string.IsNullOrEmpty(currency))
		{
			return false;
		}
		if (!StringMatch(currency, "^[A-Z]{3}$"))
		{
			return false;
		}
		return true;
	}

	public static bool ValidateEventPartLength(string eventPart, bool allowNull)
	{
		if (allowNull && string.IsNullOrEmpty(eventPart))
		{
			return true;
		}
		if (string.IsNullOrEmpty(eventPart))
		{
			return false;
		}
		if (eventPart.Length > 64)
		{
			return false;
		}
		return true;
	}

	public static bool ValidateEventPartCharacters(string eventPart)
	{
		if (!StringMatch(eventPart, "^[A-Za-z0-9\\s\\-_\\.\\(\\)\\!\\?]{1,64}$"))
		{
			return false;
		}
		return true;
	}

	public static bool ValidateEventIdLength(string eventId)
	{
		if (string.IsNullOrEmpty(eventId))
		{
			return false;
		}
		if (!StringMatch(eventId, "^[^:]{1,64}(?::[^:]{1,64}){0,4}$"))
		{
			return false;
		}
		return true;
	}

	public static bool ValidateEventIdCharacters(string eventId)
	{
		if (string.IsNullOrEmpty(eventId))
		{
			return false;
		}
		if (!StringMatch(eventId, "^[A-Za-z0-9\\s\\-_\\.\\(\\)\\!\\?]{1,64}(:[A-Za-z0-9\\s\\-_\\.\\(\\)\\!\\?]{1,64}){0,4}$"))
		{
			return false;
		}
		return true;
	}

	public static bool ValidateBuild(string build)
	{
		if (!ValidateShortString(build, canBeEmpty: false))
		{
			return false;
		}
		return true;
	}

	public static bool ValidateUserId(string uId)
	{
		if (!ValidateString(uId, canBeEmpty: false))
		{
			return false;
		}
		return true;
	}

	public static bool ValidateShortString(string shortString, bool canBeEmpty)
	{
		if (canBeEmpty && string.IsNullOrEmpty(shortString))
		{
			return true;
		}
		if (string.IsNullOrEmpty(shortString) || shortString.Length > 32)
		{
			return false;
		}
		return true;
	}

	public static bool ValidateString(string s, bool canBeEmpty)
	{
		if (canBeEmpty && string.IsNullOrEmpty(s))
		{
			return true;
		}
		if (string.IsNullOrEmpty(s) || s.Length > 64)
		{
			return false;
		}
		return true;
	}

	public static bool ValidateLongString(string longString, bool canBeEmpty)
	{
		if (canBeEmpty && string.IsNullOrEmpty(longString))
		{
			return true;
		}
		if (string.IsNullOrEmpty(longString) || longString.Length > 8192)
		{
			return false;
		}
		return true;
	}

	public static bool ValidateConnectionType(string connectionType)
	{
		return StringMatch(connectionType, "^(wwan|wifi|lan|offline)$");
	}

	public static bool ValidateCustomDimensions(params string[] customDimensions)
	{
		return ValidateArrayOfStrings(20L, 32L, allowNoValues: false, "custom dimensions", customDimensions);
	}

	public static bool ValidateResourceCurrencies(params string[] resourceCurrencies)
	{
		if (!ValidateArrayOfStrings(20L, 64L, allowNoValues: false, "resource currencies", resourceCurrencies))
		{
			return false;
		}
		foreach (string s in resourceCurrencies)
		{
			if (!StringMatch(s, "^[A-Za-z]+$"))
			{
				return false;
			}
		}
		return true;
	}

	public static bool ValidateResourceItemTypes(params string[] resourceItemTypes)
	{
		if (!ValidateArrayOfStrings(20L, 32L, allowNoValues: false, "resource item types", resourceItemTypes))
		{
			return false;
		}
		foreach (string eventPart in resourceItemTypes)
		{
			if (!ValidateEventPartCharacters(eventPart))
			{
				return false;
			}
		}
		return true;
	}

	public static bool ValidateDimension01(string dimension01)
	{
		if (string.IsNullOrEmpty(dimension01))
		{
			return false;
		}
		if (!GAState.HasAvailableCustomDimensions01(dimension01))
		{
			return false;
		}
		return true;
	}

	public static bool ValidateDimension02(string dimension02)
	{
		if (string.IsNullOrEmpty(dimension02))
		{
			return false;
		}
		if (!GAState.HasAvailableCustomDimensions02(dimension02))
		{
			return false;
		}
		return true;
	}

	public static bool ValidateDimension03(string dimension03)
	{
		if (string.IsNullOrEmpty(dimension03))
		{
			return false;
		}
		if (!GAState.HasAvailableCustomDimensions03(dimension03))
		{
			return false;
		}
		return true;
	}

	public static bool ValidateArrayOfStrings(long maxCount, long maxStringLength, bool allowNoValues, string logTag, params string[] arrayOfStrings)
	{
		string value = logTag;
		if (string.IsNullOrEmpty(value))
		{
			value = "Array";
		}
		if (arrayOfStrings == null)
		{
			return false;
		}
		if (!allowNoValues && arrayOfStrings.Length == 0)
		{
			return false;
		}
		if (maxCount > 0 && arrayOfStrings.Length > maxCount)
		{
			return false;
		}
		for (int i = 0; i < arrayOfStrings.Length; i++)
		{
			int num = arrayOfStrings[i]?.Length ?? 0;
			if (num == 0)
			{
				return false;
			}
			if (maxStringLength > 0 && num > maxStringLength)
			{
				return false;
			}
		}
		return true;
	}

	public static bool ValidateFacebookId(string facebookId)
	{
		if (!ValidateString(facebookId, canBeEmpty: false))
		{
			return false;
		}
		return true;
	}

	public static bool ValidateGender(string gender)
	{
		if (gender == string.Empty || (!(gender == GAGender.male.ToString()) && !(gender == GAGender.female.ToString())))
		{
			return false;
		}
		return true;
	}

	public static bool ValidateBirthyear(int birthYear)
	{
		if (birthYear < 0 || birthYear > 9999)
		{
			return false;
		}
		return true;
	}

	public static bool ValidateClientTs(long clientTs)
	{
		if (clientTs < -9223372036854775807L || clientTs > 9223372036854775806L)
		{
			return false;
		}
		return true;
	}
}
