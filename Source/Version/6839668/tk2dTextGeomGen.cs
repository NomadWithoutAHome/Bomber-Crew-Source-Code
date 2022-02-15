using UnityEngine;

public static class tk2dTextGeomGen
{
	public class GeomData
	{
		internal tk2dTextMeshData textMeshData;

		internal tk2dFontData fontInst;

		internal string formattedText = string.Empty;

		internal bool allCaps;
	}

	private static GeomData tmpData = new GeomData();

	private static readonly Color32[] channelSelectColors = new Color32[4]
	{
		new Color32(0, 0, byte.MaxValue, 0),
		new Color(0f, 255f, 0f, 0f),
		new Color(255f, 0f, 0f, 0f),
		new Color(0f, 0f, 0f, 255f)
	};

	private static Color32 meshTopColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	private static Color32 meshBottomColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	private static float meshGradientTexU = 0f;

	private static int curGradientCount = 1;

	private static Color32 errorColor = new Color32(byte.MaxValue, 0, byte.MaxValue, byte.MaxValue);

	public static GeomData Data(tk2dTextMeshData textMeshData, tk2dFontData fontData, string formattedText)
	{
		tmpData.textMeshData = textMeshData;
		tmpData.fontInst = fontData;
		tmpData.formattedText = formattedText;
		return tmpData;
	}

	public static Vector2 GetMeshDimensionsForString(string str, GeomData geomData)
	{
		tk2dTextMeshData textMeshData = geomData.textMeshData;
		tk2dFontData fontInst = geomData.fontInst;
		Vector3 vector = new Vector3(textMeshData.scale.x * textMeshData.scaleModificationFit.x * textMeshData.scaleModificationRep.x, textMeshData.scale.y * textMeshData.scaleModificationFit.y * textMeshData.scaleModificationRep.y, textMeshData.scale.z * textMeshData.scaleModificationFit.z * textMeshData.scaleModificationRep.z);
		float b = 0f;
		float num = 0f;
		float num2 = 0f;
		bool flag = false;
		int num3 = 0;
		for (int i = 0; i < str.Length && num3 < textMeshData.maxChars; i++)
		{
			if (flag)
			{
				flag = false;
				continue;
			}
			int num4 = str[i];
			if (num4 == 10)
			{
				b = Mathf.Max(num, b);
				num = 0f;
				num2 -= (fontInst.lineHeight + textMeshData.lineSpacing) * vector.y;
				continue;
			}
			if (textMeshData.inlineStyling && num4 == 94 && i + 1 < str.Length)
			{
				if (str[i + 1] != '^')
				{
					int num5 = 0;
					switch (str[i + 1])
					{
					case 'c':
						num5 = 5;
						break;
					case 'C':
						num5 = 9;
						break;
					case 'g':
						num5 = 9;
						break;
					case 'G':
						num5 = 17;
						break;
					}
					i += num5;
					continue;
				}
				flag = true;
			}
			bool flag2 = num4 == 94;
			tk2dFontChar tk2dFontChar2;
			if (fontInst.useDictionary)
			{
				if (!fontInst.charDict.ContainsKey(num4))
				{
					num4 = 0;
				}
				tk2dFontChar2 = fontInst.charDict[num4];
			}
			else
			{
				if (num4 >= fontInst.chars.Length)
				{
					num4 = 0;
				}
				tk2dFontChar2 = fontInst.chars[num4];
			}
			if (flag2)
			{
				num4 = 94;
			}
			num += (tk2dFontChar2.advance + textMeshData.spacing) * vector.x;
			if (textMeshData.kerning && i < str.Length - 1)
			{
				tk2dFontKerning[] kerning = fontInst.kerning;
				foreach (tk2dFontKerning tk2dFontKerning2 in kerning)
				{
					if (tk2dFontKerning2.c0 == str[i] && tk2dFontKerning2.c1 == str[i + 1])
					{
						num += tk2dFontKerning2.amount * vector.x;
						break;
					}
				}
			}
			num3++;
		}
		b = Mathf.Max(num, b);
		num2 -= (fontInst.lineHeight + textMeshData.lineSpacing) * vector.y;
		return new Vector2(b, num2);
	}

