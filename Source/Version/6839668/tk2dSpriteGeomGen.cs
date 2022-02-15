using UnityEngine;

public static class tk2dSpriteGeomGen
{
	private static readonly int[] boxIndicesBack = new int[36]
	{
		0, 1, 2, 2, 1, 3, 6, 5, 4, 7,
		5, 6, 3, 7, 6, 2, 3, 6, 4, 5,
		1, 4, 1, 0, 6, 4, 0, 6, 0, 2,
		1, 7, 3, 5, 7, 1
	};

	private static readonly int[] boxIndicesFwd = new int[36]
	{
		2, 1, 0, 3, 1, 2, 4, 5, 6, 6,
		5, 7, 6, 7, 3, 6, 3, 2, 1, 5,
		4, 0, 1, 4, 0, 4, 6, 2, 0, 6,
		3, 7, 1, 1, 7, 5
	};

	private static readonly Vector3[] boxUnitVertices = new Vector3[8]
	{
		new Vector3(-1f, -1f, -1f),
		new Vector3(-1f, -1f, 1f),
		new Vector3(1f, -1f, -1f),
		new Vector3(1f, -1f, 1f),
		new Vector3(-1f, 1f, -1f),
		new Vector3(-1f, 1f, 1f),
		new Vector3(1f, 1f, -1f),
		new Vector3(1f, 1f, 1f)
	};

	private static Matrix4x4 boxScaleMatrix = Matrix4x4.identity;

	public static void SetSpriteColors(Color32[] dest, int offset, int numVertices, Color c, bool premulAlpha)
	{
		if (premulAlpha)
		{
			c.r *= c.a;
			c.g *= c.a;
			c.b *= c.a;
		}
		Color32 color = c;
		for (int i = 0; i < numVertices; i++)
		{
			dest[offset + i] = color;
		}
	}

	public static Vector2 GetAnchorOffset(tk2dBaseSprite.Anchor anchor, float width, float height)
	{
		Vector2 zero = Vector2.zero;
		switch (anchor)
		{
		case tk2dBaseSprite.Anchor.LowerCenter:
		case tk2dBaseSprite.Anchor.MiddleCenter:
		case tk2dBaseSprite.Anchor.UpperCenter:
			zero.x = (int)(width / 2f);
			break;
		case tk2dBaseSprite.Anchor.LowerRight:
		case tk2dBaseSprite.Anchor.MiddleRight:
		case tk2dBaseSprite.Anchor.UpperRight:
			zero.x = (int)width;
			break;
		}
		switch (anchor)
		{
		case tk2dBaseSprite.Anchor.MiddleLeft:
		case tk2dBaseSprite.Anchor.MiddleCenter:
		case tk2dBaseSprite.Anchor.MiddleRight:
			zero.y = (int)(height / 2f);
			break;
		case tk2dBaseSprite.Anchor.LowerLeft:
		case tk2dBaseSprite.Anchor.LowerCenter:
		case tk2dBaseSprite.Anchor.LowerRight:
			zero.y = (int)height;
			break;
		}
		return zero;
	}

	public static void GetSpriteGeomDesc(out int numVertices, out int numIndices, tk2dSpriteDefinition spriteDef)
	{
		numVertices = spriteDef.positions.Length;
		numIndices = spriteDef.indices.Length;
	}

	public static void SetSpriteGeom(Vector3[] pos, Vector2[] uv, Vector2[] uv2, Vector3[] norm, Vector4[] tang, int offset, tk2dSpriteDefinition spriteDef, Vector3 scale)
	{
		for (int i = 0; i < spriteDef.positions.Length; i++)
		{
			ref Vector3 reference = ref pos[offset + i];
			reference = Vector3.Scale(spriteDef.positions[i], scale);
		}
		for (int j = 0; j < spriteDef.uvs.Length; j++)
		{
			ref Vector2 reference2 = ref uv[offset + j];
			reference2 = spriteDef.uvs[j];
		}
		for (int k = 0; k < spriteDef.uv2s.Length; k++)
		{
			ref Vector2 reference3 = ref uv2[offset + k];
			reference3 = spriteDef.uv2s[k];
		}
		if (norm != null && spriteDef.normals != null)
		{
			for (int l = 0; l < spriteDef.normals.Length; l++)
			{
				ref Vector3 reference4 = ref norm[offset + l];
				reference4 = spriteDef.normals[l];
			}
		}
		if (tang != null && spriteDef.tangents != null)
		{
			for (int m = 0; m < spriteDef.tangents.Length; m++)
			{
				ref Vector4 reference5 = ref tang[offset + m];
				reference5 = spriteDef.tangents[m];
			}
		}
	}

