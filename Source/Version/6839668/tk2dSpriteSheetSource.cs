using System;
using UnityEngine;

[Serializable]
public class tk2dSpriteSheetSource
{
	public enum Anchor
	{
		UpperLeft,
		UpperCenter,
		UpperRight,
		MiddleLeft,
		MiddleCenter,
		MiddleRight,
		LowerLeft,
		LowerCenter,
		LowerRight
	}

	public enum SplitMethod
	{
		UniformDivision
	}

	public Texture2D texture;

	public int tilesX;

	public int tilesY;

	public int numTiles;

	public Anchor anchor = Anchor.MiddleCenter;

	public tk2dSpriteCollectionDefinition.Pad pad;

	public Vector3 scale = new Vector3(1f, 1f, 1f);

	public bool additive;

	public bool active;

	public int tileWidth;

	public int tileHeight;

	public int tileMarginX;

	public int tileMarginY;

	public int tileSpacingX;

	public int tileSpacingY;

	public SplitMethod splitMethod;

	public int version;

	public const int CURRENT_VERSION = 1;

	public tk2dSpriteCollectionDefinition.ColliderType colliderType;

	public string Name => (!(texture != null)) ? "New Sprite Sheet" : texture.name;

	public void CopyFrom(tk2dSpriteSheetSource src)
	{
		texture = src.texture;
		tilesX = src.tilesX;
		tilesY = src.tilesY;
		numTiles = src.numTiles;
		anchor = src.anchor;
		pad = src.pad;
		scale = src.scale;
		colliderType = src.colliderType;
		version = src.version;
		active = src.active;
		tileWidth = src.tileWidth;
		tileHeight = src.tileHeight;
		tileSpacingX = src.tileSpacingX;
		tileSpacingY = src.tileSpacingY;
		tileMarginX = src.tileMarginX;
		tileMarginY = src.tileMarginY;
		splitMethod = src.splitMethod;
	}

	public bool CompareTo(tk2dSpriteSheetSource src)
	{
		if (texture != src.texture)
		{
			return false;
		}
		if (tilesX != src.tilesX)
		{
			return false;
		}
		if (tilesY != src.tilesY)
		{
			return false;
		}
		if (numTiles != src.numTiles)
		{
			return false;
		}
		if (anchor != src.anchor)
		{
			return false;
		}
		if (pad != src.pad)
		{
			return false;
		}
		if (scale != src.scale)
		{
			return false;
		}
		if (colliderType != src.colliderType)
		{
			return false;
		}
		if (version != src.version)
		{
			return false;
		}
		if (active != src.active)
		{
			return false;
		}
		if (tileWidth != src.tileWidth)
		{
			return false;
		}
		if (tileHeight != src.tileHeight)
		{
			return false;
		}
		if (tileSpacingX != src.tileSpacingX)
		{
			return false;
		}
		if (tileSpacingY != src.tileSpacingY)
		{
			return false;
		}
		if (tileMarginX != src.tileMarginX)
		{
			return false;
		}
		if (tileMarginY != src.tileMarginY)
		{
			return false;
		}
		if (splitMethod != src.splitMethod)
		{
			return false;
		}
		return true;
	}
}
