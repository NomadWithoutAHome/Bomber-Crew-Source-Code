using System.Collections.Generic;
using tk2dRuntime;
using UnityEngine;

[AddComponentMenu("2D Toolkit/Sprite/tk2dStaticSpriteBatcher")]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
public class tk2dStaticSpriteBatcher : MonoBehaviour, ISpriteCollectionForceBuild
{
	public enum Flags
	{
		None = 0,
		GenerateCollider = 1,
		FlattenDepth = 2,
		SortToCamera = 4
	}

	public static int CURRENT_VERSION = 3;

	public int version;

	public tk2dBatchedSprite[] batchedSprites;

	public tk2dTextMeshData[] allTextMeshData;

	public tk2dSpriteCollectionData spriteCollection;

	[SerializeField]
	private Flags flags = Flags.GenerateCollider;

	private Mesh mesh;

	private Mesh colliderMesh;

	[SerializeField]
	private Vector3 _scale = new Vector3(1f, 1f, 1f);

	public bool CheckFlag(Flags mask)
	{
		return (flags & mask) != 0;
	}

	public void SetFlag(Flags mask, bool value)
	{
		if (CheckFlag(mask) != value)
		{
			if (value)
			{
				flags |= mask;
			}
			else
			{
				flags &= ~mask;
			}
			Build();
		}
	}

	private void Awake()
	{
		Build();
	}

	private bool UpgradeData()
	{
		if (version == CURRENT_VERSION)
		{
			return false;
		}
		if (_scale == Vector3.zero)
		{
			_scale = Vector3.one;
		}
		if (version < 2 && batchedSprites != null)
		{
			tk2dBatchedSprite[] array = batchedSprites;
			foreach (tk2dBatchedSprite tk2dBatchedSprite2 in array)
			{
				tk2dBatchedSprite2.parentId = -1;
			}
		}
		if (version < 3)
		{
			if (batchedSprites != null)
			{
				tk2dBatchedSprite[] array2 = batchedSprites;
				foreach (tk2dBatchedSprite tk2dBatchedSprite3 in array2)
				{
					if (tk2dBatchedSprite3.spriteId == -1)
					{
						tk2dBatchedSprite3.type = tk2dBatchedSprite.Type.EmptyGameObject;
						continue;
					}
					tk2dBatchedSprite3.type = tk2dBatchedSprite.Type.Sprite;
					if (tk2dBatchedSprite3.spriteCollection == null)
					{
						tk2dBatchedSprite3.spriteCollection = spriteCollection;
					}
				}
				UpdateMatrices();
			}
			spriteCollection = null;
		}
		version = CURRENT_VERSION;
		return true;
	}

	protected void OnDestroy()
	{
		if ((bool)mesh)
		{
			Object.Destroy(mesh);
		}
		if ((bool)colliderMesh)
		{
			Object.Destroy(colliderMesh);
		}
	}

	public void UpdateMatrices()
	{
		bool flag = false;
		tk2dBatchedSprite[] array = batchedSprites;
		foreach (tk2dBatchedSprite tk2dBatchedSprite2 in array)
		{
			if (tk2dBatchedSprite2.parentId != -1)
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			Matrix4x4 matrix4x = default(Matrix4x4);
			List<tk2dBatchedSprite> list = new List<tk2dBatchedSprite>(batchedSprites);
			list.Sort((tk2dBatchedSprite a, tk2dBatchedSprite b) => a.parentId.CompareTo(b.parentId));
			{
				foreach (tk2dBatchedSprite item in list)
				{
					matrix4x.SetTRS(item.position, item.rotation, item.localScale);
					item.relativeMatrix = ((item.parentId != -1) ? batchedSprites[item.parentId].relativeMatrix : Matrix4x4.identity) * matrix4x;
				}
				return;
			}
		}
		tk2dBatchedSprite[] array2 = batchedSprites;
		foreach (tk2dBatchedSprite tk2dBatchedSprite3 in array2)
		{
			tk2dBatchedSprite3.relativeMatrix.SetTRS(tk2dBatchedSprite3.position, tk2dBatchedSprite3.rotation, tk2dBatchedSprite3.localScale);
		}
	}

