using System.Collections.Generic;
using UnityEngine;

namespace tk2dRuntime.TileMap;

public static class BuilderUtil
{
	private static List<int> TilePrefabsX;

	private static List<int> TilePrefabsY;

	private static List<int> TilePrefabsLayer;

	private static List<GameObject> TilePrefabsInstance;

	private const int tileMask = 16777215;

	public static bool InitDataStore(tk2dTileMap tileMap)
	{
		bool result = false;
		int numLayers = tileMap.data.NumLayers;
		if (tileMap.Layers == null)
		{
			tileMap.Layers = new Layer[numLayers];
			for (int i = 0; i < numLayers; i++)
			{
				tileMap.Layers[i] = new Layer(tileMap.data.Layers[i].hash, tileMap.width, tileMap.height, tileMap.partitionSizeX, tileMap.partitionSizeY);
			}
			result = true;
		}
		else
		{
			Layer[] array = new Layer[numLayers];
			for (int j = 0; j < numLayers; j++)
			{
				LayerInfo layerInfo = tileMap.data.Layers[j];
				bool flag = false;
				for (int k = 0; k < tileMap.Layers.Length; k++)
				{
					if (tileMap.Layers[k].hash == layerInfo.hash)
					{
						array[j] = tileMap.Layers[k];
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					array[j] = new Layer(layerInfo.hash, tileMap.width, tileMap.height, tileMap.partitionSizeX, tileMap.partitionSizeY);
				}
			}
			int num = 0;
			Layer[] array2 = array;
			foreach (Layer layer in array2)
			{
				if (!layer.IsEmpty)
				{
					num++;
				}
			}
			int num2 = 0;
			Layer[] layers = tileMap.Layers;
			foreach (Layer layer2 in layers)
			{
				if (!layer2.IsEmpty)
				{
					num2++;
				}
			}
			if (num != num2)
			{
				result = true;
			}
			tileMap.Layers = array;
		}
		if (tileMap.ColorChannel == null)
		{
			tileMap.ColorChannel = new ColorChannel(tileMap.width, tileMap.height, tileMap.partitionSizeX, tileMap.partitionSizeY);
		}
		return result;
	}

	private static GameObject GetExistingTilePrefabInstance(tk2dTileMap tileMap, int tileX, int tileY, int tileLayer)
	{
		int tilePrefabsListCount = tileMap.GetTilePrefabsListCount();
		for (int i = 0; i < tilePrefabsListCount; i++)
		{
			tileMap.GetTilePrefabsListItem(i, out var x, out var y, out var layer, out var instance);
			if (x == tileX && y == tileY && layer == tileLayer)
			{
				return instance;
			}
		}
		return null;
	}

	public static void SpawnPrefabsForChunk(tk2dTileMap tileMap, SpriteChunk chunk, int baseX, int baseY, int layer, int[] prefabCounts)
	{
		int[] spriteIds = chunk.spriteIds;
		GameObject[] tilePrefabs = tileMap.data.tilePrefabs;
		Vector3 tileSize = tileMap.data.tileSize;
		Transform transform = chunk.gameObject.transform;
		float x = 0f;
		float y = 0f;
		tileMap.data.GetTileOffset(out x, out y);
		for (int i = 0; i < tileMap.partitionSizeY; i++)
		{
			float num = (float)((baseY + i) & 1) * x;
			for (int j = 0; j < tileMap.partitionSizeX; j++)
			{
				int tileFromRawTile = GetTileFromRawTile(spriteIds[i * tileMap.partitionSizeX + j]);
				if (tileFromRawTile < 0 || tileFromRawTile >= tilePrefabs.Length)
				{
					continue;
				}
				Object @object = tilePrefabs[tileFromRawTile];
				if (!(@object != null))
				{
					continue;
				}
				prefabCounts[tileFromRawTile]++;
				GameObject gameObject = GetExistingTilePrefabInstance(tileMap, baseX + j, baseY + i, layer);
				bool flag = gameObject != null;
				if (gameObject == null)
				{
					gameObject = Object.Instantiate(@object, Vector3.zero, Quaternion.identity) as GameObject;
				}
				if (gameObject != null)
				{
					GameObject gameObject2 = @object as GameObject;
					Vector3 localPosition = new Vector3(tileSize.x * ((float)j + num), tileSize.y * (float)i, 0f);
					bool flag2 = false;
					TileInfo tileInfoForSprite = tileMap.data.GetTileInfoForSprite(tileFromRawTile);
					if (tileInfoForSprite != null)
					{
						flag2 = tileInfoForSprite.enablePrefabOffset;
					}
					if (flag2 && gameObject2 != null)
					{
						localPosition += gameObject2.transform.position;
					}
					if (!flag)
					{
						gameObject.name = @object.name + " " + prefabCounts[tileFromRawTile];
					}
					tk2dUtil.SetTransformParent(gameObject.transform, transform);
					gameObject.transform.localPosition = localPosition;
					TilePrefabsX.Add(baseX + j);
					TilePrefabsY.Add(baseY + i);
					TilePrefabsLayer.Add(layer);
					TilePrefabsInstance.Add(gameObject);
				}
			}
		}
	}

	public static void SpawnPrefabs(tk2dTileMap tileMap, bool forceBuild)
	{
		TilePrefabsX = new List<int>();
		TilePrefabsY = new List<int>();
		TilePrefabsLayer = new List<int>();
		TilePrefabsInstance = new List<GameObject>();
		int[] prefabCounts = new int[tileMap.data.tilePrefabs.Length];
		int num = tileMap.Layers.Length;
		for (int i = 0; i < num; i++)
		{
			Layer layer = tileMap.Layers[i];
			LayerInfo layerInfo = tileMap.data.Layers[i];
			if (layer.IsEmpty || layerInfo.skipMeshGeneration)
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
					if (!chunk.IsEmpty && (forceBuild || chunk.Dirty))
					{
						SpawnPrefabsForChunk(tileMap, chunk, baseX, baseY, i, prefabCounts);
					}
				}
			}
		}
		tileMap.SetTilePrefabsList(TilePrefabsX, TilePrefabsY, TilePrefabsLayer, TilePrefabsInstance);
	}

	public static void HideTileMapPrefabs(tk2dTileMap tileMap)
	{
		if (tileMap.renderData == null || tileMap.Layers == null)
		{
			return;
		}
		if (tileMap.PrefabsRoot == null)
		{
			GameObject gameObject2 = (tileMap.PrefabsRoot = tk2dUtil.CreateGameObject("Prefabs"));
			GameObject gameObject3 = gameObject2;
			gameObject3.transform.parent = tileMap.renderData.transform;
			gameObject3.transform.localPosition = Vector3.zero;
			gameObject3.transform.localRotation = Quaternion.identity;
			gameObject3.transform.localScale = Vector3.one;
		}
		int tilePrefabsListCount = tileMap.GetTilePrefabsListCount();
		bool[] array = new bool[tilePrefabsListCount];
		for (int i = 0; i < tileMap.Layers.Length; i++)
		{
			Layer layer = tileMap.Layers[i];
			for (int j = 0; j < layer.spriteChannel.chunks.Length; j++)
			{
				SpriteChunk spriteChunk = layer.spriteChannel.chunks[j];
				if (spriteChunk.gameObject == null)
				{
					continue;
				}
				Transform transform = spriteChunk.gameObject.transform;
				int childCount = transform.childCount;
				for (int k = 0; k < childCount; k++)
				{
					GameObject gameObject4 = transform.GetChild(k).gameObject;
					for (int l = 0; l < tilePrefabsListCount; l++)
					{
						tileMap.GetTilePrefabsListItem(l, out var _, out var _, out var _, out var instance);
						if (instance == gameObject4)
						{
							array[l] = true;
							break;
						}
					}
				}
			}
		}
		Object[] tilePrefabs = tileMap.data.tilePrefabs;
		List<int> list = new List<int>();
		List<int> list2 = new List<int>();
		List<int> list3 = new List<int>();
		List<GameObject> list4 = new List<GameObject>();
		for (int m = 0; m < tilePrefabsListCount; m++)
		{
			tileMap.GetTilePrefabsListItem(m, out var x2, out var y2, out var layer3, out var instance2);
			if (!array[m])
			{
				int num = ((x2 < 0 || x2 >= tileMap.width || y2 < 0 || y2 >= tileMap.height) ? (-1) : tileMap.GetTile(x2, y2, layer3));
				if (num >= 0 && num < tilePrefabs.Length && tilePrefabs[num] != null)
				{
					array[m] = true;
				}
			}
			if (array[m])
			{
				list.Add(x2);
				list2.Add(y2);
				list3.Add(layer3);
				list4.Add(instance2);
				tk2dUtil.SetTransformParent(instance2.transform, tileMap.PrefabsRoot.transform);
			}
		}
		tileMap.SetTilePrefabsList(list, list2, list3, list4);
	}

	private static Vector3 GetTilePosition(tk2dTileMap tileMap, int x, int y)
	{
		return new Vector3(tileMap.data.tileSize.x * (float)x, tileMap.data.tileSize.y * (float)y, 0f);
	}

	public static void CreateRenderData(tk2dTileMap tileMap, bool editMode, Dictionary<Layer, bool> layersActive)
	{
		if (tileMap.renderData == null)
		{
			tileMap.renderData = tk2dUtil.CreateGameObject(tileMap.name + " Render Data");
		}
		tileMap.renderData.transform.position = tileMap.transform.position;
		float num = 0f;
		int num2 = 0;
		Layer[] layers = tileMap.Layers;
		foreach (Layer layer in layers)
		{
			float z = tileMap.data.Layers[num2].z;
			if (num2 != 0)
			{
				num -= z;
			}
			if (layer.IsEmpty && layer.gameObject != null)
			{
				tk2dUtil.DestroyImmediate(layer.gameObject);
				layer.gameObject = null;
			}
			else if (!layer.IsEmpty && layer.gameObject == null)
			{
				(layer.gameObject = tk2dUtil.CreateGameObject(string.Empty)).transform.parent = tileMap.renderData.transform;
			}
			int unityLayer = tileMap.data.Layers[num2].unityLayer;
			if (layer.gameObject != null)
			{
				if (!editMode && layersActive.ContainsKey(layer) && layer.gameObject.activeSelf != layersActive[layer])
				{
					layer.gameObject.SetActive(layersActive[layer]);
				}
				layer.gameObject.name = tileMap.data.Layers[num2].name;
				layer.gameObject.transform.localPosition = new Vector3(0f, 0f, (!tileMap.data.layersFixedZ) ? num : (0f - z));
				layer.gameObject.transform.localRotation = Quaternion.identity;
				layer.gameObject.transform.localScale = Vector3.one;
				layer.gameObject.layer = unityLayer;
			}
			GetLoopOrder(tileMap.data.sortMethod, layer.numColumns, layer.numRows, out var x, out var x2, out var dx, out var y, out var y2, out var dy);
			float num3 = 0f;
			for (int j = y; j != y2; j += dy)
			{
				for (int k = x; k != x2; k += dx)
				{
					SpriteChunk chunk = layer.GetChunk(k, j);
					bool flag = layer.IsEmpty || chunk.IsEmpty;
					if (editMode)
					{
						flag = false;
					}
					if (flag && chunk.HasGameData)
					{
						chunk.DestroyGameData(tileMap);
					}
					else if (!flag && chunk.gameObject == null)
					{
						string name = "Chunk " + j + " " + k;
						GameObject gameObject = (chunk.gameObject = tk2dUtil.CreateGameObject(name));
						gameObject.transform.parent = layer.gameObject.transform;
						MeshFilter meshFilter = tk2dUtil.AddComponent<MeshFilter>(gameObject);
						tk2dUtil.AddComponent<MeshRenderer>(gameObject);
						chunk.mesh = tk2dUtil.CreateMesh();
						meshFilter.mesh = chunk.mesh;
					}
					if (chunk.gameObject != null)
					{
						Vector3 tilePosition = GetTilePosition(tileMap, k * tileMap.partitionSizeX, j * tileMap.partitionSizeY);
						tilePosition.z += num3;
						chunk.gameObject.transform.localPosition = tilePosition;
						chunk.gameObject.transform.localRotation = Quaternion.identity;
						chunk.gameObject.transform.localScale = Vector3.one;
						chunk.gameObject.layer = unityLayer;
						if (editMode)
						{
							chunk.DestroyColliderData(tileMap);
						}
					}
					num3 -= 1E-06f;
				}
			}
			num2++;
		}
	}

	public static void GetLoopOrder(tk2dTileMapData.SortMethod sortMethod, int w, int h, out int x0, out int x1, out int dx, out int y0, out int y1, out int dy)
	{
		switch (sortMethod)
		{
		case tk2dTileMapData.SortMethod.BottomLeft:
			x0 = 0;
			x1 = w;
			dx = 1;
			y0 = 0;
			y1 = h;
			dy = 1;
			break;
		case tk2dTileMapData.SortMethod.BottomRight:
			x0 = w - 1;
			x1 = -1;
			dx = -1;
			y0 = 0;
			y1 = h;
			dy = 1;
			break;
		case tk2dTileMapData.SortMethod.TopLeft:
			x0 = 0;
			x1 = w;
			dx = 1;
			y0 = h - 1;
			y1 = -1;
			dy = -1;
			break;
		case tk2dTileMapData.SortMethod.TopRight:
			x0 = w - 1;
			x1 = -1;
			dx = -1;
			y0 = h - 1;
			y1 = -1;
			dy = -1;
			break;
		default:
			DebugLogWrapper.LogError("Unhandled sort method");
			goto case tk2dTileMapData.SortMethod.BottomLeft;
		}
	}

	public static int GetTileFromRawTile(int rawTile)
	{
		if (rawTile == -1)
		{
			return -1;
		}
		return rawTile & 0xFFFFFF;
	}

	public static bool IsRawTileFlagSet(int rawTile, tk2dTileFlags flag)
	{
		if (rawTile == -1)
		{
			return false;
		}
		return ((uint)rawTile & (uint)flag) != 0;
	}

	public static void SetRawTileFlag(ref int rawTile, tk2dTileFlags flag, bool setValue)
	{
		if (rawTile != -1)
		{
			rawTile = ((!setValue) ? (rawTile & (int)(~flag)) : (rawTile | (int)flag));
		}
	}

	public static void InvertRawTileFlag(ref int rawTile, tk2dTileFlags flag)
	{
		if (rawTile != -1)
		{
			bool flag2 = ((uint)rawTile & (uint)flag) == 0;
			rawTile = ((!flag2) ? (rawTile & (int)(~flag)) : (rawTile | (int)flag));
		}
	}

	public static Vector3 ApplySpriteVertexTileFlags(tk2dTileMap tileMap, tk2dSpriteDefinition spriteDef, Vector3 pos, bool flipH, bool flipV, bool rot90)
	{
		float num = tileMap.data.tileOrigin.x + 0.5f * tileMap.data.tileSize.x;
		float num2 = tileMap.data.tileOrigin.y + 0.5f * tileMap.data.tileSize.y;
		float num3 = pos.x - num;
		float num4 = pos.y - num2;
		if (rot90)
		{
			float num5 = num3;
			num3 = num4;
			num4 = 0f - num5;
		}
		if (flipH)
		{
			num3 *= -1f;
		}
		if (flipV)
		{
			num4 *= -1f;
		}
		pos.x = num + num3;
		pos.y = num2 + num4;
		return pos;
	}

	public static Vector2 ApplySpriteVertexTileFlags(tk2dTileMap tileMap, tk2dSpriteDefinition spriteDef, Vector2 pos, bool flipH, bool flipV, bool rot90)
	{
		float num = tileMap.data.tileOrigin.x + 0.5f * tileMap.data.tileSize.x;
		float num2 = tileMap.data.tileOrigin.y + 0.5f * tileMap.data.tileSize.y;
		float num3 = pos.x - num;
		float num4 = pos.y - num2;
		if (rot90)
		{
			float num5 = num3;
			num3 = num4;
			num4 = 0f - num5;
		}
		if (flipH)
		{
			num3 *= -1f;
		}
		if (flipV)
		{
			num4 *= -1f;
		}
		pos.x = num + num3;
		pos.y = num2 + num4;
		return pos;
	}
}
