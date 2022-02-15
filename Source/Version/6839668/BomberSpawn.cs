using BomberCrewCommon;
using UnityEngine;

public class BomberSpawn : Singleton<BomberSpawn>
{
	[SerializeField]
	private float m_yOffset = 3.3f;

	private GameObject m_spawnedBomber;

	private BomberSystems m_systemsGetter;

	private bool m_isSpawned;

	private void Awake()
	{
		SpawnBomber();
	}

	private void OnDisable()
	{
		CloseBomber();
	}

	public void CloseBomber()
	{
		if (Singleton<SaveDataContainer>.Instance != null)
		{
		}
		if (m_spawnedBomber != null)
		{
			Object.Destroy(m_spawnedBomber);
			m_systemsGetter = null;
			m_spawnedBomber = null;
		}
	}

	public void SpawnBomber()
	{
		if (!(m_spawnedBomber == null))
		{
			return;
		}
		if (Singleton<MissionCoordinator>.Instance != null)
		{
			Singleton<MissionCoordinator>.Instance.SpawnMissionStart();
		}
		GameObject bomberPrefab = Singleton<GameFlow>.Instance.GetGameMode().GetBomberPrefab();
		if (bomberPrefab != null)
		{
			m_spawnedBomber = Object.Instantiate(bomberPrefab);
			m_spawnedBomber.transform.parent = base.transform;
			m_spawnedBomber.transform.localPosition = new Vector3(0f, m_yOffset, 0f);
			m_spawnedBomber.transform.localScale = bomberPrefab.transform.localScale;
			m_spawnedBomber.transform.localRotation = Quaternion.identity * Quaternion.AngleAxis(6f, Vector3.forward);
			MissionPlaceableObject objectByType = Singleton<MissionCoordinator>.Instance.GetObjectByType("MissionStart");
			if (objectByType != null)
			{
				m_spawnedBomber.btransform().position = objectByType.gameObject.btransform().position + new Vector3d(0f, m_yOffset, 0f);
				m_spawnedBomber.transform.localRotation = objectByType.transform.localRotation * Quaternion.AngleAxis(6f, Vector3.forward);
			}
			m_systemsGetter = m_spawnedBomber.GetComponent<BomberSystems>();
			m_systemsGetter.GetBomberState().GetPhysicsModel().transform.position = m_spawnedBomber.transform.position;
			m_systemsGetter.GetBomberState().GetPhysicsModel().transform.rotation = m_spawnedBomber.transform.rotation;
			m_isSpawned = true;
		}
	}

	public BomberSystems GetBomberSystems()
	{
		if (!m_isSpawned)
		{
			SpawnBomber();
		}
		return m_systemsGetter;
	}
}
