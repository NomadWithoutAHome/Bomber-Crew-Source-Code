using UnityEngine;

public static class tk2dUtil
{
	private static string label = string.Empty;

	private static bool undoEnabled;

	public static bool UndoEnabled
	{
		get
		{
			return undoEnabled;
		}
		set
		{
			undoEnabled = value;
		}
	}

	public static void BeginGroup(string name)
	{
		undoEnabled = true;
		label = name;
	}

	public static void EndGroup()
	{
		label = string.Empty;
	}

	public static void DestroyImmediate(Object obj)
	{
		if (!(obj == null))
		{
			Object.DestroyImmediate(obj);
		}
	}

	public static GameObject CreateGameObject(string name)
	{
		return new GameObject(name);
	}

	public static Mesh CreateMesh()
	{
		Mesh mesh = new Mesh();
		mesh.MarkDynamic();
		return mesh;
	}

	public static T AddComponent<T>(GameObject go) where T : Component
	{
		return go.AddComponent<T>();
	}

	public static void SetActive(GameObject go, bool active)
	{
		if (active != go.activeSelf)
		{
			go.SetActive(active);
		}
	}

	public static void SetTransformParent(Transform t, Transform parent)
	{
		t.parent = parent;
	}
}