	public void Build()
	{
		UpgradeData();
		if (mesh == null)
		{
			mesh = new Mesh();
			mesh.hideFlags = HideFlags.DontSave;
			GetComponent<MeshFilter>().mesh = mesh;
		}
		else
		{
			mesh.Clear();
		}
		if ((bool)colliderMesh)
		{
			Object.Destroy(colliderMesh);
			colliderMesh = null;
		}
		if (batchedSprites != null && batchedSprites.Length != 0)
		{
			SortBatchedSprites();
			BuildRenderMesh();
			BuildPhysicsMesh();
		}
	}

	private void SortBatchedSprites()
	{
		List<tk2dBatchedSprite> list = new List<tk2dBatchedSprite>();
		List<tk2dBatchedSprite> list2 = new List<tk2dBatchedSprite>();
		List<tk2dBatchedSprite> list3 = new List<tk2dBatchedSprite>();
		tk2dBatchedSprite[] array = batchedSprites;
		foreach (tk2dBatchedSprite tk2dBatchedSprite2 in array)
		{
			if (!tk2dBatchedSprite2.IsDrawn)
			{
				list3.Add(tk2dBatchedSprite2);
				continue;
			}
			Material material = GetMaterial(tk2dBatchedSprite2);
			if (material != null)
			{
				if (material.renderQueue == 2000)
				{
					list.Add(tk2dBatchedSprite2);
				}
				else
				{
					list2.Add(tk2dBatchedSprite2);
				}
			}
			else
			{
				list.Add(tk2dBatchedSprite2);
			}
		}
		List<tk2dBatchedSprite> list4 = new List<tk2dBatchedSprite>(list.Count + list2.Count + list3.Count);
		list4.AddRange(list);
		list4.AddRange(list2);
		list4.AddRange(list3);
		Dictionary<tk2dBatchedSprite, int> dictionary = new Dictionary<tk2dBatchedSprite, int>();
		int num = 0;
		foreach (tk2dBatchedSprite item in list4)
		{
			dictionary[item] = num++;
		}
		foreach (tk2dBatchedSprite item2 in list4)
		{
			if (item2.parentId != -1)
			{
				item2.parentId = dictionary[batchedSprites[item2.parentId]];
			}
		}
		batchedSprites = list4.ToArray();
	}

	private Material GetMaterial(tk2dBatchedSprite bs)
	{
		return bs.type switch
		{
			tk2dBatchedSprite.Type.EmptyGameObject => null, 
			tk2dBatchedSprite.Type.TextMesh => allTextMeshData[bs.xRefId].font.materialInst, 
			_ => bs.GetSpriteDefinition().materialInst, 
		};
	}

