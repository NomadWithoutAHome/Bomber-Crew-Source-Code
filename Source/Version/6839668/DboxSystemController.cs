using BomberCrewCommon;
using dbox;
using UnityEngine;

public class DboxSystemController : Singleton<DboxSystemController>
{
	private void OnEnable()
	{
		try
		{
			DboxSdkWrapper.InitializeDbox("BomberCrew_RunnerDuck", Singleton<VersionInfo>.Instance.GetChangeListVersion());
			DboxSdkWrapper.OpenDbox();
		}
		catch
		{
			Debug.LogError("[DBOX] Didn't initialise!");
		}
		Object.DontDestroyOnLoad(base.gameObject);
	}

	private void OnApplicationQuit()
	{
		try
		{
			DboxSdkWrapper.StopDbox();
			DboxSdkWrapper.CloseDbox();
			DboxSdkWrapper.TerminateDbox();
		}
		catch
		{
		}
	}
}
