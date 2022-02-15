using UnityEngine;

public static class DisplayUtils
{
	private static readonly Vector3[] boxExtents = new Vector3[8]
	{
		new Vector3(-1f, -1f, -1f),
		new Vector3(1f, -1f, -1f),
		new Vector3(-1f, 1f, -1f),
		new Vector3(1f, 1f, -1f),
		new Vector3(-1f, -1f, 1f),
		new Vector3(1f, -1f, 1f),
		new Vector3(-1f, 1f, 1f),
		new Vector3(1f, 1f, 1f)
	};

	public static Bounds GetRendererBoundsOfGameObjectAndAllChildren(GameObject gameObject)
	{
		Bounds result = ((!(gameObject.GetComponent<Renderer>() != null)) ? new Bounds(gameObject.transform.position, Vector3.zero) : gameObject.GetComponent<Renderer>().bounds);
		Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in componentsInChildren)
		{
			if (renderer.bounds.size.magnitude != 0f)
			{
				result.Encapsulate(renderer.bounds);
			}
		}
		return result;
	}

	public static Bounds GetRendererBoundsInChildren(Transform t)
	{
		Vector3 vector = new Vector3(float.MinValue, float.MinValue, float.MinValue);
		Vector3 vector2 = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
		Vector3[] array = new Vector3[2] { vector2, vector };
		GetRendererBoundsInChildren(t.worldToLocalMatrix, array, t);
		if (array[0] != vector2 && array[1] != vector)
		{
			ref Vector3 reference = ref array[0];
			reference = Vector3.Min(array[0], Vector3.zero);
			ref Vector3 reference2 = ref array[1];
			reference2 = Vector3.Max(array[1], Vector3.zero);
			Bounds result = default(Bounds);
			result.min = array[0];
			result.max = array[1];
			return result;
		}
		return default(Bounds);
	}

	private static void GetRendererBoundsInChildren(Matrix4x4 rootWorldToLocal, Vector3[] minMax, Transform t)
	{
		MeshFilter component = t.GetComponent<MeshFilter>();
		if (component != null && component.sharedMesh != null)
		{
			Bounds bounds = component.sharedMesh.bounds;
			Matrix4x4 matrix4x = rootWorldToLocal * t.localToWorldMatrix;
			for (int i = 0; i < 8; i++)
			{
				Vector3 v = bounds.center + Vector3.Scale(bounds.extents, boxExtents[i]);
				Vector3 rhs = matrix4x.MultiplyPoint(v);
				ref Vector3 reference = ref minMax[0];
				reference = Vector3.Min(minMax[0], rhs);
				ref Vector3 reference2 = ref minMax[1];
				reference2 = Vector3.Max(minMax[1], rhs);
			}
		}
		int childCount = t.childCount;
		for (int j = 0; j < childCount; j++)
		{
			Transform child = t.GetChild(j);
			if (t.gameObject.activeSelf)
			{
				GetRendererBoundsInChildren(rootWorldToLocal, minMax, child);
			}
		}
	}

	public static Bounds GetMeshBoundsOfGameObjectAndAllChildren(GameObject gameObject)
	{
		Bounds result = ((!(gameObject.GetComponent<MeshFilter>() != null)) ? new Bounds(gameObject.transform.position, Vector3.zero) : gameObject.GetComponent<MeshFilter>().mesh.bounds);
		MeshFilter[] componentsInChildren = gameObject.GetComponentsInChildren<MeshFilter>(includeInactive: true);
		foreach (MeshFilter meshFilter in componentsInChildren)
		{
			result.Encapsulate(meshFilter.mesh.bounds);
		}
		return result;
	}

	public static void DisableRenderer(GameObject target)
	{
		DisableRenderer(target, includeAllChildren: true);
	}

	public static void DisableRenderer(GameObject target, bool includeAllChildren)
	{
		SetRendererState(target, includeAllChildren, state: false);
	}

	public static void EnableRenderer(GameObject target)
	{
		EnableRenderer(target, includeAllChildren: true);
	}

	public static void EnableRenderer(GameObject target, bool includeAllChildren)
	{
		SetRendererState(target, includeAllChildren, state: true);
	}

	public static void DisableCollider(GameObject target, bool includeAllChildren = true)
	{
		SetColliderState(target, includeAllChildren, state: false);
	}

	public static void EnsableCollider(GameObject target, bool includeAllChildren = true)
	{
		SetColliderState(target, includeAllChildren, state: true);
	}

	public static void DisableRendererAndCollider(GameObject target, bool includeAllChildren = true)
	{
		SetRendererState(target, includeAllChildren, state: false);
		SetColliderState(target, includeAllChildren, state: false);
	}

	public static void EnableRendererAndCollider(GameObject target, bool includeAllChildren = true)
	{
		SetRendererState(target, includeAllChildren, state: true);
		SetColliderState(target, includeAllChildren, state: true);
	}

	private static void SetRendererState(GameObject target, bool includeAllChildren, bool state)
	{
		if (target.GetComponent<Renderer>() != null)
		{
			target.GetComponent<Renderer>().enabled = state;
		}
		if (includeAllChildren)
		{
			Renderer[] componentsInChildren = target.GetComponentsInChildren<Renderer>();
			foreach (Renderer renderer in componentsInChildren)
			{
				renderer.enabled = state;
			}
		}
	}

	private static void SetColliderState(GameObject target, bool includeAllChildren, bool state)
	{
		if (target.GetComponent<Collider>() != null)
		{
			target.GetComponent<Collider>().enabled = state;
		}
		if (includeAllChildren)
		{
			Collider[] componentsInChildren = target.GetComponentsInChildren<Collider>();
			foreach (Collider collider in componentsInChildren)
			{
				collider.enabled = state;
			}
		}
	}

	public static void SetSortingLayerName(GameObject target, string sortingLayerName, bool includeAllChildren)
	{
		if (target.GetComponent<Renderer>() != null)
		{
			target.GetComponent<Renderer>().sortingLayerName = sortingLayerName;
		}
		if (includeAllChildren)
		{
			Renderer[] componentsInChildren = target.GetComponentsInChildren<Renderer>();
			foreach (Renderer renderer in componentsInChildren)
			{
				renderer.sortingLayerName = sortingLayerName;
			}
		}
	}

	public static void SetLayer(GameObject target, int layer, bool includeAllChildren)
	{
		target.layer = layer;
		if (includeAllChildren)
		{
			Transform[] componentsInChildren = target.GetComponentsInChildren<Transform>();
			foreach (Transform transform in componentsInChildren)
			{
				transform.gameObject.layer = layer;
			}
		}
	}
}
