using UnityEngine;

[AddComponentMenu("2D Toolkit/Sprite/tk2dSlicedSprite")]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
public class tk2dSlicedSprite : tk2dBaseSprite
{
	private Mesh mesh;

	private Vector2[] meshUvs;

	private Vector3[] meshVertices;

	private Color32[] meshColors;

	private Vector3[] meshNormals;

	private Vector4[] meshTangents;

	private int[] meshIndices;

	[SerializeField]
	private Vector2 _dimensions = new Vector2(50f, 50f);

	[SerializeField]
	private Anchor _anchor;

	[SerializeField]
	private bool _borderOnly;

	[SerializeField]
	private bool legacyMode;

	public float borderTop = 0.2f;

	public float borderBottom = 0.2f;

	public float borderLeft = 0.2f;

	public float borderRight = 0.2f;

	[SerializeField]
	protected bool _createBoxCollider;

	private Vector3 boundsCenter = Vector3.zero;

	private Vector3 boundsExtents = Vector3.zero;

	public bool BorderOnly
	{
		get
		{
			return _borderOnly;
		}
		set
		{
			if (value != _borderOnly)
			{
				_borderOnly = value;
				UpdateIndices();
			}
		}
	}

	public Vector2 dimensions
	{
		get
		{
			return _dimensions;
		}
		set
		{
			if (value != _dimensions)
			{
				_dimensions = value;
				UpdateVertices();
				UpdateCollider();
			}
		}
	}

	public Anchor anchor
	{
		get
		{
			return _anchor;
		}
		set
		{
			if (value != _anchor)
			{
				_anchor = value;
				UpdateVertices();
				UpdateCollider();
			}
		}
	}

	public bool CreateBoxCollider
	{
		get
		{
			return _createBoxCollider;
		}
		set
		{
			if (_createBoxCollider != value)
			{
				_createBoxCollider = value;
				UpdateCollider();
			}
		}
	}

	public void SetBorder(float left, float bottom, float right, float top)
	{
		if (borderLeft != left || borderBottom != bottom || borderRight != right || borderTop != top)
		{
			borderLeft = left;
			borderBottom = bottom;
			borderRight = right;
			borderTop = top;
			UpdateVertices();
		}
	}

	private new void Awake()
	{
		base.Awake();
		mesh = new Mesh();
		mesh.MarkDynamic();
		mesh.hideFlags = HideFlags.DontSave;
		GetComponent<MeshFilter>().mesh = mesh;
		if (boxCollider == null)
		{
			boxCollider = GetComponent<BoxCollider>();
		}
		if (boxCollider2D == null)
		{
			boxCollider2D = GetComponent<BoxCollider2D>();
		}
		if ((bool)base.Collection)
		{
			if (_spriteId < 0 || _spriteId >= base.Collection.Count)
			{
				_spriteId = 0;
			}
			Build();
		}
	}

	protected void OnDestroy()
	{
		if ((bool)mesh)
		{
			Object.Destroy(mesh);
		}
	}

	protected new void SetColors(Color32[] dest)
	{
		tk2dSpriteGeomGen.SetSpriteColors(dest, 0, 16, _color, collectionInst.premultipliedAlpha);
	}

	protected void SetGeometry(Vector3[] vertices, Vector2[] uvs)
	{
		tk2dSpriteDefinition currentSprite = base.CurrentSprite;
		float colliderOffsetZ = ((!(boxCollider != null)) ? 0f : boxCollider.center.z);
		float colliderExtentZ = ((!(boxCollider != null)) ? 0.5f : (boxCollider.size.z * 0.5f));
		tk2dSpriteGeomGen.SetSlicedSpriteGeom(meshVertices, meshUvs, 0, out boundsCenter, out boundsExtents, currentSprite, _scale, dimensions, new Vector2(borderLeft, borderBottom), new Vector2(borderRight, borderTop), anchor, colliderOffsetZ, colliderExtentZ);
		if (meshNormals.Length > 0 || meshTangents.Length > 0)
		{
			tk2dSpriteGeomGen.SetSpriteVertexNormals(meshVertices, meshVertices[0], meshVertices[15], currentSprite.normals, currentSprite.tangents, meshNormals, meshTangents);
		}
		if (currentSprite.positions.Length != 4 || currentSprite.complexGeometry)
		{
			for (int i = 0; i < vertices.Length; i++)
			{
				ref Vector3 reference = ref vertices[i];
				reference = Vector3.zero;
			}
		}
	}

	private void SetIndices()
	{
		int num = ((!_borderOnly) ? 54 : 48);
		meshIndices = new int[num];
		tk2dSpriteGeomGen.SetSlicedSpriteIndices(meshIndices, 0, 0, base.CurrentSprite, _borderOnly);
	}