	private void BuildRenderMesh()
	{
		List<Material> list = new List<Material>();
		List<List<int>> list2 = new List<List<int>>();
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = CheckFlag(Flags.FlattenDepth);
		tk2dBatchedSprite[] array = batchedSprites;
		foreach (tk2dBatchedSprite tk2dBatchedSprite2 in array)
		{
			tk2dSpriteDefinition spriteDefinition = tk2dBatchedSprite2.GetSpriteDefinition();
			if (spriteDefinition != null)
			{
				flag |= spriteDefinition.normals != null && spriteDefinition.normals.Length > 0;
				flag2 |= spriteDefinition.tangents != null && spriteDefinition.tangents.Length > 0;
			}
			if (tk2dBatchedSprite2.type == tk2dBatchedSprite.Type.TextMesh)
			{
				tk2dTextMeshData tk2dTextMeshData2 = allTextMeshData[tk2dBatchedSprite2.xRefId];
				if (tk2dTextMeshData2.font != null && tk2dTextMeshData2.font.inst.textureGradients)
				{
					flag3 = true;
				}
			}
		}
		List<int> list3 = new List<int>();
		List<int> list4 = new List<int>();
		int num = 0;
		tk2dBatchedSprite[] array2 = batchedSprites;
		foreach (tk2dBatchedSprite tk2dBatchedSprite3 in array2)
		{
			if (!tk2dBatchedSprite3.IsDrawn)
			{
				break;
			}
			tk2dSpriteDefinition spriteDefinition2 = tk2dBatchedSprite3.GetSpriteDefinition();
			int numVertices = 0;
			int numIndices = 0;
			switch (tk2dBatchedSprite3.type)
			{
			case tk2dBatchedSprite.Type.Sprite:
				if (spriteDefinition2 != null)
				{
					tk2dSpriteGeomGen.GetSpriteGeomDesc(out numVertices, out numIndices, spriteDefinition2);
				}
				break;
			case tk2dBatchedSprite.Type.TiledSprite:
				if (spriteDefinition2 != null)
				{
					tk2dSpriteGeomGen.GetTiledSpriteGeomDesc(out numVertices, out numIndices, spriteDefinition2, tk2dBatchedSprite3.Dimensions);
				}
				break;
			case tk2dBatchedSprite.Type.SlicedSprite:
				if (spriteDefinition2 != null)
				{
					tk2dSpriteGeomGen.GetSlicedSpriteGeomDesc(out numVertices, out numIndices, spriteDefinition2, tk2dBatchedSprite3.CheckFlag(tk2dBatchedSprite.Flags.SlicedSprite_BorderOnly));
				}
				break;
			case tk2dBatchedSprite.Type.ClippedSprite:
				if (spriteDefinition2 != null)
				{
					tk2dSpriteGeomGen.GetClippedSpriteGeomDesc(out numVertices, out numIndices, spriteDefinition2);
				}
				break;
			case tk2dBatchedSprite.Type.TextMesh:
			{
				tk2dTextMeshData tk2dTextMeshData3 = allTextMeshData[tk2dBatchedSprite3.xRefId];
				tk2dTextGeomGen.GetTextMeshGeomDesc(out numVertices, out numIndices, tk2dTextGeomGen.Data(tk2dTextMeshData3, tk2dTextMeshData3.font.inst, tk2dBatchedSprite3.FormattedText));
				break;
			}
			}
			num += numVertices;
			list3.Add(numVertices);
			list4.Add(numIndices);
		}
		Vector3[] array3 = ((!flag) ? null : new Vector3[num]);
		Vector4[] array4 = ((!flag2) ? null : new Vector4[num]);
		Vector3[] array5 = new Vector3[num];
		Color32[] array6 = new Color32[num];
		Vector2[] array7 = new Vector2[num];
		Vector2[] array8 = ((!flag3) ? null : new Vector2[num]);
		int num2 = 0;
		Material material = null;
		List<int> list5 = null;
		Matrix4x4 identity = Matrix4x4.identity;
		identity.m00 = _scale.x;
		identity.m11 = _scale.y;
		identity.m22 = _scale.z;
		int num3 = 0;
		tk2dBatchedSprite[] array9 = batchedSprites;
		foreach (tk2dBatchedSprite tk2dBatchedSprite4 in array9)
		{
			if (!tk2dBatchedSprite4.IsDrawn)
			{
				break;
			}
			if (tk2dBatchedSprite4.type == tk2dBatchedSprite.Type.EmptyGameObject)
			{
				num3++;
				continue;
			}
			tk2dSpriteDefinition spriteDefinition3 = tk2dBatchedSprite4.GetSpriteDefinition();
			int num4 = list3[num3];
			int num5 = list4[num3];
			Material material2 = GetMaterial(tk2dBatchedSprite4);
			if (material2 != material)
			{
				if (material != null)
				{
					list.Add(material);
					list2.Add(list5);
				}
				material = material2;
				list5 = new List<int>();
			}
			Vector3[] array10 = new Vector3[num4];
			Vector2[] array11 = new Vector2[num4];
			Vector2[] array12 = ((!flag3) ? null : new Vector2[num4]);
			Color32[] array13 = new Color32[num4];
			Vector3[] array14 = ((!flag) ? null : new Vector3[num4]);
			Vector4[] array15 = ((!flag2) ? null : new Vector4[num4]);
			int[] array16 = new int[num5];
			Vector3 boundsCenter = Vector3.zero;
			Vector3 boundsExtents = Vector3.zero;
			switch (tk2dBatchedSprite4.type)
			{
			case tk2dBatchedSprite.Type.Sprite:
				if (spriteDefinition3 != null)
				{
					tk2dSpriteGeomGen.SetSpriteGeom(array10, array11, array12, array14, array15, 0, spriteDefinition3, Vector3.one);
					flag3 = true;
					tk2dSpriteGeomGen.SetSpriteIndices(array16, 0, num2, spriteDefinition3);
				}
				break;
			case tk2dBatchedSprite.Type.TiledSprite:
				if (spriteDefinition3 != null)
				{
					tk2dSpriteGeomGen.SetTiledSpriteGeom(array10, array11, 0, out boundsCenter, out boundsExtents, spriteDefinition3, Vector3.one, tk2dBatchedSprite4.Dimensions, tk2dBatchedSprite4.anchor, tk2dBatchedSprite4.BoxColliderOffsetZ, tk2dBatchedSprite4.BoxColliderExtentZ);
					tk2dSpriteGeomGen.SetTiledSpriteIndices(array16, 0, num2, spriteDefinition3, tk2dBatchedSprite4.Dimensions);
				}
				break;
			case tk2dBatchedSprite.Type.SlicedSprite:
				if (spriteDefinition3 != null)
				{
					tk2dSpriteGeomGen.SetSlicedSpriteGeom(array10, array11, 0, out boundsCenter, out boundsExtents, spriteDefinition3, Vector3.one, tk2dBatchedSprite4.Dimensions, tk2dBatchedSprite4.SlicedSpriteBorderBottomLeft, tk2dBatchedSprite4.SlicedSpriteBorderTopRight, tk2dBatchedSprite4.anchor, tk2dBatchedSprite4.BoxColliderOffsetZ, tk2dBatchedSprite4.BoxColliderExtentZ);
					tk2dSpriteGeomGen.SetSlicedSpriteIndices(array16, 0, num2, spriteDefinition3, tk2dBatchedSprite4.CheckFlag(tk2dBatchedSprite.Flags.SlicedSprite_BorderOnly));
				}
				break;
			case tk2dBatchedSprite.Type.ClippedSprite:
				if (spriteDefinition3 != null)
				{
					tk2dSpriteGeomGen.SetClippedSpriteGeom(array10, array11, 0, out boundsCenter, out boundsExtents, spriteDefinition3, Vector3.one, tk2dBatchedSprite4.ClippedSpriteRegionBottomLeft, tk2dBatchedSprite4.ClippedSpriteRegionTopRight, tk2dBatchedSprite4.BoxColliderOffsetZ, tk2dBatchedSprite4.BoxColliderExtentZ);
					tk2dSpriteGeomGen.SetClippedSpriteIndices(array16, 0, num2, spriteDefinition3);
				}
				break;
			case tk2dBatchedSprite.Type.TextMesh:
			{
				tk2dTextMeshData tk2dTextMeshData4 = allTextMeshData[tk2dBatchedSprite4.xRefId];
				tk2dTextGeomGen.GeomData geomData = tk2dTextGeomGen.Data(tk2dTextMeshData4, tk2dTextMeshData4.font.inst, tk2dBatchedSprite4.FormattedText);
				int target = tk2dTextGeomGen.SetTextMeshGeom(array10, array11, array12, array13, 0, geomData);
				if (!geomData.fontInst.isPacked)
				{
					Color32 color = tk2dTextMeshData4.color;
					Color32 color2 = ((!tk2dTextMeshData4.useGradient) ? tk2dTextMeshData4.color : tk2dTextMeshData4.color2);
					for (int l = 0; l < array13.Length; l++)
					{
						Color32 color3 = ((l % 4 >= 2) ? color2 : color);
						byte b = (byte)(array13[l].r * color3.r / 255);
						byte b2 = (byte)(array13[l].g * color3.g / 255);
						byte b3 = (byte)(array13[l].b * color3.b / 255);
						byte b4 = (byte)(array13[l].a * color3.a / 255);
						if (geomData.fontInst.premultipliedAlpha)
						{
							b = (byte)(b * b4 / 255);
							b2 = (byte)(b2 * b4 / 255);
							b3 = (byte)(b3 * b4 / 255);
						}
						ref Color32 reference = ref array13[l];
						reference = new Color32(b, b2, b3, b4);
					}
				}
				tk2dTextGeomGen.SetTextMeshIndices(array16, 0, num2, geomData, target);
				break;
			}
			}
			tk2dBatchedSprite4.CachedBoundsCenter = boundsCenter;
			tk2dBatchedSprite4.CachedBoundsExtents = boundsExtents;
			if (num4 > 0 && tk2dBatchedSprite4.type != tk2dBatchedSprite.Type.TextMesh)
			{
				bool premulAlpha = tk2dBatchedSprite4.spriteCollection != null && tk2dBatchedSprite4.spriteCollection.premultipliedAlpha;
				tk2dSpriteGeomGen.SetSpriteColors(array13, 0, num4, tk2dBatchedSprite4.color, premulAlpha);
			}
			Matrix4x4 matrix4x = identity * tk2dBatchedSprite4.relativeMatrix;
			for (int m = 0; m < num4; m++)
			{
				Vector3 v = Vector3.Scale(array10[m], tk2dBatchedSprite4.baseScale);
				v = matrix4x.MultiplyPoint(v);
				if (flag4)
				{
					v.z = 0f;
				}
				array5[num2 + m] = v;
				ref Vector2 reference2 = ref array7[num2 + m];
				reference2 = array11[m];
				if (flag3)
				{
					ref Vector2 reference3 = ref array8[num2 + m];
					reference3 = array12[m];
				}
				ref Color32 reference4 = ref array6[num2 + m];
				reference4 = array13[m];
				if (flag)
				{
					ref Vector3 reference5 = ref array3[num2 + m];
					reference5 = tk2dBatchedSprite4.rotation * array14[m];
				}
				if (flag2)
				{
					Vector3 vector = new Vector3(array15[m].x, array15[m].y, array15[m].z);
					vector = tk2dBatchedSprite4.rotation * vector;
					ref Vector4 reference6 = ref array4[num2 + m];
					reference6 = new Vector4(vector.x, vector.y, vector.z, array15[m].w);
				}
			}
			list5.AddRange(array16);
			num2 += num4;
			num3++;
		}
		if (list5 != null)
		{
			list.Add(material);
			list2.Add(list5);
		}
		if ((bool)mesh)
		{
			mesh.vertices = array5;
			mesh.uv = array7;
			if (flag3)
			{
				mesh.uv2 = array8;
			}
			mesh.colors32 = array6;
			if (flag)
			{
				mesh.normals = array3;
			}
			if (flag2)
			{
				mesh.tangents = array4;
			}
			mesh.subMeshCount = list2.Count;
			for (int n = 0; n < list2.Count; n++)
			{
				mesh.SetTriangles(list2[n].ToArray(), n);
			}
			mesh.RecalculateBounds();
		}
		GetComponent<Renderer>().sharedMaterials = list.ToArray();
	}

