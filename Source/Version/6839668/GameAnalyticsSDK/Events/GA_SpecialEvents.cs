using System.Collections;
using UnityEngine;

namespace GameAnalyticsSDK.Events;

public class GA_SpecialEvents : MonoBehaviour
{
	private static int _frameCountAvg;

	private static float _lastUpdateAvg;

	private int _frameCountCrit;

	private float _lastUpdateCrit;

	private static int _criticalFpsCount;

	public void Start()
	{
		StartCoroutine(SubmitFPSRoutine());
		StartCoroutine(CheckCriticalFPSRoutine());
	}

	private IEnumerator SubmitFPSRoutine()
	{
		while (Application.isPlaying && GameAnalytics.SettingsGA.SubmitFpsAverage)
		{
			yield return new WaitForSeconds(30f);
			SubmitFPS();
		}
	}

	private IEnumerator CheckCriticalFPSRoutine()
	{
		while (Application.isPlaying && GameAnalytics.SettingsGA.SubmitFpsCritical)
		{
			yield return new WaitForSeconds(GameAnalytics.SettingsGA.FpsCirticalSubmitInterval);
			CheckCriticalFPS();
		}
	}

	public void Update()
	{
		if (GameAnalytics.SettingsGA.SubmitFpsAverage)
		{
			_frameCountAvg++;
		}
		if (GameAnalytics.SettingsGA.SubmitFpsCritical)
		{
			_frameCountCrit++;
		}
	}

	public static void SubmitFPS()
	{
		if (GameAnalytics.SettingsGA.SubmitFpsAverage)
		{
			float num = Time.time - _lastUpdateAvg;
			if (num > 1f)
			{
				float num2 = (float)_frameCountAvg / num;
				_lastUpdateAvg = Time.time;
				_frameCountAvg = 0;
				if (num2 > 0f)
				{
					GameAnalytics.NewDesignEvent("GA:AverageFPS", (int)num2);
				}
			}
		}
		if (GameAnalytics.SettingsGA.SubmitFpsCritical && _criticalFpsCount > 0)
		{
			GameAnalytics.NewDesignEvent("GA:CriticalFPS", _criticalFpsCount);
			_criticalFpsCount = 0;
		}
	}

	public void CheckCriticalFPS()
	{
		if (!GameAnalytics.SettingsGA.SubmitFpsCritical)
		{
			return;
		}
		float num = Time.time - _lastUpdateCrit;
		if (num >= 1f)
		{
			float num2 = (float)_frameCountCrit / num;
			_lastUpdateCrit = Time.time;
			_frameCountCrit = 0;
			if (num2 <= (float)GameAnalytics.SettingsGA.FpsCriticalThreshold)
			{
				_criticalFpsCount++;
			}
		}
	}
}
