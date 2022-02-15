using System.Collections;
using System.Text;

namespace GameAnalyticsSDK.Utilities;

public class GA_MiniJSON
{
	public const int TOKEN_NONE = 0;

	public const int TOKEN_CURLY_OPEN = 1;

	public const int TOKEN_CURLY_CLOSE = 2;

	public const int TOKEN_SQUARED_OPEN = 3;

	public const int TOKEN_SQUARED_CLOSE = 4;

	public const int TOKEN_COLON = 5;

	public const int TOKEN_COMMA = 6;

	public const int TOKEN_STRING = 7;

	public const int TOKEN_NUMBER = 8;

	public const int TOKEN_TRUE = 9;

	public const int TOKEN_FALSE = 10;

	public const int TOKEN_NULL = 11;

	private const int BUILDER_CAPACITY = 2000;

	protected static GA_MiniJSON instance = new GA_MiniJSON();

	protected int lastErrorIndex = -1;

	protected string lastDecode = string.Empty;

	public static object JsonDecode(string json)
	{
		instance.lastDecode = json;
		if (json != null)
		{
			char[] json2 = json.ToCharArray();
			int index = 0;
			bool success = true;
			object result = instance.ParseValue(json2, ref index, ref success);
			if (success)
			{
				instance.lastErrorIndex = -1;
			}
			else
			{
				instance.lastErrorIndex = index;
			}
			return result;
		}
		return null;
	}

	public static string JsonEncode(object json)
	{
		StringBuilder stringBuilder = new StringBuilder(2000);
		return (!instance.SerializeValue(json, stringBuilder)) ? null : stringBuilder.ToString();
	}

	public static bool LastDecodeSuccessful()
	{
		return instance.lastErrorIndex == -1;
	}

	public static int GetLastErrorIndex()
	{
		return instance.lastErrorIndex;
	}

	public static string GetLastErrorSnippet()
	{
		if (instance.lastErrorIndex == -1)
		{
			return string.Empty;
		}
		int num = instance.lastErrorIndex - 5;
		int num2 = instance.lastErrorIndex + 15;
		if (num < 0)
		{
			num = 0;
		}
		if (num2 >= instance.lastDecode.Length)
		{
			num2 = instance.lastDecode.Length - 1;
		}
		return instance.lastDecode.Substring(num, num2 - num + 1);
	}

	protected Hashtable ParseObject(char[] json, ref int index)
	{
		Hashtable hashtable = new Hashtable();
		NextToken(json, ref index);
		bool flag = false;
		while (!flag)
		{
			switch (LookAhead(json, index))
			{
			case 0:
				return null;
			case 6:
				NextToken(json, ref index);
				continue;
			case 2:
				NextToken(json, ref index);
				return hashtable;
			}
			string text = ParseString(json, ref index);
			if (text == null)
			{
				return null;
			}
			int num = NextToken(json, ref index);
			if (num != 5)
			{
				return null;
			}
			bool success = true;
			object value = ParseValue(json, ref index, ref success);
			if (!success)
			{
				return null;
			}
			hashtable[text] = value;
		}
		return hashtable;
	}

	protected ArrayList ParseArray(char[] json, ref int index)
	{
		ArrayList arrayList = new ArrayList();
		NextToken(json, ref index);
		bool flag = false;
		while (!flag)
		{
			switch (LookAhead(json, index))
			{
			case 0:
				return null;
			case 6:
				NextToken(json, ref index);
				continue;
			case 4:
				break;
			default:
			{
				bool success = true;
				object value = ParseValue(json, ref index, ref success);
				if (!success)
				{
					return null;
				}
				arrayList.Add(value);
				continue;
			}
			}
			NextToken(json, ref index);
			break;
		}
		return arrayList;
	}

	protected object ParseValue(char[] json, ref int index, ref bool success)
	{
		switch (LookAhead(json, index))
		{
		case 7:
			return ParseString(json, ref index);
		case 8:
			return ParseNumber(json, ref index);
		case 1:
			return ParseObject(json, ref index);
		case 3:
			return ParseArray(json, ref index);
		case 9:
			NextToken(json, ref index);
			return bool.Parse("TRUE");
		case 10:
			NextToken(json, ref index);
			return bool.Parse("FALSE");
		case 11:
			NextToken(json, ref index);
			return null;
		default:
			success = false;
			return null;
		}
	}

	protected string ParseString(char[] json, ref int index)
	{
		string text = string.Empty;
		EatWhitespace(json, ref index);
		char c = json[index];
		index++;
		bool flag = false;
		while (!flag && index != json.Length)
		{
			c = json[index];
			index++;
			switch (c)
			{
			case '"':
				flag = true;
				break;
			case '\\':
			{
				if (index == json.Length)
				{
					break;
				}
				c = json[index];
				index++;
				switch (c)
				{
				case '"':
					text += '"';
					continue;
				case '\\':
					text += '\\';
					continue;
				case '/':
					text += '/';
					continue;
				case 'b':
					text += '\b';
					continue;
				case 'f':
					text += '\f';
					continue;
				case 'n':
					text += '\n';
					continue;
				case 'r':
					text += '\r';
					continue;
				case 't':
					text += '\t';
					continue;
				case 'u':
					break;
				default:
					continue;
				}
				int num = json.Length - index;
				if (num < 4)
				{
					break;
				}
				char[] array = new char[4];
				for (int i = 0; i < 4; i++)
				{
					array[i] = json[index + i];
				}
				text = text + "&#x" + new string(array) + ";";
				index += 4;
				continue;
			}
			default:
				text += c;
				continue;
			}
			break;
		}
		if (!flag)
		{
			return null;
		}
		return text;
	}

