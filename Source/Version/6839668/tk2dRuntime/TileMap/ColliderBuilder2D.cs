using System;
using System.Collections.Generic;
using UnityEngine;

namespace tk2dRuntime.TileMap;

public static class ColliderBuilder2D
{
	public static void Build(tk2dTileMap tileMap, bool forceBuild)
	{
		bool flag = !forceBuild;
		int num = tileMap.Layers.Length;
		for (int i = 0; i < num; i++)
		{
			Layer layer = tileMap.Layers[i];
			if (layer.IsEmpty || !tileMap.data.Layers[i].generateCollider)
			{
				continue;
			}
			for (int j = 0; j < layer.numRows; j++)
			{
				int baseY = j * layer.divY;
				for (int k = 0; k < layer.numColumns; k++)
				{
					int baseX = k * layer.divX;
					SpriteChunk chunk = layer.GetChunk(k, j);
					if ((flag && !chunk.Dirty) || chunk.IsEmpty)
					{
						continue;
					}
					BuildForChunk(tileMap, chunk, baseX, baseY);
					PhysicsMaterial2D physicsMaterial2D = tileMap.data.Layers[i].physicsMaterial2D;
					foreach (EdgeCollider2D edgeCollider in chunk.edgeColliders)
					{
						if (edgeCollider != null)
						{
							edgeCollider.sharedMaterial = physicsMaterial2D;
						}
					}
				}
			}
		}
	}

	public static void BuildForChunk(tk2dTileMap tileMap, SpriteChunk chunk, int baseX, int baseY)
	{
		Vector2[] vertices = new Vector2[0];
		int[] indices = new int[0];
		List<Vector2[]> list = new List<Vector2[]>();
		BuildLocalMeshForChunk(tileMap, chunk, baseX, baseY, ref vertices, ref indices);
		if (indices.Length > 4)
		{
			vertices = WeldVertices(vertices, ref indices);
			indices = RemoveDuplicateEdges(indices);
		}
		list = MergeEdges(vertices, indices);
		if (chunk.meshCollider != null)
		{
			tk2dUtil.DestroyImmediate(chunk.meshCollider);
			chunk.meshCollider = null;
		}
		if (list.Count == 0)
		{
			for (int i = 0; i < chunk.edgeColliders.Count; i++)
			{
				if (chunk.edgeColliders[i] != null)
				{
					tk2dUtil.DestroyImmediate(chunk.edgeColliders[i]);
				}
			}
			chunk.edgeColliders.Clear();
			return;
		}
		int count = list.Count;
		for (int j = count; j < chunk.edgeColliders.Count; j++)
		{
			if (chunk.edgeColliders[j] != null)
			{
				tk2dUtil.DestroyImmediate(chunk.edgeColliders[j]);
			}
		}
		int num = chunk.edgeColliders.Count - count;
		if (num > 0)
		{
			chunk.edgeColliders.RemoveRange(chunk.edgeColliders.Count - num, num);
		}
		for (int k = 0; k < chunk.edgeColliders.Count; k++)
		{
			if (chunk.edgeColliders[k] == null)
			{
				chunk.edgeColliders[k] = tk2dUtil.AddComponent<EdgeCollider2D>(chunk.gameObject);
			}
		}
		while (chunk.edgeColliders.Count < count)
		{
			chunk.edgeColliders.Add(tk2dUtil.AddComponent<EdgeCollider2D>(chunk.gameObject));
		}
		for (int l = 0; l < count; l++)
		{
			chunk.edgeColliders[l].points = list[l];
		}
	}