	public static float GetYAnchorForHeight(float textHeight, GeomData geomData)
	{
		tk2dTextMeshData textMeshData = geomData.textMeshData;
		tk2dFontData fontInst = geomData.fontInst;
		Vector3 vector = new Vector3(textMeshData.scale.x * textMeshData.scaleModificationFit.x * textMeshData.scaleModificationRep.x, textMeshData.scale.y * textMeshData.scaleModificationFit.y * textMeshData.scaleModificationRep.y, textMeshData.scale.z * textMeshData.scaleModificationFit.z * textMeshData.scaleModificationRep.z);
		int num = (int)textMeshData.anchor / 3;
		float num2 = (fontInst.lineHeight + textMeshData.lineSpacing) * vector.y;
		switch (num)
		{
		case 0:
			return 0f - num2;
		case 1:
		{
			float num3 = (0f - textHeight) / 2f - num2;
			if (fontInst.version >= 2)
			{
				float num4 = fontInst.texelSize.y * vector.y;
				return Mathf.Floor(num3 / num4) * num4;
			}
			return num3;
		}
		case 2:
			return 0f - textHeight - num2;
		default:
			return 0f - num2;
		}
	}

	public static float GetXAnchorForWidth(float lineWidth, GeomData geomData)
	{
		tk2dTextMeshData textMeshData = geomData.textMeshData;
		tk2dFontData fontInst = geomData.fontInst;
		Vector3 vector = new Vector3(textMeshData.scale.x * textMeshData.scaleModificationFit.x * textMeshData.scaleModificationRep.x, textMeshData.scale.y * textMeshData.scaleModificationFit.y * textMeshData.scaleModificationRep.y, textMeshData.scale.z * textMeshData.scaleModificationFit.z * textMeshData.scaleModificationRep.z);
		switch ((int)textMeshData.anchor % 3)
		{
		case 0:
			return 0f;
		case 1:
		{
			float num = (0f - lineWidth) / 2f;
			if (fontInst.version >= 2)
			{
				float num2 = fontInst.texelSize.x * vector.x;
				return Mathf.Floor(num / num2) * num2;
			}
			return num;
		}
		case 2:
			return 0f - lineWidth;
		default:
			return 0f;
		}
	}

	private static void PostAlignTextData(Vector3[] pos, int offset, int targetStart, int targetEnd, float offsetX)
	{
		for (int i = targetStart * 4; i < targetEnd * 4; i++)
		{
			Vector3 vector = pos[offset + i];
			vector.x += offsetX;
			pos[offset + i] = vector;
		}
	}

	private static int GetFullHexColorComponent(int c1, int c2)
	{
		int num = 0;
		if (c1 >= 48 && c1 <= 57)
		{
			num += (c1 - 48) * 16;
		}
		else if (c1 >= 97 && c1 <= 102)
		{
			num += (10 + c1 - 97) * 16;
		}
		else
		{
			if (c1 < 65 || c1 > 70)
			{
				return -1;
			}
			num += (10 + c1 - 65) * 16;
		}
		if (c2 >= 48 && c2 <= 57)
		{
			return num + (c2 - 48);
		}
		if (c2 >= 97 && c2 <= 102)
		{
			return num + (10 + c2 - 97);
		}
		if (c2 >= 65 && c2 <= 70)
		{
			return num + (10 + c2 - 65);
		}
		return -1;
	}

	private static int GetCompactHexColorComponent(int c)
	{
		if (c >= 48 && c <= 57)
		{
			return (c - 48) * 17;
		}
		if (c >= 97 && c <= 102)
		{
			return (10 + c - 97) * 17;
		}
		if (c >= 65 && c <= 70)
		{
			return (10 + c - 65) * 17;
		}
		return -1;
	}

	private static int GetStyleHexColor(string str, bool fullHex, ref Color32 color)
	{
		int num;
		int num2;
		int num3;
		int num4;
		if (fullHex)
		{
			if (str.Length < 8)
			{
				return 1;
			}
			num = GetFullHexColorComponent(str[0], str[1]);
			num2 = GetFullHexColorComponent(str[2], str[3]);
			num3 = GetFullHexColorComponent(str[4], str[5]);
			num4 = GetFullHexColorComponent(str[6], str[7]);
		}
		else
		{
			if (str.Length < 4)
			{
				return 1;
			}
			num = GetCompactHexColorComponent(str[0]);
			num2 = GetCompactHexColorComponent(str[1]);
			num3 = GetCompactHexColorComponent(str[2]);
			num4 = GetCompactHexColorComponent(str[3]);
		}
		if (num == -1 || num2 == -1 || num3 == -1 || num4 == -1)
		{
			return 1;
		}
		color = new Color32((byte)num, (byte)num2, (byte)num3, (byte)num4);
		return 0;
	}