	public static void SetSpriteIndices(int[] indices, int offset, int vStart, tk2dSpriteDefinition spriteDef)
	{
		for (int i = 0; i < spriteDef.indices.Length; i++)
		{
			indices[offset + i] = vStart + spriteDef.indices[i];
		}
	}

	public static void GetClippedSpriteGeomDesc(out int numVertices, out int numIndices, tk2dSpriteDefinition spriteDef)
	{
		if (spriteDef.positions.Length == 4)
		{
			numVertices = 4;
			numIndices = 6;
		}
		else
		{
			numVertices = 0;
			numIndices = 0;
		}
	}

	public static void SetClippedSpriteGeom(Vector3[] pos, Vector2[] uv, int offset, out Vector3 boundsCenter, out Vector3 boundsExtents, tk2dSpriteDefinition spriteDef, Vector3 scale, Vector2 clipBottomLeft, Vector2 clipTopRight, float colliderOffsetZ, float colliderExtentZ)
	{
		boundsCenter = Vector3.zero;
		boundsExtents = Vector3.zero;
		if (spriteDef.positions.Length == 4)
		{
			Vector3 vector = spriteDef.untrimmedBoundsData[0] - spriteDef.untrimmedBoundsData[1] * 0.5f;
			Vector3 vector2 = spriteDef.untrimmedBoundsData[0] + spriteDef.untrimmedBoundsData[1] * 0.5f;
			float num = Mathf.Lerp(vector.x, vector2.x, clipBottomLeft.x);
			float num2 = Mathf.Lerp(vector.x, vector2.x, clipTopRight.x);
			float num3 = Mathf.Lerp(vector.y, vector2.y, clipBottomLeft.y);
			float num4 = Mathf.Lerp(vector.y, vector2.y, clipTopRight.y);
			Vector3 vector3 = spriteDef.boundsData[1];
			Vector3 vector4 = spriteDef.boundsData[0] - vector3 * 0.5f;
			float value = (num - vector4.x) / vector3.x;
			float value2 = (num2 - vector4.x) / vector3.x;
			float value3 = (num3 - vector4.y) / vector3.y;
			float value4 = (num4 - vector4.y) / vector3.y;
			Vector2 vector5 = new Vector2(Mathf.Clamp01(value), Mathf.Clamp01(value3));
			Vector2 vector6 = new Vector2(Mathf.Clamp01(value2), Mathf.Clamp01(value4));
			Vector3 vector7 = spriteDef.positions[0];
			Vector3 vector8 = spriteDef.positions[3];
			Vector3 vector9 = new Vector3(Mathf.Lerp(vector7.x, vector8.x, vector5.x) * scale.x, Mathf.Lerp(vector7.y, vector8.y, vector5.y) * scale.y, vector7.z * scale.z);
			Vector3 vector10 = new Vector3(Mathf.Lerp(vector7.x, vector8.x, vector6.x) * scale.x, Mathf.Lerp(vector7.y, vector8.y, vector6.y) * scale.y, vector7.z * scale.z);
			boundsCenter.Set(vector9.x + (vector10.x - vector9.x) * 0.5f, vector9.y + (vector10.y - vector9.y) * 0.5f, colliderOffsetZ);
			boundsExtents.Set((vector10.x - vector9.x) * 0.5f, (vector10.y - vector9.y) * 0.5f, colliderExtentZ);
			ref Vector3 reference = ref pos[offset];
			reference = new Vector3(vector9.x, vector9.y, vector9.z);
			ref Vector3 reference2 = ref pos[offset + 1];
			reference2 = new Vector3(vector10.x, vector9.y, vector9.z);
			ref Vector3 reference3 = ref pos[offset + 2];
			reference3 = new Vector3(vector9.x, vector10.y, vector9.z);
			ref Vector3 reference4 = ref pos[offset + 3];
			reference4 = new Vector3(vector10.x, vector10.y, vector9.z);
			if (spriteDef.flipped == tk2dSpriteDefinition.FlipMode.Tk2d)
			{
				Vector2 vector11 = new Vector2(Mathf.Lerp(spriteDef.uvs[0].x, spriteDef.uvs[3].x, vector5.y), Mathf.Lerp(spriteDef.uvs[0].y, spriteDef.uvs[3].y, vector5.x));
				Vector2 vector12 = new Vector2(Mathf.Lerp(spriteDef.uvs[0].x, spriteDef.uvs[3].x, vector6.y), Mathf.Lerp(spriteDef.uvs[0].y, spriteDef.uvs[3].y, vector6.x));
				ref Vector2 reference5 = ref uv[offset];
				reference5 = new Vector2(vector11.x, vector11.y);
				ref Vector2 reference6 = ref uv[offset + 1];
				reference6 = new Vector2(vector11.x, vector12.y);
				ref Vector2 reference7 = ref uv[offset + 2];
				reference7 = new Vector2(vector12.x, vector11.y);
				ref Vector2 reference8 = ref uv[offset + 3];
				reference8 = new Vector2(vector12.x, vector12.y);
			}
			else if (spriteDef.flipped == tk2dSpriteDefinition.FlipMode.TPackerCW)
			{
				Vector2 vector13 = new Vector2(Mathf.Lerp(spriteDef.uvs[0].x, spriteDef.uvs[3].x, vector5.y), Mathf.Lerp(spriteDef.uvs[0].y, spriteDef.uvs[3].y, vector5.x));
				Vector2 vector14 = new Vector2(Mathf.Lerp(spriteDef.uvs[0].x, spriteDef.uvs[3].x, vector6.y), Mathf.Lerp(spriteDef.uvs[0].y, spriteDef.uvs[3].y, vector6.x));
				ref Vector2 reference9 = ref uv[offset];
				reference9 = new Vector2(vector13.x, vector13.y);
				ref Vector2 reference10 = ref uv[offset + 2];
				reference10 = new Vector2(vector14.x, vector13.y);
				ref Vector2 reference11 = ref uv[offset + 1];
				reference11 = new Vector2(vector13.x, vector14.y);
				ref Vector2 reference12 = ref uv[offset + 3];
				reference12 = new Vector2(vector14.x, vector14.y);
			}
			else
			{
				Vector2 vector15 = new Vector2(Mathf.Lerp(spriteDef.uvs[0].x, spriteDef.uvs[3].x, vector5.x), Mathf.Lerp(spriteDef.uvs[0].y, spriteDef.uvs[3].y, vector5.y));
				Vector2 vector16 = new Vector2(Mathf.Lerp(spriteDef.uvs[0].x, spriteDef.uvs[3].x, vector6.x), Mathf.Lerp(spriteDef.uvs[0].y, spriteDef.uvs[3].y, vector6.y));
				ref Vector2 reference13 = ref uv[offset];
				reference13 = new Vector2(vector15.x, vector15.y);
				ref Vector2 reference14 = ref uv[offset + 1];
				reference14 = new Vector2(vector16.x, vector15.y);
				ref Vector2 reference15 = ref uv[offset + 2];
				reference15 = new Vector2(vector15.x, vector16.y);
				ref Vector2 reference16 = ref uv[offset + 3];
				reference16 = new Vector2(vector16.x, vector16.y);
			}
		}
	}

