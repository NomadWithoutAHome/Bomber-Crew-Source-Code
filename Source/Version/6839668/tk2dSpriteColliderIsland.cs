using System;
using UnityEngine;

[Serializable]
public class tk2dSpriteColliderIsland
{
	public bool connected = true;

	public Vector2[] points;

	public bool IsValid()
	{
		if (connected)
		{
			return points.Length >= 3;
		}
		return points.Length >= 2;
	}

	public void CopyFrom(tk2dSpriteColliderIsland src)
	{
		connected = src.connected;
		points = new Vector2[src.points.Length];
		for (int i = 0; i < points.Length; i++)
		{
			ref Vector2 reference = ref points[i];
			reference = src.points[i];
		}
	}

	public bool CompareTo(tk2dSpriteColliderIsland src)
	{
		if (connected != src.connected)
		{
			return false;
		}
		if (points.Length != src.points.Length)
		{
			return false;
		}
		for (int i = 0; i < points.Length; i++)
		{
			if (points[i] != src.points[i])
			{
				return false;
			}
		}
		return true;
	}
}