	private void BuildPhysicsMesh()
	{
		MeshCollider component = GetComponent<MeshCollider>();
		if (component != null)
		{
			if (GetComponent<Collider>() != component)
			{
				return;
			}
			if (!CheckFlag(Flags.GenerateCollider))
			{
				Object.Destroy(component);
			}
		}
		EdgeCollider2D[] components = GetComponents<EdgeCollider2D>();
		if (!CheckFlag(Flags.GenerateCollider))
		{
			EdgeCollider2D[] array = components;
			foreach (EdgeCollider2D obj in array)
			{
				Object.Destroy(obj);
			}
		}
		if (!CheckFlag(Flags.GenerateCollider))
		{
			return;
		}
		bool flattenDepth = CheckFlag(Flags.FlattenDepth);
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		bool flag = true;
		tk2dBatchedSprite[] array2 = batchedSprites;
		foreach (tk2dBatchedSprite tk2dBatchedSprite2 in array2)
		{
			if (!tk2dBatchedSprite2.IsDrawn)
			{
				break;
			}
			tk2dSpriteDefinition spriteDefinition = tk2dBatchedSprite2.GetSpriteDefinition();
			bool flag2 = false;
			bool flag3 = false;
			switch (tk2dBatchedSprite2.type)
			{
			case tk2dBatchedSprite.Type.Sprite:
				if (spriteDefinition != null && spriteDefinition.colliderType == tk2dSpriteDefinition.ColliderType.Mesh)
				{
					flag2 = true;
				}
				if (spriteDefinition != null && spriteDefinition.colliderType == tk2dSpriteDefinition.ColliderType.Box)
				{
					flag3 = true;
				}
				break;
			case tk2dBatchedSprite.Type.TiledSprite:
			case tk2dBatchedSprite.Type.SlicedSprite:
			case tk2dBatchedSprite.Type.ClippedSprite:
				flag3 = tk2dBatchedSprite2.CheckFlag(tk2dBatchedSprite.Flags.Sprite_CreateBoxCollider);
				break;
			}
			if (flag2)
			{
				num += spriteDefinition.colliderIndicesFwd.Length;
				num2 += spriteDefinition.colliderVertices.Length;
				num3 += spriteDefinition.edgeCollider2D.Length;
				num3 += spriteDefinition.polygonCollider2D.Length;
			}
			else if (flag3)
			{
				num += 36;
				num2 += 8;
				num3++;
			}
			if (spriteDefinition.physicsEngine == tk2dSpriteDefinition.PhysicsEngine.Physics2D)
			{
				flag = false;
			}
		}
		if ((flag && num == 0) || (!flag && num3 == 0))
		{
			if (colliderMesh != null)
			{
				Object.Destroy(colliderMesh);
				colliderMesh = null;
			}
			if (component != null)
			{
				Object.Destroy(component);
			}
			EdgeCollider2D[] array3 = components;
			foreach (EdgeCollider2D obj2 in array3)
			{
				Object.Destroy(obj2);
			}
			return;
		}
		if (flag)
		{
			EdgeCollider2D[] array4 = components;
			foreach (EdgeCollider2D obj3 in array4)
			{
				Object.Destroy(obj3);
			}
		}
		else
		{
			if (colliderMesh != null)
			{
				Object.Destroy(colliderMesh);
			}
			if (component != null)
			{
				Object.Destroy(component);
			}
		}
		if (flag)
		{
			BuildPhysicsMesh3D(component, flattenDepth, num2, num);
		}
		else
		{
			BuildPhysicsMesh2D(components, num3);
		}
	}