	public static void SetClippedSpriteIndices(int[] indices, int offset, int vStart, tk2dSpriteDefinition spriteDef)
	{
		if (spriteDef.positions.Length == 4)
		{
			indices[offset] = vStart;
			indices[offset + 1] = vStart + 3;
			indices[offset + 2] = vStart + 1;
			indices[offset + 3] = vStart + 2;
			indices[offset + 4] = vStart + 3;
			indices[offset + 5] = vStart;
		}
	}

	public static void GetSlicedSpriteGeomDesc(out int numVertices, out int numIndices, tk2dSpriteDefinition spriteDef, bool borderOnly)
	{
		if (spriteDef.positions.Length == 4)
		{
			numVertices = 16;
			numIndices = ((!borderOnly) ? 54 : 48);
		}
		else
		{
			numVertices = 0;
			numIndices = 0;
		}
	}

	public static void SetSlicedSpriteGeom(Vector3[] pos, Vector2[] uv, int offset, out Vector3 boundsCenter, out Vector3 boundsExtents, tk2dSpriteDefinition spriteDef, Vector3 scale, Vector2 dimensions, Vector2 borderBottomLeft, Vector2 borderTopRight, tk2dBaseSprite.Anchor anchor, float colliderOffsetZ, float colliderExtentZ)
	{
		boundsCenter = Vector3.zero;
		boundsExtents = Vector3.zero;
		if (spriteDef.positions.Length != 4)
		{
			return;
		}
		float x = spriteDef.texelSize.x;
		float y = spriteDef.texelSize.y;
		Vector3[] positions = spriteDef.positions;
		float num = positions[1].x - positions[0].x;
		float num2 = positions[2].y - positions[0].y;
		float num3 = borderTopRight.y * num2;
		float y2 = borderBottomLeft.y * num2;
		float num4 = borderTopRight.x * num;
		float x2 = borderBottomLeft.x * num;
		float num5 = dimensions.x * x;
		float num6 = dimensions.y * y;
		float num7 = 0f;
		float num8 = 0f;
		switch (anchor)
		{
		case tk2dBaseSprite.Anchor.LowerCenter:
		case tk2dBaseSprite.Anchor.MiddleCenter:
		case tk2dBaseSprite.Anchor.UpperCenter:
			num7 = -(int)(dimensions.x / 2f);
			break;
		case tk2dBaseSprite.Anchor.LowerRight:
		case tk2dBaseSprite.Anchor.MiddleRight:
		case tk2dBaseSprite.Anchor.UpperRight:
			num7 = -(int)dimensions.x;
			break;
		}
		switch (anchor)
		{
		case tk2dBaseSprite.Anchor.MiddleLeft:
		case tk2dBaseSprite.Anchor.MiddleCenter:
		case tk2dBaseSprite.Anchor.MiddleRight:
			num8 = -(int)(dimensions.y / 2f);
			break;
		case tk2dBaseSprite.Anchor.UpperLeft:
		case tk2dBaseSprite.Anchor.UpperCenter:
		case tk2dBaseSprite.Anchor.UpperRight:
			num8 = -(int)dimensions.y;
			break;
		}
		num7 *= x;
		num8 *= y;
		boundsCenter.Set(scale.x * (num5 * 0.5f + num7), scale.y * (num6 * 0.5f + num8), colliderOffsetZ);
		boundsExtents.Set(scale.x * (num5 * 0.5f), scale.y * (num6 * 0.5f), colliderExtentZ);
		Vector2[] uvs = spriteDef.uvs;
		Vector2 vector = uvs[1] - uvs[0];
		Vector2 vector2 = uvs[2] - uvs[0];
		Vector3 vector3 = new Vector3(num7, num8, 0f);
		Vector3[] array = new Vector3[4]
		{
			vector3,
			vector3 + new Vector3(0f, y2, 0f),
			vector3 + new Vector3(0f, num6 - num3, 0f),
			vector3 + new Vector3(0f, num6, 0f)
		};
		Vector2[] array2 = new Vector2[4]
		{
			uvs[0],
			uvs[0] + vector2 * borderBottomLeft.y,
			uvs[0] + vector2 * (1f - borderTopRight.y),
			uvs[0] + vector2
		};
		for (int i = 0; i < 4; i++)
		{
			ref Vector3 reference = ref pos[offset + i * 4];
			reference = array[i];
			ref Vector3 reference2 = ref pos[offset + i * 4 + 1];
			reference2 = array[i] + new Vector3(x2, 0f, 0f);
			ref Vector3 reference3 = ref pos[offset + i * 4 + 2];
			reference3 = array[i] + new Vector3(num5 - num4, 0f, 0f);
			ref Vector3 reference4 = ref pos[offset + i * 4 + 3];
			reference4 = array[i] + new Vector3(num5, 0f, 0f);
			for (int j = 0; j < 4; j++)
			{
				ref Vector3 reference5 = ref pos[offset + i * 4 + j];
				reference5 = Vector3.Scale(pos[offset + i * 4 + j], scale);
			}
			ref Vector2 reference6 = ref uv[offset + i * 4];
			reference6 = array2[i];
			ref Vector2 reference7 = ref uv[offset + i * 4 + 1];
			reference7 = array2[i] + vector * borderBottomLeft.x;
			ref Vector2 reference8 = ref uv[offset + i * 4 + 2];
			reference8 = array2[i] + vector * (1f - borderTopRight.x);
			ref Vector2 reference9 = ref uv[offset + i * 4 + 3];
			reference9 = array2[i] + vector;
		}
	}

