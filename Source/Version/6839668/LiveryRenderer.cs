using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class LiveryRenderer : Singleton<LiveryRenderer>
{
	public class LiveryDrawCommand
	{
		public Texture2D m_drawTexture;

		public tk2dFontData m_font;

		public string m_textToWrite;

		public Vector3 m_positionCtr;

		public Vector3 m_size;

		public Color m_color;

		public int m_rotate;

		public bool m_dontScaleFonts;

		public bool m_useRoundMask;
	}

	[SerializeField]
	private Material m_defaultDrawingMaterial;

	[SerializeField]
	private Material m_roundMaskDrawingMaterial;

	private int[,] m_rotateSettings = new int[4, 8]
	{
		{ 0, 0, 1, 0, 0, 1, 1, 1 },
		{ 0, 0, 0, 1, 1, 0, 1, 1 },
		{ 0, 1, 0, 0, 1, 1, 1, 0 },
		{ 1, 0, 1, 1, 0, 0, 0, 1 }
	};

	private void Awake()
	{
	}

	private void RenderQuad(Vector3 ctr, Vector3 size, Texture2D tex, Color color, int rotate, bool useRoundMask)
	{
		Material original = ((!useRoundMask) ? m_defaultDrawingMaterial : m_roundMaskDrawingMaterial);
		Material material = Object.Instantiate(original);
		material.mainTexture = tex;
		material.SetPass(0);
		Vector3 vector = ctr - size * 0.5f;
		Vector3 vector2 = ctr + size * 0.5f;
		GL.Begin(5);
		GL.Color(color);
		GL.TexCoord2(m_rotateSettings[rotate, 0], m_rotateSettings[rotate, 1]);
		GL.Vertex3(vector.x, vector.y, 0f);
		GL.Color(color);
		GL.TexCoord2(m_rotateSettings[rotate, 2], m_rotateSettings[rotate, 3]);
		GL.Vertex3(vector2.x, vector.y, 0f);
		GL.Color(color);
		GL.TexCoord2(m_rotateSettings[rotate, 4], m_rotateSettings[rotate, 5]);
		GL.Vertex3(vector.x, vector2.y, 0f);
		GL.Color(color);
		GL.TexCoord2(m_rotateSettings[rotate, 6], m_rotateSettings[rotate, 7]);
		GL.Vertex3(vector2.x, vector2.y, 0f);
		GL.End();
	}

	private tk2dFontChar GetFromFont(tk2dFontData font, char c)
	{
		if (font.useDictionary)
		{
			tk2dFontChar value = null;
			font.InitDictionary();
			font.charDict.TryGetValue(c, out value);
			return value;
		}
		if (c < font.chars.Length)
		{
			return font.chars[(uint)c];
		}
		return null;
	}

	private float GetStringScale(tk2dFontData font, float maxHeight, float maxWidth, string stringIn)
	{
		float num = 1f;
		float width = GetWidth(font, stringIn);
		return Mathf.Min(maxWidth / width, maxHeight / num);
	}

	private float GetWidth(tk2dFontData font, string stringIn)
	{
		float num = 0f;
		int num2 = 0;
		foreach (char c in stringIn)
		{
			tk2dFontChar fromFont = GetFromFont(font, c);
			if (fromFont != null)
			{
				num += fromFont.advance / font.lineHeight;
				num2++;
			}
		}
		return num;
	}

	private void RenderString(tk2dFontData font, Vector3 startPos, string str, float scaleFontX, float scaleFontY, Color fg)
	{
		Vector3 vector = startPos;
		startPos.y -= 0.5f * scaleFontY;
		foreach (char c in str)
		{
			tk2dFontChar fromFont = GetFromFont(font, c);
			if (fromFont != null)
			{
				float num = 1f / font.lineHeight;
				font.material.SetPass(0);
				Vector3 p = fromFont.p0;
				p.x *= scaleFontX * num;
				p.y *= scaleFontY * num;
				Vector3 p2 = fromFont.p1;
				p2.x *= scaleFontX * num;
				p2.y *= scaleFontY * num;
				Vector3 vector2 = p + startPos;
				Vector3 vector3 = p2 + startPos;
				GL.Begin(5);
				if (fromFont.flipped)
				{
					GL.Color(fg);
					GL.TexCoord2(fromFont.uv1.x, fromFont.uv1.y);
					GL.Vertex3(vector2.x, vector2.y, 0f);
					GL.Color(fg);
					GL.TexCoord2(fromFont.uv1.x, fromFont.uv0.y);
					GL.Vertex3(vector3.x, vector2.y, 0f);
					GL.Color(fg);
					GL.TexCoord2(fromFont.uv0.x, fromFont.uv1.y);
					GL.Vertex3(vector2.x, vector3.y, 0f);
					GL.Color(fg);
					GL.TexCoord2(fromFont.uv0.x, fromFont.uv0.y);
					GL.Vertex3(vector3.x, vector3.y, 0f);
				}
				else
				{
					GL.Color(fg);
					GL.TexCoord2(fromFont.uv0.x, fromFont.uv0.y);
					GL.Vertex3(vector2.x, vector2.y, 0f);
					GL.Color(fg);
					GL.TexCoord2(fromFont.uv1.x, fromFont.uv0.y);
					GL.Vertex3(vector3.x, vector2.y, 0f);
					GL.Color(fg);
					GL.TexCoord2(fromFont.uv0.x, fromFont.uv1.y);
					GL.Vertex3(vector2.x, vector3.y, 0f);
					GL.Color(fg);
					GL.TexCoord2(fromFont.uv1.x, fromFont.uv1.y);
					GL.Vertex3(vector3.x, vector3.y, 0f);
				}
				GL.End();
				startPos.x += fromFont.advance * num * scaleFontX;
			}
		}
	}

	public void GenerateLiveryTexture(Texture2D baseTextureIn, List<LiveryDrawCommand> toDraw, Texture2D outputTexture)
	{
		int width = baseTextureIn.width;
		int height = baseTextureIn.height;
		RenderTexture renderTexture2 = (RenderTexture.active = new RenderTexture(width, height, 0));
		GL.Clear(clearDepth: true, clearColor: true, Color.black);
		GL.PushMatrix();
		GL.LoadOrtho();
		GL.LoadPixelMatrix(0f, width, 0f, height);
		RenderQuad(new Vector3(width / 2, height / 2), new Vector3(width, height), baseTextureIn, Color.white, 0, useRoundMask: false);
		foreach (LiveryDrawCommand item in toDraw)
		{
			if (item.m_font == null)
			{
				RenderQuad(item.m_positionCtr, item.m_size, item.m_drawTexture, item.m_color, item.m_rotate, item.m_useRoundMask);
			}
			else if (item.m_font != null && item.m_textToWrite != null)
			{
				float num = GetStringScale(item.m_font, Mathf.Abs(item.m_size.y), Mathf.Abs(item.m_size.x), item.m_textToWrite);
				float width2 = GetWidth(item.m_font, item.m_textToWrite);
				if (item.m_dontScaleFonts)
				{
					num = item.m_font.lineHeight;
				}
				RenderString(item.m_font, item.m_positionCtr + new Vector3((0f - width2) * num * 0.5f * Mathf.Sign(item.m_size.x), 0f, 0f), item.m_textToWrite, num * Mathf.Sign(item.m_size.x), num * Mathf.Sign(item.m_size.y), item.m_color);
			}
		}
		GL.PopMatrix();
		outputTexture.ReadPixels(new Rect(0f, 0f, width, height), 0, 0);
		outputTexture.Apply();
		RenderTexture.active = null;
	}
}
