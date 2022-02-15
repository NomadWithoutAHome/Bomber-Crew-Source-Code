using BomberCrewCommon;
using UnityEngine;

public class RaycastHelpers : Singleton<RaycastHelpers>
{
	[SerializeField]
	private LayerMask m_solidObjectLayerMask;

	public Vector3 FindWalkToPosition(Vector3 start, Vector3 end)
	{
		return end;
	}

	public Vector3 FindSafePosition(Vector3 source)
	{
		Vector3[] array = new Vector3[4]
		{
			new Vector3(1f, 1f),
			new Vector3(-1f, -1f),
			new Vector3(1f, -1f),
			new Vector3(-1f, 1f)
		};
		Vector3[] array2 = array;
		foreach (Vector3 vector in array2)
		{
			if (Physics2D.Raycast(source, vector, 10f, m_solidObjectLayerMask).collider == null)
			{
				return source + vector * 10f;
			}
		}
		return source;
	}
}