	private static int SetColorsFromStyleCommand(string args, bool twoColors, bool fullHex)
	{
		int num = ((!twoColors) ? 1 : 2) * ((!fullHex) ? 4 : 8);
		bool flag = false;
		if (args.Length >= num)
		{
			if (GetStyleHexColor(args, fullHex, ref meshTopColor) != 0)
			{
				flag = true;
			}
			if (twoColors)
			{
				string str = args.Substring((!fullHex) ? 4 : 8);
				if (GetStyleHexColor(str, fullHex, ref meshBottomColor) != 0)
				{
					flag = true;
				}
			}
			else
			{
				meshBottomColor = meshTopColor;
			}
		}
		else
		{
			flag = true;
		}
		if (flag)
		{
			meshTopColor = (meshBottomColor = errorColor);
		}
		return num;
	}

	private static void SetGradientTexUFromStyleCommand(int arg)
	{
		meshGradientTexU = (float)(arg - 48) / (float)((curGradientCount <= 0) ? 1 : curGradientCount);
	}

	private static int HandleStyleCommand(string cmd)
	{
		if (cmd.Length == 0)
		{
			return 0;
		}
		int num = cmd[0];
		string args = cmd.Substring(1);
		int result = 0;
		switch (num)
		{
		case 99:
			result = 1 + SetColorsFromStyleCommand(args, twoColors: false, fullHex: false);
			break;
		case 67:
			result = 1 + SetColorsFromStyleCommand(args, twoColors: false, fullHex: true);
			break;
		case 103:
			result = 1 + SetColorsFromStyleCommand(args, twoColors: true, fullHex: false);
			break;
		case 71:
			result = 1 + SetColorsFromStyleCommand(args, twoColors: true, fullHex: true);
			break;
		}
		if (num >= 48 && num <= 57)
		{
			SetGradientTexUFromStyleCommand(num);
			result = 1;
		}
		return result;
	}

	public static void GetTextMeshGeomDesc(out int numVertices, out int numIndices, GeomData geomData)
	{
		tk2dTextMeshData textMeshData = geomData.textMeshData;
		numVertices = textMeshData.maxChars * 4;
		numIndices = textMeshData.maxChars * 6;
	}

