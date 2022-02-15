using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace tk2dRuntime;

internal static class SpriteCollectionGenerator
{
	public static tk2dSpriteCollectionData CreateFromTexture(Texture texture, tk2dSpriteCollectionSize size, Rect region, Vector2 anchor)
	{
		return CreateFromTexture(texture, size, new string[1] { "Unnamed" }, new Rect[1] { region }, new Vector2[1] { anchor });
	}

	public static tk2dSpriteCollectionData CreateFromTexture(Texture texture, tk2dSpriteCollectionSize size, string[] names, Rect[] regions, Vector2[] anchors)
	{
		Vector2 textureDimensions = new Vector2(texture.width, texture.height);
		return CreateFromTexture(texture, size, textureDimensions, names, regions, null, anchors, null);
	}

	public static tk2dSpriteCollectionData CreateFromTexture(Texture texture, tk2dSpriteCollectionSize size, Vector2 textureDimensions, string[] names, Rect[] regions, Rect[] trimRects, Vector2[] anchors, bool[] rotated)
	{
		return CreateFromTexture(null, texture, size, textureDimensions, names, regions, trimRects, anchors, rotated);
	}

	public static tk2dSpriteCollectionData CreateFromTexture(GameObject parentObject, Texture texture, tk2dSpriteCollectionSize size, Vector2 textureDimensions, string[] names, Rect[] regions, Rect[] trimRects, Vector2[] anchors, bool[] rotated)
	{
		GameObject gameObject = ((!(parentObject != null)) ? new GameObject("SpriteCollection") : parentObject);
		tk2dSpriteCollectionData tk2dSpriteCollectionData = gameObject.AddComponent<tk2dSpriteCollectionData>();
		tk2dSpriteCollectionData.Transient = true;
		tk2dSpriteCollectionData.version = 3;
		tk2dSpriteCollectionData.invOrthoSize = 1f / size.OrthoSize;
		tk2dSpriteCollectionData.halfTargetHeight = size.TargetHeight * 0.5f;
		tk2dSpriteCollectionData.premultipliedAlpha = false;
		string name = "tk2d/BlendVertexColor";
		tk2dSpriteCollectionData.material = new Material(Shader.Find(name));
		tk2dSpriteCollectionData.material.mainTexture = texture;
		tk2dSpriteCollectionData.materials = new Material[1] { tk2dSpriteCollectionData.material };
		tk2dSpriteCollectionData.textures = new Texture[1] { texture };
		tk2dSpriteCollectionData.buildKey = Random.Range(0, int.MaxValue);
		float scale = 2f * size.OrthoSize / size.TargetHeight;
		Rect trimRect = new Rect(0f, 0f, 0f, 0f);
		tk2dSpriteCollectionData.spriteDefinitions = new tk2dSpriteDefinition[regions.Length];
		for (int i = 0; i < regions.Length; i++)
		{
			bool flag = rotated != null && rotated[i];
			if (trimRects != null)
			{
				trimRect = trimRects[i];
			}
			else if (flag)
			{
				trimRect.Set(0f, 0f, regions[i].height, regions[i].width);
			}
			else
			{
				trimRect.Set(0f, 0f, regions[i].width, regions[i].height);
			}
			tk2dSpriteCollectionData.spriteDefinitions[i] = CreateDefinitionForRegionInTexture(names[i], textureDimensions, scale, regions[i], trimRect, anchors[i], flag);
		}
		tk2dSpriteDefinition[] spriteDefinitions = tk2dSpriteCollectionData.spriteDefinitions;
		foreach (tk2dSpriteDefinition tk2dSpriteDefinition in spriteDefinitions)
		{
			tk2dSpriteDefinition.material = tk2dSpriteCollectionData.material;
		}
		return tk2dSpriteCollectionData;
	}

