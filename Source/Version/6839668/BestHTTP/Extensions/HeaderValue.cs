using System;
using System.Collections.Generic;

namespace BestHTTP.Extensions;

public sealed class HeaderValue
{
	public string Key { get; set; }

	public string Value { get; set; }

	public List<HeaderValue> Options { get; set; }

	public bool HasValue => !string.IsNullOrEmpty(Value);

	public HeaderValue()
	{
	}

	public HeaderValue(string key)
	{
		Key = key;
	}

	public void Parse(string headerStr, ref int pos)
	{
		ParseImplementation(headerStr, ref pos, isOptionIsAnOption: true);
	}

	public bool TryGetOption(string key, out HeaderValue option)
	{
		option = null;
		if (Options == null || Options.Count == 0)
		{
			return false;
		}
		for (int i = 0; i < Options.Count; i++)
		{
			if (string.Equals(Options[i].Key, key, StringComparison.OrdinalIgnoreCase))
			{
				option = Options[i];
				return true;
			}
		}
		return false;
	}

	private void ParseImplementation(string headerStr, ref int pos, bool isOptionIsAnOption)
	{
		string text2 = (Key = headerStr.Read(ref pos, (char ch) => ch != ';' && ch != '=' && ch != ','));
		char? c = headerStr.Peek(pos - 1);
		bool flag = ((!c.HasValue) ? null : new int?(c.Value)) == 61;
		bool flag2 = isOptionIsAnOption && ((!c.HasValue) ? null : new int?(c.Value)) == 59;
		while ((c.HasValue && flag) || flag2)
		{
			if (flag)
			{
				string text4 = (Value = headerStr.ReadPossibleQuotedText(ref pos));
			}
			else if (flag2)
			{
				HeaderValue headerValue = new HeaderValue();
				headerValue.ParseImplementation(headerStr, ref pos, isOptionIsAnOption: false);
				if (Options == null)
				{
					Options = new List<HeaderValue>();
				}
				Options.Add(headerValue);
			}
			if (!isOptionIsAnOption)
			{
				break;
			}
			c = headerStr.Peek(pos - 1);
			flag = ((!c.HasValue) ? null : new int?(c.Value)) == 61;
			flag2 = isOptionIsAnOption && ((!c.HasValue) ? null : new int?(c.Value)) == 59;
		}
	}

	public override string ToString()
	{
		if (!string.IsNullOrEmpty(Value))
		{
			return Key + '=' + Value;
		}
		return Key;
	}
}
