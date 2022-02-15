using BomberCrewCommon;
using UnityEngine;

public class XPRequirements : Singleton<XPRequirements>
{
	[SerializeField]
	private int[] m_xpRequiredForNextLevel;

	[SerializeField]
	private int m_maxLevel = 12;

	public int GetXPRequiredForLevel(int level)
	{
		return m_xpRequiredForNextLevel[level - 1];
	}
}
