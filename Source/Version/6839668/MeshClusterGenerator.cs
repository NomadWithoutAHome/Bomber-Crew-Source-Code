using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[ExecuteInEditMode]
public class MeshClusterGenerator : MonoBehaviour
{
	public enum Axis
	{
		x,
		y,
		z
	}

	public Axis m_sortingWorldAxis = Axis.y;

	public bool m_reverseSortOrder;
}
