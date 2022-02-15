using AudioNames;
using BomberCrewCommon;
using UnityEngine;
using WingroveAudio;

public class MissionConfig : MonoBehaviour
{
	private void Start()
	{
		bool flag = GetComponent<MissionPlaceableObject>().GetParameter("winterEnvironment") == "true";
		if (Singleton<EnvironmentMaterialSetup>.Instance != null)
		{
			Singleton<EnvironmentMaterialSetup>.Instance.SetWinter(flag);
		}
		Singleton<BomberWeatherEffects>.Instance.EnableLightSnowflakeEffects(flag);
		if (flag)
		{
			Singleton<GameFlow>.Instance.GetCurrentMissionInfo().GetMissionLog().SetWinterEnvironment();
			Singleton<BomberSpawn>.Instance.GetBomberSystems().GetTemperatureOxygen().SetWinterEnvironmentOn();
			Singleton<MusicSelectionRules>.Instance.Trigger(MusicSelectionRules.MusicTriggerEvents.WinterEnvironment);
		}
		WingroveRoot.Instance.SetParameterGlobal(MusicEvents.Parameters.CacheVal_MusicXmas(), flag ? 1 : 0);
	}

	public float GetStartTime()
	{
		string parameter = GetComponent<MissionPlaceableObject>().GetParameter("timeOfDay");
		float result = 0f;
		float.TryParse(parameter, out result);
		return result;
	}
}