	private static tk2dSpriteDefinition CreateDefinitionForRegionInTexture(string name, Vector2 textureDimensions, float scale, Rect uvRegion, Rect trimRect, Vector2 anchor, bool rotated)
	{
		float height = uvRegion.height;
		float width = uvRegion.width;
		float x = textureDimensions.x;
		float y = textureDimensions.y;
		tk2dSpriteDefinition tk2dSpriteDefinition = new tk2dSpriteDefinition();
		tk2dSpriteDefinition.flipped = (rotated ? tk2dSpriteDefinition.FlipMode.TPackerCW : tk2dSpriteDefinition.FlipMode.None);
		tk2dSpriteDefinition.extractRegion = false;
		tk2dSpriteDefinition.name = name;
		tk2dSpriteDefinition.colliderType = tk2dSpriteDefinition.ColliderType.Unset;
		Vector2 vector = new Vector2(0.001f, 0.001f);
		Vector2 vector2 = new Vector2((uvRegion.x + vector.x) / x, 1f - (uvRegion.y + uvRegion.height + vector.y) / y);
		Vector2 vector3 = new Vector2((uvRegion.x + uvRegion.width - vector.x) / x, 1f - (uvRegion.y - vector.y) / y);
		Vector2 vector4 = new Vector2(trimRect.x - anchor.x, 0f - trimRect.y + anchor.y);
		if (rotated)
		{
			vector4.y -= width;
		}
		vector4 *= scale;
		Vector3 vector5 = new Vector3((0f - anchor.x) * scale, anchor.y * scale, 0f);
		Vector3 vector6 = vector5 + new Vector3(trimRect.width * scale, (0f - trimRect.height) * scale, 0f);
		Vector3 vector7 = new Vector3(0f, (0f - height) * scale, 0f);
		Vector3 vector8 = vector7 + new Vector3(width * scale, height * scale, 0f);
		if (rotated)
		{
			tk2dSpriteDefinition.positions = new Vector3[4]
			{
				new Vector3(0f - vector8.y + vector4.x, vector7.x + vector4.y, 0f),
				new Vector3(0f - vector7.y + vector4.x, vector7.x + vector4.y, 0f),
				new Vector3(0f - vector8.y + vector4.x, vector8.x + vector4.y, 0f),
				new Vector3(0f - vector7.y + vector4.x, vector8.x + vector4.y, 0f)
			};
			tk2dSpriteDefinition.uvs = new Vector2[4]
			{
				new Vector2(vector2.x, vector3.y),
				new Vector2(vector2.x, vector2.y),
				new Vector2(vector3.x, vector3.y),
				new Vector2(vector3.x, vector2.y)
			};
		}
		else
		{
			tk2dSpriteDefinition.positions = new Vector3[4]
			{
				new Vector3(vector7.x + vector4.x, vector7.y + vector4.y, 0f),
				new Vector3(vector8.x + vector4.x, vector7.y + vector4.y, 0f),
				new Vector3(vector7.x + vector4.x, vector8.y + vector4.y, 0f),
				new Vector3(vector8.x + vector4.x, vector8.y + vector4.y, 0f)
			};
			tk2dSpriteDefinition.uvs = new Vector2[4]
			{
				new Vector2(vector2.x, vector2.y),
				new Vector2(vector3.x, vector2.y),
				new Vector2(vector2.x, vector3.y),
				new Vector2(vector3.x, vector3.y)
			};
		}
		tk2dSpriteDefinition.normals = new Vector3[0];
		tk2dSpriteDefinition.tangents = new Vector4[0];
		tk2dSpriteDefinition.indices = new int[6] { 0, 3, 1, 2, 3, 0 };
		Vector3 vector9 = new Vector3(vector5.x, vector6.y, 0f);
		Vector3 vector10 = new Vector3(vector6.x, vector5.y, 0f);
		tk2dSpriteDefinition.boundsData = new Vector3[2]
		{
			(vector10 + vector9) / 2f,
			vector10 - vector9
		};
		tk2dSpriteDefinition.untrimmedBoundsData = new Vector3[2]
		{
			(vector10 + vector9) / 2f,
			vector10 - vector9
		};
		tk2dSpriteDefinition.texelSize = new Vector2(scale, scale);
		return tk2dSpriteDefinition;
	}

	public static tk2dSpriteCollectionData CreateFromTexturePacker(tk2dSpriteCollectionSize spriteCollectionSize, string texturePackerFileContents, Texture texture)
	{
		List<string> list = new List<string>();
		List<Rect> list2 = new List<Rect>();
		List<Rect> list3 = new List<Rect>();
		List<Vector2> list4 = new List<Vector2>();
		List<bool> list5 = new List<bool>();
		int num = 0;
		TextReader textReader = new StringReader(texturePackerFileContents);
		bool flag = false;
		bool flag2 = false;
		string item = string.Empty;
		Rect item2 = default(Rect);
		Rect item3 = default(Rect);
		Vector2 zero = Vector2.zero;
		Vector2 zero2 = Vector2.zero;
		for (string text = textReader.ReadLine(); text != null; text = textReader.ReadLine())
		{
			if (text.Length > 0)
			{
				char c = text[0];
				switch (num)
				{
				case 0:
					switch (c)
					{
					case 'w':
						zero.x = int.Parse(text.Substring(2));
						break;
					case 'h':
						zero.y = int.Parse(text.Substring(2));
						break;
					case '~':
						num++;
						break;
					}
					break;
				case 1:
					switch (c)
					{
					case 'n':
						item = text.Substring(2);
						break;
					case 'r':
						flag = int.Parse(text.Substring(2)) == 1;
						break;
					case 's':
					{
						string[] array = text.Split();
						item2.Set(int.Parse(array[1]), int.Parse(array[2]), int.Parse(array[3]), int.Parse(array[4]));
						break;
					}
					case 'o':
					{
						string[] array2 = text.Split();
						item3.Set(int.Parse(array2[1]), int.Parse(array2[2]), int.Parse(array2[3]), int.Parse(array2[4]));
						flag2 = true;
						break;
					}
					case '~':
						list.Add(item);
						list5.Add(flag);
						list2.Add(item2);
						if (!flag2)
						{
							if (flag)
							{
								item3.Set(0f, 0f, item2.height, item2.width);
							}
							else
							{
								item3.Set(0f, 0f, item2.width, item2.height);
							}
						}
						list3.Add(item3);
						zero2.Set((int)(item3.width / 2f), (int)(item3.height / 2f));
						list4.Add(zero2);
						item = string.Empty;
						flag2 = false;
						flag = false;
						break;
					}
					break;
				}
			}
		}
		return CreateFromTexture(texture, spriteCollectionSize, zero, list.ToArray(), list2.ToArray(), list3.ToArray(), list4.ToArray(), list5.ToArray());
	}
}