	public static void SetSlicedSpriteIndices(int[] indices, int offset, int vStart, tk2dSpriteDefinition spriteDef, bool borderOnly)
	{
		if (spriteDef.positions.Length == 4)
		{
			int[] array = new int[54]
			{
				0, 4, 1, 1, 4, 5, 1, 5, 2, 2,
				5, 6, 2, 6, 3, 3, 6, 7, 4, 8,
				5, 5, 8, 9, 6, 10, 7, 7, 10, 11,
				8, 12, 9, 9, 12, 13, 9, 13, 10, 10,
				13, 14, 10, 14, 11, 11, 14, 15, 5, 9,
				6, 6, 9, 10
			};
			int num = array.Length;
			if (borderOnly)
			{
				num -= 6;
			}
			for (int i = 0; i < num; i++)
			{
				indices[offset + i] = vStart + array[i];
			}
		}
	}

	public static void SetRadialSpriteindices(int[] indices, int offset, int vStart, tk2dSpriteDefinition spriteDef)
	{
		if (spriteDef.positions.Length == 4)
		{
			int[] array = new int[54]
			{
				0, 4, 1, 1, 4, 5, 1, 1, 1, 2,
				2, 2, 2, 6, 3, 3, 6, 7, 4, 4,
				4, 5, 5, 5, 6, 6, 6, 7, 7, 7,
				8, 12, 9, 9, 12, 13, 9, 9, 9, 10,
				10, 10, 10, 14, 11, 11, 14, 15, 5, 5,
				5, 5, 5, 5
			};
			int num = array.Length;
			for (int i = 0; i < num; i++)
			{
				indices[offset + i] = vStart + array[i];
			}
		}
	}