	private bool NearEnough(float value, float compValue, float scale)
	{
		float num = Mathf.Abs(value - compValue);
		return Mathf.Abs(num / scale) < 0.01f;
	}

	private void PermanentUpgradeLegacyMode()
	{
		tk2dSpriteDefinition currentSprite = base.CurrentSprite;
		float x = currentSprite.untrimmedBoundsData[0].x;
		float y = currentSprite.untrimmedBoundsData[0].y;
		float x2 = currentSprite.untrimmedBoundsData[1].x;
		float y2 = currentSprite.untrimmedBoundsData[1].y;
		if (NearEnough(x, 0f, x2) && NearEnough(y, (0f - y2) / 2f, y2))
		{
			_anchor = Anchor.UpperCenter;
		}
		else if (NearEnough(x, 0f, x2) && NearEnough(y, 0f, y2))
		{
			_anchor = Anchor.MiddleCenter;
		}
		else if (NearEnough(x, 0f, x2) && NearEnough(y, y2 / 2f, y2))
		{
			_anchor = Anchor.LowerCenter;
		}
		else if (NearEnough(x, (0f - x2) / 2f, x2) && NearEnough(y, (0f - y2) / 2f, y2))
		{
			_anchor = Anchor.UpperRight;
		}
		else if (NearEnough(x, (0f - x2) / 2f, x2) && NearEnough(y, 0f, y2))
		{
			_anchor = Anchor.MiddleRight;
		}
		else if (NearEnough(x, (0f - x2) / 2f, x2) && NearEnough(y, y2 / 2f, y2))
		{
			_anchor = Anchor.LowerRight;
		}
		else if (NearEnough(x, x2 / 2f, x2) && NearEnough(y, (0f - y2) / 2f, y2))
		{
			_anchor = Anchor.UpperLeft;
		}
		else if (NearEnough(x, x2 / 2f, x2) && NearEnough(y, 0f, y2))
		{
			_anchor = Anchor.MiddleLeft;
		}
		else if (NearEnough(x, x2 / 2f, x2) && NearEnough(y, y2 / 2f, y2))
		{
			_anchor = Anchor.LowerLeft;
		}
		else
		{
			DebugLogWrapper.LogError("tk2dSlicedSprite (" + base.name + ") error - Unable to determine anchor upgrading from legacy mode. Please fix this manually.");
			_anchor = Anchor.MiddleCenter;
		}
		float num = x2 / currentSprite.texelSize.x;
		float num2 = y2 / currentSprite.texelSize.y;
		_dimensions.x = _scale.x * num;
		_dimensions.y = _scale.y * num2;
		_scale.Set(1f, 1f, 1f);
		legacyMode = false;
	}

	public override void Build()
	{
		if (legacyMode)
		{
			PermanentUpgradeLegacyMode();
		}
		tk2dSpriteDefinition currentSprite = base.CurrentSprite;
		meshUvs = new Vector2[16];
		meshVertices = new Vector3[16];
		meshColors = new Color32[16];
		meshNormals = new Vector3[0];
		meshTangents = new Vector4[0];
		if (currentSprite.normals != null && currentSprite.normals.Length > 0)
		{
			meshNormals = new Vector3[16];
		}
		if (currentSprite.tangents != null && currentSprite.tangents.Length > 0)
		{
			meshTangents = new Vector4[16];
		}
		SetIndices();
		SetGeometry(meshVertices, meshUvs);
		SetColors(meshColors);
		if (mesh == null)
		{
			mesh = new Mesh();
			mesh.MarkDynamic();
			mesh.hideFlags = HideFlags.DontSave;
		}
		else
		{
			mesh.Clear();
		}
		mesh.vertices = meshVertices;
		mesh.colors32 = meshColors;
		mesh.uv = meshUvs;
		mesh.normals = meshNormals;
		mesh.tangents = meshTangents;
		mesh.triangles = meshIndices;
		mesh.RecalculateBounds();
		mesh.bounds = tk2dBaseSprite.AdjustedMeshBounds(mesh.bounds, renderLayer);
		GetComponent<MeshFilter>().mesh = mesh;
		UpdateCollider();
		UpdateMaterial();
	}

	protected override void UpdateGeometry()
	{
		UpdateGeometryImpl();
	}

	protected override void UpdateColors()
	{
		UpdateColorsImpl();
	}

	protected override void UpdateVertices()
	{
		UpdateGeometryImpl();
	}

	private void UpdateIndices()
	{
		if (mesh != null)
		{
			SetIndices();
			mesh.triangles = meshIndices;
		}
	}

