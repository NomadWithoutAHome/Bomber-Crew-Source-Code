using System.Collections.Generic;

namespace GameAnalyticsSDK.Setup;

public class Studio
{
	public string Name { get; private set; }

	public string ID { get; private set; }

	public List<Game> Games { get; private set; }

	public Studio(string name, string id, List<Game> games)
	{
		Name = name;
		ID = id;
		Games = games;
	}

	public static string[] GetStudioNames(List<Studio> studios, bool addFirstEmpty = true)
	{
		if (studios == null)
		{
			return new string[1] { "-" };
		}
		if (addFirstEmpty)
		{
			string[] array = new string[studios.Count + 1];
			array[0] = "-";
			string text = string.Empty;
			for (int i = 0; i < studios.Count; i++)
			{
				array[i + 1] = studios[i].Name + text;
				text += " ";
			}
			return array;
		}
		string[] array2 = new string[studios.Count];
		string text2 = string.Empty;
		for (int j = 0; j < studios.Count; j++)
		{
			array2[j] = studios[j].Name + text2;
			text2 += " ";
		}
		return array2;
	}

	public static string[] GetGameNames(int index, List<Studio> studios)
	{
		if (studios == null || studios[index].Games == null)
		{
			return new string[1] { "-" };
		}
		string[] array = new string[studios[index].Games.Count + 1];
		array[0] = "-";
		string text = string.Empty;
		for (int i = 0; i < studios[index].Games.Count; i++)
		{
			array[i + 1] = studios[index].Games[i].Name + text;
			text += " ";
		}
		return array;
	}
}