	private void BuildPhysicsMesh2D(EdgeCollider2D[] edgeColliders, int numEdgeColliders)
	{
		for (int i = numEdgeColliders; i < edgeColliders.Length; i++)
		{
			Object.Destroy(edgeColliders[i]);
		}
		Vector2[] array = new Vector2[5];
		if (numEdgeColliders > edgeColliders.Length)
		{
			EdgeCollider2D[] array2 = new EdgeCollider2D[numEdgeColliders];
			int num = Mathf.Min(numEdgeColliders, edgeColliders.Length);
			for (int j = 0; j < num; j++)
			{
				array2[j] = edgeColliders[j];
			}
			for (int k = num; k < numEdgeColliders; k++)
			{
				array2[k] = base.gameObject.AddComponent<EdgeCollider2D>();
			}
			edgeColliders = array2;
		}
		Matrix4x4 identity = Matrix4x4.identity;
		identity.m00 = _scale.x;
		identity.m11 = _scale.y;
		identity.m22 = _scale.z;
		int num2 = 0;
		tk2dBatchedSprite[] array3 = batchedSprites;
		foreach (tk2dBatchedSprite tk2dBatchedSprite2 in array3)
		{
			if (!tk2dBatchedSprite2.IsDrawn)
			{
				break;
			}
			tk2dSpriteDefinition spriteDefinition = tk2dBatchedSprite2.GetSpriteDefinition();
			bool flag = false;
			bool flag2 = false;
			Vector3 vector = Vector3.zero;
			Vector3 vector2 = Vector3.zero;
			switch (tk2dBatchedSprite2.type)
			{
			case tk2dBatchedSprite.Type.Sprite:
				if (spriteDefinition != null && spriteDefinition.colliderType == tk2dSpriteDefinition.ColliderType.Mesh)
				{
					flag = true;
				}
				if (spriteDefinition != null && spriteDefinition.colliderType == tk2dSpriteDefinition.ColliderType.Box)
				{
					flag2 = true;
					vector = spriteDefinition.colliderVertices[0];
					vector2 = spriteDefinition.colliderVertices[1];
				}
				break;
			case tk2dBatchedSprite.Type.TiledSprite:
			case tk2dBatchedSprite.Type.SlicedSprite:
			case tk2dBatchedSprite.Type.ClippedSprite:
				flag2 = tk2dBatchedSprite2.CheckFlag(tk2dBatchedSprite.Flags.Sprite_CreateBoxCollider);
				if (flag2)
				{
					vector = tk2dBatchedSprite2.CachedBoundsCenter;
					vector2 = tk2dBatchedSprite2.CachedBoundsExtents;
				}
				break;
			}
			Matrix4x4 matrix4x = identity * tk2dBatchedSprite2.relativeMatrix;
			if (flag)
			{
				tk2dCollider2DData[] edgeCollider2D = spriteDefinition.edgeCollider2D;
				foreach (tk2dCollider2DData tk2dCollider2DData2 in edgeCollider2D)
				{
					Vector2[] array4 = new Vector2[tk2dCollider2DData2.points.Length];
					for (int n = 0; n < tk2dCollider2DData2.points.Length; n++)
					{
						ref Vector2 reference = ref array4[n];
						reference = matrix4x.MultiplyPoint(tk2dCollider2DData2.points[n]);
					}
					edgeColliders[num2].points = array4;
				}
				tk2dCollider2DData[] polygonCollider2D = spriteDefinition.polygonCollider2D;
				foreach (tk2dCollider2DData tk2dCollider2DData3 in polygonCollider2D)
				{
					Vector2[] array5 = new Vector2[tk2dCollider2DData3.points.Length + 1];
					for (int num4 = 0; num4 < tk2dCollider2DData3.points.Length; num4++)
					{
						ref Vector2 reference2 = ref array5[num4];
						reference2 = matrix4x.MultiplyPoint(tk2dCollider2DData3.points[num4]);
					}
					ref Vector2 reference3 = ref array5[tk2dCollider2DData3.points.Length];
					reference3 = array5[0];
					edgeColliders[num2].points = array5;
				}
				num2++;
			}
			else if (flag2)
			{
				Vector3 vector3 = vector - vector2;
				Vector3 vector4 = vector + vector2;
				ref Vector2 reference4 = ref array[0];
				reference4 = matrix4x.MultiplyPoint(new Vector2(vector3.x, vector3.y));
				ref Vector2 reference5 = ref array[1];
				reference5 = matrix4x.MultiplyPoint(new Vector2(vector4.x, vector3.y));
				ref Vector2 reference6 = ref array[2];
				reference6 = matrix4x.MultiplyPoint(new Vector2(vector4.x, vector4.y));
				ref Vector2 reference7 = ref array[3];
				reference7 = matrix4x.MultiplyPoint(new Vector2(vector3.x, vector4.y));
				ref Vector2 reference8 = ref array[4];
				reference8 = array[0];
				edgeColliders[num2].points = array;
				num2++;
			}
		}
	}

