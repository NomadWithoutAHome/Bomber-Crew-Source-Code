using tk2dRuntime;
using UnityEngine;

[AddComponentMenu("2D Toolkit/Sprite/tk2dSpriteFromTexture")]
[ExecuteInEditMode]
public class tk2dSpriteFromTexture : MonoBehaviour
{
	public Texture texture;

	public tk2dSpriteCollectionSize spriteCollectionSize = new tk2dSpriteCollectionSize();

	public tk2dBaseSprite.Anchor anchor = tk2dBaseSprite.Anchor.MiddleCenter;

	private tk2dSpriteCollectionData spriteCollection;

	private tk2dBaseSprite _sprite;

	private tk2dBaseSprite Sprite
	{
		get
		{
			if (_sprite == null)
			{
				_sprite = GetComponent<tk2dBaseSprite>();
				if (_sprite == null)
				{
					_sprite = base.gameObject.AddComponent<tk2dSprite>();
				}
			}
			return _sprite;
		}
	}

	public bool HasSpriteCollection => spriteCollection != null;

	private void Awake()
	{
		Create(spriteCollectionSize, texture, anchor);
	}

	private void OnDestroy()
	{
		DestroyInternal();
		Renderer component = GetComponent<Renderer>();
		if (component != null)
		{
			component.material = null;
		}
	}

	public void Create(tk2dSpriteCollectionSize spriteCollectionSize, Texture texture, tk2dBaseSprite.Anchor anchor)
	{
		DestroyInternal();
		if (texture != null)
		{
			this.spriteCollectionSize.CopyFrom(spriteCollectionSize);
			this.texture = texture;
			this.anchor = anchor;
			GameObject gameObject = new GameObject("tk2dSpriteFromTexture - " + texture.name);
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			gameObject.hideFlags = HideFlags.DontSave;
			Vector2 anchorOffset = tk2dSpriteGeomGen.GetAnchorOffset(anchor, texture.width, texture.height);
			spriteCollection = SpriteCollectionGenerator.CreateFromTexture(gameObject, texture, spriteCollectionSize, new Vector2(texture.width, texture.height), new string[1] { "unnamed" }, new Rect[1]
			{
				new Rect(0f, 0f, texture.width, texture.height)
			}, null, new Vector2[1] { anchorOffset }, new bool[1]);
			string spriteCollectionName = "SpriteFromTexture " + texture.name;
			spriteCollection.spriteCollectionName = spriteCollectionName;
			spriteCollection.spriteDefinitions[0].material.name = spriteCollectionName;
			spriteCollection.spriteDefinitions[0].material.hideFlags = HideFlags.DontSave | HideFlags.HideInInspector;
			Sprite.SetSprite(spriteCollection, 0);
		}
	}

	public void Clear()
	{
		DestroyInternal();
	}

	public void ForceBuild()
	{
		DestroyInternal();
		Create(spriteCollectionSize, texture, anchor);
	}

	private void DestroyInternal()
	{
		if (spriteCollection != null)
		{
			if (spriteCollection.spriteDefinitions[0].material != null)
			{
				Object.DestroyImmediate(spriteCollection.spriteDefinitions[0].material);
			}
			Object.DestroyImmediate(spriteCollection.gameObject);
			spriteCollection = null;
		}
	}
}