	public static void SetRadialSpriteGeom(Vector3[] pos, Vector2[] uv, Vector3[] norm, Vector4[] tang, int offset, tk2dSpriteDefinition spriteDef, Vector3 scale)
	{
		Vector3 vector = (spriteDef.positions[3] - spriteDef.positions[0]) * 0.5f + spriteDef.positions[0];
		Vector3 vector2 = Vector3.Scale(spriteDef.positions[1] - spriteDef.positions[0], scale) * 0.5f;
		Vector3 vector3 = Vector3.Scale(spriteDef.positions[2] - spriteDef.positions[0], scale) * 0.5f;
		ref Vector3 reference = ref pos[5];
		ref Vector3 reference2 = ref pos[6];
		ref Vector3 reference3 = ref pos[9];
		reference = (reference2 = (reference3 = (pos[10] = vector)));
		ref Vector3 reference4 = ref pos[0];
		reference4 = Vector3.Scale(spriteDef.positions[0], scale);
		ref Vector3 reference5 = ref pos[3];
		reference5 = Vector3.Scale(spriteDef.positions[1], scale);
		ref Vector3 reference6 = ref pos[12];
		reference6 = Vector3.Scale(spriteDef.positions[2], scale);
		ref Vector3 reference7 = ref pos[15];
		reference7 = Vector3.Scale(spriteDef.positions[3], scale);
		ref Vector3 reference8 = ref pos[1];
		ref Vector3 reference9 = ref pos[2];
		reference8 = (reference9 = vector - vector3);
		ref Vector3 reference10 = ref pos[4];
		ref Vector3 reference11 = ref pos[8];
		reference10 = (reference11 = vector - vector2);
		ref Vector3 reference12 = ref pos[7];
		ref Vector3 reference13 = ref pos[11];
		reference12 = (reference13 = vector + vector2);
		ref Vector3 reference14 = ref pos[13];
		ref Vector3 reference15 = ref pos[14];
		reference14 = (reference15 = vector + vector3);
		Vector2 vector4 = (spriteDef.uvs[1] - spriteDef.uvs[0]) * 0.5f;
		Vector2 vector5 = (spriteDef.uvs[2] - spriteDef.uvs[0]) * 0.5f;
		ref Vector2 reference16 = ref uv[5];
		ref Vector2 reference17 = ref uv[6];
		ref Vector2 reference18 = ref uv[9];
		ref Vector2 reference19 = ref uv[10];
		reference16 = (reference17 = (reference18 = (reference19 = spriteDef.uvs[0] + vector4 + vector5)));
		ref Vector2 reference20 = ref uv[0];
		reference20 = spriteDef.uvs[0];
		ref Vector2 reference21 = ref uv[3];
		reference21 = spriteDef.uvs[1];
		ref Vector2 reference22 = ref uv[12];
		reference22 = spriteDef.uvs[2];
		ref Vector2 reference23 = ref uv[15];
		reference23 = spriteDef.uvs[3];
		ref Vector2 reference24 = ref uv[1];
		ref Vector2 reference25 = ref uv[2];
		reference24 = (reference25 = uv[5] - vector5);
		ref Vector2 reference26 = ref uv[4];
		ref Vector2 reference27 = ref uv[8];
		reference26 = (reference27 = uv[5] - vector4);
		ref Vector2 reference28 = ref uv[7];
		ref Vector2 reference29 = ref uv[11];
		reference28 = (reference29 = uv[5] + vector4);
		ref Vector2 reference30 = ref uv[13];
		ref Vector2 reference31 = ref uv[14];
		reference30 = (reference31 = uv[5] + vector5);
		if (norm != null && spriteDef.normals != null)
		{
			for (int i = 0; i < spriteDef.normals.Length; i++)
			{
				ref Vector3 reference32 = ref norm[offset + i];
				reference32 = spriteDef.normals[i];
			}
		}
		if (tang != null && spriteDef.tangents != null)
		{
			for (int j = 0; j < spriteDef.tangents.Length; j++)
			{
				ref Vector4 reference33 = ref tang[offset + j];
				reference33 = spriteDef.tangents[j];
			}
		}
	}

	public static void GetTiledSpriteGeomDesc(out int numVertices, out int numIndices, tk2dSpriteDefinition spriteDef, Vector2 dimensions)
	{
		int num = (int)Mathf.Ceil(dimensions.x * spriteDef.texelSize.x / spriteDef.untrimmedBoundsData[1].x);
		int num2 = (int)Mathf.Ceil(dimensions.y * spriteDef.texelSize.y / spriteDef.untrimmedBoundsData[1].y);
		numVertices = num * num2 * 4;
		numIndices = num * num2 * 6;
	}

