using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

public class STL
{
	public static void ExportBinary(GameObject[] gameObjects, string filePath, float scale, float bloat)
	{
		GetMeshesAndMatrixes(gameObjects, out var meshes, out var matrices);
		ExportBinary(meshes, matrices, filePath, scale, bloat);
	}

	public static void ExportBinary(MeshFilter[] filters, string filePath)
	{
		GetMeshesAndMatrixes(filters, out var meshes, out var matrices);
		ExportBinary(meshes, matrices, filePath, 1f, 1f);
	}

	public static void ExportBinary(SkinnedMeshRenderer[] skins, string filePath)
	{
		GetMeshesAndMatrixes(skins, out var meshes, out var matrices);
		ExportBinary(meshes, matrices, filePath, 1f, 1f);
	}

	public static void ExportBinary(Mesh mesh, Matrix4x4 matrix, string filePath)
	{
		ExportBinary(new Mesh[1] { mesh }, new Matrix4x4[1] { matrix }, filePath, 1f, 1f);
	}

	public static void ExportBinary(Mesh[] meshes, Matrix4x4[] matrices, string filePath, float scale, float bloat)
	{
		if (meshes.Length != matrices.Length)
		{
			Debug.LogError("Mesh array length and matrix array length must match.");
			return;
		}
		try
		{
			using BinaryWriter binaryWriter = new BinaryWriter(File.Open(filePath, FileMode.Create));
			binaryWriter.Write(new char[80]);
			int num = 0;
			foreach (Mesh mesh in meshes)
			{
				for (int j = 0; j < mesh.subMeshCount; j++)
				{
					num += mesh.GetTriangles(j).Length;
				}
			}
			uint value = (uint)(num / 3);
			binaryWriter.Write(value);
			short value2 = 0;
			for (int k = 0; k < meshes.Length; k++)
			{
				Vector3[] vertices = meshes[k].vertices;
				Vector3[] normals = meshes[k].normals;
				for (int l = 0; l < vertices.Length; l++)
				{
					ref Vector3 reference = ref vertices[l];
					reference = matrices[k].MultiplyPoint(vertices[l]) * scale;
					vertices[l] += matrices[k].MultiplyVector(normals[l]) * bloat;
				}
				for (int m = 0; m < meshes[k].subMeshCount; m++)
				{
					int[] triangles = meshes[k].GetTriangles(m);
					for (int n = 0; n < triangles.Length; n += 3)
					{
						Vector3 vector = vertices[triangles[n + 1]] - vertices[triangles[n]];
						Vector3 vector2 = vertices[triangles[n + 2]] - vertices[triangles[n]];
						Vector3 vector3 = new Vector3(vector.y * vector2.z - vector.z * vector2.y, vector.z * vector2.x - vector.x * vector2.z, vector.x * vector2.y - vector.y * vector2.x);
						for (int num2 = 0; num2 < 3; num2++)
						{
							binaryWriter.Write(vector3[num2]);
						}
						for (int num2 = 0; num2 < 3; num2++)
						{
							binaryWriter.Write(vertices[triangles[n]][num2]);
						}
						for (int num2 = 0; num2 < 3; num2++)
						{
							binaryWriter.Write(vertices[triangles[n + 1]][num2]);
						}
						for (int num2 = 0; num2 < 3; num2++)
						{
							binaryWriter.Write(vertices[triangles[n + 2]][num2]);
						}
						binaryWriter.Write(value2);
					}
				}
			}
			binaryWriter.Close();
		}
		catch (Exception ex)
		{
			Debug.LogWarning("FAILED exporting STL object at : " + filePath + "\n" + ex);
		}
	}

	public static void ExportText(GameObject[] gameObjects, string filePath)
	{
		GetMeshesAndMatrixes(gameObjects, out var meshes, out var matrices);
		ExportText(meshes, matrices, filePath);
	}

	public static void ExportText(Mesh mesh, Matrix4x4 matrix, string filePath)
	{
		ExportBinary(new Mesh[1] { mesh }, new Matrix4x4[1] { matrix }, filePath, 1f, 1f);
	}

