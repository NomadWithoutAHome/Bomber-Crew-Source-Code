using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class LowPolyWater : MonoBehaviour
{
	public enum GridType
	{
		Hexagonal,
		Square
	}

	public Material material;

	public Camera _camera;

	public int sizeX = 30;

	public int sizeZ = 30;

	public float waveScale = 1f;

	[Range(0f, 1f)]
	public float noise;

	public GridType gridType;

	private bool generate;

	private const int maxVerts = 65535;

	private const float sin60 = 0.8660254f;

	private const float inv_tan60 = 0.577350259f;

	[SerializeField]
	private string m_layerName = "Environment";

	[HideInInspector]
	public float scale = -1f;

	[HideInInspector]
	public int size = -1;

	private void Start()
	{
		if (!(material == null) && material.HasProperty("_EdgeBlend") && material.GetFloat("_EdgeBlend") > 0.1f)
		{
			SetupCamera();
		}
	}

	private void SetupCamera()
	{
		if (_camera == null)
		{
			_camera = Camera.main;
		}
		_camera.depthTextureMode |= DepthTextureMode.Depth;
	}

	private void OnEnable()
	{
		if (scale != -1f || size != -1)
		{
			sizeX = size;
			sizeZ = size;
			waveScale = scale;
			scale = -1f;
			size = -1;
		}
		Scale(material, waveScale);
		generate = true;
		Generate();
	}

	public static void Scale(Material material, float scale)
	{
		if (!(material == null) && material.HasProperty("__Scale"))
		{
			material.SetFloat("__Scale", scale);
			material.SetFloat("__RHeight", material.GetFloat("_RHeight") * scale);
			material.SetFloat("__RSpeed", material.GetFloat("_RSpeed") * scale);
			material.SetFloat("__Height", material.GetFloat("_Height") * scale);
			material.SetFloat("__Speed", material.GetFloat("_Speed") * scale);
			Texture texture = material.GetTexture("_NoiseTex");
			if (texture != null)
			{
				material.SetFloat("__TexSize", (float)texture.height * scale);
			}
		}
	}

	private void OnDestroy()
	{
		CleanUp();
	}

	private void CleanUp()
	{
		MeshFilter[] componentsInChildren = GetComponentsInChildren<MeshFilter>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (Application.isPlaying)
			{
				Object.Destroy(componentsInChildren[i].gameObject);
			}
			else
			{
				Object.DestroyImmediate(componentsInChildren[i].gameObject);
			}
		}
	}

	private void Generate()
	{
		if (material == null || !material.HasProperty("_EdgeBlend"))
		{
			return;
		}
		if (material.GetFloat("_EdgeBlend") > 0.1f)
		{
			SetupCamera();
		}
		Scale(material, waveScale);
		if (generate)
		{
			generate = false;
			if (gridType == GridType.Hexagonal)
			{
				GenerateHexagonal();
			}
			else
			{
				GenerateSquare();
			}
		}
	}

	private float Encode(Vector3 v)
	{
		float num = Mathf.Round((v.x + 5f) * 10000f);
		float num2 = Mathf.Round((v.z + 5f) * 10000f) / 100000f;
		return num + num2;
	}

	private void BakeMesh(List<Vector3> verts, List<int> inds, float rotation = 0f)
	{
		List<Vector2> list = new List<Vector2>(inds.Count);
		List<int> list2 = new List<int>(inds.Count);
		List<Vector3> list3 = new List<Vector3>(inds.Count);
		for (int i = 0; i < inds.Count; i += 3)
		{
			list2.Add(i % 65535);
			list2.Add((i + 1) % 65535);
			list2.Add((i + 2) % 65535);
			Vector3 vector = verts[inds[i]];
			Vector3 vector2 = verts[inds[i + 1]];
			Vector3 vector3 = verts[inds[i + 2]];
			list3.Add(vector);
			list3.Add(vector2);
			list3.Add(vector3);
			Vector2 item = default(Vector2);
			item.x = Encode(vector - vector2);
			item.y = Encode(vector - vector3);
			list.Add(item);
			item.x = Encode(vector2 - vector3);
			item.y = Encode(vector2 - vector);
			list.Add(item);
			item.x = Encode(vector3 - vector);
			item.y = Encode(vector3 - vector2);
			list.Add(item);
		}
		CleanUp();
		int num = Mathf.CeilToInt((float)list3.Count / 65535f);
		int num2 = 0;
		int num3 = 0;
		while (num2 < num)
		{
			GameObject gameObject = new GameObject("WaterChunk");
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.Euler(0f, rotation, 0f);
			gameObject.transform.localScale = Vector3.one;
			MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
			MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
			meshRenderer.sharedMaterial = material;
			meshRenderer.receiveShadows = false;
			meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
			Mesh mesh = new Mesh();
			mesh.name = "WaterChunk";
			int count = ((num2 != num - 1) ? 65535 : (list3.Count - num3));
			mesh.SetVertices(list3.GetRange(num3, count));
			mesh.SetTriangles(list2.GetRange(num3, count), 0);
			mesh.SetUVs(0, list.GetRange(num3, count));
			mesh.hideFlags = HideFlags.HideAndDontSave;
			meshFilter.mesh = mesh;
			gameObject.hideFlags = HideFlags.HideAndDontSave;
			gameObject.layer = LayerMask.NameToLayer(m_layerName);
			num2++;
			num3 += 65535;
		}
	}

	private void Add(List<Vector3> verts, Vector3 toAdd, float delta)
	{
		if (noise > 0f)
		{
			Vector2 vector = Random.insideUnitCircle * noise * delta / 2f;
			toAdd.x += vector.x;
			toAdd.z += vector.y;
		}
		verts.Add(toAdd);
	}

	private void GenerateSquare()
	{
		List<Vector3> verts = new List<Vector3>();
		List<int> list = new List<int>();
		int num = sizeX * 2;
		int num2 = sizeZ * 2;
		float num3 = 0.8660254f;
		Vector3 vector = Vector3.right * num3;
		Vector3 vector2 = new Vector3((float)(-sizeX) * 0.8660254f, 0f, (float)(-sizeZ) * 0.8660254f);
		for (int i = 0; i < num2 + 1; i++)
		{
			bool flag = i % 2 != 0;
			Vector3 toAdd = vector2 + Vector3.forward * i * num3;
			int num4 = num + ((!flag) ? 1 : 2);
			for (int j = 0; j < num4; j++)
			{
				Add(verts, toAdd, num3);
				if (flag && (j == 0 || j == num4 - 2))
				{
					toAdd += vector / 2f;
				}
				else
				{
					toAdd += vector;
				}
			}
		}
		int num5 = 0;
		for (int k = 0; k < num2; k++)
		{
			bool flag2 = k % 2 != 0;
			int num6 = num + ((!flag2) ? 1 : 2);
			int num7 = num + ((!flag2) ? 0 : 0);
			int num8 = num5 + num6;
			for (int l = 0; l < num7; l++)
			{
				int num9 = num5 + 1;
				int num10 = num8 + 1;
				list.Add(num5);
				if (flag2)
				{
					list.Add(num8);
					list.Add(num9);
					list.Add(num8);
					list.Add(num10);
					list.Add(num9);
				}
				else
				{
					list.Add(num10);
					list.Add(num9);
					list.Add(num5);
					list.Add(num8);
					list.Add(num10);
				}
				num5 = num9;
				num8 = num10;
			}
			list.Add(num5);
			if (flag2)
			{
				list.Add(num8);
				list.Add(num5 + 1);
				num5 += 2;
			}
			else
			{
				list.Add(num8);
				list.Add(num8 + 1);
				num5++;
			}
		}
		BakeMesh(verts, list);
	}

	private void GenerateHexagonal()
	{
		List<Vector3> list = new List<Vector3>();
		List<int> list2 = new List<int>();
		float num = size / size;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = sizeX + sizeZ + 1;
		int num6 = -sizeX;
		int num7 = sizeX;
		for (int i = num6; i <= num7; i++)
		{
			float num8 = 0.8660254f * num * (float)i;
			int num9 = num5 - Mathf.Abs(i);
			int num10 = -(sizeZ + sizeX) / 2;
			if (i < 0)
			{
				num10 += Mathf.Abs(i);
			}
			int num11 = num10 + num9 - 1;
			num3 += num9;
			for (int j = num10; j <= num11; j++)
			{
				float z = 0.577350259f * num8 + num * (float)j;
				Vector3 item = new Vector3(num8, 0f, z);
				if (noise > 0f)
				{
					Vector2 vector = Random.insideUnitCircle * noise * num / 2f;
					item.x += vector.x;
					item.z += vector.y;
				}
				list.Add(item);
				if (num2 < num3 - 1)
				{
					if (i >= num6 && i < num7)
					{
						int num12 = 0;
						if (i < 0)
						{
							num12 = 1;
						}
						list2.Add(num2);
						list2.Add(num2 + 1);
						list2.Add(num2 + num9 + num12);
					}
					if (i > num6 && i <= num7)
					{
						int num13 = 0;
						if (i > 0)
						{
							num13 = 1;
						}
						list2.Add(num2 + 1);
						list2.Add(num2);
						list2.Add(num2 - num4 + num13);
					}
				}
				num2++;
			}
			num4 = num9;
		}
		BakeMesh(list, list2);
	}
}
