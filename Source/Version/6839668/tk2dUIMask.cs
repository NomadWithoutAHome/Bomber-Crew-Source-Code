using UnityEngine;

[AddComponentMenu("2D Toolkit/UI/Core/tk2dUIMask")]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
public class tk2dUIMask : MonoBehaviour
{
	public tk2dBaseSprite.Anchor anchor = tk2dBaseSprite.Anchor.MiddleCenter;

	public Vector2 size = new Vector2(1f, 1f);

	public float depth = 1f;

	public bool createBoxCollider = true;

	private MeshFilter _thisMeshFilter;

	private BoxCollider _thisBoxCollider;

	private static readonly Vector2[] uv = new Vector2[4]
	{
		new Vector2(0f, 0f),
		new Vector2(1f, 0f),
		new Vector2(0f, 1f),
		new Vector2(1f, 1f)
	};

	private static readonly int[] indices = new int[6] { 0, 3, 1, 2, 3, 0 };

	private MeshFilter ThisMeshFilter
	{
		get
		{
			if (_thisMeshFilter == null)
			{
				_thisMeshFilter = GetComponent<MeshFilter>();
			}
			return _thisMeshFilter;
		}
	}

	private BoxCollider ThisBoxCollider
	{
		get
		{
			if (_thisBoxCollider == null)
			{
				_thisBoxCollider = GetComponent<BoxCollider>();
			}
			return _thisBoxCollider;
		}
	}

	private void Awake()
	{
		Build();
	}

	private void OnDestroy()
	{
		if (ThisMeshFilter.sharedMesh != null)
		{
			Object.Destroy(ThisMeshFilter.sharedMesh);
		}
	}

	private Mesh FillMesh(Mesh mesh)
	{
		Vector3 vector = Vector3.zero;
		switch (anchor)
		{
		case tk2dBaseSprite.Anchor.UpperLeft:
			vector = new Vector3(0f, 0f - size.y, 0f);
			break;
		case tk2dBaseSprite.Anchor.UpperCenter:
			vector = new Vector3((0f - size.x) / 2f, 0f - size.y, 0f);
			break;
		case tk2dBaseSprite.Anchor.UpperRight:
			vector = new Vector3(0f - size.x, 0f - size.y, 0f);
			break;
		case tk2dBaseSprite.Anchor.MiddleLeft:
			vector = new Vector3(0f, (0f - size.y) / 2f, 0f);
			break;
		case tk2dBaseSprite.Anchor.MiddleCenter:
			vector = new Vector3((0f - size.x) / 2f, (0f - size.y) / 2f, 0f);
			break;
		case tk2dBaseSprite.Anchor.MiddleRight:
			vector = new Vector3(0f - size.x, (0f - size.y) / 2f, 0f);
			break;
		case tk2dBaseSprite.Anchor.LowerLeft:
			vector = new Vector3(0f, 0f, 0f);
			break;
		case tk2dBaseSprite.Anchor.LowerCenter:
			vector = new Vector3((0f - size.x) / 2f, 0f, 0f);
			break;
		case tk2dBaseSprite.Anchor.LowerRight:
			vector = new Vector3(0f - size.x, 0f, 0f);
			break;
		}
		Vector3[] array2 = (mesh.vertices = new Vector3[4]
		{
			vector + new Vector3(0f, 0f, 0f - depth),
			vector + new Vector3(size.x, 0f, 0f - depth),
			vector + new Vector3(0f, size.y, 0f - depth),
			vector + new Vector3(size.x, size.y, 0f - depth)
		});
		mesh.uv = uv;
		mesh.triangles = indices;
		Bounds bounds = default(Bounds);
		bounds.SetMinMax(vector, vector + new Vector3(size.x, size.y, 0f));
		mesh.bounds = bounds;
		return mesh;
	}

	private void OnDrawGizmosSelected()
	{
		Mesh sharedMesh = ThisMeshFilter.sharedMesh;
		if (sharedMesh != null)
		{
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Bounds bounds = sharedMesh.bounds;
			Gizmos.color = new Color32(56, 146, 227, 96);
			float num = (0f - depth) * 1.001f;
			Vector3 center = new Vector3(bounds.center.x, bounds.center.y, num * 0.5f);
			Vector3 vector = new Vector3(bounds.extents.x * 2f, bounds.extents.y * 2f, Mathf.Abs(num));
			Gizmos.DrawCube(center, vector);
			Gizmos.color = new Color32(22, 145, byte.MaxValue, byte.MaxValue);
			Gizmos.DrawWireCube(center, vector);
		}
	}

	public void Build()
	{
		if (ThisMeshFilter.sharedMesh == null)
		{
			Mesh mesh = new Mesh();
			mesh.MarkDynamic();
			mesh.hideFlags = HideFlags.DontSave;
			ThisMeshFilter.mesh = FillMesh(mesh);
		}
		else
		{
			FillMesh(ThisMeshFilter.sharedMesh);
		}
		if (createBoxCollider)
		{
			if (ThisBoxCollider == null)
			{
				_thisBoxCollider = base.gameObject.AddComponent<BoxCollider>();
			}
			Bounds bounds = ThisMeshFilter.sharedMesh.bounds;
			ThisBoxCollider.center = new Vector3(bounds.center.x, bounds.center.y, 0f - depth);
			ThisBoxCollider.size = new Vector3(bounds.size.x, bounds.size.y, 0.0002f);
		}
		else if (ThisBoxCollider != null)
		{
			Object.Destroy(ThisBoxCollider);
		}
	}

	public void ReshapeBounds(Vector3 dMin, Vector3 dMax)
	{
		Vector3 vector = new Vector3(size.x, size.y);
		Vector3 zero = Vector3.zero;
		switch (anchor)
		{
		case tk2dBaseSprite.Anchor.LowerLeft:
			zero.Set(0f, 0f, 0f);
			break;
		case tk2dBaseSprite.Anchor.LowerCenter:
			zero.Set(0.5f, 0f, 0f);
			break;
		case tk2dBaseSprite.Anchor.LowerRight:
			zero.Set(1f, 0f, 0f);
			break;
		case tk2dBaseSprite.Anchor.MiddleLeft:
			zero.Set(0f, 0.5f, 0f);
			break;
		case tk2dBaseSprite.Anchor.MiddleCenter:
			zero.Set(0.5f, 0.5f, 0f);
			break;
		case tk2dBaseSprite.Anchor.MiddleRight:
			zero.Set(1f, 0.5f, 0f);
			break;
		case tk2dBaseSprite.Anchor.UpperLeft:
			zero.Set(0f, 1f, 0f);
			break;
		case tk2dBaseSprite.Anchor.UpperCenter:
			zero.Set(0.5f, 1f, 0f);
			break;
		case tk2dBaseSprite.Anchor.UpperRight:
			zero.Set(1f, 1f, 0f);
			break;
		}
		zero = Vector3.Scale(zero, vector) * -1f;
		Vector3 vector2 = vector + dMax - dMin;
		Vector3 vector3 = new Vector3((!Mathf.Approximately(vector.x, 0f)) ? (zero.x * vector2.x / vector.x) : 0f, (!Mathf.Approximately(vector.y, 0f)) ? (zero.y * vector2.y / vector.y) : 0f);
		Vector3 position = zero + dMin - vector3;
		position.z = 0f;
		base.transform.position = base.transform.TransformPoint(position);
		size = new Vector2(vector2.x, vector2.y);
		Build();
	}
}
