using System;
using System.Collections.Generic;
using UnityEngine;

public class MED_DamageObjectsOnTrigger : LevelItem
{
	[SerializeField]
	private LevelItem[] m_otherObjects;

	[SerializeField]
	private string m_trigger;

	public override string GetName()
	{
		return "DamageObjectsOnTrigger";
	}

	public override Dictionary<string, string> GetParameters(Func<LevelItem, string> resolverFunc)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["trigger"] = m_trigger;
		dictionary["numObjects"] = m_otherObjects.Length.ToString();
		for (int i = 0; i < m_otherObjects.Length; i++)
		{
			dictionary["object" + i] = resolverFunc(m_otherObjects[i]);
		}
		return dictionary;
	}

	public override bool HasSize()
	{
		return false;
	}
}
