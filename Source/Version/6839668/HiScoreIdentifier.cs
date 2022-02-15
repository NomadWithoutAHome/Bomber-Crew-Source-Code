using System;
using UnityEngine;

[Serializable]
public class HiScoreIdentifier
{
	[SerializeField]
	private string m_steamIdentifier;

	[SerializeField]
	private string m_xboxIdentifier;

	[SerializeField]
	private int m_xboxStatIdentifier;

	[SerializeField]
	private int m_switchIdentifier;

	[SerializeField]
	private int m_ps4Id;

	public int GetPS4Id()
	{
		return m_ps4Id;
	}

	public string GetSteamIdentifier()
	{
		return m_steamIdentifier;
	}

	public string GetXboxIdentifier()
	{
		return m_xboxIdentifier;
	}

	public int GetXboxStatIdentifier()
	{
		return m_xboxStatIdentifier;
	}

	public int GetSwitchIdentifier()
	{
		return m_switchIdentifier;
	}

	public bool Matches(HiScoreIdentifier other)
	{
		return m_steamIdentifier == other.m_steamIdentifier;
	}
}