	public static void SetTiledSpriteGeom(Vector3[] pos, Vector2[] uv, int offset, out Vector3 boundsCenter, out Vector3 boundsExtents, tk2dSpriteDefinition spriteDef, Vector3 scale, Vector2 dimensions, tk2dBaseSprite.Anchor anchor, float colliderOffsetZ, float colliderExtentZ)
	{
		boundsCenter = Vector3.zero;
		boundsExtents = Vector3.zero;
		int num = (int)Mathf.Ceil(dimensions.x * spriteDef.texelSize.x / spriteDef.untrimmedBoundsData[1].x);
		int num2 = (int)Mathf.Ceil(dimensions.y * spriteDef.texelSize.y / spriteDef.untrimmedBoundsData[1].y);
		Vector2 vector = new Vector2(dimensions.x * spriteDef.texelSize.x * scale.x, dimensions.y * spriteDef.texelSize.y * scale.y);
		Vector2 vector2 = Vector2.Scale(spriteDef.texelSize, scale) * 0.1f;
		Vector3 zero = Vector3.zero;
		switch (anchor)
		{
		case tk2dBaseSprite.Anchor.LowerCenter:
		case tk2dBaseSprite.Anchor.MiddleCenter:
		case tk2dBaseSprite.Anchor.UpperCenter:
			zero.x = 0f - vector.x / 2f;
			break;
		case tk2dBaseSprite.Anchor.LowerRight:
		case tk2dBaseSprite.Anchor.MiddleRight:
		case tk2dBaseSprite.Anchor.UpperRight:
			zero.x = 0f - vector.x;
			break;
		}
		switch (anchor)
		{
		case tk2dBaseSprite.Anchor.MiddleLeft:
		case tk2dBaseSprite.Anchor.MiddleCenter:
		case tk2dBaseSprite.Anchor.MiddleRight:
			zero.y = 0f - vector.y / 2f;
			break;
		case tk2dBaseSprite.Anchor.UpperLeft:
		case tk2dBaseSprite.Anchor.UpperCenter:
		case tk2dBaseSprite.Anchor.UpperRight:
			zero.y = 0f - vector.y;
			break;
		}
		Vector3 vector3 = zero;
		zero -= Vector3.Scale(spriteDef.positions[0], scale);
		boundsCenter.Set(vector.x * 0.5f + vector3.x, vector.y * 0.5f + vector3.y, colliderOffsetZ);
		boundsExtents.Set(vector.x * 0.5f, vector.y * 0.5f, colliderExtentZ);
		int num3 = 0;
		Vector3 vector4 = Vector3.Scale(spriteDef.untrimmedBoundsData[1], scale);
		Vector3 zero2 = Vector3.zero;
		Vector3 vector5 = zero2;
		for (int i = 0; i < num2; i++)
		{
			vector5.x = zero2.x;
			for (int j = 0; j < num; j++)
			{
				float num4 = 1f;
				float num5 = 1f;
				if (Mathf.Abs(vector5.x + vector4.x) > Mathf.Abs(vector.x) + vector2.x)
				{
					num4 = vector.x % vector4.x / vector4.x;
				}
				if (Mathf.Abs(vector5.y + vector4.y) > Mathf.Abs(vector.y) + vector2.y)
				{
					num5 = vector.y % vector4.y / vector4.y;
				}
				Vector3 vector6 = vector5 + zero;
				if (num4 != 1f || num5 != 1f)
				{
					Vector2 zero3 = Vector2.zero;
					Vector2 vector7 = new Vector2(num4, num5);
					Vector3 vector8 = new Vector3(Mathf.Lerp(spriteDef.positions[0].x, spriteDef.positions[3].x, zero3.x) * scale.x, Mathf.Lerp(spriteDef.positions[0].y, spriteDef.positions[3].y, zero3.y) * scale.y, spriteDef.positions[0].z * scale.z);
					Vector3 vector9 = new Vector3(Mathf.Lerp(spriteDef.positions[0].x, spriteDef.positions[3].x, vector7.x) * scale.x, Mathf.Lerp(spriteDef.positions[0].y, spriteDef.positions[3].y, vector7.y) * scale.y, spriteDef.positions[0].z * scale.z);
					ref Vector3 reference = ref pos[offset + num3];
					reference = vector6 + new Vector3(vector8.x, vector8.y, vector8.z);
					ref Vector3 reference2 = ref pos[offset + num3 + 1];
					reference2 = vector6 + new Vector3(vector9.x, vector8.y, vector8.z);
					ref Vector3 reference3 = ref pos[offset + num3 + 2];
					reference3 = vector6 + new Vector3(vector8.x, vector9.y, vector8.z);
					ref Vector3 reference4 = ref pos[offset + num3 + 3];
					reference4 = vector6 + new Vector3(vector9.x, vector9.y, vector8.z);
					if (spriteDef.flipped == tk2dSpriteDefinition.FlipMode.Tk2d)
					{
						Vector2 vector10 = new Vector2(Mathf.Lerp(spriteDef.uvs[0].x, spriteDef.uvs[3].x, zero3.y), Mathf.Lerp(spriteDef.uvs[0].y, spriteDef.uvs[3].y, zero3.x));
						Vector2 vector11 = new Vector2(Mathf.Lerp(spriteDef.uvs[0].x, spriteDef.uvs[3].x, vector7.y), Mathf.Lerp(spriteDef.uvs[0].y, spriteDef.uvs[3].y, vector7.x));
						ref Vector2 reference5 = ref uv[offset + num3];
						reference5 = new Vector2(vector10.x, vector10.y);
						ref Vector2 reference6 = ref uv[offset + num3 + 1];
						reference6 = new Vector2(vector10.x, vector11.y);
						ref Vector2 reference7 = ref uv[offset + num3 + 2];
						reference7 = new Vector2(vector11.x, vector10.y);
						ref Vector2 reference8 = ref uv[offset + num3 + 3];
						reference8 = new Vector2(vector11.x, vector11.y);
					}
					else if (spriteDef.flipped == tk2dSpriteDefinition.FlipMode.TPackerCW)
					{
						Vector2 vector12 = new Vector2(Mathf.Lerp(spriteDef.uvs[0].x, spriteDef.uvs[3].x, zero3.y), Mathf.Lerp(spriteDef.uvs[0].y, spriteDef.uvs[3].y, zero3.x));
						Vector2 vector13 = new Vector2(Mathf.Lerp(spriteDef.uvs[0].x, spriteDef.uvs[3].x, vector7.y), Mathf.Lerp(spriteDef.uvs[0].y, spriteDef.uvs[3].y, vector7.x));
						ref Vector2 reference9 = ref uv[offset + num3];
						reference9 = new Vector2(vector12.x, vector12.y);
						ref Vector2 reference10 = ref uv[offset + num3 + 2];
						reference10 = new Vector2(vector13.x, vector12.y);
						ref Vector2 reference11 = ref uv[offset + num3 + 1];
						reference11 = new Vector2(vector12.x, vector13.y);
						ref Vector2 reference12 = ref uv[offset + num3 + 3];
						reference12 = new Vector2(vector13.x, vector13.y);
					}
					else
					{
						Vector2 vector14 = new Vector2(Mathf.Lerp(spriteDef.uvs[0].x, spriteDef.uvs[3].x, zero3.x), Mathf.Lerp(spriteDef.uvs[0].y, spriteDef.uvs[3].y, zero3.y));
						Vector2 vector15 = new Vector2(Mathf.Lerp(spriteDef.uvs[0].x, spriteDef.uvs[3].x, vector7.x), Mathf.Lerp(spriteDef.uvs[0].y, spriteDef.uvs[3].y, vector7.y));
						ref Vector2 reference13 = ref uv[offset + num3];
						reference13 = new Vector2(vector14.x, vector14.y);
						ref Vector2 reference14 = ref uv[offset + num3 + 1];
						reference14 = new Vector2(vector15.x, vector14.y);
						ref Vector2 reference15 = ref uv[offset + num3 + 2];
						reference15 = new Vector2(vector14.x, vector15.y);
						ref Vector2 reference16 = ref uv[offset + num3 + 3];
						reference16 = new Vector2(vector15.x, vector15.y);
					}
				}
				else
				{
					ref Vector3 reference17 = ref pos[offset + num3];
					reference17 = vector6 + Vector3.Scale(spriteDef.positions[0], scale);
					ref Vector3 reference18 = ref pos[offset + num3 + 1];
					reference18 = vector6 + Vector3.Scale(spriteDef.positions[1], scale);
					ref Vector3 reference19 = ref pos[offset + num3 + 2];
					reference19 = vector6 + Vector3.Scale(spriteDef.positions[2], scale);
					ref Vector3 reference20 = ref pos[offset + num3 + 3];
					reference20 = vector6 + Vector3.Scale(spriteDef.positions[3], scale);
					ref Vector2 reference21 = ref uv[offset + num3];
					reference21 = spriteDef.uvs[0];
					ref Vector2 reference22 = ref uv[offset + num3 + 1];
					reference22 = spriteDef.uvs[1];
					ref Vector2 reference23 = ref uv[offset + num3 + 2];
					reference23 = spriteDef.uvs[2];
					ref Vector2 reference24 = ref uv[offset + num3 + 3];
					reference24 = spriteDef.uvs[3];
				}
				num3 += 4;
				vector5.x += vector4.x;
			}
			vector5.y += vector4.y;
		}
	}