	private static void BuildLocalMeshForChunk(tk2dTileMap tileMap, SpriteChunk chunk, int baseX, int baseY, ref Vector2[] vertices, ref int[] indices)
	{
		List<Vector2> list = new List<Vector2>();
		List<int> list2 = new List<int>();
		Vector2[] array = new Vector2[4];
		int[] array2 = new int[8] { 0, 1, 1, 2, 2, 3, 3, 0 };
		int[] array3 = new int[8] { 0, 3, 3, 2, 2, 1, 1, 0 };
		int num = tileMap.SpriteCollectionInst.spriteDefinitions.Length;
		Vector2 vector = new Vector3(tileMap.data.tileSize.x, tileMap.data.tileSize.y);
		GameObject[] tilePrefabs = tileMap.data.tilePrefabs;
		float x = 0f;
		float y = 0f;
		tileMap.data.GetTileOffset(out x, out y);
		int[] spriteIds = chunk.spriteIds;
		for (int i = 0; i < tileMap.partitionSizeY; i++)
		{
			float num2 = (float)((baseY + i) & 1) * x;
			for (int j = 0; j < tileMap.partitionSizeX; j++)
			{
				int rawTile = spriteIds[i * tileMap.partitionSizeX + j];
				int tileFromRawTile = BuilderUtil.GetTileFromRawTile(rawTile);
				Vector2 vector2 = new Vector2(vector.x * ((float)j + num2), vector.y * (float)i);
				if (tileFromRawTile < 0 || tileFromRawTile >= num || (bool)tilePrefabs[tileFromRawTile])
				{
					continue;
				}
				bool flag = BuilderUtil.IsRawTileFlagSet(rawTile, tk2dTileFlags.FlipX);
				bool flag2 = BuilderUtil.IsRawTileFlagSet(rawTile, tk2dTileFlags.FlipY);
				bool rot = BuilderUtil.IsRawTileFlagSet(rawTile, tk2dTileFlags.Rot90);
				bool flag3 = false;
				if (flag)
				{
					flag3 = !flag3;
				}
				if (flag2)
				{
					flag3 = !flag3;
				}
				tk2dSpriteDefinition tk2dSpriteDefinition = tileMap.SpriteCollectionInst.spriteDefinitions[tileFromRawTile];
				int count = list.Count;
				if (tk2dSpriteDefinition.colliderType == tk2dSpriteDefinition.ColliderType.Box)
				{
					Vector3 vector3 = tk2dSpriteDefinition.colliderVertices[0];
					Vector3 vector4 = tk2dSpriteDefinition.colliderVertices[1];
					Vector3 vector5 = vector3 - vector4;
					Vector3 vector6 = vector3 + vector4;
					ref Vector2 reference = ref array[0];
					reference = new Vector2(vector5.x, vector5.y);
					ref Vector2 reference2 = ref array[1];
					reference2 = new Vector2(vector6.x, vector5.y);
					ref Vector2 reference3 = ref array[2];
					reference3 = new Vector2(vector6.x, vector6.y);
					ref Vector2 reference4 = ref array[3];
					reference4 = new Vector2(vector5.x, vector6.y);
					for (int k = 0; k < 4; k++)
					{
						list.Add(BuilderUtil.ApplySpriteVertexTileFlags(tileMap, tk2dSpriteDefinition, array[k], flag, flag2, rot) + vector2);
					}
					int[] array4 = ((!flag3) ? array2 : array3);
					for (int l = 0; l < 8; l++)
					{
						list2.Add(count + array4[l]);
					}
				}
				else
				{
					if (tk2dSpriteDefinition.colliderType != tk2dSpriteDefinition.ColliderType.Mesh)
					{
						continue;
					}
					tk2dCollider2DData[] edgeCollider2D = tk2dSpriteDefinition.edgeCollider2D;
					foreach (tk2dCollider2DData tk2dCollider2DData in edgeCollider2D)
					{
						count = list.Count;
						Vector2[] points = tk2dCollider2DData.points;
						foreach (Vector2 pos in points)
						{
							list.Add(BuilderUtil.ApplySpriteVertexTileFlags(tileMap, tk2dSpriteDefinition, pos, flag, flag2, rot) + vector2);
						}
						int num3 = tk2dCollider2DData.points.Length;
						if (flag3)
						{
							for (int num4 = num3 - 1; num4 > 0; num4--)
							{
								list2.Add(count + num4);
								list2.Add(count + num4 - 1);
							}
						}
						else
						{
							for (int num5 = 0; num5 < num3 - 1; num5++)
							{
								list2.Add(count + num5);
								list2.Add(count + num5 + 1);
							}
						}
					}
					tk2dCollider2DData[] polygonCollider2D = tk2dSpriteDefinition.polygonCollider2D;
					foreach (tk2dCollider2DData tk2dCollider2DData2 in polygonCollider2D)
					{
						count = list.Count;
						Vector2[] points2 = tk2dCollider2DData2.points;
						foreach (Vector2 pos2 in points2)
						{
							list.Add(BuilderUtil.ApplySpriteVertexTileFlags(tileMap, tk2dSpriteDefinition, pos2, flag, flag2, rot) + vector2);
						}
						int num8 = tk2dCollider2DData2.points.Length;
						if (flag3)
						{
							for (int num9 = num8; num9 > 0; num9--)
							{
								list2.Add(count + num9 % num8);
								list2.Add(count + num9 - 1);
							}
						}
						else
						{
							for (int num10 = 0; num10 < num8; num10++)
							{
								list2.Add(count + num10);
								list2.Add(count + (num10 + 1) % num8);
							}
						}
					}
				}
			}
		}
		vertices = list.ToArray();
		indices = list2.ToArray();
	}