	public static int SetTextMeshGeom(Vector3[] pos, Vector2[] uv, Vector2[] uv2, Color32[] color, int offset, GeomData geomData)
	{
		tk2dTextMeshData textMeshData = geomData.textMeshData;
		tk2dFontData fontInst = geomData.fontInst;
		string formattedText = geomData.formattedText;
		Vector3 vector = new Vector3(textMeshData.scale.x * textMeshData.scaleModificationFit.x * textMeshData.scaleModificationRep.x, textMeshData.scale.y * textMeshData.scaleModificationFit.y * textMeshData.scaleModificationRep.y, textMeshData.scale.z * textMeshData.scaleModificationFit.z * textMeshData.scaleModificationRep.z);
		meshTopColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
		meshBottomColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
		meshGradientTexU = (float)textMeshData.textureGradient / (float)((fontInst.gradientCount <= 0) ? 1 : fontInst.gradientCount);
		curGradientCount = fontInst.gradientCount;
		float yAnchorForHeight = GetYAnchorForHeight(GetMeshDimensionsForString(geomData.formattedText, geomData).y, geomData);
		float num = 0f;
		float num2 = 0f;
		int num3 = 0;
		int num4 = 0;
		for (int i = 0; i < formattedText.Length && num3 < textMeshData.maxChars; i++)
		{
			int num5 = formattedText[i];
			bool flag = num5 == 94;
			if (geomData.textMeshData.allCaps && num5 >= 97 && num5 <= 122)
			{
				num5 += -32;
			}
			tk2dFontChar tk2dFontChar2;
			if (fontInst.useDictionary)
			{
				if (!fontInst.charDict.ContainsKey(num5))
				{
					num5 = 0;
				}
				tk2dFontChar2 = fontInst.charDict[num5];
			}
			else
			{
				if (num5 >= fontInst.chars.Length)
				{
					num5 = 0;
				}
				tk2dFontChar2 = fontInst.chars[num5];
			}
			if (flag)
			{
				num5 = 94;
			}
			if (num5 == 10)
			{
				float lineWidth = num;
				int targetEnd = num3;
				if (num4 != num3)
				{
					float xAnchorForWidth = GetXAnchorForWidth(lineWidth, geomData);
					PostAlignTextData(pos, offset, num4, targetEnd, xAnchorForWidth);
				}
				num4 = num3;
				num = 0f;
				num2 -= (fontInst.lineHeight + textMeshData.lineSpacing) * vector.y;
				continue;
			}
			if (textMeshData.inlineStyling && num5 == 94)
			{
				if (i + 1 >= formattedText.Length || formattedText[i + 1] != '^')
				{
					i += HandleStyleCommand(formattedText.Substring(i + 1));
					continue;
				}
				i++;
			}
			ref Vector3 reference = ref pos[offset + num3 * 4];
			reference = new Vector3(num + tk2dFontChar2.p0.x * vector.x, yAnchorForHeight + num2 + tk2dFontChar2.p0.y * vector.y, 0f);
			ref Vector3 reference2 = ref pos[offset + num3 * 4 + 1];
			reference2 = new Vector3(num + tk2dFontChar2.p1.x * vector.x, yAnchorForHeight + num2 + tk2dFontChar2.p0.y * vector.y, 0f);
			ref Vector3 reference3 = ref pos[offset + num3 * 4 + 2];
			reference3 = new Vector3(num + tk2dFontChar2.p0.x * vector.x, yAnchorForHeight + num2 + tk2dFontChar2.p1.y * vector.y, 0f);
			ref Vector3 reference4 = ref pos[offset + num3 * 4 + 3];
			reference4 = new Vector3(num + tk2dFontChar2.p1.x * vector.x, yAnchorForHeight + num2 + tk2dFontChar2.p1.y * vector.y, 0f);
			if (tk2dFontChar2.flipped)
			{
				ref Vector2 reference5 = ref uv[offset + num3 * 4];
				reference5 = new Vector2(tk2dFontChar2.uv1.x, tk2dFontChar2.uv1.y);
				ref Vector2 reference6 = ref uv[offset + num3 * 4 + 1];
				reference6 = new Vector2(tk2dFontChar2.uv1.x, tk2dFontChar2.uv0.y);
				ref Vector2 reference7 = ref uv[offset + num3 * 4 + 2];
				reference7 = new Vector2(tk2dFontChar2.uv0.x, tk2dFontChar2.uv1.y);
				ref Vector2 reference8 = ref uv[offset + num3 * 4 + 3];
				reference8 = new Vector2(tk2dFontChar2.uv0.x, tk2dFontChar2.uv0.y);
			}
			else
			{
				ref Vector2 reference9 = ref uv[offset + num3 * 4];
				reference9 = new Vector2(tk2dFontChar2.uv0.x, tk2dFontChar2.uv0.y);
				ref Vector2 reference10 = ref uv[offset + num3 * 4 + 1];
				reference10 = new Vector2(tk2dFontChar2.uv1.x, tk2dFontChar2.uv0.y);
				ref Vector2 reference11 = ref uv[offset + num3 * 4 + 2];
				reference11 = new Vector2(tk2dFontChar2.uv0.x, tk2dFontChar2.uv1.y);
				ref Vector2 reference12 = ref uv[offset + num3 * 4 + 3];
				reference12 = new Vector2(tk2dFontChar2.uv1.x, tk2dFontChar2.uv1.y);
			}
			if (fontInst.textureGradients)
			{
				ref Vector2 reference13 = ref uv2[offset + num3 * 4];
				reference13 = tk2dFontChar2.gradientUv[0] + new Vector2(meshGradientTexU, 0f);
				ref Vector2 reference14 = ref uv2[offset + num3 * 4 + 1];
				reference14 = tk2dFontChar2.gradientUv[1] + new Vector2(meshGradientTexU, 0f);
				ref Vector2 reference15 = ref uv2[offset + num3 * 4 + 2];
				reference15 = tk2dFontChar2.gradientUv[2] + new Vector2(meshGradientTexU, 0f);
				ref Vector2 reference16 = ref uv2[offset + num3 * 4 + 3];
				reference16 = tk2dFontChar2.gradientUv[3] + new Vector2(meshGradientTexU, 0f);
			}
			if (fontInst.isPacked)
			{
				Color32 color2 = channelSelectColors[tk2dFontChar2.channel];
				color[offset + num3 * 4] = color2;
				color[offset + num3 * 4 + 1] = color2;
				color[offset + num3 * 4 + 2] = color2;
				color[offset + num3 * 4 + 3] = color2;
			}
			else
			{
				ref Color32 reference17 = ref color[offset + num3 * 4];
				reference17 = meshTopColor;
				ref Color32 reference18 = ref color[offset + num3 * 4 + 1];
				reference18 = meshTopColor;
				ref Color32 reference19 = ref color[offset + num3 * 4 + 2];
				reference19 = meshBottomColor;
				ref Color32 reference20 = ref color[offset + num3 * 4 + 3];
				reference20 = meshBottomColor;
			}
			num += (tk2dFontChar2.advance + textMeshData.spacing) * vector.x;
			if (textMeshData.kerning && i < formattedText.Length - 1)
			{
				tk2dFontKerning[] kerning = fontInst.kerning;
				foreach (tk2dFontKerning tk2dFontKerning2 in kerning)
				{
					if (tk2dFontKerning2.c0 == formattedText[i] && tk2dFontKerning2.c1 == formattedText[i + 1])
					{
						num += tk2dFontKerning2.amount * vector.x;
						break;
					}
				}
			}
			num3++;
		}
		if (num4 != num3)
		{
			float lineWidth2 = num;
			int targetEnd2 = num3;
			float xAnchorForWidth2 = GetXAnchorForWidth(lineWidth2, geomData);
			PostAlignTextData(pos, offset, num4, targetEnd2, xAnchorForWidth2);
		}
		for (int k = num3; k < textMeshData.maxChars; k++)
		{
			ref Vector3 reference21 = ref pos[offset + k * 4];
			ref Vector3 reference22 = ref pos[offset + k * 4 + 1];
			ref Vector3 reference23 = ref pos[offset + k * 4 + 2];
			ref Vector3 reference24 = ref pos[offset + k * 4 + 3];
			reference21 = (reference22 = (reference23 = (reference24 = Vector3.zero)));
			ref Vector2 reference25 = ref uv[offset + k * 4];
			ref Vector2 reference26 = ref uv[offset + k * 4 + 1];
			ref Vector2 reference27 = ref uv[offset + k * 4 + 2];
			ref Vector2 reference28 = ref uv[offset + k * 4 + 3];
			reference25 = (reference26 = (reference27 = (reference28 = Vector2.zero)));
			if (fontInst.textureGradients)
			{
				ref Vector2 reference29 = ref uv2[offset + k * 4];
				ref Vector2 reference30 = ref uv2[offset + k * 4 + 1];
				ref Vector2 reference31 = ref uv2[offset + k * 4 + 2];
				ref Vector2 reference32 = ref uv2[offset + k * 4 + 3];
				reference29 = (reference30 = (reference31 = (reference32 = Vector2.zero)));
			}
			if (!fontInst.isPacked)
			{
				ref Color32 reference33 = ref color[offset + k * 4];
				ref Color32 reference34 = ref color[offset + k * 4 + 1];
				reference33 = (reference34 = meshTopColor);
				ref Color32 reference35 = ref color[offset + k * 4 + 2];
				ref Color32 reference36 = ref color[offset + k * 4 + 3];
				reference35 = (reference36 = meshBottomColor);
			}
			else
			{
				ref Color32 reference37 = ref color[offset + k * 4];
				ref Color32 reference38 = ref color[offset + k * 4 + 1];
				ref Color32 reference39 = ref color[offset + k * 4 + 2];
				ref Color32 reference40 = ref color[offset + k * 4 + 3];
				reference37 = (reference38 = (reference39 = (reference40 = Color.clear)));
			}
		}
		return num3;
	}

	public static void SetTextMeshIndices(int[] indices, int offset, int vStart, GeomData geomData, int target)
	{
		tk2dTextMeshData textMeshData = geomData.textMeshData;
		for (int i = 0; i < textMeshData.maxChars; i++)
		{
			indices[offset + i * 6] = vStart + i * 4;
			indices[offset + i * 6 + 1] = vStart + i * 4 + 1;
			indices[offset + i * 6 + 2] = vStart + i * 4 + 3;
			indices[offset + i * 6 + 3] = vStart + i * 4 + 2;
			indices[offset + i * 6 + 4] = vStart + i * 4;
			indices[offset + i * 6 + 5] = vStart + i * 4 + 3;
		}
	}
}
