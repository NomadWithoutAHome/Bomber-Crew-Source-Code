using System;
using UnityEngine;

namespace STLExamples;

public class BasicExample : MonoBehaviour
{
	private const int objectCount = 100;

	private GameObject[] objects;

	private void Start()
	{
		GenerateNewObjects();
	}

	public void GenerateNewObjects()
	{
		if (objects != null)
		{
			for (int i = 0; i < objects.Length; i++)
			{
				UnityEngine.Object.Destroy(objects[i]);
			}
		}
		objects = new GameObject[100];
		for (int j = 0; j < objects.Length; j++)
		{
			objects[j] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			objects[j].transform.parent = base.transform;
			objects[j].transform.localScale = Vector3.one * UnityEngine.Random.Range(0.1f, 1f);
			objects[j].transform.position = UnityEngine.Random.insideUnitSphere * 2f;
		}
	}

	public void ExportToBinarySTL()
	{
		string text = DefaultDirectory() + "/stl_example_binary.stl";
		STL.ExportBinary(objects, text, 1f, 1f);
		Debug.Log("Exported " + 100 + " objects to binary STL file." + Environment.NewLine + text);
	}

	public void ExportToTextSTL()
	{
		string text = DefaultDirectory() + "/stl_example_text.stl";
		STL.ExportText(objects, text);
		Debug.Log("Exported " + 100 + " objects to text based STL file." + Environment.NewLine + text);
	}

	private static string DefaultDirectory()
	{
		string empty = string.Empty;
		if (Application.platform == RuntimePlatform.OSXEditor)
		{
			return Environment.GetEnvironmentVariable("HOME") + "/Desktop";
		}
		return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
	}
}