	public static void ExportText(Mesh[] meshes, Matrix4x4[] matrices, string filePath)
	{
		if (meshes.Length != matrices.Length)
		{
			Debug.LogError("Mesh array length and matrix array length must match.");
			return;
		}
		try
		{
			bool append = false;
			using StreamWriter streamWriter = new StreamWriter(filePath, append);
			streamWriter.WriteLine("Solid Unity Mesh");
			CultureInfo cultureInfo = CultureInfo.CreateSpecificCulture("en-US");
			for (int i = 0; i < meshes.Length; i++)
			{
				StringBuilder stringBuilder = new StringBuilder();
				Vector3[] vertices = meshes[i].vertices;
				for (int j = 0; j < vertices.Length; j++)
				{
					ref Vector3 reference = ref vertices[j];
					reference = matrices[i].MultiplyPoint(vertices[j]);
				}
				for (int k = 0; k < meshes[i].subMeshCount; k++)
				{
					int[] triangles = meshes[i].GetTriangles(k);
					for (int l = 0; l < triangles.Length; l += 3)
					{
						Vector3 vector = vertices[triangles[l + 1]] - vertices[triangles[l]];
						Vector3 vector2 = vertices[triangles[l + 2]] - vertices[triangles[l]];
						Vector3 vector3 = new Vector3(vector.y * vector2.z - vector.z * vector2.y, vector.z * vector2.x - vector.x * vector2.z, vector.x * vector2.y - vector.y * vector2.x);
						stringBuilder.AppendLine("facet normal " + vector3.x.ToString("e", cultureInfo) + " " + vector3.y.ToString("e", cultureInfo) + " " + vector3.z.ToString("e", cultureInfo));
						stringBuilder.AppendLine("outer loop");
						stringBuilder.AppendLine("vertex " + vertices[triangles[l]].x.ToString("e", cultureInfo) + " " + vertices[triangles[l]].y.ToString("e", cultureInfo) + " " + vertices[triangles[l]].z.ToString("e", cultureInfo));
						stringBuilder.AppendLine("vertex " + vertices[triangles[l + 1]].x.ToString("e", cultureInfo) + " " + vertices[triangles[l + 1]].y.ToString("e", cultureInfo) + " " + vertices[triangles[l + 1]].z.ToString("e", cultureInfo));
						stringBuilder.AppendLine("vertex " + vertices[triangles[l + 2]].x.ToString("e", cultureInfo) + " " + vertices[triangles[l + 2]].y.ToString("e", cultureInfo) + " " + vertices[triangles[l + 2]].z.ToString("e", cultureInfo));
						stringBuilder.AppendLine("endloop");
						stringBuilder.AppendLine("endfacet");
					}
				}
				streamWriter.Write(stringBuilder.ToString());
			}
			streamWriter.WriteLine("endsolid Unity Mesh");
			streamWriter.Close();
		}
		catch (Exception ex)
		{
			Debug.LogWarning("FAILED exporting wavefront obj at : " + filePath + "\n" + ex);
		}
	}

	public static void GetMeshesAndMatrixes(GameObject[] objects, out Mesh[] meshes, out Matrix4x4[] matrices)
	{
		List<Mesh> list = new List<Mesh>();
		List<Matrix4x4> list2 = new List<Matrix4x4>();
		for (int i = 0; i < objects.Length; i++)
		{
			MeshFilter[] components = objects[i].GetComponents<MeshFilter>();
			for (int j = 0; j < components.Length; j++)
			{
				if (components[j] != null)
				{
					list.Add(components[j].sharedMesh);
					list2.Add(components[j].transform.localToWorldMatrix);
				}
			}
			SkinnedMeshRenderer[] components2 = objects[i].GetComponents<SkinnedMeshRenderer>();
			for (int k = 0; k < components2.Length; k++)
			{
				if (components2[k] != null)
				{
					Mesh mesh = new Mesh();
					components2[k].BakeMesh(mesh);
					list.Add(mesh);
					list2.Add(components2[k].transform.localToWorldMatrix);
				}
			}
		}
		meshes = list.ToArray();
		matrices = list2.ToArray();
	}

	public static void GetMeshesAndMatrixes(MeshFilter[] filters, out Mesh[] meshes, out Matrix4x4[] matrices)
	{
		List<Mesh> list = new List<Mesh>();
		List<Matrix4x4> list2 = new List<Matrix4x4>();
		for (int i = 0; i < filters.Length; i++)
		{
			if (filters[i] != null)
			{
				list.Add(filters[i].sharedMesh);
				list2.Add(filters[i].transform.localToWorldMatrix);
			}
		}
		meshes = list.ToArray();
		matrices = list2.ToArray();
	}

	public static void GetMeshesAndMatrixes(SkinnedMeshRenderer[] skins, out Mesh[] meshes, out Matrix4x4[] matrices)
	{
		List<Mesh> list = new List<Mesh>();
		List<Matrix4x4> list2 = new List<Matrix4x4>();
		for (int i = 0; i < skins.Length; i++)
		{
			if (skins[i] != null)
			{
				list.Add(skins[i].sharedMesh);
				list2.Add(skins[i].transform.localToWorldMatrix);
			}
		}
		meshes = list.ToArray();
		matrices = list2.ToArray();
	}

	private static string DateTimeCode()
	{
		return DateTime.Now.ToString("yy") + DateTime.Now.ToString("MM") + DateTime.Now.ToString("dd") + "_" + DateTime.Now.ToString("hh") + DateTime.Now.ToString("mm") + DateTime.Now.ToString("ss");
	}
}
