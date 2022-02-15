using UnityEngine;

public class DebugDamageTestTool : MonoBehaviour
{
	[SerializeField]
	private Camera m_mainCamera;

	[SerializeField]
	private LayerMask m_damageLayers;

	[SerializeField]
	private ProjectileType m_projectileType;

	[SerializeField]
	private GameObject m_flakPrefab;
}
