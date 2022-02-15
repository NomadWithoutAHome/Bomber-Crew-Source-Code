using System;
using System.Collections.Generic;
using UnityEngine;

namespace tk2dRuntime.TileMap;

[Serializable]
public class SpriteChunk
{
	private bool dirty;

	public int[] spriteIds;

	public GameObject gameObject;

	public Mesh mesh;

	public MeshCollider meshCollider;

	public Mesh colliderMesh;

	public List<EdgeCollider2D> edgeColliders = new List<EdgeCollider2D>();

	public bool Dirty
	{
		get
		{
			return dirty;
		}
		set
		{
			dirty = value;
		}
	}

	public bool IsEmpty => spriteIds.Length == 0;

	public bool HasGameData => gameObject != null || mesh != null || meshCollider != null || colliderMesh != null || edgeColliders.Count > 0;

	public SpriteChunk()
	{
		spriteIds = new int[0];
	}

	public void DestroyGameData(tk2dTileMap tileMap)
	{
		if (mesh != null)
		{
			tileMap.DestroyMesh(mesh);
		}
		if (gameObject != null)
		{
			tk2dUtil.DestroyImmediate(gameObject);
		}
		gameObject = null;
		mesh = null;
		DestroyColliderData(tileMap);
	}

	public void DestroyColliderData(tk2dTileMap tileMap)
	{
		if (colliderMesh != null)
		{
			tileMap.DestroyMesh(colliderMesh);
		}
		if (meshCollider != null && meshCollider.sharedMesh != null && meshCollider.sharedMesh != colliderMesh)
		{
			tileMap.DestroyMesh(meshCollider.sharedMesh);
		}
		if (meshCollider != null)
		{
			tk2dUtil.DestroyImmediate(meshCollider);
		}
		meshCollider = null;
		colliderMesh = null;
		if (edgeColliders.Count > 0)
		{
			for (int i = 0; i < edgeColliders.Count; i++)
			{
				tk2dUtil.DestroyImmediate(edgeColliders[i]);
			}
			edgeColliders.Clear();
		}
	}
}
