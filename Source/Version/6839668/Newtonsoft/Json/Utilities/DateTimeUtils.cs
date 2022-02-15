using System;
using System.Globalization;
using System.Xml;

namespace Newtonsoft.Json.Utilities;

internal static class DateTimeUtils
{
	public static string GetLocalOffset(this DateTime d)
	{
		TimeSpan utcOffset = TimeZoneInfo.Local.GetUtcOffset(d);
		return utcOffset.Hours.ToString("+00;-00", CultureInfo.InvariantCulture) + ":" + utcOffset.Minutes.ToString("00;00", CultureInfo.InvariantCulture);
	}

	public static XmlDateTimeSerializationMode ToSerializationMode(DateTimeKind kind)
	{
		return kind switch
		{
			DateTimeKind.Local => XmlDateTimeSerializationMode.Local, 
			DateTimeKind.Unspecified => XmlDateTimeSerializationMode.Unspecified, 
			DateTimeKind.Utc => XmlDateTimeSerializationMode.Utc, 
			_ => throw MiscellaneousUtils.CreateArgumentOutOfRangeException("kind", kind, "Unexpected DateTimeKind value."), 
		};
	}
}
