using UnityEngine;

public class AATurretCluster : MonoBehaviour
{
	[SerializeField]
	private GameObject m_turretPrefab;

	[SerializeField]
	private int m_numToSpawn;

	[SerializeField]
	private int m_rows;

	[SerializeField]
	private float m_distance;

	public void Start()
	{
		for (int i = 0; i < m_numToSpawn; i++)
		{
			GameObject gameObject = Object.Instantiate(m_turretPrefab);
			gameObject.transform.parent = base.transform.parent;
			int num = i % m_rows;
			int num2 = i / m_rows;
			gameObject.btransform().position = base.gameObject.btransform().position + new Vector3d((float)num * m_distance, 0f, (float)num2 * m_distance);
		}
	}
}
