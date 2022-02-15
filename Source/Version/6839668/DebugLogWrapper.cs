using System.Diagnostics;
using UnityEngine;

public static class DebugLogWrapper
{
	[Conditional("DOLOGS")]
	public static void Log(object message)
	{
		UnityEngine.Debug.Log(message);
	}

	[Conditional("DOLOGS")]
	public static void LogWarning(object message)
	{
		UnityEngine.Debug.LogWarning(message);
	}

	[Conditional("DOLOGS")]
	public static void Log(object message, Object context)
	{
		UnityEngine.Debug.Log(message, context);
	}

	[Conditional("DOLOGS")]
	public static void LogWarning(object message, Object context)
	{
		UnityEngine.Debug.LogWarning(message, context);
	}

	public static void LogError(object message)
	{
		UnityEngine.Debug.LogError(message);
	}

	public static void LogError(object message, Object context)
	{
		UnityEngine.Debug.LogError(message, context);
	}
}
