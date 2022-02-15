using System;
using UnityEngine;

[AddComponentMenu("2D Toolkit/Sprite/tk2dSpriteRadial")]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
public class tk2dRadialSprite : tk2dBaseSprite
{
	private Mesh mesh;

	private Vector2[] meshUvs;

	private Vector3[] meshVertices;

	private Color32[] meshColors;

	private Vector3[] meshNormals;

	private Vector4[] meshTangents;

	private int[] meshIndices;

	[SerializeField]
	protected bool _createBoxCollider;

	private Vector3 boundsCenter = Vector3.zero;

	private Vector3 boundsExtents = Vector3.zero;

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

	private new void Awake()
	{
		base.Awake();
		mesh = new Mesh();
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
			UnityEngine.Object.Destroy(mesh);
		}
	}

	protected new void SetColors(Color32[] dest)
	{
		tk2dSpriteGeomGen.SetSpriteColors(dest, 0, 16, _color, collectionInst.premultipliedAlpha);
	}

	protected void SetGeometry(Vector3[] vertices, Vector2[] uvs)
	{
		tk2dSpriteDefinition currentSprite = base.CurrentSprite;
		tk2dSpriteGeomGen.SetRadialSpriteGeom(meshVertices, meshUvs, meshNormals, meshTangents, 0, currentSprite, base.scale);
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
		meshIndices = new int[54];
		tk2dSpriteGeomGen.SetRadialSpriteindices(meshIndices, 0, 0, base.CurrentSprite);
	}

	private bool NearEnough(float value, float compValue, float scale)
	{
		float num = Mathf.Abs(value - compValue);
		return Mathf.Abs(num / scale) < 0.01f;
	}

	public override void Build()
	{
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
		if (GetComponent<Renderer>().sharedMaterial != collectionInst.spriteDefinitions[base.spriteId].materialInst)
		{
			GetComponent<Renderer>().material = collectionInst.spriteDefinitions[base.spriteId].materialInst;
		}
	}

	protected override int GetCurrentVertexCount()
	{
		return 16;
	}

	public void SetValue(float _value)
	{
		SetGeometry(meshVertices, meshUvs);
		float num = (float)Math.PI * 2f * _value;
		float num2 = Mathf.Min(Mathf.Abs(Mathf.Tan(num)), 1f) * Mathf.Sign(Mathf.Sin(num));
		float num3 = Mathf.Min(Mathf.Abs(Mathf.Tan(num + (float)Math.PI / 2f)), 1f) * Mathf.Sign(Mathf.Cos(num));
		float magnitude = (meshVertices[3] - meshVertices[0]).magnitude;
		float magnitude2 = (meshVertices[12] - meshVertices[0]).magnitude;
		Vector3 vector = default(Vector3);
		vector.x = meshVertices[5].x + num2 * magnitude * 0.5f;
		vector.y = meshVertices[5].y + num3 * magnitude2 * 0.5f;
		vector.z = meshVertices[5].z;
		if (_value <= 0.125f)
		{
			meshVertices[14] = vector;
			meshUvs[14] += (meshUvs[15] - meshUvs[14]) * num2;
		}
		else if (_value <= 0.25f)
		{
			meshVertices[15] = vector;
			ref Vector3 reference = ref meshVertices[14];
			reference = meshVertices[15];
			meshUvs[15] += (meshUvs[11] - meshUvs[15]) * (1f - num3);
			ref Vector2 reference2 = ref meshUvs[14];
			reference2 = meshUvs[15];
		}
		else if (_value <= 0.375f)
		{
			ref Vector3 reference3 = ref meshVertices[14];
			ref Vector3 reference4 = ref meshVertices[15];
			reference3 = (reference4 = meshVertices[11]);
			meshVertices[7] = vector;
			ref Vector2 reference5 = ref meshUvs[14];
			ref Vector2 reference6 = ref meshUvs[15];
			reference5 = (reference6 = meshUvs[11]);
			meshUvs[7] += (meshUvs[3] - meshUvs[7]) * Mathf.Abs(num3);
		}
		else if (_value <= 0.5f)
		{
			ref Vector3 reference7 = ref meshVertices[14];
			ref Vector3 reference8 = ref meshVertices[15];
			reference7 = (reference8 = meshVertices[11]);
			meshVertices[3] = vector;
			ref Vector3 reference9 = ref meshVertices[7];
			reference9 = meshVertices[3];
			ref Vector2 reference10 = ref meshUvs[14];
			ref Vector2 reference11 = ref meshUvs[15];
			reference10 = (reference11 = meshUvs[11]);
			meshUvs[3] += (meshUvs[2] - meshUvs[3]) * (1f - num2);
			ref Vector2 reference12 = ref meshUvs[7];
			reference12 = meshUvs[3];
		}
		else if (_value <= 0.625f)
		{
			ref Vector3 reference13 = ref meshVertices[14];
			ref Vector3 reference14 = ref meshVertices[15];
			reference13 = (reference14 = meshVertices[11]);
			ref Vector3 reference15 = ref meshVertices[7];
			ref Vector3 reference16 = ref meshVertices[3];
			reference15 = (reference16 = meshVertices[2]);
			meshVertices[1] = vector;
			ref Vector2 reference17 = ref meshUvs[14];
			ref Vector2 reference18 = ref meshUvs[15];
			reference17 = (reference18 = meshUvs[11]);
			ref Vector2 reference19 = ref meshUvs[7];
			ref Vector2 reference20 = ref meshUvs[3];
			reference19 = (reference20 = meshUvs[2]);
			meshUvs[1] += (meshUvs[0] - meshUvs[1]) * Mathf.Abs(num2);
		}
		else if (_value <= 0.75f)
		{
			ref Vector3 reference21 = ref meshVertices[14];
			ref Vector3 reference22 = ref meshVertices[15];
			reference21 = (reference22 = meshVertices[11]);
			ref Vector3 reference23 = ref meshVertices[7];
			ref Vector3 reference24 = ref meshVertices[3];
			reference23 = (reference24 = meshVertices[2]);
			meshVertices[0] = vector;
			ref Vector3 reference25 = ref meshVertices[1];
			reference25 = meshVertices[0];
			ref Vector2 reference26 = ref meshUvs[14];
			ref Vector2 reference27 = ref meshUvs[15];
			reference26 = (reference27 = meshUvs[11]);
			ref Vector2 reference28 = ref meshUvs[7];
			ref Vector2 reference29 = ref meshUvs[3];
			reference28 = (reference29 = meshUvs[2]);
			meshUvs[0] += (meshUvs[4] - meshUvs[0]) * (1f - Mathf.Abs(num3));
			ref Vector2 reference30 = ref meshUvs[1];
			reference30 = meshUvs[0];
		}
		else if (_value <= 0.875f)
		{
			ref Vector3 reference31 = ref meshVertices[14];
			ref Vector3 reference32 = ref meshVertices[15];
			reference31 = (reference32 = meshVertices[11]);
			ref Vector3 reference33 = ref meshVertices[7];
			ref Vector3 reference34 = ref meshVertices[3];
			reference33 = (reference34 = meshVertices[2]);
			ref Vector3 reference35 = ref meshVertices[1];
			ref Vector3 reference36 = ref meshVertices[0];
			reference35 = (reference36 = meshVertices[4]);
			meshVertices[8] = vector;
			ref Vector2 reference37 = ref meshUvs[14];
			ref Vector2 reference38 = ref meshUvs[15];
			reference37 = (reference38 = meshUvs[11]);
			ref Vector2 reference39 = ref meshUvs[7];
			ref Vector2 reference40 = ref meshUvs[3];
			reference39 = (reference40 = meshUvs[2]);
			ref Vector2 reference41 = ref meshUvs[1];
			ref Vector2 reference42 = ref meshUvs[0];
			reference41 = (reference42 = meshUvs[4]);
			meshUvs[8] += (meshUvs[12] - meshUvs[8]) * num3;
		}
		else
		{
			ref Vector3 reference43 = ref meshVertices[14];
			ref Vector3 reference44 = ref meshVertices[15];
			reference43 = (reference44 = meshVertices[11]);
			ref Vector3 reference45 = ref meshVertices[7];
			ref Vector3 reference46 = ref meshVertices[3];
			reference45 = (reference46 = meshVertices[2]);
			ref Vector3 reference47 = ref meshVertices[1];
			ref Vector3 reference48 = ref meshVertices[0];
			reference47 = (reference48 = meshVertices[4]);
			meshVertices[12] = vector;
			ref Vector3 reference49 = ref meshVertices[8];
			reference49 = meshVertices[12];
			ref Vector2 reference50 = ref meshUvs[14];
			ref Vector2 reference51 = ref meshUvs[15];
			reference50 = (reference51 = meshUvs[11]);
			ref Vector2 reference52 = ref meshUvs[7];
			ref Vector2 reference53 = ref meshUvs[3];
			reference52 = (reference53 = meshUvs[2]);
			ref Vector2 reference54 = ref meshUvs[1];
			ref Vector2 reference55 = ref meshUvs[0];
			reference54 = (reference55 = meshUvs[4]);
			meshUvs[12] += (meshUvs[13] - meshUvs[12]) * (1f - Mathf.Abs(num2));
			ref Vector2 reference56 = ref meshUvs[8];
			reference56 = meshUvs[12];
		}
		UpdateGeometry();
	}
}
