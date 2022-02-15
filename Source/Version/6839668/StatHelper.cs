using BomberCrewCommon;
using UnityEngine;

public class StatHelper : Singleton<StatHelper>
{
	public class StatInfo
	{
		public float m_value;

		public bool m_isPercent;

		public bool m_biggerIsBetter;

		public bool m_isTime;

		public string m_statPostfixTranslated;

		public string m_titleText;

		public static StatInfo Create(float value, string titleText)
		{
			StatInfo statInfo = new StatInfo();
			statInfo.m_titleText = titleText;
			statInfo.m_value = value;
			statInfo.m_isTime = false;
			statInfo.m_isPercent = false;
			statInfo.m_biggerIsBetter = true;
			statInfo.m_statPostfixTranslated = string.Empty;
			return statInfo;
		}

		public static StatInfo CreatePercent(float value01, bool biggerIsBetter, string titleText)
		{
			StatInfo statInfo = new StatInfo();
			statInfo.m_titleText = titleText;
			statInfo.m_value = value01;
			statInfo.m_isTime = false;
			statInfo.m_isPercent = true;
			statInfo.m_biggerIsBetter = biggerIsBetter;
			statInfo.m_statPostfixTranslated = "%";
			return statInfo;
		}

		public static StatInfo CreateTime(float valueSeconds, bool biggerIsBetter, string titleText)
		{
			StatInfo statInfo = new StatInfo();
			statInfo.m_titleText = titleText;
			statInfo.m_value = valueSeconds;
			statInfo.m_isTime = true;
			statInfo.m_isPercent = false;
			statInfo.m_biggerIsBetter = biggerIsBetter;
			statInfo.m_statPostfixTranslated = "%";
			return statInfo;
		}

		public static StatInfo CreateSmallerBetter(float value, string titleText)
		{
			StatInfo statInfo = new StatInfo();
			statInfo.m_titleText = titleText;
			statInfo.m_value = value;
			statInfo.m_isTime = false;
			statInfo.m_isPercent = false;
			statInfo.m_biggerIsBetter = false;
			statInfo.m_statPostfixTranslated = string.Empty;
			return statInfo;
		}

		public static StatInfo CreateUnitType(float value, string unitTypeNamedText, bool biggerIsBetter, string titleText)
		{
			StatInfo statInfo = new StatInfo();
			statInfo.m_titleText = titleText;
			statInfo.m_value = value;
			statInfo.m_isTime = false;
			statInfo.m_isPercent = false;
			statInfo.m_biggerIsBetter = biggerIsBetter;
			statInfo.m_statPostfixTranslated = Singleton<LanguageProvider>.Instance.GetNamedTextImmediate(unitTypeNamedText);
			return statInfo;
		}

		public static StatInfo CreateDiff(StatInfo left, StatInfo right)
		{
			StatInfo statInfo = new StatInfo();
			statInfo.m_titleText = left.m_titleText;
			statInfo.m_value = left.m_value - right.m_value;
			statInfo.m_isTime = left.m_isTime;
			statInfo.m_isPercent = left.m_isPercent;
			statInfo.m_statPostfixTranslated = left.m_statPostfixTranslated;
			statInfo.m_biggerIsBetter = left.m_biggerIsBetter;
			return statInfo;
		}

		public int GetIntValue()
		{
			return Mathf.RoundToInt((!m_isPercent) ? m_value : (m_value * 100f));
		}
	}

	[SerializeField]
	private Color m_goodStatColor;

	[SerializeField]
	private Color m_badStatColor;

	public string GetStatString(StatInfo si, bool includePlus)
	{
		int intValue = si.GetIntValue();
		string arg = ((intValue < 0 || !includePlus) ? string.Empty : "+");
		if (si.m_isTime)
		{
			int num = Mathf.Abs(intValue);
			if (intValue < 0)
			{
				arg = "-";
			}
			int num2 = num % 60;
			int num3 = (num - num2) / 60;
			return $"{arg}{num3}:{num2:00}";
		}
		return $"{arg}{intValue}{si.m_statPostfixTranslated}";
	}

	public string GetStatStringCompare(StatInfo left, StatInfo right, string format)
	{
		string statString = GetStatString(left, includePlus: false);
		StatInfo si = StatInfo.CreateDiff(left, right);
		string statString2 = GetStatString(si, includePlus: true);
		return string.Format(format, left, right);
	}

	public void SetStatStringCompare(StatInfo left, StatInfo right, string leftFormat, string rightFormat, TextSetter leftText, TextSetter rightText)
	{
		string statString = GetStatString(left, includePlus: false);
		leftText.SetText(string.Format(leftFormat, statString));
		if (right != null)
		{
			StatInfo statInfo = StatInfo.CreateDiff(left, right);
			string statString2 = GetStatString(statInfo, includePlus: true);
			rightText.SetText((statInfo.GetIntValue() != 0) ? string.Format(rightFormat, statString2) : string.Empty);
			bool flag = ((!(statInfo.m_value >= 0f)) ? (!left.m_biggerIsBetter) : left.m_biggerIsBetter);
			if (rightText is tk2dTextMesh)
			{
				((tk2dTextMesh)rightText).color = ((!flag) ? m_badStatColor : m_goodStatColor);
			}
		}
		else
		{
			rightText.SetText(string.Empty);
		}
	}

	public void SetStatStringComparePreviewChange(StatInfo left, StatInfo right, string leftFormat, string rightFormat, TextSetter leftText, TextSetter rightText)
	{
		string statString = GetStatString(right, includePlus: false);
		leftText.SetText(string.Format(leftFormat, statString));
		if (right != null)
		{
			StatInfo statInfo = StatInfo.CreateDiff(right, left);
			string statString2 = GetStatString(statInfo, includePlus: true);
			rightText.SetText((statInfo.GetIntValue() != 0) ? string.Format(rightFormat, statString2) : string.Empty);
			bool flag = ((!(statInfo.m_value >= 0f)) ? (!left.m_biggerIsBetter) : left.m_biggerIsBetter);
			if (rightText is tk2dTextMesh)
			{
				((tk2dTextMesh)rightText).color = ((!flag) ? m_badStatColor : m_goodStatColor);
			}
		}
		else
		{
			rightText.SetText(string.Empty);
		}
	}
}