	public static void SetTiledSpriteIndices(int[] indices, int offset, int vStart, tk2dSpriteDefinition spriteDef, Vector2 dimensions)
	{
		GetTiledSpriteGeomDesc(out var _, out var numIndices, spriteDef, dimensions);
		int num = 0;
		for (int i = 0; i < numIndices; i += 6)
		{
			indices[offset + i] = vStart + spriteDef.indices[0] + num;
			indices[offset + i + 1] = vStart + spriteDef.indices[1] + num;
			indices[offset + i + 2] = vStart + spriteDef.indices[2] + num;
			indices[offset + i + 3] = vStart + spriteDef.indices[3] + num;
			indices[offset + i + 4] = vStart + spriteDef.indices[4] + num;
			indices[offset + i + 5] = vStart + spriteDef.indices[5] + num;
			num += 4;
		}
	}

	public static void SetBoxMeshData(Vector3[] pos, int[] indices, int posOffset, int indicesOffset, int vStart, Vector3 origin, Vector3 extents, Matrix4x4 mat, Vector3 baseScale)
	{
		boxScaleMatrix.m03 = origin.x * baseScale.x;
		boxScaleMatrix.m13 = origin.y * baseScale.y;
		boxScaleMatrix.m23 = origin.z * baseScale.z;
		boxScaleMatrix.m00 = extents.x * baseScale.x;
		boxScaleMatrix.m11 = extents.y * baseScale.y;
		boxScaleMatrix.m22 = extents.z * baseScale.z;
		Matrix4x4 matrix4x = mat * boxScaleMatrix;
		for (int i = 0; i < 8; i++)
		{
			ref Vector3 reference = ref pos[posOffset + i];
			reference = matrix4x.MultiplyPoint(boxUnitVertices[i]);
		}
		float num = mat.m00 * mat.m11 * mat.m22 * baseScale.x * baseScale.y * baseScale.z;
		int[] array = ((!(num >= 0f)) ? boxIndicesBack : boxIndicesFwd);
		for (int j = 0; j < array.Length; j++)
		{
			indices[indicesOffset + j] = vStart + array[j];
		}
	}

