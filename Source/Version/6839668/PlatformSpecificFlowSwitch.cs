using BomberCrewCommon;

public class PlatformSpecificFlowSwitch : Singleton<PlatformSpecificFlowSwitch>, LoadableSystem
{
	public bool IsLoadComplete()
	{
		return true;
	}

	public void StartLoad()
	{
	}

	public void ContinueLoad()
	{
	}

	public string GetName()
	{
		return "[SWITCH_SPECIFIC]";
	}

	public LoadableSystem[] GetDependencies()
	{
		return null;
	}
}
