using BomberCrewCommon;
using UnityEngine;

public class PlatformSpecificFlowXboxOne : Singleton<PlatformSpecificFlowXboxOne>, LoadableSystem
{
	[SerializeField]
	private GameObject m_signedOutPrompt;

	public void ContinueLoad()
	{
	}

	public LoadableSystem[] GetDependencies()
	{
		return null;
	}

	public bool IsLoadComplete()
	{
		return true;
	}

	public void StartLoad()
	{
	}

	public string GetName()
	{
		return "PLATFORM_XBOXONE";
	}
}