	public static void SetSpriteDefinitionMeshData(Vector3[] pos, int[] indices, int posOffset, int indicesOffset, int vStart, tk2dSpriteDefinition spriteDef, Matrix4x4 mat, Vector3 baseScale)
	{
		for (int i = 0; i < spriteDef.colliderVertices.Length; i++)
		{
			Vector3 v = Vector3.Scale(spriteDef.colliderVertices[i], baseScale);
			v = mat.MultiplyPoint(v);
			pos[posOffset + i] = v;
		}
		float num = mat.m00 * mat.m11 * mat.m22;
		int[] array = ((!(num >= 0f)) ? spriteDef.colliderIndicesBack : spriteDef.colliderIndicesFwd);
		for (int j = 0; j < array.Length; j++)
		{
			indices[indicesOffset + j] = vStart + array[j];
		}
	}

	public static void SetSpriteVertexNormals(Vector3[] pos, Vector3 pMin, Vector3 pMax, Vector3[] spriteDefNormals, Vector4[] spriteDefTangents, Vector3[] normals, Vector4[] tangents)
	{
		Vector3 vector = pMax - pMin;
		int num = pos.Length;
		for (int i = 0; i < num; i++)
		{
			Vector3 vector2 = pos[i];
			float num2 = (vector2.x - pMin.x) / vector.x;
			float num3 = (vector2.y - pMin.y) / vector.y;
			float num4 = (1f - num2) * (1f - num3);
			float num5 = num2 * (1f - num3);
			float num6 = (1f - num2) * num3;
			float num7 = num2 * num3;
			if (spriteDefNormals != null && spriteDefNormals.Length == 4 && i < normals.Length)
			{
				ref Vector3 reference = ref normals[i];
				reference = spriteDefNormals[0] * num4 + spriteDefNormals[1] * num5 + spriteDefNormals[2] * num6 + spriteDefNormals[3] * num7;
			}
			if (spriteDefTangents != null && spriteDefTangents.Length == 4 && i < tangents.Length)
			{
				ref Vector4 reference2 = ref tangents[i];
				reference2 = spriteDefTangents[0] * num4 + spriteDefTangents[1] * num5 + spriteDefTangents[2] * num6 + spriteDefTangents[3] * num7;
			}
		}
	}
}
