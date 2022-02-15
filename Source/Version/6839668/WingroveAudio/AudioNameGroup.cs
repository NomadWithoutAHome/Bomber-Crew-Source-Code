using System.Text;
using UnityEngine;

namespace WingroveAudio;

public class AudioNameGroup : ScriptableObject
{
	[SerializeField]
	private string[] m_events;

	[SerializeField]
	private string[] m_parameters;

	public string[] GetEvents()
	{
		if (m_events == null)
		{
			m_events = new string[0];
		}
		return m_events;
	}

	public string[] GetParameters()
	{
		if (m_parameters == null)
		{
			m_parameters = new string[0];
		}
		return m_parameters;
	}

	public void AddEvent(string eventName)
	{
	}

	public void AddParameter(string eventName)
	{
	}

	private string SanitiseString(string inputString)
	{
		string[] array = inputString.Split(" ,_.;".ToCharArray());
		string text = string.Empty;
		int num = 0;
		string[] array2 = array;
		foreach (string text2 in array2)
		{
			int num2 = 0;
			string text3 = text2;
			foreach (char c in text3)
			{
				if (num2 == 0)
				{
					text = ((!char.IsLetterOrDigit(c)) ? (text + "K") : ((num != 0) ? (text + char.ToUpperInvariant(c)) : ((!char.IsLetter(c)) ? (text + "K" + c) : (text + char.ToUpperInvariant(c)))));
				}
				else if (char.IsLetterOrDigit(c))
				{
					text += char.ToLowerInvariant(c);
				}
				num2++;
			}
			num++;
		}
		return text;
	}

	public string GenerateStaticCSharp()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("namespace AudioNames");
		stringBuilder.AppendLine("{");
		stringBuilder.AppendLine("    public static class " + base.name);
		stringBuilder.AppendLine("    {");
		stringBuilder.AppendLine("        public static class Events");
		stringBuilder.AppendLine("        {");
		string[] events = m_events;
		foreach (string text in events)
		{
			stringBuilder.AppendLine("            public const string " + SanitiseString(text) + " = \"" + text + "\";");
		}
		stringBuilder.AppendLine("        }");
		stringBuilder.AppendLine("        public static class Parameters");
		stringBuilder.AppendLine("        {");
		string[] parameters = m_parameters;
		foreach (string text2 in parameters)
		{
			string text3 = SanitiseString(text2);
			stringBuilder.AppendLine("            public const string " + text3 + " = \"" + text2 + "\";");
			stringBuilder.AppendLine("            private static int CacheVal_" + text3 + "_Internal = 0;");
			stringBuilder.AppendLine("            public static int CacheVal_" + text3 + "()");
			stringBuilder.AppendLine("            {");
			stringBuilder.AppendLine("                if (CacheVal_" + text3 + "_Internal == 0)");
			stringBuilder.AppendLine("                {");
			stringBuilder.AppendLine("                    CacheVal_" + text3 + "_Internal = WingroveAudio.WingroveRoot.Instance.GetParameterId(" + text3 + ");");
			stringBuilder.AppendLine("                }");
			stringBuilder.AppendLine("                return CacheVal_" + text3 + "_Internal;");
			stringBuilder.AppendLine("            }");
		}
		stringBuilder.AppendLine("        }");
		stringBuilder.AppendLine("    }");
		stringBuilder.AppendLine("}");
		return stringBuilder.ToString();
	}
}
