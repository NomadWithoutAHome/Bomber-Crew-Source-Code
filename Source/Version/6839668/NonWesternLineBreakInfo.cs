using System.Collections.Generic;

public class NonWesternLineBreakInfo
{
	private static bool m_generatedCache = false;

	private static string m_disallowedStartCharacters = "!%),.:;?]}¢°·’\"†‡›℃∶、。〃〆〕〗〞﹚﹜！＂％＇），．：；？！］｝～)]｝〕〉》」』】〙〗〟’\"｠»ヽヾーァィゥェォッャュョヮヵヶぁぃぅぇぉっゃゅょゎゕゖㇰㇱㇲㇳㇴㇵㇶㇷㇸㇹㇺㇻㇼㇽㇾㇿ々〻‐゠–〜?!‼⁇⁈⁉・、:;,。.—…‥〳〴〵1234567890!%),.:;?]}¢°’\"†‡℃〆〈《「『〕！％），．：；？］｝ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

	private static string m_disallowedEndCharacters = "$(£¥·‘\"〈《「『【〔〖〝﹙﹛＄（．［｛￡￥([｛〔〈《「『【〘〖〝‘\"｟«—…‥〳〴〵1234567890$([\\{£¥‘\"々〇〉》」〔＄（［｛｠￥￦ #ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

	private static HashSet<char> m_disallowedStartHashset = new HashSet<char>();

	private static HashSet<char> m_disallowedEndHashset = new HashSet<char>();

	public static HashSet<char> GetDisallowedCharactersStart()
	{
		if (!m_generatedCache)
		{
			GenerateCache();
		}
		return m_disallowedStartHashset;
	}

	public static HashSet<char> GetDisallowedCharactersEnd()
	{
		if (!m_generatedCache)
		{
			GenerateCache();
		}
		return m_disallowedEndHashset;
	}

	private static void GenerateCache()
	{
		string disallowedStartCharacters = m_disallowedStartCharacters;
		foreach (char item in disallowedStartCharacters)
		{
			m_disallowedStartHashset.Add(item);
		}
		string disallowedEndCharacters = m_disallowedEndCharacters;
		foreach (char item2 in disallowedEndCharacters)
		{
			m_disallowedEndHashset.Add(item2);
		}
	}
}