	private void BuildPhysicsMesh3D(MeshCollider meshCollider, bool flattenDepth, int numVertices, int numIndices)
	{
		if (meshCollider == null)
		{
			meshCollider = base.gameObject.AddComponent<MeshCollider>();
		}
		if (colliderMesh == null)
		{
			colliderMesh = new Mesh();
			colliderMesh.hideFlags = HideFlags.DontSave;
		}
		else
		{
			colliderMesh.Clear();
		}
		int num = 0;
		Vector3[] array = new Vector3[numVertices];
		int num2 = 0;
		int[] array2 = new int[numIndices];
		Matrix4x4 identity = Matrix4x4.identity;
		identity.m00 = _scale.x;
		identity.m11 = _scale.y;
		identity.m22 = _scale.z;
		tk2dBatchedSprite[] array3 = batchedSprites;
		foreach (tk2dBatchedSprite tk2dBatchedSprite2 in array3)
		{
			if (!tk2dBatchedSprite2.IsDrawn)
			{
				break;
			}
			tk2dSpriteDefinition spriteDefinition = tk2dBatchedSprite2.GetSpriteDefinition();
			bool flag = false;
			bool flag2 = false;
			Vector3 origin = Vector3.zero;
			Vector3 extents = Vector3.zero;
			switch (tk2dBatchedSprite2.type)
			{
			case tk2dBatchedSprite.Type.Sprite:
				if (spriteDefinition != null && spriteDefinition.colliderType == tk2dSpriteDefinition.ColliderType.Mesh)
				{
					flag = true;
				}
				if (spriteDefinition != null && spriteDefinition.colliderType == tk2dSpriteDefinition.ColliderType.Box)
				{
					flag2 = true;
					origin = spriteDefinition.colliderVertices[0];
					extents = spriteDefinition.colliderVertices[1];
				}
				break;
			case tk2dBatchedSprite.Type.TiledSprite:
			case tk2dBatchedSprite.Type.SlicedSprite:
			case tk2dBatchedSprite.Type.ClippedSprite:
				flag2 = tk2dBatchedSprite2.CheckFlag(tk2dBatchedSprite.Flags.Sprite_CreateBoxCollider);
				if (flag2)
				{
					origin = tk2dBatchedSprite2.CachedBoundsCenter;
					extents = tk2dBatchedSprite2.CachedBoundsExtents;
				}
				break;
			}
			Matrix4x4 mat = identity * tk2dBatchedSprite2.relativeMatrix;
			if (flattenDepth)
			{
				mat.m23 = 0f;
			}
			if (flag)
			{
				tk2dSpriteGeomGen.SetSpriteDefinitionMeshData(array, array2, num, num2, num, spriteDefinition, mat, tk2dBatchedSprite2.baseScale);
				num += spriteDefinition.colliderVertices.Length;
				num2 += spriteDefinition.colliderIndicesFwd.Length;
			}
			else if (flag2)
			{
				tk2dSpriteGeomGen.SetBoxMeshData(array, array2, num, num2, num, origin, extents, mat, tk2dBatchedSprite2.baseScale);
				num += 8;
				num2 += 36;
			}
		}
		colliderMesh.vertices = array;
		colliderMesh.triangles = array2;
		meshCollider.sharedMesh = colliderMesh;
	}

	public bool UsesSpriteCollection(tk2dSpriteCollectionData spriteCollection)
	{
		return this.spriteCollection == spriteCollection;
	}

	public void ForceBuild()
	{
		Build();
	}
}