	private static int CompareWeldVertices(Vector2 a, Vector2 b)
	{
		float num = 0.01f;
		float f = a.x - b.x;
		if (Mathf.Abs(f) > num)
		{
			return (int)Mathf.Sign(f);
		}
		float f2 = a.y - b.y;
		if (Mathf.Abs(f2) > num)
		{
			return (int)Mathf.Sign(f2);
		}
		return 0;
	}

	private static Vector2[] WeldVertices(Vector2[] vertices, ref int[] indices)
	{
		int[] array = new int[vertices.Length];
		for (int i = 0; i < vertices.Length; i++)
		{
			array[i] = i;
		}
		Array.Sort(array, (int a, int b) => CompareWeldVertices(vertices[a], vertices[b]));
		List<Vector2> list = new List<Vector2>();
		int[] array2 = new int[vertices.Length];
		Vector2 vector = vertices[array[0]];
		list.Add(vector);
		array2[array[0]] = list.Count - 1;
		for (int j = 1; j < array.Length; j++)
		{
			Vector2 vector2 = vertices[array[j]];
			if (CompareWeldVertices(vector2, vector) != 0)
			{
				vector = vector2;
				list.Add(vector);
				array2[array[j]] = list.Count - 1;
			}
			array2[array[j]] = list.Count - 1;
		}
		for (int k = 0; k < indices.Length; k++)
		{
			indices[k] = array2[indices[k]];
		}
		return list.ToArray();
	}

	private static int CompareDuplicateFaces(int[] indices, int face0index, int face1index)
	{
		for (int i = 0; i < 2; i++)
		{
			int num = indices[face0index + i] - indices[face1index + i];
			if (num != 0)
			{
				return num;
			}
		}
		return 0;
	}

	private static int[] RemoveDuplicateEdges(int[] indices)
	{
		int[] sortedFaceIndices = new int[indices.Length];
		for (int i = 0; i < indices.Length; i += 2)
		{
			if (indices[i] > indices[i + 1])
			{
				sortedFaceIndices[i] = indices[i + 1];
				sortedFaceIndices[i + 1] = indices[i];
			}
			else
			{
				sortedFaceIndices[i] = indices[i];
				sortedFaceIndices[i + 1] = indices[i + 1];
			}
		}
		int[] array = new int[indices.Length / 2];
		for (int j = 0; j < indices.Length; j += 2)
		{
			array[j / 2] = j;
		}
		Array.Sort(array, (int a, int b) => CompareDuplicateFaces(sortedFaceIndices, a, b));
		List<int> list = new List<int>();
		for (int k = 0; k < array.Length; k++)
		{
			if (k != array.Length - 1 && CompareDuplicateFaces(sortedFaceIndices, array[k], array[k + 1]) == 0)
			{
				k++;
				continue;
			}
			for (int l = 0; l < 2; l++)
			{
				list.Add(indices[array[k] + l]);
			}
		}
		return list.ToArray();
	}

	private static List<Vector2[]> MergeEdges(Vector2[] verts, int[] indices)
	{
		List<Vector2[]> list = new List<Vector2[]>();
		List<Vector2> list2 = new List<Vector2>();
		List<int> list3 = new List<int>();
		Vector2 zero = Vector2.zero;
		Vector2 zero2 = Vector2.zero;
		bool[] array = new bool[indices.Length / 2];
		bool flag = true;
		while (flag)
		{
			flag = false;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i])
				{
					continue;
				}
				array[i] = true;
				int num = indices[i * 2];
				int num2 = indices[i * 2 + 1];
				zero = (verts[num2] - verts[num]).normalized;
				list3.Add(num);
				list3.Add(num2);
				for (int j = i + 1; j < array.Length; j++)
				{
					if (array[j])
					{
						continue;
					}
					int num3 = indices[j * 2];
					if (num3 == num2)
					{
						int num4 = indices[j * 2 + 1];
						zero2 = (verts[num4] - verts[num3]).normalized;
						if (Vector2.Dot(zero2, zero) > 0.999f)
						{
							list3.RemoveAt(list3.Count - 1);
						}
						list3.Add(num4);
						array[j] = true;
						zero = zero2;
						j = i;
						num2 = num4;
					}
				}
				flag = true;
				break;
			}
			if (flag)
			{
				list2.Clear();
				list2.Capacity = Mathf.Max(list2.Capacity, list3.Count);
				for (int k = 0; k < list3.Count; k++)
				{
					list2.Add(verts[list3[k]]);
				}
				list.Add(list2.ToArray());
				list3.Clear();
			}
		}
		return list;
	}
}
