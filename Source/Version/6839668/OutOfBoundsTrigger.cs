using BomberCrewCommon;
using UnityEngine;

public class OutOfBoundsTrigger : MonoBehaviour
{
	[SerializeField]
	private MissionPlaceableObject m_placeable;

	[SerializeField]
	private GameObject[] m_fighterPrefab;

	private static int m_numTimesSpawned;

	private static float m_timeSinceSpawn;

	private static int m_numOutOfBounders;

	private int m_myBoundsIndex;

	private void Awake()
	{
		m_numTimesSpawned = 0;
		m_timeSinceSpawn = 0f;
		m_myBoundsIndex = m_numOutOfBounders;
		m_numOutOfBounders++;
	}

	private void Update()
	{
		if (m_timeSinceSpawn > 30f)
		{
			Vector3d position = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBigTransform().position;
			if (m_placeable.IsPointWithinAreaExcludeHeight(position) && !Singleton<FighterCoordinator>.Instance.AreAnyFightersEngaged())
			{
				SpawnFighters();
				m_timeSinceSpawn = 0f;
			}
		}
		if (m_myBoundsIndex == 0)
		{
			m_timeSinceSpawn += Time.deltaTime;
		}
	}

	public void SpawnFighters()
	{
		FighterWing fighterWing = new FighterWing();
		int num = 3;
		bool shouldHunt = true;
		Vector3d position = Singleton<BomberSpawn>.Instance.GetBomberSystems().GetBigTransform().position;
		Vector3d normalized = new Vector3d(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
		Vector3d vector3d = position + normalized * 2500.0;
		for (int i = 0; i < num; i++)
		{
			GameObject gameObject = m_fighterPrefab[Mathf.Min(m_numTimesSpawned, m_fighterPrefab.Length - 1)];
			GameObject gameObject2 = Object.Instantiate(gameObject);
			gameObject2.transform.parent = null;
			gameObject2.transform.localScale = gameObject.transform.localScale;
			gameObject2.btransform().position = vector3d + new Vector3d(i * 50, 0f, 0f);
			gameObject2.GetComponent<FighterPlane>().SetFromArea(fighterWing, shouldHunt);
		}
		m_numTimesSpawned++;
		Singleton<FighterCoordinator>.Instance.RegisterWing(fighterWing);
	}
}
