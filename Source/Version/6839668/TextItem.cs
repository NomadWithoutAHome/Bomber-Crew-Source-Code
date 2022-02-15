using System;

[Serializable]
public class TextItem
{
	public string key;

	public string value;

	public string tag;

	public string notes;

	public string group;

	public int version;

	public bool dirty;

	public bool keyLocked;

	public bool isDuplicate;

	public void CleanUpFormattingBeforeSave()
	{
		value = value.Replace("\\n", "\n");
		char newChar = '\'';
		char oldChar = '‘';
		char oldChar2 = '’';
		value = value.Replace(oldChar, newChar);
		value = value.Replace(oldChar2, newChar);
	}
}