	protected void UpdateColorsImpl()
	{
		if (meshColors == null || meshColors.Length == 0)
		{
			Build();
			return;
		}
		SetColors(meshColors);
		mesh.colors32 = meshColors;
	}

	protected void UpdateGeometryImpl()
	{
		if (meshVertices == null || meshVertices.Length == 0)
		{
			Build();
			return;
		}
		SetGeometry(meshVertices, meshUvs);
		mesh.vertices = meshVertices;
		mesh.uv = meshUvs;
		mesh.normals = meshNormals;
		mesh.tangents = meshTangents;
		mesh.RecalculateBounds();
		mesh.bounds = tk2dBaseSprite.AdjustedMeshBounds(mesh.bounds, renderLayer);
		UpdateCollider();
	}

	protected override void UpdateCollider()
	{
		if (!CreateBoxCollider)
		{
			return;
		}
		if (base.CurrentSprite.physicsEngine == tk2dSpriteDefinition.PhysicsEngine.Physics3D)
		{
			if (boxCollider != null)
			{
				boxCollider.size = 2f * boundsExtents;
				boxCollider.center = boundsCenter;
			}
		}
		else if (base.CurrentSprite.physicsEngine == tk2dSpriteDefinition.PhysicsEngine.Physics2D && boxCollider2D != null)
		{
			boxCollider2D.size = 2f * boundsExtents;
			boxCollider2D.offset = boundsCenter;
		}
	}

	protected override void CreateCollider()
	{
		UpdateCollider();
	}

	protected override void UpdateMaterial()
	{
		Renderer component = GetComponent<Renderer>();
		if (component.sharedMaterial != collectionInst.spriteDefinitions[base.spriteId].materialInst)
		{
			component.material = collectionInst.spriteDefinitions[base.spriteId].materialInst;
		}
	}

	protected override int GetCurrentVertexCount()
	{
		return 16;
	}

	public override void ReshapeBounds(Vector3 dMin, Vector3 dMax)
	{
		float num = 0.1f;
		tk2dSpriteDefinition currentSprite = base.CurrentSprite;
		Vector2 vector = new Vector2(_dimensions.x * currentSprite.texelSize.x, _dimensions.y * currentSprite.texelSize.y);
		Vector3 vector2 = new Vector3(vector.x * _scale.x, vector.y * _scale.y);
		Vector3 zero = Vector3.zero;
		switch (_anchor)
		{
		case Anchor.LowerLeft:
			zero.Set(0f, 0f, 0f);
			break;
		case Anchor.LowerCenter:
			zero.Set(0.5f, 0f, 0f);
			break;
		case Anchor.LowerRight:
			zero.Set(1f, 0f, 0f);
			break;
		case Anchor.MiddleLeft:
			zero.Set(0f, 0.5f, 0f);
			break;
		case Anchor.MiddleCenter:
			zero.Set(0.5f, 0.5f, 0f);
			break;
		case Anchor.MiddleRight:
			zero.Set(1f, 0.5f, 0f);
			break;
		case Anchor.UpperLeft:
			zero.Set(0f, 1f, 0f);
			break;
		case Anchor.UpperCenter:
			zero.Set(0.5f, 1f, 0f);
			break;
		case Anchor.UpperRight:
			zero.Set(1f, 1f, 0f);
			break;
		}
		zero = Vector3.Scale(zero, vector2) * -1f;
		Vector3 vector3 = vector2 + dMax - dMin;
		vector3.x /= vector.x;
		vector3.y /= vector.y;
		if (Mathf.Abs(vector.x * vector3.x) < currentSprite.texelSize.x * num && Mathf.Abs(vector3.x) < Mathf.Abs(_scale.x))
		{
			dMin.x = 0f;
			vector3.x = _scale.x;
		}
		if (Mathf.Abs(vector.y * vector3.y) < currentSprite.texelSize.y * num && Mathf.Abs(vector3.y) < Mathf.Abs(_scale.y))
		{
			dMin.y = 0f;
			vector3.y = _scale.y;
		}
		Vector2 vector4 = new Vector3((!Mathf.Approximately(_scale.x, 0f)) ? (vector3.x / _scale.x) : 0f, (!Mathf.Approximately(_scale.y, 0f)) ? (vector3.y / _scale.y) : 0f);
		Vector3 vector5 = new Vector3(zero.x * vector4.x, zero.y * vector4.y);
		Vector3 position = dMin + zero - vector5;
		position.z = 0f;
		base.transform.position = base.transform.TransformPoint(position);
		dimensions = new Vector2(_dimensions.x * vector4.x, _dimensions.y * vector4.y);
	}
}
