using BomberCrewCommon;
using UnityEngine;

public class PlatformSpecificFlowPS4 : Singleton<PlatformSpecificFlowPS4>, LoadableSystem
{
	[SerializeField]
	private string m_titleId;

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
		return "PS4_SPECIFIC";
	}

	public LoadableSystem[] GetDependencies()
	{
		return null;
	}
}