	protected float ParseNumber(char[] json, ref int index)
	{
		EatWhitespace(json, ref index);
		int lastIndexOfNumber = GetLastIndexOfNumber(json, index);
		int num = lastIndexOfNumber - index + 1;
		char[] array = new char[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = json[index + i];
		}
		index = lastIndexOfNumber + 1;
		return float.Parse(new string(array));
	}

	protected int GetLastIndexOfNumber(char[] json, int index)
	{
		int i;
		for (i = index; i < json.Length && "0123456789+-.eE".IndexOf(json[i]) != -1; i++)
		{
		}
		return i - 1;
	}

	protected void EatWhitespace(char[] json, ref int index)
	{
		while (index < json.Length && " \t\n\r".IndexOf(json[index]) != -1)
		{
			index++;
		}
	}

	protected int LookAhead(char[] json, int index)
	{
		int index2 = index;
		return NextToken(json, ref index2);
	}

	protected int NextToken(char[] json, ref int index)
	{
		EatWhitespace(json, ref index);
		if (index == json.Length)
		{
			return 0;
		}
		char c = json[index];
		index++;
		switch (c)
		{
		case '{':
			return 1;
		case '}':
			return 2;
		case '[':
			return 3;
		case ']':
			return 4;
		case ',':
			return 6;
		case '"':
			return 7;
		case '-':
		case '0':
		case '1':
		case '2':
		case '3':
		case '4':
		case '5':
		case '6':
		case '7':
		case '8':
		case '9':
			return 8;
		case ':':
			return 5;
		default:
		{
			index--;
			int num = json.Length - index;
			if (num >= 5 && json[index] == 'f' && json[index + 1] == 'a' && json[index + 2] == 'l' && json[index + 3] == 's' && json[index + 4] == 'e')
			{
				index += 5;
				return 10;
			}
			if (num >= 4 && json[index] == 't' && json[index + 1] == 'r' && json[index + 2] == 'u' && json[index + 3] == 'e')
			{
				index += 4;
				return 9;
			}
			if (num >= 4 && json[index] == 'n' && json[index + 1] == 'u' && json[index + 2] == 'l' && json[index + 3] == 'l')
			{
				index += 4;
				return 11;
			}
			return 0;
		}
		}
	}

	protected bool SerializeObjectOrArray(object objectOrArray, StringBuilder builder)
	{
		if (objectOrArray is Hashtable)
		{
			return SerializeObject((Hashtable)objectOrArray, builder);
		}
		if (objectOrArray is ArrayList)
		{
			return SerializeArray((ArrayList)objectOrArray, builder);
		}
		return false;
	}

	protected bool SerializeObject(Hashtable anObject, StringBuilder builder)
	{
		builder.Append("{");
		IDictionaryEnumerator enumerator = anObject.GetEnumerator();
		bool flag = true;
		while (enumerator.MoveNext())
		{
			string aString = enumerator.Key.ToString();
			object value = enumerator.Value;
			if (!flag)
			{
				builder.Append(", ");
			}
			SerializeString(aString, builder);
			builder.Append(":");
			if (!SerializeValue(value, builder))
			{
				return false;
			}
			flag = false;
		}
		builder.Append("}");
		return true;
	}

	protected bool SerializeArray(ArrayList anArray, StringBuilder builder)
	{
		builder.Append("[");
		bool flag = true;
		for (int i = 0; i < anArray.Count; i++)
		{
			object value = anArray[i];
			if (!flag)
			{
				builder.Append(", ");
			}
			if (!SerializeValue(value, builder))
			{
				return false;
			}
			flag = false;
		}
		builder.Append("]");
		return true;
	}

	protected bool SerializeValue(object value, StringBuilder builder)
	{
		if (value == null)
		{
			builder.Append("null");
		}
		else if (value.GetType().IsArray)
		{
			SerializeArray(new ArrayList((ICollection)value), builder);
		}
		else if (value is string)
		{
			SerializeString((string)value, builder);
		}
		else if (value is char)
		{
			SerializeString(value.ToString(), builder);
		}
		else if (value is Hashtable)
		{
			SerializeObject((Hashtable)value, builder);
		}
		else if (value is ArrayList)
		{
			SerializeArray((ArrayList)value, builder);
		}
		else if (value is bool && (bool)value)
		{
			builder.Append("true");
		}
		else if (value is bool && !(bool)value)
		{
			builder.Append("false");
		}
		else
		{
			if (!(value is float))
			{
				return false;
			}
			SerializeNumber((float)value, builder);
		}
		return true;
	}

	protected void SerializeString(string aString, StringBuilder builder)
	{
		builder.Append("\"");
		char[] array = aString.ToCharArray();
		foreach (char c in array)
		{
			switch (c)
			{
			case '"':
				builder.Append("\\\"");
				continue;
			case '\\':
				builder.Append("\\\\");
				continue;
			case '\b':
				builder.Append("\\b");
				continue;
			case '\f':
				builder.Append("\\f");
				continue;
			case '\n':
				builder.Append("\\n");
				continue;
			case '\r':
				builder.Append("\\r");
				continue;
			case '\t':
				builder.Append("\\t");
				continue;
			}
			int num = c;
			if (num >= 32 && num <= 126)
			{
				builder.Append(c);
			}
		}
		builder.Append("\"");
	}

	protected void SerializeNumber(float number, StringBuilder builder)
	{
		builder.Append(number.ToString());
	}
}
