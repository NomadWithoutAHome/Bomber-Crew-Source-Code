using UnityEngine;

public class ReflectionSpriteFade : MonoBehaviour
{
	private void OnEnable()
	{
		SetAlpha();
	}

	public void SetAlpha()
	{
		Mesh sharedMesh = base.gameObject.GetComponent<MeshFilter>().sharedMesh;
		Vector3[] vertices = sharedMesh.vertices;
		Color[] colors = sharedMesh.colors;
		for (int i = 0; i < vertices.Length; i++)
		{
			float a = colors[i].a * (1f - vertices[i].y / (0f - sharedMesh.bounds.size.y));
			Color color = new Color(colors[i].r, colors[i].g, colors[i].b, a);
			colors[i] = color;
		}
		sharedMesh.colors = colors;
	}
}
